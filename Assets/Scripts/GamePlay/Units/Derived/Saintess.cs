using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class Saintess : Follower
{
    [Header("Saintess")]
    [SerializeField] float passiveHealRange;
    [SerializeField] float passiveHealAmount;
    [SerializeField] float passiveHealPeriod;

    private float lastHealTime;

    private readonly List<Collider2D> unitsInReach = new();

    void Start()
    {
        Init();
        InitStats("Saintess", 60, 1.6f, 700, 7, 0.2f, 0.5f, FAST);
        InitSkills("Teleport", 30, Keyboard.current.digit1Key);

        ShowAttackRange(true);
    }

    protected override void Init()
    {
        base.Init();

        moveButton = Mouse.current.rightButton;
    }

    protected override void HandlePassiveSkill()
    {
        // 스킬명: 힐
        // 효과: 0.5초마다 범위 내 아군의 hp를 1씩 회복
        
        if (Time.time < lastHealTime + passiveHealPeriod) return;

        lastHealTime = Time.time;

        // 추후 애니메이션(VFX) 넣기

        GameplayUtils.FindAllInRange(transform, passiveHealRange, unitFilter, unitsInReach);

        foreach (Collider2D col in unitsInReach)
        {
            if (col.TryGetComponent(out Unit unit) && unit.IsHealable)
            {
                unit.TakeHeal(passiveHealAmount);
            }
        }
    }

    protected override async void UseSkill1()
    {
        // 스킬명: 텔레포트
        // 효과: 지휘관에게 텔레포트

        if (!skillInfo1.StartCooldown()) return;
        // 추후 애니메이션 넣기

        await Task.Delay(0);

        Teleport(GameplayManager.instance.Commander.transform.position);
    }

    protected override void UseSkill2() { }
}
