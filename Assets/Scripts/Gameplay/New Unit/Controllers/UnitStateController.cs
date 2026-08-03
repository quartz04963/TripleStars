using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class UnitStateController : MonoBehaviour
{
    [SerializeField] protected Unit unit;

    [SerializeField] protected HpInfo hp;

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
    
    [Header("스프라이트 관련")]
    [SerializeField] protected Sprite standingSprite;
    [SerializeField] protected Sprite stunnedSprite;

    protected Collider2D hitCollider;
    protected SpriteRenderer spriteRenderer;

    public bool IsAlive => isAlive;
    public bool IsStunned => isStunned;
    public bool IsKnockedBack => isKnockedBack;
    public float AttackFactor => attackFactor;
    

    protected virtual void Awake()
    {
        hitCollider = GetComponent<Collider2D>();
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

    public virtual bool TakeDamage(float damage, bool isDodgeable = true)
    {
        if (isImmune) return false;

        hp.AddHp(-damage * damageFactor);

        if (hp.CurrentHP <= 0) Die();

        return true;
    }

    protected virtual void Die()
    {
        isAlive = false;

        // 추후 애니메이션 넣기

        gameObject.SetActive(false);
    }

    public virtual void TakeHeal(int heal)
    {
        hp.AddHp(heal);
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

    public virtual bool CanMove()
    {
        return !(isStunned || isKnockedBack || isPredelaying);
    }

    public virtual bool CanAttack()
    {
        return !(isStunned || isKnockedBack || isPredelaying);
    }

    public virtual async void Stun(float duration)
    {
        if (isImmune) return;

        isStunned = true;
        spriteRenderer.sprite = stunnedSprite;

        if (!isKnockedBack) unit.movement.StopMove();

        await GameplayUtils.DelayForSeconds(duration);

        spriteRenderer.sprite = standingSprite;
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

}
