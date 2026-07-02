using System;

public enum BossCode
{
    None,
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
public readonly struct BossLobbyData
{
    public readonly BossCode bossCode;
    public readonly string name;
    public readonly Difficulty difficulty;

    public BossLobbyData(BossCode bossCode, string name, Difficulty difficulty)
    {
        this.bossCode = bossCode;
        this.name = name;
        this.difficulty = difficulty;
    }
}
