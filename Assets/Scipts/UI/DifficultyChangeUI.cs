using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DifficultyChangeUI : MonoBehaviour
{
    [Header("难度按钮")]
    [SerializeField] private Button easyButton;
    [SerializeField] private Button veryEasyButton;
    [SerializeField] private Button normalButton;
    [SerializeField] private Button hardButton;
    [SerializeField] private Button nightMareButton;
    [SerializeField] private Button abyssButton;
    [SerializeField] private Button exitButton;

    private StartScene mainStartScene;

    [Header("开始界面按钮父物体")]
    [SerializeField] private GameObject startButtons;


    private void Start()
    {
        easyButton.onClick.AddListener(() => 
        {
            SelectDifficulty(GameModeManager.Difficulty.Easy);
        });
        veryEasyButton.onClick.AddListener(() => 
        {
            SelectDifficulty(GameModeManager.Difficulty.VerryEasy);
        });
        normalButton.onClick.AddListener(() =>
        {
            SelectDifficulty(GameModeManager.Difficulty.Normal);
        });
        hardButton.onClick.AddListener(() =>
        {
            SelectDifficulty(GameModeManager.Difficulty.Hard);
        });
        nightMareButton.onClick.AddListener(() =>
        {
            SelectDifficulty(GameModeManager.Difficulty.NightMare);
        });
        abyssButton.onClick.AddListener(() =>
        {
            SelectDifficulty(GameModeManager.Difficulty.Abyss);
        }); 
        exitButton.onClick.AddListener(() =>
        {
            //gameObject.SetActive(false);
            //startButtons.SetActive(true);
            Hide();
        });

        gameObject.SetActive(false);
    }

    public void Show(StartScene startScene)
    {
        mainStartScene = startScene; 
        gameObject.SetActive(true);

        float transScale = 1.02f;

        transform.DOKill();

        //出现动画
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one,0.6f).SetEase(Ease.OutBack)
            .OnComplete(() => 
            {
                transform.DOScale(new Vector3(transScale, transScale, transScale), 0.4f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1,LoopType.Yoyo);
            });

        startButtons.gameObject.SetActive(false);
    }

    public void Hide() 
    {
        startButtons.gameObject.SetActive(true);

        mainStartScene.ShowStartSceneButtons(false);

        gameObject.SetActive(false);

    }

    private void SelectDifficulty(GameModeManager.Difficulty diff)
    {
        GameModeManager.difficulty =diff;

        gameObject.SetActive(false);

        mainStartScene.LoadBattleScene();
    }
}
