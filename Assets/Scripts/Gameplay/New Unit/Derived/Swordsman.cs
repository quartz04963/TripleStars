using System.Threading;
using UnityEngine.InputSystem.Controls;

public class Swordsman : Unit
{
    private SkillUseInfo roll;
    private SkillUseInfo flameSword;

    private CancellationTokenSource rollCTS;
    private CancellationTokenSource flameSwordCTS;

    public CancellationTokenSource RollCTS => rollCTS;
    public CancellationTokenSource FlameSwordSTS => flameSwordCTS;

    public SwordsmanStats Stats => (SwordsmanStats)stats;
    public SwordsmanStateController State => (SwordsmanStateController)state;
    public SwordsmanMovementController Movement => (SwordsmanMovementController)movement;
    public MeleeAttackController BaseAttack => (MeleeAttackController)baseAttack;


    void Update()
    {
        if (state.CanAttack())
        {
            if (roll.SkillKey.isPressed)
            {
                Roll();
            }

            if (flameSword.SkillKey.isPressed)
            {
                FlameSword();
            }
        }
    }

    public override void Init(HpInfo hp, ButtonControl mouseButton, SkillUseInfo skill1, KeyControl skill1Key, SkillUseInfo skill2, KeyControl skill2Key)
    {
        state.SetHpInfo(hp);

        roll = skill1;
        roll.Init("Roll", skill1Key, Stats.rollCooldown);

        flameSword = skill2;
        flameSword.Init("Flame Sword", skill2Key, Stats.flameSwordCooldown);
    }

    async void Roll()
    {
        // 스킬명: 구르기
        // 효과: 사용 시 1초 간 구름

        if (!roll.StartCooldown()) return; // 스킬 사용 시작과 동시에 쿨다운
        
        // 추후 애니메이션 넣기
        
        State.Roll();

        rollCTS = new CancellationTokenSource();

        try
        {
            await GameplayUtils.DelayForSeconds(Stats.rollDuration, rollCTS.Token);
        }
        catch {}
        finally
        {
            State.EndRoll();
        }
    }

    async void FlameSword()
    {
        // 스킬명: 화염 검
        // 효과: 전방에 검을 휘둘러 60 * 5 대미지
        if (baseAttack.Target == null) return;

        if (!flameSword.StartCooldown()) return;

        movement.StopMove();

        // 추후 애니메이션 넣기
        
        Boss target = baseAttack.Target;

        flameSwordCTS = new CancellationTokenSource();

        try
        {
            await state.Predelay(Stats.flameSwordPredelay, flameSwordCTS.Token);
        }
        catch {}

        if (target != BaseAttack.PeakNextTarget()) return;

        BaseAttack.TurnAttackCollider(target.transform);

        BossBody weakpoint = BaseAttack.GetHitWeakpoint();

        for (int i = 0; i < Stats.flameSwordHitNumber; i++) // 첫 타가 치명타로 적중 시 나머지 타수도 치명타
        {
            if (weakpoint != null) weakpoint.TakeDamage(Stats.flameSwordDmg * state.AttackFactor, this);
            else target.state.TakeDamage(Stats.flameSwordDmg * state.AttackFactor, this);

            await GameplayUtils.DelayForSeconds(Stats.flameSwordHitInterval); // 차징 종료 후 공격은 캔슬되지 않음
        }

    }
}
