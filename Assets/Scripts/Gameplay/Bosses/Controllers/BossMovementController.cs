using UnityEngine;

public class BossMovementController : EnemyMovementController
{
    protected bool isRotationFixed;
    protected float fixedAngle;
    
    public Boss Boss => (Boss)enemy;


    protected virtual void FixedUpdate()
    {
        if (Boss.BossState.BossState == BossState.READY && !Boss.targeting.IsTargetInRange() && Boss.targeting.Target != null)
        {
            Chase();
            return;
        } 

        rigidbody.MoveRotation(fixedAngle);
        rigidbody.linearVelocity = Vector2.zero;
    }

    public virtual void Chase()
    {
        isRotationFixed = false;

        FaceTarget();

        Vector2 direction = (Boss.targeting.Target.transform.position - Boss.transform.position).normalized;

        rigidbody.linearVelocity = direction * GameplayUtils.ToWorldDistance(moveSpeed);
    }

    public virtual void FaceTarget()
    {
        if (Boss.targeting.Target == null) return;

        Vector2 longitude = Boss.targeting.Target.transform.position - Boss.transform.position;
        float angle = Mathf.Atan2(longitude.y, longitude.x) * Mathf.Rad2Deg;
        
        rigidbody.MoveRotation(angle);

        fixedAngle = angle;
    }
}
