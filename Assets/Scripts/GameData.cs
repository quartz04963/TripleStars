using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Scriptable Objects/GameData")]
public class GameData : ScriptableObject
{
    public bool isTutorialShowed;

    public void Reset()
    {
        isTutorialShowed = false;
    }
}
