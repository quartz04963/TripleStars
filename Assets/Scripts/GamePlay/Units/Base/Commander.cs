using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

abstract public class Commander : Unit
{
    protected Vector2 moveDirection;
    protected Enemy target;
    
    private float lastAttackTime;

    protected override void Update()
    {
        base.Update();

        GetDirection();
    }

    protected virtual void GetDirection()
    {
        moveDirection = Vector2.zero;

        if (Keyboard.current.wKey.isPressed) moveDirection += Vector2.up;
        if (Keyboard.current.aKey.isPressed) moveDirection += Vector2.left;
        if (Keyboard.current.sKey.isPressed) moveDirection += Vector2.down;
        if (Keyboard.current.dKey.isPressed) moveDirection += Vector2.right;

        moveDirection.Normalize();
    }

    protected override void HandleMove()
    {
        if (!CanMove()) return;

        rigidbody.linearVelocity = moveDirection * GameplayUtils.ToWorldDistance(moveSpeed);

        if (moveDirection.x < 0) characterSR.transform.localScale = new Vector3(1, 1, 1);
        else if (moveDirection.x > 0) characterSR.transform.localScale = new Vector3(-1, 1, 1);
    }

    protected override void StopMove()
    {
        rigidbody.linearVelocity = moveDirection = Vector2.zero;
    }

    protected override async void HandleAttack()
    {
        if (!CanAttack()) return;

        if (Time.time < lastAttackTime + attackPeriod) return;

        if (target == null || !GameplayUtils.IsInRange(transform, target.transform, GameplayUtils.ToWorldDistance(attackRange))) // 우선 때리던 적을 계속 때리고 다음으로 가장 가까운 적을 타겟팅
        {
            GameplayUtils.FindAllInRange(transform, GameplayUtils.ToWorldDistance(attackRange), enemyFilter, enemiesInRange);
            target = GameplayUtils.FindNearest<Enemy>(transform, enemiesInRange); // 일단 근접 캐릭터도 약점 타격 가능
        }

        if (target == null) return;

        lastAttackTime = Time.time;
        // 추후 애니메이션 넣기

        await Task.Delay((int)(attackDelay * 1000)); // 데미지가 들어가기까지 모션 딜레이는 있으나 도중에 캔슬되지는 않음
        target.TakeDamage(attackDamage * attackFactor);
    }
}
