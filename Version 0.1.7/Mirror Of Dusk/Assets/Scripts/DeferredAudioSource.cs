using System;
using UnityEngine;

public class DeferredAudioSource : MonoBehaviour
{
    public string audioClipName;

    public void Initialize()
    {
        base.GetComponent<AudioSource>().clip = AssetLoader<AudioClip>.GetCachedAsset(this.audioClipName);
    }
}
