using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected string unitName;
    [SerializeField] protected HpInfo hpInfo;

    public virtual void TakeDamage(float damage)
    {
        hpInfo.AddHp(damage);
    }
}
