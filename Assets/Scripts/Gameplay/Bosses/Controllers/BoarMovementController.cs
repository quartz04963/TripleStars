using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BoarMovementController : BossMovementController
{
    private bool isRushAiming;
    private bool isRushing;

    private bool isRoaming;
    private bool isClockwise;
    private bool isTargetInRoamRange;
    private float rotationTime;
    private Vector3 roamCenter;

    private readonly List<RaycastHit2D> hitResults = new(); 

    public Boar Boar => (Boar)boss;
    public BoarStats BoarStats => (BoarStats)boss.stats;
    public BoarStateController BoarState => (BoarStateController)boss.state;


    void FixedUpdate()
    {
        if (BoarState.BossState == BossState.READY)
        {
            if (boss.targeting.Target != null && !boss.targeting.IsTargetInRange())
            {
                Chase();
                return;
            }
        } 

        if (BoarState.BossState == BossState.ATTACKING)
        {
            if (isRushAiming) 
            {
                FaceTarget();
            }
            if (isRushing)
            {
                UpdateRush();
                return;
            } 

            if (isRoaming)
            {
                if (!isTargetInRoamRange) RoamMoveStaright();
                else RoamRevolve();
                return;
            }
        }

        FixPositionAndRotation();
    }


    public async Task RushAim(float duration)
    {
        isRushAiming = true;

        await GameplayUtils.DelayForSeconds(duration);

        isRushAiming = false;
    }

    public void StartRush()
    {
        isRushing = true;

        Vector2 direction = transform.right; // 스프라이트가 오른쪽을 바라보고 있음 전제
        rigidbody.linearVelocity = direction * GameplayUtils.ToWorldDistance(BoarStats.rushDistance) / BoarStats.rushLastingDuration; // 등속도로 돌진
    }

    public void EndRush()
    {
        rigidbody.linearVelocity = Vector2.zero;

        isRushing = false;
    }

    async void UpdateRush()
    {
        Vector2 delta = rigidbody.linearVelocity * Time.fixedDeltaTime;

        int count = Boar.RushCollider.Cast(delta.normalized, GameplayUtils.unitFilter, hitResults, delta.magnitude);
        if (count > 0)
        {
            var nearest = GameplayUtils.FindNearest<UnitStateController>(Boar.RushCollider.transform, hitResults);

            if (nearest.TakeDamage(BoarStats.rushDamage))
            {
                nearest.Knockback(delta, BoarStats.rushKnockbackDistance, BoarStats.rushChainDamage);
                nearest.Stun(BoarStats.rushStunDuration);
            }

            Boar.RushCTS.Cancel();

            await BoarState.Recover(BoarStats.rushPostdelay, true);

            return;
        }

        count = Boar.HeadCollider.Cast(delta.normalized, GameplayUtils.wallFilter, hitResults, delta.magnitude);
        if (count > 0)
        {            
            BoarState.IncreaseWallCrashCount();

            Boar.RushCTS.Cancel();

            await BoarState.Groggy(BoarStats.rushGroggyDuration);

            return;
        }
    }

    public void StartRoam()
    {
        isRoaming = true;
        isTargetInRoamRange = false;
        
        isClockwise = Random.Range(0f, 1f) < 0.5f;

        rotationTime = 0;

        roamCenter = Boar.targeting.Target.transform.position;
    }

    public void EndRoam()
    {
        isRoaming = false;

        rigidbody.linearVelocity = Vector2.zero;
    }

    void RoamMoveStaright()
    {
        FaceTarget();

        Vector3 longitude = transform.position - roamCenter; 

        float epsilon = GameplayUtils.ToWorldDistance(moveSpeed) * Time.fixedDeltaTime;

        if (longitude.magnitude > GameplayUtils.ToWorldDistance(BoarStats.roamDistance) + epsilon) 
        {
            rigidbody.linearVelocity = -longitude.normalized * GameplayUtils.ToWorldDistance(moveSpeed);
        }
        else if (longitude.magnitude < GameplayUtils.ToWorldDistance(BoarStats.roamDistance) - epsilon)
        {
            rigidbody.linearVelocity = longitude.normalized * GameplayUtils.ToWorldDistance(moveSpeed);
        }
        else
        {
            isTargetInRoamRange = true;
        }
    }

    void RoamRevolve()
    {
        rotationTime += Time.deltaTime;

        Vector3 longitude = transform.position - roamCenter; 

        float angle = Mathf.Atan2(longitude.y, longitude.x) * Mathf.Rad2Deg;
        float speed = GameplayUtils.ToWorldDistance(BoarStats.roamDistance) * Mathf.PI * 0.5f;  
        angle += isClockwise? -90 : 90;

        rigidbody.linearVelocity = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * speed;

        FaceTarget();

        if (rotationTime > 1f)
        {
            Boar.RoamCTS.Cancel();
        }
    }
}
