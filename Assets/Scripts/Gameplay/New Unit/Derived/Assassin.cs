using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Assassin : Unit
{
    [Header("Assassin")]
    [SerializeField] GameObject posionShurikenPrf;

    private SkillUseInfo poisonShuriken;

    private float unmovedTime;
    private float lastPoisonTime;
    private readonly Dictionary<Boss, int> poisionStacks = new();

    public AssassinStats Stats => (AssassinStats)stats;
    public AssassinStateController State => (AssassinStateController)state;
    public MouseMovementController Movement => (MouseMovementController)movement;


    void Update()
    {
        UpdateHide();
        UpdatePoison();

        if (state.CanAttack())
        {
            if (poisonShuriken.SkillKey.isPressed)
            {
                PoisonShuriken();
            }
        }
    }


    public override void Init(HpInfo hp, ButtonControl mouseButton, SkillUseInfo skill1, KeyControl skill1Key, SkillUseInfo skill2, KeyControl skill2Key)
    {
        state.SetHpInfo(hp);

        Movement.SetButtonControl(mouseButton);

        poisonShuriken = skill1;
        poisonShuriken.Init("Poison Shuriken", skill1Key, Stats.poisonShurikenCooldown);
    }

    protected void UpdateHide()
    {
        // 스킬명: 은신
        // 효과: 3초 동안 가만히 있으면 보스 어그로 설정 대상에서 제외 및 논타겟 공격이 50% 확률로 빗나감
        
        unmovedTime = movement.IsMoving ? 0 : unmovedTime + Time.deltaTime;

        State.IsHiding = unmovedTime >= Stats.hideThreshold;
    }

    protected void PoisonShuriken()
    {
        // 스킬명: 독 수리검
        // 효과: 적중 시 초당 10 독 대미지, 최대 5중첩, 피격 시 중첩 초기화 

        if (baseAttack.Target == null) return;
        
        if (!poisonShuriken.StartCooldown()) return;
        // 추후 애니메이션 넣기
        
        Projectile projectile = Instantiate(posionShurikenPrf, transform).GetComponent<Projectile>();
        projectile.Init(0, this, baseAttack.Target.transform);
    }

    public void IncreasePoisonStack(Boss boss)
    {
        if (poisionStacks.TryGetValue(boss, out int stack))
        {
            if (stack < Stats.poisonMaxStack) poisionStacks[boss]++;
        }
        else
        {
            poisionStacks[boss] = 1;
        }
    }

    public void ClearPoisonStack()
    {
        foreach (var key in poisionStacks.Keys.ToList()) poisionStacks[key] = 0;
    }

    void UpdatePoison()
    {
        if (Time.time >= lastPoisonTime + Stats.poisonPeriod)
        {
            lastPoisonTime = Time.time;

            var deadKeys = poisionStacks.Keys.Where(enemy => enemy == null).ToList();
            foreach (var key in deadKeys) poisionStacks.Remove(key);

            foreach (Boss boss in poisionStacks.Keys)
            {
                boss.state.TakeDamage(Stats.poisonDmg * poisionStacks[boss] * state.AttackFactor, this);
            }
        }
    }
}

