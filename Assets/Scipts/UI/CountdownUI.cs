using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountdownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;

    public event EventHandler OnCountdownOver;
    public static event EventHandler OnCountdownStart;

    public void StartCountdown()
    {
        gameObject.SetActive(true);
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        OnCountdownStart?.Invoke(this, EventArgs.Empty);

        string[] steps = { "3", "2", "1", "GO!" };

        foreach (string step in steps)
        {
            countdownText.text = step;

            countdownText.transform.localScale = Vector3.zero;
            countdownText.transform.localRotation = Quaternion.Euler(0, 0, -45); 
            countdownText.alpha = 0f;

            Sequence seq = DOTween.Sequence();

            seq.Append(countdownText.transform.DOScale(1.5f, 0.4f).SetEase(Ease.OutBack));
            seq.Join(countdownText.transform.DORotate(Vector3.zero, 0.4f).SetEase(Ease.OutBack));
            seq.Join(DOTween.To(() => countdownText.alpha, x => countdownText.alpha = x, 1f, 0.3f));

            seq.AppendInterval(0.2f);

            seq.Append(countdownText.transform.DOScale(2.5f, 0.2f));
            seq.Join(DOTween.To(() => countdownText.alpha, x => countdownText.alpha = x, 0f, 0.2f));

            yield return seq.WaitForCompletion();
        }

        OnCountdownOver?.Invoke(this, EventArgs.Empty);
        gameObject.SetActive(false);
    }

}
