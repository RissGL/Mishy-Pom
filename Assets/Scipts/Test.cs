using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private PlayerBoard board;
    [SerializeField] private MishyPreviewQueue m_Queue;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            board.mishyManager.MishyColumnUp(2);
        }
    }

    
}
