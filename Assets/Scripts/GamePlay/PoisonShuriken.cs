using UnityEngine;

public class PoisonShuriken : Projectile
{
    protected override void Hit(Enemy enemy)
    {
        if (GameplayManager.instance.Attacker is Assassin assassin)
        {
            assassin.IncreasePoisonStack(enemy);
        }
        
        base.Hit(enemy);
    }
}
