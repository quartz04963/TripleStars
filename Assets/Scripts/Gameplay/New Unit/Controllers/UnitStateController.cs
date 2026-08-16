using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Animator), typeof(SpriteRenderer))]
public class UnitStateController : MonoBehaviour
{
    protected static readonly int IdleHash = Animator.StringToHash("Idle");
    protected static readonly int StunnedHash = Animator.StringToHash("Stunned");

    public Unit unit;

    [SerializeField] protected HpInfo hp;
    [SerializeField] protected GameObject effectorPrf;

    [Header("상태 변수")]
    [SerializeField] protected bool isAlive = true;
    [SerializeField] protected bool isImmune = false;
    [SerializeField] protected bool isPredelaying = false;
    [SerializeField] protected bool isStunned = false;
    [SerializeField] protected bool isKnockedBack = false;

    [Header("버프/디버프 수치 관련")]
    [SerializeField] protected float damageFactor = 1.0f;
    [SerializeField] protected float attackFactor = 1.0f;
    [SerializeField] protected float moveSpeedFactor = 1.0f;

    protected Collider2D hitCollider;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;

    public bool IsAlive => isAlive;
    public bool IsStunned => isStunned;
    public bool IsKnockedBack => isKnockedBack;
    public float AttackFactor => attackFactor;
    public float MoveSpeedFactor => moveSpeedFactor;

    protected virtual void Awake()
    {
        hitCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        damageFactor = unit.stats.damageFactor;
    }

    
    public virtual void SetHpInfo(HpInfo hpInfo)
    {
        hp = hpInfo;
        
        hpInfo.Init(unit.unitName, unit.stats.maxHP);
    }

    public virtual bool IsTargetable()
    {
        return isAlive;
    }

    public virtual bool IsHealable()
    {
        return isAlive;
    }

    public virtual bool TakeDamage(float damage, bool isDodgeable = true)
    {
        if (isImmune) return false;

        hp.AddHp(-damage * damageFactor);

        // Debug.Log(unit.unitName + " , " + damage);

        if (hp.CurrentHP <= 0) Die();

        return true;
    }

    protected virtual async void Die()
    {
        isAlive = false;
        
        animator.Play(StunnedHash);
        spriteRenderer.color = Color.gray;

        await Task.Yield(); // 넉백 판정을 위해 한 프레임 대기;

        if (++GameplayManager.instance.DeathCount >= GameplayManager.instance.gameData.lifeCount)
        {
            GameplayManager.instance.GameOver();
        }

        Respawn();
    }

    protected virtual async void Respawn()
    {
        float reviveTime = GameplayManager.instance.gameData.reviveTime_seconds;
        
        Immune(reviveTime);
        await GameplayUtils.DelayForSeconds(reviveTime);

        isAlive = true;

        animator.Play(IdleHash);
        spriteRenderer.color = Color.white;

        TakeHeal(unit.stats.maxHP);
    }

    public virtual void TakeHeal(int heal)
    {
        if (!IsHealable()) return;
        
        hp.AddHp(heal);

        for (int i = 0; i < 3; i++)
        {
            var effector = Instantiate(effectorPrf, transform).GetComponent<Effector>();
            float dx = Random.Range(-0.5f, 0.5f);
            float dy = Random.Range(-0.5f, 0.5f);
            effector.transform.localPosition = new Vector3(dx, dy, 0);
            effector.PlayEffect(60, "Heal", 1f);
        }
    }

    public virtual async void Immune(float duration)
    {
        isImmune = true;
        hitCollider.enabled = false;

        await GameplayUtils.DelayForSeconds(duration);

        hitCollider.enabled = true;
        isImmune = false;
    }

    public virtual async Task Predelay(float duration, CancellationToken token)
    {
        isPredelaying = true;

        try
        {
            await GameplayUtils.DelayForSeconds(duration, token);
        } 
        finally
        {
            isPredelaying = false;
        }
    }

    public virtual void AddAttackFactor(float delta)
    {
        attackFactor += delta;
    }

    public virtual void AddMoveSpeedFactor(float delta)
    {
        moveSpeedFactor += delta;
    }

    public virtual void ResetMoveSpeedFactor()
    {
        moveSpeedFactor = 1f;
    }

    public virtual bool CanMove()
    {
        if (GameplayManager.instance.IsPaused) return false;

        return isAlive && !(isStunned || isKnockedBack || isPredelaying);
    }

    public virtual bool CanAttack()
    {
        if (GameplayManager.instance.IsPaused) return false;
        
        return isAlive && !(isStunned || isKnockedBack || isPredelaying);
    }

    public virtual async void Stun(float duration)
    {
        if (isImmune) return;

        isStunned = true;
        animator.Play(StunnedHash);

        if (!isKnockedBack) unit.movement.StopMove();

        await GameplayUtils.DelayForSeconds(duration);

        if (isAlive) animator.Play(IdleHash);
        isStunned = false;
    }

    public virtual void Knockback(Vector2 direction, float distance, int chainDamage = 0)
    {
        if (isImmune) return;

        isKnockedBack = true;
        unit.movement.StartKnockback(direction, distance, chainDamage);
    }

    public virtual void EndKnockback()
    {
        isKnockedBack = false;
    }

    public virtual void FlipSprite(Vector2 direction)
    {
        if (direction.x < 0) spriteRenderer.transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x > 0) spriteRenderer.transform.localScale = new Vector3(-1, 1, 1);
    }

    public virtual void PlayAnimation(string name)
    {
        animator.Play(name, 0, 0f);
    }

}
