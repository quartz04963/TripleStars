using UnityEngine;

[CreateAssetMenu(fileName = "SelectionData", menuName = "Scriptable Objects/SelectionData")]
public class SelectionData : ScriptableObject
{
    public BossCode bossCode;
    public Difficulty difficulty;

    public UnitCode commanderCode;
    public UnitCode attackerCode;
    public UnitCode supporterCode;

    public void Reset()
    {
        bossCode = BossCode.NULL;
        difficulty = Difficulty.EASY;
        
        commanderCode = UnitCode.NULL;
        attackerCode = UnitCode.NULL;
        supporterCode = UnitCode.NULL;
    }
}
