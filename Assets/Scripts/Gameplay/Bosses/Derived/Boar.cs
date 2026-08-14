using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public enum BoarPattern
{
    RUSH, // 돌진
    ROAR, // 포효
    ROAM, // 돌아들어가기
}
    
public class Boar : Boss
{
    [SerializeField] SectorRange headbuttSector;
    [SerializeField] BoxCollider2D headCollider;
    [SerializeField] BoxCollider2D bodyCollider;
    [SerializeField] Collider2D rushCollider;

    private CancellationTokenSource rushCTS;
    private CancellationTokenSource roamCTS;  

    private readonly List<Collider2D> collisionResults = new();

    public BoarStats Stats => (BoarStats)stats;
    public BoarStateController State => (BoarStateController)state;
    public BoarMovementController Movement => (BoarMovementController)movement;

    public Collider2D HeadCollider => headCollider;
    public Collider2D RushCollider => rushCollider;
    
    public CancellationTokenSource RushCTS => rushCTS;
    public CancellationTokenSource RoamCTS => roamCTS;

    protected override void Start()
    {
        headbuttSector.Init(Stats.headbuttRangeRadius, Stats.headbuttRangeAngle);
        headbuttSector.gameObject.SetActive(false);

        float rushWidth = GameplayUtils.ToWorldDistance(Stats.rushRangeWidth);
        rushCollider.transform.localScale = new Vector3(rushCollider.transform.localScale.x, rushWidth, 1);
        rushCollider.gameObject.SetActive(false);

        base.Start();
    }

    protected override async void DoNormalPattern()
    {
        await Headbutt();
    }

    protected override async void DoSpecialPattern(int patternCode)
    {
        switch ((BoarPattern)patternCode)
        {
            case BoarPattern.RUSH: await Rush(); break;
            case BoarPattern.ROAR: await Roar(); break;
            case BoarPattern.ROAM: await Roam(); break;
        }
    }

    #region 멧돼지 고유 패턴
    private async Task Headbutt() // 일반 패턴 - 박치기
    {
        Debug.Log("박치기");

        movement.FaceTarget();
        headbuttSector.gameObject.SetActive(true);
        state.PlayAnimation("Headbutt Charging");

        await GameplayUtils.DelayForSeconds(Stats.headbuttPredelay);

        state.PlayAnimation("Headbutt");

        // 피격 판정 처리
        headbuttSector.Collider.Overlap(GameplayUtils.unitFilter, collisionResults);
        
        foreach (Collider2D col in collisionResults)
        {
            if (!col.TryGetComponent(out UnitStateController unit)) continue;

            if (unit.TakeDamage(Stats.headbuttDamage))
            {
                Vector2 direction = transform.right; // 스프라이트가 오른쪽을 바라보고 있음 전제

                unit.Knockback(direction, Stats.headbuttKnockbackDistance, Stats.headbuttChainDamage);
                unit.Stun(Stats.headbuttStunDuration);
            }
        }

        headbuttSector.gameObject.SetActive(false);

        await State.Recover(Stats.headbuttPostdelay, true);
    }

    private async Task Rush() // 특수 패턴 - 폭주 돌진
    {
        Debug.Log("폭주 돌진");

        state.PlayAnimation("Rush Charging");

        await Movement.RushAim(Stats.rushAimingDuration);

        rushCollider.gameObject.SetActive(true);

        await GameplayUtils.DelayForSeconds(Stats.rushPredelay - Stats.rushAimingDuration);

        Movement.StartRush();
        state.PlayAnimation("Rush");
    
        rushCTS = new CancellationTokenSource();

        try
        {
            await GameplayUtils.DelayForSeconds(Stats.rushLastingDuration, rushCTS.Token);
        } 
        catch {}
        finally
        {
            Movement.EndRush();
            rushCollider.gameObject.SetActive(false);

            if (Movement.WasCrashedIntoWall)
            {
                await State.Groggy(Stats.rushGroggyDuration);
            }
            else 
            {
                await State.Recover(Stats.rushPostdelay, true);
            }
        }
    }

    private async Task Roar() // 특수 패턴 - 포효
    {
        Debug.Log("포효");

        await GameplayUtils.DelayForSeconds(Stats.roarPredelay);

        float time = 0;
        while (time < Stats.roarLastingDuration)
        {
            time += Time.deltaTime;

            foreach (Unit unit in GameplayManager.instance.allUnits)
            {
                unit.state.Stun(Stats.roarStunDuration);
            }

            await Task.Yield();
        }

        await State.Recover(Stats.roarPostdelay, true);
    }

    private async Task Roam() // 특수 패턴 - 돌아들어가기
    {
        Debug.Log("돌아들어가기");

        Movement.StartRoam();
        state.PlayAnimation("Move");

        roamCTS = new CancellationTokenSource();

        try
        {
            await GameplayUtils.DelayForSeconds(999, roamCTS.Token);
        }
        catch (OperationCanceledException)
        {
            Movement.EndRoam();
            await State.Recover(Stats.roamPostdelay);
        }
    }

    #endregion
}
