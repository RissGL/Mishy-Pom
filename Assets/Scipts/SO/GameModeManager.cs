using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameModeManager 
{
    public enum GameMode 
    {
        SinglePlayer, 
        PvE,          
        PvP
    }

    public enum Difficulty
    {
        VerryEasy,
        Easy,
        Normal,
        Hard,
        NightMare,
        Abyss,
    }

    public static GameMode currentGameMode { get; set; } =  GameMode.PvP;

    public static Difficulty difficulty { get; set; } = Difficulty.Normal;

    public static DifficultyConfig GetDifficultyConfig()
    {
        switch (difficulty)
        {
            case Difficulty.VerryEasy:
                return new DifficultyConfig
                {
                    thinkTime = 1.5f,
                    moveTime=0.4f,
                    basePushUpChance=12.5f,
                    chanceUpPerHalfMin=3f
                };
                break;
            case Difficulty.Easy:
                return new DifficultyConfig
                {
                    thinkTime = 1f,
                    moveTime = 0.35f,
                    basePushUpChance = 12.5f,
                    chanceUpPerHalfMin = 3f
                };
            case Difficulty.Normal:
                return new DifficultyConfig
                {
                    thinkTime = 0.6f,
                    moveTime = 0.2f,
                    basePushUpChance = 10f,
                    chanceUpPerHalfMin = 2.5f
                };
            case Difficulty.Hard:
                return new DifficultyConfig
                {
                    thinkTime = 0.4f,
                    moveTime = 0.2f,
                    basePushUpChance = 7.5f,
                    chanceUpPerHalfMin = 2.25f
                };
            case Difficulty.NightMare:
                return new DifficultyConfig
                {
                    thinkTime = 0.2f,
                    moveTime = 0.2f,
                    basePushUpChance = 6f,
                    chanceUpPerHalfMin = 2f
                };
            case Difficulty.Abyss:
                return new DifficultyConfig
                {
                    thinkTime = 0.08f,
                    moveTime = 0.1f,
                    basePushUpChance = 6f,
                    chanceUpPerHalfMin = 2f
                }; 
            break;
                default:
                return new DifficultyConfig {
                    thinkTime = 1f,
                    moveTime = 0.3f,
                    basePushUpChance = 10f,
                    chanceUpPerHalfMin = 2.5f
                };
        }
    }

    public struct DifficultyConfig 
    {
        public float thinkTime;
        public float moveTime;
        public float basePushUpChance;    // push基础概率
        public float chanceUpPerHalfMin;  // push概率增长速度
    }
}
