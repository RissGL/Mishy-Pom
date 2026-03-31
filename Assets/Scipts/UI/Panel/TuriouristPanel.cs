using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TuriouristPanel : BasePanel
{
    [SerializeField] private Button CloseButton;

    private void Awake()
    {
        CloseButton.onClick.AddListener(() =>
        {
            Hide();
        });
        gameObject.SetActive(false);
    }
}
