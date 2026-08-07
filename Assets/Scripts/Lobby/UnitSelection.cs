using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitSelection : MonoBehaviour
{
    [SerializeField] LobbyUnit currentUnit;
    [SerializeField] LobbyUnit selectedCommander;
    [SerializeField] LobbyUnit selectedAttacker;
    [SerializeField] LobbyUnit selectedSupporter;

    [SerializeField] Button selectButton;
    [SerializeField] Button deselectButton;
    [SerializeField] TextMeshProUGUI unitNameText;
    [SerializeField] TextMeshProUGUI roleText;
    [SerializeField] TextMeshProUGUI unitDescriptionText;
    [SerializeField] TextMeshProUGUI skillDescriptionText1;
    [SerializeField] TextMeshProUGUI skillDescriptionText2;
    [SerializeField] GameObject selectionChangePanel;
    [SerializeField] TextMeshProUGUI selectionChangeText;

    public UnitCode SelectedCommander => selectedCommander != null ? selectedCommander.UnitData.unitCode : UnitCode.NULL;
    public UnitCode SelectedAttacker => selectedAttacker != null ? selectedAttacker.UnitData.unitCode : UnitCode.NULL;
    public UnitCode SelectedSupporter => selectedSupporter != null ? selectedSupporter.UnitData.unitCode : UnitCode.NULL;

    // 추후 딕셔너리 정리 또는 ScriptableObject로 바꾸기
    void ChangeState()
    {
        LobbyManager.instance.Camera.ZoomAndMove(new Vector3(0, 0, -10), 5, 0f);
        
        if (selectedCommander == null || selectedAttacker == null || selectedSupporter == null)
        {
            LobbyManager.instance.ChangeState(LobbyState.BOSS_SELECTED);
        }
        else
        {
            LobbyManager.instance.ChangeState(LobbyState.READY);
        }
    }

    void SetSelctionChangeText()
    {
        Role role = currentUnit.UnitData.role;
        string selectedUnitName = role == Role.COMMANDER ? selectedCommander.UnitData.name :
                                  role == Role.ATTACKER ? selectedAttacker.UnitData.name :
                                  role == Role.SUPPORTER ? selectedSupporter.UnitData.name : "ERROR";

        // 임시 문구
        selectionChangeText.SetText(
            "The " + role + " " + selectedUnitName + " was already selected. " +
            "Will you change your selection with The " + role + " " + currentUnit.UnitData.name + "?"
        );
    }

    public void ChangePanel(LobbyUnit unit)
    {
        currentUnit = unit;

        UnitLobbyData unitData = unit.UnitData;

        selectButton.gameObject.SetActive(!unitData.isSelected);
        deselectButton.gameObject.SetActive(unitData.isSelected);   
        
        unitNameText.SetText(unitData.name);
        roleText.SetText(unitData.role.ToString());

        // 추후 string table로 변경 가능
        unitDescriptionText.SetText(unitData.unitDescription);
        skillDescriptionText1.SetText(unitData.skill1Description);
        skillDescriptionText2.SetText(unitData.skill2Description);

        selectionChangePanel.SetActive(false);
    }

    public void RemoveSelectedUnits()
    {
        if (selectedCommander != null) selectedCommander.Deselect();
        selectedCommander = null;

        if (selectedAttacker != null) selectedAttacker.Deselect();
        selectedAttacker = null;

        if (selectedSupporter != null) selectedSupporter.Deselect();
        selectedSupporter = null;
    }

    #region 버튼 클릭
    public void Select()
    {
        switch (currentUnit.UnitData.role)
        {
            case Role.COMMANDER: 
                if (selectedCommander != null) selectedCommander.Deselect();
                selectedCommander = currentUnit; 
                break;
            case Role.ATTACKER: 
                if (selectedAttacker != null) selectedAttacker.Deselect();
                selectedAttacker = currentUnit; 
                break;
            case Role.SUPPORTER: 
                if (selectedSupporter != null) selectedSupporter.Deselect();
                selectedSupporter = currentUnit; 
                break;
        }

        currentUnit.Select();
        ChangeState();
    }

    public void Deselect()
    {
        switch (currentUnit.UnitData.role)
        {
            case Role.COMMANDER: 
                selectedCommander = null; 
                break;
            case Role.ATTACKER: 
                selectedAttacker = null; 
                break;
            case Role.SUPPORTER: 
                selectedSupporter = null; 
                break;
        }

        currentUnit.Deselect();
        ChangeState();
    }
    
    public void OnSelectClicked()
    {
        bool isOccupied = (currentUnit.UnitData.role == Role.COMMANDER && selectedCommander != null) || 
                          (currentUnit.UnitData.role == Role.ATTACKER && selectedAttacker != null) ||
                          (currentUnit.UnitData.role == Role.SUPPORTER && selectedSupporter != null);

        if (isOccupied)
        {
            SetSelctionChangeText();
            selectionChangePanel.SetActive(true); 
            EventSystem.current.SetSelectedGameObject(null);
        }
        else
        {
            Select();
        }
    }

    public void OnDeselectClicked()
    {
        Deselect();
    }

    public void OnBackClicked()
    {
        ChangeState();
    }
    #endregion

    #region 디버깅용
    public void ShowSelectedUnits()
    {
        Debug.Log(
            selectedCommander + " / " + selectedAttacker + " / " + selectedSupporter
        );
    }
    #endregion
}
