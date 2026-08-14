using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SlimeMovementController : BossMovementController
{
    [SerializeField] List<Collider2D> bodyColliders;

    private bool isJumping;
    private bool isExplodeAiming;
    private Unit farthest;

    public SlimeStateController SlimeState => (SlimeStateController)boss.state;
    public Unit Farthest => farthest;

    
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
            if (isExplodeAiming)
            {
                FacePos(farthest.transform.position);
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

    public async Task ExplodeAim(float duration)
    {
        isExplodeAiming = true;

        // 대상 정하기
        float maxSqrDist = (GameplayManager.instance.commander.transform.position - boss.transform.position).sqrMagnitude;
        farthest = GameplayManager.instance.commander;

        float sqrDist = (GameplayManager.instance.attacker.transform.position - boss.transform.position).sqrMagnitude;
        if (sqrDist > maxSqrDist) 
        {
            maxSqrDist = sqrDist;
            farthest = GameplayManager.instance.attacker;
        }

        sqrDist = (GameplayManager.instance.supporter.transform.position - boss.transform.position).sqrMagnitude;
        if (sqrDist > maxSqrDist) 
        {
            farthest = GameplayManager.instance.supporter;
        }

        await GameplayUtils.DelayForSeconds(duration);

        isExplodeAiming = false;
    }
}
