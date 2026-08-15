using System.Threading.Tasks;
using UnityEngine;

public class BoarStateController : BossStateController
{
    [SerializeField] int wallCrashCount = 0;
    
    public BoarStats BoarStats => (BoarStats)boss.stats;

    protected override void FillPatternBalls()
    {
        patternBalls.Clear();

        for (int i = 0; i < BoarStats.rushChance; i++) patternBalls.Add((int)BoarPattern.RUSH);
        for (int i = 0; i < BoarStats.roarChance; i++) patternBalls.Add((int)BoarPattern.ROAR);
        for (int i = 0; i < BoarStats.roamChance; i++) patternBalls.Add((int)BoarPattern.ROAM);
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

    public override void EnableWeakPoint(bool isEnabled)
    {
        if (wallCrashCount > BoarStats.weakpointExposureThreshold) return;

        base.EnableWeakPoint(isEnabled);
    }

    public void IncreaseWallCrashCount()
    {
        if (++wallCrashCount >= BoarStats.weakpointExposureThreshold)
        {
            weakpoint.IsWeakPoint = true;
        }
    }
}
