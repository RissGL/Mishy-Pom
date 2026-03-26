using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager instance { get; private set; }

    private Camera mainCamera;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;

        mainCamera = Camera.main;
    }

    /// <summary>
    /// 轻微震动 (适用于：快速下落砸地)
    /// </summary>
    public void ShakeLight()
    {
        mainCamera.DOComplete();

        // 持续时间，震动强度，震动频率
        mainCamera.DOShakePosition(0.1f, 0.2f, 15);
    }

    /// <summary>
    /// 中等震动 
    /// </summary>
    public void ShakeMedium()
    {
        mainCamera.DOComplete();
        mainCamera.DOShakePosition(0.2f, 0.4f, 25);
    }

    /// <summary>
    /// 剧烈震动 
    /// </summary>
    public void ShakeHeavy()
    {
        mainCamera.DOComplete();
        mainCamera.DOShakePosition(0.4f, 0.8f, 40);
    }
}
