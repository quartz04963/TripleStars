using UnityEngine;

[CreateAssetMenu(fileName = "VanguardStats", menuName = "Scriptable Objects/VanguardStats")]
public class VanguardStats : UnitStats
{
    [Header("힐")]
    public int healRange;
    public int healAmount;
    public float atkBuffRate;
    public float healPeriod;

    [Header("집결")]
    public float assembleCooldown;
}
