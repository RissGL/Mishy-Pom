using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance { get; private set; }
    private AudioSource audioSource;

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
        audioSource.playOnAwake=true;
        audioSource.loop=true;
    }

    public void SetVolume(float volume) 
    {
        audioSource.volume=volume;
    }

    public void AddVolume(float addVolume)
    {
        audioSource.volume = Mathf.Clamp01(audioSource.volume +addVolume);
    }

    public float GetVolume() 
    {
        return audioSource.volume;
    }
}
