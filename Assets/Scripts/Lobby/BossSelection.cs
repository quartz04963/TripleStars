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

    public BossCode SelectedBossCode => selectedBoss != null ? selectedBoss.bossCode : BossCode.NULL;
    public Difficulty Difficulty => selectedBoss != null ? selectedBoss.difficulty : Difficulty.EASY;

    // 하드코딩, 추후 딕셔너리 정리 또는 ScriptableObject로 바꾸기
    void InitLists()
    {
        bossDataList = new List<BossLobbyData>
        {
            new BossLobbyData(BossCode.SLIME, "거대 슬라임", Difficulty.EASY),
            new BossLobbyData(BossCode.BOAR, "폭주 멧돼지", Difficulty.NORMAL),
        };

        bossSpriteDict = new Dictionary<BossCode, Sprite>
        {
            {BossCode.SLIME, bossSpriteList[0]},
            {BossCode.BOAR, bossSpriteList[1]},
        };
    }

    void InitPanels()
    {
        if (bossDataList.Count <= 3) nextButton.gameObject.SetActive(false);
        if (bossDataList.Count <= 2) bossPanel3.gameObject.SetActive(false);
        if (bossDataList.Count <= 1) bossPanel2.gameObject.SetActive(false);
        if (bossDataList.Count == 0) bossPanel1.gameObject.SetActive(false);
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
            
        if (bossDataList.Count >= 1) bossPanel1.Init(leftSlot, bossSpriteDict[leftSlot.bossCode]);
        if (bossDataList.Count >= 2) bossPanel2.Init(middleSlot, bossSpriteDict[middleSlot.bossCode]);
        if (bossDataList.Count >= 3) bossPanel3.Init(rightSlot, bossSpriteDict[rightSlot.bossCode]);
    }

    public void OnNextClicked()
    {
        slot++;
        ChangePanels();
    }

    public void SelectBoss(BossLobbyData bossData)
    {
        selectedBoss = bossData;
        LobbyManager.instance.ChangeState(LobbyState.BOSS_SELECTED);
    }
}
