using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

abstract public class Unit : MonoBehaviour
{
    public static readonly float SLOW = 240;
    public static readonly float NORMAL = 360;
    public static readonly float FAST = 480;
    public static readonly float VERYFAST = 600;

    [Header("유닛 정보")]
    [SerializeField] protected string unitName;
    [SerializeField] protected float damageFactor;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float attackDamage;
    [SerializeField] protected float attackDelay;
    [SerializeField] protected float attackPeriod;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float attackFactor = 1.0f;
    [SerializeField] protected HpInfo hpInfo;
    [SerializeField] protected SkillUseInfo skillInfo1;
    [SerializeField] protected SkillUseInfo skillInfo2;

    [Header("컴포넌트 및 기타 참조")]
    [SerializeField] protected Rigidbody2D rigidbody;
    [SerializeField] protected Collider2D hitCollider;
    [SerializeField] protected Collider2D knockbackChainCollider;
    [SerializeField] protected SpriteRenderer characterSR;
    [SerializeField] protected SpriteRenderer attackRangeSR;
    [SerializeField] protected GameObject baseAttackProjectilePrf;

    [Header("스프라이트")]
    [SerializeField] protected Sprite standingSprite;
    [SerializeField] protected Sprite stunnedSprite;

    [Header("상태 변수")]
    [SerializeField] protected bool isAlive = true;
    [SerializeField] protected bool isImmune = false;
    [SerializeField] protected bool isHealable = true;
    [SerializeField] protected bool isTargetable = true;
    [SerializeField] protected bool isPredelaying = false;
    [SerializeField] protected bool isStunned = false;
    [SerializeField] protected bool isKnockbacked = false;

    [Header("기타 수치")]
    [SerializeField] protected float knockbackDecay = 3000;
    [SerializeField] protected float currentKnockbackChainDamage;


    protected ContactFilter2D unitFilter;
    protected ContactFilter2D wallFilter;
    protected ContactFilter2D enemyFilter;
    protected readonly List<RaycastHit2D> collidedUnits = new();
    protected readonly List<RaycastHit2D> collidedWalls = new();
    protected readonly List<Collider2D> enemiesInRange = new();

    public bool IsAlive => isAlive;
    public bool IsHealable => isHealable;
    public bool IsTargetable => isTargetable;
    public bool IsStunned => isStunned;

    protected virtual void Awake()
    {
        unitFilter = new ContactFilter2D()
        {
            useLayerMask = true,
            layerMask = LayerMask.GetMask("Unit"),
            useTriggers = true,
        };

        enemyFilter = new ContactFilter2D()
        {
            useLayerMask = true,
            layerMask = LayerMask.GetMask("Enemy"),
            useTriggers = true,
        };

        wallFilter = new ContactFilter2D()
        {
            useLayerMask = true,
            layerMask = LayerMask.GetMask("Wall"),
        };
    }

    protected virtual void Update()
    {
        HandleAttack();
        HandlePassiveSkill();
        HandleSkillUse();
    }

    protected virtual void FixedUpdate()
    {
        HandleMove();
        UpdateKnockback();
    }

    public virtual void SetInfos(HpInfo hpInfo, SkillUseInfo skillInfo1, SkillUseInfo skillInfo2)
    {
        this.hpInfo = hpInfo;
        this.skillInfo1 = skillInfo1;
        this.skillInfo2 = skillInfo2;
    }
    
    public virtual void InitStats(string unitName, float maxHp, float damageFactor, float attackRange, float attackDamage, float attackDelay, float attackPeriod, float moveSpeed)
    {
        this.unitName = unitName;
        this.damageFactor = damageFactor;
        this.attackRange = attackRange;
        this.attackDamage = attackDamage;
        this.attackDelay = attackDelay;
        this.attackPeriod = attackPeriod;
        this.moveSpeed = moveSpeed;

        hpInfo.Init(unitName, maxHp);
    }

    public virtual void InitSkills(string skillName1, float cooldown1, KeyControl skillKey1, string skillName2 = null, float cooldown2 = 0, KeyControl skillKey2 = null)
    {
        skillInfo1.Init(skillName1, cooldown1, skillKey1);

        if (skillName2 != null)
        {
            skillInfo2.Init(skillName2, cooldown2, skillKey2);
        }
        else if (skillInfo2 != null)
        {
            skillInfo2.gameObject.SetActive(false);
        }
    }

    public virtual void ShowAttackRange(bool isActive)
    {
        float diameter = 2 * GameplayUtils.ToWorldDistance(attackRange);

        attackRangeSR.transform.localScale = new Vector3(diameter, diameter, 1);
        attackRangeSR.material.SetFloat("_Thickness", 5f / diameter);
        attackRangeSR.gameObject.SetActive(isActive);
    }

