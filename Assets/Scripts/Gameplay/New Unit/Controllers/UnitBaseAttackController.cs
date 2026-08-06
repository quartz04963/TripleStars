using UnityEngine;

[RequireComponent(typeof(LineRenderer), typeof(CircleCollider2D))]
abstract public class UnitBaseAttackController : MonoBehaviour
{
    public Unit unit;

    [SerializeField] protected int damage;
    [SerializeField] protected int range;
    [SerializeField] protected float period;
    [SerializeField] protected Boss target;

    protected CircleCollider2D rangeCollider;
    protected LineRenderer rangeOutline;

    public Boss Target => target;


    protected virtual void Awake()
    {
        rangeCollider = GetComponent<CircleCollider2D>();

        rangeOutline = GetComponent<LineRenderer>();
        rangeOutline.loop = true;
        rangeOutline.useWorldSpace = false;
    }

    protected virtual void Start()
    {
        damage = unit.stats.baseAttackDamage;
        range = unit.stats.baseAttackRange;
        period = unit.stats.baseAttackPeriod;

        rangeCollider.radius = GameplayUtils.ToWorldDistance(range);

        ShowRange(true);
    }

    protected virtual void Update()
    {
        if (unit.state.CanAttack())
        {
            FindTarget();
            Attack();
        }
    }


    public virtual void ShowRange(bool isActive)
    {
        int segments = 90;

        if (!isActive)
        {
            rangeOutline.positionCount = 0;
        }
        else
        {
            rangeOutline.positionCount = segments + 1;

            for (int i = 0; i <= segments; i++)
            {
                float angle = 2 * Mathf.PI * i / segments;

                float x = Mathf.Cos(angle) * GameplayUtils.ToWorldDistance(range);
                float y = Mathf.Sin(angle) * GameplayUtils.ToWorldDistance(range);

                rangeOutline.SetPosition(i, new Vector3(x, y, 0));
            }
        }
    }

    abstract protected void FindTarget();
    abstract protected void Attack();
    
}
