using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitStats", menuName = "Scriptable Objects/UnitStats")]
public class UnitStats : ScriptableObject
{
    [Header("기본 스탯")]
    public int maxHP;
    public float moveSpeed;
    public float damageFactor;

    public int baseAttackRange;
    public int baseAttackDamage;
    public float baseAttackPeriod;

    [Header("부활 시간")]
    public List<float> reviveTimes = new(){ 10, 30, 45, 60, 120 };
    
}
