using TMPro;
using UnityEngine;

public class LobbyUnit : MonoBehaviour
{
    [SerializeField] UnitLobbyData unitData;

    [SerializeField] GameObject textBalloon;
    [SerializeField] TextMeshProUGUI jokeText;

    public UnitLobbyData UnitData => unitData; 

    void Joke()
    {
        Debug.Log("Joke() called");
        // TODO: 농담 말풍선 띄웠다가 잠시 후 사라지는 기능
    }

    public void OnClicked()
    {
        if (LobbyManager.instance.State == State.STANDBY)
        {
            Joke();
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

        // TODO: 윤곽선 하이라이트 활성화
    }

    public void Deselect()
    {
        unitData.IsSelected = false;

        // TODO: 윤곽선 하이라이트 비활성화
    }
}
