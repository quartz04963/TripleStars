using UnityEngine;

[CreateAssetMenu(fileName = "AssassinStats", menuName = "Scriptable Objects/AssassinStats")]
public class AssassinStats : UnitStats
{
    [Header("은신")]
    public float hideThreshold;
    public float hideDodgeRate;

    [Header("독 수리검")]
    public float poisonShurikenCooldown;
    public float poisonDmg;
    public float poisonPeriod;
    public int poisonMaxStack;
    
}
