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
            int defScore = -300;
            int attackScore = -300;

            if (aiBoard.mishyManager.GetSpawnY() - aiBoard.gridManager.GetSpawnXTopY()-1
                <= playerBoard.skillSystem.GetMatchColumn())//¼õ1Ô¤Áô»º³å¿Õ¼ä
            {
                defScore += 500;
            }

            if (playerBoard.mishyManager.GetSpawnY()-playerBoard.gridManager.GetSpawnXTopY()-1<=
                aiBoard.skillSystem.GetMatchColumn()-playerBoard.skillSystem.GetMatchColumn())
            {
                attackScore += 800;
            }

            if (playerBoard.mishyManager.GetSpawnY() - playerBoard.gridManager.GetSpawnXTopY()-1 <=
            aiBoard.skillSystem.GetMatchColumn())
            {
                attackScore += 400;
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
            }
            yield return new WaitForSeconds(config.thinkTime);
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

        score -= dropY * 6;
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
        return 0;
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

        if (CheckType(x - 1, y, type)) bonus += 50f;
        if (CheckType(x + 1, y, type)) bonus += 50f;
        if (CheckType(x, y - 1, type)) bonus += 55f;

        return bonus;
    }
}
