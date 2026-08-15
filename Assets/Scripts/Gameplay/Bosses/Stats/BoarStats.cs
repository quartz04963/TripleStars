using UnityEngine;

[CreateAssetMenu(fileName = "BoarStats", menuName = "Scriptable Objects/BoarStats")]
public class BoarStats : BossStats
{
    [Header("박치기")]
    public int headbuttRangeRadius;
    public int headbuttRangeAngle;
    public int headbuttKnockbackDistance;
    public int headbuttDamage;
    public int headbuttChainDamage;
    public float headbuttPredelay;
    public float headbuttPostdelay;
    public float headbuttStunDuration;
    public float headbuttChance;
    
    [Header("폭주 돌진")]
    public int rushRangeWidth;
    public int rushDistance;
    public int rushKnockbackDistance;
    public int rushDamage;
    public int rushChainDamage;
    public float rushPredelay;
    public float rushPostdelay;
    public float rushAimingDuration;
    public float rushLastingDuration;
    public float rushGroggyDuration;
    public float rushStunDuration;
    public float rushChance;

    [Header("포효")]
    public int roarRangeRadius;
    public float roarPredelay;
    public float roarPostdelay;
    public float roarLastingDuration;
    public float roarStunDuration;
    public float roarChance;

    [Header("돌아들어가기")]
    public int roamDistance;
    public float roamPostdelay;
    public float roamDuration;
    public float roamChance;

    [Header("기타")]
    public int weakpointExposureThreshold;
}