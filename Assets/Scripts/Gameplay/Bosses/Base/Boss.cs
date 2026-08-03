using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum BossState
{
    READY,
    ATTACKING, //패턴 실행 중
    RECOVERY, //후딜레이
    GROGGY,
    STANDING,
}

abstract public class Boss : Enemy
{
    [Header("Boss")]
    [SerializeField] protected float criticalFactor;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float attackRange;
    [SerializeField] protected BossState state;
    [SerializeField] protected Rigidbody2D rigidbody;

    [SerializeField] protected float commanderAccumulativeDamage;
    [SerializeField] protected float commanderAggroThreshold;

    [SerializeField] protected Unit target;
    
    protected int patternCount;
    protected List<int> patternBalls = new();

    public float CriticalFactor => criticalFactor;

    public override void TakeDamage(float damage, Unit unit)
    {
        base.TakeDamage(damage, unit);

        if (unit == GameplayManager.instance.commander)
        {
            commanderAccumulativeDamage += damage;

            if (commanderAccumulativeDamage >= commanderAggroThreshold)
            {
                Target(GameplayManager.instance.commander);
            }
        }
    }

    public virtual bool Target(Unit newTarget)
    {
        if (newTarget.state.IsTargetable()) 
        {
            if (target == GameplayManager.instance.commander && newTarget != target)
            {
                commanderAccumulativeDamage = 0;
            }

            target = newTarget;
        }

        return newTarget.state.IsTargetable();
    }

    public virtual void ChangeTarget()
    {
        List<Unit> candidates = GameplayManager.instance.allUnits.FindAll(unit => unit != target && unit.state.IsTargetable());

        if (candidates.Count == 0) return;
        
        int next = Random.Range(0, candidates.Count);
        Target(candidates[next]);
    }

    public virtual bool IsTargeting(Unit unit)
    {
        return unit == target;
    }


    protected virtual void Chase()
    {
        FaceTarget();

        Vector2 direction = (target.transform.position - transform.position).normalized;

        rigidbody.linearVelocity = direction * GameplayUtils.ToWorldDistance(moveSpeed);
    }

    #region 패턴 결정
    protected virtual void DoNextPattern(int normal = 2, int special = 1)
    {
        if (patternCount++ % (normal + special) < normal)
        {
            DoNormalPattern();
        }
        else
        {
            DoSpecialPattern(GetNextSpecialPattern());
        }
    }

    protected virtual int GetNextSpecialPattern()
    {
        if (patternBalls.Count == 0) FillPatternBalls();

        int index = Random.Range(0, patternBalls.Count);
        int result = patternBalls[index];

        int last = patternBalls.Count - 1;
        patternBalls[index] = patternBalls[last];
        patternBalls.RemoveAt(last);

        return result;
    }

    abstract protected void DoNormalPattern();
    abstract protected void DoSpecialPattern(int patternCode);
    abstract protected void FillPatternBalls();

    #endregion

    protected virtual async Task Recover(int frame)
    {
        state = BossState.RECOVERY;
        // 추후 애니메이션 넣기
        await GameplayUtils.DelayForFrames(frame);

        state = BossState.READY;
    }

    protected virtual async Task Groggy(int groggyFrame, int standingFrame = 60) // 그로기
    {
        state = BossState.GROGGY;
        // 추후 애니메이션 넣기

        await GameplayUtils.DelayForFrames(groggyFrame);

        state = BossState.STANDING;
        // 추후 애니메이션 넣기

        await GameplayUtils.DelayForFrames(standingFrame);

        state = BossState.READY;
    }

    protected virtual void FaceTarget()
    {
        if (target == null) return;

        Vector2 direction = (target.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
