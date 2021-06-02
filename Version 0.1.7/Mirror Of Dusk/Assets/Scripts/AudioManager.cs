using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using GCFreeUtils;
using UnityEngine;
using UnityEngine.Audio;

public static class AudioManager
{
    public enum Channel
    {
        Default,
        Level
    }

    public enum Property
    {
        MasterVolume,
        Options_BGMVolume,
        Options_SFXVolume,
        Options_AmbienceVolume,
        Options_VoiceVolume
    }

    public enum Snapshots
    {
        Cutscene,
        FrontEnd,
        Loadscreen,
        Unpaused,
        Paused
    }

    private const float VOLUME_MAX = 0f;
    private const float VOLUME_MIN = -80f;
    public static GCFreePredicateList<string> OnCheckEvent = new GCFreePredicateList<string>(10, true);
    public static AudioMixer _mixer;
    private static bool checkIfPlaying;

    public delegate bool OnCheckIfPlaying(string key);
    public delegate void OnSfxHandler(string key);
    public delegate void OnTransformHandler(string key, Transform transform);
    public delegate void OnAttenuautionHandler(string key, bool attenuation, float endVolume);
    public delegate void OnChangeBGMHandler(float end, float time);
    public delegate void OnChangeBGMVolumeHandler(float end, float time, bool fadeOut);
    public delegate void OnChangeSFXHandler(string key, float end, float time);
    public delegate void OnWarbleBGMPitchHandler(int warbles, float[] minValue, float[] maxValue, float[] warbleTime, float[] playTime);
    public delegate void OnSnapshotHandler(string[] names, float[] weight, float time);
    public delegate void OnBGMPlayListManualHandler(bool loopPlayListAfter);

    public static event AudioManager.OnAttenuautionHandler OnAttenuation;
    public static event AudioManager.OnTransformHandler OnFollowObject;
    public static event AudioManager.OnSnapshotHandler OnSnapshotEvent;
    public static event AudioManager.OnChangeBGMHandler OnBGMSlowdown;
    public static event AudioManager.OnChangeBGMVolumeHandler OnBGMFadeVolume;
    public static event AudioManager.OnChangeSFXHandler OnSFXSlowdown;
    public static event AudioManager.OnChangeSFXHandler OnSFXFadeVolume;
    public static event AudioManager.OnWarbleBGMPitchHandler OnBGMPitchWarble;
    public static event AudioManager.OnBGMPlayListManualHandler OnPlayManualBGM;
    public static event AudioManager.OnSfxHandler OnPlayEvent;
    public static event AudioManager.OnSfxHandler OnPlayLoopEvent;
    public static event AudioManager.OnSfxHandler OnStopEvent;
    public static event AudioManager.OnSfxHandler OnPauseEvent;
    public static event AudioManager.OnSfxHandler OnUnpauseEvent;
    public static event Action OnStopAllEvent;
    public static event Action OnStopBGMEvent;
    public static event Action OnPlayBGMEvent;
    public static event Action OnPlayBGMPlaylistEvent;
    public static event Action OnPauseAllSFXEvent;
    public static event Action OnUnpauseAllSFXEvent;
    public static event Action OnStopManualBGMTrackEvent;

    private static AudioMixer mixer
    {
        get
        {
            if (AudioManager._mixer == null)
            {
                AudioManager._mixer = AudioManagerMixer.GetMixer();
            }
            return AudioManager._mixer;
        }
    }

    public static float voiceOptionsVolume
    {
        get
        {
            float result;
            AudioManager.mixer.GetFloat(AudioManager.Property.Options_VoiceVolume.ToString(), out result);
            return result;
        }
        set
        {
            AudioManager.mixer.SetFloat(AudioManager.Property.Options_VoiceVolume.ToString(), value);
        }
    }

    public static float ambienceOptionsVolume
    {
        get
        {
            float result;
            AudioManager.mixer.GetFloat(AudioManager.Property.Options_AmbienceVolume.ToString(), out result);
            return result;
        }
        set
        {
            AudioManager.mixer.SetFloat(AudioManager.Property.Options_AmbienceVolume.ToString(), value);
        }
    }

