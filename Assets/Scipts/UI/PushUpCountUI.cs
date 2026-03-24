using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PushUpCountUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countUI;

    private void Start()
    {
        countUI.text = "0";
        MishyManager.Instance.OnTurnPushNumUpdate += Instance_OnTurnPushNumUpdate;
    }

    private void OnDestroy()
    {
        MishyManager.Instance.OnTurnPushNumUpdate -= Instance_OnTurnPushNumUpdate;
    }

    private void Instance_OnTurnPushNumUpdate(object sender, int e)
    {
        countUI.text=e.ToString();
    }
}
