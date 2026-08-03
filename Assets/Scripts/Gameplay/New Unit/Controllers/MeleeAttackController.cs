using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackController : UnitBaseAttackController
{
    [SerializeField] BoxCollider2D attackCollider;

    private float lastAttackTime;
    private readonly List<Collider2D> collidersInRange = new();


    protected override void Start()
    {
        base.Start();

        attackCollider.size = new Vector2(GameplayUtils.ToWorldDistance(unit.stats.baseAttackRange), 0.1f);
        attackCollider.offset = new Vector2(GameplayUtils.ToWorldDistance(unit.stats.baseAttackRange) / 2f, 0);
    }


    protected override void Attack()
    {
        if (Time.time < lastAttackTime + period) return;

        if (target == null) return;

        lastAttackTime = Time.time;

        if (target is BossBody bossBody)
        {
            TurnAttackCollider(bossBody.Boss.transform);

            BossBody weakpoint = GetHitWeakpoint();

            if (weakpoint == null) target.TakeDamage(damage * unit.state.AttackFactor, unit);
            else weakpoint.TakeDamage(damage * unit.state.AttackFactor, unit);
        }
        else
        {
            TurnAttackCollider(target.transform);

            target.TakeDamage(damage * unit.state.AttackFactor, unit);
        }
    }

    protected override void FindTarget()
    {
        rangeCollider.Overlap(GameplayUtils.enemyFilter, collidersInRange);

        bool isInRange = collidersInRange.Exists(col => col.GetComponent<Enemy>() == target);
        
        if (target == null || !isInRange)
        {
            target = GameplayUtils.FindNearest<Enemy>(transform, collidersInRange); 
        }
    }

    public void TurnAttackCollider(Transform target)
    {
        Vector2 direction = target.position - transform.position;

        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

        attackCollider.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public BossBody GetHitWeakpoint()
    {
        attackCollider.Overlap(GameplayUtils.enemyFilter, collidersInRange);

        foreach (Collider2D col in collidersInRange)
        {
            if (col.TryGetComponent(out BossBody bossBody) && bossBody.IsWeakPoint)
            {
                return bossBody;
            }
        }

        return null;
    }

    public Enemy PeakNextTarget()
    {
        rangeCollider.Overlap(GameplayUtils.enemyFilter, collidersInRange);

        bool isInRange = collidersInRange.Exists(col => col.GetComponent<Enemy>() == target);
        
        if (target == null || !isInRange)
        {
            return GameplayUtils.FindNearest<Enemy>(transform, collidersInRange); 
        }
        else
        {
            return target;
        }
    }
}
