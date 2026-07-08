using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected string unitName;
    [SerializeField] protected HpInfo hpInfo;

    void Start()
    {
        hpInfo.Init(unitName, 100);
    }

    public virtual void TakeDamage(float damage)
    {
        hpInfo.AddHp(-1 * damage);
    }
}
