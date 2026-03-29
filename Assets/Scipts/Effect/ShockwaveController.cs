using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveController : MonoBehaviour
{
    [Header("冲击波配置")]
    [SerializeField] private float expandDuration = 0.5f;   // 扩散时间
    [SerializeField] private float maxRadius = 1f;          // 最大半径
    [SerializeField] private float startDistortion = 0.1f;  // 初始最大扭曲力度

    private Material shockwaveMat;

    private void Start()
    {
        transform.localScale = new Vector3(8f, 8f, 1f); 

        Renderer renderer = GetComponent<Renderer>();

        if (renderer != null)
        {
            shockwaveMat = renderer.material;
            shockwaveMat.SetFloat("_DistortionStrength", startDistortion);

            shockwaveMat.DOFloat(maxRadius, "_Radius", expandDuration).SetEase(Ease.OutQuad);

            shockwaveMat.DOFloat(0f, "_DistortionStrength", expandDuration).SetEase(Ease.OutCubic)
                .OnComplete(() => 
                {
                    Destroy(gameObject);
                });

        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (shockwaveMat != null)
        {
            shockwaveMat.DOKill();
        }
    }
}
