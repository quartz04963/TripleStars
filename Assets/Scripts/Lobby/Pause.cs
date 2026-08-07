using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public void Resume()
    {
        GameplayManager.instance.Resume();
    }

    public void Exit()
    {
        GameplayManager.instance.Resume();
        SceneManager.LoadScene("Lobby");
    }

    public void Title()
    {
        GameplayManager.instance.Resume();
        SceneManager.LoadScene("Title");
    }
}
