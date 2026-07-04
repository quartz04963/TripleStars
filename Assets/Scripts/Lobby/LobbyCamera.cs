using PrimeTween;
using UnityEngine;

public class LobbyCamera : MonoBehaviour
{
    [SerializeField] Camera camera;

    public void ZoomAndMove(Vector3 pos, float oSize, float duration)
    {
        Tween.StopAll(transform);
        Tween.StopAll(camera);
        Tween.Position(transform, pos, duration);
        Tween.CameraOrthographicSize(camera, oSize, duration);
    }
}
