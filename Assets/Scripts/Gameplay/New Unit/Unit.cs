using UnityEngine;
using UnityEngine.InputSystem.Controls;

abstract public class Unit : MonoBehaviour
{
    public string unitName;
    
    public UnitStats stats;
    public UnitStateController state;
    public UnitMovementController movement;
    public UnitBaseAttackController baseAttack;    

    abstract public void Init(HpInfo hp, ButtonControl mouseButton, SkillUseInfo skill1, KeyControl skill1Key, SkillUseInfo skill2, KeyControl skill2Key);

}
