using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "LanguageData", menuName = "Config/LanguageData")]
public class LanguageDataSO : ScriptableObject
{

    [Header("”Ô—‘∂‘”¶◊÷ÃÂ")]
    public TMP_FontAsset languageFont;

    public enum LanguageType
    {
        Chinese,
        English,
        Japanese,
    }

    public LanguageType languageType;

    public List<Translation> translations;

    [System.Serializable]
    public struct Translation
    {
        public string key;
        public string value;
    }

    public Dictionary<string, string> dict;

    public void Init()
    {
        dict=new Dictionary<string, string>();

        foreach (var trans in translations) 
        {
            if (!dict.ContainsKey(trans.key))
            {
                dict.Add(trans.key, trans.value);
            }
        }

    }

    public string GetValue(string key) 
    {
        if (dict==null)
        {
            Init();
        }

        return dict.TryGetValue(key,out string result)?result : $"[{key}]";
    }

}
