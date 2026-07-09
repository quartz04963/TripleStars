using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected string enemyName;
    [SerializeField] protected HpInfo hpInfo;

    private Unit target;

    public virtual void TakeDamage(float damage)
    {
        hpInfo.AddHp(-1 * damage);
    }

    public virtual void ChangeTarget(Unit newTarget)
    {
        target.Untarget();
        
        target = newTarget;
        newTarget.Target();
    }
}
