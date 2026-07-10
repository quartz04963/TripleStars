using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossSelection : MonoBehaviour
{
    [SerializeField] int slot = 0;
    [SerializeField] BossLobbyData selectedBoss;

    [SerializeField] BossPanel bossPanel1;
    [SerializeField] BossPanel bossPanel2;
    [SerializeField] BossPanel bossPanel3;
    [SerializeField] Button nextButton;

    [SerializeField] List<BossLobbyData> bossDataList;
    [SerializeField] List<Sprite> bossSpriteList;
    public static Dictionary<BossCode, Sprite> bossSpriteDict;

    public BossCode SelectedBoss => selectedBoss != null ? selectedBoss.BossCode : BossCode.NULL;

    // 하드코딩, 추후 딕셔너리 정리 또는 ScriptableObject로 바꾸기
    void InitLists()
    {
        bossDataList = new List<BossLobbyData>
        {
            new BossLobbyData(BossCode.BOSS_1, "Boss 1", Difficulty.EASY),
            new BossLobbyData(BossCode.BOSS_2, "Boss 2", Difficulty.EASY),
            new BossLobbyData(BossCode.BOSS_3, "Boss 3", Difficulty.NORMAL),
            new BossLobbyData(BossCode.BOSS_4, "Boss 4", Difficulty.NORMAL),
            new BossLobbyData(BossCode.BOSS_5, "Boss 5", Difficulty.HARD),
            new BossLobbyData(BossCode.BOSS_6, "Boss 6", Difficulty.HARD),
        };

        bossSpriteDict = new Dictionary<BossCode, Sprite>
        {
            {BossCode.BOSS_1, bossSpriteList[0]},
            {BossCode.BOSS_2, bossSpriteList[1]},
            {BossCode.BOSS_3, bossSpriteList[2]},
            {BossCode.BOSS_4, bossSpriteList[3]},
            {BossCode.BOSS_5, bossSpriteList[4]},
            {BossCode.BOSS_6, bossSpriteList[5]},
        };
    }

    void InitPanels()
    {
        if (bossDataList.Count <= 3)
        {
            nextButton.gameObject.SetActive(false);
        }
        if (bossDataList.Count <= 2)
        {
            bossPanel3.gameObject.SetActive(false);
        }
        if (bossDataList.Count <= 1)
        {
            bossPanel2.gameObject.SetActive(false);
        }
        if (bossDataList.Count == 0)
        {
            bossPanel1.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        InitLists();
        InitPanels();
        ChangePanels();
    }

    void ChangePanels()
    {
        BossLobbyData leftSlot = bossDataList[slot % bossDataList.Count];
        BossLobbyData middleSlot = bossDataList[(slot + 1) % bossDataList.Count];
        BossLobbyData rightSlot = bossDataList[(slot + 2) % bossDataList.Count];
            
        if (bossDataList.Count >= 1) bossPanel1.Init(leftSlot, bossSpriteDict[leftSlot.BossCode]);
        if (bossDataList.Count >= 2) bossPanel2.Init(middleSlot, bossSpriteDict[middleSlot.BossCode]);
        if (bossDataList.Count >= 3) bossPanel3.Init(rightSlot, bossSpriteDict[rightSlot.BossCode]);
    }

    public void OnNextClicked()
    {
        slot++;
        ChangePanels();
    }

    public void SelectBoss(BossLobbyData bossData)
    {
        selectedBoss = bossData;
        LobbyManager.instance.ChangeState(State.BOSS_SELECTED);
    }
}
