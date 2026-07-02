using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    enum State
    {
        STANDBY,
        SELECTING_BOSS, 
        BOSS_SELECTED,
        SELECTING_CHARACTER,
        READY,
    }
    [SerializeField] State state;
    [SerializeField] BossCode selectedBoss;
    [SerializeField] Button startButton;
    [SerializeField] Button goButton;
    [SerializeField] Button menuButton;
    [SerializeField] Button backButton;
    [SerializeField] GameObject bossSelectionPanel;

    public static LobbyManager instance;

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Back();
        }
    }
    
    void ChangeState(State toState)
    {
        state = toState;

        switch (toState)
        {
            case State.STANDBY:
                startButton.gameObject.SetActive(true);
                goButton.gameObject.SetActive(false);
                menuButton.gameObject.SetActive(true);
                backButton.gameObject.SetActive(false);
                bossSelectionPanel.gameObject.SetActive(false);
                break;

            case State.SELECTING_BOSS:
                startButton.gameObject.SetActive(false);
                goButton.gameObject.SetActive(false);
                menuButton.gameObject.SetActive(true);
                backButton.gameObject.SetActive(true);
                bossSelectionPanel.gameObject.SetActive(true);
                break;

            case State.BOSS_SELECTED:
                startButton.gameObject.SetActive(false);
                goButton.gameObject.SetActive(true);
                menuButton.gameObject.SetActive(true);
                backButton.gameObject.SetActive(true);
                bossSelectionPanel.gameObject.SetActive(false);
                goButton.interactable = false;
                break;

            case State.SELECTING_CHARACTER:
                startButton.gameObject.SetActive(false);
                goButton.gameObject.SetActive(false);
                menuButton.gameObject.SetActive(false);
                backButton.gameObject.SetActive(false);
                bossSelectionPanel.gameObject.SetActive(false);
                break;

            case State.READY:
                startButton.gameObject.SetActive(false);
                goButton.gameObject.SetActive(true);
                menuButton.gameObject.SetActive(true);
                backButton.gameObject.SetActive(true);
                bossSelectionPanel.gameObject.SetActive(false);
                goButton.interactable = true;
                break;
        }
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

        // 스테이지 돌입
    }

    public void Back()
    {
        switch (state)
        {
            case State.SELECTING_BOSS: ChangeState(State.STANDBY); break;
            case State.BOSS_SELECTED: ChangeState(State.SELECTING_BOSS); break;
            case State.READY: ChangeState(State.SELECTING_BOSS); break;
        }
    }
    #endregion

    public void SelectBoss(BossCode bossCode)
    {
        if (state != State.SELECTING_BOSS) return;

        selectedBoss = bossCode;
        ChangeState(State.BOSS_SELECTED);
    }
}
