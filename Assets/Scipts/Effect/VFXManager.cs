using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VEFManager : MonoBehaviour
{
    [SerializeField] private PlayerBoard board;
    [SerializeField] private FloatingScoreText floatingScoreVfx;


    private void Awake()
    {
        board.scoreSystem.OnAddScore += ScoreSystem_OnAddScore;
        board.matchSystem.OnBadMishyClear += MatchSystem_OnBadMishyClear;

        board.matchSystem.OnMatchCleared += MatchSystem_OnMatchCleared;
    }

    private void MatchSystem_OnMatchCleared(object sender, MatchSystem.MatchInfo e)
    {
        if (e.matchCombo >= 3 || e.matchCount > 4)
        {
            CameraShakeManager.instance.ShakeMedium();
            HitStopManager.Instance.TriggerHitStop(0f);
        }
        else 
        {
            CameraShakeManager.instance.ShakeMedium();
        }
    }

    private void MatchSystem_OnBadMishyClear(object sender, Vector3 e)
    {
        FloatingScoreText floatingScoreText = Instantiate(floatingScoreVfx, e, Quaternion.identity);
        floatingScoreText.BadMishyScoreSetUp();
        HitStopManager.Instance.TriggerHitStop(0.05f);
    }

    private void OnDestroy()
    {
        if (board != null && board.scoreSystem != null)
        {
            board.scoreSystem.OnAddScore -= ScoreSystem_OnAddScore;
        }
        board.matchSystem.OnMatchCleared -= MatchSystem_OnMatchCleared;
    }

    private void ScoreSystem_OnAddScore(object sender, ScoreSystem.ScoreAddedEventArgs e)
    {
        if (e.comboCount > 1)
        {
            FloatingScoreText floatingScoreText = Instantiate(floatingScoreVfx, e.centerPos, Quaternion.identity);
            floatingScoreText.SetUp(e.comboCount, e.comboAddedScore);

        }
    }
}
