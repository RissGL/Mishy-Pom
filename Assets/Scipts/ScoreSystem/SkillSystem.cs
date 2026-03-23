using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSystem : MonoBehaviour
{
    private int columnUpCount = 0;
    public static event EventHandler<SkillUseInfo> OnSkillUse;
    private SkillSystemVisual skillSystemVisual;


    private void Start()
    {
        skillSystemVisual = GetComponent<SkillSystemVisual>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            UseSkill(false);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            UseSkill(true);
        }
    }
    public void UseSkill(bool skillType)
    {
        if (columnUpCount < 1)
        {
            return;
        }
        OnSkillUse?.Invoke(this, new SkillUseInfo(skillType, columnUpCount));
        columnUpCount = 0;

        skillSystemVisual.SetSkillPointVisual(false);
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
            skillSystemVisual.SetSkillPointVisual(true);
        }
    }
}
