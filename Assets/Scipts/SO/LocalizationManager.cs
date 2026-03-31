using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    [Header("ÓïÑÔSOÎÄ¼þ")]
    public List<LanguageDataSO> languageDataList;



    private LanguageDataSO currentLanguageData;

    public static event EventHandler OnLanguageChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(this);

        int savedLangIndex = PlayerPrefs.GetInt("SavedLanguage", 0);
        SetLanguageByIndex(savedLangIndex);
    }

    public void SetLanguage(LanguageDataSO.LanguageType languageType)
    {
        for (int i = 0; i < languageDataList.Count; i++) 
        {
            if (languageType == languageDataList[i].languageType)
            {
                currentLanguageData = languageDataList[i];
                currentLanguageData.Init();

                PlayerPrefs.SetInt("SavedLanguage", i);
                PlayerPrefs.Save();

                OnLanguageChanged?.Invoke(this, EventArgs.Empty);
                return;
            }
        }
    }

    private void SetLanguageByIndex(int index)
    {
        if (index >= 0 && index < languageDataList.Count)
        {
            SetLanguage(languageDataList[index].languageType);
        }
    }

    public string GetText(string key)
    {
        if (currentLanguageData == null) return $"[{key}]";
        return currentLanguageData.GetValue(key);
    }

    public TMP_FontAsset GetCurrentFont()
    {
        if (currentLanguageData == null) return null;
        return currentLanguageData.languageFont;
    }
}
