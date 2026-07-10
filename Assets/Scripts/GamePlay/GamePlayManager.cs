using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] SelectionData selectionData;
    [SerializeField] Boss boss;
    [SerializeField] Commander commander;
    [SerializeField] Follower attacker;
    [SerializeField] Follower supporter;
    [SerializeField] GameObject[] bossPrefabs;
    [SerializeField] GameObject[] unitPrefabs;

    [Header("Info Classes")]
    [SerializeField] HpInfo commanderHpInfo;
    [SerializeField] HpInfo attackerHpInfo;
    [SerializeField] HpInfo supporterHpInfo;
    [SerializeField] SkillUseInfo commanderSkillInfo1;
    [SerializeField] SkillUseInfo commanderSkillInfo2;
    [SerializeField] SkillUseInfo attackerSkillInfo;
    [SerializeField] SkillUseInfo supporterSkillInfo1;
    [SerializeField] SkillUseInfo supporterSkillInfo2;
    
    public static GameplayManager instance;

    public Boss Boss => boss;
    public Commander Commander => commander;
    public Follower Attacker => attacker;
    public Follower Supporter => supporter;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        InitUnits();
    }

    void InitBoss()
    {
        // TODO: 보스 코드에 따라 보스 소환하기
    }

    void InitUnits()
    {
        GameObject commanderPrf = unitPrefabs[(int)selectionData.commanderCode - 1];
        GameObject attackerPrf = unitPrefabs[(int)selectionData.attackerCode - 1];
        GameObject supporterPrf = unitPrefabs[(int)selectionData.supporterCode - 1];

        Instantiate(commanderPrf).TryGetComponent(out commander);
        Instantiate(attackerPrf).TryGetComponent(out attacker);
        Instantiate(supporterPrf).TryGetComponent(out supporter);

        commander.SetInfos(commanderHpInfo, commanderSkillInfo1, commanderSkillInfo2);
        attacker.SetInfos(attackerHpInfo, attackerSkillInfo, null);
        supporter.SetInfos(supporterHpInfo, supporterSkillInfo1, supporterSkillInfo2);
    }
}
