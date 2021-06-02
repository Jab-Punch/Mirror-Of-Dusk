using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

[Serializable]
public class BattleSettingsData
{
    private const string Key = "mirror_of_dusk_battlesettings_data_v1";
    private static BattleSettingsData.BattleSettingsDataLoadFromCloudHandler _loadFromCloudHandler;
    private static BattleSettingsData _data;
    public bool hasBootedUpGame;
    public bool initialized = false;
    public Dictionary<BattleMode, BattleModeSettings> battleModeSettings;
    
    [Serializable]
    public class BattleModeSettings
    {
        public int modeStyle = 0;
        public int modeReflection = 0;
        public int defaultStockCount = 1;
        public int timerSetting = 0;
        public int defaultInitialHealth = 3000;
        public int defaultInitialShards = 50;
        public int defaultTotalShards = 200;
        public int defaultShardStrength = 150;
        public float damageRatio = 1.0f;
        public float barrierRatio = 1.0f;

        public BattleModeSettings()
        {
            this.modeStyle = 0;
            this.modeReflection = 0;
            this.defaultStockCount = 1;
            this.timerSetting = 0;
            this.defaultInitialHealth = 3000;
            this.defaultInitialShards = 50;
            this.defaultTotalShards = 200;
            this.defaultShardStrength = 150;
            this.damageRatio = 1.0f;
            this.barrierRatio = 1.0f;
        }
    }

    public delegate void BattleSettingsDataLoadFromCloudHandler(bool success);
    private static LoadCloudDataHandler f__m0;
    private static SaveCloudDataHandler f__m1;

    public BattleSettingsData()
    {
        this.battleModeSettings = new Dictionary<BattleMode, BattleModeSettings>()
        {
            {BattleMode.Training, new BattleModeSettings()},
            {BattleMode.Arcade, new BattleModeSettings()},
            {BattleMode.Story, new BattleModeSettings()},
            {BattleMode.Multiplayer, new BattleModeSettings()},
            {BattleMode.Online, new BattleModeSettings()}
        };
        this.hasBootedUpGame = false;
        this.initialized = true;
    }

    public static BattleSettingsData Data
    {
        get
        {
            if (BattleSettingsData._data == null)
            {
                if (BattleSettingsData.hasKey())
                {
                    try
                    {
                        BattleSettingsData._data = JsonUtility.FromJson<BattleSettingsData>(PlayerPrefs.GetString("mirror_of_dusk_battlesettings_data_v1"));
                    }
                    catch (ArgumentException)
                    {
                        BattleSettingsData._data = new BattleSettingsData();
                        BattleSettingsData.Save();
                    }
                }
                else
                {
                    BattleSettingsData._data = new BattleSettingsData();
                    BattleSettingsData.Save();
                }
                if (BattleSettingsData._data == null)
                {
                    return null;
                }
                BattleSettingsData.ApplySettings();
            }
            return BattleSettingsData._data;
        }
    }

    public static void Save()
    {
        string text = JsonUtility.ToJson(BattleSettingsData._data);
        PlayerPrefs.SetString("mirror_of_dusk_battlesettings_data_v1", text);
        PlayerPrefs.Save();
    }

    public static void LoadFromCloud(BattleSettingsData.BattleSettingsDataLoadFromCloudHandler handler)
    {
        BattleSettingsData._loadFromCloudHandler = handler;
        if (OnlineManager.Instance.Interface.CloudStorageInitialized)
        {
            OnlineInterface @interface = OnlineManager.Instance.Interface;
            string[] keys = new string[]
            {
                "mirror_of_dusk_battlesettings_data_v1"
            };
            if (BattleSettingsData.f__m0 == null)
            {
                BattleSettingsData.f__m0 = new LoadCloudDataHandler(BattleSettingsData.OnLoadedCloudData);
            }
            @interface.LoadCloudData(keys, BattleSettingsData.f__m0);
        }
    }

    public static void SaveToCloud()
    {
        if (OnlineManager.Instance.Interface.CloudStorageInitialized)
        {
            string value = JsonUtility.ToJson(BattleSettingsData._data);
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary["mirror_of_dusk_battlesettings_data_v1"] = value;
            OnlineInterface @interface = OnlineManager.Instance.Interface;
            IDictionary<string, string> data = dictionary;
            if (BattleSettingsData.f__m1 == null)
            {
                BattleSettingsData.f__m1 = new SaveCloudDataHandler(BattleSettingsData.OnSavedCloudData);
            }
            @interface.SaveCloudData(data, BattleSettingsData.f__m1);
        }
    }

    private static void OnSavedCloudData(bool success)
    {

    }

    private static void OnLoadedCloudData(string[] data, CloudLoadResult result)
    {
        if (result == CloudLoadResult.Failed)
        {
            BattleSettingsData.LoadFromCloud(BattleSettingsData._loadFromCloudHandler);
            return;
        }
        try
        {
            if (result == CloudLoadResult.NoData)
            {
                if (BattleSettingsData.hasKey())
                {
                    try
                    {
                        BattleSettingsData._data = JsonUtility.FromJson<BattleSettingsData>(PlayerPrefs.GetString("mirror_of_dusk_battlesettings_data_v1"));
                    }
                    catch (ArgumentException)
                    {
                        BattleSettingsData._data = new BattleSettingsData();
                    }
                }
                else
                {
                    BattleSettingsData._data = new BattleSettingsData();
                }
                BattleSettingsData.SaveToCloud();
            }
            else
            {
                //UnityEngine.Debug.Log(data[0]);
                BattleSettingsData._data = JsonUtility.FromJson<BattleSettingsData>(data[0]);
            }
        }
        catch (ArgumentException)
        {

        }
        if (BattleSettingsData._loadFromCloudHandler != null)
        {
            BattleSettingsData._loadFromCloudHandler(true);
            BattleSettingsData._loadFromCloudHandler = null;
        }
    }

    public static void Reset()
    {
        BattleSettingsData._data = new BattleSettingsData();
        BattleSettingsData.Save();
    }

    public static void ApplySettings()
    {
        if (BattleSettingsData.OnSettingsAppliedEvent != null)
        {
            BattleSettingsData.OnSettingsAppliedEvent();
        }
        BattleSettingsData.Save();
    }

    public static void ApplySettingsOnStartup()
    {
        
    }

    private static bool hasKey()
    {
        return PlayerPrefs.HasKey("mirror_of_dusk_battlesettings_data_v1");
    }

    public static event Action OnSettingsAppliedEvent;


}
