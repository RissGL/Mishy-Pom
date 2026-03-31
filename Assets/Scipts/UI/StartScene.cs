using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    [Header("按钮")]
    [SerializeField] private Button pvpButton;
    [SerializeField] private Button pveButton;
    [SerializeField] private Button singleButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button turiouristPanelButton;


    [Header("按钮父节点")]
    [SerializeField] private Transform startSceneButtons;

    [Header("UI")]
    [SerializeField] private GameObject backGround;
    [SerializeField] private GameObject slider;
    [SerializeField] private Image sliderImage;

    [SerializeField] private DifficultyChangeUI difficultyChangeUI;
    [SerializeField] private SettingUI settingUI;
    [SerializeField] private TuriouristPanel turiouristPanel;


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
        settingButton.onClick.AddListener(() => 
        {
            settingUI.Show();
        });

        turiouristPanelButton.onClick.AddListener(() => 
        {
            turiouristPanel.Show();
        });
        exitButton.onClick.AddListener(() => { Application.Quit(); });

        ShowStartSceneButtons();
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
        settingButton.gameObject.SetActive(false);
        sliderImage.fillAmount = 0;
        exitButton.gameObject.SetActive(false);
        turiouristPanelButton.gameObject.SetActive(false);

        while (!ac.isDone)
        {
            sliderImage.fillAmount = ac.progress;
            yield return null;
        }
        slider.gameObject.SetActive(false);
        Destroy(gameObject);
    }

    public void ShowStartSceneButtons(bool withDelay=true) 
    {
        startSceneButtons.transform.DOKill();

        startSceneButtons.transform.localScale = Vector3.zero;

        float delayTime = withDelay ? 0.4f : 0f;
        startSceneButtons.DOKill();

        startSceneButtons.transform.DOScale
            (Vector3.one, 0.6f)
            .SetEase(Ease.OutBack).SetDelay(delayTime)
            .OnComplete(() => 
            {
               startSceneButtons.DOScale
           (new Vector3(1.02f, 1.02f, 1.02f), 0.4f)
           .SetEase(Ease.InOutSine)
           .SetLoops(-1, LoopType.Yoyo);

                SetSelectedButton();
            });
       
    }

    public void SetSelectedButton() 
    {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(pveButton.gameObject);
    }
}
