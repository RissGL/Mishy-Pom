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
        scoreText.text = comboNum.ToString()+"Combo\n" +"+"+score.ToString();

        color=scoreText.color;
        transform.position -= Vector3.forward;
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
}
