using System;
using UnityEngine;

public class AudioNoiseHandler : MonoBehaviour
{
    private static AudioNoiseHandler noiseHandler;
    private const string PATH = "Audio/AudioNoiseHandler";

    public static AudioNoiseHandler Instance
    {
        get
        {
            if (AudioNoiseHandler.noiseHandler == null)
            {
                AudioNoiseHandler audioNoiseHandler = UnityEngine.Object.Instantiate(Resources.Load("Audio/AudioNoiseHandler")) as AudioNoiseHandler;
                audioNoiseHandler.name = "NoiseHandler";
            }
            return AudioNoiseHandler.noiseHandler;
        }
    }

    private void Awake()
    {
        base.useGUILayout = false;
        AudioNoiseHandler.noiseHandler = this;
        base.GetComponent<AudioSource>().ignoreListenerPause = true;
        UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
    }
    
    public void ConfirmSound()
    {
        AudioManager.Play("confirm1");
    }

    public void EnterGameSound()
    {
        AudioManager.Play("enter_game");
    }
}
