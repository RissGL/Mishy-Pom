using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingScoreText : MonoBehaviour
{
    [SerializeField] private TextMeshPro scoreText;
    private float moveSpeed = 2.0f;
    private float disappearTimer=0.8f;
    private Color color;

    public void SetUp(int comboNum,int score) 
    {
        //scoreText.text = comboNum.ToString()+"Combo\n" +"+"+score.ToString()+"Score";
        scoreText.richText = true;
        scoreText.text = FormatScoreText( comboNum, score);

        color =scoreText.color;
        transform.position -= Vector3.forward;

        StartCoroutine(BounceRoutine());
    }

    private string FormatScoreText(int comboNum, int score)
    {
        string s = score.ToString();
        string c = comboNum.ToString();

        string cFirst = c[0].ToString();
        string cRest = c.Length > 1 ? c.Substring(1) : "";

        /*
        string sFirst = s[0].ToString();
        string sRest = s.Length > 1 ? s.Substring(1) : "";
        */

        return $"<size=150%><gradient=GoldGradient>{cFirst}</gradient></size>{cRest} Combo\n" +
               $"+<size=150%><gradient=GoldGradient>{s}</gradient></size>Score";
    }

    public void BadMishyScoreSetUp()
    {
        scoreText.text = $"<size=150%><gradient=GoldGradient>{100} </gradient></size>ScoreScore";

        color = scoreText.color;
        transform.position -= Vector3.forward;

        StartCoroutine(BounceRoutine());
    }

    private void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        disappearTimer-= Time.deltaTime;

        if (disappearTimer < 0) 
        {
            float fadeTimer = 3f;

            color.a-=fadeTimer* Time.deltaTime;
            scoreText.color=color;

            if (color.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator BounceRoutine()
    {
        float bounceTime = 0.15f; 
        float maxScale = 1.6f;    

        float startAngle = 20f;      
        float overshootAngle = -10f; 
        float endAngle = 0f;         

        transform.localScale = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0, 0, startAngle);

        float t = 0f;
        while (t < bounceTime)
        {
            t += Time.deltaTime;
            float normalizedTime = t / bounceTime;

            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * maxScale, normalizedTime);

            float currentAngle = Mathf.Lerp(startAngle, overshootAngle, normalizedTime);
            transform.localRotation = Quaternion.Euler(0, 0, currentAngle);

            yield return null;
        }

        transform.localScale = Vector3.one * maxScale;
        transform.localRotation = Quaternion.Euler(0, 0, overshootAngle);

        t = 0f;
        while (t < bounceTime)
        {
            t += Time.deltaTime;
            float normalizedTime = t / bounceTime;

            transform.localScale = Vector3.Lerp(Vector3.one * maxScale, Vector3.one, normalizedTime);

            float currentAngle = Mathf.Lerp(overshootAngle, endAngle, normalizedTime);
            transform.localRotation = Quaternion.Euler(0, 0, currentAngle);

            yield return null;
        }

        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
    }
}
