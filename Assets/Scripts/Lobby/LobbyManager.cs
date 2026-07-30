using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum LobbyState
{
    STANDBY,
    SELECTING_BOSS, 
    BOSS_SELECTED,
    SELECTING_UNIT,
    READY,
}

public class LobbyManager : MonoBehaviour
{
    [SerializeField] LobbyState state;
    [SerializeField] SelectionData selectionData;
    [SerializeField] BossSelection bossSelection;
    [SerializeField] UnitSelection unitSelection;
    [SerializeField] LobbyCamera lobbyCamera;

    [SerializeField] Button startButton;
    [SerializeField] Button goButton;
    [SerializeField] Button menuButton;
    [SerializeField] Button backButton;

    public static LobbyManager instance;

    public LobbyState State => state;
    public BossSelection BossSelection => bossSelection;
    public UnitSelection UnitSelection => unitSelection;
    public LobbyCamera Camera => lobbyCamera;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        ChangeState(LobbyState.STANDBY);
    }

    public void ChangeState(LobbyState toState)
    {
        state = toState;

        bool startActive = toState == LobbyState.STANDBY ? true : false;
        bool goActive = toState == LobbyState.BOSS_SELECTED || toState == LobbyState.READY ? true : false;
        bool menuActive = toState == LobbyState.SELECTING_UNIT ? false : true;
        bool backActive = toState == LobbyState.STANDBY || toState == LobbyState.SELECTING_UNIT ? false : true;
        bool bossSelectionActive = toState == LobbyState.SELECTING_BOSS ? true : false;
        bool unitSelectionActive = toState == LobbyState.SELECTING_UNIT ? true : false;

        startButton.gameObject.SetActive(startActive);
        goButton.gameObject.SetActive(goActive);
        menuButton.gameObject.SetActive(menuActive);
        backButton.gameObject.SetActive(backActive);
        bossSelection.gameObject.SetActive(bossSelectionActive);
        unitSelection.gameObject.SetActive(unitSelectionActive);

        goButton.interactable = toState == LobbyState.READY ? true : false;

        if (toState == LobbyState.STANDBY) 
        {
            unitSelection.RemoveSelectedUnits();
        }
        // unitSelection.ShowSelectedUnits();
    }

    #region 버튼 클릭
    public void GameStart()
    {
        if (state != LobbyState.STANDBY) return;

        ChangeState(LobbyState.SELECTING_BOSS);
    }

    public void Go()
    {
        if (state != LobbyState.READY) return;

        selectionData.bossCode = bossSelection.SelectedBoss;
        selectionData.commanderCode = unitSelection.SelectedCommander;
        selectionData.attackerCode = unitSelection.SelectedAttacker;
        selectionData.supporterCode = unitSelection.SelectedSupporter;

        SceneManager.LoadScene("Gameplay");
    }

    public void Back()
    {
        switch (state)
        {
            case LobbyState.SELECTING_BOSS: 
            case LobbyState.BOSS_SELECTED:
            case LobbyState.READY: 
                ChangeState(LobbyState.STANDBY);
                break;
        }
    }
    #endregion
}
