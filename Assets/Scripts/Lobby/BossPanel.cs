using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossPanel : MonoBehaviour
{
    [SerializeField] BossLobbyData bossData;
    
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI difficultyText;
    [SerializeField] Image bossImg;

    public void Init(BossLobbyData bossData, Sprite sprite)
    {
        this.bossData = bossData;
        
        nameText.SetText(bossData.Name);
        difficultyText.SetText(bossData.Difficulty.ToString());

        bossImg.sprite = sprite;
    }

    public void OnClicked()
    {
        LobbyManager.instance.BossSelection.SelectBoss(bossData);
    }
}
