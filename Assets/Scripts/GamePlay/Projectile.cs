using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float speed;
    [SerializeField] Rigidbody2D rigidbody;

    protected Enemy target;

    void FixedUpdate()
    {
        Move();
    }
    
    public void Init(Enemy target, float damage)
    {
        this.target = target;
        this.damage = damage;
    }

    protected virtual void Move()
    {
        Vector2 direction = (target.transform.position - transform.position).normalized;
        rigidbody.linearVelocity = direction * speed / GameplayUtils.MAGNITUDE;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Enemy enemy) && enemy == target)
        {
            Hit();
        }
    }

    protected virtual void Hit()
    {
        target.TakeDamage(damage);
        Destroy(gameObject);
    }
}
