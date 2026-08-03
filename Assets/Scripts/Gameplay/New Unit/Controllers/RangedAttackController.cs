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

        Projectile projectile = Instantiate(projectilePrf, transform).GetComponent<Projectile>();
        
        if (target is BossBody bossBody)
        {
            projectile.Init(damage * unit.state.AttackFactor, unit, bossBody.Boss.transform);
        }
        else
        {
            projectile.Init(damage * unit.state.AttackFactor, unit, target.transform);
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
}
