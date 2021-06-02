using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class UserConfigData
{
    private const string KEY = "mirrorofdusk_userconfig_data_v1_user_";
    private static readonly string[] SAVE_FILE_KEYS = new string[]
    {
        "mirrorofdusk_userconfig_data_v1_user_0",
        "mirrorofdusk_userconfig_data_v1_user_1",
        "mirrorofdusk_userconfig_data_v1_user_2",
        "mirrorofdusk_userconfig_data_v1_user_3",
        "mirrorofdusk_userconfig_data_v1_user_4",
        "mirrorofdusk_userconfig_data_v1_user_5",
        "mirrorofdusk_userconfig_data_v1_user_6",
        "mirrorofdusk_userconfig_data_v1_user_7",
        "mirrorofdusk_userconfig_data_v1_user_8",
        "mirrorofdusk_userconfig_data_v1_user_9",
        "mirrorofdusk_userconfig_data_v1_user_10",
        "mirrorofdusk_userconfig_data_v1_user_11",
        "mirrorofdusk_userconfig_data_v1_user_12",
        "mirrorofdusk_userconfig_data_v1_user_13",
        "mirrorofdusk_userconfig_data_v1_user_14",
        "mirrorofdusk_userconfig_data_v1_user_15",
        "mirrorofdusk_userconfig_data_v1_user_16",
        "mirrorofdusk_userconfig_data_v1_user_17",
        "mirrorofdusk_userconfig_data_v1_user_18",
        "mirrorofdusk_userconfig_data_v1_user_19",
        "mirrorofdusk_userconfig_data_v1_user_20",
        "mirrorofdusk_userconfig_data_v1_user_21",
        "mirrorofdusk_userconfig_data_v1_user_22",
        "mirrorofdusk_userconfig_data_v1_user_23",
        "mirrorofdusk_userconfig_data_v1_user_24",
        "mirrorofdusk_userconfig_data_v1_user_25",
        "mirrorofdusk_userconfig_data_v1_user_26",
        "mirrorofdusk_userconfig_data_v1_user_27",
        "mirrorofdusk_userconfig_data_v1_user_28",
        "mirrorofdusk_userconfig_data_v1_user_29",
        "mirrorofdusk_userconfig_data_v1_user_30",
        "mirrorofdusk_userconfig_data_v1_user_31",
        "mirrorofdusk_userconfig_data_v1_user_32",
        "mirrorofdusk_userconfig_data_v1_user_33",
        "mirrorofdusk_userconfig_data_v1_user_34",
        "mirrorofdusk_userconfig_data_v1_user_35",
        "mirrorofdusk_userconfig_data_v1_user_36",
        "mirrorofdusk_userconfig_data_v1_user_37",
        "mirrorofdusk_userconfig_data_v1_user_38",
        "mirrorofdusk_userconfig_data_v1_user_39",
        "mirrorofdusk_userconfig_data_v1_user_40",
        "mirrorofdusk_userconfig_data_v1_user_41",
        "mirrorofdusk_userconfig_data_v1_user_42",
        "mirrorofdusk_userconfig_data_v1_user_43",
        "mirrorofdusk_userconfig_data_v1_user_44",
        "mirrorofdusk_userconfig_data_v1_user_45",
        "mirrorofdusk_userconfig_data_v1_user_46",
        "mirrorofdusk_userconfig_data_v1_user_47",
        "mirrorofdusk_userconfig_data_v1_user_48",
        "mirrorofdusk_userconfig_data_v1_user_49",
        "mirrorofdusk_userconfig_data_v1_user_50",
        "mirrorofdusk_userconfig_data_v1_user_51",
        "mirrorofdusk_userconfig_data_v1_user_52",
        "mirrorofdusk_userconfig_data_v1_user_53",
        "mirrorofdusk_userconfig_data_v1_user_54",
        "mirrorofdusk_userconfig_data_v1_user_55",
        "mirrorofdusk_userconfig_data_v1_user_56",
        "mirrorofdusk_userconfig_data_v1_user_57",
        "mirrorofdusk_userconfig_data_v1_user_58",
        "mirrorofdusk_userconfig_data_v1_user_59",
        "mirrorofdusk_userconfig_data_v1_user_60",
        "mirrorofdusk_userconfig_data_v1_user_61",
        "mirrorofdusk_userconfig_data_v1_user_62",
        "mirrorofdusk_userconfig_data_v1_user_63",
        "mirrorofdusk_userconfig_data_v1_user_64",
        "mirrorofdusk_userconfig_data_v1_user_65",
        "mirrorofdusk_userconfig_data_v1_user_66",
        "mirrorofdusk_userconfig_data_v1_user_67",
        "mirrorofdusk_userconfig_data_v1_user_68",
        "mirrorofdusk_userconfig_data_v1_user_69",
        "mirrorofdusk_userconfig_data_v1_user_70",
        "mirrorofdusk_userconfig_data_v1_user_71",
        "mirrorofdusk_userconfig_data_v1_user_72",
        "mirrorofdusk_userconfig_data_v1_user_73",
        "mirrorofdusk_userconfig_data_v1_user_74",
        "mirrorofdusk_userconfig_data_v1_user_75",
        "mirrorofdusk_userconfig_data_v1_user_76",
        "mirrorofdusk_userconfig_data_v1_user_77",
        "mirrorofdusk_userconfig_data_v1_user_78",
        "mirrorofdusk_userconfig_data_v1_user_79",
        "mirrorofdusk_userconfig_data_v1_user_80",
        "mirrorofdusk_userconfig_data_v1_user_81",
        "mirrorofdusk_userconfig_data_v1_user_82",
        "mirrorofdusk_userconfig_data_v1_user_83",
        "mirrorofdusk_userconfig_data_v1_user_84",
        "mirrorofdusk_userconfig_data_v1_user_85",
        "mirrorofdusk_userconfig_data_v1_user_86",
        "mirrorofdusk_userconfig_data_v1_user_87",
        "mirrorofdusk_userconfig_data_v1_user_88",
        "mirrorofdusk_userconfig_data_v1_user_89",
        "mirrorofdusk_userconfig_data_v1_user_90",
        "mirrorofdusk_userconfig_data_v1_user_91",
        "mirrorofdusk_userconfig_data_v1_user_92",
        "mirrorofdusk_userconfig_data_v1_user_93",
        "mirrorofdusk_userconfig_data_v1_user_94",
        "mirrorofdusk_userconfig_data_v1_user_95",
        "mirrorofdusk_userconfig_data_v1_user_96",
        "mirrorofdusk_userconfig_data_v1_user_97",
        "mirrorofdusk_userconfig_data_v1_user_98",
        "mirrorofdusk_userconfig_data_v1_user_99"
    };

    private static int _CurrentSaveFileIndex = 0;
    private static bool _initialized = false;
    public static bool inGame = false;

    private class DefaultActionIds
    {
        public MirrorOfDuskButton button;
        public Rewired.AxisRange axisRange;

        public DefaultActionIds(MirrorOfDuskButton button)
        {
            this.button = button;
            this.axisRange = Rewired.AxisRange.Positive;
        }

        public DefaultActionIds(MirrorOfDuskButton button, Rewired.AxisRange axisRange)
        {
            this.button = button;
            this.axisRange = axisRange;
        }
    }

    private static Dictionary<System.Guid, Dictionary<string, int>> elementIdTemplateDictionary = new Dictionary<Guid, Dictionary<string, int>>()
    {
        { new System.Guid("83b427e4-086f-47f3-bb06-be266abd1ca5"), new Dictionary<string, int>() {
            { "--", -1 },
            { "X", 7 },
            { "Y", 8 },
            { "B", 4 },
            { "A", 5 },
            { "LB", 10 },
            { "RB", 12 },
            { "LT", 11 },
            { "RT", 13 },
            { "LS", 17 },
            { "RS", 18 },
            { "S", 14 }
        } }
    };

    private static Dictionary<System.Guid, Dictionary<string, int>> elementIdDictionary = new Dictionary<Guid, Dictionary<string, int>>()
    {
        { new System.Guid("00000000-0000-0000-0000-000000000000"), new Dictionary<string, int>() {
            { "--", -1 },
            { "X", 32 },
            { "Y", 35 },
            { "B", 34 },
            { "A", 33 },
            { "LB", 36 },
            { "RB", 37 },
            { "LT", 38 },
            { "RT", 39 },
            { "LS", 42 },
            { "RS", 43 },
            { "S", 40 }
        } },
        { new System.Guid("cd9718bf-a87a-44bc-8716-60a0def28a9f"), new Dictionary<string, int>() {
            { "--", -1 },
            { "X", 8 },
            { "Y", 9 },
            { "B", 7 },
            { "A", 6 },
            { "LB", 10 },
            { "RB", 11 },
            { "LT", 4 },
            { "RT", 5 },
            { "LS", 16 },
            { "RS", 17 },
            { "S", 15 }
        } }
    };

    private static Dictionary<System.Guid, Dictionary<int, int>> elementIdTemplateGlyphIndex = new Dictionary<Guid, Dictionary<int, int>>()
    {
        { new System.Guid("83b427e4-086f-47f3-bb06-be266abd1ca5"), new Dictionary<int, int>() {
            { -1, 0 },
            { 7, 6 },
            { 8, 9 },
            { 4, 3 },
            { 5, 0 },
            { 10, 12 },
            { 12, 15 },
            { 11, 18 },
            { 13, 21 },
            { 17, 33 },
            { 18, 42 },
            { 19, 54 },
            { 20, 52 },
            { 21, 55 },
            { 22, 53 },
            { 14, 63 },
            { 15, 61 }
        } }
    };

    private static Dictionary<System.Guid, Dictionary<int, int>> elementIdGlyphIndex = new Dictionary<Guid, Dictionary<int, int>>()
    {
        { new System.Guid("00000000-0000-0000-0000-000000000000"), new Dictionary<int, int>() {
            { -1, 0 },
            { 32, 6 },
            { 35, 9 },
            { 34, 3 },
            { 33, 0 },
            { 36, 12 },
            { 37, 15 },
            { 38, 18 },
            { 39, 21 },
            { 42, 33 },
            { 43, 42 },
            { 160, 54 },
            { 161, 52 },
            { 162, 55 },
            { 163, 53 },
            { 40, 63 },
            { 41, 61 }
        } },
        { new System.Guid("cd9718bf-a87a-44bc-8716-60a0def28a9f"), new Dictionary<int, int>() {
            { -1, 0 },
            { 8, 8 },
            { 9, 11 },
            { 7, 5 },
            { 6, 2 },
            { 10, 13 },
            { 11, 16 },
            { 4, 19 },
            { 5, 22 },
            { 16, 33 },
            { 17, 42 },
            { 18, 54 },
            { 19, 52 },
            { 20, 55 },
            { 21, 53 },
            { 15, 66 },
            { 13, 60 }
        } }
    };

    private static Dictionary<System.Guid, Dictionary<int, Dictionary<int, int>>> defaultActionIdIndex = new Dictionary<Guid, Dictionary<int, Dictionary<int, int>>>()
    {
        { new System.Guid("83b427e4-086f-47f3-bb06-be266abd1ca5"), new Dictionary<int, Dictionary<int, int>>() {
            { (int)(MirrorOfDuskButton.LightAttack),
                new Dictionary<int, int>() { { 1, 6 } }
            },
            { (int)(MirrorOfDuskButton.HeavyAttack),
                new Dictionary<int, int>() { { 1, 9 } }
            },
            { (int)(MirrorOfDuskButton.SpecialAttack),
                new Dictionary<int, int>() { { 1, 3 } }
            },
            { (int)(MirrorOfDuskButton.Jump),
                new Dictionary<int, int>() { { 1, 0 } }
            },
            { (int)(MirrorOfDuskButton.Dodge),
                new Dictionary<int, int>() { { 1, 12 } }
            },
            { (int)(MirrorOfDuskButton.Block),
                new Dictionary<int, int>() { { 1, 15 } }
            },
            { (int)(MirrorOfDuskButton.UseMirror),
                new Dictionary<int, int>() { { 1, 18 } }
            },
            { (int)(MirrorOfDuskButton.Grab),
                new Dictionary<int, int>() { { 1, 21 } }
            },
            { (int)(MirrorOfDuskButton.Accept),
                new Dictionary<int, int>() { { 1, 0 } }
            },
            { (int)(MirrorOfDuskButton.Cancel),
                new Dictionary<int, int>() { { 1, 3 } }
            },
            { (int)(MirrorOfDuskButton.Edit),
                new Dictionary<int, int>() { { 1, 9 } }
            },
            { (int)(MirrorOfDuskButton.ScrollLeft),
                new Dictionary<int, int>() { { 1, 12 } }
            },
            { (int)(MirrorOfDuskButton.ScrollRight),
                new Dictionary<int, int>() { { 1, 15 } }
            },
            { (int)(MirrorOfDuskButton.MoveHorizontal),
                new Dictionary<int, int>() { { 1, 52 }, { 2, 53 } }
            },
            { (int)(MirrorOfDuskButton.MoveVertical),
                new Dictionary<int, int>() { { 1, 54 }, { 2, 55 } }
            },
            { (int)(MirrorOfDuskButton.Pause),
                new Dictionary<int, int>() { { 1, 61 } }
            }
        } }
    };

    private static UserConfigData[] _saveFiles;

    public static UserConfigData[] saveFiles {
         get { return _saveFiles; }
    }

    public static UserConfigData defaultUserConfigData = new UserConfigData("Default_UserName");

    private static UserConfigData.UserConfigDataInitHandler _userConfigDataInitHandler;

    private static InitializeCloudStoreHandler f__m0;
    private static LoadCloudDataHandler f__m1;
    private static LoadCloudDataHandler f__m2;
    private static SaveCloudDataHandler f__m3;
    private static SaveCloudDataHandler f__m4;

    public delegate void UserConfigDataInitHandler(bool success);

    [Serializable]
    public class UserConfigProfile
    {
        public string userProfileName;
        public UserConfigButtons userConfigButtons;
        public bool created;

        [Serializable]
        public class UserConfigButtons
        {
            public UserConfigButtonSet[] userConfigButtonSet;

            [Serializable]
            public class UserConfigButtonSet
            {
                public MirrorOfDuskButton button;
                public string buttonElementChar;

                public UserConfigButtonSet(MirrorOfDuskButton button, string buttonElementChar)
                {
                    this.button = button;
                    this.buttonElementChar = buttonElementChar;
                }
            }

            public UserConfigButtons()
            {
                this.userConfigButtonSet = new UserConfigButtonSet[]
                {
                        new UserConfigButtonSet(MirrorOfDuskButton.LightAttack, "X"),
                        new UserConfigButtonSet(MirrorOfDuskButton.HeavyAttack, "Y"),
                        new UserConfigButtonSet(MirrorOfDuskButton.SpecialAttack, "B"),
                        new UserConfigButtonSet(MirrorOfDuskButton.Jump, "A"),
                        new UserConfigButtonSet(MirrorOfDuskButton.Block, "RB"),
                        new UserConfigButtonSet(MirrorOfDuskButton.Dodge, "LB"),
                        new UserConfigButtonSet(MirrorOfDuskButton.Taunt, "--"),
                        new UserConfigButtonSet(MirrorOfDuskButton.Grab, "RT"),
                        new UserConfigButtonSet(MirrorOfDuskButton.TerrorAttack, "--"),
                        new UserConfigButtonSet(MirrorOfDuskButton.PlungeCancel, "--"),
                        new UserConfigButtonSet(MirrorOfDuskButton.UseMirror, "LT")
                };

            }

            public UserConfigButtons(string _x, string _y, string _b, string _a, string _r, string _l, string _t, string _ra, string _bc, string _rb, string _abc)
            {
                this.userConfigButtonSet = new UserConfigButtonSet[]
                {
                        new UserConfigButtonSet(MirrorOfDuskButton.LightAttack, _x),
                        new UserConfigButtonSet(MirrorOfDuskButton.HeavyAttack, _y),
                        new UserConfigButtonSet(MirrorOfDuskButton.SpecialAttack, _b),
                        new UserConfigButtonSet(MirrorOfDuskButton.Jump, _a),
                        new UserConfigButtonSet(MirrorOfDuskButton.Block, _r),
                        new UserConfigButtonSet(MirrorOfDuskButton.Dodge, _l),
                        new UserConfigButtonSet(MirrorOfDuskButton.Taunt, _t),
                        new UserConfigButtonSet(MirrorOfDuskButton.Grab, _ra),
                        new UserConfigButtonSet(MirrorOfDuskButton.TerrorAttack, _bc),
                        new UserConfigButtonSet(MirrorOfDuskButton.PlungeCancel, _rb),
                        new UserConfigButtonSet(MirrorOfDuskButton.UseMirror, _abc)
                };

            }
        }

        public UserConfigProfile()
        {
            this.userProfileName = "---";
            this.created = false;
            this.userConfigButtons = new UserConfigButtons();
        }

        public UserConfigProfile(string userProfileName)
        {
            this.userProfileName = userProfileName;
            this.created = true;
            this.userConfigButtons = new UserConfigButtons();
        }

        

    }

    public UserConfigProfile userConfigProfile;

    public UserConfigData()
    {
        this.userConfigProfile = new UserConfigProfile();
    }

    public UserConfigData(string name)
    {
        this.userConfigProfile = new UserConfigProfile(name);
    }

    public UserConfigData.UserConfigProfile GetUserConfigProfile()
    {
        return this.userConfigProfile;
    }

    public static int GetUserConfigElementIdentifierId(Rewired.Joystick _joystick, string button)
    {
        if (_joystick != null)
        {
            if (!elementIdDictionary.ContainsKey(_joystick.hardwareTypeGuid)) {
                for (int t = 0; t < _joystick.Templates.Count; t++)
                {
                    if (elementIdTemplateDictionary.ContainsKey(_joystick.Templates[t].typeGuid))
                    {
                        if (!elementIdTemplateDictionary[_joystick.Templates[t].typeGuid].ContainsKey(button)) return -1;
                        return elementIdTemplateDictionary[_joystick.Templates[t].typeGuid][button];
                    }
                }
                return -1;
            }
            if (!elementIdDictionary[_joystick.hardwareTypeGuid].ContainsKey(button)) return -1;
            return elementIdDictionary[_joystick.hardwareTypeGuid][button];
        } else
        {
            System.Guid _hardwareTypeGuid = new System.Guid("83b427e4-086f-47f3-bb06-be266abd1ca5");
            if (!elementIdTemplateDictionary[_hardwareTypeGuid].ContainsKey(button)) return -1;
            return elementIdTemplateDictionary[_hardwareTypeGuid][button];
        }
    }

    public static string GetUserConfigActionElementName(Rewired.Joystick _joystick, int id)
    {
        if (!elementIdDictionary.ContainsKey(_joystick.hardwareTypeGuid)) {
            for (int t = 0; t < _joystick.Templates.Count; t++)
            {
                if (elementIdTemplateDictionary.ContainsKey(_joystick.Templates[t].typeGuid))
                {
                    foreach (KeyValuePair<string, int> elemId in elementIdTemplateDictionary[_joystick.Templates[t].typeGuid])
                    {
                        if (elemId.Value == id)
                        {
                            return elemId.Key;
                        }
                    }
                    return "--";
                }
            }
            return "--";
        }
        foreach (KeyValuePair<string, int> elemId in elementIdDictionary[_joystick.hardwareTypeGuid])
        {
            if (elemId.Value == id)
            {
                return elemId.Key;
            }
        }
        return "--";
    }

    public static int GetControllerGlyphId(Rewired.Joystick _joystick, int id)
    {
        if (!elementIdGlyphIndex.ContainsKey(_joystick.hardwareTypeGuid))
        {
            for (int t = 0; t < _joystick.Templates.Count; t++)
            {
                if (elementIdTemplateGlyphIndex.ContainsKey(_joystick.Templates[t].typeGuid))
                {
                    if (elementIdTemplateGlyphIndex[_joystick.Templates[t].typeGuid].ContainsKey(id))
                    {
                        return elementIdTemplateGlyphIndex[_joystick.Templates[t].typeGuid][id];
                    }
                    return 0;
                }
            }
            return 0;
        }
        if (elementIdGlyphIndex[_joystick.hardwareTypeGuid].ContainsKey(id))
        {
            return elementIdGlyphIndex[_joystick.hardwareTypeGuid][id];
        }
        return 0;
    }

    public static int GetControllerGlyphId(int id)
    {
        if (elementIdTemplateGlyphIndex[new System.Guid("83b427e4-086f-47f3-bb06-be266abd1ca5")].ContainsKey(id))
        {
            return elementIdTemplateGlyphIndex[new System.Guid("83b427e4-086f-47f3-bb06-be266abd1ca5")][id];
        }
        return 0;
    }

    public static int GetDefaultActionId(int actionId)
    {
        return UserConfigData.GetDefaultActionId(actionId, 1);
    }

    public static int GetDefaultActionId(int actionId, int axisRange)
    {
        if (!defaultActionIdIndex[new System.Guid("83b427e4-086f-47f3-bb06-be266abd1ca5")].ContainsKey(actionId)) return 0;
        if (!defaultActionIdIndex[new System.Guid("83b427e4-086f-47f3-bb06-be266abd1ca5")][actionId].ContainsKey(axisRange)) return 0;
        return defaultActionIdIndex[new System.Guid("83b427e4-086f-47f3-bb06-be266abd1ca5")][actionId][axisRange];
    }

    public static int CurrentSaveFileIndex
    {
        get
        {
            return Mathf.Clamp(UserConfigData._CurrentSaveFileIndex, 0, UserConfigData.SAVE_FILE_KEYS.Length - 1);
        }
        set
        {
            UserConfigData._CurrentSaveFileIndex = Mathf.Clamp(value, 0, UserConfigData.SAVE_FILE_KEYS.Length - 1);
        }
    }

    public static bool Initialized
    {
        get { return UserConfigData._initialized; } private set { UserConfigData._initialized = value; }
    }

    public static UserConfigData Data
    {
        get { return UserConfigData.GetDataForUser(UserConfigData.CurrentSaveFileIndex); }
    }

    public static UserConfigData GetDataForUser(int user)
    {
        if (UserConfigData._saveFiles == null || UserConfigData._saveFiles.Length != UserConfigData.SAVE_FILE_KEYS.Length)
        {
            UserConfigData._saveFiles = new UserConfigData[UserConfigData.SAVE_FILE_KEYS.Length];
            for (int i = 0; i < UserConfigData.SAVE_FILE_KEYS.Length; i++)
            {
                UserConfigData._saveFiles[i] = new UserConfigData();
            }
        }
        return UserConfigData._saveFiles[user];
    }

    public static void ClearUser(int user)
    {
        if (UserConfigData._saveFiles == null || UserConfigData._saveFiles.Length != UserConfigData.SAVE_FILE_KEYS.Length)
        {
            return;
        }
        UserConfigData._saveFiles[user] = new UserConfigData();
        UserConfigData.Save(user);
    }

    public static void Init(UserConfigData.UserConfigDataInitHandler handler)
    {
        UserConfigData._saveFiles = new UserConfigData[UserConfigData.SAVE_FILE_KEYS.Length];
        for (int i = 0; i < UserConfigData.SAVE_FILE_KEYS.Length; i++)
        {
            UserConfigData._saveFiles[i] = new UserConfigData();
        }
        UserConfigData._userConfigDataInitHandler = handler;
        OnlineInterface @interface = OnlineManager.Instance.Interface;
        PlayerId player = PlayerId.PlayerOne;
        if (UserConfigData.f__m0 == null)
		{
            UserConfigData.f__m0 = new InitializeCloudStoreHandler(UserConfigData.OnCloudStorageInitialized);
        }
        @interface.InitializeCloudStorage(player, UserConfigData.f__m0);
    }

    private static void OnCloudStorageInitialized(bool success)
    {
        if (!success)
        {
            UserConfigData._userConfigDataInitHandler(false);
            return;
        }
        OnlineInterface @interface = OnlineManager.Instance.Interface;
        string[] save_FILE_KEYS = UserConfigData.SAVE_FILE_KEYS;
        if (UserConfigData.f__m1 == null)
		{
            UserConfigData.f__m1 = new LoadCloudDataHandler(UserConfigData.OnLoaded);
        }
        @interface.LoadCloudData(save_FILE_KEYS, UserConfigData.f__m1);
    }

    private static void OnLoaded(string[] data, CloudLoadResult result)
    {
        if (result == CloudLoadResult.Failed)
        {
            global::UnityEngine.Debug.LogError("[UserConfigData] LOAD FAILED", null);
            OnlineInterface @interface = OnlineManager.Instance.Interface;
            string[] save_FILE_KEYS = UserConfigData.SAVE_FILE_KEYS;
            if (UserConfigData.f__m2 == null)
			{
                UserConfigData.f__m2 = new LoadCloudDataHandler(UserConfigData.OnLoaded);
            }
            @interface.LoadCloudData(save_FILE_KEYS, UserConfigData.f__m2);
            return;
        }
        if (result == CloudLoadResult.NoData)
        {
            global::UnityEngine.Debug.LogError("[UserConfigData] No data. Saving default data to cloud", null);
            UserConfigData.SaveAll();
            return;
        }
        bool flag = false;
        for (int i = 0; i < UserConfigData.SAVE_FILE_KEYS.Length; i++)
        {
            if (data[i] != null)
            {
                UserConfigData userConfigData = null;
                try
                {
                    userConfigData = JsonUtility.FromJson<UserConfigData>(data[i]);
                    if (userConfigData != null)
                    {
                        userConfigData = UserConfigData.Migrate(userConfigData);
                        flag = true;
                    }
                }
                catch (ArgumentException ex)
                {
                    global::UnityEngine.Debug.LogError("Unable to parse user config data. " + ex.StackTrace, null);
                }
                if (userConfigData == null)
                {
                    global::UnityEngine.Debug.LogError("[UserConfigData] Data could not be unserialized for key: " + UserConfigData.SAVE_FILE_KEYS[i], null);
                }
                else
                {
                    UserConfigData._saveFiles[i] = userConfigData;
                }
            }
        }
        UserConfigData.Initialized = true;
        if (flag)
        {
            UserConfigData.SaveAll();
        }
        if (UserConfigData._userConfigDataInitHandler != null)
        {
            UserConfigData._userConfigDataInitHandler(true);
            UserConfigData._userConfigDataInitHandler = null;
        }
    }

    public static UserConfigData Migrate(UserConfigData userConfigData)
    {
        /*for (int i = 0; i < playerData.coinManager.LevelsAndCoins.Count; i++)
        {
            UserConfigData.PlayerCoinManager.LevelAndCoins levelAndCoins = new UserConfigData.PlayerCoinManager.LevelAndCoins();
            levelAndCoins.level = playerData.coinManager.LevelsAndCoins[i].level;
            playerData.coinManager.LevelsAndCoins[i] = levelAndCoins;
        }
        for (int j = 0; j < playerData.coinManager.coins.Count; j++)
        {
            string coinID = playerData.coinManager.coins[j].coinID;
            bool flag = false;
            for (int k = 0; k < UserConfigData.platformingCoinIDs.Length; k++)
            {
                List<UserConfigData.PlayerCoinManager.LevelAndCoins> levelsAndCoins = playerData.coinManager.LevelsAndCoins;
                int index = -1;
                for (int l = 0; l < levelsAndCoins.Count; l++)
                {
                    if (levelsAndCoins[l].level == UserConfigData.platformingCoinIDs[k].levelId)
                    {
                        index = l;
                    }
                }
                for (int m = 0; m < UserConfigData.platformingCoinIDs[k].coinIds.Length; m++)
                {
                    string coinID2 = UserConfigData.platformingCoinIDs[k].coinIds[m][0];
                    for (int n = 0; n < UserConfigData.platformingCoinIDs[k].coinIds[m].Length; n++)
                    {
                        if (coinID == UserConfigData.platformingCoinIDs[k].coinIds[m][n])
                        {
                            playerData.coinManager.coins[j].coinID = coinID2;
                            flag = true;
                            switch (m)
                            {
                                case 0:
                                    levelsAndCoins[index].Coin1Collected = true;
                                    break;
                                case 1:
                                    levelsAndCoins[index].Coin2Collected = true;
                                    break;
                                case 2:
                                    levelsAndCoins[index].Coin3Collected = true;
                                    break;
                                case 3:
                                    levelsAndCoins[index].Coin4Collected = true;
                                    break;
                                case 4:
                                    levelsAndCoins[index].Coin5Collected = true;
                                    break;
                            }
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
                if (flag)
                {
                    break;
                }
            }
        }
        playerData.coinManager.hasMigratedCoins = true;*/
        return userConfigData;
    }

    private static string GetSaveFileKey(int fileIndex)
    {
        return UserConfigData.SAVE_FILE_KEYS[fileIndex];
    }

    private static void Save(int fileIndex)
    {
        //UserConfigData._saveFiles[fileIndex].dialoguerState = Dialoguer.GetGlobalVariablesState();
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        dictionary[UserConfigData.SAVE_FILE_KEYS[fileIndex]] = JsonUtility.ToJson(UserConfigData._saveFiles[fileIndex]);
        OnlineInterface @interface = OnlineManager.Instance.Interface;
        IDictionary<string, string> data = dictionary;
        if (UserConfigData.f__m3 == null)
		{
            UserConfigData.f__m3 = new SaveCloudDataHandler(UserConfigData.OnSaved);
        }
        @interface.SaveCloudData(data, UserConfigData.f__m3);
    }

    private static void SaveAll()
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        for (int i = 0; i < UserConfigData.SAVE_FILE_KEYS.Length; i++)
        {
            dictionary[UserConfigData.SAVE_FILE_KEYS[i]] = JsonUtility.ToJson(UserConfigData._saveFiles[i]);
        }
        OnlineInterface @interface = OnlineManager.Instance.Interface;
        IDictionary<string, string> data = dictionary;
        if (UserConfigData.f__m4 == null)
		{
            UserConfigData.f__m4 = new SaveCloudDataHandler(UserConfigData.OnSavedAll);
        }
        @interface.SaveCloudData(data, UserConfigData.f__m4);
    }

    private static void OnSaved(bool success)
    {
        if (!success)
        {
            global::UnityEngine.Debug.LogError("[UserConfigData] SAVE FAILED. Retrying...", null);
            UserConfigData.Save(UserConfigData.CurrentSaveFileIndex);
        }
    }

    private static void OnSavedAll(bool success)
    {
        if (success)
        {
            UserConfigData.Initialized = true;
            if (UserConfigData._userConfigDataInitHandler != null)
            {
                UserConfigData._userConfigDataInitHandler(true);
                UserConfigData._userConfigDataInitHandler = null;
            }
        }
        else
        {
            global::UnityEngine.Debug.LogError("[UserConfigData] SAVE FAILED. Retrying...", null);
            UserConfigData.SaveAll();
        }
    }

    public static void SaveCurrentFile()
    {
        UserConfigData.Save(UserConfigData.CurrentSaveFileIndex);
    }

    public static void ResetAll()
    {
        for (int i = 0; i < UserConfigData.SAVE_FILE_KEYS.Length; i++)
        {
            UserConfigData.ClearUser(i);
        }
    }

    public static void Unload()
    {
        UserConfigData._saveFiles = null;
    }


}
