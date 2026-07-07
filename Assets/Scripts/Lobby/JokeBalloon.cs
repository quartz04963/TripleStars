using PrimeTween;
using UnityEngine;

public class JokeBalloon : TextBalloon
{
    [SerializeField] float fadeInDuration;
    [SerializeField] float remainingDuration;
    [SerializeField] float fadeOutDuration;

    private bool isAvailable = true;
    private Sequence sequence;
    
    void Update()
    {
        if (LobbyManager.instance.State != State.STANDBY)
        {
            if (sequence.isAlive) sequence.Complete();
        }
    }

    public void Joke()
    {
        if (!isAvailable) return;

        if (sequence.isAlive) sequence.Stop();

        isAvailable = false;
        SetText(GetJokeText());

        sequence = Sequence.Create();
        ChainAppear(sequence, fadeInDuration);
        sequence.ChainDelay(remainingDuration)
            .ChainCallback(() => isAvailable = true);
        ChainDisappear(sequence, fadeOutDuration);
    }
    
    // 추후 요청 시 기능 구현
    public string GetJokeText()
    {
        return "Joke";
    }
}
