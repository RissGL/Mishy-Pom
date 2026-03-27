using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBrain : MonoBehaviour
{
    private MishyPlayerController controller;
    private SkillSystem skillSystem;

    private bool isThinkingMoving = false;
    private PlayerBoard aiBoard;

    private PlayerBoard playerBoard;

    public void SetPlayerBoard(PlayerBoard playerBoard)
    {
        this.playerBoard = playerBoard;
    }

    private void Awake()
    {
        controller=GetComponent <MishyPlayerController>();
        
        aiBoard=GetComponentInParent<PlayerBoard>();
        skillSystem=aiBoard.skillSystem;
    }

    private void Update()
    {
        if (PlayerManager.CurrentState != PlayerManager.GameState.Playing)
            return;

        if (controller.GetIsActive() && !isThinkingMoving)
        {
            if (PlayerManager.CurrentState != PlayerManager.GameState.Playing)
                return;
            StartCoroutine(AILoopRoutine());
        }
    }

    private IEnumerator AILoopRoutine()
    {
        isThinkingMoving=true;
        var config = GameModeManager.GetDifficultyConfig();


        yield return new WaitForSeconds(config.thinkTime);

        if (aiBoard.skillSystem.GetMatchColumn() > 0)
        {
            int aiSafeDist = aiBoard.mishyManager.GetSpawnY() - aiBoard.gridManager.GetSpawnXTopY() - 1;
            int playerSafeDist = playerBoard.mishyManager.GetSpawnY() - playerBoard.gridManager.GetSpawnXTopY() - 1;

            int aiSkills = aiBoard.skillSystem.GetMatchColumn();
            int playerSkills = playerBoard.skillSystem.GetMatchColumn();

            int rawScore = aiBoard.scoreSystem.GetCurrentScore();
            int wastedPoints = rawScore % 100;

            int defScore = 0;
            int attackScore = 0;

            if (aiSafeDist <= 4)
            {
                defScore += 10000; 
            }
            else if (aiSafeDist <= 8 && (aiSafeDist - playerSkills) <= 2 && aiSkills >= 1)
            {
                defScore += 500; 
            }

            if (defScore == 0)
            {
                if (playerSafeDist <= aiSkills - playerSkills)
                {
                    attackScore += 10000;
                }
                else if (aiSkills >= 3 && playerSafeDist <= aiSkills + 6)
                {
                    attackScore += 800;
                }
                else if (aiSkills >= 5)
                {
                    attackScore += 500;
                }
            }

            // 如果不是“生死攸关”的绝杀或濒死时刻
            // 并且当前被浪费的分数比较多
            // 并且总层数还没到大后期
            bool isEmergency = (defScore >= 10000 || attackScore >= 10000);

            if (!isEmergency && aiSkills < 7 && wastedPoints >= 60)
            {
                attackScore = 0;
                defScore = 0;
            }

            if (attackScore > 0 || defScore > 0)
            {
                if (attackScore > defScore)
                {
                    aiBoard.skillSystem.UseSkill(true);
                }
                else
                {
                    aiBoard.skillSystem.UseSkill(false);
                }
                yield return new WaitForSeconds(config.thinkTime);
            }
        }


        Mishy mishy_one=controller.GetMishyOne();
        Mishy mishy_two=controller.GetMishyTwo();

        if (mishy_one == null || mishy_two == null) 
        {
            isThinkingMoving=false;
            yield break;
        }

        MishyType mishyOneType=mishy_one.GetMishyType();
        MishyType mishyTwoType=mishy_two.GetMishyType();

        float bestScore = -9999f;
        int bestX = 0;
        bool bestSwap=false;
        int width = aiBoard.gridManager.GetWidth();

        for (int x = 0; x < width; x++)
        {
            for (int swap = 0; swap < 2; swap++)
            {
                bool isSwap = (swap == 1);

                MishyType bottomType=isSwap?mishyTwoType:mishyOneType;
                MishyType topType=isSwap?mishyOneType:mishyTwoType;

                float score = EvaluatePlacement(x, bottomType, topType);

                score += UnityEngine.Random.Range(0, 2f);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestX = x;
                    bestSwap=isSwap;
                }
            }
        }

        if (bestSwap) 
        {
            controller.CmdSwap();
            yield return new WaitForSeconds(config.thinkTime);
        }

        if (!controller.GetIsActive() || controller.GetMishyOne() == null)
        {
            isThinkingMoving = false;
            controller.IsHoldingFastFall = false;
            yield break;
        }

        int currentX=controller.GetMishyOne().GetGridPosition().x;
        while (currentX != bestX&&controller.GetIsActive())
        {
            if (currentX < bestX)
            {
                controller.CmdMoveRight();
                currentX++;
            }
            else if(currentX>bestX)
            {
                controller.CmdMoveLeft();
                currentX--;
            }

            yield return new WaitForSeconds(config.moveTime);
        }

        if (controller.GetIsActive()) 
        {
            yield return new WaitForSeconds(config.thinkTime);
            controller.IsHoldingFastFall = true;
            controller.CmdFastFallTrigger();
        }

        yield return new WaitUntil(() => !controller.GetIsActive()||mishy_one!=controller.GetMishyOne());
        isThinkingMoving = false;
        controller.IsHoldingFastFall = false;
    }

    private float EvaluatePlacement(int x, MishyType bottomType, MishyType topType) 
    {
        float score = 0;
        int dropY = GetDropY(x);

        int dangerLine = aiBoard.mishyManager.GetSpawnY() - 4;//离的近掉分快
        if (dropY >= dangerLine)
        {
            score -= (dropY - dangerLine + 1) * 200f; 
        }
        else
        {
            score -= dropY * 1.5f; 
        }

        score += GetMatchBonus(x, dropY, bottomType);
        score += GetMatchBonus(x, dropY + 1, topType);

        return score;
    }

    private int GetDropY(int x)
    {
        for (int y = 0; y < aiBoard.gridManager.GetHeight(); y++) 
        {
            if (!aiBoard.gridManager.HasMishy(new GridPosition(x, y)))
            {
                return y;
            }
        }
        return aiBoard.gridManager.GetHeight() - 1;
    }

    private bool CheckType(int x,int y,MishyType mishyType)
    {
        if (aiBoard.gridManager.TryGetGridMishy(new GridPosition(x, y), out Mishy mishy))
        {
            if (mishy.GetMishyType() == mishyType)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }
        return false;
    }

    private float GetMatchBonus(int x, int y, MishyType type)
    {
        float bonus = 0;

        if (CheckType(x - 1, y, type)) bonus += 20f;
        if (CheckType(x + 1, y, type)) bonus += 20f;
        if (CheckType(x, y - 1, type)) bonus += 20f;

        bool isMatch = false;
        if (CheckType(x - 1, y, type) && CheckType(x - 2, y, type)) isMatch = true;
        if (CheckType(x + 1, y, type) && CheckType(x + 2, y, type)) isMatch = true;
        if (CheckType(x, y - 1, type) && CheckType(x, y - 2, type)) isMatch = true;
        if (CheckType(x - 1, y, type) && CheckType(x + 1, y, type)) isMatch = true;

        if (isMatch)
        {
            bonus += 200f; // 基础消除分

            int blocksAbove = 0;

            for (int checkY = y + 1; checkY < aiBoard.gridManager.GetHeight(); checkY++)
            {
                if (aiBoard.gridManager.HasMishy(new GridPosition(x, checkY))) blocksAbove++;
                if (aiBoard.gridManager.HasMishy(new GridPosition(x - 1, checkY))) blocksAbove++;
                if (aiBoard.gridManager.HasMishy(new GridPosition(x + 1, checkY))) blocksAbove++;
            }

            bonus += blocksAbove * 50f;
        }

        return bonus;
    }
}
