using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Saintess : Unit
{
    [SerializeField] CircleCollider2D healCollider;

    private SkillUseInfo teleport;

    private float lastHealTime;
    private readonly List<Collider2D> collidersInRange = new();

    public SaintessStats Stats => (SaintessStats)stats;
    public MouseMovementController Movement => (MouseMovementController)movement;


    void Start()
    {
        healCollider.radius = GameplayUtils.ToWorldDistance(Stats.healRange);
    }

    void Update()
    {
        Heal();

        if (state.CanAttack())
        {
            if (teleport.SkillKey.isPressed)
            {
                Teleport();
            }
        }
    }


    public override void Init(HpInfo hp, ButtonControl mouseButton, SkillUseInfo skill1, KeyControl skill1Key, SkillUseInfo skill2, KeyControl skill2Key)
    {
        state.SetHpInfo(hp);

        Movement.SetButtonControl(mouseButton);

        teleport = skill1;
        teleport.Init("Teleport", skill1Key, Stats.teleportCooldown);

        skill2.gameObject.SetActive(false);
    }

    void Heal()
    {
        // 스킬명: 힐
        // 효과: 0.5초마다 범위 내 아군의 hp를 1씩 회복
        
        if (Time.time < lastHealTime + Stats.healPeriod) return;

        lastHealTime = Time.time;

        // 추후 애니메이션(VFX) 넣기

        healCollider.Overlap(GameplayUtils.unitFilter, collidersInRange);

        foreach (Collider2D col in collidersInRange)
        {
            if (col.TryGetComponent(out UnitStateController unit))
            {
                unit.TakeHeal(Stats.healAmount);
            }
        }

        state.TakeHeal(Stats.healAmount);
    }

    void Teleport()
    {
        // 스킬명: 텔레포트
        // 효과: 지휘관에게 텔레포트

        if (!teleport.StartCooldown()) return;
        // 추후 애니메이션 넣기

        movement.Teleport(GameplayManager.instance.commander.transform.position);
    }
}