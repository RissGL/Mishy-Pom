using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public enum GameState 
    {
        Playing,
        Pause,
        GameOver
    }

    public GameState state;

    [SerializeField] private PlayerBoard player_one;
    [SerializeField] private PlayerBoard player_two;

    [SerializeField] private GameOverUI gameOverUI;

    private float gameTime=0;
    private bool isGameOver = false;

    private void Update()
    {


        gameTime += Time.deltaTime;
    }
    private void Awake()
    {
        player_one.matchSystem.OnGameOver += Player1_MatchSystem_OnGameOver;
        player_two.matchSystem.OnGameOver += Player2_MatchSystem_OnGameOver;

        player_one.skillSystem.OnSkillExecute += Player1_SkillSystem_OnSkillUse;
        player_two.skillSystem.OnSkillExecute += Player2_SkillSystem_OnSkillUse;
    }

    private void Player2_SkillSystem_OnSkillUse(object sender, SkillSystem.SkillUseInfo e)
    {
        if (isGameOver) return;
        player_one.pushUpColumn.PushMultiColumnMishyUp(e.matchColumnCount);
    }

    private void Player1_SkillSystem_OnSkillUse(object sender, SkillSystem.SkillUseInfo e)
    {
        if (isGameOver) return;
        player_two.pushUpColumn.PushMultiColumnMishyUp(e.matchColumnCount);
    }


    private void Player2_MatchSystem_OnGameOver(object sender, PlayerBoard e)
    {
        ExecuteGameOverSequence("Player 1");
    }

    private void Player1_MatchSystem_OnGameOver(object sender, PlayerBoard e)
    {
        ExecuteGameOverSequence("Player 2");
    }

    private void ExecuteGameOverSequence(string winnerName)
    {
        isGameOver = true;

        player_one.playerController.enabled = false;
        player_two.playerController.enabled = false;

        gameOverUI.Show(winnerName,gameTime);
    }
}
