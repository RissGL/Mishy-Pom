using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    [Header("key")]
    [SerializeField] private string key;

    private TextMeshProUGUI textMeshProUGUI;

    private void Awake()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        RefreshText();
        LocalizationManager.OnLanguageChanged += LocalizationManager_OnLanguageChanged;
    }

    private void LocalizationManager_OnLanguageChanged(object sender, System.EventArgs e)
    {
        RefreshText();
    }

    private void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= LocalizationManager_OnLanguageChanged;
    }

    private void RefreshText() 
    {
        if (textMeshProUGUI != null && !string.IsNullOrEmpty(key))
        {
            textMeshProUGUI .text = LocalizationManager.Instance.GetText(key);
            textMeshProUGUI.font= LocalizationManager.Instance.GetCurrentFont();
        }
    }
}
