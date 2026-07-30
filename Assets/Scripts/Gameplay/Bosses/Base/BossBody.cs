using UnityEngine;

public class BossBody : Enemy
{
    [Header("Boss Body")]
    [SerializeField] bool isWeakPoint;
    [SerializeField] Boss boss;

    public bool IsWeakPoint 
    {
        get => isWeakPoint;
        set => isWeakPoint = value;
    }
    public Boss Boss => boss;

    public override void TakeDamage(float damage)
    {
        if (isWeakPoint)
        {
            boss.TakeDamage(damage * boss.CriticalFactor);
        } 
        else
        {
            boss.TakeDamage(damage);
        }
    }
}
