using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfigManager : MonoBehaviour
{
    public static GameConfigManager Instance { get; private set; }

    [SerializeField] private DifficultyDatabaseSO difficultyDatabase;

    private void Awake()
    {
        Instance = this;
    }

    public DifficultyDatabaseSO.DifficultyConfig GetCurrentDifficultyConfig() 
    {
        return difficultyDatabase.GetDifficultyConfig(GameModeManager.difficulty);
    }
}
