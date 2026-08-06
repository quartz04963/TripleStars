using UnityEngine;

abstract public class Boss : MonoBehaviour
{
    public string bossName;

    public BossTargetingController targeting;

    protected int patternCount;

    public BossStats stats;
    public BossStateController state;
    public BossMovementController movement;
    

    void Update()
    {
        if (state.BossState == BossState.READY && targeting.IsTargetInRange()) 
        {
            DoNextPattern(2, 1);
        }
    }

    public virtual void Init(HpInfo hp)
    {
        state.SetHpInfo(hp);
    }

    protected virtual void DoNextPattern(int normal = 2, int special = 1)
    {
        state.BossState = BossState.ATTACKING;

        if (patternCount++ % (normal + special) < normal)
        {
            DoNormalPattern();
        }
        else
        {
            DoSpecialPattern(state.GetNextSpecialPattern());
        }
    }

    abstract protected void DoNormalPattern();

    abstract protected void DoSpecialPattern(int patternCode);

}
