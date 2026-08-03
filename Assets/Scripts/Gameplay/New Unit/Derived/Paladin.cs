using UnityEngine.InputSystem.Controls;

public class Paladin : Unit
{
    private SkillUseInfo shield;
    private SkillUseInfo bait;

    public PaladinStats Stats => (PaladinStats)stats;


    void Start()
    {
        baseAttack.ShowRange(true);
    }

    void Update()
    {
        if (state.CanAttack())
        {
            if (shield.SkillKey.isPressed)
            {
                Shield();
            }

            if (bait.SkillKey.isPressed)
            {
                Bait();
            }
        }
    }


    public override void Init(HpInfo hp, ButtonControl mouseButton, SkillUseInfo skill1, KeyControl skill1Key, SkillUseInfo skill2, KeyControl skill2Key)
    {
        state.SetHpInfo(hp);

        shield = skill1;
        shield.Init("Shield", skill1Key, Stats.shieldCooldown);

        bait = skill2;
        bait.Init("I'm Your Opponent", skill2Key, Stats.baitCooldown);
    }

    void Shield()
    {
        // 스킬명: 방어
        // 효과: 사용 시 1초 간 무적

        if (!shield.StartCooldown()) return;
        // 추후 애니메이션 넣기

        state.Immune(Stats.shieldDuration);
    }

    void Bait()
    {
        // 스킬명: 네 상대는 나다
        // 효과: 보스 어그로 끌기

        if (!bait.StartCooldown()) return;
        // 추후 애니메이션 넣기

        GameplayManager.instance.boss.Target(this);
    }
}
