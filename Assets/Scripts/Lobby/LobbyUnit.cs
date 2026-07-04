using System.Threading.Tasks;
using UnityEngine;

public class LobbyUnit : MonoBehaviour
{
    [SerializeField] UnitLobbyData unitData;
    [SerializeField] JokeBalloon jokeBalloon;

    [SerializeField] SpriteRenderer spriteRenderer;

    public UnitLobbyData UnitData => unitData; 

    public async void OnClicked()
    {
        if (LobbyManager.instance.State == State.STANDBY)
        {
            jokeBalloon.Joke();
        }
        else if (LobbyManager.instance.State == State.BOSS_SELECTED || LobbyManager.instance.State == State.READY)
        {
            LobbyManager.instance.Camera.ZoomAndMove(transform.position + new Vector3(1.11f, -0.22f, -10f), 1.25f, 0.5f);
            await Task.Delay(500);

            LobbyManager.instance.ChangeState(State.SELECTING_UNIT);
            LobbyManager.instance.UnitSelection.ChangePanel(this);
        }
    }

    public void Select()
    {
        unitData.IsSelected = true;
        spriteRenderer.material.SetFloat("_OutlineEnabled", 1f);
    }

    public void Deselect()
    {
        unitData.IsSelected = false;
        spriteRenderer.material.SetFloat("_OutlineEnabled", 0f);
    }
}
