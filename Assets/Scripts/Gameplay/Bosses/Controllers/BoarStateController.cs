using System.Threading.Tasks;
using UnityEngine;

public class BoarStateController : BossStateController
{
    [SerializeField] BossBody head;
    [SerializeField] BossBody body;

    [SerializeField] int wallCrashCount = 0;
    
    public Boar Boar => (Boar)boss;
    public BoarStats BoarStats => (BoarStats)boss.stats;

    protected virtual void Start()
    {
        float bodyScale = GameplayUtils.ToWorldDistance(BoarStats.bodyScale);
        float headScale = GameplayUtils.ToWorldDistance(BoarStats.headScale);

        body.gameObject.transform.localScale = new Vector3(bodyScale, bodyScale, 1);
        head.gameObject.transform.localScale = new Vector3(headScale, headScale, 1);
        head.gameObject.transform.position = transform.position + new Vector3((bodyScale + headScale) / 2f, 0, 0);
    }

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
        EnableWeakPoint(true);
        // 추후 애니메이션 넣기

        await GameplayUtils.DelayForSeconds(groggyDuration);

        bossState = BossState.STANDING;
        EnableWeakPoint(false);
        // 추후 애니메이션 넣기

        await GameplayUtils.DelayForSeconds(groggyDuration);
    }

    public void EnableWeakPoint(bool isEnabled)
    {
        if (wallCrashCount > Boar.Stats.weakpointExposureThreshold) return;

        // 추후 머리 스프라이트 변경;

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
