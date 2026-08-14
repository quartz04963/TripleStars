using UnityEngine;

[CreateAssetMenu(fileName = "SlimeStats", menuName = "Scriptable Objects/SlimeStats")]
public class SlimeStats : BossStats
{
    [Header("3연속 뛰기")]
    public int firstJumpRangeRadius;
    public int firstJumpDamage;
    public float firstJumpPredelay;
    public float firstJumpPostdelay;

    public int secondJumpRangeRadius;
    public int secondJumpDamage;
    public float secondJumpPredelay;
    public float secondJumpPostdelay;

    public int lastJumpRangeRadius;
    public int lastJumpDamage;
    public float lastJumpPredelay;
    public float lastJumpPostdelay;
    public float lastJumpStunDuration;

    [Header("점액질 웨이브")]
    public int waveFrequency;
    public int waveRangeRadius;
    public int waveDamage;
    public float wavePredelay;
    public float wavePostdelay;
    public float waveSlowFactor;
    public float waveSlowDuration;

    [Header("털어내기")]
    public int shakeFrequency;
    public int shakeSpreadRadius;
    public int shakeSlowAreaRadius;
    public int shakeSlowAreaNumber;
    public float shakePredelay;
    public float shakePostdelay;
    public float shakeSlowFactor;

    [Header("폭발성 점액")]
    public int explodeFrequency;
    public int explodeBurstDamage;
    public int explodeBurstRangeRadius;
    public int explodeBurstDamageToSlime;
    public int explodeBurstRangeForSlime;
    public int explodeMucusSpeed;
    public float explodeMucusLifetime;
    public float explodeMucusBurstDelay;
    public float explodePredelay;
    public float explodePostdelay;
    public float explodeGroggyDuration;
    
}
