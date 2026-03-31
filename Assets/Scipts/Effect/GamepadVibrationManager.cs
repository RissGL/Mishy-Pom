using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerBoard))]
public class GamepadVibrationManager : MonoBehaviour
{
    private PlayerBoard board;

    private void Awake()
    {
        board = GetComponent<PlayerBoard>();
    }

    private void Start()
    {
        board.matchSystem.OnMatchCleared += MatchSystem_OnMatchCleared;
        board.matchSystem.OnSkillMatch += MatchSystem_OnSkillMatch;
        board.matchSystem.OnGameOver += MatchSystem_OnGameOver;

        board.skillSystem.OnSkillCast += SkillSystem_OnSkillCast;
        board.pushUpColumn.OnMultiMishyPushUp += PushUpColumn_OnMultiMishyPushUp;
    }

    private void OnDestroy()
    {
        // 记得注销事件，防止内存泄漏
        board.matchSystem.OnMatchCleared -= MatchSystem_OnMatchCleared;
        board.matchSystem.OnSkillMatch -= MatchSystem_OnSkillMatch;
        board.matchSystem.OnGameOver -= MatchSystem_OnGameOver;

        board.skillSystem.OnSkillCast -= SkillSystem_OnSkillCast;
        board.pushUpColumn.OnMultiMishyPushUp -= PushUpColumn_OnMultiMishyPushUp;
    }


    private void MatchSystem_OnMatchCleared(object sender, MatchSystem.MatchInfo e)
    {
        if (e.matchCombo >= 3 || e.matchCount > 4)
        {
            Rumble(0.5f, 0.7f, 0.3f); // 中等震动
        }
        else
        {
            Rumble(0.2f, 0.4f, 0.1f); // 轻微震动
        }
    }

    private void MatchSystem_OnSkillMatch(object sender, int e)
    {
        Rumble(0.2f, 0.4f, 0.15f); 
    }

    private void SkillSystem_OnSkillCast(object sender, SkillSystem.SkillUseInfo e)
    {
        Rumble(0.9f, 0.9f, 0.7f);
    }

    private void PushUpColumn_OnMultiMishyPushUp(object sender, MishyType[][] e)
    {
        Rumble(0.6f, 0.5f, 0.3f);
    }

    private void MatchSystem_OnGameOver(object sender, PlayerBoard e)
    {
        Rumble(1.5f, 1.5f, 0.8f);
    }


    private void Rumble(float lowFrequency, float highFrequency, float duration)
    {
        Gamepad gamepad = Gamepad.current;
        if (gamepad == null) return;

        gamepad.SetMotorSpeeds(lowFrequency, highFrequency);

        StopAllCoroutines();
        StartCoroutine(StopRumbleRoutine(duration, gamepad));
    }

    private IEnumerator StopRumbleRoutine(float duration, Gamepad gamepad)
    {
        yield return new WaitForSecondsRealtime(duration);
        gamepad.SetMotorSpeeds(0f, 0f);
    }

    private void OnDisable()
    {
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0f, 0f);
        }
    }
}