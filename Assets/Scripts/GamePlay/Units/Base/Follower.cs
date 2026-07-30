using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

abstract public class Follower : Unit
{
    [Header("Follower")]
    [SerializeField] LineRenderer lineRenderer;

    protected Enemy target;
    protected ButtonControl moveButton; // 초기화 필수

    private Vector2 destination;
    private Vector2 moveDirection;
    private float lastAttackTime;

    protected override void Update()
    {
        base.Update();

        GetDestination();
    }

    protected virtual void Init()
    {
        destination = transform.position;
    }

    #region 이동 관련
    protected virtual void GetDestination()
    {
        if (!moveButton.isPressed) return;

        destination = GameplayUtils.MouseToWorldPoint();
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

    protected override void HandleMove()
    {
        if (!CanMove()) return;

        if ((destination - (Vector2)transform.position).sqrMagnitude < GameplayUtils.ToWorldDistance(moveSpeed) * Time.deltaTime)
        {
            StopMove();
            return;
        }

        moveDirection = (destination - (Vector2)transform.position).normalized;
        rigidbody.linearVelocity = moveDirection * GameplayUtils.ToWorldDistance(moveSpeed);
        ShowPath(true);

        if (moveDirection.x < 0) characterSR.transform.localScale = new Vector3(1, 1, 1);
        else if (moveDirection.x > 0) characterSR.transform.localScale = new Vector3(-1, 1, 1);
    }

    protected override void StopMove()
    {
        destination = transform.position;
        moveDirection = Vector2.zero;
        rigidbody.linearVelocity = Vector2.zero;
        ShowPath(false);
    }

    #endregion

    protected override async void HandleAttack()
    {
        if (!CanAttack()) return;

        if (Time.time < lastAttackTime + attackPeriod) return;

        if (target == null || !GameplayUtils.IsInRange(transform, target.transform, attackRange)) // 우선 때리던 적을 계속 때리고 다음으로 가장 가까운 적을 타겟팅
        {
            GameplayUtils.FindAllInRange(transform, attackRange, enemyFilter, enemiesInRange);
            target = GameplayUtils.FindNearest<Enemy>(transform, enemiesInRange); // 일단 근접 캐릭터도 약점 타격 가능
        }

        if (target == null) return;

        lastAttackTime = Time.time;
        // 추후 애니메이션 넣기

        await Task.Delay((int)(attackDelay * 1000)); // 투사체 발사까지 모션 딜레이는 있으나 도중에 캔슬되지는 않음

        Projectile projectile = Instantiate(baseAttackProjectilePrf, transform.position, transform.rotation).GetComponent<Projectile>();
        projectile.Init(target, attackDamage * attackFactor);
    }
}
