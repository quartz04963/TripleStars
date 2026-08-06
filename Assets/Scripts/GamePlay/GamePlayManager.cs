using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] SelectionData selectionData;
    [SerializeField] GameObject[] bossPrefabs;
    [SerializeField] GameObject[] unitPrefabs;
    public Boss boss;
    public Unit commander;
    public Unit attacker;
    public Unit supporter;

    [Header("Info Classes")]
    [SerializeField] HpInfo bossHpInfo;
    [SerializeField] HpInfo commanderHpInfo;
    [SerializeField] HpInfo attackerHpInfo;
    [SerializeField] HpInfo supporterHpInfo;
    [SerializeField] SkillUseInfo commanderSkill1Info;
    [SerializeField] SkillUseInfo commanderSkill2Info;
    [SerializeField] SkillUseInfo attackerSkillInfo;
    [SerializeField] SkillUseInfo supporterSkill1Info;
    [SerializeField] SkillUseInfo supporterSkill2Info;

    private KeyControl commanderSkill1Key = Keyboard.current.spaceKey;
    private KeyControl commanderSkill2Key = Keyboard.current.qKey;
    private KeyControl attackerSkill1Key = Keyboard.current.eKey;
    private KeyControl supporterSkill1Key = Keyboard.current.digit1Key;
    private KeyControl supporterSkill2Key = Keyboard.current.digit2Key;
    private ButtonControl attackerMoveButton = Mouse.current.leftButton;
    private ButtonControl supporterMoveButton = Mouse.current.rightButton;

    public static GameplayManager instance;
    public readonly List<Unit> allUnits = new();

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        InitUnits();
        InitBoss();
    }

    void InitBoss()
    {
        // TODO: 보스 코드에 따라 보스 소환하기

        boss.targeting.SetTarget(commander);

        boss.Init(bossHpInfo);
    }

    void InitUnits()
    {
        GameObject commanderPrf = unitPrefabs[(int)selectionData.commanderCode - 1];
        GameObject attackerPrf = unitPrefabs[(int)selectionData.attackerCode - 1];
        GameObject supporterPrf = unitPrefabs[(int)selectionData.supporterCode - 1];

        Instantiate(commanderPrf).TryGetComponent(out commander);
        Instantiate(attackerPrf).TryGetComponent(out attacker);
        Instantiate(supporterPrf).TryGetComponent(out supporter);

        commander.Init(commanderHpInfo, null, commanderSkill1Info, commanderSkill1Key, commanderSkill2Info, commanderSkill2Key);
        attacker.Init(attackerHpInfo, attackerMoveButton, attackerSkillInfo, attackerSkill1Key, null, null);
        supporter.Init(supporterHpInfo, supporterMoveButton, supporterSkill1Info, supporterSkill1Key, supporterSkill2Info, supporterSkill2Key);

        allUnits.Add(commander);
        allUnits.Add(attacker);
        allUnits.Add(supporter);
    }

    public void AddUnit(Unit unit)
    {
        allUnits.Add(unit);
    }
}
