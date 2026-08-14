using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class SlimeJumpArea : MonoBehaviour
{
    private CircleCollider2D collider;
    private readonly List<Collider2D> hitColliiders = new();


    void Awake()
    {
        collider = GetComponent<CircleCollider2D>();
    }

    public void SetSizeAndPosition(float radius, Transform target)
    {
        gameObject.SetActive(true);

        float diameter = 2 * GameplayUtils.ToWorldDistance(radius);
        transform.localScale = new Vector3(diameter, diameter, 1);

        transform.position = target.position;
    }

    public void Hit(float damage, float stunDuration)
    {
        collider.Overlap(GameplayUtils.unitFilter, hitColliiders);

        foreach (Collider2D col in hitColliiders)
        {
            if (!col.TryGetComponent(out UnitStateController unit)) continue;

            if (unit.TakeDamage(damage) && stunDuration > 0)
            {
                unit.Stun(stunDuration);
            }
        }

        gameObject.SetActive(false);
    }
}
