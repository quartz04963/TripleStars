using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] protected float damage;
    [SerializeField] protected float speed = 2000;
    [SerializeField] protected float lifeTime = 10;
    [SerializeField] protected Unit caster;

    protected Rigidbody2D rigidbody;
    protected Collider2D collider;

    private float time = 0;
    private readonly List<RaycastHit2D> hits = new();

    protected virtual void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
    }

    protected virtual void Update()
    {
        time += Time.deltaTime;

        if (time >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void FixedUpdate()
    {
        Vector2 delta = rigidbody.linearVelocity * Time.fixedDeltaTime;

        int count = collider.Cast(delta.normalized, GameplayUtils.enemyFilter, hits, delta.magnitude);
   
        if (count > 0)
        {
            var nearest = GameplayUtils.FindNearest<Enemy>(transform, hits);

            if (nearest != null)
            {
                Hit(nearest);
            }
        }
    }
    
    public void Init(float damage, Unit caster, Transform target)
    {
        this.damage = damage;
        this.caster = caster;

        Vector2 direction = (target.position - transform.position).normalized;
        rigidbody.linearVelocity = direction * GameplayUtils.ToWorldDistance(speed);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    protected virtual void Hit(Enemy enemy)
    {
        if (enemy is BossBody bossBody)
        {
            bossBody.TakeDamage(damage, caster);
        }
        else
        {
            enemy.TakeDamage(damage, caster);
        }
                
        Destroy(gameObject);
    }
}
