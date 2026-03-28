using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyDatabase", menuName = "ScriptableObjects/DifficultyDatabase")]
public class DifficultyDatabaseSO : ScriptableObject
{
    [System.Serializable]
    public struct DifficultyConfig
    {
        public GameModeManager.Difficulty difficultyLevel;
        public float thinkTime;
        public float moveTime;
        public float basePushUpChance;    // push基础概率
        public float chanceUpPerHalfMin;  // push概率增长速度
        public float maxPushUpChance;
    }

    public List<DifficultyConfig> difficultyConfigs;

    public DifficultyConfig GetDifficultyConfig(GameModeManager.Difficulty diff)
    {
        foreach (DifficultyConfig config in difficultyConfigs)
        {
            if(config.difficultyLevel==diff)
            {
                return config;
            }
        }
        return difficultyConfigs.Count > 0 ? difficultyConfigs[0] : new DifficultyConfig();
    }
}