    public static float sfxOptionsVolume
    {
        get
        {
            float result;
            AudioManager.mixer.GetFloat(AudioManager.Property.Options_SFXVolume.ToString(), out result);
            return result;
        }
        set
        {
            AudioManager.mixer.SetFloat(AudioManager.Property.Options_SFXVolume.ToString(), value);
        }
    }

    public static float bgmOptionsVolume
    {
        get
        {
            float result;
            AudioManager.mixer.GetFloat(AudioManager.Property.Options_BGMVolume.ToString(), out result);
            return result;
        }
        set
        {
            AudioManager.mixer.SetFloat(AudioManager.Property.Options_BGMVolume.ToString(), value);
        }
    }

    public static float masterVolume
    {
        get
        {
            float result;
            AudioManager.mixer.GetFloat(AudioManager.Property.MasterVolume.ToString(), out result);
            return result;
        }
        set
        {
            AudioManager.mixer.SetFloat(AudioManager.Property.MasterVolume.ToString(), value);
        }
    }

    public static bool CheckIfPlaying(string key)
    {
        AudioManager.checkIfPlaying = false;
        if (AudioManager.OnCheckEvent != null)
        {
            key = key.ToLowerIfNecessary();
            AudioManager.checkIfPlaying = AudioManager.OnCheckEvent.CallAnyTrue(key);
            return AudioManager.checkIfPlaying;
        }
        return false;
    }

    public static void PlayBGMPlaylistManually(bool goThroughPlaylistAfter)
    {
        if (AudioManager.OnPlayManualBGM != null)
        {
            AudioManager.OnPlayManualBGM(goThroughPlaylistAfter);
        }
    }

    public static void StopBGMPlaylistManually()
    {
        if (AudioManager.OnStopManualBGMTrackEvent != null)
        {
            AudioManager.OnStopManualBGMTrackEvent();
        }
    }

    public static void ChangeSFXPitch(string key, float endPitch, float time)
    {
        if (AudioManager.OnSFXSlowdown != null)
        {
            AudioManager.OnSFXSlowdown(key, endPitch, time);
        }
    }

    public static void ChangeBGMPitch(float endPitch, float time)
    {
        if (AudioManager.OnBGMSlowdown != null)
        {
            AudioManager.OnBGMSlowdown(endPitch, time);
        }
    }

    public static void FadeBGMVolume(float endVolume, float time, bool fadeOut)
    {
        if (AudioManager.OnBGMFadeVolume != null)
        {
            AudioManager.OnBGMFadeVolume(endVolume, time, fadeOut);
        }
    }

    public static void WarbleBGMPitch(int warbles, float[] minValue, float[] maxValue, float[] incrementTime, float[] playTime)
    {
        if (AudioManager.OnBGMPitchWarble != null)
        {
            AudioManager.OnBGMPitchWarble(warbles, minValue, maxValue, incrementTime, playTime);
        }
    }

    public static void Attenuation(string key, bool attenuation, float endVolume)
    {
        if (AudioManager.OnAttenuation != null)
        {
            AudioManager.OnAttenuation(key, attenuation, endVolume);
        }
    }

    public static void Play(string key)
    {
        key = key.ToLowerIfNecessary();
        if (AudioManager.OnPlayEvent != null)
        {
            AudioManager.OnPlayEvent(key);
        }
    }

    public static void Stop(string key)
    {
        key = key.ToLowerIfNecessary();
        if (AudioManager.OnStopEvent != null)
        {
            AudioManager.OnStopEvent(key);
        }
    }

    public static void PlayLoop(string key)
    {
        key = key.ToLowerIfNecessary();
        if (AudioManager.OnPlayLoopEvent != null)
        {
            AudioManager.OnPlayLoopEvent(key);
        }
    }

    public static void Pause(string key)
    {
        key = key.ToLowerIfNecessary();
        if (AudioManager.OnPauseEvent != null)
        {
            AudioManager.OnPauseEvent(key);
        }
    }

