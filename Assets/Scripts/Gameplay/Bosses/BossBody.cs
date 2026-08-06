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
        float finalDamage = damage * (isWeakPoint ? boss.stats.weakDamageFactor : 1f);

        boss.state.TakeDamage(finalDamage, unit);
    }
}
