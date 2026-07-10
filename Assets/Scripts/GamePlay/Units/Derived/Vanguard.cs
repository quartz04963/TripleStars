using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class Vanguard : Follower
{
    [Header("Vanguard")]
    [SerializeField] float passiveHealRange;
    [SerializeField] float passiveHealAmount;
    [SerializeField] float passiveAtkBuffRate;
    [SerializeField] float passiveHealPeriod;
    [SerializeField] LayerMask unitLayer;

    private float lastHealTime;
    private ContactFilter2D unitSearchFilter;
    private readonly List<Collider2D> unitsInReach = new();
    private List<Unit> buffedUnitList = new();

    protected override void Awake()
    {
        base.Awake();

        unitSearchFilter = new ContactFilter2D()
        {
            useLayerMask = true,
            layerMask = unitLayer,
            useTriggers = true,
        };
    }

    void Start()
    {
        Init();
        InitStats("Vanguard", 70, 1.8f, 1800, 10, 0.3f, 1f, NORMAL);
        InitSkills("Assemble", 50, Keyboard.current.digit1Key);

        ShowAttackReachArea(true);
    }

    void Update()
    {
        GetDestination();
        HandleAttack();
        HandlePassiveSkill();
        HandleSkillUse();
    }

    void FixedUpdate()
    {
        HandleMove();
    }

    protected override void Init()
    {
        base.Init();

        moveButton = Mouse.current.rightButton;
    }

    protected override void HandlePassiveSkill()
    {
        // 스킬명: 힐
        // 효과 1: 범위 내 아군 공격력 10% 증가

        foreach (Unit unit in buffedUnitList)
        {
            unit.AddAttackFactor(-1 * passiveAtkBuffRate);
        }

        buffedUnitList.Clear();
        unitsInReach.Clear();

        Physics2D.OverlapCircle(transform.position, passiveHealRange / GamePlayUtils.MAGNITUDE, unitSearchFilter, unitsInReach);

        foreach (Collider2D col in unitsInReach)
        {
            if (!col.TryGetComponent<Unit>(out var unit)) continue;

            buffedUnitList.Add(unit);
            unit.AddAttackFactor(passiveAtkBuffRate);
        }

        // 효과 2: 1초마다 범위 내 아군의 hp를 1씩 회복

        if (Time.time < lastHealTime + passiveHealPeriod) return;

        lastHealTime = Time.time;

        // 추후 애니메이션(VFX) 넣기

        foreach (Collider2D col in unitsInReach)
        {
            Unit unit = col.GetComponent<Unit>();
            if (unit != null && unit.IsHealable)
            {
                unit.TakeHeal(passiveHealAmount);
            }
        }
    }

    protected override void HandleSkillUse()
    {
        if (isAttackable)
        {
            if (skillInfo1.KeyControl.isPressed)
            {
                UseSkill1();
            }
        }
    }

    protected override async void UseSkill1()
    {
        // 스킬명: 집결
        // 효과: 모든 아군(지휘관 & 어태커)을 본인에게 텔레포트

        if (!skillInfo1.StartCooldown()) return;
        // 추후 애니메이션 넣기

        await Task.Delay(0);

        GamePlayManager.instance.Commander.transform.position = transform.position;
        GamePlayManager.instance.Attacker.Teleport(transform.position);
    }

    protected override void UseSkill2() { }
}
