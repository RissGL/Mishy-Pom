using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillSystem : MonoBehaviour
{
    private PlayerBoard board;

    private int columnChangeCount = 0;
    public event EventHandler<SkillUseInfo> OnSkillExecute;
    public event EventHandler<SkillUseInfo> OnSkillCast;

    private SkillSystemVisual skillSystemVisual;

    [Header("技能按键配置")]
    public InputActionReference skillSelfAction;
    public InputActionReference skillEnemyAction;

    public int GetMatchColumn()=>columnChangeCount;


    private void Awake()
    {
        board = GetComponentInParent<PlayerBoard>();
    }

    private void Start()
    {
        skillSystemVisual = GetComponent<SkillSystemVisual>();
    }

    /// <summary>
    /// true是攻击。false是防御
    /// </summary>
    /// <param name="skillType"></param>
    public void UseSkill(bool skillType)
    {
        if (columnChangeCount < 1)
        {
            return;
        }

        if (board.matchSystem.IsMatching)
        {
            //消除的时候不能释放技能
            return;
        }
        int currentCount = columnChangeCount;

        if (board.skillBeamEffect != null)
        {
            CameraShakeManager.instance.ShakeHeavy();

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

        columnChangeCount = 0;

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
        columnChangeCount = e / 100;
        if(columnChangeCount>0)
        {
            skillSystemVisual.SetSkillPointVisual(true);
        }
    }
}
