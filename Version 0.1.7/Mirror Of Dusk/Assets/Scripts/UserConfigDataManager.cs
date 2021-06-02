using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Rewired;
using UnityEngine;

public static class UserConfigDataManager
{
    private enum UserConfigDataStatus
    {
        Uninitialized,
        Received,
        Initialized
    }

    private static UserConfigDataManager.UserConfigDataStatus userConfigDataStatus;
    public static List<UserConfigData.UserConfigProfile> availableUserProfiles;

    public static void Awake()
    {
        UserConfigDataManager.availableUserProfiles = new List<UserConfigData.UserConfigProfile>();
        UserConfigDataManager.userConfigDataStatus = UserConfigDataStatus.Received;
    }

    public static void Init()
    {
        if (UserConfigDataManager.userConfigDataStatus == UserConfigDataStatus.Initialized)
        {
            return;
        }
        if (UserConfigDataManager.userConfigDataStatus == UserConfigDataStatus.Received)
        {
            UserConfigDataManager.availableUserProfiles.Add(UserConfigData.defaultUserConfigData.userConfigProfile);
            for (int i = 0; i < UserConfigData.saveFiles.Length; i++)
            {
                if (UserConfigData.saveFiles[i].userConfigProfile.created)
                {
                    UserConfigDataManager.availableUserProfiles.Add(UserConfigData.saveFiles[i].userConfigProfile);
                }
            }
            UserConfigDataManager.userConfigDataStatus = UserConfigDataStatus.Initialized;
        }
    }

    public static void AddNewProfile(string name)
    {
        if (UserConfigDataManager.availableUserProfiles.Count < 102)
        {
            UserConfigDataManager.availableUserProfiles.Add(new UserConfigData.UserConfigProfile(name));
            UserConfigData.saveFiles[UserConfigDataManager.availableUserProfiles.Count - 2].userConfigProfile = UserConfigDataManager.availableUserProfiles[UserConfigDataManager.availableUserProfiles.Count - 1];
        }
    }

    public static void EditCurrentProfile(string name)
    {
        UserConfigDataManager.availableUserProfiles[SettingsData.Data.currentUserConfigProfile].userProfileName = name;
        UserConfigData.saveFiles[SettingsData.Data.currentUserConfigProfile - 1].userConfigProfile = UserConfigDataManager.availableUserProfiles[SettingsData.Data.currentUserConfigProfile];
    }

    public static void DeleteCurrentProfile(int profile)
    {
        /*List<UserConfigData.UserConfigProfile> tempUserList = new List<UserConfigData.UserConfigProfile>();
        for (int i = 0; i < UserConfigDataManager.availableUserProfiles.Count; i++)
        {
            if (i == profile)
            {
                continue;
            }
            tempUserList.Add(UserConfigDataManager.availableUserProfiles[i]);
        }

        UserConfigDataManager.availableUserProfiles = new List<UserConfigData.UserConfigProfile>();
        for (int i = 0; i < tempUserList.Count; i++)
        {
            UserConfigDataManager.availableUserProfiles.Add(tempUserList[i]);
        }*/
        UserConfigDataManager.availableUserProfiles.Remove(UserConfigDataManager.availableUserProfiles[profile]);
    }
}
