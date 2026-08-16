using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popups : MonoBehaviour
{
    [SerializeField] GameObject clearPopup;
    [SerializeField] float starInterval;
    [SerializeField] Image star1Img;
    [SerializeField] Image star2Img;
    [SerializeField] Image star3Img;
    [SerializeField] Sprite starOnSprite;
    [SerializeField] TextMeshProUGUI clearCondition3Tmp;
    [SerializeField] TextMeshProUGUI failCondition3Tmp;
    [SerializeField] TextMeshProUGUI clearPraiseTmp;

    [SerializeField] GameObject failPoupup;

    public async void EnableClearPopup(bool condition2, bool condition3, float clearTimeThreshold)
    {
        clearPopup.SetActive(true);
        clearPraiseTmp.gameObject.SetActive(false);

        clearCondition3Tmp.SetText(clearTimeThreshold + "분 안에 클리어");
        
        await GameplayUtils.DelayForSecondsRealTime(starInterval);
        star1Img.sprite = starOnSprite;

        if (condition2)
        {
            await GameplayUtils.DelayForSecondsRealTime(starInterval);
            star2Img.sprite = starOnSprite;
        }

        if (condition3)
        {
            await GameplayUtils.DelayForSecondsRealTime(starInterval);
            star3Img.sprite = starOnSprite;
        }

        await GameplayUtils.DelayForSecondsRealTime(starInterval);

        string praise = condition2 && condition3 ? "Triple stars!" : 
                        condition2 || condition3 ? "Great!" : "Good!";
        
        clearPraiseTmp.SetText(praise);
        clearPraiseTmp.gameObject.SetActive(true);
    }

    public void EnableFailPopup(float clearTimeThreshold)
    {
        failCondition3Tmp.SetText(clearTimeThreshold + "분 안에 클리어");
        failPoupup.SetActive(true);
    }

    public void Resume()
    {
        GameplayManager.instance.Resume();
    }

    public void Exit()
    {
        GameplayManager.instance.Resume();
        TransitionManager.instance.Transit("Lobby");
    }

    public void Title()
    {
        GameplayManager.instance.Resume();
        TransitionManager.instance.Transit("Title");
    }

    public void Retry()
    {
        GameplayManager.instance.Resume();
        TransitionManager.instance.Transit("Gameplay");
    }
}