    public static void Unpaused(string key)
    {
        key = key.ToLowerIfNecessary();
        if (AudioManager.OnUnpauseEvent != null)
        {
            AudioManager.OnUnpauseEvent(key);
        }
    }

    public static void FadeSFXVolume(string key, float endVolume, float time)
    {
        if (AudioManager.OnSFXFadeVolume != null)
        {
            AudioManager.OnSFXFadeVolume(key, endVolume, time);
        }
    }

    public static void FollowObject(IEnumerable<string> keys, Transform transform)
    {
        foreach (string text in keys)
        {
            AudioManager.FollowObject(keys, transform);
        }
    }

    public static void FollowObject(string key, Transform transform)
    {
        key = key.ToLowerIfNecessary();
        if (AudioManager.OnFollowObject != null)
        {
            AudioManager.OnFollowObject(key, transform);
        }
    }

    //ToDo: Create Sfx
    /*
    [Obsolete("Use Play(string key) instead")]
    public static void Play(Sfx sfx)
    {
        AudioManager.Play(sfx.ToString());
    }

    [Obsolete("Use Stop(string key) instead")]
    public static void Stop(Sfx sfx)
    {
        AudioManager.Stop(sfx.ToString());
    }
    */

    public static void StopAll()
    {
        if (AudioManager.OnStopAllEvent != null)
        {
            AudioManager.OnStopAllEvent();
        }
    }

    public static void StopBGM()
    {
        if (AudioManager.OnStopBGMEvent != null)
        {
            AudioManager.OnStopBGMEvent();
        }
    }

    public static void PlayBGM()
    {
        if (AudioManager.OnPlayBGMEvent != null)
        {
            AudioManager.OnPlayBGMEvent();
        }
    }

    public static void PlaylistBGM()
    {
        if (AudioManager.OnPlayBGMPlaylistEvent != null)
        {
            AudioManager.OnPlayBGMPlaylistEvent();
        }
    }

    public static void PauseAllSFX()
    {
        if (AudioManager.OnPauseAllSFXEvent != null)
        {
            AudioManager.OnPauseAllSFXEvent();
        }
    }

    public static void UnpauseAllSFX()
    {
        if (AudioManager.OnUnpauseAllSFXEvent != null)
        {
            AudioManager.OnUnpauseAllSFXEvent();
        }
    }

    public static void SnapshotTransition(string[] snapshotNames, float[] weights, float time)
    {
        if (AudioManager.OnSnapshotEvent != null)
        {
            AudioManager.OnSnapshotEvent(snapshotNames, weights, time);
        }
    }

    public static void HandleSnapshot(string snapshot, float time)
    {
        string[] array = new string[]
        {
            AudioManager.Snapshots.Cutscene.ToString(),
            AudioManager.Snapshots.FrontEnd.ToString(),
            AudioManager.Snapshots.Loadscreen.ToString(),
            AudioManager.Snapshots.Unpaused.ToString(),
            AudioManager.Snapshots.Paused.ToString()
        };
        float[] array2 = new float[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            array2[i] = ((!(array[i] == snapshot)) ? 0f : 1f);
        }
        AudioManager.SnapshotTransition(array, array2, time);
    }

    public static void SnapshotReset(string sceneName, float time)
    {
        string[] array = new string[]
        {
            AudioManager.Snapshots.Cutscene.ToString(),
            AudioManager.Snapshots.Unpaused.ToString(),
            AudioManager.Snapshots.Paused.ToString()
        };
        int num = 0;
        for (int i = 0; i < array.Length; i++)
        {
            if (SettingsData.Data.duskAudioEnabled)
            {
                if (array[i] == AudioManager.Snapshots.Unpaused.ToString())
                {
                    num = i;
                }
            }
            else if (array[i] == AudioManager.Snapshots.Unpaused.ToString())
            {
                num = i;
            }
        }
        float[] array2 = new float[array.Length];
        for (int j = 0; j < array.Length; j++)
        {
            array2[j] = ((j != num) ? 0f : 1f);
        }
        AudioManager.SnapshotTransition(array, array2, time);
    }
}
