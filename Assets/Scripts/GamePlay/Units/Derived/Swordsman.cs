using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class Swordsman : Commander
{
    [Header("Swordsman")]
    [SerializeField] float rollingSpeed;
    [SerializeField] float rollingDuration;
    [SerializeField] float flameSwordDmg;
    [SerializeField] float flameSwordHitDelay;
    [SerializeField] float flameSwordHitInterval;
    [SerializeField] int flameSwordHitTime;

    private bool isRolling;
    private Vector2 lastMoveDirection = Vector2.up;
    private CancellationTokenSource cts1;
    private CancellationTokenSource cts2;

    void Start()
    {
        InitStats("Swordsman", 100, 1f, 80, 22, 0.3f, 1f, FAST);
        InitSkills("Roll", 2, Keyboard.current.spaceKey, "Flaming Sword", 15, Keyboard.current.qKey);

        ShowAttackRange(true);
    }

    protected override void GetDirection()
    {
        base.GetDirection();

        // 구르기 캔슬
        if (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.aKey.wasPressedThisFrame || 
            Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame) 
        {
            cts1?.Cancel();
            rigidbody.linearVelocity = Vector2.zero;
        }

        // 구르기 방향 저장
        if (moveDirection != Vector2.zero) lastMoveDirection = moveDirection;
    }

    protected override bool CanMove()
    {
        return !isRolling && base.CanMove();
    }

    public override void Stun(int frame)
    {
        if (!isImmune) cts2?.Cancel();

        base.Stun(frame);
    }

    public override void Knockback(Vector2 direction, float distance, float chainDamage = 0)
    {
        if (!isImmune) cts2?.Cancel();
        
        base.Knockback(direction, distance, chainDamage);
    }

    protected override void HandlePassiveSkill() { }

    protected override async void UseSkill1()
    {
        // 스킬명: 구르기
        // 효과: 사용 시 1초 간 구름

        if (!skillInfo1.StartCooldown()) return; // 스킬 사용 시작과 동시에 쿨다운
        // 추후 애니메이션 넣기
        
        isRolling = true;
        rigidbody.linearVelocity = lastMoveDirection * GameplayUtils.ToWorldDistance(rollingSpeed);

        cts1 = new CancellationTokenSource();

        try
        {
            await Task.Delay((int)(rollingDuration * 1000), cts1.Token);
        }
        catch (OperationCanceledException)
        {
            isRolling = false;
        }

        isRolling = false;
    }

    protected override async void UseSkill2()
    {
        // 스킬명: 화염 검
        // 효과: 전방에 검을 휘둘러 60 * 5 대미지

        if (!skillInfo2.StartCooldown()) return;
        // 추후 애니메이션 넣기

        isPredelaying = true;

        cts2 = new CancellationTokenSource();

        try
        {
            await Task.Delay((int)(flameSwordHitDelay * 1000), cts2.Token); // 추후 캔슬되게 수정 가능
        }
        catch (OperationCanceledException)
        {
            isPredelaying = false;
        }

        if (target != null && GameplayUtils.IsInRange(target.transform, transform, attackRange))
        {
            for (int i = 0; i < flameSwordHitTime; i++)
            {
                target.TakeDamage(flameSwordDmg * attackFactor);
                await Task.Delay((int)(flameSwordHitInterval * 1000));
            }
        }

        isPredelaying = false;
    }
}
