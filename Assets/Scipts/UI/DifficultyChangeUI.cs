using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private GameObject Buttons;


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
            gameObject.SetActive(false);
            Buttons.SetActive(true);
        });

        gameObject.SetActive(false);
    }

    public void Show(StartScene startScene)
    {
        mainStartScene = startScene; 
        gameObject.SetActive(true);
        Buttons.SetActive(false);
    }

    private void SelectDifficulty(GameModeManager.Difficulty diff)
    {
        GameModeManager.difficulty =diff;

        gameObject.SetActive(false);

        mainStartScene.LoadBattleScene();
    }
}
