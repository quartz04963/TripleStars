using UnityEngine.InputSystem.Controls;
using UnityEngine;

public class Paladin : Unit
{
    [SerializeField] GameObject effectorPrf;

    private SkillUseInfo shield;
    private SkillUseInfo bait;

    public PaladinStats Stats => (PaladinStats)stats;


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
        shield.Init("방패 전개", skill1Key, Stats.shieldCooldown);

        bait = skill2;
        bait.Init("네 상대는 나다", skill2Key, Stats.baitCooldown);
    }

    void Shield()
    {
        // 스킬명: 방어
        // 효과: 사용 시 1초 간 무적

        if (!shield.StartCooldown()) return;

        var effector = Instantiate(effectorPrf, transform).GetComponent<Effector>();
        effector.PlayEffect(200, "Shield", 1f);

        state.Immune(Stats.shieldDuration);
    }

    async void Bait()
    {
        // 스킬명: 네 상대는 나다
        // 효과: 보스 어그로 끌기

        if (!bait.StartCooldown()) return;

        for (int i = 0; i < 3; i++)
        {
            var effector = Instantiate(effectorPrf, transform).GetComponent<Effector>();
            effector.PlayEffect(500, "Shockwave", 1f);

            await GameplayUtils.DelayForSeconds(0.22f);
        }

        if (GameplayManager.instance.boss is Boar boar)
        {
            boar.WasBaited = true;
        }

        GameplayManager.instance.boss.targeting.SetTarget(this);
    }
}
