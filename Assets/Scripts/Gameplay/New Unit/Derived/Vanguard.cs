using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Vanguard : Unit
{
    [SerializeField] CircleCollider2D healCollider;

    private SkillUseInfo assemble;

    private float lastHealTime;
    private readonly List<Collider2D> collidersInReach = new();
    private readonly List<UnitStateController> buffedUnitList = new();

    public VanguardStats Stats => (VanguardStats)stats;
    public MouseMovementController Movement => (MouseMovementController)movement;


    void Start()
    {
        healCollider.radius = GameplayUtils.ToWorldDistance(Stats.healRange);
    }
    
    void Update()
    {
        if (state.IsAlive)
        {
            Heal();
        }

        if (state.CanAttack())
        {
            if (assemble.SkillKey.isPressed)
            {
                Assemble();
            }
        }
    }

    public override void Init(HpInfo hp, ButtonControl mouseButton, SkillUseInfo skill1, KeyControl skill1Key, SkillUseInfo skill2, KeyControl skill2Key)
    {
        state.SetHpInfo(hp);

        Movement.SetButtonControl(mouseButton);

        assemble = skill1;
        assemble.Init("Assemble", skill1Key, Stats.assembleCooldown);

        skill2.gameObject.SetActive(false);
    }

    void Heal()
    {
        // 스킬명: 힐
        // 효과 1: 범위 내 아군 공격력 10% 증가

        foreach (UnitStateController unit in buffedUnitList)
        {
            unit.AddAttackFactor(-Stats.atkBuffRate);
        }

        buffedUnitList.Clear();

        healCollider.Overlap(GameplayUtils.unitFilter, collidersInReach);

        foreach (Collider2D col in collidersInReach)
        {
            if (!col.TryGetComponent(out UnitStateController unit)) continue;

            buffedUnitList.Add(unit);
            unit.AddAttackFactor(Stats.atkBuffRate);
        }

        buffedUnitList.Add(state);
        state.AddAttackFactor(Stats.atkBuffRate);

        // 효과 2: 1초마다 범위 내 아군의 hp를 1씩 회복

        if (Time.time < lastHealTime + Stats.healPeriod) return;

        lastHealTime = Time.time;

        // 추후 애니메이션(VFX) 넣기

        foreach (Collider2D col in collidersInReach)
        {
            if (col.TryGetComponent(out UnitStateController unit))
            {
                unit.TakeHeal(Stats.healAmount);
            }
        }

        state.TakeHeal(Stats.healAmount);
    }

    void Assemble()
    {
        // 스킬명: 집결
        // 효과: 모든 아군(지휘관 & 어태커)을 본인에게 텔레포트

        if (!assemble.StartCooldown()) return;
        // 추후 애니메이션 넣기

        GameplayManager.instance.commander.movement.Teleport(transform.position);
        GameplayManager.instance.attacker.movement.Teleport(transform.position);
    }

}
