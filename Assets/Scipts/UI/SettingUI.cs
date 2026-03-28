using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [Header("°´Å¥ÅäÖÃ")]
    [SerializeField] private Button bgmAddVoiceButton;
    [SerializeField] private Button bgmReduceVoiceButton;
    [SerializeField] private Button sfxAddVoiceButton;
    [SerializeField] private Button sfxReduceVoiceButton;

    [SerializeField] private Button ExitButton;

    [SerializeField] private TextMeshProUGUI bgmVolume;
    [SerializeField] private TextMeshProUGUI sfxVolume;

    private void Awake()
    {
        bgmAddVoiceButton.onClick.AddListener(() =>
        {
            BGMManager.Instance.AddVolume(0.1f);
            UpdateVisual();
        });
        bgmReduceVoiceButton.onClick.AddListener(() =>
        {
            BGMManager.Instance.AddVolume(-0.1f);
            UpdateVisual();
        });
        sfxAddVoiceButton.onClick.AddListener(() => 
        {
            SFXConfig.AddVoice();
            UpdateVisual();
        });
        sfxReduceVoiceButton.onClick.AddListener(() => 
        {
            SFXConfig.ReduceVoice();
            UpdateVisual();
        });
        ExitButton.onClick.AddListener(() => 
        {
            gameObject.SetActive(false);
        } );
    }

    private void Start()
    {
        UpdateVisual();
        gameObject.SetActive(false); 
    }

    public void Show() 
    {
        UpdateVisual();
        gameObject.SetActive(true);
    }

    private void UpdateVisual()
    {
        bgmVolume.text = Mathf.RoundToInt((BGMManager.Instance.GetVolume() * 10)).ToString();
        sfxVolume.text = Mathf.RoundToInt(SFXConfig.sfxVolume * 10).ToString();
    }
}
