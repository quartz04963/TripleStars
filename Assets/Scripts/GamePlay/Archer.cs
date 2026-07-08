using UnityEngine;
using UnityEngine.InputSystem;

public class Archer : Follower
{
    void Start()
    {
        Init();
        InitStats("Archer", 60, 2f, 2000, 200, 0.3f, 2f, FAST);
        InitSkills("Explosive Arrow", 15, Keyboard.current.eKey);

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

        moveButton = Mouse.current.leftButton;
    }

    protected override void HandlePassiveSkill()
    {
        // 스킬명: 도주
        // 효과: 어그로 끌렸을 때 이동속도 VERYFAST로 증가
        
        if (isTargeted)
        {
            moveSpeed = VERYFAST;
        }
        else
        {
            moveSpeed = FAST;
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

    protected override void UseSkill1()
    {
        // TODO: 스킬 구현

        // 스킬명: 폭탄 화살
        // 효과: 300 대미지 화살 발사

        skillInfo1.StartCooldown();
    }

    protected override void UseSkill2() { }
}
