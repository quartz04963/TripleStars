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
    [SerializeField] Image unitImg;
    [SerializeField] TextMeshProUGUI unitNameText;
    [SerializeField] TextMeshProUGUI roleText;
    [SerializeField] TextMeshProUGUI unitDescriptionText;
    [SerializeField] TextMeshProUGUI skillDescriptionText1;
    [SerializeField] TextMeshProUGUI skillDescriptionText2;
    [SerializeField] GameObject selectionChangePanel;
    [SerializeField] TextMeshProUGUI selectionChangeText;

    [SerializeField] List<Sprite> unitArtList;
    public static Dictionary<UnitCode, Sprite> unitArtDict;

    // 추후 딕셔너리 정리 필요
    void InitLists()
    {
        unitArtDict = new Dictionary<UnitCode, Sprite>
        {
            {UnitCode.PALADIN, unitArtList[0]},
            {UnitCode.ARCHER, unitArtList[1]},
            {UnitCode.SAINTESS, unitArtList[2]},
            {UnitCode.SWORDSMAN, unitArtList[3]},
            {UnitCode.ASSASSIN, unitArtList[4]},
            {UnitCode.VANGUARD, unitArtList[5]},
        };
    }

    void ChangeState()
    {
        LobbyManager.instance.Camera.ZoomAndMove(new Vector3(0, 0, -10), 5, 0f);
        
        if (selectedCommander == null || selectedAttacker == null || selectedSupporter == null)
        {
            LobbyManager.instance.ChangeState(State.BOSS_SELECTED);
        }
        else
        {
            LobbyManager.instance.ChangeState(State.READY);
        }
    }

    void SetSelctionChangeText()
    {
        Role role = currentUnit.UnitData.Role;
        string selectedUnitName = role == Role.COMMANDER ? selectedCommander.UnitData.Name :
                                  role == Role.ATTACKER ? selectedAttacker.UnitData.Name :
                                  role == Role.SUPPORTER ? selectedSupporter.UnitData.Name : "ERROR";

        // 임시 문구
        selectionChangeText.SetText(
            "The " + role + " " + selectedUnitName + " was already selected. " +
            "Will you change your selection with The " + role + " " + currentUnit.UnitData.Name + "?"
        );
    }

    public void ChangePanel(LobbyUnit unit)
    {
        InitLists();

        currentUnit = unit;

        UnitLobbyData unitData = unit.UnitData;

        selectButton.gameObject.SetActive(!unitData.IsSelected);
        deselectButton.gameObject.SetActive(unitData.IsSelected);   
        
        unitImg.sprite = unitArtDict[unitData.UnitCode];

        unitNameText.SetText(unitData.Name);
        roleText.SetText(unitData.Role.ToString());

        // 추후 string table로 변경 가능
        unitDescriptionText.SetText(unitData.UnitDescription);
        skillDescriptionText1.SetText(unitData.SkillDescription1);
        skillDescriptionText2.SetText(unitData.SkillDescription2);

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
        switch (currentUnit.UnitData.Role)
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
        switch (currentUnit.UnitData.Role)
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
        bool isOccupied = (currentUnit.UnitData.Role == Role.COMMANDER && selectedCommander != null) || 
                          (currentUnit.UnitData.Role == Role.ATTACKER && selectedAttacker != null) ||
                          (currentUnit.UnitData.Role == Role.SUPPORTER && selectedSupporter != null);

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
