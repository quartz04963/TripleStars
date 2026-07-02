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
        nameText.SetText(bossData.name);
        difficultyText.SetText(getDifficultyString(bossData.difficulty));

        bossImg.sprite = sprite;
    }

    public void OnClicked()
    {
        LobbyManager.instance.SelectBoss(bossData.bossCode);
    }

    // 추후 기획 요청에 따라 텍스트 수정 필요
    public static string getDifficultyString(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.EASY: return "EASY";
            case Difficulty.NORMAL: return "NORMAL";
            case Difficulty.HARD: return "HARD";
            default: return "ERROR";
        }
    }
}
