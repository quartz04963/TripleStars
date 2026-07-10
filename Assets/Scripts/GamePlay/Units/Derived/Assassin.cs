using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class Assassin : Follower
{
    [Header("Assassin")]
    [SerializeField] float passiveRequiredTime;
    [SerializeField] float passiveAvoidRate;
    [SerializeField] float poisonLastingDmg;
    [SerializeField] float poisonDmgPeriod;
    [SerializeField] int poisonMaxStack;
    [SerializeField] GameObject posionShurikenPrf;

    private bool isHiding;
    private float unmovedTime;
    private float lastPoisonDmgTime;
    private Dictionary<Enemy, int> poisionStackDict = new();

    void Start()
    {
        Init();
        InitStats("Assassin", 50, 1.8f, 1300, 40, 0.2f, 0.5f, VERYFAST);
        InitSkills("Posion Shuriken", 8, Keyboard.current.eKey);

        ShowAttackReachArea(true);
    }

    void Update()
    {
        GetDestination();
        HandleAttack();
        HandlePassiveSkill();
        HandleSkillUse();
        HandleSkillLastingEffect();
    }

    void FixedUpdate()
    {
        HandleMove();
    }

    public override bool TakeDamage(float damage, bool isAvoidable = false)
    {
        if (!isHiding || !isAvoidable)
        {
            return base.TakeDamage(damage, isAvoidable);
        }
        else if (Random.Range(0f, 1.0f) < passiveAvoidRate)
        {
            return false;
        }
        else
        {
            foreach (var key in poisionStackDict.Keys) // 피격 시 독 대미지 중첩 초기화
            {
                if (key != null) poisionStackDict[key] = 0;
            }

            return base.TakeDamage(damage, isAvoidable);
        }
    }

    protected override void Init()
    {
        base.Init();

        moveButton = Mouse.current.leftButton;
    }

    protected override void HandlePassiveSkill()
    {
        // 스킬명: 은신
        // 효과: 3초 동안 가만히 있으면 보스 어그로 설정 대상에서 제외 및 논타겟 공격이 50% 확률로 빗나감
        if (isMovable)
        {
            if (rigidbody.linearVelocity.magnitude > 0)
            {
                unmovedTime = 0;
            }
            else
            {
                unmovedTime += Time.deltaTime;
            }
        }

        if (unmovedTime < passiveRequiredTime)
        {
            isHiding = false;
            isTargetable = true;
        } 
        else
        {
            isHiding = true;
            isTargetable = false;
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
        // 스킬명: 독 수리검
        // 효과: 적중 시 초당 10 독 대미지, 최대 5중첩, 피격 시 중첩 초기화 

        if (target == null) return; // 일단 타겟팅된 적에게 발사
        
        if (!skillInfo1.StartCooldown()) return;
        // 추후 애니메이션 넣기
        
        await Task.Delay(0);

        Projectile projectile = Instantiate(posionShurikenPrf, transform.position, transform.rotation).GetComponent<Projectile>();
        projectile.Init(target, 0);
    }

    protected override void UseSkill2() { }

    public void IncreasePoisonStack(Enemy enemy)
    {
        if (poisionStackDict.TryGetValue(enemy, out int stack))
        {
            if (stack < poisonMaxStack) poisionStackDict[enemy]++;
        }
        else
        {
            poisionStackDict[enemy] = 1;
        }
    }

    void HandleSkillLastingEffect()
    {
        // 적중 시 초당 10 독 대미지
        if (Time.time >= lastPoisonDmgTime + poisonDmgPeriod)
        {
            lastPoisonDmgTime = Time.time;

            var deadKeys = poisionStackDict.Keys.Where(enemy => enemy == null).ToList();

            foreach (var key in deadKeys) poisionStackDict.Remove(key);

            foreach (Enemy enemy in poisionStackDict.Keys)
            {
                enemy.TakeDamage(poisonLastingDmg * poisionStackDict[enemy] * attackFactor);
            }
        }
    }
}

