using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Archer : Unit
{
    [Header("Archer")]
    [SerializeField] GameObject explosiveArrowPrf;

    private SkillUseInfo explosiveArrow;

    public ArcherStats Stats => (ArcherStats)stats;
    public MouseMovementController Movement => (MouseMovementController)movement;


    void Update()
    {
        if (state.CanAttack())
        {
            if (explosiveArrow.SkillKey.isPressed)
            {
                ExplosiveArrow();
            }
        }
    }


    public override void Init(HpInfo hp, ButtonControl mouseButton, SkillUseInfo skill1, KeyControl skill1Key, SkillUseInfo skill2, KeyControl skill2Key)
    {
        state.SetHpInfo(hp);

        Movement.SetButtonControl(mouseButton);

        explosiveArrow = skill1;
        explosiveArrow.Init("폭탄 화살", skill1Key, Stats.explosiveArrowCooldown);
    }

    void ExplosiveArrow()
    {
        // 스킬명: 폭탄 화살
        // 효과: 300 대미지 화살 발사

        if (baseAttack.Target == null) return;
        
        if (!explosiveArrow.StartCooldown()) return;

        state.PlayAnimation("Attack");

        var projectile = Instantiate(explosiveArrowPrf, transform).GetComponent<Projectile>();

        projectile.Init(Stats.explosiveArrowDmg * state.AttackFactor, this, baseAttack.Target.transform);
    }
}
