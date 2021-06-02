using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManagerComponent : MonoBehaviour
{
    [SerializeField] private AudioManager.Channel channel;
    [SerializeField] private List<AudioManagerComponent.SoundGroup.Source> bgmSources;
    [SerializeField] private List<AudioManagerComponent.SoundGroup> sounds = new List<AudioManagerComponent.SoundGroup>();
    [SerializeField] private List<AudioManagerComponent.SoundGroup> bgmPlaylist = new List<AudioManagerComponent.SoundGroup>();
    [SerializeField] private bool autoplayBGM = true;
    [SerializeField] private bool autoplayBGMPlaylist = true;
    [SerializeField] private float[] minValue;
    private Dictionary<string, AudioManagerComponent.SoundGroup> dict;
    public static bool ShowAudioPlaying;
    public static bool ShowAudioVariations;

    [Serializable]
    public class SoundGroup
    {
        [SerializeField]
        private List<AudioManagerComponent.SoundGroup.Source> sources = new List<AudioManagerComponent.SoundGroup.Source>
        {
            new AudioManagerComponent.SoundGroup.Source()
        };

        //public Sfx trigger;
        public string key;
        private bool isPlaying;
        public Transform emissionTransform;
        public bool activatedManually;
        public bool isFadedOut;
        private float volume;

        [Serializable]
        public class Source
        {
            [SerializeField] internal AudioSource audio;
            public float originalVolume;
            public bool wasJustPlayed;
            public bool isFadedOut;
            public bool noLoop;

            internal void Init(bool initializeDeferrals)
            {
                if (initializeDeferrals)
                {
                    DeferredAudioSource component = this.audio.GetComponent<DeferredAudioSource>();
                    if (component != null)
                    {
                        component.Initialize();
                    }
                }
                if (this.audio != null && this.audio.clip != null)
                {
                    this.audio.ignoreListenerPause = true;
                    this.originalVolume = this.audio.volume;
                }
            }

            public void SetVolume(float v)
            {
                if (this.audio != null && this.audio.clip != null)
                {
                    this.audio.volume = v * this.originalVolume;
                }
            }

            public void Play()
            {
                if (this.audio != null && this.audio.clip != null)
                {
                    this.audio.PlayOneShot(this.audio.clip);
                }
            }

            public void PlayLooped()
            {
                if (this.audio != null && this.audio.clip != null)
                {
                    this.audio.loop = true;
                    this.audio.Play();
                }
            }

            public IEnumerator change_pitch_cr(float end, float time)
            {
                float t = 0f;
                if (this.audio != null && this.audio.clip != null)
                {
                    while (t < time)
                    {
                        float val = Mathf.Lerp(0f, 1f, t / time);
                        this.audio.pitch = Mathf.Lerp(this.audio.pitch, end, val);
                        t += Time.deltaTime;
                        yield return null;
                    }
                    this.audio.pitch = end;
                }
                yield break;
            }

            public IEnumerator change_volume_cr(float endVolume, float time, bool onFadeOut)
            {
                float t = 0f;
                float startVol = (!onFadeOut) ? 0f : this.audio.volume;
                float endVol = (!onFadeOut) ? this.audio.volume : endVolume;
                if (!onFadeOut)
                {
                    this.audio.Play();
                    this.isFadedOut = false;
                } else
                {
                    this.isFadedOut = true;
                }
                if (this.audio != null && this.audio.clip != null)
                {
                    while (t < time)
                    {
                        float val = Mathf.Lerp(0f, 1f, t / time);
                        this.audio.volume = Mathf.Lerp(startVol, endVol, val);
                        t += Time.deltaTime;
                        yield return null;
                    }
                    if (onFadeOut)
                    {
                        this.audio.Stop();
                        this.audio.volume = this.originalVolume;
                        this.isFadedOut = true;
                    }
                    this.audio.volume = endVol;
                }
                yield return null;
                yield break;
            }

            public IEnumerator warble_pitch_cr(int warbles, float[] minValue, float[] maxValue, float[] incrementAmount, float[] playTime)
            {
                bool isDecreasing = (bool)(UnityEngine.Random.Range(0, 2) == 1);
                /*if (UnityEngine.Random.Range(0, 2) == 1)
                {
                    isDecreasing = true;
                } else
                {
                    isDecreasing = false;
                }*/
                float t = 0f;
                float startPitch = 1f;
                if (this.audio != null && this.audio.clip != null)
                {
                    for (int i = 0; i < warbles; i++)
                    {
                        while (t < playTime[i])
                        {
                            t += Time.deltaTime;
                            if (isDecreasing)
                            {
                                if (this.audio.pitch > minValue[i])
                                {
                                    this.audio.pitch -= incrementAmount[i];
                                } else
                                {
                                    isDecreasing = false;
                                }
                            } else if (this.audio.pitch < maxValue[i])
                            {
                                this.audio.pitch += incrementAmount[i];
                            } else
                            {
                                isDecreasing = true;
                            }
                            yield return null;
                        }
                        t = 0f;
                        yield return null;
                    }
                    this.audio.pitch = startPitch;
                }
                yield break;
            }

            public void Stop()
            {
                if (this.audio != null && this.audio.clip != null)
                {
                    this.audio.loop = false;
                    this.audio.Stop();
                }
            }

            public void Pause()
            {
                if (this.audio != null && this.audio.clip != null)
                {
                    this.audio.Pause();
                }
            }

            public void UnPause()
            {
                if (this.audio != null && this.audio.clip != null)
                {
                    this.audio.UnPause();
                }
            }

            public void Pan(float pan)
            {
                if (this.audio != null && this.audio.clip != null)
                {
                    this.audio.panStereo = pan;
                }
            }

            public float ClipLength()
            {
                if (this.audio != null && this.audio.clip != null)
                {
                    return this.audio.clip.length;
                }
                Debug.LogError("Clip is null", null);
                return 0f;
            }

            public void FollowObject(Vector3 position)
            {
                if (this.audio != null && this.audio.clip != null && this.audio.isPlaying)
                {
                    this.audio.transform.position = position;
                }
            }

            public bool isPlaying()
            {
                return this.audio != null && this.audio.clip != null && this.audio.isPlaying;
            }

            public void OnAttenuate(bool attenuating, float volumeChange)
            {
                if (this.audio != null && this.audio.clip != null)
                {
                    if (attenuating)
                    {
                        this.audio.volume = volumeChange;
                    }
                    else
                    {
                        this.audio.volume = this.originalVolume;
                    }
                }
            }
        }

        internal void Init(bool initializeDeferrals = false)
        {
            this.key = this.key.ToLowerIfNecessary();
            for (int i = 0; i < this.sources.Count; i++)
            {
                if (this.sources[i].audio == null)
                {
                    this.sources.RemoveAt(i);
                    i--;
                }
            }
            foreach (AudioManagerComponent.SoundGroup.Source source in this.sources)
            {
                source.Init(initializeDeferrals);
            }
        }

        internal void SetMixerGroup(AudioMixerGroup group)
        {
            foreach (AudioManagerComponent.SoundGroup.Source source in this.sources)
            {
                if (source.audio != null && source.audio.outputAudioMixerGroup == null)
                {
                    source.audio.outputAudioMixerGroup = group;
                }
            }
        }

        private AudioManagerComponent.SoundGroup.Source GetSource()
        {
            return this.sources[UnityEngine.Random.Range(0, this.sources.Count)];
        }

        public void SetVolume(float v)
        {
            foreach (AudioManagerComponent.SoundGroup.Source source in this.sources)
            {
                source.SetVolume(v);
            }
        }

        public void Play()
        {
            AudioManagerComponent.SoundGroup.Source source = this.GetSource();
            if (this.sources.Count > 1)
            {
                foreach (AudioManagerComponent.SoundGroup.Source source2 in this.sources)
                {
                    if (!source.wasJustPlayed)
                    {
                        break;
                    }
                    source = this.GetSource();
                }
            }
            source.wasJustPlayed = true;
            source.Play();
            foreach (AudioManagerComponent.SoundGroup.Source source3 in this.sources)
            {
                if (source3 != source)
                {
                    source3.wasJustPlayed = false;
                }
            }
        }

        public void PlayLoop()
        {
            this.GetSource().PlayLooped();
        }

        public void Pan(float pan)
        {
            foreach (AudioManagerComponent.SoundGroup.Source source in this.sources)
            {
                source.Pan(pan);
            }
        }

        public void FollowObject(Vector3 position)
        {
            foreach (AudioManagerComponent.SoundGroup.Source source in this.sources)
            {
                source.FollowObject(position);
            }
        }

        public bool CheckIfPlaying()
        {
            this.isPlaying = false;
            foreach (AudioManagerComponent.SoundGroup.Source source in this.sources)
            {
                source.isPlaying();
                if (source.isPlaying())
                {
                    this.isPlaying = true;
                }
            }
            return this.isPlaying;
        }

        public float ClipLength()
        {
            float result = 0f;
            foreach (AudioManagerComponent.SoundGroup.Source source in this.sources)
            {
                result = source.ClipLength();
            }
            return result;
        }

        public void OnAttenuate(bool attentuating, float endVolume)
        {
            foreach (AudioManagerComponent.SoundGroup.Source source in this.sources)
            {
                source.OnAttenuate(attentuating, endVolume);
            }
        }

        public IEnumerator warble_pitch_cr(int warbles, float[] minValue, float[] maxValue, float[] incrementAmount, float[] playTime)
        {
            bool isDecreasing = (bool)(UnityEngine.Random.Range(0, 2) == 1);
            /*if (UnityEngine.Random.Range(0, 2) == 1)
            {
                isDecreasing = true;
            } else
            {
                isDecreasing = false;
            }*/
            float t = 0f;
            float startPitch = 1f;
            foreach (AudioManagerComponent.SoundGroup.Source s in this.sources)
            {
                if (s != null && s.audio.clip != null)
                {
                    for (int i = 0; i < warbles; i++)
                    {
                        while (t < playTime[i])
                        {
                            t += Time.deltaTime;
                            if (isDecreasing)
                            {
                                if (s.audio.pitch > minValue[i])
                                {
                                    s.audio.pitch -= incrementAmount[i];
                                }
                                else
                                {
                                    isDecreasing = false;
                                }
                            }
                            else if (s.audio.pitch < maxValue[i])
                            {
                                s.audio.pitch += incrementAmount[i];
                            }
                            else
                            {
                                isDecreasing = true;
                            }
                            yield return null;
                        }
                        t = 0f;
                        yield return null;
                    }
                    s.audio.pitch = startPitch;
                }
            }
            yield break;
        }

        public IEnumerator change_pitch_sfx(float end, float time)
        {
            foreach (AudioManagerComponent.SoundGroup.Source s in this.sources)
            {
                float t = 0f;
                if (s != null && s.audio.clip != null)
                {
                    while (t < time)
                    {
                        float val = Mathf.Lerp(0f, 1f, t / time);
                        s.audio.pitch = Mathf.Lerp(s.audio.pitch, end, val);
                        t += Time.deltaTime;
                        yield return null;
                    }
                    s.audio.pitch = end;
                }
                yield return null;
            }
            yield break;
        }

        public IEnumerator change_volume_sfx(float end, float time)
        {
            foreach (AudioManagerComponent.SoundGroup.Source s in this.sources)
            {
                float t = 0f;
                if (s != null && s.audio.clip != null)
                {
                    while (t < time)
                    {
                        float val = Mathf.Lerp(0f, 1f, t / time);
                        s.audio.volume = Mathf.Lerp(s.audio.volume, end, val);
                        t += Time.deltaTime;
                        yield return null;
                    }
                    s.audio.volume = end;
                    if (end == 0f)
                    {
                        s.audio.Stop();
                    }
                }
                yield return null;
            }
            yield break;
        }

        public IEnumerator change_volume_cr(float endVolume, float time, bool onFadeOut)
        {
            foreach (AudioManagerComponent.SoundGroup.Source s in this.sources)
            {
                float t = 0f;
                float startVol = (!onFadeOut) ? 0f : s.audio.volume;
                float endVol = (!onFadeOut) ? s.audio.volume : endVolume;
                if (!onFadeOut)
                {
                    s.audio.Play();
                    this.isFadedOut = false;
                }
                else
                {
                    this.isFadedOut = true;
                }
                if (s.audio != null && s.audio.clip != null)
                {
                    while (t < time)
                    {
                        float val = Mathf.Lerp(0f, 1f, t / time);
                        s.audio.volume = Mathf.Lerp(startVol, endVol, val);
                        t += Time.deltaTime;
                        yield return null;
                    }
                    if (onFadeOut)
                    {
                        s.audio.Stop();
                        s.audio.volume = s.originalVolume;
                        this.isFadedOut = true;
                    }
                }
                yield return null;
            }
            yield break;
        }

        public void Stop()
        {
            foreach (AudioManagerComponent.SoundGroup.Source source in this.sources)
            {
                source.Stop();
            }
        }
        
        public void Pause()
        {
            foreach (AudioManagerComponent.SoundGroup.Source source in this.sources)
            {
                source.Pause();
            }
        }
        
        public void Unpause()
        {
            foreach (AudioManagerComponent.SoundGroup.Source source in this.sources)
            {
                source.UnPause();
            }
        }
    }

    protected void Awake()
    {
        base.useGUILayout = false;
        this.SetChannels();
        this.dict = new Dictionary<string, SoundGroup>();
        foreach(AudioManagerComponent.SoundGroup soundGroup in this.sounds)
        {
            soundGroup.Init(true);
            this.dict[soundGroup.key.ToLowerIfNecessary()] = soundGroup;
        }
        foreach (AudioManagerComponent.SoundGroup soundGroup2 in this.bgmPlaylist)
        {
            soundGroup2.Init(true);
            this.dict[soundGroup2.key.ToLowerIfNecessary()] = soundGroup2;
        }
        foreach (AudioManagerComponent.SoundGroup.Source source in this.bgmSources)
        {
            source.Init(true);
        }
        this.AddEvents();
    }

    private void OnDestroy()
    {
        this.RemoveEvents();
    }

    private void OnValidate()
    {
        foreach (AudioManagerComponent.SoundGroup soundGroup in this.sounds)
        {
            /*if (string.IsNullOrEmpty(soundGroup.key))
            {
                soundGroup.key = soundGroup.trigger.ToString();
            }*/
            soundGroup.key = soundGroup.key.ToLower();
        }
        foreach (AudioManagerComponent.SoundGroup soundGroup2 in this.bgmPlaylist)
        {
            /*if (string.IsNullOrEmpty(soundGroup2.key))
            {
                soundGroup2.key = soundGroup2.trigger.ToString();
            }*/
            soundGroup2.key = soundGroup2.key.ToLower();
        }
    }

    private void AddEvents()
    {
        //ToDo: Create Events for Delegates
        
        AudioManager.OnPlayBGMEvent += this.StartBGM;
        /*
        AudioManager.OnPlayBGMPlaylistEvent += this.StartBGMPlaylist;
        AudioManager.OnSnapshotEvent += this.SnapshotTransition;
        AudioManager.OnCheckEvent.Add(new Predicate<string>(this.OnIsPlaying));*/
        AudioManager.OnPlayEvent += this.OnPlay;
        /*AudioManager.OnPlayLoopEvent += this.OnPlayLoop;
        AudioManager.OnStopEvent += this.OnStop;
        AudioManager.OnPauseEvent += this.OnPause;
        AudioManager.OnUnpauseEvent += this.OnUnpause;
        AudioManager.OnFollowObject += this.OnFollowOject;
        AudioManager.OnStopAllEvent += this.OnStopAll;
        AudioManager.OnStopBGMEvent += this.OnStopBGM;
        AudioManager.OnPauseAllSFXEvent += this.OnPauseAllSFX;
        AudioManager.OnUnpauseAllSFXEvent += this.OnUnpauseAllSFX;
        AudioManager.OnBGMSlowdown += this.OnBGMSlowdown;
        AudioManager.OnSFXSlowDown += this.OnSFXSlowDown;
        AudioManager.OnSFXFadeVolume += this.OnSFXVolume;
        AudioManager.OnBGMPitchWarble += this.OnBGMWarblePitch;
        AudioManager.OnAttenuation += this.OnAttenuation;
        AudioManager.OnPlayManualBGM += this.PlayManualBGMTrack;
        AudioManager.OnStopManualBGMTrackEvent += this.StopManualBGMTrack;
        AudioManager.OnBGMFadeVolume += this.OnBGMVolumeFade;
        if (this.autoplayBGM)
        {
            SceneLoader.OnLoaderCompleteEvent += this.StartBGM;
        }
        if (this.autoplayBGMPlaylist)
        {
            SceneLoader.OnLoaderCompleteEvent += this.StartBGMPlaylist;
        }*/
    }

    private void RemoveEvents()
    {
        AudioManager.OnPlayBGMEvent -= this.StartBGM;
        AudioManager.OnPlayEvent -= this.OnPlay;
    }

    private void Update()
    {
        for (int i = 0; i < this.sounds.Count; i++)
        {
            if (this.sounds[i].emissionTransform != null)
            {
                this.sounds[i].FollowObject(this.sounds[i].emissionTransform.position);
            }
        }
        /*foreach(AudioManagerComponent.SoundGroup soundGroup in this.sounds)
        {
            if (soundGroup.emissionTransform != null)
            {
                soundGroup.FollowObject(soundGroup.emissionTransform.position);
            }
        }*/
    }

    private void StartBGM()
    {
        //ToDo: Add StopBGM()
        for (int i = 0; i < this.bgmSources.Count; i++)
        {
            if (this.bgmSources[i].noLoop)
            {
                this.bgmSources[i].Play();
            } else
            {
                this.bgmSources[i].PlayLooped();
            }
        }
    }

    private void OnPlay(string key)
    {
        if (this.dict.ContainsKey(key))
        {
            if (AudioManagerComponent.ShowAudioVariations || AudioManagerComponent.ShowAudioPlaying)
            {
            }
            this.dict[key].Play();
        }
    }

    private void SetChannels()
    {
        AudioManagerMixer.Groups groups = AudioManagerMixer.GetGroups();
        AudioManager.Channel channel = this.channel;
        AudioMixerGroup audioMixerGroup;
        AudioMixerGroup mixerGroup;
        if (channel == AudioManager.Channel.Default || channel != AudioManager.Channel.Level)
        {
            audioMixerGroup = groups.bgm;
            mixerGroup = groups.sfx;
        }
        else
        {
            audioMixerGroup = groups.stageBgm;
            mixerGroup = groups.stageSfx;
        }
        foreach (AudioManagerComponent.SoundGroup.Source source in this.bgmSources)
        {
            if (source.audio.outputAudioMixerGroup == null)
            {
                source.audio.outputAudioMixerGroup = audioMixerGroup;
            }
        }
        foreach (AudioManagerComponent.SoundGroup soundGroup in this.sounds)
        {
            soundGroup.SetMixerGroup(mixerGroup);
        }
        foreach (AudioManagerComponent.SoundGroup soundGroup2 in this.bgmPlaylist)
        {
            soundGroup2.SetMixerGroup(audioMixerGroup);
        }
    }
}
