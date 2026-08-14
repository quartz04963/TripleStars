using UnityEngine;

public class SlimeSlowArea : MonoBehaviour
{
    private Slime slime;

    public void Init(Vector3 pos, float radius)
    {
        transform.position = pos;

        float diameter = 2 * GameplayUtils.ToWorldDistance(radius);
        transform.localScale = new Vector3(diameter, diameter, 1);

        slime = (Slime)GameplayManager.instance.boss;
    }

    void OnTriggerStay2D(Collider2D other) 
    {
        if (!other.TryGetComponent(out UnitStateController unit)) return;

        slime.State.AddUnitOnSlowArea(unit.unit);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent(out UnitStateController unit)) return;

        slime.State.RemoveUnitOnSlowArea(unit.unit);
    }
}
