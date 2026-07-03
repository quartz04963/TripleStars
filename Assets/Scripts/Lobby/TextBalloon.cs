using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextBalloon : MonoBehaviour
{
    [SerializeField] Image background;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] CanvasGroup canvasGroup;

    public virtual void ChainAppear(Sequence sequence, float duration)
    {
        canvasGroup.alpha = 0f;
        sequence.Chain(Tween.Alpha(canvasGroup, 1f, duration));
    }

    public virtual void ChainDisappear(Sequence sequence, float duration)
    {
        sequence.Chain(Tween.Alpha(canvasGroup, 0f, duration));
    }

    public virtual void SetText(string str)
    {
        text.SetText(str);
    }
}
