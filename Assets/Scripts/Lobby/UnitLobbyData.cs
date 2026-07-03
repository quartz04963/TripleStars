using System;
using UnityEngine;

public enum UnitCode
{
    NULL,
    UNIT_1,
    UNIT_2,
    UNIT_3,
    UNIT_4,
    UNIT_5,
    UNIT_6,
}

public enum Role
{
    COMMANDER,
    ATTACKER,
    SUPPORTER,
}

[Serializable]
public class UnitLobbyData
{
    [SerializeField] bool isSelected = false;
    [SerializeField] UnitCode unitCode;
    [SerializeField] string name;
    [SerializeField] Role role;
    [SerializeField] string unitDescription;
    [SerializeField] string skillDescription1;
    [SerializeField] string skillDescription2;

    public bool IsSelected {
        get => isSelected;
        set => isSelected = value;
    }

    public UnitCode UnitCode => unitCode;
    public string Name => name;
    public Role Role => role;
    public string UnitDescription => unitDescription;
    public string SkillDescription1 => skillDescription1;
    public string SkillDescription2 => skillDescription2;

    public UnitLobbyData(UnitCode unitCode, string name, Role role, string unitDescription, string skillDescription1, string skillDescription2)
    {
        this.unitCode = unitCode;
        this.name = name;
        this.role = role;
        this.unitDescription = unitDescription;
        this.skillDescription1 = skillDescription1;
        this.skillDescription2 = skillDescription2;
    }
}
