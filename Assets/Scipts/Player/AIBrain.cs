using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBrain : MonoBehaviour
{
    private MishyPlayerController controller;
    private SkillSystem skillSystem;

    private bool isThinkingMoving = false;
    private PlayerBoard board;

    private void Awake()
    {
        controller=GetComponent <MishyPlayerController>();
        
        board=GetComponentInParent<PlayerBoard>();
        skillSystem=board.skillSystem;
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

        yield return new WaitForSeconds(0.25f);

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
        int width = board.gridManager.GetWidth();

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
            yield return new WaitForSeconds(0.15f);
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

            yield return new WaitForSeconds(0.1f);
        }

        if (controller.GetIsActive()) 
        {
            yield return new WaitForSeconds(0.1f);
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
        for (int y = 0; y < board.gridManager.GetHeight(); y++) 
        {
            if (!board.gridManager.HasMishy(new GridPosition(x, y)))
            {
                return y;
            }
        }
        return 0;
    }

    private bool CheckType(int x,int y,MishyType mishyType)
    {
        if (board.gridManager.TryGetGridMishy(new GridPosition(x, y), out Mishy mishy))
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
