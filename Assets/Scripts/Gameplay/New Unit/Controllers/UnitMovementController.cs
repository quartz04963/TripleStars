using System.Collections.Generic;
using UnityEngine;

public enum UnitSpeed
{
    SLOW = 240,
    NORMAL = 360,
    FAST = 480,
    VERYFAST = 600,
}

[RequireComponent(typeof(Collider2D))]
abstract public class UnitMovementController : MonoBehaviour
{
    public Unit unit;

    [SerializeField] protected bool isMoving;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected int currentKnockbackChainDmg;
    [SerializeField] protected float knockbackDecay = 3000;
    [SerializeField] protected Collider2D knockbackChainCollider;
    [SerializeField] protected Rigidbody2D rigidbody;

    protected Collider2D bodyCollider;
    protected readonly List<RaycastHit2D> hits = new();

    public bool IsMoving => isMoving;
    public float MoveSpeed {
        get => moveSpeed;
        set => moveSpeed = value;
    }
    

    protected virtual void Awake()
    {
        bodyCollider = GetComponent<Collider2D>();
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
            return;
        }

        if (!unit.state.IsAlive)
        {
            StopMove();
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
            if (HandleHitUnit(hits)) return;
            if (HandleHitWall(hits)) return;
        }
    }

    protected virtual bool HandleHitUnit(List<RaycastHit2D> hits)
    {
        Vector2 delta = rigidbody.linearVelocity * Time.fixedDeltaTime;

        int count = knockbackChainCollider.Cast(delta.normalized, GameplayUtils.unitFilter, hits, delta.magnitude);

        for (int i = count - 1; i >= 0; i--)
        {
            var unitState = hits[i].collider.GetComponent<UnitStateController>();

            if (unitState == null || unitState.IsKnockedBack) hits.RemoveAt(i);
        }

        if (hits.Count > 0)
        {
            var nearest = GameplayUtils.FindNearest<UnitStateController>(transform, hits);
            nearest.TakeDamage(currentKnockbackChainDmg);

            unit.state.TakeDamage(currentKnockbackChainDmg);
            StopMove();
            
            return true;
        }

        return false;
    }

    protected virtual bool HandleHitWall(List<RaycastHit2D> hits)
    {
        Vector2 delta = rigidbody.linearVelocity * Time.fixedDeltaTime;

        int count = rigidbody.Cast(delta.normalized, GameplayUtils.wallFilter, hits, delta.magnitude);

        if (count > 0)
        {
            unit.state.TakeDamage(currentKnockbackChainDmg);
            StopMove();

            return true;
        }

        return false;
    }

    public virtual void Teleport(Vector3 pos)
    {
        unit.transform.position = pos;

        StopMove();
    }

    abstract public void StopMove();
    abstract protected void Move();

}
