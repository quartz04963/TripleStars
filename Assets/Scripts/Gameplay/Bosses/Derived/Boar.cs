using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public enum PatternCode
{
    RUSH, // 돌진
    ROAR, // 포효
    ROAM, // 돌아들어가기
}
    
public class Boar : Boss
{
    [SerializeField] SectorRange headbuttSector;
    [SerializeField] Collider2D headCollider;
    [SerializeField] Collider2D rushCollider;

    private CancellationTokenSource rushCTS;
    private CancellationTokenSource roamCTS;  

    private readonly List<Collider2D> collisionResults = new();

    public BoarStats BoarStats => (BoarStats)stats;
    public BoarStateController BoarState => (BoarStateController)state;
    public BoarMovementController BoarMovement => (BoarMovementController)movement;

    public Collider2D HeadCollider => headCollider;
    public Collider2D RushCollider => rushCollider;
    
    public CancellationTokenSource RushCTS => rushCTS;
    public CancellationTokenSource RoamCTS => roamCTS;

    async void Start()
    {
        float headX = GameplayUtils.ToWorldDistance((BoarStats.headScale + BoarStats.bodyScale) / 2f);
        headbuttSector.Init(BoarStats.headbuttRangeRadius, BoarStats.headbuttRangeAngle);
        headbuttSector.transform.position = transform.position + new Vector3(headX, 0, 0);
        headbuttSector.gameObject.SetActive(false);

        float rushHeight = GameplayUtils.ToWorldDistance(BoarStats.headScale * 1.5f);
        float rushWidth = GameplayUtils.ToWorldDistance(BoarStats.rushRangeWidth);
        rushCollider.transform.localScale = new Vector3(rushHeight, rushWidth, 1);
        rushCollider.transform.position = transform.position + new Vector3(headX, 0, 0);
        rushCollider.gameObject.SetActive(false);

        await BossState.Recover(5f);
    }

    protected override async void DoNormalPattern()
    {
        await Headbutt();
    }

    protected override async void DoSpecialPattern(int patternCode)
    {
        switch ((PatternCode)patternCode)
        {
            case PatternCode.RUSH: await Rush(); break;
            case PatternCode.ROAR: await Roar(); break;
            case PatternCode.ROAM: await Roam(); break;
        }
    }

    #region 멧돼지 고유 패턴
    private async Task Headbutt() // 일반 패턴 - 박치기
    {
        Debug.Log("박치기");

        BossMovement.FaceTarget();

        headbuttSector.gameObject.SetActive(true);

        await GameplayUtils.DelayForSeconds(BoarStats.headbuttPredelay);

        // 피격 판정 처리
        headbuttSector.Collider.Overlap(GameplayUtils.unitFilter, collisionResults);
        
        foreach (Collider2D col in collisionResults)
        {
            if (!col.TryGetComponent(out UnitStateController unit)) continue;

            if (unit.TakeDamage(BoarStats.headbuttDamage))
            {
                Vector2 direction = transform.right; // 스프라이트가 오른쪽을 바라보고 있음 전제

                unit.Knockback(direction, BoarStats.headbuttKnockbackDistance, BoarStats.headbuttChainDamage);
                unit.Stun(BoarStats.headbuttStunDuration);
            }
        }

        headbuttSector.gameObject.SetActive(false);

        await BoarState.Recover(BoarStats.headbuttPostdelay, true);
    }

    private async Task Rush() // 특수 패턴 - 폭주 돌진
    {
        Debug.Log("폭주 돌진");

        await BoarMovement.RushAim(BoarStats.rushAimingDuration);

        rushCollider.gameObject.SetActive(true);

        await GameplayUtils.DelayForSeconds(BoarStats.rushPredelay - BoarStats.rushAimingDuration);

        BoarMovement.StartRush();
        rushCTS = new CancellationTokenSource();

        try
        {
            await GameplayUtils.DelayForSeconds(BoarStats.rushLastingDuration, rushCTS.Token);
        } 
        finally
        {
            BoarMovement.EndRush();
            rushCollider.gameObject.SetActive(false);
        }

        await BoarState.Recover(BoarStats.rushPostdelay, true);
    }

    private async Task Roar() // 특수 패턴 - 포효
    {
        Debug.Log("포효");

        await GameplayUtils.DelayForSeconds(BoarStats.roarPredelay);

        float time = 0;
        while (time < BoarStats.roarLastingDuration)
        {
            time += Time.deltaTime;

            foreach (Unit unit in GameplayManager.instance.allUnits)
            {
                unit.state.Stun(BoarStats.roarStunDuration);
            }

            await Task.Yield();
        }

        await BoarState.Recover(BoarStats.roarPostdelay, true);
    }

    private async Task Roam() // 특수 패턴 - 돌아들어가기
    {
        Debug.Log("돌아들어가기");

        BoarMovement.StartRoam();
        roamCTS = new CancellationTokenSource();

        try
        {
            await GameplayUtils.DelayForSeconds(999, roamCTS.Token);
        }
        catch (OperationCanceledException)
        {
            BoarMovement.EndRoam();
            await BoarState.Recover(BoarStats.roamPostdelay);
        }
    }

    #endregion
}
