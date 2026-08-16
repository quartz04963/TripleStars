using UnityEngine;

public class ExplosiveArrow : Projectile
{
    [SerializeField] GameObject effectorPrf;

    protected override void Hit(Enemy enemy)
    {
        var effector = Instantiate(effectorPrf, transform.position, transform.rotation).GetComponent<Effector>();
        effector.PlayEffect(500, "Explode", 1f);

        base.Hit(enemy);
    }
}
