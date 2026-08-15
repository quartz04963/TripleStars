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
    [SerializeField] TextMeshProUGUI unitNameTmp;
    [SerializeField] TextMeshProUGUI roleTmp;
    [SerializeField] TextMeshProUGUI unitDescriptionTmp;
    [SerializeField] TextMeshProUGUI skillDescriptionTmp1;
    [SerializeField] TextMeshProUGUI skillDescriptionTmp2;
    [SerializeField] GameObject selectionChangePanel;
    [SerializeField] TextMeshProUGUI selectionChangeTmp;

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
        
        string roleText = role == Role.COMMANDER ? "지휘관" :
                          role == Role.ATTACKER ? "원거리 딜러" :
                          role == Role.SUPPORTER ? "지원가" : "ERROR";

        // 임시 문구
        selectionChangeTmp.SetText(
            roleText + " " + selectedUnitName + "(이)가 이미 선택되었습니다. " +
            roleText + " " + currentUnit.UnitData.name + "로 선택을 바꾸시겠습니까?"
        );
    }

    public void ChangePanel(LobbyUnit unit)
    {
        currentUnit = unit;

        UnitLobbyData unitData = unit.UnitData;

        selectButton.gameObject.SetActive(!unitData.isSelected);
        deselectButton.gameObject.SetActive(unitData.isSelected);   
        
        unitNameTmp.SetText("이름: " + unitData.name);

        string roleText = unitData.role == Role.COMMANDER ? "지휘관" :
                          unitData.role == Role.ATTACKER ? "원거리 딜러" :
                          unitData.role == Role.SUPPORTER ? "지원가" : "ERROR";

        roleTmp.SetText("역할: " + roleText);

        // 추후 string table로 변경 가능
        unitDescriptionTmp.SetText(unitData.unitDescription);
        skillDescriptionTmp1.SetText(unitData.skill1Description);
        skillDescriptionTmp2.SetText(unitData.skill2Description);

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
}
