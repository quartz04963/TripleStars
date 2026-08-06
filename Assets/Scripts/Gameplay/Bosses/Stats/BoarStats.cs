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
    
    [Header("폭주 돌진")]
    public int rushFrequency;
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

    [Header("포효")]
    public int roarFrequency;
    public float roarPredelay;
    public float roarPostdelay;
    public float roarLastingDuration;
    public float roarStunDuration;

    [Header("돌아들어가기")]
    public int roamFrequency;
    public int roamDistance;
    public float roamPostdelay;
    public float roamDuration;

    [Header("기타")]
    public int bodyScale;
    public int headScale;
    public int weakpointExposureThreshold;
}