    public virtual bool TakeDamage(float damage, bool isAvoidable = true)
    {
        if (isImmune) return false;

        hpInfo.AddHp(-damage * damageFactor);
        // 추후 애니메이션 넣기

        if (hpInfo.Hp <= 0) Die();
  
        return true;
    }
    
    protected virtual void Die()
    {
        isAlive = false;

        gameObject.SetActive(false);

        // 추후 애니메이션 넣기
    }

    public virtual void TakeHeal(float heal)
    {
        hpInfo.AddHp(heal);
        // 추후 애니메이션 넣기
    }

    public virtual async Task Immune(float duration)
    {
        isImmune = true;
        hitCollider.enabled = false;

        await Task.Delay((int)(duration * 1000));

        isImmune = false;
        hitCollider.enabled = true;
    }

    public virtual void AddAttackFactor(float delta)
    {
        attackFactor += delta;
    }

    protected virtual bool CanMove()
    {
        return !(isStunned || isKnockbacked || isPredelaying);
    }

    protected virtual bool CanAttack()
    {
        return !(isStunned || isKnockbacked || isPredelaying);
    }

    public virtual async void Stun(int frame)
    {
        if (isImmune) return;
        
        isStunned = true;
        characterSR.sprite = stunnedSprite;

        if (!isKnockbacked) StopMove();
        // TODO: 추후 애니메이션 넣기

        await GameplayUtils.DelayForFrames(frame);

        characterSR.sprite = standingSprite;
        isStunned = false;
    }

    public virtual void Knockback(Vector2 direction, float distance, float chainDamage = 0)
    {
        if (isImmune) return;
        
        float initialSpeed = GameplayUtils.ToWorldDistance(Mathf.Sqrt(2 * distance * knockbackDecay));
        rigidbody.linearVelocity = direction.normalized * initialSpeed;

        currentKnockbackChainDamage = chainDamage;

        isKnockbacked = true;
    }

    protected virtual void UpdateKnockback()
    {
        if (!isKnockbacked) return;

        rigidbody.linearVelocity = Vector2.MoveTowards(rigidbody.linearVelocity, Vector2.zero, GameplayUtils.ToWorldDistance(knockbackDecay) * Time.fixedDeltaTime);

        if (rigidbody.linearVelocity.sqrMagnitude < 0.01f)
        {
            StopMove();

            isKnockbacked = false;
            return;
        }

        if (currentKnockbackChainDamage == 0) return;

        // 유닛 충돌 검사
        Vector2 delta = rigidbody.linearVelocity * Time.fixedDeltaTime;

        int count = knockbackChainCollider.Cast(delta.normalized, unitFilter, collidedUnits, delta.magnitude);

        for (int i = count - 1; i >= 0; i--)
        {
            Unit unit = collidedUnits[i].transform.GetComponentInParent<Unit>();

            if (unit == null || unit.isKnockbacked) collidedUnits.RemoveAt(i); // 넉백된 유닛끼리는 충돌 판정 안 함
        }

        if (collidedUnits.Count > 0)
        {
            Unit nearest = GameplayUtils.FindNearest<Unit>(transform, collidedUnits);
            nearest.TakeDamage(currentKnockbackChainDamage);

            TakeDamage(currentKnockbackChainDamage);
            StopMove(); // 속도가 빠를 때 충돌하면 충돌 지점과 약간 거리가 벌어지는 문제가 있음

            return;
        }

        // 벽 충돌 검사
        count = rigidbody.Cast(delta.normalized, wallFilter, collidedWalls, delta.magnitude);
        if (count > 0)
        {
            TakeDamage(currentKnockbackChainDamage);
            StopMove(); // 마찬가지 문제
        }
    }

    public virtual void Teleport(Vector3 destination)
    {
        // 추후 애니메이션 넣기

        StopMove();

        transform.position = destination;
    }
    
    abstract protected void StopMove();
    abstract protected void HandleMove();
    abstract protected void HandleAttack();
    abstract protected void HandlePassiveSkill();

    protected virtual void HandleSkillUse()
    {
        if (!CanAttack()) return;

        if (skillInfo1.KeyControl.isPressed)
        {
            UseSkill1();
        }
        if (skillInfo2 != null && skillInfo2.gameObject.activeSelf && skillInfo2.KeyControl.isPressed)
        {
            UseSkill2();
        }
    }

    abstract protected void UseSkill1();
    abstract protected void UseSkill2();

}
