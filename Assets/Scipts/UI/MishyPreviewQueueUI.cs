using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MishyPreviewQueueUI : MonoBehaviour
{
    [SerializeField] private MishyPreviewQueue previewQueue;

    private void Start()
    {
        previewQueue.OnNextMishyNeedSpawn += PreviewQueue_OnNextMishyNeedSpawn;
        previewQueue.OnNextMishyEequeue += PreviewQueue_OnNextMishyDequeue;
        previewQueue.OnPreviewQueueInit += PreviewQueue_OnPreviewQueueInit;
    }

    private void OnDestroy()
    {
        previewQueue.OnNextMishyNeedSpawn -= PreviewQueue_OnNextMishyNeedSpawn;
        previewQueue.OnNextMishyEequeue -= PreviewQueue_OnNextMishyDequeue;
        previewQueue.OnPreviewQueueInit -= PreviewQueue_OnPreviewQueueInit;
    }

    private void PreviewQueue_OnPreviewQueueInit(object sender, System.EventArgs e)
    {
    }

    private void PreviewQueue_OnNextMishyDequeue(object sender, Queue<MishyType> e)
    {
    }

    private void PreviewQueue_OnNextMishyNeedSpawn(object sender, MishyPreviewQueue.MishyPairEventArgs e)
    {
    }
}
