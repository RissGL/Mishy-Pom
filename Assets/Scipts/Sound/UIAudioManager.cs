using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager Instance { get; private set; }
    private AudioSource audioSource;

    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip buttonHoverClipSound;
    [SerializeField] private AudioClip buttonExitClipSound;

    [SerializeField] private AudioClip panelOpenClipSound;
    [SerializeField] private AudioClip panelCloseClipSound;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject); 
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    private void Start()
    {
        UIButtonJuice.OnButtonPressed += UIButtonJuice_OnButtonPressed;
        UIButtonJuice.OnButtonHover += UIButtonJuice_OnButtonHover;
        UIButtonJuice.OnButtonExit += UIButtonJuice_OnButtonExit;

        BasePanel.OnPanelShow += BasePanel_OnPanelOpen;
        BasePanel.OnPanelHide += BasePanel_OnPanelClose;

    }

    private void OnDestroy()
    {
        UIButtonJuice.OnButtonPressed -= UIButtonJuice_OnButtonPressed;
        UIButtonJuice.OnButtonHover -= UIButtonJuice_OnButtonHover;
        UIButtonJuice.OnButtonExit -= UIButtonJuice_OnButtonExit;

        BasePanel.OnPanelShow -= BasePanel_OnPanelOpen;
        BasePanel.OnPanelHide -= BasePanel_OnPanelClose;
    }

    private void BasePanel_OnPanelClose(object sender, EventArgs e)
    {
        audioSource.pitch = UnityEngine.Random.Range(0.95f, 1f);

        audioSource.PlayOneShot(panelCloseClipSound,SFXConfig.sfxVolume);
    }

    private void BasePanel_OnPanelOpen(object sender, EventArgs e)
    {
        audioSource.pitch = UnityEngine. Random.Range(1f, 1.05f);
        audioSource.PlayOneShot(panelOpenClipSound, SFXConfig.sfxVolume);
    }

    private void UIButtonJuice_OnButtonExit(object sender, EventArgs e)
    {
        audioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);

        audioSource.PlayOneShot(buttonExitClipSound, SFXConfig.sfxVolume);
    }

    private void UIButtonJuice_OnButtonHover(object sender, EventArgs e)
    {
        audioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);

        audioSource.PlayOneShot(buttonHoverClipSound,1.5f*SFXConfig.sfxVolume);
    }

    private void UIButtonJuice_OnButtonPressed(object sender, System.EventArgs e)
    {
        audioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);

        audioSource.PlayOneShot(buttonClickSound, SFXConfig.sfxVolume);
    }


}
