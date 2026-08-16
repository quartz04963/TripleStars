using UnityEngine;

public class BossBody : Enemy
{
    public Boss boss;

    [SerializeField] bool isWeakPoint;
    
    public bool IsWeakPoint 
    {
        get => isWeakPoint;
        set => isWeakPoint = value;
    }
    
    public override void TakeDamage(float damage, Unit unit)
    {
        float finalDamage = damage;
        if (isWeakPoint)
        {
            finalDamage *= boss.stats.weakDamageFactor;
            
            if (unit is Archer archer) finalDamage *= archer.Stats.criticalAttackFactor;
        }

        boss.state.TakeDamage(finalDamage, unit);
    }
}
