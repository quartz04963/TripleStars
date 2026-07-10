using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

abstract public class Follower : Unit
{
    [Header("Follower")]
    [SerializeField] LineRenderer lineRenderer;

    protected ButtonControl moveButton; // 초기화 필수
    protected Enemy target;

    private Vector2 destination;
    private Vector2 moveDirection;
    private float lastAttackTime;

    protected virtual void Init()
    {
        destination = transform.position;
    }

    #region 이동 관련
    protected virtual void GetDestination()
    {
        if (!moveButton.isPressed) return;

        destination = MouseToWorldPoint();
    }

    protected virtual void ResetDestination()
    {
        destination = transform.position;
        moveDirection = Vector2.zero;
        rigidbody.linearVelocity = Vector2.zero;
        ShowPath(false);
    }

    protected virtual void ShowPath(bool isActive)
    {
        if (!isActive)
        {
            lineRenderer.positionCount = 0;
            return;
        } 

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, destination);
    }

    public override void Teleport(Vector3 destination)
    {
        base.Teleport(destination);
        ResetDestination();
    }

    protected override void HandleMove()
    {
        if (!isMovable) return;

        if ((destination - (Vector2)transform.position).sqrMagnitude < moveSpeed / GameplayUtils.MAGNITUDE * Time.deltaTime)
        {
            ResetDestination();
            return;
        }

        moveDirection = (destination - (Vector2)transform.position).normalized;
        rigidbody.linearVelocity = moveDirection * moveSpeed / GameplayUtils.MAGNITUDE;
        ShowPath(true);

        // 추후 애니메이션 넣기
    }
    #endregion

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

        await Task.Delay((int)(attackDelay * 1000)); // 투사체 발사까지 모션 딜레이는 있으나 도중에 캔슬되지는 않음

        Projectile projectile = Instantiate(baseAttackProjectilePrf, transform.position, transform.rotation).GetComponent<Projectile>();
        projectile.Init(target, attackDamage * attackFactor);
    }

    #region 유틸리티
    protected Vector2 MouseToWorldPoint()
    {
        Vector3 screenPos = Mouse.current.position.ReadValue();
        screenPos.z = 0;

        return Camera.main.ScreenToWorldPoint(screenPos);
    }
    #endregion
}
