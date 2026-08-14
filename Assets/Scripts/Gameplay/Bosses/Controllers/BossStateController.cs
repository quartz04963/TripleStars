using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public enum BossState
{
    READY,
    ATTACKING, //패턴 실행 중
    RECOVERY, //후딜레이
    GROGGY,
    STANDING,
}

abstract public class BossStateController : MonoBehaviour
{
    protected static readonly int StandingHash = Animator.StringToHash("Standing");
    protected static readonly int GroggyHash = Animator.StringToHash("Groggy");
    protected static readonly int MoveHash = Animator.StringToHash("Move");

    public Boss boss;
    
    [SerializeField] protected HpInfo hp;
    [SerializeField] protected BossState bossState;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected Animator animator;
    [SerializeField] protected BossBody weakpoint;
    
    protected readonly List<int> patternBalls = new();
    
    public BossState BossState {
        get => bossState;
        set => bossState = value;
    }
    
   
    public virtual void SetHpInfo(HpInfo hpInfo)
    {
        hp = hpInfo;

        hpInfo.Init(boss.bossName, boss.stats.maxHP);
    }

    public virtual void TakeDamage(float damage, Unit unit)
    {
        hp.AddHp(-damage);

        if (hp.CurrentHP <= 0) Die();

        if (unit == GameplayManager.instance.commander)
        {
            boss.targeting.HandleCommanderAggro(damage);
        }

        // Debug.Log(damage);
    }

    protected virtual void Die()
    {
        spriteRenderer.color = Color.gray;
        animator.Play("Groggy");
        
        boss.gameObject.SetActive(false);

        GameplayManager.instance.StageClear();
    }

    public virtual int GetNextSpecialPattern()
    {
        if (patternBalls.Count == 0) FillPatternBalls();

        int index = Random.Range(0, patternBalls.Count);
        int result = patternBalls[index];

        int last = patternBalls.Count - 1;
        patternBalls[index] = patternBalls[last];
        patternBalls.RemoveAt(last);

        return result;
    }

    abstract protected void FillPatternBalls();

    public virtual async Task Recover(float duration)
    {
        bossState = BossState.RECOVERY;
        animator.Play(StandingHash);

        await GameplayUtils.DelayForSeconds(duration);

        bossState = BossState.READY;
        animator.Play(MoveHash);
    }

    public virtual async Task Recover(float duration, CancellationToken token)
    {
        bossState = BossState.RECOVERY;
        animator.Play(StandingHash);

        await GameplayUtils.DelayForSeconds(duration, token);

        bossState = BossState.READY;
        animator.Play(MoveHash);
    }

    public virtual async Task Recover(float duration, bool isWeakened = false)
    {
        if (isWeakened) EnableWeakPoint(true);

        await Recover(duration);

        if (isWeakened) EnableWeakPoint(false);
    }


    public virtual async Task Groggy(float groggyDuration, float standingDuration = 1f) // 그로기
    {
        bossState = BossState.GROGGY;
        animator.Play(GroggyHash);

        await GameplayUtils.DelayForSeconds(groggyDuration);

        bossState = BossState.STANDING;
        animator.Play(StandingHash);

        await GameplayUtils.DelayForSeconds(standingDuration);

        bossState = BossState.READY;
    }

    public virtual void PlayAnimation(string name)
    {
        animator.Play(name, 0, 0f);
    }

    public virtual void EnableWeakPoint(bool isEnabled)
    {
        weakpoint.IsWeakPoint = isEnabled;
    }

}
