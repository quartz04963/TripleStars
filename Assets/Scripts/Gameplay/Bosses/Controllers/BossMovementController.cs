using UnityEngine;

public class BossMovementController : MonoBehaviour
{
    public Boss boss;
    
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected Rigidbody2D rigidbody;

    protected float fixedAngle;
    

    protected virtual void Start()
    {
        moveSpeed = boss.stats.moveSpeed;
    }


    public virtual void Chase()
    {
        FaceTarget();

        Vector2 direction = (boss.targeting.Target.transform.position - boss.transform.position).normalized;

        rigidbody.linearVelocity = direction * GameplayUtils.ToWorldDistance(moveSpeed);
    }

    public virtual void FaceTarget()
    {
        if (boss.targeting.Target == null) return;

        Vector2 longitude = boss.targeting.Target.transform.position - boss.transform.position;
        float angle = Mathf.Atan2(longitude.y, longitude.x) * Mathf.Rad2Deg;
        
        rigidbody.MoveRotation(angle);

        fixedAngle = angle;
    }

    public virtual void FacePos(Vector3 pos)
    {
        Vector2 longitude = pos - boss.transform.position;
        float angle = Mathf.Atan2(longitude.y, longitude.x) * Mathf.Rad2Deg;

        rigidbody.MoveRotation(angle);

        fixedAngle = angle;
    }

    protected virtual void FixPositionAndRotation()
    {
        rigidbody.MoveRotation(fixedAngle);
        rigidbody.linearVelocity = Vector2.zero;
    }
}
