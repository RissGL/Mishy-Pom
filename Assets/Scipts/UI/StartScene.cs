using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject backGround;
    [SerializeField] private GameObject slider;
    [SerializeField] private Image sliderImage;


    private void Awake()
    {
        slider.gameObject.SetActive(false);
        startButton.onClick.AddListener(StartButton_OnClick);
        exitButton.onClick.AddListener(() => { Application.Quit(); });
    }

    private void StartButton_OnClick()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(LoadSceneAsync(SceneName.BATTLE_GAME_SCENE));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation ac = SceneManager.LoadSceneAsync(sceneName);
        slider.gameObject.SetActive(true);
        backGround.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);
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
