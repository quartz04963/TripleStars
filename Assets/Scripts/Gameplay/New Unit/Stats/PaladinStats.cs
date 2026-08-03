using UnityEngine;

[CreateAssetMenu(fileName = "PaladinStats", menuName = "Scriptable Objects/PaladinStats")]
public class PaladinStats : UnitStats
{
    [Header("방어")]
    public float shieldCooldown;
    public float shieldDuration;

    [Header("네 상대는 나다")]
    public float baitCooldown;
}
