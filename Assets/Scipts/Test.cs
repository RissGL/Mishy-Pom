using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private MishyPreviewQueue m_Queue;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            m_Queue.DequeueNextMishy();
        }
    }
}
