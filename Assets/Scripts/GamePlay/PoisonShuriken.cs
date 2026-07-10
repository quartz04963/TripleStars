using UnityEngine;

public class PoisonShuriken : Projectile
{
    protected override void Hit()
    {
        if (GameplayManager.instance.Attacker is Assassin assassin)
        {
            assassin.IncreasePoisonStack(target);
        }
        
        base.Hit();
    }
}
