using UnityEngine;
using UnityEngine.UI;

public enum State
{
    STANDBY,
    SELECTING_BOSS, 
    BOSS_SELECTED,
    SELECTING_UNIT,
    READY,
}

public class LobbyManager : MonoBehaviour
{
    [SerializeField] State state;
    [SerializeField] BossSelection bossSelection;
    [SerializeField] UnitSelection unitSelection;

    [SerializeField] Button startButton;
    [SerializeField] Button goButton;
    [SerializeField] Button menuButton;
    [SerializeField] Button backButton;

    public static LobbyManager instance;

    public State State => state;
    public BossSelection BossSelection => bossSelection;
    public UnitSelection UnitSelection => unitSelection;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        ChangeState(State.STANDBY);
    }

    void Update()
    {

    }
    
    public void ChangeState(State toState)
    {
        state = toState;

        bool startActive = toState == State.STANDBY ? true : false;
        bool goActive = toState == State.BOSS_SELECTED || toState == State.READY ? true : false;
        bool menuActive = toState == State.SELECTING_UNIT ? false : true;
        bool backActive = toState == State.STANDBY || toState == State.SELECTING_UNIT ? false : true;
        bool bossSelectionActive = toState == State.SELECTING_BOSS ? true : false;
        bool unitSelectionActive = toState == State.SELECTING_UNIT ? true : false;

        startButton.gameObject.SetActive(startActive);
        goButton.gameObject.SetActive(goActive);
        menuButton.gameObject.SetActive(menuActive);
        backButton.gameObject.SetActive(backActive);
        bossSelection.gameObject.SetActive(bossSelectionActive);
        unitSelection.gameObject.SetActive(unitSelectionActive);

        goButton.interactable = toState == State.READY ? true : false;

        if (toState == State.STANDBY) 
        {
            unitSelection.RemoveSelectedUnits();
        }
        // unitSelection.ShowSelectedUnits();
    }

    #region 버튼 클릭
    public void GameStart()
    {
        if (state != State.STANDBY) return;

        ChangeState(State.SELECTING_BOSS);
    }

    public void Go()
    {
        if (state != State.READY) return;

        // TODO: 스테이지 돌입
    }

    public void Back()
    {
        switch (state)
        {
            case State.SELECTING_BOSS: 
            case State.BOSS_SELECTED:
            case State.READY: 
                ChangeState(State.STANDBY);
                break;
        }
    }
    #endregion
}
