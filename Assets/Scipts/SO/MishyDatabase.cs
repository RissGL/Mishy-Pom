using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MishyDatabase;

[CreateAssetMenu(fileName = "NewMishyDatabase", menuName = "Config/Mishy Database")]
public class MishyDatabase : ScriptableObject
{
    [System.Serializable]
    public struct MishyConfig 
    {
        public MishyType type;
        public GameObject mishyPrefab;
        public string mishyName;
        public Color debugColor;
        public Sprite mishySprite;
        public Color mishyColor;
    }
    public List<MishyConfig> mishyConfigs;
    public GameObject GetPrefab(MishyType type)
    {
        var config = mishyConfigs.Find(c => c.type == type);
        if (config.mishyPrefab != null) return config.mishyPrefab;

        Debug.LogError($"未找到 {type} 对应的预制体！");
        return null;
    }

    public Sprite GetSprite(MishyType type)
    {
        var config = mishyConfigs.Find(c => c.type == type);
        if (config.mishyPrefab != null) return config.mishySprite;

        Debug.LogError($"未找到 {type} 对应的精灵图！");
        return null;
    }

    public Color GetColor(MishyType type)
    {
        var config = mishyConfigs.Find(c => c.type == type);
        if (config.mishyColor.a == 0) return Color.white;
        return config.mishyColor;
    }
}
