using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class LobbyUnit : MonoBehaviour
{
    [SerializeField] UnitLobbyData unitData;
    [SerializeField] TextMeshProUGUI nameTmp;
    // [SerializeField] JokeBalloon jokeBalloon;

    [SerializeField] SpriteRenderer spriteRenderer;

    public UnitLobbyData UnitData => unitData; 

    void Awake()
    {
        nameTmp.SetText(unitData.name);
    }

    public async void OnClicked()
    {
        if (LobbyManager.instance.State == LobbyState.STANDBY)
        {
            // jokeBalloon.Joke();
        }
        else if (LobbyManager.instance.State == LobbyState.BOSS_SELECTED || LobbyManager.instance.State == LobbyState.READY)
        {
            LobbyManager.instance.Camera.ZoomAndMove(transform.position + new Vector3(1.11f, -0.22f, -10f), 1.25f, 0.5f);
            LobbyManager.instance.UnitSelection.ChangePanel(this);
            
            await Task.Delay(500);

            LobbyManager.instance.ChangeState(LobbyState.SELECTING_UNIT);
        }
    }

    public void Select()
    {
        unitData.isSelected = true;
        spriteRenderer.material.SetFloat("_OutlineEnabled", 1f);
    }

    public void Deselect()
    {
        unitData.isSelected = false;
        spriteRenderer.material.SetFloat("_OutlineEnabled", 0f);
    }
}
