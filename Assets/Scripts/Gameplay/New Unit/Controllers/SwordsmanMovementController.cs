using UnityEngine;

public class SwordsmanMovementController : KeyboardMovementController
{
    private Vector2 rollDirection = Vector2.up;

    private Swordsman Unit => (Swordsman)unit;


    protected override void Update()
    {
        GetDirection();

        if (direction != Vector2.zero)
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
        rigidbody.linearVelocity = Vector2.zero;
    }
}
