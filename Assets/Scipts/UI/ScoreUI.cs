using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private PlayerBoard board;

    private int screenScore = 0;
    private int trueScore;

    private Coroutine rollingCoroutine;

    private void Start()
    {
        scoreText.text = FormatScoreText(screenScore);
    }

    private void OnEnable()
    {
        board.scoreSystem.OnUpdateScore += ScoreSystem_OnUpdateScore;
    }

    private void OnDisable()
    {
        board.scoreSystem.OnUpdateScore -= ScoreSystem_OnUpdateScore;
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
    private string FormatScoreText(int currentScore)
    {
        string s = currentScore.ToString();

        if (s.Length >= 3)
        {
            return $"<size=135>{s[0]}</size>{s.Substring(1)}";
        }
        return s;
    }


    private IEnumerator ScoreRolling()
    {
        int startScore = screenScore;
        float duration = 0.3f+(trueScore-screenScore)/80f; // 总时长
        float elapsed = 0f;    // 已消耗时间
        int lastUpdatedScore = -1;

        while (elapsed < duration) 
        {
            elapsed += Time.deltaTime;

            float t=elapsed/duration;

            screenScore = Mathf.RoundToInt(Mathf.Lerp(startScore,trueScore, t));

            if (screenScore != lastUpdatedScore)
            {
                scoreText.text = FormatScoreText(screenScore);
            }
            yield return null;
        }

        screenScore=trueScore;
        scoreText.text = FormatScoreText(screenScore);
        rollingCoroutine =null;
    }
}
