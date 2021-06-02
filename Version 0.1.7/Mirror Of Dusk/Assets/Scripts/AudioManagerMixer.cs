using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManagerMixer : MonoBehaviour
{
    private const string PATH = "Audio/AudioMixer";
    private static AudioManagerMixer Manager;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioManagerMixer.Groups audioGroups;

    private static void Init()
    {
        if (AudioManagerMixer.Manager == null)
        {
            AudioManagerMixer.Manager = Resources.Load<AudioManagerMixer>("Audio/AudioMixer");
        }
    }

    public static AudioMixer GetMixer()
    {
        AudioManagerMixer.Init();
        return AudioManagerMixer.Manager.mixer;
    }

    public static AudioManagerMixer.Groups GetGroups()
    {
        AudioManagerMixer.Init();
        return AudioManagerMixer.Manager.audioGroups;
    }

    [Serializable]
    public class Groups
    {
        [SerializeField] private AudioMixerGroup _master;
        [SerializeField] private AudioMixerGroup _master_Options;
        [SerializeField] private AudioMixerGroup _bgm_Options;
        [SerializeField] private AudioMixerGroup _sfx_Options;
        [SerializeField] private AudioMixerGroup _ambience_Options;
        [SerializeField] private AudioMixerGroup _voice_Options;

        [Space(10f)]
        [Header("BGM")]
        [SerializeField]
        private AudioMixerGroup _bgm;

        [SerializeField]
        private AudioMixerGroup _stageBgm;

        [SerializeField]
        private AudioMixerGroup _musicSting;

        [Space(10f)]
        [Header("SFX")]
        [SerializeField]
        private AudioMixerGroup _sfx;

        [SerializeField]
        private AudioMixerGroup _stageSfx;

        [SerializeField]
        private AudioMixerGroup _ambience;

        [SerializeField]
        private AudioMixerGroup _voice;

        public AudioMixerGroup master
        {
            get
            {
                return this._master;
            }
        }

        public AudioMixerGroup master_Options
        {
            get
            {
                return this._master_Options;
            }
        }

        public AudioMixerGroup bgm_Options
        {
            get
            {
                return this._bgm_Options;
            }
        }

        public AudioMixerGroup sfx_Options
        {
            get
            {
                return this._sfx_Options;
            }
        }

        public AudioMixerGroup ambience_Options
        {
            get
            {
                return this._ambience_Options;
            }
        }

        public AudioMixerGroup voice_Options
        {
            get
            {
                return this._voice_Options;
            }
        }

        public AudioMixerGroup bgm
        {
            get
            {
                return this._bgm;
            }
        }

        public AudioMixerGroup stageBgm
        {
            get
            {
                return this._stageBgm;
            }
        }

        public AudioMixerGroup musicSting
        {
            get
            {
                return this._musicSting;
            }
        }

        public AudioMixerGroup sfx
        {
            get
            {
                return this._sfx;
            }
        }

        public AudioMixerGroup stageSfx
        {
            get
            {
                return this._stageSfx;
            }
        }

        public AudioMixerGroup ambience
        {
            get
            {
                return this._ambience;
            }
        }

        public AudioMixerGroup voice
        {
            get
            {
                return this._voice;
            }
        }
    }
}
