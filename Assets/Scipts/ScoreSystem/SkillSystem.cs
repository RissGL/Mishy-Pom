using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSystem : MonoBehaviour
{
    private int columnUpCount = 0;
    [SerializeField] private Transform skillPointVisual;
    public static event EventHandler OnSkillUse;

    public struct SkillUseInfo 
    {
        bool isUpToEnemy;//是否是让敌人上升
    }

    private void OnEnable()
    {
        ScoreSystem.OnUpdateScore += ScoreSystem_OnUpdateScore;
    }

    private void OnDisable()
    {
        ScoreSystem.OnUpdateScore -= ScoreSystem_OnUpdateScore;
    }
    private void ScoreSystem_OnUpdateScore(object sender, int e)
    {
        columnUpCount = e / 100;
        if(columnUpCount>0)
        { 
            skillPointVisual.gameObject.SetActive(true);
        }
    }
}
