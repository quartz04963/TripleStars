using UnityEngine;

[CreateAssetMenu(fileName = "SelectionData", menuName = "Scriptable Objects/SelectionData")]
public class SelectionData : ScriptableObject
{
    public BossCode bossCode;
    public UnitCode commanderCode;
    public UnitCode attackerCode;
    public UnitCode supporterCode;

    public void Reset()
    {
        bossCode = BossCode.NULL;
        
        commanderCode = UnitCode.NULL;
        attackerCode = UnitCode.NULL;
        supporterCode = UnitCode.NULL;
    }
}
