using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Scriptable Objects/GameData")]
public class GameData : ScriptableObject
{
    private bool isTutorialShowed;

    [Header("전투 관련")]
    public int lifeCount;
    public float reviveTime_seconds;
    public float bossSpawnDelay_seconds;
    public int[] clearTimeThresholds_minutes = new int[]{ 3, 5, 7 };

    public bool IsTutorialShowed
    {
        get => isTutorialShowed;
        set => isTutorialShowed = value;
    }

}
