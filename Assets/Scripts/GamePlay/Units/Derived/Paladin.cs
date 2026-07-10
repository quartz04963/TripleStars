using System.Threading.Tasks;
using UnityEngine.InputSystem;

public class Paladin : Commander
{
    void Start()
    {
        InitStats("Paladin", 100, 0.8f, 600, 15, 0.3f, 1f, SLOW);
        InitSkills("Defense", 5, Keyboard.current.spaceKey, "I'm Your Opponent", 30, Keyboard.current.qKey);

        ShowAttackReachArea(true);
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

    protected override void HandlePassiveSkill() { }

    protected override void HandleSkillUse()
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

    protected override async void UseSkill1()
    {
        // 스킬명: 방어
        // 효과: 사용 시 1초 간 무적

        if (!skillInfo1.StartCooldown()) return;
        // 추후 애니메이션 넣기

        isImmune = true;
        await Task.Delay(1000);
        isImmune = false;
    }

    protected override void UseSkill2()
    {
        // 스킬명: 네 상대는 나다
        // 효과: 보스 어그로 끌기

        if (!skillInfo2.StartCooldown()) return;
        // 추후 애니메이션 넣기

        GamePlayManager.instance.Boss.ChangeTarget(this);
    }
}
