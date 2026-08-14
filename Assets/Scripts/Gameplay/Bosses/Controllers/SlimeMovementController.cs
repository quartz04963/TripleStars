using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SlimeMovementController : BossMovementController
{
    [SerializeField] List<Collider2D> bodyColliders;

    private bool isJumping;

    public SlimeStateController SlimeState => (SlimeStateController)boss.state;

    
     void FixedUpdate()
    {
        if (SlimeState.BossState == BossState.READY)
        {
            if (boss.targeting.Target != null && !boss.targeting.IsTargetInRange())
            {
                Chase();
                return;
            }
        } 

        if (SlimeState.BossState == BossState.ATTACKING)
        {
            if (isJumping)
            {
                return;
            }
        }

        FixPositionAndRotation();
    }


    public async Task JumpTo(Vector3 pos, float duration)
    {
        isJumping = true;

        foreach (Collider2D col in bodyColliders) col.enabled = false;

        FaceTarget();

        Vector2 direction = (pos - boss.transform.position).normalized;
        float speed = (pos - boss.transform.position).magnitude / duration;
        rigidbody.linearVelocity = direction * speed;

        await GameplayUtils.DelayForSeconds(duration);
        
        foreach (Collider2D col in bodyColliders) col.enabled = true;

        rigidbody.MovePosition(pos);
        rigidbody.linearVelocity = Vector2.zero;

        isJumping = false;
    }
}
