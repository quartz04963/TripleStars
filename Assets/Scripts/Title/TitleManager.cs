using UnityEngine;

public class TitleManager : MonoBehaviour
{
    public void GameStart()
    {
        TransitionManager.instance.Transit("Lobby");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
