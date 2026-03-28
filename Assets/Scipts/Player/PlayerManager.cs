using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static GameState CurrentState { get; private set; }
    public enum GameState 
    {
        CountDown,
        Playing,
        Pause,
        GameOver
    }


    [SerializeField] private InputActionReference pauseAction;

    [SerializeField] private PlayerBoard player_one;
    [SerializeField] private PlayerBoard player_two;

    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private PauseUI pauseUI;

    private float gameTime=0;
    private bool isGameOver = false;

    [Header("µ¹¼ÆÊ±")]
    [SerializeField] private CountdownUI countdownUI;

    public void InitGameMode() 
    {
        GameModeManager.GameMode mode=GameModeManager.currentGameMode;

        switch (mode)
        {
            case GameModeManager.GameMode.PvP:
                break;
                case GameModeManager.GameMode.PvE:
                var config=GameConfigManager.Instance.GetCurrentDifficultyConfig();
                PlayerInputHandler p2Input = player_two.GetComponentInChildren<PlayerInputHandler>();
                if (p2Input != null)
                {
                    Destroy(p2Input); 
                }
                AIBrain aIBrain= player_two.playerController.gameObject.AddComponent<AIBrain>();
                aIBrain.SetPlayerBoard(player_one);
                player_two.mishyManager.SetBasePushUpChance(config.basePushUpChance);
                player_two.mishyManager.SetChanceUpPerHalfMinute(config.chanceUpPerHalfMin);
                player_two.mishyManager.SetMaxPushUpChance(config.maxPushUpChance);
                break;
            case GameModeManager.GameMode.SinglePlayer:
                player_two.gameObject.SetActive(false);
                break;
        }
    }


    private void Update()
    {
        if (CurrentState == GameState.GameOver)
            return;

        if (pauseAction.action.WasPressedThisFrame())
        {
            TogglePause();
        }

        gameTime += Time.deltaTime;
    }

    public void TogglePause() 
    {
        if (CurrentState == GameState.Playing)
        {
            CurrentState = GameState.Pause;
            pauseUI.Show(gameTime);
            Time.timeScale = 0f;
        }
        else if (CurrentState == GameState.Pause)
        {
            CurrentState = GameState.Playing;
            pauseUI.Hide();
            Time.timeScale = 1f;
        }
    }

    private void OnEnable()
    {
        pauseAction?.action.Enable();
    }

    private void OnDisable()
    {
        pauseAction?.action.Disable();
    }

    private void Awake()
    {
        CurrentState = GameState.CountDown;

        player_one.matchSystem.OnGameOver += Player1_MatchSystem_OnGameOver;
        player_two.matchSystem.OnGameOver += Player2_MatchSystem_OnGameOver;

        player_one.skillSystem.OnSkillExecute += Player1_SkillSystem_OnSkillUse;
        player_two.skillSystem.OnSkillExecute += Player2_SkillSystem_OnSkillUse;

        countdownUI.OnCountdownOver += CountdownUI_OnCountdownOver;
    }

    private void Start()
    {
        InitGameMode();

        countdownUI.StartCountdown();
    }

    private void CountdownUI_OnCountdownOver(object sender, System.EventArgs e)
    {
        CurrentState=GameState.Playing;
    }

    private void Player2_SkillSystem_OnSkillUse(object sender, SkillSystem.SkillUseInfo e)
    {
        if (isGameOver) return;
        if (e.isUpToEnemy)
        {
            player_one.pushUpColumn.PushMultiColumnMishyUp(e.matchColumnCount);
        }
    }

    private void Player1_SkillSystem_OnSkillUse(object sender, SkillSystem.SkillUseInfo e)
    {
        if (isGameOver) return;
        if (e.isUpToEnemy)
        {
            player_two.pushUpColumn.PushMultiColumnMishyUp(e.matchColumnCount);
        }
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
