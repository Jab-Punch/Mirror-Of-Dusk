using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public Dictionary<string, AudioClip> soundFile { get; private set; }
    private AudioSource audioSource;

    [System.Serializable]
    public class SFX
    {
        public string name;
        public AudioClip sound;
    }

    public SFX[] sfx;

    void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        soundFile = new Dictionary<string, AudioClip>();
        for (int i = 0; i < sfx.Length; i++)
        {
            soundFile.Add(sfx[i].name, sfx[i].sound);
        }
    }

    public void PlaySound(string name)
    {
        try {
            audioSource.PlayOneShot(soundFile[name], 1.0f);
        } catch (System.NullReferenceException)
        {
            Debug.LogWarning("could not find sound: " + name);
        }
    }
}
