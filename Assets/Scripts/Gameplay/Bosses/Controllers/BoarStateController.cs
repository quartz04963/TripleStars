using System.Threading.Tasks;
using UnityEngine;

public class BoarStateController : BossStateController
{
    [SerializeField] BossBody head;

    [SerializeField] int wallCrashCount = 0;
    
    public Boar Boar => (Boar)boss;
    public BoarStats BoarStats => (BoarStats)boss.stats;

    protected override void FillPatternBalls()
    {
        patternBalls.Clear();

        for (int i = 0; i < BoarStats.rushFrequency; i++) patternBalls.Add((int)PatternCode.RUSH);
        for (int i = 0; i < BoarStats.roarFrequency; i++) patternBalls.Add((int)PatternCode.ROAR);
        for (int i = 0; i < BoarStats.roamFrequency; i++) patternBalls.Add((int)PatternCode.ROAM);
    }

    public async Task Recover(float duration, bool isWeakened = false)
    {
        if (isWeakened) EnableWeakPoint(true);

        await base.Recover(duration);

        if (isWeakened) EnableWeakPoint(false);
    }

    public override async Task Groggy(float groggyDuration, float standingDuration = 1)
    {
        Debug.Log("그로기");

        bossState = BossState.GROGGY;
        animator.Play(GroggyHash);

        EnableWeakPoint(true);

        await GameplayUtils.DelayForSeconds(groggyDuration);

        bossState = BossState.STANDING;
        animator.Play(StandingHash);

        EnableWeakPoint(false);

        await GameplayUtils.DelayForSeconds(standingDuration);

        bossState = BossState.READY;
    }

    public void EnableWeakPoint(bool isEnabled)
    {
        if (wallCrashCount > Boar.Stats.weakpointExposureThreshold) return;

        head.IsWeakPoint = isEnabled;
    }

    public void IncreaseWallCrashCount()
    {
        if (++wallCrashCount >= BoarStats.weakpointExposureThreshold)
        {
            head.IsWeakPoint = true;
        }
    }
}
