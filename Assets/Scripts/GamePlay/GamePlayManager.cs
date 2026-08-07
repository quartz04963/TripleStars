using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class GameplayManager : MonoBehaviour
{
    [Header("엔티티")]
    public Boss boss;
    public Unit commander;
    public Unit attacker;
    public Unit supporter;

    [SerializeField] Popups popups;

    [Header("플레이 진행 상황 변수")]
    [SerializeField] bool isPaused = false;
    [SerializeField] bool isBossSpawned = false;
    [SerializeField] float bossSpawnDelay;
    [SerializeField] float clearTime;

    [Header("데이터")]
    [SerializeField] SelectionData selectionData;
    [SerializeField] float[] clearTimeThresholds = new float[]{ 180, 300, 420 };
    [SerializeField] GameObject[] bossPrefabs;
    [SerializeField] GameObject[] unitPrefabs;
    
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

    [Header("보스 스폰")]
    [SerializeField] GameObject bossSpawnArea;
    [SerializeField] GameObject bossSpawnWarning;
    [SerializeField] TextMeshProUGUI bossSpawnWarningTmp;
    [SerializeField] TextMeshProUGUI bossSpawnTimerTmp;

    private KeyControl commanderSkill1Key = Keyboard.current.spaceKey;
    private KeyControl commanderSkill2Key = Keyboard.current.qKey;
    private KeyControl attackerSkill1Key = Keyboard.current.eKey;
    private KeyControl supporterSkill1Key = Keyboard.current.digit1Key;
    private KeyControl supporterSkill2Key = Keyboard.current.digit2Key;
    private ButtonControl attackerMoveButton = Mouse.current.leftButton;
    private ButtonControl supporterMoveButton = Mouse.current.rightButton;

    public static GameplayManager instance;
    public readonly List<Unit> allUnits = new();

    public bool IsPaused => isPaused;


    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    async void Start()
    {
        InitUnits();

        await SpawnBoss();
    }

    void Update()
    {
        if (isBossSpawned) clearTime += Time.deltaTime;

        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            StageClear();
        }

        else if (Keyboard.current.xKey.wasPressedThisFrame)
        {
            GameOver();
        }
    }


    async Task SpawnBoss()
    {
        bossHpInfo.gameObject.SetActive(false);

        bossSpawnWarning.SetActive(true);
        bossSpawnArea.SetActive(true);

        float time = 0;
        while (time < bossSpawnDelay)
        {
            time += Time.deltaTime;

            bossSpawnTimerTmp.SetText("" + (int)(bossSpawnDelay - time + 1));

            await Task.Yield();
        }

        bossSpawnWarning.SetActive(false);
        bossSpawnArea.SetActive(false);

        // TODO: 보스 코드에 따라 보스 소환하기
        
        boss.gameObject.transform.position = bossSpawnArea.transform.position;
        boss.gameObject.SetActive(true);
        boss.targeting.SetTarget(commander);
        boss.Init(bossHpInfo);

        bossHpInfo.gameObject.SetActive(true);

        isBossSpawned = true;
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

    public void Pause()
    {
        Time.timeScale = 0f;

        isPaused = true;
    }

    public void Resume()
    {
        Time.timeScale = 1f;

        isPaused = false;
    }

    public void StageClear()
    {
        Pause();

        bool condition2 = commander.state.ReviveCount + attacker.state.ReviveCount + supporter.state.ReviveCount == 0;
        bool condition3 = clearTime <= clearTimeThresholds[(int)selectionData.difficulty];
        int clearTimeThreshold = (int)(clearTimeThresholds[(int)selectionData.difficulty] / 60);

        popups.EnableClearPopup(condition2, condition3, clearTimeThreshold);
    }

    public void GameOver()
    {
        Pause();

        popups.EnableFailPopup();
    }
}
