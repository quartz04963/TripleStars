using UnityEngine;

[CreateAssetMenu(fileName = "ArcherStats", menuName = "Scriptable Objects/ArcherStats")]
public class ArcherStats : UnitStats
{
    [Header("강력한 한 방")]
    public float criticalAttackFactor;

    [Header("폭발 화살")]
    public float explosiveArrowCooldown;
    public float explosiveArrowDmg;
    
}
