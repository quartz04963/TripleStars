using TMPro;
using UnityEngine;

public class LobbyUnit : MonoBehaviour
{
    [SerializeField] UnitLobbyData unitData;
    [SerializeField] JokeBalloon jokeBalloon;

    [SerializeField] SpriteRenderer spriteRenderer;

    public UnitLobbyData UnitData => unitData; 

    public void OnClicked()
    {
        if (LobbyManager.instance.State == State.STANDBY)
        {
            jokeBalloon.Joke();
        }
        else if (LobbyManager.instance.State == State.BOSS_SELECTED || LobbyManager.instance.State == State.READY)
        {
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
