using UnityEngine;

[CreateAssetMenu(fileName = "SaintessStats", menuName = "Scriptable Objects/SaintessStats")]
public class SaintessStats : UnitStats
{
    [Header("힐")]
    public int healRange;
    public int healAmount;
    public float healPeriod;

    [Header("텔레포트")]
    public float teleportCooldown;
}
