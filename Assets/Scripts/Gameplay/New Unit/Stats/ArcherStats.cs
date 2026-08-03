using UnityEngine;

[CreateAssetMenu(fileName = "ArcherStats", menuName = "Scriptable Objects/ArcherStats")]
public class ArcherStats : UnitStats
{
    [Header("도주")]
    public float flightMoveSpeed;

    [Header("폭발 화살")]
    public float explosiveArrowCooldown;
    public float explosiveArrowDmg;
    
}
