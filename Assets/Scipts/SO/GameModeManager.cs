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

    public static GameMode currentGameMode { get; set; } =  GameMode.PvP;
}
