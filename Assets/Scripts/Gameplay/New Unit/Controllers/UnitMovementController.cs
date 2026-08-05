using System.Collections.Generic;
using UnityEngine;

public enum UnitSpeed
{
    SLOW = 240,
    NORMAL = 360,
    FAST = 480,
    VERYFAST = 600,
}

abstract public class UnitMovementController : MonoBehaviour
{
    [SerializeField] protected Unit unit;

    [SerializeField] protected bool isMoving;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected int currentKnockbackChainDmg;
    [SerializeField] protected float knockbackDecay = 3000;
    [SerializeField] protected Collider2D knockbackChainCollider;
    [SerializeField] protected Rigidbody2D rigidbody;

    protected readonly List<RaycastHit2D> hits = new();

    public bool IsMoving => isMoving;
    public float MoveSpeed {
        get => moveSpeed;
        set => moveSpeed = value;
    }

    
    protected virtual void Start()
    {
        moveSpeed = unit.stats.moveSpeed;
    }

    protected virtual void FixedUpdate()
    {
        if (unit.state.IsKnockedBack)
        {
            UpdateKnockback();
            return;
        }

        if (unit.state.CanMove())
        {
            Move();
        }
    }


    public virtual void StartKnockback(Vector2 direction, float distance, int chainDamage = 0)
    {
        float initialSpeed = GameplayUtils.ToWorldDistance(Mathf.Sqrt(2 * distance * knockbackDecay));
        rigidbody.linearVelocity = direction.normalized * initialSpeed;

        currentKnockbackChainDmg = chainDamage;
    }

    protected virtual void UpdateKnockback()
    {
        float delta = GameplayUtils.ToWorldDistance(knockbackDecay * Time.fixedDeltaTime);

        rigidbody.linearVelocity = Vector2.MoveTowards(rigidbody.linearVelocity, Vector2.zero, delta);

        if (rigidbody.linearVelocity.sqrMagnitude < 0.001f)
        {
            StopMove();
            unit.state.EndKnockback();

            return;
        }

        if (currentKnockbackChainDmg > 0)
        {
            HandleHitUnit(hits);
            HandleHitWall(hits);
        }
    }

    protected virtual void HandleHitUnit(List<RaycastHit2D> hits)
    {
        Vector2 delta = rigidbody.linearVelocity * Time.fixedDeltaTime;

        int count = knockbackChainCollider.Cast(delta.normalized, GameplayUtils.unitFilter, hits, delta.magnitude);

        for (int i = count - 1; i >= 0; i--)
        {
            var unitState = hits[i].transform.GetComponent<UnitStateController>();

            if (unitState == null || unitState.IsKnockedBack) hits.RemoveAt(i);
        }

        if (hits.Count > 0)
        {
            var nearest = GameplayUtils.FindNearest<UnitStateController>(transform, hits);
            nearest.TakeDamage(currentKnockbackChainDmg);

            unit.state.TakeDamage(currentKnockbackChainDmg);
            StopMove();
        }
    }

    protected virtual void HandleHitWall(List<RaycastHit2D> hits)
    {
        Vector2 delta = rigidbody.linearVelocity * Time.fixedDeltaTime;

        int count = rigidbody.Cast(delta.normalized, GameplayUtils.wallFilter, hits, delta.magnitude);

        if (count > 0)
        {
            unit.state.TakeDamage(currentKnockbackChainDmg);
            StopMove();
        }
    }

    public virtual void Teleport(Vector3 pos)
    {
        unit.transform.position = pos;

        StopMove();
    }

    abstract public void StopMove();
    abstract protected void Move();

}
