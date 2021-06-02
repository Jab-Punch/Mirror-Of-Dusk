using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
//using Steamworks;
using UnityEngine;

public class OnlineInterfaceSteam : OnlineInterface
{
    private SteamManager steamManager;

    private string SavePath
    {
        get
        {
            if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library/Application Support/unity.JAB-Punch_Games.MirrorOfDusk/MirrorOfDusk/");
            }
            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MirrorOfDusk\\");
            }
            return String.Empty;
        }
    }

    
    public event SignInEventHandler OnUserSignedIn;
    public event SignOutEventHandler OnUserSignedOut;

    public OnlineUser MainUser
    {
        get { return null; }
    }

    public OnlineUser SecondaryUser
    {
        get { return null; }
    }

    public OnlineUser ThirdUser
    {
        get { return null; }
    }

    public OnlineUser FourthUser
    {
        get { return null; }
    }

    public bool CloudStorageInitialized
    {
        get { return true; }
    }

    public bool SupportsMultipleUsers
    {
        get { return false; }
    }

    public bool SupportsUserSignIn
    {
        get { return false; }
    }

    public void Init()
    {
        this.steamManager = new GameObject("SteamManager").AddComponent<SteamManager>();
        this.steamManager.transform.SetParent(MirrorOfDusk.Current.transform);
        if (!SteamManager.Initialized)
        {
            return;
        }
        //SteamUserStats.RequestCurrentStats();
    }

    public void Reset() { }

    public void SignInUser(bool silent, PlayerId player, ulong controllerId)
    {
        this.OnUserSignedIn(null);
    }

    public void SwitchUser(PlayerId player, ulong controllerId) { }

    public OnlineUser GetUserForController(ulong id)
    {
        return null;
    }

    public List<ulong> GetControllersForUser(PlayerId player)
    {
        return null;
    }
    
    public bool IsUserSignedIn(PlayerId player)
    {
        return false;
    }

    public OnlineUser GetUser(PlayerId player)
    {
        return null;
    }
    
    public void SetUser(PlayerId player, OnlineUser user) { }
    
    public Texture2D GetProfilePic(PlayerId player)
    {
        return null;
    }

    //public void GetAchievement(PlayerId player, string id, AchievementEventHandler achievementRetrievedHandler) { }

    /*public void UnlockAchievement(PlayerId player, string id)
    {
        if (!SteamManager.Initialized)
        {
            return;
        }
        bool flag;
        SteamUserStats.GetAchievement(id, out flag);
        if (!flag)
        {
            SteamUserStats.SetAchievement(id);
            SteamUserStats.StoreStats();
        }
    }

    public void SyncAchievementsAndStats()
    {
        if (!SteamManager.Initialized)
        {
            return;
        }
        SteamUserStats.StoreStats();
    }
    
    public void SetStat(PlayerId player, string id, int value)
    {
        if (!SteamManager.Initialized)
        {
            return;
        }
        SteamUserStats.SetStat(id, value);
    }
    
    public void SetStat(PlayerId player, string id, float value)
    {
        if (!SteamManager.Initialized)
        {
            return;
        }
        SteamUserStats.SetStat(id, value);
    }*/
    
    public void SetStat(PlayerId player, string id, string value) { }

    /*public void IncrementStat(PlayerId player, string id, int value)
    {
        if (!SteamManager.Initialized)
        {
            return;
        }
        int num;
        SteamUserStats.GetStat(id, out num);
        int num2 = num + value;
        SteamUserStats.SetStat(id, num2);
        if (id == "Parries" && (num2 == 20 || num2 == 100))
        {
            SteamUserStats.StoreStats();
        }
    }*/

    public void SetRichPresence(PlayerId player, string id, bool active) { }
    
    public void SetRichPresenceActive(PlayerId player, bool active) { }
    
    public void InitializeCloudStorage(PlayerId player, InitializeCloudStoreHandler handler)
    {
        handler(true);
    }

    public void UninitializeCloudStorage() { }

    public void SaveCloudData(IDictionary<string, string> data, SaveCloudDataHandler handler)
    {
        string savePath = this.SavePath;
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
        foreach(string text in data.Keys)
        {
            try
            {
                TextWriter textWriter = new StreamWriter(Path.Combine(savePath, text + ".sav"));
                textWriter.Write(data[text]);
                textWriter.Close();
            } catch
            {
                MirrorOfDusk.Current.StartCoroutine(this.saveFailed_cr(handler));
                handler(false);
                return;
            }
        }
        handler(true);
    }

    private IEnumerator saveFailed_cr(SaveCloudDataHandler handler)
    {
        yield return new WaitForSeconds(0.25f);
        handler(false);
        yield break;
    }

    public void LoadCloudData(string[] keys, LoadCloudDataHandler handler)
    {
        string[] array = new string[keys.Length];
        string savePath = this.SavePath;
        for (int i = 0; i < array.Length; i++)
        {
            string path = Path.Combine(savePath, keys[i] + ".sav");
            if (File.Exists(path))
            {
                try
                {
                    TextReader textReader = new StreamReader(Path.Combine(savePath, keys[i] + ".sav"));
                    array[i] = textReader.ReadToEnd();
                    textReader.Close();
                } catch
                {
                    handler(array, CloudLoadResult.Failed);
                }
            } else
            {
                handler(array, CloudLoadResult.NoData);
            }
        }
        handler(array, CloudLoadResult.Success);
    }

    public void UpdateControllerMapping() { }

    public bool ControllerMappingChanged()
    {
        return false;
    }
}
