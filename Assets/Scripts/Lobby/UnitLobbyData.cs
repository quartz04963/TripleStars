using System;
using UnityEngine;

public enum UnitCode
{
    NULL,
    PALADIN,
    ARCHER,
    SAINTESS,
    SWORDSMAN,
    ASSASSIN,
    VANGUARD,
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
    public bool isSelected = false;
    public UnitCode unitCode;
    public string name;
    public Role role;
    public string unitDescription;
    public string skill1Description;
    public string skill2Description;

    public UnitLobbyData(UnitCode unitCode, string name, Role role, string unitDescription, string skill1Description, string skill2Description)
    {
        this.unitCode = unitCode;
        this.name = name;
        this.role = role;
        this.unitDescription = unitDescription;
        this.skill1Description = skill1Description;
        this.skill2Description = skill2Description;
    }
}
