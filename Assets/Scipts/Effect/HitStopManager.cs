using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStopManager : MonoBehaviour
{
    public static HitStopManager Instance { get; private set; }
    private bool isHitStopActive = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void TriggerHitStop(float duration = 0.08f)
    {
        if (Time.timeScale == 0f) 
        {
            return;
        }

        StopAllCoroutines();

        StartCoroutine(HitStopRoutine(duration));
    }

    /// <summary>
    /// ∂Ÿ÷°
    /// </summary>
    /// <param name="duration">Õ£∂Ÿ ±º‰</param>
    /// <returns></returns>
    private IEnumerator HitStopRoutine(float duration=0.08f)
    {
        isHitStopActive = true;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        isHitStopActive = false;
    }
}
