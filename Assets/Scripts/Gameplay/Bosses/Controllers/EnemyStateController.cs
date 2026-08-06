using UnityEngine;

public class EnemyStateController : MonoBehaviour
{
    public Enemy enemy;

    [SerializeField] protected HpInfo hp;


    public virtual void SetHpInfo(HpInfo hpInfo)
    {
        hp = hpInfo;

        hpInfo.Init(enemy.enemyName, enemy.stats.maxHP);
    }

    public virtual void TakeDamage(float damage, Unit unit)
    {
        hp.AddHp(-damage);

        if (hp.CurrentHP <= 0) Die();
    }

    protected virtual void Die()
    {
        // 추후 애니메이션 넣기
        
        enemy.gameObject.SetActive(false);
    }
}
