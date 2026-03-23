using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSystem : MonoBehaviour
{
    private int columnUpCount = 0;
    [SerializeField] private Transform skillPointVisual;
    public static event EventHandler<SkillUseInfo> OnSkillUse;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (columnUpCount < 1)
            {
                return;
            }
            OnSkillUse?.Invoke(this, new SkillUseInfo(false, columnUpCount));
            columnUpCount = 0;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (columnUpCount < 1)
            {
                return;
            }
            OnSkillUse?.Invoke(this, new SkillUseInfo(true, columnUpCount));
            columnUpCount = 0;
        }
    }


    public struct SkillUseInfo 
    {
        public SkillUseInfo(bool isUpToEnemy,int columnCount)
        {
            this.isUpToEnemy = isUpToEnemy;
            this.matchColumnCount = columnCount;
        }

        public bool isUpToEnemy;//是否是让敌人上升
        public int matchColumnCount;
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
