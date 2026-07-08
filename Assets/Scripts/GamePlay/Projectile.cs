using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float speed;
    [SerializeField] Rigidbody2D rigidbody;

    private Enemy target;

    void FixedUpdate()
    {
        HandleMove();
    }
    
    public void Init(Enemy target)
    {
        this.target = target;
    }

    void HandleMove()
    {
        Vector2 direction = (target.transform.position - transform.position).normalized;
        rigidbody.linearVelocity = direction * speed / GamePlayUtils.MAGNITUDE;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Enemy enemy) && enemy == target)
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
