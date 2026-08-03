using UnityEngine;

public class SwordsmanStateController : UnitStateController
{
    [SerializeField] bool isRolling;

    private Swordsman Unit => (Swordsman)unit;


    public override bool CanMove()
    {
        return !isRolling && base.CanMove();
    }

    public override void Knockback(Vector2 direction, float distance, int chainDamage = 0)
    {
        if (!isImmune)
        {
            Unit.RollCTS?.Cancel();
            Unit.FlameSwordSTS?.Cancel();
        }

        base.Knockback(direction, distance, chainDamage);
    }

    public override void Stun(float duration)
    {
        if (!isImmune)
        {
            Unit.RollCTS?.Cancel();
            Unit.FlameSwordSTS?.Cancel();
        }

        base.Stun(duration);
    }

    public void Roll()
    {
        isRolling = true;

        Unit.Movement.StartRoll();
    }

    public void EndRoll()
    {
        isRolling = false;

        Unit.Movement.StopRoll();
    }
}
