using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.LightweightPipeline;

[Serializable]
public class SettingsData
{
    private const string Key = "mirror_of_dusk_settings_data_v1";
    private static SettingsData.SettingsDataLoadFromCloudHandler _loadFromCloudHandler;
    private static SettingsData _data;
    public bool hasBootedUpGame;
    public int screenWidth;
    public int screenHeight;
    public int vSyncCount;
    public bool fullScreen;
    public float masterVolume;
    public float sFXVolume = -20f;
    public float musicVolume = -20f;
    public float ambienceVolume = -20f;
    public float voiceVolume = -20f;
    private static bool originalAudioValuesInitialized;
    private static float originalMasterVolume;
    private static float originalSFXVolume = -20f;
    private static float originalMusicVolume = -20f;
    private static float originalAmbienceVolume = -20f;
    private static float originalVoiceVolume = -20f;
    public bool canVibrate = false;
    public ButtonGuide buttonGuide;
    public ButtonGuide buttonGuide2;
    public int keyboardUserCount;
    public bool canAutoSave = false;
    public bool fpsDisplay = false;
    public float renderScaleCount = 1.0f;
    private LightweightRenderPipelineAsset renderPipelineAsset;
    public LanguageVoicePack languageVoicePack;
    public int language = -1;
    public bool canUploadReplays = false;
    public bool canSaveReplays = false;
    public int currentUserConfigProfile = 0;

    [SerializeField]
    private float brightness;

    public bool FpsDisplay
    {
        get
        {
            return fpsDisplay;
        }
        set
        {
            fpsDisplay = value;
            if (FPSDisplayHandler.Exists)
            {
                FPSDisplayHandler.Instance.gameObject.SetActive(fpsDisplay);
            }
        }
    }

    public delegate void SettingsDataLoadFromCloudHandler(bool success);

    //[CompilerGenerated]
    private static LoadCloudDataHandler f__m0;

    //[CompilerGenerated]
    private static SaveCloudDataHandler f__m1;

    public enum ButtonGuide
    {
        Keyboard_And_Controller,
        Controller_Only,
        Keyboard_Only
    }

    public enum LanguageVoicePack
    {
        None,
        SetA_EN,
        SETA_JP
    }

    public SettingsData()
    {
        this.screenWidth = Screen.currentResolution.width;
        this.screenHeight = Screen.currentResolution.height;
        this.fullScreen = Screen.fullScreen;
        this.vSyncCount = QualitySettings.vSyncCount;
        this.masterVolume = SettingsData.originalMasterVolume;
        this.sFXVolume = SettingsData.originalSFXVolume;
        this.musicVolume = SettingsData.originalMusicVolume;
        this.ambienceVolume = SettingsData.originalAmbienceVolume;
        this.voiceVolume = SettingsData.originalVoiceVolume;
        this.keyboardUserCount = 1;
        /*if (GraphicsSettings.renderPipelineAsset != null)
        {
            this.renderPipelineAsset = (LightweightRenderPipelineAsset)GraphicsSettings.renderPipelineAsset;
            this.renderPipelineAsset.renderScale = this.renderScaleCount;
            GraphicsSettings.renderPipelineAsset = this.renderPipelineAsset;
        }*/
        this.hasBootedUpGame = false;
        this.SetCameraEffectDefaults();
    }

    public static SettingsData Data
    {
        get
        {
            if (SettingsData._data == null)
            {
                if (!SettingsData.originalAudioValuesInitialized)
                {
                    SettingsData.originalAudioValuesInitialized = true;
                    SettingsData.originalMasterVolume = AudioManager.masterVolume;
                    SettingsData.originalSFXVolume = AudioManager.sfxOptionsVolume;
                    SettingsData.originalMusicVolume = AudioManager.bgmOptionsVolume;
                    SettingsData.originalAmbienceVolume = AudioManager.ambienceOptionsVolume;
                    SettingsData.originalVoiceVolume = AudioManager.voiceOptionsVolume;
                }
                if (SettingsData.hasKey())
                {
                    try
                    {
                        SettingsData._data = JsonUtility.FromJson<SettingsData>(PlayerPrefs.GetString("mirror_of_dusk_settings_data_v1"));
                    } catch (ArgumentException)
                    {
                        SettingsData._data = new SettingsData();
                        SettingsData.Save();
                    }
                } else
                {
                    SettingsData._data = new SettingsData();
                    SettingsData.Save();
                }
                if (SettingsData._data == null)
                {
                    return null;
                }
                SettingsData.ApplySettings();
            }
            return SettingsData._data;
        }
    }

