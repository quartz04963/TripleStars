using System.Collections.Generic;
using UnityEngine;

public class RangedAttackController : UnitBaseAttackController
{
    [SerializeField] GameObject projectilePrf;

    private float lastAttackTime;
    private readonly List<Collider2D> collidersInRange = new(); 


    protected override void Attack()
    {
        if (Time.time < lastAttackTime + period) return;

        if (target == null) return;

        lastAttackTime = Time.time;

        Vector2 direction = target.transform.position - unit.transform.position;
        unit.state.FlipSprite(direction);
        unit.state.PlayAnimation("Attack");

        Projectile projectile = Instantiate(projectilePrf, transform).GetComponent<Projectile>();

        projectile.Init(damage * unit.state.AttackFactor, unit, target.transform);
    }

    protected override void FindTarget()
    {
        rangeCollider.Overlap(GameplayUtils.enemyFilter, collidersInRange);

        bool isInRange = collidersInRange.Exists(col => col.TryGetComponent(out BossBody body) && body.boss == target);
        
        if (target == null || !isInRange)
        {
            var nearest = GameplayUtils.FindNearest<BossBody>(transform, collidersInRange);
            target = nearest == null ? null : nearest.boss; 
        }
    }
}
