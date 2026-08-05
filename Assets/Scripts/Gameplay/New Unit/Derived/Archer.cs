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
        UpdateFlight();

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
        explosiveArrow.Init("Explosive Arrow", skill1Key, Stats.explosiveArrowCooldown);
    }

    void UpdateFlight()
    {
        // 스킬명: 도주
        // 효과: 어그로 끌렸을 때 이동속도 증가
        
        movement.MoveSpeed = (float)(GameplayManager.instance.boss.IsTargeting(this) ? Stats.flightMoveSpeed : Stats.moveSpeed);
    }

    void ExplosiveArrow()
    {
        // 스킬명: 폭탄 화살
        // 효과: 300 대미지 화살 발사

        if (baseAttack.Target == null) return;
        
        if (!explosiveArrow.StartCooldown()) return;

        var projectile = Instantiate(explosiveArrowPrf, transform).GetComponent<Projectile>();

        projectile.Init(Stats.explosiveArrowDmg * state.AttackFactor, this, baseAttack.Target.transform);
    }
}
