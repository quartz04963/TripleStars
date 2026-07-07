using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

abstract public class Unit : MonoBehaviour
{
    public static readonly float MAGNITUDE = 100;

    public static readonly float SLOW = 240;
    public static readonly float NORMAL = 360;
    public static readonly float FAST = 480;
    public static readonly float VERYFAST = 600;

    [Header("유닛 정보")]
    [SerializeField] protected string unitName;
    [SerializeField] protected float hitRatio;
    [SerializeField] protected float attackReach;
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

    [SerializeField] protected GameObject baseAttackProjectilePrf;

    [Header("상태 변수")]
    [SerializeField] protected bool isImmune = false;
    [SerializeField] protected bool isTargeted = false;
    [SerializeField] protected bool isTargetable = true;
    [SerializeField] protected bool isMovable = true;
    [SerializeField] protected bool isAttackable = true;

    private ContactFilter2D enemySearchFilter;
    private readonly List<Collider2D> enemiesInReach = new(); 

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
        this.hitRatio = hitRatio;
        this.attackReach = attackReach;
        this.attackDamage = attackDamage;
        this.attackDelay = attackDelay;
        this.attackPeriod = attackPeriod;
        this.moveSpeed = moveSpeed;

        hpInfo.Init(unitName, maxHp);
    }

    public virtual void InitSkills(string skillName1, float cooldown1, KeyControl skillKey1, string skillName2 = null, float cooldown2 = 0, KeyControl skillKey2 = null)
    {
        skillInfo1.Init(skillName1, cooldown1, skillKey1);

        if (skillInfo2 != null)
        {
            skillInfo2.Init(skillName2, cooldown2, skillKey2);
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

    public virtual bool TakeDamage(float damage)
    {
        if (isImmune) return false;

        hpInfo.AddHp(damage * hitRatio);
        // 추후 애니메이션 넣기

        return true;
    }

    public virtual void TakeHeal(float heal)
    {
        hpInfo.AddHp(heal);
        // 추후 애니메이션 넣기
    }

    abstract protected void HandleMove();
    abstract protected void HandleAttack();
    abstract protected void HandlePassiveSkill();
    abstract protected void UseSkill1();
    abstract protected void UseSkill2();

    #region 유틸리티
    protected virtual int FindEnemiesInReach(float radius)
    {
        enemiesInReach.Clear();

        return Physics2D.OverlapCircle(transform.position, radius, enemySearchFilter, enemiesInReach);
    }

    protected virtual Enemy FindNearestEnemy(float radius)
    {
        FindEnemiesInReach(radius);

        Enemy nearest = null;
        float minSqrDist = float.MaxValue;

        foreach (Collider2D col in enemiesInReach)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy == null) continue;

            float sqrDist = (col.transform.position - transform.position).sqrMagnitude;

            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                nearest = enemy;
            }
        }

        return nearest;
    }
    #endregion
}
