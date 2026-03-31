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

    [SerializeField] private GameObject Setting;

    [Header("按钮配置")]
    [SerializeField] private Button bgmAddVoiceButton;
    [SerializeField] private Button bgmReduceVoiceButton;
    [SerializeField] private Button sfxAddVoiceButton;
    [SerializeField] private Button sfxReduceVoiceButton;


    [SerializeField] private TextMeshProUGUI bgmVolume;
    [SerializeField] private TextMeshProUGUI sfxVolume;

    private void Start()
    {

        bgmAddVoiceButton.onClick.AddListener(() =>
        {
            BGMManager.Instance.AddVolume(0.1f);
            UpdateVisual();
        });
        bgmReduceVoiceButton.onClick.AddListener(() =>
        {
            BGMManager.Instance.AddVolume(-0.1f);
            UpdateVisual();
        });
        sfxAddVoiceButton.onClick.AddListener(() =>
        {
            SFXConfig.AddVoice();
            UpdateVisual();
        });
        sfxReduceVoiceButton.onClick.AddListener(() =>
        {
            SFXConfig.ReduceVoice();
            UpdateVisual();
        });


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
        gameObject.SetActive(false);
        slider.gameObject.SetActive(false);
        UpdateVisual();

    }

    public void Show(float playTime) 
    {
        if (PlayerManager.CurrentState == PlayerManager.GameState.GameOver)
        {
            Debug.LogWarning("游戏已经结束");
            return;
        }

        gamePlayerTime.text = "Play Time: " + $"<size=130%><gradient=Green>" +
    $"{(Mathf.RoundToInt(playTime).ToString())}" + " </size></gradient>s";
        gameObject.SetActive(true);
        slider.gameObject.SetActive(false);
        Setting.SetActive(true);


        base.Show();
        UpdateVisual();
    }

    public void Hide() 
    {
        Setting.SetActive(false);
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

    private void UpdateVisual()
    {
        bgmVolume.text = Mathf.RoundToInt((BGMManager.Instance.GetVolume() * 10)).ToString();
        sfxVolume.text = Mathf.RoundToInt(SFXConfig.sfxVolume * 10).ToString();
    }
}
