using UnityEngine;

[CreateAssetMenu(fileName = "SwordsmanStats", menuName = "Scriptable Objects/SwordsmanStats")]
public class SwordsmanStats : UnitStats
{
    [Header("구르기")]
    public float rollCooldown;
    public float rollSpeed;
    public float rollDuration;

    [Header("화염 검")]
    public float flameSwordCooldown;
    public float flameSwordDmg;
    public float flameSwordPredelay;
    public float flameSwordHitInterval;
    public int flameSwordHitNumber;
}
