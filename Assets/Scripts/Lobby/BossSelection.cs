using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossSelection : MonoBehaviour
{
    [SerializeField] int slot = 0;
    [SerializeField] BossPanel bossPanel1;
    [SerializeField] BossPanel bossPanel2;
    [SerializeField] BossPanel bossPanel3;
    [SerializeField] Button nextButton;

    [SerializeField] List<Sprite> bossSpriteList;
    [SerializeField] List<BossLobbyData> bossDataList;

    // 하드코딩
    void InitLists()
    {
        bossSpriteList = new List<Sprite>
        {
            null, null, null, null, null, null,
        };
        bossDataList = new List<BossLobbyData>
        {
            new BossLobbyData(BossCode.BOSS_1, "Boss 1", Difficulty.EASY),
            new BossLobbyData(BossCode.BOSS_2, "Boss 2", Difficulty.EASY),
            new BossLobbyData(BossCode.BOSS_3, "Boss 3", Difficulty.NORMAL),
            new BossLobbyData(BossCode.BOSS_4, "Boss 4", Difficulty.NORMAL),
            new BossLobbyData(BossCode.BOSS_5, "Boss 5", Difficulty.HARD),
            new BossLobbyData(BossCode.BOSS_6, "Boss 6", Difficulty.HARD),
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
        int leftSlot = slot % bossDataList.Count,
            middleSlot = (slot + 1) % bossDataList.Count,
            rightSlot = (slot + 2) % bossDataList.Count;
            
        if (bossDataList.Count >= 1) bossPanel1.Init(bossDataList[leftSlot], bossSpriteList[leftSlot]);
        if (bossDataList.Count >= 2) bossPanel2.Init(bossDataList[middleSlot], bossSpriteList[middleSlot]);
        if (bossDataList.Count >= 3) bossPanel3.Init(bossDataList[rightSlot], bossSpriteList[rightSlot]);
    }

    public void OnNextClicked()
    {
        slot++;
        ChangePanels();
    }
}
