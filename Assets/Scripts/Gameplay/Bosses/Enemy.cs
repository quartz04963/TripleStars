using System.Collections.Generic;
using UnityEngine;

abstract public class Enemy : MonoBehaviour
{
    public string enemyName;

    public EnemyStats stats;
    public EnemyStateController state;
    public EnemyMovementController movement;

    public virtual void Init(HpInfo hp)
    {
        state.SetHpInfo(hp);
    }
}
