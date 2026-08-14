using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SlimeStateController : BossStateController
{
    private readonly List<Unit> unitsSlowedByWave = new();
    private readonly List<Unit> unitsOnSlowArea = new();

    public SlimeStats SlimeStats => (SlimeStats)boss.stats;
    

    private void Update()
    {
        UpdateSlow();
    }


    protected override void FillPatternBalls()
    {
        patternBalls.Clear();

        for (int i = 0; i < SlimeStats.waveFrequency; i++) patternBalls.Add((int)SlimePattern.WAVE);
        for (int i = 0; i < SlimeStats.shakeFrequency; i++) patternBalls.Add((int)SlimePattern.SHAKE);
        for (int i = 0; i < SlimeStats.explodeFrequency; i++) patternBalls.Add((int)SlimePattern.EXPLODE);
    }

    public override async Task Recover(float duration, bool isWeakened = false)
    {
        if (isWeakened) EnableWeakPoint(true);

        bossState = BossState.RECOVERY;
        if (isWeakened) animator.Play(GroggyHash);
        else animator.Play(StandingHash);

        await GameplayUtils.DelayForSeconds(duration);

        bossState = BossState.READY;
        animator.Play(MoveHash);

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

    #region 슬로우 관리
    private void UpdateSlow() // 버프/디버프를 거는 존재가 여럿이라면 유닛 쪽에서 관리하는 게 맞음
    {
        foreach (Unit unit in GameplayManager.instance.allUnits)
        {
            unit.state.ResetMoveSpeedFactor();
        }

        float maxSlowFactor = Mathf.Max(SlimeStats.waveSlowFactor, SlimeStats.shakeSlowFactor);

        foreach (Unit unit in unitsSlowedByWave)
        {
            if (unitsOnSlowArea.Contains(unit)) unit.state.AddMoveSpeedFactor(-maxSlowFactor);
            else unit.state.AddMoveSpeedFactor(-SlimeStats.waveSlowFactor);
        }

        foreach (var unit in unitsOnSlowArea)
        {
            if (unitsSlowedByWave.Contains(unit)) continue;
            else unit.state.AddMoveSpeedFactor(-SlimeStats.shakeSlowFactor);
        }
    }

    public async void SlowByWave(Unit unit, float duration)
    {
        if (!unitsSlowedByWave.Contains(unit)) unitsSlowedByWave.Add(unit);

        await GameplayUtils.DelayForSeconds(duration);

        unitsSlowedByWave.Remove(unit);
    }

    public void AddUnitOnSlowArea(Unit unit)
    {
        if (!unitsOnSlowArea.Contains(unit)) unitsOnSlowArea.Add(unit);
    }

    public void RemoveUnitOnSlowArea(Unit unit)
    {
        unitsOnSlowArea.Remove(unit);
    }

    #endregion
}
