using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Effector : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public async void PlayEffect(float diameter, string effectName, float lifetime = 5f)
    {
        float worldDiameter = GameplayUtils.ToWorldDistance(diameter);
        transform.localScale = new Vector3(worldDiameter, worldDiameter, 1);

        animator.Play(effectName, 0, 0);

        await GameplayUtils.DelayForSeconds(lifetime);

        Destroy(gameObject);
    }
}
