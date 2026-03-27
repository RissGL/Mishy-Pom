using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    [Header("°´Å¥")]
    [SerializeField] private Button pvpButton;
    [SerializeField] private Button pveButton;
    [SerializeField] private Button singleButton;
    [SerializeField] private Button exitButton;

    [Header("UI")]
    [SerializeField] private GameObject backGround;
    [SerializeField] private GameObject slider;
    [SerializeField] private Image sliderImage;

    [SerializeField] private DifficultyChangeUI difficultyChangeUI;


    private void Awake()
    {
        slider.gameObject.SetActive(false);
        singleButton.onClick.AddListener(() => 
        {
            StartButton_OnClick(GameModeManager.GameMode.SinglePlayer);
        });

        pveButton.onClick.AddListener(() =>
        {
            GameModeManager.currentGameMode = GameModeManager.GameMode.PvE;
            difficultyChangeUI.Show(this);
        });

        pvpButton.onClick.AddListener(() => 
        {
            StartButton_OnClick(GameModeManager.GameMode.PvP);
        });
        exitButton.onClick.AddListener(() => { Application.Quit(); });
    }

    private void StartButton_OnClick(GameModeManager.GameMode mode)
    {
        GameModeManager.currentGameMode= mode;

        DontDestroyOnLoad(gameObject);
        StartCoroutine(LoadSceneAsync(SceneName.BATTLE_GAME_SCENE));
    }

    public void LoadBattleScene()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(LoadSceneAsync(SceneName.BATTLE_GAME_SCENE));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation ac = SceneManager.LoadSceneAsync(sceneName);
        slider.gameObject.SetActive(true);
        backGround.gameObject.SetActive(false);
        singleButton.gameObject.SetActive(false);
        pveButton.gameObject.SetActive(false);
        pvpButton.gameObject.SetActive(false);
        sliderImage.fillAmount = 0;
        exitButton.gameObject.SetActive(false);

        while (!ac.isDone)
        {
            sliderImage.fillAmount = ac.progress;
            yield return null;
        }
        slider.gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
