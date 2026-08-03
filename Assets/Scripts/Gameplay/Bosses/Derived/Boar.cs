using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Boar : Boss
{
    [Header("Boar")]
    [SerializeField] BoarStats stats;
    [SerializeField] BossBody head;
    [SerializeField] BossBody body;

    [SerializeField] GameObject headbuttRange;
    [SerializeField] SectorRange headbuttSector; 
    [SerializeField] Collider2D headbuttCollider;
    [SerializeField] GameObject rushRange;
    [SerializeField] Collider2D rushCollider;

    private bool isRushing = false;
    private int wallCrashCount = 0;
    private int wallCrashThreshold = 2;
    private CancellationTokenSource rushCTS;

    private bool isRoaming = false;
    private bool isClockwise;
    private bool isInRoamRange;
    public float rotationTime;
    private Vector3 roamCenter;  
    private CancellationTokenSource roamCTS;  

    private readonly List<RaycastHit2D> hitResults = new(); 
    private readonly List<Collider2D> collisionResults = new();

    enum PatternCode
    {
        RUSH, // 돌진
        ROAR, // 포효
        ROAM, // 돌아들어가기
    }


    async void Start()
    {
        Init();

        await Recover(300);
    }

    void Update()
    {
        if (target.state.IsStunned || !target.state.IsAlive)
        {
            ChangeTarget();
        }

        if (state == BossState.READY && GameplayUtils.IsInRange(transform, target, attackRange, unitFilter)) 
        {
            DoNextPattern(1, 1);
        }
    }

    void FixedUpdate()
    {
        if (state == BossState.READY)
        {
            if (!GameplayUtils.IsInRange(transform, target, attackRange, unitFilter))
            {
                Chase();
                return;
            }
        } 

        if (state == BossState.ATTACKING)
        {
            if (isRushing) 
            {
                UpdateRush();
                return;
            }
            if (isRoaming) 
            {
                UpdateRoam();
                return;
            }
        }
            
        rigidbody.linearVelocity = Vector2.zero;
    }

    void Init()
    {
        hpInfo.Init("Boar", stats.maxHp);

        criticalFactor = stats.criticalFactor;
        moveSpeed = stats.moveSpeed;

        float bodyScale = GameplayUtils.ToWorldDistance(stats.bodyScale);
        float headScale = GameplayUtils.ToWorldDistance(stats.headScale);

        body.gameObject.transform.localScale = new Vector3(bodyScale, bodyScale, 1);
        head.gameObject.transform.localScale = new Vector3(headScale, headScale, 1);
        head.gameObject.transform.position = transform.position + new Vector3((bodyScale + headScale) / 2f, 0, 0);

        headbuttSector.Init(stats.headbuttRangeRadius, stats.headbuttRangeAngle);
        headbuttRange.SetActive(false);

        rushRange.transform.localScale = new Vector3(1.5f, GameplayUtils.ToWorldDistance(stats.rushRangeWidth) / headScale, 1);
        rushRange.SetActive(false);
    }

    protected override async void DoNormalPattern()
    {
        state = BossState.ATTACKING;

        await Headbutt();
    }

    protected override async void DoSpecialPattern(int patternCode)
    {
        state = BossState.ATTACKING;

        switch ((PatternCode)patternCode)
        {
            case PatternCode.RUSH: await Rush(); break;
            case PatternCode.ROAR: await Roar(); break;
            case PatternCode.ROAM: await Roam(); break;
        }
    }

    protected override void FillPatternBalls()
    {
        int rush = 2, roar = 1, roam = 4; // 가중치

        patternBalls.Clear();

        for (int i = 0; i < rush; i++) patternBalls.Add((int)PatternCode.RUSH);
        for (int i = 0; i < roar; i++) patternBalls.Add((int)PatternCode.ROAR);
        for (int i = 0; i < roam; i++) patternBalls.Add((int)PatternCode.ROAM);
    }

    protected override async Task Groggy(int groggyFrame, int standingFrame = 60)
    {
        Debug.Log("그로기");
        state = BossState.GROGGY;
        EnableWeakPoint(true);
        // 추후 애니메이션 넣기

        await GameplayUtils.DelayForFrames(groggyFrame);

        state = BossState.STANDING;
        EnableWeakPoint(false);
        // 추후 애니메이션 넣기

        await GameplayUtils.DelayForFrames(standingFrame);
    }

    private async Task Recover(int frame, bool isWeakened = false)
    {
        if (isWeakened) EnableWeakPoint(true);

        await base.Recover(frame);

        if (isWeakened) EnableWeakPoint(false);
    }

    private void EnableWeakPoint(bool isEnabled)
    {
        if (wallCrashCount > wallCrashThreshold) return;

        // 추후 머리 스프라이트 변경

        head.IsWeakPoint = isEnabled;
    }

    #region 멧돼지 고유 패턴
    private async Task Headbutt() // 일반 패턴 - 박치기
    {
        Debug.Log("박치기");

        FaceTarget();

        headbuttRange.SetActive(true);

        await GameplayUtils.DelayForFrames(stats.headbuttPredelay);

        // 피격 판정 처리
        headbuttCollider.Overlap(unitFilter, collisionResults);

        foreach (Collider2D col in collisionResults)
        {
            Unit unit = col.GetComponentInParent<Unit>();
            if (unit == null) continue;
            
            if (unit.state.TakeDamage(stats.headbuttDamage))
            {
                Vector2 direction = transform.right; // 스프라이트가 오른쪽을 바라보고 있음 전제

                unit.state.Knockback(direction, stats.headbuttKnockbackDistance, stats.headbuttChainDamage);
                unit.state.Stun(stats.headbuttStunDuration);
            }
        }

        headbuttRange.SetActive(false);
        await Recover(stats.headbuttPostdelay, true);
    }

    private async Task Rush() // 특수 패턴 - 폭주 돌진
    {
        Debug.Log("폭주 돌진");

        float time = 0;
        while (time < GameplayUtils.ToSecondsFloat(stats.rushAimingDuration))
        {
            time += Time.deltaTime;

            FaceTarget();

            await Task.Yield();
        }

        rushRange.SetActive(true);

        await GameplayUtils.DelayForFrames(stats.rushPredelay - stats.rushAimingDuration);

        isRushing = true;

        Vector2 direction = transform.right; // 스프라이트가 오른쪽을 바라보고 있음 전제
        rigidbody.linearVelocity = direction * GameplayUtils.ToWorldDistance(stats.rushDistance) / GameplayUtils.ToSecondsFloat(stats.rushLastingDuration); // 등속도로 돌진

        rushCTS = new CancellationTokenSource();

        try
        {
            await GameplayUtils.DelayForFrames(stats.rushLastingDuration, rushCTS.Token);
        } 
        catch (OperationCanceledException)
        {
            rigidbody.linearVelocity = Vector2.zero;
            rushRange.SetActive(false);

            isRushing = false;
        }

        rigidbody.linearVelocity = Vector2.zero;
        rushRange.SetActive(false);

        isRushing = false;

        await Recover(stats.rushPostdelay, true);
    }

    async void UpdateRush()
    {
        Vector2 delta = rigidbody.linearVelocity * Time.fixedDeltaTime;

        int count = rushCollider.Cast(delta.normalized, unitFilter, hitResults, delta.magnitude);
        if (count > 0)
        {
            Unit nearest = GameplayUtils.FindNearest<Unit>(rushRange.transform, hitResults);

            if (nearest.state.TakeDamage(stats.rushDamage))
            {
                nearest.state.Knockback(delta, stats.rushKnockbackDistance, stats.rushChainDamage);
                nearest.state.Stun(stats.rushStunDuration);
            }

            rushCTS.Cancel();

            await Recover(stats.rushPostdelay, true);

            return;
        }

        count = head.GetComponent<Collider2D>().Cast(delta.normalized, wallFilter, hitResults, delta.magnitude);
        if (count > 0)
        {            
            wallCrashCount++;
            rushCTS.Cancel();

            await Groggy(stats.rushGroggyDuration);

            return;
        }
    }

    private async Task Roar() // 특수 패턴 - 포효
    {
        Debug.Log("포효");

        await GameplayUtils.DelayForFrames(stats.roarPredelay);

        float time = 0;
        while (time < GameplayUtils.ToSecondsFloat(stats.roarLastingDuration))
        {
            time += Time.deltaTime;

            foreach (Unit unit in GameplayManager.instance.allUnits)
            {
                unit.state.Stun(stats.roarStunDuration);
            }

            await Task.Yield();
        }

        await Recover(stats.roarPostdelay, true);
    }

    private async Task Roam() // 특수 패턴 - 돌아들어가기
    {
        Debug.Log("돌아들어가기");

        isRoaming = true;
        isInRoamRange = false;
        isClockwise = UnityEngine.Random.Range(0f, 1f) < 0.5f;

        roamCenter = target.transform.position;

        roamCTS = new CancellationTokenSource();
        try
        {
            await GameplayUtils.DelayForFrames(9999, roamCTS.Token);
        }
        catch (OperationCanceledException)
        {
            isRoaming = false;
            rigidbody.linearVelocity = Vector2.zero;

            await Recover(stats.roamPostdelay);
        }
    }

    void UpdateRoam()
    {
        Vector3 longitude = transform.position - roamCenter; 

        if (!isInRoamRange) // 먼저 적당한 거리까지 직선 이동
        {
            float epsilon = GameplayUtils.ToWorldDistance(moveSpeed) * Time.fixedDeltaTime;

            if (longitude.magnitude > GameplayUtils.ToWorldDistance(stats.roamDistance) + epsilon) 
            {
                rigidbody.linearVelocity = -longitude.normalized * GameplayUtils.ToWorldDistance(moveSpeed);
            }
            else if (longitude.magnitude < GameplayUtils.ToWorldDistance(stats.roamDistance) - epsilon)
            {
                rigidbody.linearVelocity = longitude.normalized * GameplayUtils.ToWorldDistance(moveSpeed);
            }
            else
            {
                isInRoamRange = true;
                rotationTime = 0;
            }
        }

        else
        {
            rotationTime += Time.deltaTime;

            float angle = Mathf.Atan2(longitude.y, longitude.x) * Mathf.Rad2Deg;
            float speed = GameplayUtils.ToWorldDistance(stats.roamDistance) * MathF.PI * 0.5f;  
            angle += isClockwise? -90 : 90;

            rigidbody.linearVelocity = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * speed;

            FaceTarget();

            if (rotationTime > 1f)
            {
                roamCTS.Cancel();
            }
        }
    }

    #endregion
}
