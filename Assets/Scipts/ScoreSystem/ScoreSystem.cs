using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    private PlayerBoard board;

    private int score=0;
    public const int MAX_COMBO_COUNT = 5;

    public event EventHandler<int> OnUpdateScore;
    private const int MAX_SCORE = 999;

    private const int CLEAR_BAD_MISHY_SCORE = 100;

    private void Awake()
    {
        board = GetComponentInParent<PlayerBoard>();
    }

    private void Start()
    {
        board.matchSystem.OnMatchCleared += MatchSystem_OnMatchCleared;
        board.matchSystem.OnSkillMatch += MatchSystem_OnSkillMatch;
        board.matchSystem.OnBadMishyClear += MatchSystem_OnBadMishyClear;
        board.skillSystem.OnSkillUse += SkillSystem_OnSkillUse;
    }

    private void MatchSystem_OnBadMishyClear(object sender, EventArgs e)
    {
        score += CLEAR_BAD_MISHY_SCORE;
        score = Mathf.Min(score, MAX_SCORE);
        OnUpdateScore?.Invoke(this, score);
    }

    private void MatchSystem_OnSkillMatch(object sender, int e)
    {
        score += e;
        score = Mathf.Min(score, MAX_SCORE);
        OnUpdateScore?.Invoke(this, score);
    }

    private void SkillSystem_OnSkillUse(object sender, SkillSystem.SkillUseInfo e)
    {
        score = 0;
        OnUpdateScore?.Invoke(this, score);
    }

    private void OnDestroy()
    {
        board.matchSystem.OnMatchCleared -= MatchSystem_OnMatchCleared;
        board.matchSystem.OnSkillMatch -= MatchSystem_OnSkillMatch;
        board.matchSystem.OnBadMishyClear -= MatchSystem_OnBadMishyClear;
        board.skillSystem.OnSkillUse -= SkillSystem_OnSkillUse;
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
        OnUpdateScore?.Invoke(this, score);
    }
}
