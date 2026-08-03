using UnityEngine;
using UnityEngine.InputSystem.Controls;

[RequireComponent(typeof(LineRenderer))]
public class MouseMovementController : UnitMovementController
{
    private LineRenderer lineRenderer;
    private ButtonControl button;

    private Vector2 destination;


    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    protected override void Start()
    {
        base.Start();

        destination = transform.position;
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
        Vector2 direction = destination - (Vector2)transform.position;
        float epsilon = GameplayUtils.ToWorldDistance(moveSpeed) * Time.fixedDeltaTime;
        
        if (direction.sqrMagnitude < epsilon * epsilon)
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
        
        destination = transform.position;

        ShowPath(false);
    }
}
