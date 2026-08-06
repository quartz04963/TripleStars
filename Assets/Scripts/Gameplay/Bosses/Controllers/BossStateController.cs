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

abstract public class BossStateController : EnemyStateController
{
    [SerializeField] protected BossState bossState;
    
    protected readonly List<int> patternBalls = new();
    
    public Boss Boss => (Boss)enemy;
    public BossState BossState {
        get => bossState;
        set => bossState = value;
    }
    

    public override void TakeDamage(float damage, Unit unit)
    {
        base.TakeDamage(damage, unit);

        if (unit == GameplayManager.instance.commander)
        {
            Boss.targeting.HandleCommanderAggro(damage);
        }
    }

    public virtual int GetNextSpecialPattern()
    {
        if (patternBalls.Count == 0) FillPatternBalls();

        int index = Random.Range(0, patternBalls.Count);
        int result = patternBalls[index];

        int last = patternBalls.Count - 1;
        patternBalls[index] = patternBalls[last];
        patternBalls.RemoveAt(last);

        return result;
    }

    abstract protected void FillPatternBalls();

    public virtual async Task Recover(float duration)
    {
        bossState = BossState.RECOVERY;
        // 추후 애니메이션 넣기

        await GameplayUtils.DelayForSeconds(duration);

        bossState = BossState.READY;
    }

    public virtual async Task Groggy(float groggyDuration, float standingDuration = 1f) // 그로기
    {
        bossState = BossState.GROGGY;
        // 추후 애니메이션 넣기

        await GameplayUtils.DelayForSeconds(groggyDuration);

        bossState = BossState.STANDING;
        // 추후 애니메이션 넣기

        await GameplayUtils.DelayForSeconds(standingDuration);

        bossState = BossState.READY;
    }
}
