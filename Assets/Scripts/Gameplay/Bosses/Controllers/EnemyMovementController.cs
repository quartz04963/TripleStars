using UnityEngine;

public class EnemyMovementController : MonoBehaviour
{
    public Enemy enemy;

    [SerializeField] protected float moveSpeed;
    [SerializeField] protected Rigidbody2D rigidbody;

    protected virtual void Start()
    {
        moveSpeed = enemy.stats.moveSpeed;
    }
}
