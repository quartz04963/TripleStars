using UnityEngine;

abstract public class Boss : Enemy
{
    public BossTargetingController targeting;

    protected int patternCount;

    public BossStats BossStats => (BossStats)stats;
    public BossStateController BossState => (BossStateController)state;
    public BossMovementController BossMovement => (BossMovementController)movement;

    void Update()
    {
        if (BossState.BossState == global::BossState.READY && targeting.IsTargetInRange()) 
        {
            DoNextPattern(2, 1);
        }
    }

    protected virtual void DoNextPattern(int normal = 2, int special = 1)
    {
        BossState.BossState = global::BossState.ATTACKING;

        if (patternCount++ % (normal + special) < normal)
        {
            DoNormalPattern();
        }
        else
        {
            DoSpecialPattern(BossState.GetNextSpecialPattern());
        }
    }

    abstract protected void DoNormalPattern();

    abstract protected void DoSpecialPattern(int patternCode);

}
