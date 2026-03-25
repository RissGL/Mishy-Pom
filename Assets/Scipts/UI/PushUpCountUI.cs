using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PushUpCountUI : MonoBehaviour
{
    [SerializeField] private PlayerBoard board;

    [SerializeField] private TextMeshProUGUI countUI;


    private void Start()
    {
        countUI.text = "0";
        board.mishyManager.OnTurnPushNumUpdate += Instance_OnTurnPushNumUpdate;
    }

    private void OnDestroy()
    {
        board.mishyManager.OnTurnPushNumUpdate -= Instance_OnTurnPushNumUpdate;
    }

    private void Instance_OnTurnPushNumUpdate(object sender, int e)
    {
        countUI.text=e.ToString();
    }
}
