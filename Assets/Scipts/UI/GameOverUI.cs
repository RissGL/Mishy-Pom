using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button reStartButton;
    [SerializeField] private Button endButton;

    [SerializeField] private TextMeshProUGUI gamePlayerTime;

    [SerializeField] private Image sliderImage;
    [SerializeField] private Image blackBg;
    [SerializeField] private GameObject slider;


    private void Start()
    {
        reStartButton.onClick.AddListener(() => 
        {
            StartCoroutine(LoadSceneAsync(SceneName.BATTLE_GAME_SCENE));
        });

        endButton.onClick.AddListener(() =>
        {
            StartCoroutine(LoadSceneAsync(SceneName.START_SCENE));
        });
        Hide();
    }

    public void Show(string winnerName,float time) 
    {
        titleText.text =winnerName+"  Win!";
        gamePlayerTime.text = "Play Time:" + (Mathf.RoundToInt(time).ToString()) + "s";

        gameObject.SetActive(true);
    }

    public void Hide() 
    {
        gameObject.SetActive(false);
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
        blackBg.gameObject.SetActive(true);

        while (!ac.isDone)
        {
            sliderImage.fillAmount = ac.progress;
            yield return null;
        }

        Destroy(gameObject);
    }
}
