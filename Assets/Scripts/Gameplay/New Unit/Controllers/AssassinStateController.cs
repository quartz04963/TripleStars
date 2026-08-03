using UnityEngine;

public class AssassinStateController : UnitStateController
{
    [SerializeField] bool isHiding;

    private Assassin Unit => (Assassin)unit;
    
    public bool IsHiding
    {
        get => isHiding;
        set => isHiding = value;
    }


    public override bool IsTargetable()
    {
        return !isHiding && base.IsTargetable();
    }

    public override bool TakeDamage(float damage, bool isDodgeable = true)
    {
        bool result;

        if (isDodgeable && isHiding)
        {
            if (Random.Range(0f, 1.0f) < Unit.Stats.hideDodgeRate)
            {
                return false;
            }
        }

        result = base.TakeDamage(damage, isDodgeable);
                
        if (result) Unit.ClearPoisonStack();

        return result;
    }
}
