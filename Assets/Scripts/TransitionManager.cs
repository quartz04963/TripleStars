using System.Threading.Tasks;
using PrimeTween;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager instance;

    [SerializeField] bool isTransiting;
    [SerializeField] float fadeInDuration;
    [SerializeField] float fadeOutDuration;
    [SerializeField] Image cover;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async void Transit(string sceneName)
    {
        if (isTransiting) return;

        isTransiting = true;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        Task wait = WaitForLoading(operation);
        Task fade = FadeOut();

        await Task.WhenAll(wait, fade);

        operation.allowSceneActivation = true;

        await FadeIn();

        isTransiting = false;
    }
        
    async Task FadeOut()
    {
        cover.gameObject.SetActive(true);

        await Tween.Alpha(cover, 1f, fadeOutDuration);
    }

    async Task FadeIn()
    {
        await Tween.Alpha(cover, 0f, fadeInDuration);

        cover.gameObject.SetActive(false);
    }

    async Task WaitForLoading(AsyncOperation operation)
    {
        while (operation.progress < 0.9f)
        {
            await Task.Yield();
        }
    }
}
