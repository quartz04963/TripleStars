using UnityEngine;
using UnityEngine.InputSystem.Controls;

[RequireComponent(typeof(LineRenderer))]
public class MouseMovementController : UnitMovementController
{
    private LineRenderer lineRenderer;
    private ButtonControl button;

    private Vector2 destination;


    protected override void Awake()
    {
        base.Awake();

        lineRenderer = GetComponent<LineRenderer>();
    }

    protected override void Start()
    {
        base.Start();

        destination = unit.transform.position;
    }

    void Update()
    {
        GetDestination();
    }


    public void SetButtonControl(ButtonControl button)
    {
        this.button = button;
    }

    void GetDestination()
    {
        if (button.isPressed)
        {
            destination = GameplayUtils.MouseToWorldPoint();
        }
    }

    void ShowPath(bool isActive)
    {
        if (!isActive)
        {
            lineRenderer.positionCount = 0;
            return;
        } 

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, destination);
    }

    protected override void Move()
    {
        Vector2 direction = destination - (Vector2)unit.transform.position;
        float epsilon = GameplayUtils.ToWorldDistance(moveSpeed) * Time.fixedDeltaTime;
        
        if (IsBlocked(direction) || direction.sqrMagnitude < epsilon * epsilon)
        {
            StopMove();
            return;
        }

        isMoving = true;

        rigidbody.linearVelocity = direction.normalized * GameplayUtils.ToWorldDistance(moveSpeed);

        unit.state.FlipSprite(direction);

        ShowPath(true);
    }

    public override void StopMove()
    {
        isMoving = false;

        rigidbody.linearVelocity = Vector2.zero;
        
        destination = unit.transform.position;

        ShowPath(false);
    }

    bool IsBlocked(Vector2 direction)
    {
        Vector2 normDirection = direction.normalized;
        float delta = GameplayUtils.ToWorldDistance(moveSpeed * Time.fixedDeltaTime);

        int count = bodyCollider.Cast(direction.normalized, hits, delta);

        return count > 0;
    }
}
