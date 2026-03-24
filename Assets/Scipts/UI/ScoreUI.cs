using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private int screenScore = 0;
    private int trueScore;

    private Coroutine rollingCoroutine;

    private void Start()
    {
        scoreText.text = screenScore.ToString();
    }

    private void OnEnable()
    {
        ScoreSystem.OnUpdateScore += ScoreSystem_OnUpdateScore;
    }

    private void OnDisable()
    {
        ScoreSystem.OnUpdateScore -= ScoreSystem_OnUpdateScore;
    }

    private void ScoreSystem_OnUpdateScore(object sender, int e)
    {
        trueScore = e;

        if (rollingCoroutine!=null) 
        {
            StopCoroutine(rollingCoroutine);
        }

        rollingCoroutine=StartCoroutine(ScoreRolling());
    }

    private IEnumerator ScoreRolling()
    {
        int startScore = screenScore;
        float duration = 0.3f+(trueScore-screenScore)/60f; // 总时长
        float elapsed = 0f;    // 已消耗时间

        while (elapsed < duration) 
        {
            elapsed += Time.deltaTime;

            float t=elapsed/duration;

            screenScore = Mathf.RoundToInt(Mathf.Lerp(startScore,trueScore, t));
            scoreText.text = screenScore.ToString();

            yield return null;
        }

        screenScore=trueScore;
        scoreText.text = screenScore.ToString();
        rollingCoroutine =null;
    }
}
