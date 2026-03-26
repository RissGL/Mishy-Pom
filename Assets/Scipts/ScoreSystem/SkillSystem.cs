using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillSystem : MonoBehaviour
{
    private PlayerBoard board;

    private int columnUpCount = 0;
    public event EventHandler<SkillUseInfo> OnSkillExecute;
    public event EventHandler<SkillUseInfo> OnSkillCast;

    private SkillSystemVisual skillSystemVisual;

    [Header("技能按键配置")]
    public InputActionReference skillSelfAction;
    public InputActionReference skillEnemyAction;


    private void Awake()
    {
        board = GetComponentInParent<PlayerBoard>();
    }

    private void Start()
    {
        skillSystemVisual = GetComponent<SkillSystemVisual>();
    }

    private void Update()
    {
        if (skillSelfAction.action.WasPressedThisFrame())
        {
            UseSkill(false);
        }

        if (skillEnemyAction.action.WasPressedThisFrame())
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
        int currentCount = columnUpCount;

        if (board.skillBeamEffect != null)
        {
            OnSkillCast?.Invoke(this, new SkillUseInfo(skillType, currentCount));

            board.skillBeamEffect.PlayEffect(() => 
            {
                OnSkillExecute?.Invoke(this, new SkillUseInfo(skillType, currentCount));
            });
        }
        else
        {
            OnSkillExecute?.Invoke(this, new SkillUseInfo(skillType, currentCount));
        }

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
        board.scoreSystem.OnUpdateScore += ScoreSystem_OnUpdateScore;

        skillEnemyAction?.action.Enable();
        skillSelfAction?.action.Enable();
    }

    private void OnDisable()
    {
        board.scoreSystem.OnUpdateScore -= ScoreSystem_OnUpdateScore;

        skillEnemyAction?.action.Disable();
        skillSelfAction?.action.Disable();
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
