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
    
}
