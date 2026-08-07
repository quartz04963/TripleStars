using System;
using UnityEngine;

public enum BossCode
{
    NULL,
    BOSS_1,
    BOSS_2,
    BOSS_3,
    BOSS_4,
    BOSS_5,
    BOSS_6,
}

public enum Difficulty
{
    EASY,
    NORMAL,
    HARD,
}

[Serializable]
public class BossLobbyData
{
    public BossCode bossCode;
    public string name;
    public Difficulty difficulty;

    public BossLobbyData(BossCode bossCode, string name, Difficulty difficulty)
    {
        this.bossCode = bossCode;
        this.name = name;
        this.difficulty = difficulty;
    }
}
