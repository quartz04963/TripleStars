using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class ExplosiveMucus : MonoBehaviour
{
    [SerializeField] CircleCollider2D burstCollider;
    [SerializeField] CircleCollider2D burstForSlimeCollider;
    [SerializeField] SlimeStats slimeStats;

    private Rigidbody2D rigidbody;
    private Collider2D hitCollider;

    private Slime slime;
    private Unit target;

    private float time = 0;
    private readonly List<RaycastHit2D> hits = new();
    

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        hitCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        float burstDiameter = 2 * GameplayUtils.ToWorldDistance(slimeStats.explodeBurstRangeRadius);
        burstCollider.transform.localScale = new Vector3(burstDiameter, burstDiameter, 1);
        burstCollider.gameObject.SetActive(false);

        float burstForSlimeDiameter = 2 * GameplayUtils.ToWorldDistance(slimeStats.explodeBurstRangeForSlime);
        burstForSlimeCollider.transform.localScale = new Vector3(burstForSlimeDiameter, burstForSlimeDiameter, 1);
        burstForSlimeCollider.gameObject.SetActive(false);
    }

    void Update()
    {
        if (slime == null) return;

        time += Time.deltaTime;

        if (target == null && time >= slimeStats.explodeMucusLifetime)
        {
            Destroy(gameObject);
        }

        if (target != null && time >= slimeStats.explodeMucusBurstDelay)
        {
            Explode();
        }
    }

    void FixedUpdate()
    {
        if (slime == null) return;
        
        if (target == null)
        {
            float dv = GameplayUtils.ToWorldDistance(slimeStats.explodeMucusSpeed) / slimeStats.explodeMucusLifetime * Time.fixedDeltaTime; 
            rigidbody.linearVelocity = Vector2.MoveTowards(rigidbody.linearVelocity, Vector2.zero, dv);

            float delta = rigidbody.linearVelocity.magnitude * Time.fixedDeltaTime;
            int count = hitCollider.Cast(rigidbody.linearVelocity.normalized, GameplayUtils.unitFilter, hits, delta);

            if (count > 0)
            {
                var nearest = GameplayUtils.FindNearest<UnitStateController>(transform, hits);

                if (nearest != null)
                {
                    time = 0;
                    target = nearest.unit;
                    
                    burstCollider.gameObject.SetActive(true);
                    burstForSlimeCollider.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            transform.position = target.transform.position;
        }
    }


    public void Init(Vector2 direction, Slime slime)
    {
        this.slime = slime;

        rigidbody.linearVelocity = direction.normalized * GameplayUtils.ToWorldDistance(slimeStats.explodeMucusSpeed);
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
                unit.TakeDamage(slimeStats.explodeBurstDamage);
            }
        }

        burstForSlimeCollider.Overlap(GameplayUtils.enemyFilter, colliders);

        foreach (Collider2D col in colliders)
        {
            if (!col.TryGetComponent(out BossBody bossBody)) continue;

            if (bossBody.boss == slime) 
            {
                bossBody.TakeDamage(slimeStats.explodeBurstDamageToSlime, null);

                slime.ExplodeRecoveryCTS?.Cancel();

                break;
            }
        }

        Destroy(gameObject);
    }

}
