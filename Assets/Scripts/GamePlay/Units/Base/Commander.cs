using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

abstract public class Commander : Unit
{
    protected Vector2 moveDirection;
    protected Enemy target;
    
    private float lastAttackTime;

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
        if (!isMovable) return;

        rigidbody.linearVelocity = moveDirection * moveSpeed / GameplayUtils.MAGNITUDE;
        
        // 추후 애니메이션 넣기
    }

    protected override async void HandleAttack()
    {
        if (!isAttackable) return;

        if (Time.time < lastAttackTime + attackPeriod) return;

        if (isMovable && rigidbody.linearVelocity.magnitude > 0) return; // 이동 중 공격 불가

        if (target == null || !GetInRange(target.transform, attackRange / GameplayUtils.MAGNITUDE)) // 우선 때리던 적을 계속 때리고 다음으로 가장 가까운 적을 타겟팅
        {
            target = FindNearestEnemy(attackRange / GameplayUtils.MAGNITUDE);
        }

        if (target == null) return;

        lastAttackTime = Time.time;
        // 추후 애니메이션 넣기

        await Task.Delay((int)(attackDelay * 1000)); // 데미지가 들어가기까지 모션 딜레이는 있으나 도중에 캔슬되지는 않음
        target.TakeDamage(attackDamage * attackFactor);
    }
}
