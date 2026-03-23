using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    private int score=0;
    public const int MAX_COMBO_COUNT = 5;

    public static event EventHandler<int> OnUpdateScore;
    private const int MAX_SCORE = 999;

    private void Start()
    {
        MatchSystem.OnMatchCleared += MatchSystem_OnMatchCleared;
    }

    private void OnDestroy()
    {
        MatchSystem.OnMatchCleared -= MatchSystem_OnMatchCleared;
    }

    private void MatchSystem_OnMatchCleared(object sender, MatchSystem.MatchInfo e)
    {
        score += e.matchCount;

        if(e.matchCombo>1)
        {
            int comboCount = Mathf.Min(e.matchCombo,MAX_COMBO_COUNT);
            int comboScore=0;

            for (int i = 2; i <= comboCount; i++) 
            {
                score += i * 5;
            }

            score += comboScore;
        }

        score = Mathf.Min(score, MAX_SCORE);
        Debug.Log(score);
        OnUpdateScore?.Invoke(this, score);
    }
}
