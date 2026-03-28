using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SFXConfig
{
    public static float sfxVolume
    {
        get; private set;
    } = 1.0f;

    public static void AddVoice() 
    {
        sfxVolume += 0.1f;
        if (sfxVolume > 1.0)
        {
            sfxVolume = 1.0f;
        }
    }

    public static void ReduceVoice()
    {
        sfxVolume -= 0.1f;
        if (sfxVolume < 0.0f)
        {
            sfxVolume = 0.0f;
        }
    }
}
