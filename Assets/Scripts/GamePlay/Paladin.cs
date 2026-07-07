using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class Paladin : Commander
{
    void Start()
    {
        InitStats("Paladin", 100, 0.8f, 600, 15, 0.3f, 1, 240);
        InitSkills("Defense", 5, Keyboard.current.spaceKey, "I'm your opponent", 30, Keyboard.current.qKey);
    }

    void Update()
    {
        GetDirection();
        HandleAttack();
        HandleSkillUse();
    }

    void FixedUpdate()
    {
        HandleMove();
    }

    // 테스트 용
    void HandleSkillUse()
    {
        if (isAttackable)
        {
            if (skillInfo1.KeyControl.isPressed)
            {
                UseSkill1();
            }
            if (skillInfo2.KeyControl.isPressed)
            {
                UseSkill2();
            }
        }
    }

    protected override void HandlePassiveSkill() { }

    protected override async void UseSkill1()
    {
        // 스킬명: 방어
        // 효과: 사용 시 1초 간 무적

        skillInfo1.StartCooldown();
        // 추후 애니메이션 넣기

        isImmune = true;
        await Task.Delay(1000);
        isImmune = false;
    }

    protected override void UseSkill2()
    {
        // TODO: 스킬 2 구현

        // 스킬명: 네 상대는 나다
        // 효과: 보스 어그로 끌기

        skillInfo2.StartCooldown();
    }
}
