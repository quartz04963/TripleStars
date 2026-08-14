using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class ExplosiveMucus : MonoBehaviour
{
    [SerializeField] CircleCollider2D burstCollider;
    [SerializeField] CircleCollider2D burstForSlimeCollider;

    private Rigidbody2D rigidbody;
    private Collider2D hitCollider;

    private Slime slime;
    private Unit target;

    private float time = 0;
    private Vector3 offset;
    private readonly List<RaycastHit2D> hits = new();
    

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        hitCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (slime == null) return;

        time += Time.deltaTime;

        if (target == null && time >= slime.Stats.explodeMucusLifetime)
        {
            Destroy(gameObject);
        }

        if (target != null && time >= slime.Stats.explodeMucusBurstDelay)
        {
            Explode();
        }
    }

    void FixedUpdate()
    {
        if (slime == null) return;
        
        if (target == null)
        {
            float delta = slime.Stats.explodeMucusSpeed / slime.Stats.explodeMucusLifetime * Time.fixedDeltaTime; 
        
            rigidbody.linearVelocity = Vector2.MoveTowards(rigidbody.linearVelocity, Vector2.zero, delta);

            int count = hitCollider.Cast
            (
                rigidbody.linearVelocity.normalized, 
                GameplayUtils.unitFilter,
                hits, 
                rigidbody.linearVelocity.magnitude
            );

            if (count > 0)
            {
                var nearest = GameplayUtils.FindNearest<UnitStateController>(transform, hits);

                if (nearest != null)
                {
                    time = 0;

                    target = nearest.unit;
                    offset = transform.position - target.transform.position;
                    
                    burstCollider.gameObject.SetActive(true);
                    burstForSlimeCollider.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            transform.position = target.transform.position + offset;
        }
    }


    public void Init(Vector2 direction, Slime slime)
    {
        this.slime = slime;

        rigidbody.linearVelocity = direction.normalized * GameplayUtils.ToWorldDistance(slime.Stats.explodeMucusSpeed);

        float burstDiameter = 2 * GameplayUtils.ToWorldDistance(slime.Stats.explodeRangeRadius);
        burstCollider.transform.localScale = new Vector3(burstDiameter, burstDiameter, 1);
        burstCollider.gameObject.SetActive(false);

        float burstForSlimeDiameter = 2 * GameplayUtils.ToWorldDistance(slime.Stats.explodeRangeForSlime);
        burstForSlimeCollider.transform.localScale = new Vector3(burstForSlimeDiameter, burstForSlimeDiameter, 1);
        burstForSlimeCollider.gameObject.SetActive(false);
    }

    void Explode()
    {
        // TODO: 이펙트 생성?

        List<Collider2D> colliders = new();

        burstCollider.Overlap(GameplayUtils.unitFilter, colliders);

        foreach (Collider2D col in colliders)
        {
            if (!col.TryGetComponent(out UnitStateController unit)) continue;

            if (unit.unit != target)
            {
                unit.TakeDamage(slime.Stats.explodeBurstDamage);
            }
        }

        burstForSlimeCollider.Overlap(GameplayUtils.enemyFilter, colliders);

        foreach (Collider2D col in colliders)
        {
            if (!col.TryGetComponent(out BossBody bossBody)) continue;

            if (bossBody.boss == slime) 
            {
                bossBody.TakeDamage(slime.Stats.explodeDamageToSlime, null);

                slime.ExplodeRecoveryCTS?.Cancel();

                break;
            }
        }

        Destroy(gameObject);
    }

}
