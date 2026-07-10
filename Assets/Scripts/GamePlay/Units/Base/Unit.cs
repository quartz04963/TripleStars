using System.Collections.Generic;
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
    [SerializeField] protected HpInfo hpInfo;
    [SerializeField] protected SkillUseInfo skillInfo1;
    [SerializeField] protected SkillUseInfo skillInfo2;

    [Header("컴포넌트 및 기타 참조")]
    [SerializeField] protected Rigidbody2D rigidbody;
    [SerializeField] protected LayerMask enemyLayer;
    [SerializeField] protected SpriteRenderer attackRangeSR;
    [SerializeField] protected GameObject baseAttackProjectilePrf;

    [Header("상태 변수")]
    [SerializeField] protected bool isImmune = false;
    [SerializeField] protected bool isTargeted = false;
    [SerializeField] protected bool isTargetable = true;
    [SerializeField] protected bool isMovable = true;
    [SerializeField] protected bool isAttackable = true;
    [SerializeField] protected bool isHealable = true;
    [SerializeField] protected float attackFactor = 1.0f;

    private ContactFilter2D enemySearchFilter;
    private readonly List<Collider2D> enemiesInReach = new();

    public bool IsHealable => isHealable;

    protected virtual void Awake()
    {
        enemySearchFilter = new ContactFilter2D()
        {
            useLayerMask = true,
            layerMask = enemyLayer,
            useTriggers = true,
        };
    }
    
    public virtual void InitStats(string unitName, float maxHp, float hitRatio, float attackReach, float attackDamage, float attackDelay, float attackPeriod, float moveSpeed)
    {
        this.unitName = unitName;
        this.damageFactor = hitRatio;
        this.attackRange = attackReach;
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

    public virtual bool Target()
    {
        if (!isTargetable) return false;

        isTargeted = true;
        return true;
    }

    public virtual void Untarget()
    {
        isTargeted = false;
    }

    public virtual bool TakeDamage(float damage, bool isAvoidable = true)
    {
        if (isImmune) return false;

        hpInfo.AddHp(-1 * damage * damageFactor);
        // 추후 애니메이션 넣기

        if (hpInfo.Hp <= 0)
        {
            Die();
        }

        return true;
    }

    public virtual void TakeHeal(float heal)
    {
        hpInfo.AddHp(heal);
        // 추후 애니메이션 넣기
    }

    public virtual void Die()
    {
        // 추후 애니메이션 넣기

        Destroy(gameObject);
    }

    public virtual void Teleport(Vector3 destination)
    {
        transform.position = destination;
    }

    public virtual void ShowAttackReachArea(bool isActive)
    {
        float radius = 2 * attackRange / GamePlayUtils.MAGNITUDE;

        attackRangeSR.transform.localScale = new Vector3(radius, radius, 1);
        attackRangeSR.material.SetFloat("_Thickness", 5f / radius);
        attackRangeSR.gameObject.SetActive(isActive);
    }

    public virtual void AddAttackFactor(float delta)
    {
        attackFactor += delta;
    }

    abstract protected void HandleMove();
    abstract protected void HandleAttack();
    abstract protected void HandlePassiveSkill();
    abstract protected void HandleSkillUse();
    abstract protected void UseSkill1();
    abstract protected void UseSkill2();

    #region 유틸리티
    protected virtual int FindEnemiesInRange(float radius)
    {
        enemiesInReach.Clear();

        return Physics2D.OverlapCircle(transform.position, radius, enemySearchFilter, enemiesInReach);
    }

    protected virtual Enemy FindNearestEnemy(float radius)
    {
        FindEnemiesInRange(radius);

        Enemy nearest = null;
        float minSqrDist = float.MaxValue;

        foreach (Collider2D col in enemiesInReach)
        {
            if (!col.TryGetComponent<Enemy>(out var enemy)) continue;

            float sqrDist = (col.transform.position - transform.position).sqrMagnitude;

            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                nearest = enemy;
            }
        }

        return nearest;
    }

    protected virtual bool GetInRange(Transform transform, float radius)
    {
        return (this.transform.position - transform.position).sqrMagnitude <= radius * radius;
    }
    #endregion
}
