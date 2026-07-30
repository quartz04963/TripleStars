using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float speed;
    [SerializeField] Rigidbody2D rigidbody;
    
    public void Init(Enemy target, float damage)
    {
        this.damage = damage;

        Vector2 direction = (target.transform.position - transform.position).normalized;
        rigidbody.linearVelocity = direction * GameplayUtils.ToWorldDistance(speed);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            Hit(enemy);
        }
    }

    protected virtual void Hit(Enemy enemy)
    {
        enemy.TakeDamage(damage);

        Destroy(gameObject);
    }
}
