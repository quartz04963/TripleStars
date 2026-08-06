using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public static class GameplayUtils
{
    private const float MAGNITUDE = 100;

    private static List<Collider2D> colliders = new();

    public static ContactFilter2D unitFilter;
    public static ContactFilter2D wallFilter;
    public static ContactFilter2D enemyFilter;

    static GameplayUtils()
    {
        unitFilter = new ContactFilter2D()
        {
            useLayerMask = true,
            layerMask = LayerMask.GetMask("Unit"),
            useTriggers = true,
        };

        enemyFilter = new ContactFilter2D()
        {
            useLayerMask = true,
            layerMask = LayerMask.GetMask("Enemy"),
            useTriggers = true,
        };

        wallFilter = new ContactFilter2D()
        {
            useLayerMask = true,
            layerMask = LayerMask.GetMask("Wall"),
        };
    }

    public static float ToWorldDistance(float distance)
    {
        return distance / MAGNITUDE;
    }

    public static async Task DelayForSeconds(float seconds)
    {
        await Task.Delay((int)(seconds * 1000));
    }

    public static async Task DelayForSeconds(float seconds, CancellationToken token)
    {
        float elapsed = 0;

        while (elapsed < seconds)
        {
            elapsed += Time.deltaTime;

            token.ThrowIfCancellationRequested();

            await Task.Yield();
        }
    }

    public static T FindNearest<T>(Transform transform, List<Collider2D> list) where T : Object
    {
        T nearest = null;
        float minSqrDist = float.MaxValue;

        foreach (Collider2D col in list)
        {
            if (!col.TryGetComponent(out T t)) continue;

            float sqrDist = (col.transform.position - transform.position).sqrMagnitude;

            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                nearest = t;
            }
        }

        return nearest;
    }

    public static T FindNearest<T>(Transform transform, List<RaycastHit2D> list) where T : Object
    {
        T nearest = null;
        float minSqrDist = float.MaxValue;

        foreach (RaycastHit2D hit in list)
        {            
            if (!hit.collider.TryGetComponent(out T t)) continue;

            float sqrDist = (hit.collider.transform.position - transform.position).sqrMagnitude;

            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                nearest = t;
            }
        }

        return nearest;
    }

    public static bool IsInRange<T>(Transform from, T target, float range, ContactFilter2D filter) where T : Object
    {
        colliders.Clear();

        Physics2D.OverlapCircle(from.position, ToWorldDistance(range), filter, colliders);

        foreach (Collider2D col in colliders)
        {
            if (col.TryGetComponent(out T t) && t == target) return true;
        }

        return false;
    }

    public static Vector2 MouseToWorldPoint()
    {
        Vector3 screenPos = Mouse.current.position.ReadValue();
        screenPos.z = 0;

        return Camera.main.ScreenToWorldPoint(screenPos);
    }
}
