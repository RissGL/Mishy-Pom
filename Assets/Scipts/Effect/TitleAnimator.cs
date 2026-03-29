using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TitleAnimator : MonoBehaviour
{
    private void Start()
    {
        transform.localScale= Vector3.zero;

        Sequence seq=DOTween. Sequence();

        seq.Append(transform.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutBack));

        seq.Append(transform.DOScale(new Vector3(1.03f, 1.03f, 1.03f), 0.7f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo));
    }
}
