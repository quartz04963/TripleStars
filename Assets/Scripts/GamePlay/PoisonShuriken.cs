using UnityEngine;

public class PoisonShuriken : Projectile
{
    protected override void Hit(Enemy enemy)
    {
        if (caster is Assassin assassin)
        {
            assassin.IncreasePoisonStack(enemy);
        }
        
        base.Hit(enemy);
    }
}
