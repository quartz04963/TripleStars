using UnityEngine;

[CreateAssetMenu(fileName = "BossStats", menuName = "Scriptable Objects/BossStats")]
public class BossStats : ScriptableObject
{
    [Header("보스 기본 스탯")]
    public int maxHP;
    public float moveSpeed;
    public float weakDamageFactor;
    public float attackRange;
}
