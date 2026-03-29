using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI :BasePanel
{
    [Header("按钮配置")]
    [SerializeField] private Button bgmAddVoiceButton;
    [SerializeField] private Button bgmReduceVoiceButton;
    [SerializeField] private Button sfxAddVoiceButton;
    [SerializeField] private Button sfxReduceVoiceButton;

    [SerializeField] private Button ExitButton;

    [SerializeField] private TextMeshProUGUI bgmVolume;
    [SerializeField] private TextMeshProUGUI sfxVolume;

    [SerializeField] private BasePanel basePanel;

    [Header("开始界面，用于选择")]
    [SerializeField] private StartScene startScene;

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
            Hide();
        } );

    }

    private void Start()
    {
        UpdateVisual();
        if (basePanel == null)
        {
            gameObject.SetActive(false);
        }
    }

    public override void Show() 
    {
        base.Show();
        UpdateVisual();
    }

    override public void Hide() 
    {
        base.Hide();
        startScene.SetSelectedButton();
    }

    private void UpdateVisual()
    {
        bgmVolume.text = Mathf.RoundToInt((BGMManager.Instance.GetVolume() * 10)).ToString();
        sfxVolume.text = Mathf.RoundToInt(SFXConfig.sfxVolume * 10).ToString();
    }
}
