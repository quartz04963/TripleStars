using UnityEngine;

[CreateAssetMenu(fileName = "BossStats", menuName = "Scriptable Objects/BossStats")]
public class BossStats : EnemyStats
{
    [Header("보스 기본 스탯")]
    public float weakDamageFactor;
    public float attackRange;
}
