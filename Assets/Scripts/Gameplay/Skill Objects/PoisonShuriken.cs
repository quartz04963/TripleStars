using UnityEngine;

public class PoisonShuriken : Projectile
{
    protected override void Hit(Enemy enemy)
    {
        if (caster is Assassin assassin && enemy is BossBody bossBody)
        {
            assassin.IncreasePoisonStack(bossBody.boss);
        }
        
        base.Hit(enemy);
    }
}
