using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class BossTargetingController : MonoBehaviour
{
    public Boss boss;
    
    [SerializeField] protected Unit target;
    [SerializeField] protected float commanderAccumulativeDamage;
    [SerializeField] protected float commanderAggroThreshold = 200;

    protected bool wasTargetStunned = false;

    protected CircleCollider2D rangeCollider;
    protected readonly List<Collider2D> collidersInRange = new();

    public Unit Target => target;


    protected virtual void Awake()
    {
        rangeCollider = GetComponent<CircleCollider2D>();
        rangeCollider.radius = GameplayUtils.ToWorldDistance(boss.stats.attackRange);
    }

    protected virtual void Update()
    {
        UpdateTarget();
    }


    public virtual void SetTarget(Unit newTarget)
    {
        target = newTarget;

        if (target == GameplayManager.instance.commander && newTarget != target)
        {
            commanderAccumulativeDamage = 0;
        }
    }

    public virtual void UpdateTarget()
    {
        if (wasTargetStunned) 
        {
            wasTargetStunned = target.state.IsStunned;
            return;
        }

        if (target.state.IsAlive && !target.state.IsStunned) return;

        List<Unit> candidates = GameplayManager.instance.allUnits.FindAll(unit => unit != target && unit.state.IsTargetable());

        if (candidates.Count == 0) return;
        
        int next = Random.Range(0, candidates.Count);
        SetTarget(candidates[next]);

        wasTargetStunned = candidates[next].state.IsStunned;
    }

    public virtual bool IsTargetInRange()
    {
        rangeCollider.Overlap(GameplayUtils.unitFilter, collidersInRange);

        foreach (Collider2D col in collidersInRange)
        {
            if (!col.TryGetComponent(out UnitStateController unitState)) continue;
            if (unitState.unit == target) return true;
        }
        
        return false;
    }

    public virtual void HandleCommanderAggro(float damage)
    {
        commanderAccumulativeDamage += damage;

        if (commanderAccumulativeDamage >= commanderAggroThreshold)
        {
            SetTarget(GameplayManager.instance.commander);
        }
    }
}
