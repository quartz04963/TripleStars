using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public static class GameplayUtils
{
    private const float MAGNITUDE = 100;
    private const int FPS = 60;

    public static int ToMilliSecondsInteger(int frame)
    {
        return (int)(frame * 1000f / FPS);
    }

    public static float ToSecondsFloat(int frame)
    {
        return frame / (float)FPS;
    }

    public static float ToWorldDistance(float distance)
    {
        return distance / MAGNITUDE;
    }

    public static async Task DelayForFrames(int frame)
    {
        await Task.Delay(ToMilliSecondsInteger(frame));
    }

    public static async Task DelayForFrames(int frame, CancellationToken cancellationToken)
    {
        await Task.Delay(ToMilliSecondsInteger(frame), cancellationToken);
    }

    public static T FindNearest<T>(Transform transform, List<Collider2D> list)
    {
        T nearest = default;
        float minSqrDist = float.MaxValue;

        foreach (Collider2D col in list)
        {
            if (!col.TryGetComponent(out T t)) {
                t = col.GetComponentInParent<T>();
                
                if (t == null) continue;
            }

            float sqrDist = (col.transform.position - transform.position).sqrMagnitude;

            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                nearest = t;
            }
        }

        return nearest;
    }

    public static T FindNearest<T>(Transform transform, List<RaycastHit2D> list)
    {
        T nearest = default;
        float minSqrDist = float.MaxValue;

        foreach (RaycastHit2D hit in list)
        {
            if (!hit.transform.TryGetComponent(out T t)) {
                t = hit.transform.GetComponentInParent<T>();

                if (t == null) continue;
            }

            float sqrDist = (hit.transform.position - transform.position).sqrMagnitude;

            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                nearest = t;
            }
        }

        return nearest;
    }

    public static int FindAllInRange(Transform transform, float radius, ContactFilter2D filter, List<Collider2D> list)
    {
        list.Clear();

        return Physics2D.OverlapCircle(transform.position, ToWorldDistance(radius), filter, list);
    }

    public static bool IsInRange(Transform from, Transform to, float radius)
    {
        return (from.position - to.position).sqrMagnitude <= ToWorldDistance(radius) * ToWorldDistance(radius);
    }

    public static Vector2 MouseToWorldPoint()
    {
        Vector3 screenPos = Mouse.current.position.ReadValue();
        screenPos.z = 0;

        return Camera.main.ScreenToWorldPoint(screenPos);
    }
}
