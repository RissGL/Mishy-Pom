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
