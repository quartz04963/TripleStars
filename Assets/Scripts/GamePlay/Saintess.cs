using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class Saintess : Follower
{
    [Header("Saintess")]
    [SerializeField] private float passiveHealRange;
    [SerializeField] private float passiveHealAmount;
    [SerializeField] private float passiveHealPeriod;
    [SerializeField] private LayerMask unitLayer;

    private float lastHealTime;
    private ContactFilter2D unitSearchFilter;
    private readonly List<Collider2D> unitsInReach = new();

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
        InitStats("Saintess", 60, 1.6f, 1400, 7, 0.2f, 0.5f, FAST);
        InitSkills("Teleport", 30, Keyboard.current.digit1Key);

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
        // 효과: 0.5초마다 범위 내 아군의 hp를 1씩 회복
        
        if (Time.time < lastHealTime + passiveHealPeriod) return;

        lastHealTime = Time.time;

        // 추후 애니메이션 넣기

        unitsInReach.Clear();

        Physics2D.OverlapCircle(transform.position, passiveHealRange / GamePlayUtils.MAGNITUDE, unitSearchFilter, unitsInReach);

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
        // 스킬명: 텔레포트
        // 효과: 지휘관에게 텔레포트

        if (!skillInfo1.StartCooldown()) return;
        // 추후 애니메이션 넣기

        await Task.Delay(0);

        transform.position = GamePlayManager.instance.Commander.transform.position;
        StopMove();
    }

    protected override void UseSkill2() { }
}
