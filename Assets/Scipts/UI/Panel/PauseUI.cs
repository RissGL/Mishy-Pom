using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PauseUI : BasePanel
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button reStartButton;
    [SerializeField] private Button endButton;

    [SerializeField] private TextMeshProUGUI gamePlayerTime;

    [SerializeField] private Image sliderImage;
    [SerializeField] private GameObject slider;

    [Header("设置菜单")]
    [SerializeField] private SettingUI settingUI;

    private void Start()
    {
        reStartButton.onClick.AddListener(() => 
        {
            StartCoroutine(LoadSceneAsync(SceneName.BATTLE_GAME_SCENE));
            Time.timeScale = 1f;//恢复时间
        });

        endButton.onClick.AddListener(() =>
        {
            StartCoroutine(LoadSceneAsync(SceneName.START_SCENE));
            Time.timeScale = 1f;//恢复时间
        });
        settingUI.gameObject.SetActive(false);
        gameObject.SetActive(false);
        slider.gameObject.SetActive(false);
    }

    public void Show(float playTime) 
    {
        settingUI.gameObject.SetActive(true);
        gamePlayerTime.text = "Play Time: " + $"<size=130%><gradient=Green>" +
    $"{(Mathf.RoundToInt(playTime).ToString())}" + " </size></gradient>s";
        gameObject.SetActive(true);
        slider.gameObject.SetActive(false);
 
        base.Show();
        settingUI.Show();
    }

    public void Hide() 
    {
        settingUI.gameObject.SetActive(false);
        base.Hide();
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        DontDestroyOnLoad(gameObject);
        AsyncOperation ac = SceneManager.LoadSceneAsync(sceneName);
        slider.gameObject.SetActive(true);
        titleText.gameObject.SetActive(false);
        gamePlayerTime.gameObject.SetActive(false);
        reStartButton.gameObject.SetActive(false);
        endButton.gameObject.SetActive(false);
        sliderImage.fillAmount = 0;

        while (!ac.isDone)
        {
            sliderImage.fillAmount = ac.progress;
            yield return null;
        }

        Destroy(gameObject);
    }
}
