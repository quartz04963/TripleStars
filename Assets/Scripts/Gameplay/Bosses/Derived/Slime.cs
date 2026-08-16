using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public enum SlimePattern
{
    WAVE,
    SHAKE,
    EXPLODE,
}

public class Slime : Boss
{
    [SerializeField] CircleCollider2D waveCollider;
    [SerializeField] GameObject jumpAreaPrf;
    [SerializeField] GameObject slowAreaPrf;
    [SerializeField] GameObject explosiveMucusPrf;

    private SlimeJumpArea JumpArea;
    private CancellationTokenSource explodeRecoveryCTS;

    private readonly List<Collider2D> collisionResults = new();

    public SlimeStats Stats => (SlimeStats)stats;
    public SlimeStateController State => (SlimeStateController)state;
    public SlimeMovementController Movement => (SlimeMovementController)movement;

    public CancellationTokenSource ExplodeRecoveryCTS => explodeRecoveryCTS;


    protected override void Start()
    {
        float radius = GameplayUtils.ToWorldDistance(Stats.waveRangeRadius);
        waveCollider.transform.localScale = new Vector3(2 * radius, 2 * radius, 1);
        waveCollider.gameObject.SetActive(false);

        JumpArea = Instantiate(jumpAreaPrf).GetComponent<SlimeJumpArea>();
        JumpArea.gameObject.SetActive(false);

        base.Start();
    }


    protected override async void DoNormalPattern()
    {
        await Jump();
    }

    protected override async void DoSpecialPattern(int patternCode)
    {
        switch ((SlimePattern)patternCode)
        {
            case SlimePattern.WAVE: await Wave(); break;
            case SlimePattern.SHAKE: await Shake(); break;
            case SlimePattern.EXPLODE: await Explode(); break;
        }
    }

    #region 슬라임 고유 패턴
    private async Task Jump() // 일반 패턴 - 3연속 뛰기
    {
        Debug.Log("3연속 뛰기");

        state.PlayAnimation("Jump");
        JumpArea.SetSizeAndPosition(Stats.firstJumpRangeRadius, targeting.Target.transform);
        await Movement.JumpTo(targeting.Target.transform.position, Stats.firstJumpPredelay);
        
        JumpArea.Hit(Stats.firstJumpDamage, 0);
        await GameplayUtils.DelayForSeconds(Stats.firstJumpPostdelay);

        state.PlayAnimation("Jump");
        JumpArea.SetSizeAndPosition(Stats.secondJumpRangeRadius, targeting.Target.transform);
        await Movement.JumpTo(targeting.Target.transform.position, Stats.secondJumpPredelay);
        
        JumpArea.Hit(Stats.secondJumpDamage, 0);
        await GameplayUtils.DelayForSeconds(Stats.secondJumpPostdelay);

        state.PlayAnimation("Jump");
        JumpArea.SetSizeAndPosition(Stats.lastJumpRangeRadius, targeting.Target.transform);
        await Movement.JumpTo(targeting.Target.transform.position, Stats.lastJumpPredelay);
        
        JumpArea.Hit(Stats.lastJumpDamage, Stats.lastJumpStunDuration);

        await State.Recover(Stats.lastJumpPostdelay, true);
    }

    private async Task Wave() // 특수 패턴 - 점액질 웨이브
    {
        Debug.Log("점액질 웨이브");

        waveCollider.gameObject.SetActive(true);

        state.PlayAnimation("Wave");

        await GameplayUtils.DelayForSeconds(Stats.wavePredelay);

        var effector = Instantiate(effectorPrf, transform).GetComponent<Effector>();
        effector.PlayEffect(2 * Stats.waveRangeRadius, "Blue Wave", 1.5f);

        // 피격 판정 처리
        Unit candidate = null;

        waveCollider.Overlap(GameplayUtils.unitFilter, collisionResults);

        foreach (Collider2D col in collisionResults)
        {
            if (!col.TryGetComponent(out UnitStateController unit)) continue;

            if (unit.TakeDamage(Stats.waveDamage))
            {
                State.SlowByWave(unit.unit, Stats.waveSlowDuration);

                if (unit.unit == GameplayManager.instance.attacker || unit.unit == GameplayManager.instance.supporter)
                {
                    if (candidate == null) candidate = unit.unit;
                    else if (UnityEngine.Random.Range(0f, 1f) < 0.5f) candidate = unit.unit;
                }
            }
        }

        if (candidate != null) targeting.SetTarget(candidate);

        waveCollider.gameObject.SetActive(false);

        await State.Recover(Stats.wavePostdelay, true);
    }

    private async Task Shake() // 특수 패턴 - 털어내기
    {
        Debug.Log("털어내기");

        state.PlayAnimation("Shake");

        var effector = Instantiate(effectorPrf, transform).GetComponent<Effector>();
        effector.PlayEffect(2000, "Blue Splash", 1f);

        await GameplayUtils.DelayForSeconds(Stats.shakePredelay);
        
        for (int i = 0; i < Stats.shakeSlowAreaNumber; i++)
        {
            float radius = UnityEngine.Random.Range(0f, Stats.shakeSpreadRadius);
            float angle = UnityEngine.Random.Range(0f, 360f);
            Vector3 pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * GameplayUtils.ToWorldDistance(radius);

            var slowArea = Instantiate(slowAreaPrf).GetComponent<SlimeSlowArea>();
            slowArea.Init(pos, Stats.shakeSlowAreaRadius);
        }
        
        await State.Recover(Stats.shakePostdelay);
    }

    private async Task Explode() // 특수 패턴 - 폭발성 점액
    {
        Debug.Log("폭발성 점액");

        state.PlayAnimation("Explode");

        await Movement.ExplodeAim(Stats.explodePredelay);

        Vector2 direction = Movement.Farthest.transform.position - transform.position;
        Vector3 pos = transform.position + 2 * (Vector3)direction.normalized;

        var mucus = Instantiate(explosiveMucusPrf, pos, transform.rotation).GetComponent<ExplosiveMucus>();
        mucus.Init(direction, this);

        explodeRecoveryCTS = new CancellationTokenSource();

        try
        {
            await State.Recover(Stats.explodePostdelay, explodeRecoveryCTS.Token);
        }
        catch (OperationCanceledException)
        {
            await State.Groggy(Stats.explodeGroggyDuration);
        }
    }

    #endregion
}
