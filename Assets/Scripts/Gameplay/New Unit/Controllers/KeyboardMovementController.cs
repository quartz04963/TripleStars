using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardMovementController : UnitMovementController
{
    protected Vector2 direction;


    protected virtual void Update()
    {
        if (GameplayManager.instance.IsPaused) return;
        
        GetDirection();
    }

    protected virtual void GetDirection()
    {
        direction = Vector2.zero;

        if (Keyboard.current.wKey.isPressed) direction += Vector2.up;
        if (Keyboard.current.aKey.isPressed) direction += Vector2.left;
        if (Keyboard.current.sKey.isPressed) direction += Vector2.down;
        if (Keyboard.current.dKey.isPressed) direction += Vector2.right;

        direction.Normalize();
    }

    protected override void Move()
    {
        isMoving = direction != Vector2.zero;

        rigidbody.linearVelocity = direction * GameplayUtils.ToWorldDistance(moveSpeed) * unit.state.MoveSpeedFactor;

        unit.state.FlipSprite(direction);
    }

    public override void StopMove()
    {
        isMoving = false;
        
        rigidbody.linearVelocity = Vector2.zero;
        rigidbody.MovePosition(unit.transform.position);
    }
}