    public static void Save()
    {
        string text = JsonUtility.ToJson(SettingsData._data);
        PlayerPrefs.SetString("mirror_of_dusk_settings_data_v1", text);
        PlayerPrefs.Save();
    }

    public static void LoadFromCloud(SettingsData.SettingsDataLoadFromCloudHandler handler)
    {
        SettingsData._loadFromCloudHandler = handler;
        if (OnlineManager.Instance.Interface.CloudStorageInitialized)
        {
            OnlineInterface @interface = OnlineManager.Instance.Interface;
            string[] keys = new string[]
            {
                "mirror_of_dusk_settings_data_v1"
            };
            if (SettingsData.f__m0 == null) {
                SettingsData.f__m0 = new LoadCloudDataHandler(SettingsData.OnLoadedCloudData);
            }
            @interface.LoadCloudData(keys, SettingsData.f__m0);
        }
    }

    public static void SaveToCloud()
    {
        if (OnlineManager.Instance.Interface.CloudStorageInitialized)
        {
            string value = JsonUtility.ToJson(SettingsData._data);
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary["mirror_of_dusk_settings_data_v1"] = value;
            OnlineInterface @interface = OnlineManager.Instance.Interface;
            IDictionary<string, string> data = dictionary;
            if (SettingsData.f__m1 == null)
            {
                SettingsData.f__m1 = new SaveCloudDataHandler(SettingsData.OnSavedCloudData);
            }
            @interface.SaveCloudData(data, SettingsData.f__m1);
        }
    }

    private static void OnSavedCloudData(bool success)
    {
        
    }

    private static void OnLoadedCloudData(string[] data, CloudLoadResult result)
    {
        if (result == CloudLoadResult.Failed)
        {
            SettingsData.LoadFromCloud(SettingsData._loadFromCloudHandler);
            return;
        }
        try
        {
            if (result == CloudLoadResult.NoData)
            {
                if (SettingsData.hasKey())
                {
                    try
                    {
                        SettingsData._data = JsonUtility.FromJson<SettingsData>(PlayerPrefs.GetString("mirror_of_dusk_settings_data_v1"));
                    } catch (ArgumentException)
                    {
                        SettingsData._data = new SettingsData();
                    }
                } else
                {
                    SettingsData._data = new SettingsData();
                }
                SettingsData.SaveToCloud();
            } else
            {
                UnityEngine.Debug.Log(data[0]);
                SettingsData._data = JsonUtility.FromJson<SettingsData>(data[0]);
            }
        } catch (ArgumentException)
        {

        }
        if (SettingsData._loadFromCloudHandler != null)
        {
            SettingsData._loadFromCloudHandler(true);
            SettingsData._loadFromCloudHandler = null;
        }
    }

    public static void Reset()
    {
        SettingsData._data = new SettingsData();
        SettingsData.Save();
    }

    public static void ApplySettings()
    {
        if (SettingsData.OnSettingsAppliedEvent != null)
        {
            SettingsData.OnSettingsAppliedEvent();
        }
        SettingsData.Save();
    }

    public static void ApplySettingsOnStartup()
    {
        if (Screen.width < 640 || Screen.height < 360)
        {
            SettingsData.Data.screenWidth = 640;
            SettingsData.Data.screenHeight = 360;
            SettingsData.Data.fullScreen = false;
            Screen.SetResolution(SettingsData.Data.screenWidth, SettingsData.Data.screenHeight, SettingsData.Data.fullScreen);
        }
        QualitySettings.vSyncCount = SettingsData.Data.vSyncCount;
        AudioManager.masterVolume = SettingsData.Data.masterVolume;
        AudioManager.sfxOptionsVolume = SettingsData.Data.sFXVolume;
        AudioManager.bgmOptionsVolume = SettingsData.Data.musicVolume;
        AudioManager.ambienceOptionsVolume = SettingsData.Data.ambienceVolume;
        AudioManager.voiceOptionsVolume = SettingsData.Data.voiceVolume;
    }

    private static bool hasKey()
    {
        return PlayerPrefs.HasKey("mirror_of_dusk_settings_data_v1");
    }

    //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public static event Action OnSettingsAppliedEvent;

    //ToDo: Create PlayerData
    public bool duskAudioEnabled
    {
        get
        {
            return false;
        }
    }

    private void SetCameraEffectDefaults()
    {
        this.brightness = 0f;
    }

    public float Brightness
    {
        get
        {
            this.ClampBrightness();
            return this.brightness;
        }
        set
        {
            this.brightness = value;
            this.ClampBrightness();
        }
    }

    private void ClampBrightness()
    {
        if (this.brightness < -1f)
        {
            this.brightness = -1f;
        }
        if (this.brightness > 1f)
        {
            this.brightness = 1f;
        }
    }
}
