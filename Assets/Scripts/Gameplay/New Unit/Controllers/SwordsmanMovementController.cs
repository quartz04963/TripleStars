using UnityEngine;
using UnityEngine.InputSystem;

public class SwordsmanMovementController : KeyboardMovementController
{
    private Vector2 rollDirection = Vector2.up;

    private Swordsman Unit => (Swordsman)unit;


    protected override void Update()
    {
        GetDirection();

        bool wasDirectionKeyPressed = Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.aKey.wasPressedThisFrame || 
                                      Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame;

        if (wasDirectionKeyPressed)
        {
            Unit.RollCTS?.Cancel();
        }
    }


    protected override void GetDirection()
    {
        base.GetDirection();

        // 구르기 방향 저장
        if (direction != Vector2.zero) rollDirection = direction;
    }

    public void StartRoll()
    {
        rigidbody.linearVelocity = rollDirection * GameplayUtils.ToWorldDistance(Unit.Stats.rollSpeed);
    }

    public void StopRoll()
    {
        if (!unit.state.IsKnockedBack) rigidbody.linearVelocity = Vector2.zero;
    }
}
