using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossPanel : MonoBehaviour
{
    [SerializeField] BossLobbyData bossData;
    
    [SerializeField] TextMeshProUGUI nameTmp;
    [SerializeField] TextMeshProUGUI difficultyTmp;
    [SerializeField] Image bossImg;

    public void Init(BossLobbyData bossData, Sprite sprite)
    {
        this.bossData = bossData;
        
        nameTmp.SetText(bossData.name);

        string difficultyText = bossData.difficulty == Difficulty.EASY ? "★☆☆" :
                                bossData.difficulty == Difficulty.NORMAL ? "★★☆" : "★★★";

        difficultyTmp.SetText(difficultyText);

        bossImg.sprite = sprite;
    }

    public void OnClicked()
    {
        LobbyManager.instance.BossSelection.SelectBoss(bossData);
    }
}
