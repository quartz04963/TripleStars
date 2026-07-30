using System.Threading.Tasks;
using UnityEngine;

abstract public class Enemy : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] protected string enemyName;
    [SerializeField] protected HpInfo hpInfo;

    protected ContactFilter2D unitFilter;
    protected ContactFilter2D wallFilter;

    protected virtual void Awake()
    {
        unitFilter = new ContactFilter2D()
        {
            useLayerMask = true,
            layerMask = LayerMask.GetMask("Unit"),
            useTriggers = true,
        };

        wallFilter = new ContactFilter2D()
        {
            useLayerMask = true,
            layerMask = LayerMask.GetMask("Wall"),
        };
    }

    public virtual void TakeDamage(float damage)
    {
        hpInfo.AddHp(-damage);

        // 추후 애니메이션 넣기
    }
}
