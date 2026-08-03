using UnityEngine;

[CreateAssetMenu(fileName = "BoarStats", menuName = "Scriptable Objects/BoarStats")]
public class BoarStats : ScriptableObject
{
    [Header("기본")]
    public int maxHp;
    public float criticalFactor;
    public float moveSpeed;
    public int bodyScale;
    public int headScale;

    [Header("박치기")]
    public int headbuttRangeRadius;
    public int headbuttRangeAngle;
    public int headbuttPredelay;
    public int headbuttPostdelay;
    public int headbuttStunDuration;
    public int headbuttKnockbackDistance;
    public int headbuttDamage;
    public int headbuttChainDamage;
    
    [Header("폭주 돌진")]
    public int rushRangeWidth;
    public int rushDistance;
    public int rushPredelay;
    public int rushPostdelay;
    public int rushAimingDuration;
    public int rushLastingDuration;
    public int rushGroggyDuration;
    public int rushStunDuration;
    public int rushKnockbackDistance;
    public int rushDamage;
    public int rushChainDamage;

    [Header("포효")]
    public int roarPredelay;
    public int roarPostdelay;
    public int roarLastingDuration;
    public int roarStunDuration;

    [Header("돌아들어가기")]
    public int roamPostdelay;
    public int roamDistance;
    public int roamDuration;
}