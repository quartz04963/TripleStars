using System.Collections.Generic;
using Unity.VisualScripting;
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

        unit.state.PlayAnimation("Attack");

        TurnAttackCollider(target.transform);

        BossBody weakpoint = GetHitWeakpoint();

        if (weakpoint != null) weakpoint.TakeDamage(damage * unit.state.AttackFactor, unit);
        else target.state.TakeDamage(damage * unit.state.AttackFactor, unit);
    }

    protected override void FindTarget()
    {
        rangeCollider.Overlap(GameplayUtils.enemyFilter, collidersInRange);

        bool isInRange = collidersInRange.Exists(col => col.TryGetComponent(out BossBody body) && body.boss == target);
        
        if (target == null || !isInRange)
        {
            var nearest = GameplayUtils.FindNearest<BossBody>(transform, collidersInRange);
            target = nearest != null ? nearest.boss : null; 
        }
    }

    public void TurnAttackCollider(Transform target)
    {
        Vector2 direction = target.position - unit.transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

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

    public Boss PeakNextTarget()
    {
        rangeCollider.Overlap(GameplayUtils.enemyFilter, collidersInRange);

        bool isInRange = collidersInRange.Exists(col => col.TryGetComponent(out BossBody body) && body.boss == target);
        
        if (target != null && isInRange) return target;

        var nearest = GameplayUtils.FindNearest<BossBody>(transform, collidersInRange);
        return nearest != null ? nearest.boss : null;

    }
}
