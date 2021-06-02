using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using Rewired.UI.ControlMapper;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.LightweightPipeline;
using TMPro;
//using UnityStandardAssets.ImageEffects;

public class OptionsGUI : AbstractMB
{
    public static OptionsGUI Current { get; private set; }

    [SerializeField] private GameObject mainObject;
    [SerializeField] private GameObject systemObject;
    [SerializeField] private GameObject displayObject;
    [SerializeField] private GameObject audioObject;
    [SerializeField] private GameObject networkObject;
    [SerializeField] private GameObject configSettingsObject;
    [SerializeField] private GameObject userConfigObject;
    [SerializeField] private GameObject userControllerConfigObject;
    [SerializeField] private GameObject deleteUserObject;
    [SerializeField] private OptionsNewUserNameGUI newUserNameGUIPrefab;
    private OptionsNewUserNameGUI newUserNameGUI;
    private OptionsNewUserNameGUI editUserNameGUI;
    [SerializeField] private GameObject keyboardBattleConfigObject;
    [SerializeField] private GameObject controllerMenuConfigObject;
    [SerializeField] private GameObject keyboardMenuConfigObject;
    [SerializeField] private OptionsGUISlot[] mainObjectButtons;
    [SerializeField] private GameObject[] PcOnlyObjects;
    [SerializeField] private GameObject[] UnlockedOnlyObjects;

    [SerializeField] private GameObject menuBackground;
    [SerializeField] private OptionsGUISlot[] systemObjectButtons;
    [SerializeField] private OptionsGUISlot[] displayObjectButtons;
    [SerializeField] private OptionsGUISlot[] audioObjectButtons;
    [SerializeField] private OptionsGUISlot[] networkObjectButtons;
    [SerializeField] private OptionsGUISlot[] configSettingsObjectButtons;
    [SerializeField] private OptionsGUISlot[] userConfigObjectButtons;
    [SerializeField] private OptionsGUISlot[] userControllerConfigObjectButtons;
    [SerializeField] private OptionsGUISlot[] deleteUserObjectButtons;
    [SerializeField] private OptionsGUISlot[] keyboardBattleConfigObjectButtons;
    [SerializeField] private OptionsGUISlot[] controllerMenuConfigObjectButtons;
    [SerializeField] private OptionsGUISlot[] keyboardMenuConfigObjectButtons;
    private string[] userControllerConfigCurrentChars;

    [SerializeField] private RectTransform menuDetailsCanvas;
    [SerializeField] private TextMeshProUGUI menuDetailsText;
    [SerializeField] private LocalizationHelper menuDetailsTextLocalizationHelper;
    [SerializeField] private OptionsGUI.LanguageTranslation[] languageTranslations;
    [SerializeField] private LocalizationHelper[] elementsToTranslate;
    [SerializeField] private CustomLanguageLayout[] customPositionning;

    private List<OptionsGUISlot> currentItems;
    private int currentUserProfileIndex = 0;
    private bool isConsole;

    private CanvasGroup canvasGroup;
    private CanvasGroup CurrentCanvasGroup { get; set; }
    [SerializeField] private CanvasGroup systemCanvasGroup;
    [SerializeField] private CanvasGroup displayCanvasGroup;
    [SerializeField] private CanvasGroup audioCanvasGroup;
    [SerializeField] private CanvasGroup networkCanvasGroup;
    [SerializeField] private CanvasGroup configSettingsCanvasGroup;
    [SerializeField] private CanvasGroup userConfigCanvasGroup;
    [SerializeField] private CanvasGroup userControllerConfigCanvasGroup;
    [SerializeField] private CanvasGroup deleteUserCanvasGroup;
    [SerializeField] private CanvasGroup keyboardBattleConfigCanvasGroup;
    [SerializeField] private CanvasGroup keyboardBattleConfigPollingCanvas;
    [SerializeField] private TextMeshProUGUI keyboardBattleConfigPlayerText;
    [SerializeField] private TextMeshProUGUI keyboardBattleConfigActionText;
    [SerializeField] private LocalizationHelper keyboardBattleConfigPlayerLocalization;
    [SerializeField] private LocalizationHelper keyboardBattleConfigActionLocalization;
    [SerializeField] private CanvasGroup controllerMenuConfigCanvasGroup;
    [SerializeField] private CanvasGroup keyboardMenuConfigCanvasGroup;
    [SerializeField] private CanvasGroup keyboardMenuConfigPollingCanvas;
    [SerializeField] private TextMeshProUGUI keyboardMenuConfigPlayerText;
    [SerializeField] private TextMeshProUGUI keyboardMenuConfigActionText;
    [SerializeField] private LocalizationHelper keyboardMenuConfigPlayerLocalization;
    [SerializeField] private VerticalLayoutGroup userControllerConfigVLG;
    [SerializeField] private VerticalLayoutGroup userControllerConfigVLGText;
    [SerializeField] private RectTransform userNameGUIRoot;
    [SerializeField] private TextMeshProUGUI deleteUserTextField;
    private AbstractPauseGUI pauseMenu;
    
    private float timeSincePress = 0f;
    private int menuFirstPress = 0;
    private float horizontalHoldTime = 0f;
    private int _verticalSelection;
    private int _mainVerticalSelection = -1;
    private int _configSettingsVerticalSelection = -1;
    private int _userConfigSettingsVerticalSelection = -1;
    private float pollingTimer = 0f;
    private ControllerMap cmapInstance = null;
    private ControllerMap cmapInstance2 = null;

    private MirrorOfDuskInput.AnyPlayerInput input;
    [SerializeField] private OptionsGUIPlayer playerOne;
    [SerializeField] private OptionsGUIPlayer playerTwo;
    [SerializeField] private OptionsGUIPlayer playerThree;
    [SerializeField] private OptionsGUIPlayer playerFour;
    private OptionsGUIPlayer currentPlayer = null;

    private int lastIndex;

    private List<Resolution> resolutions;
    private bool savePlayerData;
    private LightweightRenderPipelineAsset renderPipelineAsset;

    public delegate void FadeHighlighterDelegate();
    public event OptionsGUI.FadeHighlighterDelegate OnFadeHighlighterEvent;
    public delegate void UpdateArrowAnimationDelegate();
    public event OptionsGUI.UpdateArrowAnimationDelegate OnUpdateArrowAnimationEvent;
    public delegate void UpdateVerticalScrollDelegate(int currentVertPos);
    public event OptionsGUI.UpdateVerticalScrollDelegate OnUpdateVerticalScrollEvent;

    [Serializable]
    public struct LanguageTranslation
    {
        [SerializeField] public Localization.Languages language;
        [SerializeField] public string translation;
    }

    public enum State
    {
        MainOptions,
        System,
        Display,
        AudioAndLanguage,
        ConfigSettings,
        UserConfig,
        UserControllerConfig,
        EditUserName,
        EnterNewUserName,
        DeleteUserName,
        KeyboardBattleConfig,
        ControllerMenuConfig,
        KeyboardMenuConfig,
        KeyboardBattlePolling,
        KeyboardMenuPolling,
        Controls,
        Network
    }

    public enum SelectingState
    {
        Free,
        Busy
    }

    private enum SystemOptions
    {
        Vibration,
        ButtonGuide,
        KeyboardCount,
        AutoSave,
        Save,
        RestoreDefaults,
        Back
    }

    private enum DisplayOptions
    {
        HUD_Positioning,
        DisplayFPS,
        VSync,
        Resolution,
        ScreenMode,
        Brightness,
        RestoreDefaults,
        Back
    }

    private enum AudioAndLanguageOptions
    {
        BGMVol,
        SFXVol,
        AmbienceVol,
        VoiceVol,
        VoicePack,
        TextLanguage,
        RestoreDefaults,
        Back
    }

    private enum NetworkOptions
    {
        ReplayUpload,
        SavingReplays,
    }

    private enum ConfigSettingsOptions
    {
        BattleConfigController,
        BattleConfigKeyboard,
        MenuConfigController,
        MenuConfigKeyboard
    }

    private enum MenuKeyboardConfig
    {
        Key_Up,
        Key_Down,
        Key_Left,
        Key_Right,
        Key_Confirm,
        Key_Cancel,
        Key_Edit,
        Key_Start
    }

    private enum BattleKeyboardConfig
    {
        Key_Up,
        Key_Down,
        Key_Left,
        Key_Right,
        Key_LightAttack,
        Key_HeavyAttack,
        Key_SpecialAttack,
        Key_Jump,
        Key_Block,
        Key_Dodge,
        Key_Extra1,
        Key_Extra2,
        Key_Pause
    }

    private enum BattleControllerUserConfig
    {
        UserName,
        AssignedPlayerNumber,
        ControllerConfiguration,
        EditUserName,
        CreateNewUser,
        DeleteUser
    }

    private enum BattleControllerConfig
    {
        End,
        RestoreDefaults,
        A_LightAttack,
        B_HeavyAttack,
        C_SpecialAttack,
        D_Jump,
        R_Block,
        L_Dodge,
        T_Taunt,
        R_A_Grab,
        B_C_TerrorAttack,
        R_BoC_PlungeCancel,
        A_B_C_UseMirror
    }

    private IEnumerator _DetailsScroller;
    public IEnumerator DetailsScroller
    {
        get { return _DetailsScroller; }
        set { _DetailsScroller = value; }
    }

    public OptionsGUI.State state { get; private set; }
    public OptionsGUI.SelectingState selectingState { get; set; }

    public bool optionMenuOpen { get; private set; }
    public bool inputEnabled { get; private set; }

    private int verticalSelection
    {
        get
        {
            return this._verticalSelection;
        }
        set
        {
            this._verticalSelection = (value + this.currentItems.Count) % this.currentItems.Count;
            this.UpdateVerticalSelection();
        }
    }

    public int VerticalSelection
    {
        get { return this.verticalSelection; }
    }

    public bool justClosed { get; private set; }

    private ControllerMenuInputSet controllerMenuInputSet;

    private KeyboardBattleInputSet[] keyboardBattleInputSet = new KeyboardBattleInputSet[]
    {
        new KeyboardBattleInputSet(0),
        new KeyboardBattleInputSet(1)
    };

    private KeyboardMenuInputSet[] keyboardMenuInputSet = new KeyboardMenuInputSet[]
    {
        new KeyboardMenuInputSet(0),
        new KeyboardMenuInputSet(1)
    };

    private class ControllerMenuInputSet
    {
        public PlayerId playerId;
        public Controller controller;
        public ControllerMap controllerMap;
        public ConfigButtons configButtons;
        private static Dictionary<string, string> configDictionary;
        private static Controller cont;
        private static ControllerMap cmap;

        [Serializable]
        public class ConfigButtons
        {
            public ConfigButtonSet[] configButtonSet;
            public List<ElementAssignment> elementAssignmentSet;

            [Serializable]
            public class ConfigButtonSet
            {
                public OptionsGUISlot optionsGUISlot;
                public MirrorOfDuskButton button;
                public string buttonElementChar;
                public ControllerElementType controllerElementType;
                public Pole pole;
                public AxisRange axisRange;
                public int elementIdentifierId;
                public int elementMapId;

                public ConfigButtonSet(OptionsGUISlot optionsGUISlot, MirrorOfDuskButton button, string buttonElementChar, ControllerElementType controllerElementType, Pole pole, AxisRange axisRange, int elementIdentifierId)
                {
                    this.optionsGUISlot = optionsGUISlot;
                    this.button = button;
                    this.buttonElementChar = buttonElementChar;
                    this.controllerElementType = controllerElementType;
                    this.pole = pole;
                    this.axisRange = axisRange;
                    this.elementIdentifierId = elementIdentifierId;
                    if (cmap != null)
                    {
                        this.elementMapId = ControllerMenuInputSet.GetElementIdFromMap(this.button);
                    }
                }
            }

            public ConfigButtons()
            {
                this.configButtonSet = new ConfigButtonSet[]
                {
                        new ConfigButtonSet(null, MirrorOfDuskButton.Accept, "A", ControllerElementType.Button, Pole.Positive, AxisRange.Positive, -1),
                        new ConfigButtonSet(null, MirrorOfDuskButton.Cancel, "B", ControllerElementType.Button, Pole.Positive, AxisRange.Positive, -1),
                        new ConfigButtonSet(null, MirrorOfDuskButton.Edit, "Y", ControllerElementType.Button, Pole.Positive, AxisRange.Positive, -1),
                        new ConfigButtonSet(null, MirrorOfDuskButton.ScrollLeft, "LB", ControllerElementType.Button, Pole.Positive, AxisRange.Positive, -1),
                        new ConfigButtonSet(null, MirrorOfDuskButton.ScrollRight, "RB", ControllerElementType.Button, Pole.Positive, AxisRange.Positive, -1)
                };
                this.elementAssignmentSet = new List<ElementAssignment>();
            }

            public ConfigButtons(OptionsGUISlot[] optionsGUISlots, string _a, string _b, string _y, string _l, string _r, int id_0, int id_1, int id_2, int id_3, int id_4)
            {
                this.configButtonSet = new ConfigButtonSet[]
                {
                        new ConfigButtonSet(optionsGUISlots[2], MirrorOfDuskButton.Accept, ControllerMenuInputSet.FetchFromConfigDictionary(_a), ControllerElementType.Button, Pole.Positive, AxisRange.Positive, id_0),
                        new ConfigButtonSet(optionsGUISlots[3], MirrorOfDuskButton.Cancel, ControllerMenuInputSet.FetchFromConfigDictionary(_b), ControllerElementType.Button, Pole.Positive, AxisRange.Positive, id_1),
                        new ConfigButtonSet(optionsGUISlots[4], MirrorOfDuskButton.Edit, ControllerMenuInputSet.FetchFromConfigDictionary(_y), ControllerElementType.Button, Pole.Positive, AxisRange.Positive, id_2),
                        new ConfigButtonSet(optionsGUISlots[5], MirrorOfDuskButton.ScrollLeft, ControllerMenuInputSet.FetchFromConfigDictionary(_l), ControllerElementType.Button, Pole.Positive, AxisRange.Positive, id_3),
                        new ConfigButtonSet(optionsGUISlots[6], MirrorOfDuskButton.ScrollRight, ControllerMenuInputSet.FetchFromConfigDictionary(_r), ControllerElementType.Button, Pole.Positive, AxisRange.Positive, id_4)
                };
                this.elementAssignmentSet = new List<ElementAssignment>();
            }

            public void UpdateConfig(int cKey, string sKey, int elementId)
            {
                bool conflictFound = false;
                for (int i = 0; i < this.configButtonSet.Length; i++)
                {
                    if (this.configButtonSet[i].elementIdentifierId == elementId && i != cKey)
                    {
                        this.configButtonSet[i].elementIdentifierId = this.configButtonSet[cKey].elementIdentifierId;
                        this.configButtonSet[i].buttonElementChar = this.configButtonSet[cKey].buttonElementChar;
                        conflictFound = true;
                        AddConflictToElementAssignmentList(i, this.configButtonSet[i].elementIdentifierId);
                        break;
                    }
                }
                this.configButtonSet[cKey].buttonElementChar = ControllerMenuInputSet.FetchFromConfigDictionary(sKey);
                UpdateElement(cKey, elementId, this.configButtonSet[cKey].axisRange);
                AddToElementAssignmentList(cKey, elementId, conflictFound);
            }

            private void UpdateElement(int cKey, int elementId, AxisRange axisRange)
            {
                this.configButtonSet[cKey].elementIdentifierId = elementId;
                Update_Glyph(this.configButtonSet[cKey].optionsGUISlot, elementId, axisRange);
            }

            public void UpdateGlyphSprite(int cKey)
            {
                Update_Glyph(this.configButtonSet[cKey].optionsGUISlot, this.configButtonSet[cKey].elementIdentifierId, this.configButtonSet[cKey].axisRange);
            }

            public void AddToElementAssignmentList(int cKey, int elementIdentifierId, bool conflictFound)
            {
                ElementAssignment elementAssign = new ElementAssignment(elementIdentifierId, (int)this.configButtonSet[cKey].button, Pole.Positive, this.configButtonSet[cKey].elementMapId);
                if (conflictFound)
                {
                    for (int i = 0; i < this.elementAssignmentSet.Count; i++)
                    {
                        if (this.elementAssignmentSet[i].elementMapId == elementAssign.elementMapId)
                        {
                            this.elementAssignmentSet[i] = elementAssign;
                            return;
                        }
                    }
                } else
                {
                    for (int i = 0; i < this.elementAssignmentSet.Count; i++)
                    {
                        if (this.elementAssignmentSet[i].elementIdentifierId == elementAssign.elementIdentifierId)
                        {
                            this.elementAssignmentSet[i] = elementAssign;
                            return;
                        }
                    }
                    for (int i = 0; i < this.elementAssignmentSet.Count; i++)
                    {
                        if (this.elementAssignmentSet[i].elementMapId == elementAssign.elementMapId)
                        {
                            this.elementAssignmentSet[i] = elementAssign;
                            return;
                        }
                    }
                }
                this.elementAssignmentSet.Add(elementAssign);
            }

            public void AddConflictToElementAssignmentList(int cKey, int elementIdentifierId)
            {
                ElementAssignment elementAssign = new ElementAssignment(elementIdentifierId, (int)this.configButtonSet[cKey].button, Pole.Positive, this.configButtonSet[cKey].elementMapId);
                for (int i = 0; i < this.elementAssignmentSet.Count; i++)
                {
                    if (this.elementAssignmentSet[i].elementMapId == elementAssign.elementMapId)
                    {
                        this.elementAssignmentSet[i] = elementAssign;
                        return;
                    }
                }
                this.elementAssignmentSet.Add(elementAssign);
            }

            public void AddDefaultElements()
            {
                for (int i = 0; i < 5; i++)
                {
                    ElementAssignment elementAssign = new ElementAssignment(this.configButtonSet[i].elementIdentifierId, (int)this.configButtonSet[i].button, Pole.Positive, this.configButtonSet[i].elementMapId);
                    this.elementAssignmentSet.Add(elementAssign);
                }
            }

            public void AssignNewElements()
            {
                for (int i = 0; i < this.elementAssignmentSet.Count; i++)
                {
                    ControllerMenuInputSet.cmap.ReplaceOrCreateElementMap(this.elementAssignmentSet[i]);
                }
                ControllerMenuInputSet.ConfirmAddMap();
            }
        }

        public ControllerMenuInputSet()
        {
            this.playerId = 0;
            ControllerMenuInputSet.configDictionary = new Dictionary<string, string>();
            this.AddToConfigDictionary();
            this.configButtons = new ConfigButtons();
        }

        public ControllerMenuInputSet(PlayerId playerId, Controller controller, ControllerMap controllerMap)
        {
            this.playerId = playerId;
            this.controller = controller;
            this.controllerMap = controllerMap;
            ControllerMenuInputSet.cont = this.controller;
            ControllerMenuInputSet.cmap = this.controllerMap;
            ControllerMenuInputSet.configDictionary = new Dictionary<string, string>();
            this.AddToConfigDictionary();
            this.configButtons = new ConfigButtons();
        }

        private void AddToConfigDictionary()
        {
            ControllerMenuInputSet.configDictionary.Add("Button 0", "X");
            ControllerMenuInputSet.configDictionary.Add("Button 1", "A");
            ControllerMenuInputSet.configDictionary.Add("Button 2", "B");
            ControllerMenuInputSet.configDictionary.Add("Button 3", "Y");
            ControllerMenuInputSet.configDictionary.Add("Button 4", "LB");
            ControllerMenuInputSet.configDictionary.Add("Button 5", "RB");
            ControllerMenuInputSet.configDictionary.Add("Button 6", "LT");
            ControllerMenuInputSet.configDictionary.Add("Button 7", "RT");
            ControllerMenuInputSet.configDictionary.Add("Button 10", "L-Stick");
            ControllerMenuInputSet.configDictionary.Add("Button 11", "R-Stick");
        }

        private static string FetchFromConfigDictionary(string sKey)
        {
            if (ControllerMenuInputSet.configDictionary.ContainsKey(sKey))
            {
                return ControllerMenuInputSet.configDictionary[sKey];
            }
            return sKey;
        }

        private static int GetElementIdFromMap(MirrorOfDuskButton button)
        {
            ActionElementMap aem = cmap.GetFirstElementMapWithAction((int)button);
            if (aem != null)
            {
                return aem.id;
            }
            return -1;
        }

        private static void ConfirmAddMap()
        {
            PlayerManager.GetPlayerInput(PlayerId.PlayerOne).controllers.maps.AddMap(cont, cmap);
        }

        private static void Update_Glyph(OptionsGUISlot slot, int elementIdentifierId, AxisRange axisRange)
        {
            slot.button.UpdateGlyph((ControllerMenuInputSet.cont as Joystick), elementIdentifierId, axisRange);
        }
    }

    private class KeyboardBattleInputSet
    {
        public int playerId;
        public ConfigKeys configKeys;

        [Serializable]
        public class ConfigKeys
        {
            public ConfigKeySet[] configKeySet;
            public bool edited;

            [Serializable]
            public class ConfigKeySet
            {
                public MirrorOfDuskButton button;
                public string keyElementChar;
                public KeyCode keyCharCode;
                public ActionElementMap aem;
                public ControllerElementType controllerElementType;
                public Pole pole;
                public AxisRange axisRange;
                public int elementIdentifierId;
                
                public ConfigKeySet(MirrorOfDuskButton button, string keyElementChar, KeyCode keyCharCode, ActionElementMap aem, ControllerElementType controllerElementType, Pole pole, AxisRange axisRange)
                {
                    this.button = button;
                    this.keyElementChar = keyElementChar;
                    this.keyCharCode = keyCharCode;
                    this.aem = aem;
                    this.controllerElementType = controllerElementType;
                    this.pole = pole;
                    this.axisRange = axisRange;
                    if (aem != null)
                    {
                        this.elementIdentifierId = aem.elementIdentifierId;
                    } else
                    {
                        this.elementIdentifierId = -1;
                    }
                }
            }

            public ConfigKeys()
            {
                this.configKeySet = new ConfigKeySet[]
                {
                        new ConfigKeySet(MirrorOfDuskButton.MoveHorizontal, "--", KeyCode.None, null, ControllerElementType.Axis, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.MoveHorizontal, "--", KeyCode.None, null, ControllerElementType.Axis, Pole.Negative, AxisRange.Negative),
                        new ConfigKeySet(MirrorOfDuskButton.MoveVertical, "--", KeyCode.None, null, ControllerElementType.Axis, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.MoveVertical, "--", KeyCode.None, null, ControllerElementType.Axis, Pole.Negative, AxisRange.Negative),
                        new ConfigKeySet(MirrorOfDuskButton.LightAttack, "--", KeyCode.None, null, ControllerElementType.Button, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.HeavyAttack, "--", KeyCode.None, null, ControllerElementType.Button, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.SpecialAttack, "--", KeyCode.None, null, ControllerElementType.Button, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.Jump, "--", KeyCode.None, null, ControllerElementType.Button, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.Block, "--", KeyCode.None, null, ControllerElementType.Button, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.Dodge, "--", KeyCode.None, null, ControllerElementType.Button, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.Taunt, "--", KeyCode.None, null, ControllerElementType.Button, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.Grab, "--", KeyCode.None, null, ControllerElementType.Button, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.TerrorAttack, "--", KeyCode.None, null, ControllerElementType.Button, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.PlungeCancel, "--", KeyCode.None, null, ControllerElementType.Button, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.UseMirror, "--", KeyCode.None, null, ControllerElementType.Button, Pole.Positive, AxisRange.Positive)
                };
                this.edited = false;
            }

            public ConfigKeys(ActionElementMap _rt, ActionElementMap _lt, ActionElementMap _up, ActionElementMap _dn, ActionElementMap _x,
                ActionElementMap _y, ActionElementMap _b, ActionElementMap _a, ActionElementMap _r, ActionElementMap _l, ActionElementMap _t,
                ActionElementMap _ra, ActionElementMap _bc, ActionElementMap _rb, ActionElementMap _abc)
            {
                this.configKeySet = new ConfigKeySet[]
                {
                        new ConfigKeySet(MirrorOfDuskButton.MoveHorizontal, this.GetKeyboardConfigName(_rt), this.GetKeyboardConfigCode(_rt), _rt, ControllerElementType.Axis, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.MoveHorizontal, this.GetKeyboardConfigName(_lt), this.GetKeyboardConfigCode(_lt), _lt, ControllerElementType.Axis, Pole.Negative, AxisRange.Negative),
                        new ConfigKeySet(MirrorOfDuskButton.MoveVertical, this.GetKeyboardConfigName(_up), this.GetKeyboardConfigCode(_up), _up, ControllerElementType.Axis, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.MoveVertical, this.GetKeyboardConfigName(_dn), this.GetKeyboardConfigCode(_dn), _dn, ControllerElementType.Axis, Pole.Negative, AxisRange.Negative),
                        new ConfigKeySet(MirrorOfDuskButton.LightAttack, this.GetKeyboardConfigName(_x), this.GetKeyboardConfigCode(_x), _x, ControllerElementType.Button, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.HeavyAttack, this.GetKeyboardConfigName(_y), this.GetKeyboardConfigCode(_y), _y, ControllerElementType.Button, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.SpecialAttack, this.GetKeyboardConfigName(_b), this.GetKeyboardConfigCode(_b), _b, ControllerElementType.Button, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.Jump, this.GetKeyboardConfigName(_a), this.GetKeyboardConfigCode(_a), _a, ControllerElementType.Button, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.Block, this.GetKeyboardConfigName(_r), this.GetKeyboardConfigCode(_r), _r, ControllerElementType.Button, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.Dodge, this.GetKeyboardConfigName(_l), this.GetKeyboardConfigCode(_l), _l, ControllerElementType.Button, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.Taunt, this.GetKeyboardConfigName(_t), this.GetKeyboardConfigCode(_t), _t, ControllerElementType.Button, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.Grab, this.GetKeyboardConfigName(_ra), this.GetKeyboardConfigCode(_ra), _ra, ControllerElementType.Button, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.TerrorAttack, this.GetKeyboardConfigName(_bc), this.GetKeyboardConfigCode(_bc), _bc, ControllerElementType.Button, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.PlungeCancel, this.GetKeyboardConfigName(_rb), this.GetKeyboardConfigCode(_rb), _rb, ControllerElementType.Button, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.UseMirror, this.GetKeyboardConfigName(_abc), this.GetKeyboardConfigCode(_abc), _abc, ControllerElementType.Button, Pole.Positive, AxisRange.Positive)
                };
                this.edited = false;
            }

            private string GetKeyboardConfigName(ActionElementMap aem)
            {
                if (aem != null)
                {
                    if (aem.keyCode != KeyCode.None)
                        return Keyboard.GetKeyName(aem.keyCode);
                }
                return "--";
            }

            private KeyCode GetKeyboardConfigCode(ActionElementMap aem)
            {
                if (aem != null)
                    return aem.keyCode;
                return KeyCode.None;
            }
        }

        public KeyboardBattleInputSet(int playerId)
        {
            this.playerId = playerId;
            this.configKeys = new ConfigKeys();
        }
    }

    private class KeyboardMenuInputSet
    {
        public int playerId;
        public ConfigKeys configKeys;

        [Serializable]
        public class ConfigKeys
        {
            public ConfigKeySet[] configKeySet;
            public bool edited;

            [Serializable]
            public class ConfigKeySet
            {
                public MirrorOfDuskButton button;
                public string keyElementChar;
                public KeyCode keyCharCode;
                public ActionElementMap aem;
                public ControllerElementType controllerElementType;
                public Pole pole;
                public AxisRange axisRange;
                public int elementIdentifierId;

                public ConfigKeySet(MirrorOfDuskButton button, string keyElementChar, KeyCode keyCharCode, ActionElementMap aem, ControllerElementType controllerElementType, Pole pole, AxisRange axisRange)
                {
                    this.button = button;
                    this.keyElementChar = keyElementChar;
                    this.keyCharCode = keyCharCode;
                    this.aem = aem;
                    this.controllerElementType = controllerElementType;
                    this.pole = pole;
                    this.axisRange = axisRange;
                    if (aem != null)
                    {
                        this.elementIdentifierId = aem.elementIdentifierId;
                    }
                    else
                    {
                        this.elementIdentifierId = -1;
                    }
                }
            }

            public ConfigKeys()
            {
                this.configKeySet = new ConfigKeySet[]
                {
                        new ConfigKeySet(MirrorOfDuskButton.CursorHorizontal, "--", KeyCode.None, null, ControllerElementType.Axis, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.CursorHorizontal, "--", KeyCode.None, null, ControllerElementType.Axis, Pole.Negative, AxisRange.Negative),
                        new ConfigKeySet(MirrorOfDuskButton.CursorVertical, "--", KeyCode.None, null, ControllerElementType.Axis, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.CursorVertical, "--", KeyCode.None, null, ControllerElementType.Axis, Pole.Negative, AxisRange.Negative),
                        new ConfigKeySet(MirrorOfDuskButton.Accept, "--", KeyCode.None, null, ControllerElementType.Button, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.Cancel, "--", KeyCode.None, null, ControllerElementType.Button, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.Edit, "--", KeyCode.None, null, ControllerElementType.Button, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.ScrollLeft, "--", KeyCode.None, null, ControllerElementType.Button, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.ScrollRight, "--", KeyCode.None, null, ControllerElementType.Button, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.Pause, "--", KeyCode.None, null, ControllerElementType.Button, Pole.Positive, AxisRange.Positive)
                };
                this.edited = false;
            }

            public ConfigKeys(ActionElementMap _rt, ActionElementMap _lt, ActionElementMap _up, ActionElementMap _dn, ActionElementMap _a,
                ActionElementMap _b, ActionElementMap _c, ActionElementMap _sl, ActionElementMap _sr, ActionElementMap _p)
            {
                this.configKeySet = new ConfigKeySet[]
                {
                        new ConfigKeySet(MirrorOfDuskButton.CursorHorizontal, this.GetKeyboardConfigName(_rt), this.GetKeyboardConfigCode(_rt), _rt, ControllerElementType.Axis, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.CursorHorizontal, this.GetKeyboardConfigName(_lt), this.GetKeyboardConfigCode(_lt), _lt, ControllerElementType.Axis, Pole.Negative, AxisRange.Negative),
                        new ConfigKeySet(MirrorOfDuskButton.CursorVertical, this.GetKeyboardConfigName(_up), this.GetKeyboardConfigCode(_up), _up, ControllerElementType.Axis, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.CursorVertical, this.GetKeyboardConfigName(_dn), this.GetKeyboardConfigCode(_dn), _dn, ControllerElementType.Axis, Pole.Negative, AxisRange.Negative),
                        new ConfigKeySet(MirrorOfDuskButton.Accept, this.GetKeyboardConfigName(_a), this.GetKeyboardConfigCode(_a), _a, ControllerElementType.Button, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.Cancel, this.GetKeyboardConfigName(_b), this.GetKeyboardConfigCode(_b), _b, ControllerElementType.Button, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.Edit, this.GetKeyboardConfigName(_c), this.GetKeyboardConfigCode(_c), _c, ControllerElementType.Button, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.ScrollLeft, this.GetKeyboardConfigName(_sl), this.GetKeyboardConfigCode(_sl), _sl, ControllerElementType.Button, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.ScrollRight, this.GetKeyboardConfigName(_sr), this.GetKeyboardConfigCode(_sr), _sr, ControllerElementType.Button, Pole.Positive, AxisRange.Positive),
                        new ConfigKeySet(MirrorOfDuskButton.Pause, this.GetKeyboardConfigName(_p), this.GetKeyboardConfigCode(_p), _p, ControllerElementType.Button, Pole.Positive, AxisRange.Positive)
                };
                this.edited = false;
            }

            private string GetKeyboardConfigName(ActionElementMap aem)
            {
                if (aem != null)
                {
                    if (aem.keyCode != KeyCode.None)
                        return Keyboard.GetKeyName(aem.keyCode);
                }
                return "--";
            }

            private KeyCode GetKeyboardConfigCode(ActionElementMap aem)
            {
                if (aem != null)
                    return aem.keyCode;
                return KeyCode.None;
            }
        }

        public KeyboardMenuInputSet(int playerId)
        {
            this.playerId = playerId;
            this.configKeys = new ConfigKeys();
        }
    }

    protected override void Awake()
    {
        base.Awake();
        if (Current == null)
        {
            Current = this;
        }
        this.isConsole = PlatformHelper.IsConsole;
        this.optionMenuOpen = false;
        this.canvasGroup = base.GetComponent<CanvasGroup>();
        this.canvasGroup.alpha = 0f;
        this.selectingState = SelectingState.Free;
        this.currentItems = new List<OptionsGUISlot>(this.mainObjectButtons);
        UserConfigDataManager.Init();
        this.resolutions = new List<Resolution>();
        foreach (Resolution resolution in Screen.resolutions)
        {
            Resolution item = default(Resolution);
            item.width = resolution.width;
            item.height = resolution.height;
            item.refreshRate = 60;
            if (!this.resolutions.Contains(item))
            {
                this.resolutions.Add(item);
            }
        }
        if (GraphicsSettings.renderPipelineAsset != null)
        {
            //this.renderPipelineAsset = (LightweightRenderPipelineAsset)GraphicsSettings.renderPipelineAsset;
        }
        this.SetupButtons();
    }

    private void Start()
    {
        Localization.OnLanguageChangedEvent += this.UpdateLanguages;
    }

    private void OnDestroy()
    {
        Localization.OnLanguageChangedEvent -= this.UpdateLanguages;
        for (int h = 0; h < currentItems.Count; h++)
        {
            OnFadeHighlighterEvent -= currentItems[h].OnFadeHighlighter;
        }
    }

    private void UpdateLanguages()
    {
        for (int i = 0; i < this.audioObjectButtons.Length; i++)
        {
            if (this.audioObjectButtons[i].button.textLocalizationHelper != null)
            {
                this.audioObjectButtons[i].button.textLocalizationHelper.ApplyTranslation();
            }
        }
        for (int j = 0; j < this.displayObjectButtons.Length; j++)
        {
            if (this.displayObjectButtons[j].button.textLocalizationHelper != null)
            {
                this.displayObjectButtons[j].button.textLocalizationHelper.ApplyTranslation();
            }
        }
    }

    private void SetupButtons()
    {
        string[] array = new string[this.resolutions.Count];
        int index = 0;
        for (int i = 0; i < this.resolutions.Count; i++)
        {
            array[i] = this.resolutions[i].width + "x" + this.resolutions[i].height;
            if (Screen.width == this.resolutions[i].width && Screen.height == this.resolutions[i].height)
            {
                index = i;
            }
        }
        if (this.isConsole)
        {
            for (int g = 0; g < this.PcOnlyObjects.Length; g++)
            {
                PcOnlyObjects[g].gameObject.SetActive(false);
            }
        }
        string[] array2 = new string[201];
        int brightnessIndex = 0;
        for (int i = 0; i < 201; i++)
        {
            int num = i - 100;
            array2[i] = num.ToString();
            if ((float)num == SettingsData.Data.Brightness)
            {
                brightnessIndex = i;
            }
        }
        string[] array3 = new string[101];
        for (int i = 0; i < 101; i++)
        {
            int num = i;
            array3[i] = num.ToString();
        }
        bool active = PlayerData.inGame;
        for (int g = 0; g < this.UnlockedOnlyObjects.Length; g++)
        {
            UnlockedOnlyObjects[g].gameObject.SetActive(active);
        }
        this.displayObjectButtons[5].button.options = array2;
        this.audioObjectButtons[0].button.options = array3;
        this.audioObjectButtons[1].button.options = array3;
        this.audioObjectButtons[2].button.options = array3;
        this.audioObjectButtons[3].button.options = array3;
        if (!this.isConsole)
        {
            this.displayObjectButtons[3].button.options = array;
            this.displayObjectButtons[4].button.options = new string[]
            {
                "OptionMenuDisplayWindowed",
                "OptionMenuDisplayFullscreen"
            };
        }
        if (!this.isConsole && DEBUG_AssetLoaderManager.debugWasFound)
        {
            this.displayObjectButtons[2].button.updateSelection((QualitySettings.vSyncCount <= 0) ? 0 : 1);
            this.displayObjectButtons[3].button.updateSelection(index);
            this.displayObjectButtons[4].button.updateSelection((!Screen.fullScreen) ? 0 : 1);
        }
        this.displayObjectButtons[5].button.updateSelection((int)(SettingsData.Data.Brightness * 100f + 100f));
        this.audioObjectButtons[0].button.updateSelection((int)((SettingsData.Data.musicVolume + 80f) / 80f * 100f));
        this.audioObjectButtons[1].button.updateSelection((int)((SettingsData.Data.sFXVolume + 80f) / 80f * 100f));
        this.audioObjectButtons[2].button.updateSelection((int)((SettingsData.Data.ambienceVolume + 80f) / 80f * 100f));
        this.audioObjectButtons[3].button.updateSelection((int)((SettingsData.Data.voiceVolume + 80f) / 80f * 100f));
        this.audioObjectButtons[4].button.options = new string[]
        {
            "OptionMenuAudioSetNone",
            "OptionMenuAudioSetA_EN",
            "OptionMenuAudioSetA_JP"
        };
        int index2 = 0;
        for (int l = 0; l < this.languageTranslations.Length; l++)
        {
            if (this.languageTranslations[l].language == Localization.language)
            {
                index2 = l;
                break;
            }
            if (l == languageTranslations.Length - 1)
            {
                Localization.language = (Localization.Languages)0;
                index2 = 0;
            }
        }
        string[] array4 = new string[this.languageTranslations.Length];
        for (int m = 0; m < this.languageTranslations.Length; m++)
        {
            array4[m] = "OptionMenuLanguage" + this.languageTranslations[m].translation;
        }
        this.audioObjectButtons[5].button.options = array4;
        if (DEBUG_AssetLoaderManager.debugWasFound)
        {
            this.audioObjectButtons[5].button.updateSelection(index2);
        }
        this.userConfigObjectButtons[0].button.options = new string[UserConfigDataManager.availableUserProfiles.Count];
        for (int i = 0; i < UserConfigDataManager.availableUserProfiles.Count; i++)
        {
            this.userConfigObjectButtons[0].button.options[i] = UserConfigDataManager.availableUserProfiles[i].userProfileName;
        }
        this.newUserNameGUI = this.newUserNameGUIPrefab.InstantiatePrefab<OptionsNewUserNameGUI>();
        this.newUserNameGUI.rectTransform.SetParent(this.userNameGUIRoot, true);
        this.newUserNameGUI.Init(this, 0);
        this.editUserNameGUI = this.newUserNameGUIPrefab.InstantiatePrefab<OptionsNewUserNameGUI>();
        this.editUserNameGUI.rectTransform.SetParent(this.userNameGUIRoot, true);
        this.editUserNameGUI.Init(this, 1);
    }

    public void ChangeStateCustomLayoutScripts()
    {
        /*string text = this.displayObjectButtons[1].text.text;
        string value = Localization.Find(this.displayObjectButtons[1].options[0]).translation.SanitizedText();
        bool enabled = text.Equals(value);
        for (int i = 0; i < this.customPositionning.Length; i++)
        {
            this.customPositionning[i].enabled = enabled;
        }*/
    }

    public void Init(bool checkIfKOed)
    {
        this.input = new MirrorOfDuskInput.AnyPlayerInput(checkIfKOed);
        playerOne.OnStart();
        playerTwo.OnStart();
        playerThree.OnStart();
        playerFour.OnStart();
    }

    private void Update()
    {
        this.justClosed = false;
        if (!this.inputEnabled || MainMenuScene.Current.state != MainMenuScene.State.OptionsSelecting)
        {
            this.ResetTimeSincePress();
            menuFirstPress = 0;
            horizontalHoldTime = 0;
            return;
        }
        timeSincePress -= Time.deltaTime;
        timeSincePress = Mathf.Clamp(timeSincePress, 0f, 1000f);
        if (this.state == OptionsGUI.State.UserControllerConfig || this.state == OptionsGUI.State.ControllerMenuConfig)
        {
            /*if (MirrorOfDusk.Current.controlMapper.isOpen)
            {
                return;
            }
            this.state = OptionsGUI.State.MainOptions;
            this.canvasGroup.alpha = 1f;
            this.ToggleSubMenu(OptionsGUI.State.MainOptions);
            PlayerManager.ControlsChanged();*/
            return;
        } else if (this.state == OptionsGUI.State.EditUserName || this.state == OptionsGUI.State.EnterNewUserName)
        {
            if (this.newUserNameGUI.isOpen)
            {
                return;
            }
            if (this.editUserNameGUI.isOpen)
            {
                return;
            }
        } else if (this.state == OptionsGUI.State.KeyboardBattlePolling)
        {
            pollingTimer -= Time.deltaTime;

            

            ControllerPollingInfo pollingInfo;
            KeyCode keyCodeLabel = KeyCode.None;
            string keyLabel;
            this.PollKeyboardForAssignment(out pollingInfo, out keyLabel, out keyCodeLabel);
            if (pollingInfo.success)
            {
                if (keyCodeLabel == KeyCode.Escape)
                {
                    this.state = OptionsGUI.State.KeyboardBattleConfig;
                    this.pollingTimer = 5f;
                    this.keyboardBattleConfigPollingCanvas.alpha = 0f;
                    MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                    base.FrameDelayedCallback(new Action(this.ClosingDelay), 20);
                    return;
                }
                if (KeyboardBattleConfigPollingSelect(keyLabel, keyCodeLabel))
                {
                    AudioManager.Play("select2");
                    this.state = OptionsGUI.State.KeyboardBattleConfig;
                    this.pollingTimer = 5f;
                    this.keyboardBattleConfigPollingCanvas.alpha = 0f;
                    MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                    base.FrameDelayedCallback(new Action(this.ClosingDelay), 20);
                }
            }
            if (pollingTimer <= 0f)
            {
                this.state = OptionsGUI.State.KeyboardBattleConfig;
                this.pollingTimer = 5f;
                this.keyboardBattleConfigPollingCanvas.alpha = 0f;
                MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                base.FrameDelayedCallback(new Action(this.ClosingDelay), 20);
            }
            return;
        } else if (this.state == OptionsGUI.State.KeyboardMenuPolling)
        {
            pollingTimer -= Time.deltaTime;
            
            ControllerPollingInfo pollingInfo;
            KeyCode keyCodeLabel = KeyCode.None;
            string keyLabel;
            this.PollKeyboardForAssignment(out pollingInfo, out keyLabel, out keyCodeLabel);
            if (pollingInfo.success)
            {
                if (keyCodeLabel == KeyCode.Escape)
                {
                    this.state = OptionsGUI.State.KeyboardMenuConfig;
                    this.pollingTimer = 5f;
                    this.keyboardMenuConfigPollingCanvas.alpha = 0f;
                    MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                    base.FrameDelayedCallback(new Action(this.ClosingDelay), 20);
                    return;
                }
                if (KeyboardMenuConfigPollingSelect(keyLabel, keyCodeLabel))
                {
                    AudioManager.Play("select2");
                    this.state = OptionsGUI.State.KeyboardMenuConfig;
                    this.pollingTimer = 5f;
                    this.keyboardMenuConfigPollingCanvas.alpha = 0f;
                    MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                    base.FrameDelayedCallback(new Action(this.ClosingDelay), 20);
                }
            }
            if (pollingTimer <= 0f)
            {
                this.state = OptionsGUI.State.KeyboardMenuConfig;
                this.pollingTimer = 5f;
                this.keyboardMenuConfigPollingCanvas.alpha = 0f;
                MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                base.FrameDelayedCallback(new Action(this.ClosingDelay), 20);
            }
            return;
        } else
        {
            if (this.selectingState == SelectingState.Free)
            {
                if (this.state == OptionsGUI.State.KeyboardBattleConfig)
                {
                    if (KeyboardBattleConfigSelect())
                    {
                        return;
                    }
                }
                if (this.state == OptionsGUI.State.KeyboardMenuConfig)
                {
                    if (KeyboardMenuConfigSelect())
                    {
                        return;
                    }
                }
                if (this.GetButtonDown(MirrorOfDuskButton.Pause) || this.GetButtonDown(MirrorOfDuskButton.Cancel))
                {
                    if (this.state == OptionsGUI.State.MainOptions)
                    {
                        MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                        this.MenuCancelSound();
                        this.ToPauseMenu();
                    }
                    else
                    {
                        if (this.state == OptionsGUI.State.UserConfig || this.state == OptionsGUI.State.KeyboardBattleConfig || this.state == OptionsGUI.State.KeyboardMenuConfig)
                        {
                            MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                            this.MenuCancelSound();
                            this.ReturnToConfigSettingsMenu();
                        }
                        else if (this.state == OptionsGUI.State.UserControllerConfig || this.state == OptionsGUI.State.DeleteUserName)
                        {
                            MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                            this.MenuCancelSound();
                            this.ReturnToUserConfigMenu();
                        }
                        else
                        {
                            MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                            this.MenuCancelSound();
                            this.ToMainOptions();
                        }
                    }
                    return;
                }
                if (this.GetButtonDown(MirrorOfDuskButton.Accept))
                {
                    switch (this.state)
                    {
                        case OptionsGUI.State.MainOptions:
                            this.OptionSelect();
                            break;
                        case OptionsGUI.State.System:
                            this.SystemSelect();
                            break;
                        case OptionsGUI.State.Display:
                            this.DisplaySelect();
                            break;
                        case OptionsGUI.State.AudioAndLanguage:
                            this.AudioSelect();
                            break;
                        case OptionsGUI.State.ConfigSettings:
                            this.ConfigSettingsSelect();
                            break;
                        case OptionsGUI.State.UserConfig:
                            this.UserConfigSelect();
                            break;
                        case OptionsGUI.State.DeleteUserName:
                            this.DeleteUserSelect();
                            break;
                        case OptionsGUI.State.Network:
                            this.NetworkSelect();
                            break;
                    }
                    return;
                }
            }
            int horizontalSelectionCount = 0;
            int verticalSelectionCount = 0;
            if (this.state == OptionsGUI.State.DeleteUserName)
            {
                if (this.GetAxis(MirrorOfDuskButton.CursorHorizontal) > 0f)
                {
                    horizontalSelectionCount++;
                }
                if (this.GetAxis(MirrorOfDuskButton.CursorHorizontal) < 0f)
                {
                    horizontalSelectionCount--;
                }
            } else
            {
                if (this.GetAxis(MirrorOfDuskButton.CursorHorizontal) > 0f && this.currentItems[this.verticalSelection].button.options.Length > 0)
                {
                    horizontalSelectionCount++;
                }
                if (this.GetAxis(MirrorOfDuskButton.CursorHorizontal) < 0f && this.currentItems[this.verticalSelection].button.options.Length > 0)
                {
                    horizontalSelectionCount--;
                }
                if (this.GetAxis(MirrorOfDuskButton.CursorHorizontal) > 0f && this.state == OptionsGUI.State.KeyboardBattleConfig && this.verticalSelection >= 3)
                {
                    horizontalSelectionCount++;
                }
            }
            if (this.GetAxis(MirrorOfDuskButton.CursorVertical) < 0f)
            {
                verticalSelectionCount++;
            }
            if (this.GetAxis(MirrorOfDuskButton.CursorVertical) > 0f)
            {
                verticalSelectionCount--;
            }
            if (verticalSelectionCount == 0 && horizontalSelectionCount == 0)
            {
                menuFirstPress--;
                menuFirstPress = Mathf.Clamp(menuFirstPress, 0, 10);
                if (menuFirstPress <= 0)
                {
                    horizontalHoldTime = 0f;
                    this.ResetTimeSincePress();
                }
            }
            if (timeSincePress <= 0f)
            {
                /*if (this.GetButton(MirrorOfDuskButton.MenuRight) && this.currentItems[this.verticalSelection].options.Length > 0)
                {
                    this.currentItems[this.verticalSelection].incrementSelection();
                    this.UpdateHorizontalSelection();
                }
                if (this.GetButton(MirrorOfDuskButton.MenuLeft) && this.currentItems[this.verticalSelection].options.Length > 0)
                {
                    this.currentItems[this.verticalSelection].decrementSelection();
                    this.UpdateHorizontalSelection();
                }*/
                if (verticalSelectionCount != 0)
                {
                    if (this.state != OptionsGUI.State.DeleteUserName)
                    {
                        timeSincePress += 0.15f;
                        if (menuFirstPress == 0)
                            timeSincePress += 0.2f;
                        menuFirstPress = 3;
                        horizontalHoldTime = 0f;
                        AudioManager.Play("menu_scroll");
                        this.verticalSelection += verticalSelectionCount;
                    }
                } else
                {
                    if (horizontalSelectionCount != 0)
                    {
                        if (this.state == OptionsGUI.State.DeleteUserName)
                        {
                            timeSincePress += 0.15f;
                            if (menuFirstPress == 0)
                                timeSincePress += 0.2f;
                            menuFirstPress = 3;
                            this.horizontalHoldTime += Time.deltaTime;
                            if (horizontalSelectionCount > 0)
                            {
                                AudioManager.Play("menu_scroll");
                                this.verticalSelection += horizontalSelectionCount;
                                this.UpdateHorizontalSelection();
                            }
                            else if (horizontalSelectionCount < 0)
                            {
                                AudioManager.Play("menu_scroll");
                                this.verticalSelection += horizontalSelectionCount;
                                this.UpdateHorizontalSelection();
                            }
                            return;
                        }
                        timeSincePress += 0.15f;
                        if (menuFirstPress == 0)
                            timeSincePress += 0.2f;
                        menuFirstPress = 3;
                        this.horizontalHoldTime += Time.deltaTime;
                        this.currentItems[this.verticalSelection].button.HorizontalHoldTime = this.horizontalHoldTime;
                        if (this.state == OptionsGUI.State.KeyboardBattleConfig && horizontalSelectionCount > 0 && this.horizontalHoldTime >= 0.12f)
                        {
                            if (UpdateKeyboardBattleConfigEraseButton())
                            {
                                horizontalHoldTime = 0f;
                                AudioManager.Play("select2");
                                return;
                            }
                        }
                        //AudioManager.Play("menu_scroll");
                        if (!((this.state == OptionsGUI.State.KeyboardBattleConfig || this.state == OptionsGUI.State.KeyboardMenuConfig) && this.verticalSelection > 2))
                        {
                            if (horizontalSelectionCount > 0)
                            {
                                this.currentItems[this.verticalSelection].button.incrementSelection();
                                this.UpdateHorizontalSelection();
                            }
                            else if (horizontalSelectionCount < 0)
                            {
                                this.currentItems[this.verticalSelection].button.decrementSelection();
                                this.UpdateHorizontalSelection();
                            }
                        }
                    }
                }
            }
        }
    }

    private void UpdateVerticalSelection()
    {
        OnFadeHighlighterEvent();
        this.currentItems[verticalSelection].SummonHighlighter();
        if (OnUpdateVerticalScrollEvent != null)
        {
            OnUpdateVerticalScrollEvent(this._verticalSelection);
        }
        UpdateDetails();
        if (OnUpdateArrowAnimationEvent != null)
            OnUpdateArrowAnimationEvent();
        this.currentItems[verticalSelection].UpdateArrowsVertical();
    }

    private void UpdateHorizontalSelection()
    {
        for (int i = 0; i < this.currentItems.Count; i++)
        {
            OptionsGUISlot.Button button = this.currentItems[i].button;
            if (i == this.verticalSelection && button.options.Length > 0)
            {
                OptionsGUI.State state = this.state;
                switch (state)
                {
                    case OptionsGUI.State.System:
                        this.SystemHorizontalSelect(button);
                        break;
                    case OptionsGUI.State.Display:
                        this.DisplayHorizontalSelect(button);
                        break;
                    case OptionsGUI.State.AudioAndLanguage:
                        this.AudioHorizontalSelect(button);
                        break;
                    case OptionsGUI.State.Network:
                        this.NetworkHorizontalSelect(button);
                        break;
                    case OptionsGUI.State.UserConfig:
                        this.UserConfigHorizontalSelect(button);
                        break;
                    case OptionsGUI.State.KeyboardBattleConfig:
                        this.KeyboardBattleConfigHorizontalSelect(button);
                        break;
                    case OptionsGUI.State.KeyboardMenuConfig:
                        this.KeyboardMenuConfigHorizontalSelect(button);
                        break;
                }
            }
        }
    }

    private void UpdateDetails()
    {
        bool duplicateFound = false;
        if (this.currentItems[verticalSelection].button.detailsCode != -1 && this.menuDetailsTextLocalizationHelper != null)
        {
            string formerDetailText = this.menuDetailsText.text;
            this.menuDetailsTextLocalizationHelper.ApplyTranslation(Localization.Find(this.currentItems[verticalSelection].button.detailsCode), null);
            int spriteNum1 = 0;
            int spriteNum2 = 0;
            if ((this.currentItems[verticalSelection].button.textSpriteAction1 != -1))
            {
                if (PlayerManager.IsPlayerUsingController(PlayerId.PlayerOne))
                {
                    Controller pCont = PlayerManager.GetPlayerJoystick(PlayerId.PlayerOne);
                    ControllerMap pContMap = PlayerManager.GetPlayerInput(PlayerId.PlayerOne).controllers.maps.GetMap(pCont, 2, 1);
                    if (pCont != null)
                    {
                        spriteNum1 = UserConfigData.GetControllerGlyphId((pCont as Joystick), pContMap.GetFirstButtonMapWithAction(this.currentItems[verticalSelection].button.textSpriteAction1).elementIdentifierId);
                    }
                    else
                    {
                        spriteNum1 = UserConfigData.GetDefaultActionId(this.currentItems[verticalSelection].button.textSpriteAction1, this.currentItems[verticalSelection].button.textSpriteAxis1);
                    }
                } else
                {
                    spriteNum1 = UserConfigData.GetDefaultActionId(this.currentItems[verticalSelection].button.textSpriteAction1, this.currentItems[verticalSelection].button.textSpriteAxis1);
                }
            }
            if ((this.currentItems[verticalSelection].button.textSpriteAction2 != -1))
            {
                if (PlayerManager.IsPlayerUsingController(PlayerId.PlayerOne))
                {
                    Controller pCont = PlayerManager.GetPlayerJoystick(PlayerId.PlayerOne);
                    ControllerMap pContMap = PlayerManager.GetPlayerInput(PlayerId.PlayerOne).controllers.maps.GetMap(pCont, 2, 1);
                    if (pCont != null)
                    {
                        spriteNum2 = UserConfigData.GetControllerGlyphId((pCont as Joystick), pContMap.GetFirstButtonMapWithAction(this.currentItems[verticalSelection].button.textSpriteAction2).elementIdentifierId);
                    }
                    else
                    {
                        spriteNum2 = UserConfigData.GetDefaultActionId(this.currentItems[verticalSelection].button.textSpriteAction2, this.currentItems[verticalSelection].button.textSpriteAxis2);
                    }
                }
                else
                {
                    spriteNum2 = UserConfigData.GetDefaultActionId(this.currentItems[verticalSelection].button.textSpriteAction2, this.currentItems[verticalSelection].button.textSpriteAxis2);
                }
            }
            string tSprite1 = String.Format("<sprite index= {0}>", spriteNum1);
            string tSprite2 = String.Format("<sprite index= {0}>", spriteNum2);
            if (this.currentItems[verticalSelection].button.textSpriteAction1 != -1 && this.menuDetailsText.text.Contains("<0>"))
            {
                this.menuDetailsText.text = this.menuDetailsText.text.Replace("<0>", tSprite1);
            }
            if (this.currentItems[verticalSelection].button.textSpriteAction2 != -1 && this.menuDetailsText.text.Contains("<1>"))
            {
                this.menuDetailsText.text = this.menuDetailsText.text.Replace("<1>", tSprite2);
            }
            if (formerDetailText == this.menuDetailsText.text)
            {
                duplicateFound = true;
            }
        } else
        {
            this.menuDetailsText.text = "---";
        }
        if (!duplicateFound)
            ScrollDetails();
    }

    public void ShowMainOptionMenu()
    {
        base.StartCoroutine(this.showMainOptionMenu_cr());
    }

    public IEnumerator showMainOptionMenu_cr()
    {
        this.state = OptionsGUI.State.MainOptions;
        this.ToggleSubMenu(this.state);
        this.verticalSelection = 0;
        this.currentItems[verticalSelection].button.highlighterCanvasGroup.alpha = 1f;
        if (this.menuDetailsText != null && this.menuDetailsCanvas != null)
        {
            UpdateDetails();
        }
        float alph = 0;
        while (alph < 1f)
        {
            alph += 0.05f;
            this.canvasGroup.alpha = alph;
            yield return null;
        }
        this.optionMenuOpen = true;
        base.FrameDelayedCallback(new Action(this.Interactable), 1);
        MainMenuScene.Current.state = MainMenuScene.State.OptionsSelecting;
        yield break;
    }

    public void HideMainOptionMenu()
    {
        /*SettingsData.Save();
        if (PlatformHelper.IsConsole)
        {
            SettingsData.SaveToCloud();
        }
        if (this.savePlayerData)
        {
            PlayerData.SaveCurrentFile();
        }*/
        
        this.canvasGroup.interactable = false;
        this.canvasGroup.blocksRaycasts = false;
        this.inputEnabled = false;
        base.StartCoroutine(this.hideMainOptionMenu_cr());
    }

    public IEnumerator hideMainOptionMenu_cr()
    {
        float alph = this.canvasGroup.alpha;
        while (alph > 0f)
        {
            alph -= 0.05f;
            this.canvasGroup.alpha = alph;
            yield return null;
        }
        this.savePlayerData = false;
        this.verticalSelection = 0;
        this.justClosed = true;
        MainMenuScene.Current.state = MainMenuScene.State.Selecting;
        MainMenuScene.Current.SetPlayerStates(MainMenuPlayer.State.Selecting);
        yield break;
    }

    public void ShowSubOptionMenu(CanvasGroup canvasGroup)
    {
        base.StartCoroutine(this.showSubOptionMenu_cr(canvasGroup));
        //this.state = OptionsGUI.State.MainOptions;
        //this.ToggleSubMenu(this.state);
        //this.optionMenuOpen = true;
        //this.verticalSelection = 0;
        //this.canvasGroup.alpha = 1f;
        //base.FrameDelayedCallback(new Action(this.Interactable), 1);
        //this.UpdateVerticalSelection();
    }

    public IEnumerator showSubOptionMenu_cr(CanvasGroup canvasGroup)
    {
        if (this.state != OptionsGUI.State.DeleteUserName)
            this.verticalSelection = 0;
        this.currentItems[verticalSelection].button.highlighterCanvasGroup.alpha = 1f;
        float alph = 0;
        while (alph < 1f)
        {
            alph += 0.05f;
            canvasGroup.alpha = alph;
            yield return null;
        }
        base.FrameDelayedCallback(new Action(this.SubInteractable), 1);
        MainMenuScene.Current.state = MainMenuScene.State.OptionsSelecting;
        yield break;
    }

    public void HideSubOptionMenu(CanvasGroup canvasGroup, OptionsGUI.State state)
    {
        MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
        if (SettingsData.Data.canAutoSave)
        {
            SettingsData.Save();
            if (PlatformHelper.IsConsole)
            {
                SettingsData.SaveToCloud();
            }
            else
            {
                SettingsData.SaveToCloud();
            }
            /*if (this.savePlayerData)
            {
                PlayerData.SaveCurrentFile();
            }*/
        }

        base.StartCoroutine(this.hideSubOptionMenu_cr(canvasGroup, state));
    }

    public IEnumerator hideSubOptionMenu_cr(CanvasGroup canvasGroup, OptionsGUI.State state)
    {
        float alph = canvasGroup.alpha;
        while (alph > 0f)
        {
            alph -= 0.05f;
            canvasGroup.alpha = alph;
            yield return null;
        }
        this.ToggleSubMenu(state);
        MainMenuScene.Current.state = MainMenuScene.State.OptionsSelecting;
        yield break;
    }

    private void Interactable()
    {
        this.verticalSelection = 0;
        this.canvasGroup.interactable = true;
        this.canvasGroup.blocksRaycasts = true;
        this.inputEnabled = true;
    }

    private void SubInteractable()
    {
        if ((this.state == OptionsGUI.State.UserControllerConfig || this.state == OptionsGUI.State.ControllerMenuConfig) && currentPlayer != null)
        {
            currentPlayer.enabled = true;
        }
    }

    private void ClosingDelay()
    {
        MainMenuScene.Current.state = MainMenuScene.State.OptionsSelecting;
    }

    private void OptionSelect()
    {
        this._mainVerticalSelection = this.verticalSelection;
        this.currentItems[_mainVerticalSelection].button.holdHighlighter = true;
        switch (this.verticalSelection)
        {
            case 0:
                MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                this.MenuSelectSound();
                this.ToSystemMenu();
                break;
            case 1:
                MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                this.MenuSelectSound();
                this.ToDisplayMenu();
                break;
            case 2:
                MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                this.MenuSelectSound();
                this.ToAudioMenu();
                break;
            case 3:
                MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                this.MenuSelectSound();
                this.ToConfigSettingsMenu();
                break;
            case 4:
                MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                this.MenuSelectSound();
                this.ToNetworkMenu();
                break;
            case 5:
                MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                this.MenuCancelSound();
                this.ToPauseMenu();
                break;
        }
    }

    private void SystemSelect()
    {
        switch (this.verticalSelection)
        {
            case 0:
                break;
            case 4:
                this.MenuSelectSound();
                SettingsData.Save();
                if (PlatformHelper.IsConsole)
                {
                    SettingsData.SaveToCloud();
                }
                else
                {
                    SettingsData.SaveToCloud();
                }
                MirrorOfDusk.Current.inptm.GetComponent<Rewired.Data.UserDataStore_MirrorOfDusk>().SaveControllerData((int)PlayerId.PlayerOne, ControllerType.Keyboard, 0);
                /*if (this.savePlayerData)
                {
                    PlayerData.SaveCurrentFile();
                }*/
                break;
            case 5:
                this.MenuSelectSound();
                this.currentItems[0].button.selection = 0;
                SettingsData.Data.canVibrate = false;
                this.currentItems[0].button.updateSelection(this.currentItems[0].button.selection);
                this.currentItems[1].button.selection = 0;
                SettingsData.Data.buttonGuide = (SettingsData.ButtonGuide)this.currentItems[1].button.selection;
                this.currentItems[1].button.updateSelection(this.currentItems[1].button.selection);
                this.currentItems[3].button.selection = 0;
                SettingsData.Data.canAutoSave = false;
                this.currentItems[3].button.updateSelection(this.currentItems[3].button.selection);
                if (OnUpdateArrowAnimationEvent != null)
                    OnUpdateArrowAnimationEvent();
                break;
            case 6:
                MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                this.MenuCancelSound();
                this.ToMainOptions();
                break;
        }
    }

    private void DisplaySelect()
    {
        switch (this.verticalSelection)
        {
            case 0:
                this.MenuSelectSound();
                break;
            case 6:
                this.MenuSelectSound();
                this.currentItems[1].button.selection = 0;
                SettingsData.Data.FpsDisplay = false;
                this.currentItems[1].button.updateSelection(this.currentItems[1].button.selection);
                this.currentItems[2].button.selection = 1;
                SettingsData.Data.vSyncCount = 1;
                QualitySettings.vSyncCount = SettingsData.Data.vSyncCount;
                this.currentItems[2].button.updateSelection(this.currentItems[2].button.selection);
                this.currentItems[5].button.selection = 100;
                SettingsData.Data.Brightness = (((float)this.currentItems[5].button.selection - 100f) / 100f);
                this.currentItems[5].button.updateSelection(this.currentItems[5].button.selection);
                if (OnUpdateArrowAnimationEvent != null)
                    OnUpdateArrowAnimationEvent();
                break;
            case 7:
                MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                this.MenuCancelSound();
                this.ToMainOptions();
                break;
        }
    }

    private void AudioSelect()
    {
        switch (this.verticalSelection)
        {
            case 6:
                this.currentItems[0].button.selection = 75;
                this.currentItems[1].button.selection = 80;
                this.currentItems[2].button.selection = 75;
                this.currentItems[3].button.selection = 80;
                AudioManager.bgmOptionsVolume = (float)((this.currentItems[0].button.selection / 100f * 80f) - 80f);
                SettingsData.Data.musicVolume = AudioManager.bgmOptionsVolume;
                AudioManager.sfxOptionsVolume = (float)((this.currentItems[1].button.selection / 100f * 80f) - 80f);
                SettingsData.Data.sFXVolume = AudioManager.sfxOptionsVolume;
                AudioManager.ambienceOptionsVolume = (float)((this.currentItems[2].button.selection / 100f * 80f) - 80f);
                SettingsData.Data.ambienceVolume = AudioManager.ambienceOptionsVolume;
                AudioManager.voiceOptionsVolume = (float)((this.currentItems[3].button.selection / 100f * 80f) - 80f);
                SettingsData.Data.voiceVolume = AudioManager.voiceOptionsVolume;
                this.currentItems[0].button.updateSelection(this.currentItems[0].button.selection);
                this.currentItems[1].button.updateSelection(this.currentItems[1].button.selection);
                this.currentItems[2].button.updateSelection(this.currentItems[2].button.selection);
                this.currentItems[3].button.updateSelection(this.currentItems[3].button.selection);
                this.MenuSelectSound();
                if (OnUpdateArrowAnimationEvent != null)
                    OnUpdateArrowAnimationEvent();
                break;
            case 7:
                MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                this.MenuCancelSound();
                this.ToMainOptions();
                break;
        }
    }

    private void NetworkSelect()
    {
        switch (this.verticalSelection)
        {
            case 2:
                this.currentItems[0].button.selection = 0;
                this.currentItems[1].button.selection = 0;
                SettingsData.Data.canUploadReplays = false;
                SettingsData.Data.canSaveReplays = false;
                this.currentItems[0].button.updateSelection(this.currentItems[0].button.selection);
                this.currentItems[1].button.updateSelection(this.currentItems[1].button.selection);
                this.MenuSelectSound();
                if (OnUpdateArrowAnimationEvent != null)
                    OnUpdateArrowAnimationEvent();
                break;
            case 3:
                MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                this.MenuCancelSound();
                this.ToMainOptions();
                break;
        }
    }

    private void ConfigSettingsSelect()
    {
        switch (this.verticalSelection)
        {
            case 0:
                _configSettingsVerticalSelection = this.verticalSelection;
                this.currentItems[_configSettingsVerticalSelection].button.holdHighlighter = true;
                MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                this.MenuSelectSound();
                this.ToUserConfigMenu();
                break;
            case 1:
                _configSettingsVerticalSelection = this.verticalSelection;
                this.currentItems[_configSettingsVerticalSelection].button.holdHighlighter = true;
                MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                this.MenuSelectSound();
                this.ToKeyboardBattleConfigMenu();
                break;
            case 2:
                int foundId = GetButtonDownAndReturnPlayer(MirrorOfDuskButton.Accept);
                if (foundId != -1)
                {
                    if (foundId == 0)
                    {
                        currentPlayer = playerOne;
                    }
                    else if (foundId == 1)
                    {
                        currentPlayer = playerTwo;
                    }
                    else if (foundId == 2)
                    {
                        currentPlayer = playerThree;
                    }
                    else if (foundId == 3)
                    {
                        currentPlayer = playerFour;
                    }
                    Controller controller = PlayerManager.GetPlayerJoystick(this.currentPlayer.OG_PlayerId);
                    if (controller != null)
                    {
                        _configSettingsVerticalSelection = this.verticalSelection;
                        this.currentItems[_configSettingsVerticalSelection].button.holdHighlighter = true;
                        MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                        this.MenuSelectSound();
                        this.ToControllerMenuConfigMenu();
                    } else
                    {
                        this.MenuCancelSound();
                    }
                }
                break;
            case 3:
                _configSettingsVerticalSelection = this.verticalSelection;
                this.currentItems[_configSettingsVerticalSelection].button.holdHighlighter = true;
                MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                this.MenuSelectSound();
                this.ToKeyboardMenuConfigMenu();
                break;
            case 4:
                MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                this.MenuCancelSound();
                this.ToMainOptions();
                break;
        }
    }

    private void UserConfigSelect()
    {
        switch (this.verticalSelection)
        {
            case 1:
                this.MenuSelectSound();
                if (OnUpdateArrowAnimationEvent != null)
                    OnUpdateArrowAnimationEvent();
                break;
            case 2:
                int foundId = GetButtonDownAndReturnPlayer(MirrorOfDuskButton.Accept);
                if (foundId != -1)
                {
                    _userConfigSettingsVerticalSelection = this.verticalSelection;
                    this.currentItems[_userConfigSettingsVerticalSelection].button.holdHighlighter = true;
                    MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                    this.MenuSelectSound();
                    if (foundId == 0)
                    {
                        currentPlayer = playerOne;
                    } else if (foundId == 1)
                    {
                        currentPlayer = playerTwo;
                    } else if (foundId == 2)
                    {
                        currentPlayer = playerThree;
                    } else if (foundId == 3)
                    {
                        currentPlayer = playerFour;
                    }
                    this.ToUserControllerConfigMenu();
                }
                break;
            case 3:
                if (this.currentItems[0].button.selection != 0)
                {
                    _userConfigSettingsVerticalSelection = this.verticalSelection;
                    this.currentItems[_userConfigSettingsVerticalSelection].button.holdHighlighter = true;
                    MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                    this.MenuSelectSound();
                    this.ToEditUserName();
                } else
                {
                    this.MenuCancelSound();
                }
                break;
            case 4:
                _userConfigSettingsVerticalSelection = this.verticalSelection;
                this.currentItems[_userConfigSettingsVerticalSelection].button.holdHighlighter = true;
                MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                this.MenuSelectSound();
                this.ToNewUserName();
                break;
            case 5:
                if (this.currentItems[0].button.selection != 0)
                {
                    _userConfigSettingsVerticalSelection = this.verticalSelection;
                    this.currentItems[_userConfigSettingsVerticalSelection].button.holdHighlighter = true;
                    MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                    this.MenuSelectSound();
                    this.ToDeleteUserMenu();
                } else
                {
                    this.MenuCancelSound();
                }
                break;
            case 6:
                MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                this.MenuCancelSound();
                this.ReturnToConfigSettingsMenu();
                break;
        }
    }

    private bool UserControllerConfigSelect()
    {
        MirrorOfDuskButton testButton = MirrorOfDuskButton.None;
        switch (this.verticalSelection)
        {
            case 0:
                if (this.GetButtonDown(MirrorOfDuskButton.Cancel))
                {
                    MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                    this.MenuCancelSound();
                    this.ReturnToUserConfigMenu();
                    return true;
                }
                if (this.GetButtonDown(MirrorOfDuskButton.Accept))
                {
                    MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                    this.MenuSelectSound();
                    UserConfigDataManager.availableUserProfiles[SettingsData.Data.currentUserConfigProfile].userConfigButtons = new UserConfigData.UserConfigProfile.UserConfigButtons(
                        this.currentItems[2].button.text.text,
                        this.currentItems[3].button.text.text,
                        this.currentItems[4].button.text.text,
                        this.currentItems[5].button.text.text,
                        this.currentItems[6].button.text.text,
                        this.currentItems[7].button.text.text,
                        this.currentItems[8].button.text.text,
                        this.currentItems[9].button.text.text,
                        this.currentItems[10].button.text.text,
                        this.currentItems[11].button.text.text,
                        this.currentItems[12].button.text.text);
                    this.ReturnToUserConfigMenu();
                    return true;
                }
                break;
            case 1:
                if (this.GetButtonDown(MirrorOfDuskButton.Cancel))
                {
                    return true;
                }
                if (this.GetButtonDown(MirrorOfDuskButton.Accept))
                {
                    this.MenuSelectSound();
                    this.currentItems[2].button.text.text = "X";
                    this.currentItems[3].button.text.text = "Y";
                    this.currentItems[4].button.text.text = "B";
                    this.currentItems[5].button.text.text = "A";
                    this.currentItems[6].button.text.text = "RB";
                    this.currentItems[7].button.text.text = "LB";
                    this.currentItems[8].button.text.text = "--";
                    this.currentItems[9].button.text.text = "RT";
                    this.currentItems[10].button.text.text = "--";
                    this.currentItems[11].button.text.text = "--";
                    this.currentItems[12].button.text.text = "LT";
                    return true;
                }
                break;
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
            case 11:
            case 12:
                if (SettingsData.Data.currentUserConfigProfile != 0)
                {
                    if (GetAndReturnJoystickActionButtonDown(ref testButton))
                    {
                        if (testButton != MirrorOfDuskButton.None)
                        {
                            string tempKey = this.currentItems[this.verticalSelection].button.text.text;
                            string newKey = ButtonReturnText(testButton);
                            for (int i = 2; i <= 12; i++)
                            {
                                if (this.currentItems[i].button.text.text == newKey)
                                {
                                    this.currentItems[i].button.text.text = tempKey;
                                    break;
                                }
                            }
                            this.currentItems[this.verticalSelection].button.text.text = newKey;

                            return true;
                        }
                    }
                    if (GetAndReturnKeyboardActionButtonDown(ref testButton))
                    {
                        if (testButton != MirrorOfDuskButton.None)
                        {
                            string tempKey = this.currentItems[this.verticalSelection].button.text.text;
                            string newKey = ButtonReturnText(testButton);
                            for (int i = 2; i <= 12; i++)
                            {
                                if (this.currentItems[i].button.text.text == newKey)
                                {
                                    this.currentItems[i].button.text.text = tempKey;
                                    break;
                                }
                            }
                            this.currentItems[this.verticalSelection].button.text.text = newKey;

                            return true;
                        }
                    }
                }
                if (this.GetButtonDown(MirrorOfDuskButton.Cancel))
                {
                    return true;
                }
                break;
        }
        return false;
    }

    private bool UpdateUserControllerConfigEraseButton()
    {
        if (this.state == OptionsGUI.State.UserControllerConfig)
        {
            switch (this.verticalSelection)
            {
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                    if (this.userControllerConfigCurrentChars[this.verticalSelection - 2] == "--" || (this.verticalSelection >= 2 && this.verticalSelection <= 7))
                    {
                        return false;
                    }
                    userControllerConfigCurrentChars[this.verticalSelection - 2] = "--";
                    if (this.currentItems[this.verticalSelection].button.glyph != null)
                    {
                        if (this.currentItems[this.verticalSelection].button.glyph.sprite != null)
                        {
                            this.currentItems[this.verticalSelection].button.glyph.color = new Color(
                                this.currentItems[this.verticalSelection].button.glyph.color.r, this.currentItems[this.verticalSelection].button.glyph.color.g,
                                this.currentItems[this.verticalSelection].button.glyph.color.b, 0f);
                        }
                    }
                    this.currentItems[this.verticalSelection].button.text.text = "--";
                    return true;
            }
        }
        return false;
    }

    private void DeleteUserSelect()
    {
        switch (this.verticalSelection)
        {
            case 0:
                MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                this.MenuSelectSound();
                this.RemoveThisUser();
                this.ReturnToUserConfigMenu();
                break;
            case 1:
                MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                this.MenuCancelSound();
                this.ReturnToUserConfigMenu();
                break;
        }
    }

    private bool KeyboardBattleConfigSelect()
    {
        switch (this.verticalSelection)
        {
            case 0:
                if (this.GetButtonDown(MirrorOfDuskButton.Cancel))
                {
                    MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                    this.MenuCancelSound();
                    this.ReturnToConfigSettingsMenu();
                    return true;
                }
                if (this.GetButtonDown(MirrorOfDuskButton.Accept))
                {
                    MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                    this.MenuSelectSound();
                    if (this.keyboardBattleInputSet[0].configKeys.edited)
                    {
                        Controller controller = ReInput.controllers.GetController(ControllerType.Keyboard, 0);
                        for (int j = 3; j <= 17; j++)
                        {
                            ElementAssignment elementAssignment = new ElementAssignment(ControllerType.Keyboard, this.keyboardBattleInputSet[0].configKeys.configKeySet[j - 3].controllerElementType, this.keyboardBattleInputSet[0].configKeys.configKeySet[j - 3].elementIdentifierId, this.keyboardBattleInputSet[0].configKeys.configKeySet[j - 3].axisRange, this.keyboardBattleInputSet[0].configKeys.configKeySet[j - 3].keyCharCode, ModifierKeyFlags.None, (int)this.keyboardBattleInputSet[0].configKeys.configKeySet[j - 3].button, this.keyboardBattleInputSet[0].configKeys.configKeySet[j - 3].pole, false, (this.keyboardBattleInputSet[0].configKeys.configKeySet[j - 3].aem == null) ? -1 : this.keyboardBattleInputSet[0].configKeys.configKeySet[j - 3].aem.id);
                            this.cmapInstance.ReplaceOrCreateElementMap(elementAssignment);
                        }
                        PlayerManager.GetPlayerInput(PlayerId.PlayerOne).controllers.maps.AddMap(controller, cmapInstance);
                    }
                    if (this.keyboardBattleInputSet[1].configKeys.edited)
                    {
                        Controller controller = ReInput.controllers.GetController(ControllerType.Keyboard, 0);
                        for (int j = 3; j <= 17; j++)
                        {
                            ElementAssignment elementAssignment = new ElementAssignment(ControllerType.Keyboard, this.keyboardBattleInputSet[1].configKeys.configKeySet[j - 3].controllerElementType, this.keyboardBattleInputSet[1].configKeys.configKeySet[j - 3].elementIdentifierId, this.keyboardBattleInputSet[1].configKeys.configKeySet[j - 3].axisRange, this.keyboardBattleInputSet[1].configKeys.configKeySet[j - 3].keyCharCode, ModifierKeyFlags.None, (int)this.keyboardBattleInputSet[1].configKeys.configKeySet[j - 3].button, this.keyboardBattleInputSet[1].configKeys.configKeySet[j - 3].pole, false, (this.keyboardBattleInputSet[1].configKeys.configKeySet[j - 3].aem == null) ? -1 : this.keyboardBattleInputSet[1].configKeys.configKeySet[j - 3].aem.id);
                            this.cmapInstance2.ReplaceOrCreateElementMap(elementAssignment);
                        }
                        PlayerManager.GetPlayerInput(PlayerId.PlayerTwo).controllers.maps.AddMap(controller, cmapInstance2);
                    }
                    this.ReturnToConfigSettingsMenu();
                    return true;
                }
                break;
            case 1:
                if (this.GetButtonDown(MirrorOfDuskButton.Cancel))
                {
                    MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                    this.MenuCancelSound();
                    this.ReturnToConfigSettingsMenu();
                    return true;
                }
                break;
            case 2:
                if (this.GetButtonDown(MirrorOfDuskButton.Cancel))
                {
                    MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                    this.MenuCancelSound();
                    this.ReturnToConfigSettingsMenu();
                    return true;
                }
                if (this.GetButtonDown(MirrorOfDuskButton.Accept))
                {
                    this.MenuSelectSound();

                    Controller controller = ReInput.controllers.GetController(ControllerType.Keyboard, 0);
                    ControllerMap controllerMap = ReInput.mapping.GetControllerMapInstance(controller, 1, 1);
                    this.keyboardBattleInputSet[0].configKeys = new KeyboardBattleInputSet.ConfigKeys(
                        GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.MoveHorizontal, Pole.Positive),
                        GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.MoveHorizontal, Pole.Negative),
                        GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.MoveVertical, Pole.Positive),
                        GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.MoveVertical, Pole.Negative),
                        GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.LightAttack),
                        GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.HeavyAttack),
                        GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.SpecialAttack),
                        GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.Jump),
                        GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.Block),
                        GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.Dodge),
                        GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.Taunt),
                        GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.Grab),
                        GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.TerrorAttack),
                        GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.PlungeCancel),
                        GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.UseMirror)
                        );
                    ControllerMap controllerMap2 = ReInput.mapping.GetControllerMapInstance(controller, 1, 2);
                    this.keyboardBattleInputSet[1].configKeys = new KeyboardBattleInputSet.ConfigKeys(
                        GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.MoveHorizontal, Pole.Positive),
                        GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.MoveHorizontal, Pole.Negative),
                        GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.MoveVertical, Pole.Positive),
                        GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.MoveVertical, Pole.Negative),
                        GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.LightAttack),
                        GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.HeavyAttack),
                        GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.SpecialAttack),
                        GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.Jump),
                        GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.Block),
                        GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.Dodge),
                        GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.Taunt),
                        GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.Grab),
                        GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.TerrorAttack),
                        GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.PlungeCancel),
                        GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.UseMirror)
                        );
                    for (int i = 0; i < 2; i++)
                    {
                        this.currentItems[3].button.options[i] = keyboardBattleInputSet[i].configKeys.configKeySet[0].keyElementChar;
                        this.currentItems[4].button.options[i] = keyboardBattleInputSet[i].configKeys.configKeySet[1].keyElementChar;
                        this.currentItems[5].button.options[i] = keyboardBattleInputSet[i].configKeys.configKeySet[2].keyElementChar;
                        this.currentItems[6].button.options[i] = keyboardBattleInputSet[i].configKeys.configKeySet[3].keyElementChar;
                        this.currentItems[7].button.options[i] = keyboardBattleInputSet[i].configKeys.configKeySet[4].keyElementChar;
                        this.currentItems[8].button.options[i] = keyboardBattleInputSet[i].configKeys.configKeySet[5].keyElementChar;
                        this.currentItems[9].button.options[i] = keyboardBattleInputSet[i].configKeys.configKeySet[6].keyElementChar;
                        this.currentItems[10].button.options[i] = keyboardBattleInputSet[i].configKeys.configKeySet[7].keyElementChar;
                        this.currentItems[11].button.options[i] = keyboardBattleInputSet[i].configKeys.configKeySet[8].keyElementChar;
                        this.currentItems[12].button.options[i] = keyboardBattleInputSet[i].configKeys.configKeySet[9].keyElementChar;
                        this.currentItems[13].button.options[i] = keyboardBattleInputSet[i].configKeys.configKeySet[10].keyElementChar;
                        this.currentItems[14].button.options[i] = keyboardBattleInputSet[i].configKeys.configKeySet[11].keyElementChar;
                        this.currentItems[15].button.options[i] = keyboardBattleInputSet[i].configKeys.configKeySet[12].keyElementChar;
                        this.currentItems[16].button.options[i] = keyboardBattleInputSet[i].configKeys.configKeySet[13].keyElementChar;
                        this.currentItems[17].button.options[i] = keyboardBattleInputSet[i].configKeys.configKeySet[14].keyElementChar;
                    }
                    this.currentItems[3].button.updateNonSelection(this.currentItems[1].button.selection);
                    this.currentItems[4].button.updateNonSelection(this.currentItems[1].button.selection);
                    this.currentItems[5].button.updateNonSelection(this.currentItems[1].button.selection);
                    this.currentItems[6].button.updateNonSelection(this.currentItems[1].button.selection);
                    this.currentItems[7].button.updateNonSelection(this.currentItems[1].button.selection);
                    this.currentItems[8].button.updateNonSelection(this.currentItems[1].button.selection);
                    this.currentItems[9].button.updateNonSelection(this.currentItems[1].button.selection);
                    this.currentItems[10].button.updateNonSelection(this.currentItems[1].button.selection);
                    this.currentItems[11].button.updateNonSelection(this.currentItems[1].button.selection);
                    this.currentItems[12].button.updateNonSelection(this.currentItems[1].button.selection);
                    this.currentItems[13].button.updateNonSelection(this.currentItems[1].button.selection);
                    this.currentItems[14].button.updateNonSelection(this.currentItems[1].button.selection);
                    this.currentItems[15].button.updateNonSelection(this.currentItems[1].button.selection);
                    this.currentItems[16].button.updateNonSelection(this.currentItems[1].button.selection);
                    this.currentItems[17].button.updateNonSelection(this.currentItems[1].button.selection);
                    this.cmapInstance = controllerMap;
                    this.cmapInstance2 = controllerMap2;
                    this.keyboardBattleInputSet[0].configKeys.edited = true;
                    this.keyboardBattleInputSet[1].configKeys.edited = true;
                    return true;
                }
                break;
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
            case 11:
            case 12:
            case 13:
            case 14:
            case 15:
            case 16:
            case 17:
                if (this.GetButtonDown(MirrorOfDuskButton.Accept))
                {
                    this.state = OptionsGUI.State.KeyboardBattlePolling;
                    this.pollingTimer = 5f;
                    this.keyboardBattleConfigPollingCanvas.alpha = 1f;
                    this.FindTextLocalizer(this.keyboardBattleConfigPlayerLocalization, this.keyboardBattleConfigPlayerText, "OptionMenuPollingPlayer", new LocalizationHelper.LocalizationSubtext[] { new LocalizationHelper.LocalizationSubtext("OptionMenuPlayer1", "OptionMenuPlayer", false), new LocalizationHelper.LocalizationSubtext("PlayerNumber", " PlayerNumber:", true) });
                    this.keyboardBattleConfigPlayerText.text = this.keyboardBattleConfigPlayerText.text.Replace("PlayerNumber", this.currentItems[1].button.text.text);
                    //this.keyboardBattleConfigPlayerText.text = this.FindTextLocalizer(this.keyboardBattleConfigPlayerLocalization, "OptionMenuPlayer") + " " + (this.currentItems[1].button.text.text) + ":";
                    this.keyboardBattleConfigActionText.text = this.currentItems[this.verticalSelection].button.labelText.text;
                    return true;
                }
                if (this.GetButtonDown(MirrorOfDuskButton.Cancel))
                {
                    return true;
                }
                break;
            default:
                if (this.GetButtonDown(MirrorOfDuskButton.Cancel))
                {
                    MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                    this.MenuCancelSound();
                    this.ReturnToConfigSettingsMenu();
                    return true;
                }
                break;
        }
        return false;
    }

    private bool KeyboardMenuConfigSelect()
    {
        switch (this.verticalSelection)
        {
            case 0:
                if (this.GetButtonDown(MirrorOfDuskButton.Cancel))
                {
                    MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                    this.MenuCancelSound();
                    this.ReturnToConfigSettingsMenu();
                    return true;
                }
                if (this.GetButtonDown(MirrorOfDuskButton.Accept))
                {
                    MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                    this.MenuSelectSound();
                    if (this.keyboardMenuInputSet[0].configKeys.edited)
                    {
                        Controller controller = ReInput.controllers.GetController(ControllerType.Keyboard, 0);
                        for (int j = 3; j <= 12; j++)
                        {
                            ElementAssignment elementAssignment = new ElementAssignment(ControllerType.Keyboard, this.keyboardMenuInputSet[0].configKeys.configKeySet[j - 3].controllerElementType, this.keyboardMenuInputSet[0].configKeys.configKeySet[j - 3].elementIdentifierId, this.keyboardMenuInputSet[0].configKeys.configKeySet[j - 3].axisRange, this.keyboardMenuInputSet[0].configKeys.configKeySet[j - 3].keyCharCode, ModifierKeyFlags.None, (int)this.keyboardMenuInputSet[0].configKeys.configKeySet[j - 3].button, this.keyboardMenuInputSet[0].configKeys.configKeySet[j - 3].pole, false, (this.keyboardMenuInputSet[0].configKeys.configKeySet[j - 3].aem == null) ? -1 : this.keyboardMenuInputSet[0].configKeys.configKeySet[j - 3].aem.id);
                            this.cmapInstance.ReplaceOrCreateElementMap(elementAssignment);
                        }
                        PlayerManager.GetPlayerInput(PlayerId.PlayerOne).controllers.maps.AddMap(controller, cmapInstance);
                    }
                    if (this.keyboardMenuInputSet[1].configKeys.edited)
                    {
                        Controller controller = ReInput.controllers.GetController(ControllerType.Keyboard, 0);
                        for (int j = 3; j <= 12; j++)
                        {
                            ElementAssignment elementAssignment = new ElementAssignment(ControllerType.Keyboard, this.keyboardMenuInputSet[1].configKeys.configKeySet[j - 3].controllerElementType, this.keyboardMenuInputSet[1].configKeys.configKeySet[j - 3].elementIdentifierId, this.keyboardMenuInputSet[1].configKeys.configKeySet[j - 3].axisRange, this.keyboardMenuInputSet[1].configKeys.configKeySet[j - 3].keyCharCode, ModifierKeyFlags.None, (int)this.keyboardMenuInputSet[1].configKeys.configKeySet[j - 3].button, this.keyboardMenuInputSet[1].configKeys.configKeySet[j - 3].pole, false, (this.keyboardMenuInputSet[1].configKeys.configKeySet[j - 3].aem == null) ? -1 : this.keyboardMenuInputSet[1].configKeys.configKeySet[j - 3].aem.id);
                            this.cmapInstance2.ReplaceOrCreateElementMap(elementAssignment);
                        }
                        PlayerManager.GetPlayerInput(PlayerId.PlayerTwo).controllers.maps.AddMap(controller, cmapInstance2);
                    }
                    this.ReturnToConfigSettingsMenu();
                    return true;
                }
                break;
            case 1:
                if (this.GetButtonDown(MirrorOfDuskButton.Cancel))
                {
                    MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                    this.MenuCancelSound();
                    this.ReturnToConfigSettingsMenu();
                    return true;
                }
                break;
            case 2:
                if (this.GetButtonDown(MirrorOfDuskButton.Cancel))
                {
                    MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                    this.MenuCancelSound();
                    this.ReturnToConfigSettingsMenu();
                    return true;
                }
                if (this.GetButtonDown(MirrorOfDuskButton.Accept))
                {
                    this.MenuSelectSound();

                    Controller controller = ReInput.controllers.GetController(ControllerType.Keyboard, 0);
                    ControllerMap controllerMap = ReInput.mapping.GetControllerMapInstance(controller, 2, 1);

                    this.keyboardMenuInputSet[0].configKeys = new KeyboardMenuInputSet.ConfigKeys(
                            GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.CursorHorizontal, Pole.Positive),
                            GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.CursorHorizontal, Pole.Negative),
                            GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.CursorVertical, Pole.Positive),
                            GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.CursorVertical, Pole.Negative),
                            GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.Accept),
                            GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.Cancel),
                            GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.Edit),
                            GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.ScrollLeft),
                            GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.ScrollRight),
                            GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.Pause)
                            );

                    ControllerMap controllerMap2 = ReInput.mapping.GetControllerMapInstance(controller, 2, 2);

                    this.keyboardMenuInputSet[1].configKeys = new KeyboardMenuInputSet.ConfigKeys(
                            GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.CursorHorizontal, Pole.Positive),
                            GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.CursorHorizontal, Pole.Negative),
                            GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.CursorVertical, Pole.Positive),
                            GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.CursorVertical, Pole.Negative),
                            GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.Accept),
                            GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.Cancel),
                            GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.Edit),
                            GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.ScrollLeft),
                            GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.ScrollRight),
                            GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.Pause)
                        );


                    for (int i = 0; i < 2; i++)
                    {
                        this.currentItems[3].button.options[i] = keyboardMenuInputSet[i].configKeys.configKeySet[0].keyElementChar;
                        this.currentItems[4].button.options[i] = keyboardMenuInputSet[i].configKeys.configKeySet[1].keyElementChar;
                        this.currentItems[5].button.options[i] = keyboardMenuInputSet[i].configKeys.configKeySet[2].keyElementChar;
                        this.currentItems[6].button.options[i] = keyboardMenuInputSet[i].configKeys.configKeySet[3].keyElementChar;
                        this.currentItems[7].button.options[i] = keyboardMenuInputSet[i].configKeys.configKeySet[4].keyElementChar;
                        this.currentItems[8].button.options[i] = keyboardMenuInputSet[i].configKeys.configKeySet[5].keyElementChar;
                        this.currentItems[9].button.options[i] = keyboardMenuInputSet[i].configKeys.configKeySet[6].keyElementChar;
                        this.currentItems[10].button.options[i] = keyboardMenuInputSet[i].configKeys.configKeySet[7].keyElementChar;
                        this.currentItems[11].button.options[i] = keyboardMenuInputSet[i].configKeys.configKeySet[8].keyElementChar;
                        this.currentItems[12].button.options[i] = keyboardMenuInputSet[i].configKeys.configKeySet[9].keyElementChar;
                    }

                    this.currentItems[3].button.updateNonSelection(this.currentItems[1].button.selection);
                    this.currentItems[4].button.updateNonSelection(this.currentItems[1].button.selection);
                    this.currentItems[5].button.updateNonSelection(this.currentItems[1].button.selection);
                    this.currentItems[6].button.updateNonSelection(this.currentItems[1].button.selection);
                    this.currentItems[7].button.updateNonSelection(this.currentItems[1].button.selection);
                    this.currentItems[8].button.updateNonSelection(this.currentItems[1].button.selection);
                    this.currentItems[9].button.updateNonSelection(this.currentItems[1].button.selection);
                    this.currentItems[10].button.updateNonSelection(this.currentItems[1].button.selection);
                    this.currentItems[11].button.updateNonSelection(this.currentItems[1].button.selection);
                    this.currentItems[12].button.updateNonSelection(this.currentItems[1].button.selection);
                    this.cmapInstance = controllerMap;
                    this.cmapInstance2 = controllerMap2;
                    this.keyboardMenuInputSet[0].configKeys.edited = true;
                    this.keyboardMenuInputSet[1].configKeys.edited = true;
                    return true;
                }
                break;
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
            case 11:
            case 12:
                if (this.GetButtonDown(MirrorOfDuskButton.Accept))
                {
                    this.state = OptionsGUI.State.KeyboardMenuPolling;
                    this.pollingTimer = 5f;
                    this.keyboardMenuConfigPollingCanvas.alpha = 1f;
                    this.FindTextLocalizer(this.keyboardMenuConfigPlayerLocalization, this.keyboardMenuConfigPlayerText, "OptionMenuPollingPlayer", new LocalizationHelper.LocalizationSubtext[] { new LocalizationHelper.LocalizationSubtext("OptionMenuPlayer1", "OptionMenuPlayer", false), new LocalizationHelper.LocalizationSubtext("PlayerNumber", " PlayerNumber:", true) });
                    this.keyboardMenuConfigPlayerText.text = this.keyboardMenuConfigPlayerText.text.Replace("PlayerNumber", this.currentItems[1].button.text.text);
                    //this.keyboardMenuConfigPlayerText.text = "Player " + (this.currentItems[1].button.text.text) + ":";
                    this.keyboardMenuConfigActionText.text = this.currentItems[this.verticalSelection].button.labelText.text;
                    return true;
                }
                if (this.GetButtonDown(MirrorOfDuskButton.Cancel))
                {
                    return true;
                }
                break;
            default:
                if (this.GetButtonDown(MirrorOfDuskButton.Cancel))
                {
                    MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                    this.MenuCancelSound();
                    this.ReturnToConfigSettingsMenu();
                    return true;
                }
                break;
        }
        return false;
    }

    private bool KeyboardBattleConfigPollingSelect(string keyLabel, KeyCode keyCodeLabel)
    {
        if (keyLabel == "" || keyLabel == String.Empty)
            return false;
        if (this.verticalSelection >= 3)
        {
            string tempKey = "--";
            KeyCode tempKeyCode = KeyCode.None;

            Controller controller = ReInput.controllers.GetController(ControllerType.Keyboard, 0);
            ElementAssignment elementAssignment = new ElementAssignment(ControllerType.Keyboard, this.keyboardBattleInputSet[this.currentItems[1].button.selection].configKeys.configKeySet[this.verticalSelection].controllerElementType, this.keyboardBattleInputSet[this.currentItems[1].button.selection].configKeys.configKeySet[this.verticalSelection].elementIdentifierId, this.keyboardBattleInputSet[this.currentItems[1].button.selection].configKeys.configKeySet[this.verticalSelection].axisRange, keyCodeLabel, ModifierKeyFlags.None, (int)this.keyboardBattleInputSet[this.currentItems[1].button.selection].configKeys.configKeySet[this.verticalSelection].button, this.keyboardBattleInputSet[this.currentItems[1].button.selection].configKeys.configKeySet[this.verticalSelection].pole, false, (this.keyboardBattleInputSet[this.currentItems[1].button.selection].configKeys.configKeySet[this.verticalSelection].aem == null) ? -1 : this.keyboardBattleInputSet[this.currentItems[1].button.selection].configKeys.configKeySet[this.verticalSelection].aem.id);
            ControllerMap findMap = ((this.currentItems[1].button.selection == 0) ? PlayerManager.GetPlayerInput(PlayerId.PlayerOne).controllers.maps.GetMap(controller, 1, 1) : PlayerManager.GetPlayerInput(PlayerId.PlayerTwo).controllers.maps.GetMap(controller, 1, 2));
            ElementAssignmentConflictCheck conflictCheck;
            CreateConflictCheck(this.currentItems[1].button.selection, controller, findMap, keyCodeLabel, elementAssignment, out conflictCheck);
            if (GetFirstElementAssignmentConflict(PlayerManager.GetPlayerInput(PlayerId.PlayerOne), conflictCheck, true))
            {
                this.MenuCancelSound();
                return false;
            }
            if (GetFirstElementAssignmentConflict(PlayerManager.GetPlayerInput(PlayerId.PlayerTwo), conflictCheck, true))
            {
                this.MenuCancelSound();
                return false;
            }

            for (int i = 3; i <= 17; i++)
            {
                if (!(i == this.verticalSelection && this.currentItems[i].button.selection == 0))
                {
                    if (this.currentItems[i].button.options[0] == keyLabel)
                    {
                        tempKey = this.currentItems[this.verticalSelection].button.options[this.currentItems[1].button.selection];
                        tempKeyCode = this.keyboardBattleInputSet[this.currentItems[1].button.selection].configKeys.configKeySet[this.verticalSelection - 3].keyCharCode;
                        this.currentItems[i].button.options[0] = tempKey;
                        this.keyboardBattleInputSet[0].configKeys.configKeySet[i - 3].keyElementChar = tempKey;
                        this.keyboardBattleInputSet[0].configKeys.configKeySet[i - 3].keyCharCode = tempKeyCode;
                        this.keyboardBattleInputSet[0].configKeys.edited = true;
                        if (this.currentItems[1].button.selection == 0)
                            this.currentItems[i].button.updateNonSelection(0);
                        break;
                    }
                }
                if (!(i == this.verticalSelection && this.currentItems[i].button.selection == 1))
                {
                    if (this.currentItems[i].button.options[1] == keyLabel)
                    {
                        tempKey = this.currentItems[this.verticalSelection].button.options[this.currentItems[1].button.selection];
                        tempKeyCode = this.keyboardBattleInputSet[this.currentItems[1].button.selection].configKeys.configKeySet[this.verticalSelection - 3].keyCharCode;
                        this.currentItems[i].button.options[1] = tempKey;
                        this.keyboardBattleInputSet[1].configKeys.configKeySet[i - 3].keyElementChar = tempKey;
                        this.keyboardBattleInputSet[1].configKeys.configKeySet[i - 3].keyCharCode = tempKeyCode;
                        this.keyboardBattleInputSet[1].configKeys.edited = true;
                        if (this.currentItems[1].button.selection == 1)
                            this.currentItems[i].button.updateNonSelection(1);
                        break;
                    }
                }
            }
            this.currentItems[this.verticalSelection].button.options[this.currentItems[this.verticalSelection].button.selection] = keyLabel;
            this.currentItems[this.verticalSelection].button.updateSelection(this.currentItems[this.verticalSelection].button.selection);
            this.keyboardBattleInputSet[this.currentItems[1].button.selection].configKeys.edited = true;
            this.keyboardBattleInputSet[this.currentItems[1].button.selection].configKeys.configKeySet[this.verticalSelection - 3].keyElementChar = keyLabel;
            this.keyboardBattleInputSet[this.currentItems[1].button.selection].configKeys.configKeySet[this.verticalSelection - 3].keyCharCode = keyCodeLabel;
            return true;
        }
        return false;
    }

    private bool KeyboardMenuConfigPollingSelect(string keyLabel, KeyCode keyCodeLabel)
    {
        if (keyLabel == "" || keyLabel == String.Empty)
            return false;
        if (this.verticalSelection >= 3)
        {
            string tempKey = "--";
            KeyCode tempKeyCode = KeyCode.None;
            
            if (this.verticalSelection == 12)
            {
                Controller controller = ReInput.controllers.GetController(ControllerType.Keyboard, 0);
                ElementAssignment elementAssignment = new ElementAssignment(ControllerType.Keyboard, this.keyboardMenuInputSet[this.currentItems[1].button.selection].configKeys.configKeySet[9].controllerElementType, this.keyboardMenuInputSet[this.currentItems[1].button.selection].configKeys.configKeySet[9].elementIdentifierId, this.keyboardMenuInputSet[this.currentItems[1].button.selection].configKeys.configKeySet[9].axisRange, keyCodeLabel, ModifierKeyFlags.None, (int)this.keyboardMenuInputSet[this.currentItems[1].button.selection].configKeys.configKeySet[9].button, this.keyboardMenuInputSet[this.currentItems[1].button.selection].configKeys.configKeySet[9].pole, false, (this.keyboardMenuInputSet[this.currentItems[1].button.selection].configKeys.configKeySet[9].aem == null) ? -1 : this.keyboardMenuInputSet[this.currentItems[1].button.selection].configKeys.configKeySet[9].aem.id);
                ControllerMap findMap = ((this.currentItems[1].button.selection == 0) ? PlayerManager.GetPlayerInput(PlayerId.PlayerOne).controllers.maps.GetMap(controller, 2, 1) : PlayerManager.GetPlayerInput(PlayerId.PlayerTwo).controllers.maps.GetMap(controller, 2, 2));
                ElementAssignmentConflictCheck conflictCheck;
                CreateConflictCheck(this.currentItems[1].button.selection, controller, findMap, keyCodeLabel, elementAssignment, out conflictCheck);
                if (GetFirstElementAssignmentConflict(PlayerManager.GetPlayerInput(PlayerId.PlayerOne), conflictCheck, false))
                {
                    this.MenuCancelSound();
                    return false;
                }
                if (GetFirstElementAssignmentConflict(PlayerManager.GetPlayerInput(PlayerId.PlayerTwo), conflictCheck, false))
                {
                    this.MenuCancelSound();
                    return false;
                }
            }

            for (int i = 3; i <= 12; i++)
            {
                if (!(i == this.verticalSelection && this.currentItems[i].button.selection == 0))
                {
                    if (this.currentItems[i].button.options[0] == keyLabel)
                    {
                        tempKey = this.currentItems[this.verticalSelection].button.options[this.currentItems[1].button.selection];
                        tempKeyCode = this.keyboardMenuInputSet[this.currentItems[1].button.selection].configKeys.configKeySet[this.verticalSelection - 3].keyCharCode;
                        this.currentItems[i].button.options[0] = tempKey;
                        this.keyboardMenuInputSet[0].configKeys.configKeySet[i - 3].keyElementChar = tempKey;
                        this.keyboardMenuInputSet[0].configKeys.configKeySet[i - 3].keyCharCode = tempKeyCode;
                        this.keyboardMenuInputSet[0].configKeys.edited = true;
                        if (this.currentItems[1].button.selection == 0)
                            this.currentItems[i].button.updateNonSelection(0);
                        break;
                    }
                }
                if (!(i == this.verticalSelection && this.currentItems[i].button.selection == 1))
                {
                    if (this.currentItems[i].button.options[1] == keyLabel)
                    {
                        tempKey = this.currentItems[this.verticalSelection].button.options[this.currentItems[1].button.selection];
                        tempKeyCode = this.keyboardMenuInputSet[this.currentItems[1].button.selection].configKeys.configKeySet[this.verticalSelection - 3].keyCharCode;
                        this.currentItems[i].button.options[1] = tempKey;
                        this.keyboardMenuInputSet[1].configKeys.configKeySet[i - 3].keyElementChar = tempKey;
                        this.keyboardMenuInputSet[1].configKeys.configKeySet[i - 3].keyCharCode = tempKeyCode;
                        this.keyboardMenuInputSet[1].configKeys.edited = true;
                        if (this.currentItems[1].button.selection == 1)
                            this.currentItems[i].button.updateNonSelection(1);
                        break;
                    }
                }
            }
            this.currentItems[this.verticalSelection].button.options[this.currentItems[this.verticalSelection].button.selection] = keyLabel;
            this.currentItems[this.verticalSelection].button.updateSelection(this.currentItems[this.verticalSelection].button.selection);
            this.keyboardMenuInputSet[this.currentItems[1].button.selection].configKeys.edited = true;
            this.keyboardMenuInputSet[this.currentItems[1].button.selection].configKeys.configKeySet[this.verticalSelection - 3].keyElementChar = keyLabel;
            this.keyboardMenuInputSet[this.currentItems[1].button.selection].configKeys.configKeySet[this.verticalSelection - 3].keyCharCode = keyCodeLabel;
            return true;
        }
        return false;
    }

    private void GetDefaultBattleAEM(int _inputSet, int _configSet, ActionElementMap _aem)
    {
        KeyCode setKey = (_aem != null ? _aem.keyCode : KeyCode.None);
        this.keyboardBattleInputSet[_inputSet].configKeys.configKeySet[_configSet].keyCharCode = setKey;
        this.keyboardBattleInputSet[_inputSet].configKeys.configKeySet[_configSet].keyElementChar = ((setKey == KeyCode.None) ? "--" : Keyboard.GetKeyName(setKey));
        this.currentItems[_configSet + 3].button.options[_inputSet] = this.keyboardBattleInputSet[_inputSet].configKeys.configKeySet[_configSet].keyElementChar;
    }

    private void GetDefaultMenuAEM(int _inputSet, int _configSet, ActionElementMap _aem)
    {
        KeyCode setKey = (_aem != null ? _aem.keyCode : KeyCode.None);
        this.keyboardMenuInputSet[_inputSet].configKeys.configKeySet[_configSet].keyCharCode = setKey;
        this.keyboardMenuInputSet[_inputSet].configKeys.configKeySet[_configSet].keyElementChar = ((setKey == KeyCode.None) ? "--" : Keyboard.GetKeyName(setKey));
        this.currentItems[_configSet + 3].button.options[_inputSet] = this.keyboardMenuInputSet[_inputSet].configKeys.configKeySet[_configSet].keyElementChar;
    }

    private bool UpdateKeyboardBattleConfigEraseButton()
    {
        switch (this.verticalSelection)
        {
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
            case 11:
            case 12:
            case 13:
            case 14:
            case 15:
            case 16:
            case 17:
                if (this.currentItems[this.verticalSelection].button.text.text == "--" || (this.verticalSelection >= 3 && this.verticalSelection <= 12))
                {
                    return false;
                }
                this.currentItems[this.verticalSelection].button.text.text = "--";
                return true;
        }
        return false;
    }

    private void CreateConflictCheck(int pId, Controller cont, ControllerMap cmap, KeyCode newKey, ElementAssignment assignment, out ElementAssignmentConflictCheck conflictCheck)
    {
        conflictCheck = assignment.ToElementAssignmentConflictCheck();
        conflictCheck.playerId = pId;
        conflictCheck.controllerType = cont.type;
        conflictCheck.controllerId = cont.id;
        conflictCheck.controllerMapId = cmap.id;
        conflictCheck.keyboardKey = newKey;
        //conflictCheck.modifierKeyFlags = ModifierKeyFlags.None;
        //conflictCheck.actionId = actionId;
        //conflictCheck.axisContribution = Pole.Positive;
        //conflictCheck.elementMapId = elemMapId;
    }

    private bool GetFirstElementAssignmentConflict(Player player, ElementAssignmentConflictCheck conflictCheck, bool specific)
    {
        foreach (ElementAssignmentConflictInfo c in player.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck))
        {
            if (specific)
            {
                if (c.actionId == (int)MirrorOfDuskButton.Pause)
                    return true;
            } else
            {
                return true;
            }
        };
        return false;
    }

    private void ToPauseMenu()
    {
        MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
        this.optionMenuOpen = false;
        this.HideMainOptionMenu();
    }

    private void ToMainOptions()
    {
        if (this.state == OptionsGUI.State.AudioAndLanguage)
        {
            if (Localization.language != this.languageTranslations[this.currentItems[5].button.selection].language)
            {
                Localization.language = this.languageTranslations[this.currentItems[5].button.selection].language;
                for (int i = 0; i < this.elementsToTranslate.Length; i++)
                {
                    this.elementsToTranslate[i].ApplyTranslation();
                }
            }
        }
        this.state = OptionsGUI.State.MainOptions;
        if (this.CurrentCanvasGroup != null)
        {
            this.HideSubOptionMenu(this.CurrentCanvasGroup, this.state);
            this.CurrentCanvasGroup = null;
        }
    }

    private void ToSystemMenu()
    {
        this.state = OptionsGUI.State.System;
        if (!this.isConsole)
        {
            this.ChangeStateCustomLayoutScripts();
        }
        this.ToggleSubMenu(this.state);
        this.CurrentCanvasGroup = this.systemCanvasGroup;
        this.ShowSubOptionMenu(this.systemCanvasGroup);
    }

    private void ToDisplayMenu()
    {
        this.state = OptionsGUI.State.Display;
        if (!this.isConsole)
        {
            this.ChangeStateCustomLayoutScripts();
        }
        this.ToggleSubMenu(this.state);
        this.CurrentCanvasGroup = this.displayCanvasGroup;
        this.ShowSubOptionMenu(this.displayCanvasGroup);
    }

    private void ToAudioMenu()
    {
        this.state = OptionsGUI.State.AudioAndLanguage;
        if (!this.isConsole)
        {
            this.ChangeStateCustomLayoutScripts();
        }
        this.ToggleSubMenu(this.state);
        this.CurrentCanvasGroup = this.audioCanvasGroup;
        this.ShowSubOptionMenu(this.audioCanvasGroup);
    }

    private void ToConfigSettingsMenu()
    {
        this.state = OptionsGUI.State.ConfigSettings;
        if (!this.isConsole)
        {
            this.ChangeStateCustomLayoutScripts();
        }
        this.ToggleSubMenu(this.state);
        this.CurrentCanvasGroup = this.configSettingsCanvasGroup;
        this.ShowSubOptionMenu(this.configSettingsCanvasGroup);
    }

    private void ReturnToConfigSettingsMenu()
    {
        this.state = OptionsGUI.State.ConfigSettings;
        if (this.CurrentCanvasGroup != null)
        {
            this.HideSubOptionMenu(this.CurrentCanvasGroup, this.state);
            this.CurrentCanvasGroup = this.configSettingsCanvasGroup;
        }
    }

    private void ToUserConfigMenu()
    {
        this.state = OptionsGUI.State.UserConfig;
        if (!this.isConsole)
        {
            this.ChangeStateCustomLayoutScripts();
        }
        this.ToggleSubMenu(this.state);
        this.CurrentCanvasGroup = this.userConfigCanvasGroup;
        this.ShowSubOptionMenu(this.userConfigCanvasGroup);
    }

    private void ToKeyboardBattleConfigMenu()
    {
        this.state = OptionsGUI.State.KeyboardBattleConfig;
        if (!this.isConsole)
        {
            this.ChangeStateCustomLayoutScripts();
        }
        this.ToggleSubMenu(this.state);
        this.CurrentCanvasGroup = this.keyboardBattleConfigCanvasGroup;
        this.ShowSubOptionMenu(this.keyboardBattleConfigCanvasGroup);
    }

    private void ToControllerMenuConfigMenu()
    {
        this.state = OptionsGUI.State.ControllerMenuConfig;
        if (!this.isConsole)
        {
            this.ChangeStateCustomLayoutScripts();
        }
        this.ToggleSubMenu(this.state);
        this.CurrentCanvasGroup = this.controllerMenuConfigCanvasGroup;
        this.ShowSubOptionMenu(this.controllerMenuConfigCanvasGroup);
    }

    private void ToKeyboardMenuConfigMenu()
    {
        this.state = OptionsGUI.State.KeyboardMenuConfig;
        if (!this.isConsole)
        {
            this.ChangeStateCustomLayoutScripts();
        }
        this.ToggleSubMenu(this.state);
        this.CurrentCanvasGroup = this.keyboardMenuConfigCanvasGroup;
        this.ShowSubOptionMenu(this.keyboardMenuConfigCanvasGroup);
    }

    private void ReturnToUserConfigMenu()
    {
        this.state = OptionsGUI.State.UserConfig;
        if (this.CurrentCanvasGroup != null)
        {
            this.HideSubOptionMenu(this.CurrentCanvasGroup, this.state);
            this.CurrentCanvasGroup = this.userConfigCanvasGroup;
        }
    }

    private void ToUserControllerConfigMenu()
    {
        this.state = OptionsGUI.State.UserControllerConfig;
        if (!this.isConsole)
        {
            this.ChangeStateCustomLayoutScripts();
        }
        this.ToggleSubMenu(this.state);
        this.CurrentCanvasGroup = this.userControllerConfigCanvasGroup;
        this.ShowSubOptionMenu(this.userControllerConfigCanvasGroup);
    }

    private void ToNewUserName()
    {
        this.state = OptionsGUI.State.EnterNewUserName;
        if (!this.isConsole)
        {
            this.ChangeStateCustomLayoutScripts();
        }
        this.ToggleSubMenu(this.state);
    }

    private void ToEditUserName()
    {
        this.state = OptionsGUI.State.EditUserName;
        if (!this.isConsole)
        {
            this.ChangeStateCustomLayoutScripts();
        }
        this.ToggleSubMenu(this.state);
    }

    private void ToNetworkMenu()
    {
        this.state = OptionsGUI.State.Network;
        if (!this.isConsole)
        {
            this.ChangeStateCustomLayoutScripts();
        }
        this.ToggleSubMenu(this.state);
        this.CurrentCanvasGroup = this.networkCanvasGroup;
        this.ShowSubOptionMenu(this.networkCanvasGroup);
    }

    private void ToDeleteUserMenu()
    {
        this.state = OptionsGUI.State.DeleteUserName;
        if (!this.isConsole)
        {
            this.ChangeStateCustomLayoutScripts();
        }
        this.ToggleSubMenu(this.state);
        this.CurrentCanvasGroup = this.deleteUserCanvasGroup;
        this.ShowSubOptionMenu(this.deleteUserCanvasGroup);
    }

    private void SystemHorizontalSelect(OptionsGUISlot.Button button)
    {
        switch (this.verticalSelection)
        {
            case 0:
                if (button.selection > 0)
                {
                    SettingsData.Data.canVibrate = true;
                } else
                {
                    SettingsData.Data.canVibrate = false;
                }
                break;
            case 1:
                SettingsData.Data.buttonGuide = (SettingsData.ButtonGuide)this.currentItems[1].button.selection;
                break;
            case 2:
                SettingsData.Data.keyboardUserCount = this.currentItems[2].button.selection + 1;
                break;
            case 3:
                if (button.selection > 0)
                {
                    SettingsData.Data.canAutoSave = true;
                }
                else
                {
                    SettingsData.Data.canAutoSave = false;
                }
                break;
        }
    }

    private void DisplayHorizontalSelect(OptionsGUISlot.Button button)
    {
        switch (this.verticalSelection)
        {
            case 1:
                if (button.selection > 0)
                {
                    SettingsData.Data.FpsDisplay = true;
                }
                else
                {
                    SettingsData.Data.FpsDisplay = false;
                }
                break;
            case 2:
                SettingsData.Data.vSyncCount = ((button.selection != 0) ? 1 : 0);
                QualitySettings.vSyncCount = SettingsData.Data.vSyncCount;
                break;
            case 3:
                if (button.selection < this.resolutions.Count)
                {
                    SettingsData.Data.screenWidth = this.resolutions[button.selection].width;
                    SettingsData.Data.screenHeight = this.resolutions[button.selection].height;
                    Screen.SetResolution(SettingsData.Data.screenWidth, SettingsData.Data.screenHeight, Screen.fullScreen, 60);
                }
                break;
            case 4:
                SettingsData.Data.fullScreen = (button.selection == 1);
                if (!this.isConsole)
                {
                    this.ChangeStateCustomLayoutScripts();
                }
                Screen.fullScreen = SettingsData.Data.fullScreen;
                break;
            case 5:
                SettingsData.Data.Brightness = (((float)button.selection - 100f) / 100f);
                break;
        }
    }

    private void AudioHorizontalSelect(OptionsGUISlot.Button button)
    {
        switch (this.verticalSelection)
        {
            case 0:
                AudioManager.bgmOptionsVolume = (float)((button.selection / 100f * 80f) - 80f);
                SettingsData.Data.musicVolume = AudioManager.bgmOptionsVolume;
                break;
            case 1:
                AudioManager.sfxOptionsVolume = (float)((button.selection / 100f * 80f) - 80f);
                SettingsData.Data.sFXVolume = AudioManager.sfxOptionsVolume;
                break;
            case 2:
                AudioManager.ambienceOptionsVolume = (float)((button.selection / 100f * 80f) - 80f);
                SettingsData.Data.ambienceVolume = AudioManager.ambienceOptionsVolume;
                break;
            case 3:
                AudioManager.voiceOptionsVolume = (float)((button.selection / 100f * 80f) - 80f);
                SettingsData.Data.voiceVolume = AudioManager.voiceOptionsVolume;
                break;
            case 4:
                SettingsData.Data.languageVoicePack = (SettingsData.LanguageVoicePack)this.currentItems[4].button.selection;
                break;
            case 5:
                //Localization.language = this.languageTranslations[button.selection].language;
                /*for (int i = 0; i < this.elementsToTranslate.Length; i++)
                {
                    this.elementsToTranslate[i].ApplyTranslation();
                }*/
                break;
        }
    }

    private void NetworkHorizontalSelect(OptionsGUISlot.Button button)
    {
        switch (this.verticalSelection)
        {
            case 0:
                if (button.selection > 0)
                {
                    SettingsData.Data.canUploadReplays = true;
                }
                else
                {
                    SettingsData.Data.canUploadReplays = false;
                }
                break;
            case 1:
                if (button.selection > 0)
                {
                    SettingsData.Data.canSaveReplays = true;
                }
                else
                {
                    SettingsData.Data.canSaveReplays = false;
                }
                break;
        }
    }

    private void UserConfigHorizontalSelect(OptionsGUISlot.Button button)
    {
        switch (this.verticalSelection)
        {
            case 0:
                SettingsData.Data.currentUserConfigProfile = button.selection;
                break;
        }
    }

    private void KeyboardBattleConfigHorizontalSelect(OptionsGUISlot.Button button)
    {
        switch (this.verticalSelection)
        {
            case 1:
                this.currentItems[3].button.updateNonSelection(button.selection);
                this.currentItems[4].button.updateNonSelection(button.selection);
                this.currentItems[5].button.updateNonSelection(button.selection);
                this.currentItems[6].button.updateNonSelection(button.selection);
                this.currentItems[7].button.updateNonSelection(button.selection);
                this.currentItems[8].button.updateNonSelection(button.selection);
                this.currentItems[9].button.updateNonSelection(button.selection);
                this.currentItems[10].button.updateNonSelection(button.selection);
                this.currentItems[11].button.updateNonSelection(button.selection);
                this.currentItems[12].button.updateNonSelection(button.selection);
                this.currentItems[13].button.updateNonSelection(button.selection);
                this.currentItems[14].button.updateNonSelection(button.selection);
                this.currentItems[15].button.updateNonSelection(button.selection);
                this.currentItems[16].button.updateNonSelection(button.selection);
                this.currentItems[17].button.updateNonSelection(button.selection);
                break;
        }
    }

    private void KeyboardMenuConfigHorizontalSelect(OptionsGUISlot.Button button)
    {
        switch (this.verticalSelection)
        {
            case 1:
                this.currentItems[3].button.updateNonSelection(button.selection);
                this.currentItems[4].button.updateNonSelection(button.selection);
                this.currentItems[5].button.updateNonSelection(button.selection);
                this.currentItems[6].button.updateNonSelection(button.selection);
                this.currentItems[7].button.updateNonSelection(button.selection);
                this.currentItems[8].button.updateNonSelection(button.selection);
                this.currentItems[9].button.updateNonSelection(button.selection);
                this.currentItems[10].button.updateNonSelection(button.selection);
                this.currentItems[11].button.updateNonSelection(button.selection);
                this.currentItems[12].button.updateNonSelection(button.selection);
                break;
        }
    }

    protected void MenuSelectSound()
    {
        AudioManager.Play("confirm1");
    }

    protected void MenuCancelSound()
    {
        AudioManager.Play("cancel1");
    }

    public void ResetTimeSincePress()
    {
        this.timeSincePress = 0f;
    }

    private void ToggleSubMenu(OptionsGUI.State state)
    {
        this.currentItems.Clear();
        switch (state)
        {
            case OptionsGUI.State.MainOptions:
                this.mainObject.SetActive(true);
                if (this.systemObject.activeSelf)
                {
                    this.systemObject.SetActive(false);
                }
                if (this.displayObject.activeSelf)
                {
                    this.displayObject.SetActive(false);
                }
                this.audioObject.SetActive(false);
                this.configSettingsObject.SetActive(false);
                this.networkObject.SetActive(false);
                this.userConfigObject.SetActive(false);
                this.userControllerConfigObject.SetActive(false);
                this.deleteUserObject.SetActive(false);
                this.keyboardBattleConfigObject.SetActive(false);
                this.controllerMenuConfigObject.SetActive(false);
                this.keyboardMenuConfigObject.SetActive(false);
                this.currentItems.AddRange(this.mainObjectButtons);
                break;
            case OptionsGUI.State.System:
                this.mainObject.SetActive(true);
                this.systemObject.SetActive(true);
                this.displayObject.SetActive(false);
                this.audioObject.SetActive(false);
                this.configSettingsObject.SetActive(false);
                this.networkObject.SetActive(false);
                this.currentItems.AddRange(this.systemObjectButtons);
                SystemOptionSettings();
                break;
            case OptionsGUI.State.Display:
                this.mainObject.SetActive(true);
                this.systemObject.SetActive(false);
                this.displayObject.SetActive(true);
                this.audioObject.SetActive(false);
                this.configSettingsObject.SetActive(false);
                this.networkObject.SetActive(false);
                this.currentItems.AddRange(this.displayObjectButtons);
                DisplayOptionSettings();
                break;
            case OptionsGUI.State.AudioAndLanguage:
                this.mainObject.SetActive(true);
                this.systemObject.SetActive(false);
                this.displayObject.SetActive(false);
                this.audioObject.SetActive(true);
                this.configSettingsObject.SetActive(false);
                this.networkObject.SetActive(false);
                this.currentItems.AddRange(this.audioObjectButtons);
                AudioOptionSettings();
                break;
            case OptionsGUI.State.ConfigSettings:
                this.mainObject.SetActive(true);
                this.systemObject.SetActive(false);
                this.displayObject.SetActive(false);
                this.audioObject.SetActive(false);
                this.configSettingsObject.SetActive(true);
                this.networkObject.SetActive(false);
                this.userConfigObject.SetActive(false);
                this.keyboardBattleConfigObject.SetActive(false);
                this.controllerMenuConfigObject.SetActive(false);
                this.keyboardMenuConfigObject.SetActive(false);
                this.currentItems.AddRange(this.configSettingsObjectButtons);
                break;
            case OptionsGUI.State.UserConfig:
                this.mainObject.SetActive(true);
                this.systemObject.SetActive(false);
                this.displayObject.SetActive(false);
                this.audioObject.SetActive(false);
                this.configSettingsObject.SetActive(true);
                this.networkObject.SetActive(false);
                this.userConfigObject.SetActive(true);
                this.userControllerConfigObject.SetActive(false);
                this.deleteUserObject.SetActive(false);
                this.currentItems.AddRange(this.userConfigObjectButtons);
                UserConfigOptionSettings();
                break;
            case OptionsGUI.State.UserControllerConfig:
                this.mainObject.SetActive(true);
                this.systemObject.SetActive(false);
                this.displayObject.SetActive(false);
                this.audioObject.SetActive(false);
                this.configSettingsObject.SetActive(true);
                this.networkObject.SetActive(false);
                this.userConfigObject.SetActive(true);
                this.userControllerConfigObject.SetActive(true);
                this.currentItems.AddRange(this.userControllerConfigObjectButtons);
                UserControllerConfigOptionSettings();
                break;
            case OptionsGUI.State.EditUserName:
            case OptionsGUI.State.EnterNewUserName:
                this.mainObject.SetActive(true);
                this.systemObject.SetActive(false);
                this.displayObject.SetActive(false);
                this.audioObject.SetActive(false);
                this.configSettingsObject.SetActive(true);
                this.networkObject.SetActive(false);
                this.userConfigObject.SetActive(true);
                SetupEnterNewUserName(state);
                break;
            case OptionsGUI.State.DeleteUserName:
                this.mainObject.SetActive(true);
                this.systemObject.SetActive(false);
                this.displayObject.SetActive(false);
                this.audioObject.SetActive(false);
                this.configSettingsObject.SetActive(true);
                this.networkObject.SetActive(false);
                this.userConfigObject.SetActive(true);
                this.deleteUserObject.SetActive(true);
                this.currentItems.AddRange(this.deleteUserObjectButtons);
                DeleteUserOptionSettings();
                break;
            case OptionsGUI.State.KeyboardBattleConfig:
                this.mainObject.SetActive(true);
                this.systemObject.SetActive(false);
                this.displayObject.SetActive(false);
                this.audioObject.SetActive(false);
                this.configSettingsObject.SetActive(true);
                this.networkObject.SetActive(false);
                this.keyboardBattleConfigObject.SetActive(true);
                this.currentItems.AddRange(this.keyboardBattleConfigObjectButtons);
                KeyboardBattleConfigOptionSettings();
                break;
            case OptionsGUI.State.ControllerMenuConfig:
                this.mainObject.SetActive(true);
                this.systemObject.SetActive(false);
                this.displayObject.SetActive(false);
                this.audioObject.SetActive(false);
                this.configSettingsObject.SetActive(true);
                this.networkObject.SetActive(false);
                this.controllerMenuConfigObject.SetActive(true);
                this.currentItems.AddRange(this.controllerMenuConfigObjectButtons);
                ControllerMenuConfigOptionSettings();
                break;
            case OptionsGUI.State.KeyboardMenuConfig:
                this.mainObject.SetActive(true);
                this.systemObject.SetActive(false);
                this.displayObject.SetActive(false);
                this.audioObject.SetActive(false);
                this.configSettingsObject.SetActive(true);
                this.networkObject.SetActive(false);
                this.keyboardMenuConfigObject.SetActive(true);
                this.currentItems.AddRange(this.keyboardMenuConfigObjectButtons);
                KeyboardMenuConfigOptionSettings();
                break;
            case OptionsGUI.State.Network:
                this.mainObject.SetActive(true);
                this.systemObject.SetActive(false);
                this.displayObject.SetActive(false);
                this.audioObject.SetActive(false);
                this.configSettingsObject.SetActive(false);
                this.networkObject.SetActive(true);
                this.currentItems.AddRange(this.networkObjectButtons);
                NetworkOptionSettings();
                break;
        }
        if (state != OptionsGUI.State.Controls)
        {
            if (state == OptionsGUI.State.MainOptions)
            {
                if (this._mainVerticalSelection != -1)
                {
                    this.verticalSelection = this._mainVerticalSelection;
                    this._mainVerticalSelection = -1;
                }
                else
                {
                    this.verticalSelection = 0;
                }
            }
            if (state == OptionsGUI.State.DeleteUserName)
            {
                this.verticalSelection = 1;
            }
            if (state == OptionsGUI.State.ConfigSettings)
            {
                if (this._configSettingsVerticalSelection != -1)
                {
                    this.verticalSelection = this._configSettingsVerticalSelection;
                    this._configSettingsVerticalSelection = -1;
                }
                else
                {
                    this.verticalSelection = 0;
                }
            }
            if (state == OptionsGUI.State.UserConfig)
            {
                if (this._userConfigSettingsVerticalSelection != -1)
                {
                    this.verticalSelection = this._userConfigSettingsVerticalSelection;
                    this._userConfigSettingsVerticalSelection = -1;
                }
                else
                {
                    this.verticalSelection = 0;
                }
            }
            for (int i = 0; i < currentItems.Count; i++)
            {
                this.currentItems[i].button.holdHighlighter = false;
            }
            //this.UpdateVerticalSelection();
        }
    }

    public void SetToggle(OptionsGUI.State state)
    {
        this.state = state;
        this.ToggleSubMenu(state);
    }

    protected bool GetButtonDown(MirrorOfDuskButton button)
    {
        if (this.input.GetButtonDown(button))
        {
            //AudioManager.Play("confirm1");
            return true;
        }
        return false;
    }

    private int GetButtonDownAndReturnPlayer(MirrorOfDuskButton button)
    {
        return this.input.GetButtonDownAndReturnPlayer(button);
    }

    protected float GetAxis(MirrorOfDuskButton button)
    {
        return this.input.GetAxis(button);
    }

    protected bool GetButton(MirrorOfDuskButton button)
    {
        return this.input.GetButton(button);
    }

    protected bool GetAndReturnJoystickActionButtonDown(ref MirrorOfDuskButton testButton)
    {
        //MirrorOfDuskButton testButton = MirrorOfDuskButton.None;
        if (this.input.GetAndReturnJoystickActionButtonDown(ref testButton))
        {
            AudioManager.Play("select2");
            return true;
        }
        return false;
    }

    protected bool GetAndReturnKeyboardActionButtonDown(ref MirrorOfDuskButton testButton)
    {
        if (this.input.GetAndReturnKeyboardActionButtonDown(ref testButton))
        {
            AudioManager.Play("select2");
            return true;
        }
        return false;
    }

    private string ButtonReturnText(MirrorOfDuskButton testButton)
    {
        string result = String.Empty;
        switch (testButton)
        {
            case MirrorOfDuskButton.LightAttack:
                result = "X";
                break;
            case MirrorOfDuskButton.HeavyAttack:
                result = "Y";
                break;
            case MirrorOfDuskButton.SpecialAttack:
                result = "B";
                break;
            case MirrorOfDuskButton.Jump:
                result = "A";
                break;
            case MirrorOfDuskButton.Block:
                result = "RB";
                break;
            case MirrorOfDuskButton.Dodge:
                result = "LB";
                break;
            case MirrorOfDuskButton.Grab:
                result = "RT";
                break;
            case MirrorOfDuskButton.UseMirror:
                result = "LT";
                break;
            default:
                break;
        }
        return result;
    }

    public void OnPressOptionsUpDown(bool down, bool up, bool right)
    {
        int horizontalSelectionCount = 0;
        int verticalSelectionCount = 0;
        if (right && this.verticalSelection >= 2)
        {
            horizontalSelectionCount++;
        }
        if (down)
        {
            verticalSelectionCount++;
        }
        if (up)
        {
            verticalSelectionCount--;
        }
        if (verticalSelectionCount == 0 && horizontalSelectionCount == 0)
        {
            menuFirstPress--;
            menuFirstPress = Mathf.Clamp(menuFirstPress, 0, 10);
            if (menuFirstPress <= 0)
            {
                horizontalHoldTime = 0f;
                this.ResetTimeSincePress();
            }
        }
        if (timeSincePress <= 0f)
        {
            if (verticalSelectionCount != 0)
            {
                timeSincePress += 0.15f;
                if (menuFirstPress == 0)
                    timeSincePress += 0.2f;
                menuFirstPress = 3;
                horizontalHoldTime = 0f;
                AudioManager.Play("menu_scroll");
                this.verticalSelection += verticalSelectionCount;
            }
            else
            {
                if (horizontalSelectionCount != 0)
                {
                    timeSincePress += 0.15f;
                    if (menuFirstPress == 0)
                        timeSincePress += 0.2f;
                    menuFirstPress = 3;
                    this.horizontalHoldTime += Time.deltaTime;
                    this.currentItems[this.verticalSelection].button.HorizontalHoldTime = this.horizontalHoldTime;
                    if (horizontalSelectionCount > 0 && this.horizontalHoldTime >= 0.12f && SettingsData.Data.currentUserConfigProfile != 0)
                    {
                        if (UpdateUserControllerConfigEraseButton())
                        {
                            horizontalHoldTime = 0f;
                            AudioManager.Play("select2");
                            return;
                        }
                    }
                }
            }
        }
    }

    public void OnPressOptionsAccept(int selection)
    {
        switch (selection)
        {
            case 0:
                MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                this.MenuSelectSound();
                if (this.state == OptionsGUI.State.UserControllerConfig)
                {
                    UserConfigDataManager.availableUserProfiles[SettingsData.Data.currentUserConfigProfile].userConfigButtons = new UserConfigData.UserConfigProfile.UserConfigButtons(
                        this.userControllerConfigCurrentChars[0],
                        this.userControllerConfigCurrentChars[1],
                        this.userControllerConfigCurrentChars[2],
                        this.userControllerConfigCurrentChars[3],
                        this.userControllerConfigCurrentChars[4],
                        this.userControllerConfigCurrentChars[5],
                        this.userControllerConfigCurrentChars[6],
                        this.userControllerConfigCurrentChars[7],
                        this.userControllerConfigCurrentChars[8],
                        this.userControllerConfigCurrentChars[9],
                        this.userControllerConfigCurrentChars[10]);
                    this.currentPlayer.enabled = false;
                    this.currentPlayer = null;
                    this.ReturnToUserConfigMenu();
                } else if (this.state == OptionsGUI.State.ControllerMenuConfig)
                {
                    this.controllerMenuInputSet.configButtons.AssignNewElements();
                    this.currentPlayer.enabled = false;
                    this.currentPlayer = null;
                    this.ReturnToConfigSettingsMenu();
                }
                break;
            case 1:
                this.MenuSelectSound();
                if (this.state == OptionsGUI.State.UserControllerConfig)
                {
                    this.userControllerConfigCurrentChars[0] = "X";
                    this.userControllerConfigCurrentChars[1] = "Y";
                    this.userControllerConfigCurrentChars[2] = "B";
                    this.userControllerConfigCurrentChars[3] = "A";
                    this.userControllerConfigCurrentChars[4] = "RB";
                    this.userControllerConfigCurrentChars[5] = "LB";
                    this.userControllerConfigCurrentChars[6] = "--";
                    this.userControllerConfigCurrentChars[7] = "RT";
                    this.userControllerConfigCurrentChars[8] = "--";
                    this.userControllerConfigCurrentChars[9] = "--";
                    this.userControllerConfigCurrentChars[10] = "LT";
                    
                    Controller controller = PlayerManager.GetPlayerJoystick(this.currentPlayer.OG_PlayerId);
                    for (int i = 2; i <= 12; i++)
                    {
                        this.userControllerConfigObjectButtons[i].button.UpdateGlyph((controller as Joystick), UserConfigData.GetUserConfigElementIdentifierId((controller as Joystick), this.userControllerConfigCurrentChars[i - 2]), AxisRange.Positive);
                        this.userControllerConfigObjectButtons[i].button.text.text = (this.userControllerConfigObjectButtons[i].button.glyph.sprite == null) ? this.userControllerConfigCurrentChars[i - 2] : String.Empty;
                    }
                }
                else if (this.state == OptionsGUI.State.ControllerMenuConfig)
                {
                    ControllerMap controllerMap = ReInput.mapping.GetControllerMapInstance(this.controllerMenuInputSet.controller, 2, 1);

                    this.controllerMenuInputSet.configButtons = new ControllerMenuInputSet.ConfigButtons(this.controllerMenuConfigObjectButtons,
                        GetControllerConfigAEM(controllerMap, (int)MirrorOfDuskButton.Accept).elementIdentifierName,
                        GetControllerConfigAEM(controllerMap, (int)MirrorOfDuskButton.Cancel).elementIdentifierName,
                        GetControllerConfigAEM(controllerMap, (int)MirrorOfDuskButton.Edit).elementIdentifierName,
                        GetControllerConfigAEM(controllerMap, (int)MirrorOfDuskButton.ScrollLeft).elementIdentifierName,
                        GetControllerConfigAEM(controllerMap, (int)MirrorOfDuskButton.ScrollRight).elementIdentifierName,
                        GetControllerConfigAEM(controllerMap, (int)MirrorOfDuskButton.Accept).elementIdentifierId,
                        GetControllerConfigAEM(controllerMap, (int)MirrorOfDuskButton.Cancel).elementIdentifierId,
                        GetControllerConfigAEM(controllerMap, (int)MirrorOfDuskButton.Edit).elementIdentifierId,
                        GetControllerConfigAEM(controllerMap, (int)MirrorOfDuskButton.ScrollLeft).elementIdentifierId,
                        GetControllerConfigAEM(controllerMap, (int)MirrorOfDuskButton.ScrollRight).elementIdentifierId
                    );
                    this.controllerMenuInputSet.configButtons.UpdateGlyphSprite(0);
                    this.controllerMenuInputSet.configButtons.UpdateGlyphSprite(1);
                    this.controllerMenuInputSet.configButtons.UpdateGlyphSprite(2);
                    this.controllerMenuInputSet.configButtons.UpdateGlyphSprite(3);
                    this.controllerMenuInputSet.configButtons.UpdateGlyphSprite(4);
                    this.controllerMenuConfigObjectButtons[2].button.text.text = (this.controllerMenuConfigObjectButtons[2].button.glyph.sprite == null) ? this.controllerMenuInputSet.configButtons.configButtonSet[0].buttonElementChar : String.Empty;
                    this.controllerMenuConfigObjectButtons[3].button.text.text = (this.controllerMenuConfigObjectButtons[3].button.glyph.sprite == null) ? this.controllerMenuInputSet.configButtons.configButtonSet[1].buttonElementChar : String.Empty;
                    this.controllerMenuConfigObjectButtons[4].button.text.text = (this.controllerMenuConfigObjectButtons[4].button.glyph.sprite == null) ? this.controllerMenuInputSet.configButtons.configButtonSet[2].buttonElementChar : String.Empty;
                    this.controllerMenuConfigObjectButtons[5].button.text.text = (this.controllerMenuConfigObjectButtons[5].button.glyph.sprite == null) ? this.controllerMenuInputSet.configButtons.configButtonSet[3].buttonElementChar : String.Empty;
                    this.controllerMenuConfigObjectButtons[6].button.text.text = (this.controllerMenuConfigObjectButtons[6].button.glyph.sprite == null) ? this.controllerMenuInputSet.configButtons.configButtonSet[4].buttonElementChar : String.Empty;

                    this.controllerMenuInputSet.configButtons.AddDefaultElements();
                }
                break;
        }
    }

    public void OnPressOptionsCancel()
    {
        MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
        this.MenuCancelSound();
        this.currentPlayer.enabled = false;
        this.currentPlayer = null;
        if (this.state == OptionsGUI.State.UserControllerConfig)
        {
            this.ReturnToUserConfigMenu();
        }
        else if (this.state == OptionsGUI.State.ControllerMenuConfig)
        {
            this.ReturnToConfigSettingsMenu();
        }
    }

    public void OnPressOptionsConfig(MirrorOfDuskButton foundButton)
    {
        string tempKey = this.currentItems[this.verticalSelection].button.text.text;
        string newKey = ButtonReturnText(foundButton);
        for (int i = 2; i <= 12; i++)
        {
            if (this.currentItems[i].button.text.text == newKey)
            {
                this.currentItems[i].button.text.text = tempKey;
                break;
            }
        }
        this.currentItems[this.verticalSelection].button.text.text = newKey;
    }

    public void OnPressOptionsConfigId(string foundButton, int elementId)
    {
        /*string tempKey = this.currentItems[this.verticalSelection].button.text.text;
        string newKey = ButtonReturnText(foundButton);
        for (int i = 2; i <= 6; i++)
        {
            if (this.currentItems[i].button.text.text == newKey)
            {
                this.currentItems[i].button.text.text = tempKey;
                break;
            }
        }*/
        if (this.state == OptionsGUI.State.UserControllerConfig)
        {
            string tempKey = this.userControllerConfigCurrentChars[this.verticalSelection - 2];
            string newKey = foundButton;
            for (int i = 0; i <= 10; i++)
            {
                if (this.userControllerConfigCurrentChars[i] == newKey)
                {
                    this.userControllerConfigCurrentChars[i] = tempKey;
                    if (this.currentItems[i + 2].button.glyph != null)
                    {
                        this.userControllerConfigObjectButtons[i + 2].button.UpdateGlyph((currentPlayer.PlayerController as Joystick), UserConfigData.GetUserConfigElementIdentifierId((currentPlayer.PlayerController as Joystick), this.userControllerConfigCurrentChars[i]), AxisRange.Positive);
                        this.userControllerConfigObjectButtons[i + 2].button.text.text = (this.userControllerConfigObjectButtons[i + 2].button.glyph.sprite == null) ? this.userControllerConfigCurrentChars[i] : String.Empty;
                    }
                    break;
                }
            }
            this.userControllerConfigCurrentChars[this.verticalSelection - 2] = newKey;
            if (this.currentItems[this.verticalSelection].button.glyph != null)
            {
                this.userControllerConfigObjectButtons[this.verticalSelection].button.UpdateGlyph((currentPlayer.PlayerController as Joystick), UserConfigData.GetUserConfigElementIdentifierId((currentPlayer.PlayerController as Joystick), newKey), AxisRange.Positive);
                this.userControllerConfigObjectButtons[this.verticalSelection].button.text.text = (this.userControllerConfigObjectButtons[this.verticalSelection].button.glyph.sprite == null) ? newKey : String.Empty;
            }
            //this.userControllerConfigObjectButtons[i].button.UpdateGlyph((currentPlayer.PlayerController as Joystick), UserConfigData.GetUserConfigElementIdentifierId((currentPlayer.PlayerController as Joystick), t), AxisRange.Positive);
            //UserConfigDataManager.availableUserProfiles[SettingsData.Data.currentUserConfigProfile].userConfigButtons.UpdateConfig(this.verticalSelection - 2, foundButton, elementId);
            AudioManager.Play("select2");
            /*for (int i = 2; i <= 12; i++)
            {
                string t = UserConfigDataManager.availableUserProfiles[SettingsData.Data.currentUserConfigProfile].userConfigButtons.userConfigButtonSet[i - 2].buttonElementChar;
                if (this.currentItems[i].button.glyph != null)
                {
                    this.userControllerConfigObjectButtons[i].button.UpdateGlyph((currentPlayer.PlayerController as Joystick), UserConfigData.GetUserConfigElementIdentifierId((currentPlayer.PlayerController as Joystick), t), AxisRange.Positive);
                    this.userControllerConfigObjectButtons[i].button.text.text = (this.userControllerConfigObjectButtons[i].button.glyph.sprite == null) ? t : String.Empty;
                }
            }*/
            for (int i = 2; i <= 12; i++)
            {
                this.userControllerConfigObjectButtons[i].button.UpdateGlyph((currentPlayer.PlayerController as Joystick), UserConfigData.GetUserConfigElementIdentifierId((currentPlayer.PlayerController as Joystick), this.userControllerConfigCurrentChars[i - 2]), AxisRange.Positive);
                this.userControllerConfigObjectButtons[i].button.text.text = (this.userControllerConfigObjectButtons[i].button.glyph.sprite == null) ? this.userControllerConfigCurrentChars[i - 2] : String.Empty;
            }
        }
        else if (this.state == OptionsGUI.State.ControllerMenuConfig)
        {
            this.controllerMenuInputSet.configButtons.UpdateConfig(this.verticalSelection - 2, foundButton, elementId);
            AudioManager.Play("select2");
            for (int i = 2; i <= 6; i++)
            {
                string t = this.controllerMenuInputSet.configButtons.configButtonSet[i - 2].buttonElementChar;
                if (this.currentItems[i].button.glyph != null)
                {
                    this.controllerMenuInputSet.configButtons.UpdateGlyphSprite(i - 2);
                    t = (this.currentItems[i].button.glyph.sprite == null) ? t : String.Empty;
                }
                this.currentItems[i].button.text.text = t;
            }
        }
    }

    private void SystemOptionSettings()
    {
        if (SettingsData.Data.canVibrate)
        {
            this.currentItems[0].button.selection = 1;
        } else
        {
            this.currentItems[0].button.selection = 0;
        }
        this.currentItems[0].button.updateSelection(this.currentItems[0].button.selection);
        this.currentItems[1].button.selection = (int)SettingsData.Data.buttonGuide;
        this.currentItems[1].button.updateSelection(this.currentItems[1].button.selection);
        this.currentItems[2].button.selection = SettingsData.Data.keyboardUserCount - 1;
        this.currentItems[2].button.updateSelection(this.currentItems[2].button.selection);
        if (SettingsData.Data.canAutoSave)
        {
            this.currentItems[3].button.selection = 1;
        }
        else
        {
            this.currentItems[3].button.selection = 0;
        }
        this.currentItems[3].button.updateSelection(this.currentItems[3].button.selection);
    }

    private void DisplayOptionSettings()
    {
        if (SettingsData.Data.FpsDisplay)
        {
            this.currentItems[1].button.selection = 1;
        }
        else
        {
            this.currentItems[1].button.selection = 0;
        }
        this.currentItems[1].button.updateSelection(this.currentItems[1].button.selection);
        this.currentItems[2].button.updateSelection(this.currentItems[2].button.selection);
        this.currentItems[4].button.updateSelection(this.currentItems[4].button.selection);
    }

    private void AudioOptionSettings()
    {
        this.currentItems[4].button.updateSelection(this.currentItems[4].button.selection);
        this.currentItems[5].button.updateSelection((int)Localization.language);
    }

    private void NetworkOptionSettings()
    {
        this.currentItems[0].button.updateSelection(this.currentItems[0].button.selection);
        this.currentItems[1].button.updateSelection(this.currentItems[1].button.selection);
    }

    private void UserConfigOptionSettings()
    {
        if (SettingsData.Data.currentUserConfigProfile >= this.currentItems[0].button.options.Length)
            SettingsData.Data.currentUserConfigProfile = 0;
        this.currentItems[0].button.selection = SettingsData.Data.currentUserConfigProfile;
        this.currentItems[0].button.updateSelection(this.currentItems[0].button.selection);
    }

    public void UpdateUserConfigProfileSelection()
    {
        if (this.state == OptionsGUI.State.EnterNewUserName)
        {
            this.userConfigObjectButtons[0].button.options = new string[UserConfigDataManager.availableUserProfiles.Count];
            for (int i = 0; i < UserConfigDataManager.availableUserProfiles.Count; i++)
            {
                this.userConfigObjectButtons[0].button.options[i] = UserConfigDataManager.availableUserProfiles[i].userProfileName;
            }
            SettingsData.Data.currentUserConfigProfile = UserConfigDataManager.availableUserProfiles.Count - 1;
            this.userConfigObjectButtons[0].button.selection = SettingsData.Data.currentUserConfigProfile;
            this.userConfigObjectButtons[0].button.updateSelection(this.userConfigObjectButtons[0].button.selection);
        }
        else if (this.state == OptionsGUI.State.EditUserName)
        {
            this.userConfigObjectButtons[0].button.options[SettingsData.Data.currentUserConfigProfile] = UserConfigDataManager.availableUserProfiles[SettingsData.Data.currentUserConfigProfile].userProfileName;
            this.userConfigObjectButtons[0].button.updateSelection(this.userConfigObjectButtons[0].button.selection);
        }
        else if (this.state == OptionsGUI.State.DeleteUserName)
        {
            SettingsData.Data.currentUserConfigProfile -= 1;
            this.userConfigObjectButtons[0].button.options = new string[UserConfigDataManager.availableUserProfiles.Count];
            for (int i = 0; i < UserConfigDataManager.availableUserProfiles.Count; i++)
            {
                this.userConfigObjectButtons[0].button.options[i] = UserConfigDataManager.availableUserProfiles[i].userProfileName;
            }
            this.userConfigObjectButtons[0].button.selection = SettingsData.Data.currentUserConfigProfile;
            this.userConfigObjectButtons[0].button.updateNonSelection(this.userConfigObjectButtons[0].button.selection);
        }
    }

    private void UserControllerConfigOptionSettings()
    {
        userControllerConfigCurrentChars = new string[11];
        Controller controller = PlayerManager.GetPlayerJoystick(this.currentPlayer.OG_PlayerId);
        if (UserConfigDataManager.availableUserProfiles[SettingsData.Data.currentUserConfigProfile] != null)
        {
            UserConfigData.UserConfigProfile.UserConfigButtons config = UserConfigDataManager.availableUserProfiles[SettingsData.Data.currentUserConfigProfile].userConfigButtons;
            for (int i = 2; i <= 12; i++)
            {
                this.userControllerConfigObjectButtons[i].button.UpdateGlyph((controller as Joystick), UserConfigData.GetUserConfigElementIdentifierId((controller as Joystick), config.userConfigButtonSet[i - 2].buttonElementChar), AxisRange.Positive);
                this.userControllerConfigObjectButtons[i].button.text.text = (this.userControllerConfigObjectButtons[i].button.glyph.sprite == null) ? config.userConfigButtonSet[i - 2].buttonElementChar : String.Empty;
                this.userControllerConfigCurrentChars[i - 2] = config.userConfigButtonSet[i - 2].buttonElementChar;
            }
            /*this.userControllerConfigObjectButtons[2].button.UpdateGlyph((controller as Joystick), UserConfigData.GetUserConfigElementIdentifierId((controller as Joystick), config.userConfigButtonSet[0].buttonElementChar), AxisRange.Positive);
            this.userControllerConfigObjectButtons[2].button.text.text = (this.userControllerConfigObjectButtons[2].button.glyph.sprite == null) ? config.userConfigButtonSet[0].buttonElementChar : String.Empty;
            //this.userControllerConfigObjectButtons[2].button.text.text = config.userConfigButtonSet[0].buttonElementChar;
            this.userControllerConfigObjectButtons[3].button.text.text = config.userConfigButtonSet[1].buttonElementChar;
            this.userControllerConfigObjectButtons[4].button.text.text = config.userConfigButtonSet[2].buttonElementChar;
            this.userControllerConfigObjectButtons[5].button.text.text = config.userConfigButtonSet[3].buttonElementChar;
            this.userControllerConfigObjectButtons[6].button.text.text = config.userConfigButtonSet[4].buttonElementChar;
            this.userControllerConfigObjectButtons[7].button.text.text = config.userConfigButtonSet[5].buttonElementChar;
            this.userControllerConfigObjectButtons[8].button.text.text = config.userConfigButtonSet[6].buttonElementChar;
            this.userControllerConfigObjectButtons[9].button.text.text = config.userConfigButtonSet[7].buttonElementChar;
            this.userControllerConfigObjectButtons[10].button.text.text = config.userConfigButtonSet[8].buttonElementChar;
            this.userControllerConfigObjectButtons[11].button.text.text = config.userConfigButtonSet[9].buttonElementChar;
            this.userControllerConfigObjectButtons[12].button.text.text = config.userConfigButtonSet[10].buttonElementChar;*/
        }
    }

    private void SetupEnterNewUserName(OptionsGUI.State state)
    {
        if (state == OptionsGUI.State.EnterNewUserName)
        {
            newUserNameGUI.gameObject.SetActive(true);
        }
        else if (state == OptionsGUI.State.EditUserName)
        {
            editUserNameGUI.gameObject.SetActive(true);
        }
    }

    private void DeleteUserOptionSettings()
    {
        this.deleteUserTextField.text = this.userConfigObjectButtons[0].button.text.text;
        this.currentItems[0].button.updateSelection(this.currentItems[0].button.selection);
        this.currentItems[1].button.updateSelection(this.currentItems[1].button.selection);
    }

    private void RemoveThisUser()
    {
        UserConfigDataManager.DeleteCurrentProfile(this.userConfigObjectButtons[0].button.selection);
        this.UpdateUserConfigProfileSelection();
    }

    private void ControllerMenuConfigOptionSettings()
    {
        Controller controller = PlayerManager.GetPlayerJoystick(this.currentPlayer.OG_PlayerId);
        if (controller != null)
        {
            ControllerMap controllerMap = PlayerManager.GetPlayerInput(this.currentPlayer.OG_PlayerId).controllers.maps.GetMap(controller, 2, 1);
            if (controllerMap != null)
            {
                this.controllerMenuInputSet = new ControllerMenuInputSet(this.currentPlayer.OG_PlayerId, controller, controllerMap);

                this.controllerMenuInputSet.configButtons = new ControllerMenuInputSet.ConfigButtons(this.controllerMenuConfigObjectButtons,
                    GetControllerConfigAEM(controllerMap, (int)MirrorOfDuskButton.Accept).elementIdentifierName,
                    GetControllerConfigAEM(controllerMap, (int)MirrorOfDuskButton.Cancel).elementIdentifierName,
                    GetControllerConfigAEM(controllerMap, (int)MirrorOfDuskButton.Edit).elementIdentifierName,
                    GetControllerConfigAEM(controllerMap, (int)MirrorOfDuskButton.ScrollLeft).elementIdentifierName,
                    GetControllerConfigAEM(controllerMap, (int)MirrorOfDuskButton.ScrollRight).elementIdentifierName,
                    GetControllerConfigAEM(controllerMap, (int)MirrorOfDuskButton.Accept).elementIdentifierId,
                    GetControllerConfigAEM(controllerMap, (int)MirrorOfDuskButton.Cancel).elementIdentifierId,
                    GetControllerConfigAEM(controllerMap, (int)MirrorOfDuskButton.Edit).elementIdentifierId,
                    GetControllerConfigAEM(controllerMap, (int)MirrorOfDuskButton.ScrollLeft).elementIdentifierId,
                    GetControllerConfigAEM(controllerMap, (int)MirrorOfDuskButton.ScrollRight).elementIdentifierId
                );
                this.controllerMenuInputSet.configButtons.UpdateGlyphSprite(0);
                this.controllerMenuInputSet.configButtons.UpdateGlyphSprite(1);
                this.controllerMenuInputSet.configButtons.UpdateGlyphSprite(2);
                this.controllerMenuInputSet.configButtons.UpdateGlyphSprite(3);
                this.controllerMenuInputSet.configButtons.UpdateGlyphSprite(4);
                this.controllerMenuConfigObjectButtons[2].button.text.text = (this.controllerMenuConfigObjectButtons[2].button.glyph.sprite == null) ? this.controllerMenuInputSet.configButtons.configButtonSet[0].buttonElementChar : String.Empty;
                this.controllerMenuConfigObjectButtons[3].button.text.text = (this.controllerMenuConfigObjectButtons[3].button.glyph.sprite == null) ? this.controllerMenuInputSet.configButtons.configButtonSet[1].buttonElementChar : String.Empty;
                this.controllerMenuConfigObjectButtons[4].button.text.text = (this.controllerMenuConfigObjectButtons[4].button.glyph.sprite == null) ? this.controllerMenuInputSet.configButtons.configButtonSet[2].buttonElementChar : String.Empty;
                this.controllerMenuConfigObjectButtons[5].button.text.text = (this.controllerMenuConfigObjectButtons[5].button.glyph.sprite == null) ? this.controllerMenuInputSet.configButtons.configButtonSet[3].buttonElementChar : String.Empty;
                this.controllerMenuConfigObjectButtons[6].button.text.text = (this.controllerMenuConfigObjectButtons[6].button.glyph.sprite == null) ? this.controllerMenuInputSet.configButtons.configButtonSet[4].buttonElementChar : String.Empty;
            }

            
        }
        
    }

    private void KeyboardBattleConfigOptionSettings()
    {
        this.currentItems[1].button.selection = 0;
        this.currentItems[1].button.updateNonSelection(0);
        
        Controller controller = ReInput.controllers.GetController(ControllerType.Keyboard, 0);
        ControllerMap controllerMap = PlayerManager.GetPlayerInput(PlayerId.PlayerOne).controllers.maps.GetMap(controller, 1, 1);
        this.keyboardBattleInputSet[0].configKeys = new KeyboardBattleInputSet.ConfigKeys(
            GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.MoveHorizontal, Pole.Positive),
            GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.MoveHorizontal, Pole.Negative),
            GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.MoveVertical, Pole.Positive),
            GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.MoveVertical, Pole.Negative),
            GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.LightAttack),
            GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.HeavyAttack),
            GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.SpecialAttack),
            GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.Jump),
            GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.Block),
            GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.Dodge),
            GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.Taunt),
            GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.Grab),
            GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.TerrorAttack),
            GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.PlungeCancel),
            GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.UseMirror)
            );
        ControllerMap controllerMap2 = PlayerManager.GetPlayerInput(PlayerId.PlayerTwo).controllers.maps.GetMap(controller, 1, 2);
        this.keyboardBattleInputSet[1].configKeys = new KeyboardBattleInputSet.ConfigKeys(
            GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.MoveHorizontal, Pole.Positive),
            GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.MoveHorizontal, Pole.Negative),
            GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.MoveVertical, Pole.Positive),
            GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.MoveVertical, Pole.Negative),
            GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.LightAttack),
            GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.HeavyAttack),
            GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.SpecialAttack),
            GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.Jump),
            GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.Block),
            GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.Dodge),
            GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.Taunt),
            GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.Grab),
            GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.TerrorAttack),
            GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.PlungeCancel),
            GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.UseMirror)
            );
        for (int i = 0; i < 2; i++)
        {
            this.currentItems[3].button.options[i] = keyboardBattleInputSet[i].configKeys.configKeySet[0].keyElementChar;
            this.currentItems[4].button.options[i] = keyboardBattleInputSet[i].configKeys.configKeySet[1].keyElementChar;
            this.currentItems[5].button.options[i] = keyboardBattleInputSet[i].configKeys.configKeySet[2].keyElementChar;
            this.currentItems[6].button.options[i] = keyboardBattleInputSet[i].configKeys.configKeySet[3].keyElementChar;
            this.currentItems[7].button.options[i] = keyboardBattleInputSet[i].configKeys.configKeySet[4].keyElementChar;
            this.currentItems[8].button.options[i] = keyboardBattleInputSet[i].configKeys.configKeySet[5].keyElementChar;
            this.currentItems[9].button.options[i] = keyboardBattleInputSet[i].configKeys.configKeySet[6].keyElementChar;
            this.currentItems[10].button.options[i] = keyboardBattleInputSet[i].configKeys.configKeySet[7].keyElementChar;
            this.currentItems[11].button.options[i] = keyboardBattleInputSet[i].configKeys.configKeySet[8].keyElementChar;
            this.currentItems[12].button.options[i] = keyboardBattleInputSet[i].configKeys.configKeySet[9].keyElementChar;
            this.currentItems[13].button.options[i] = keyboardBattleInputSet[i].configKeys.configKeySet[10].keyElementChar;
            this.currentItems[14].button.options[i] = keyboardBattleInputSet[i].configKeys.configKeySet[11].keyElementChar;
            this.currentItems[15].button.options[i] = keyboardBattleInputSet[i].configKeys.configKeySet[12].keyElementChar;
            this.currentItems[16].button.options[i] = keyboardBattleInputSet[i].configKeys.configKeySet[13].keyElementChar;
            this.currentItems[17].button.options[i] = keyboardBattleInputSet[i].configKeys.configKeySet[14].keyElementChar;
        }
        this.currentItems[3].button.updateNonSelection(0);
        this.currentItems[4].button.updateNonSelection(0);
        this.currentItems[5].button.updateNonSelection(0);
        this.currentItems[6].button.updateNonSelection(0);
        this.currentItems[7].button.updateNonSelection(0);
        this.currentItems[8].button.updateNonSelection(0);
        this.currentItems[9].button.updateNonSelection(0);
        this.currentItems[10].button.updateNonSelection(0);
        this.currentItems[11].button.updateNonSelection(0);
        this.currentItems[12].button.updateNonSelection(0);
        this.currentItems[13].button.updateNonSelection(0);
        this.currentItems[14].button.updateNonSelection(0);
        this.currentItems[15].button.updateNonSelection(0);
        this.currentItems[16].button.updateNonSelection(0);
        this.currentItems[17].button.updateNonSelection(0);
        this.cmapInstance = controllerMap;
        this.cmapInstance2 = controllerMap2;
    }

    private void KeyboardMenuConfigOptionSettings()
    {
        this.currentItems[1].button.selection = 0;
        this.currentItems[1].button.updateNonSelection(0);

        Controller controller = ReInput.controllers.GetController(ControllerType.Keyboard, 0);
        ControllerMap controllerMap = PlayerManager.GetPlayerInput(PlayerId.PlayerOne).controllers.maps.GetMap(controller, 2, 1);

        this.keyboardMenuInputSet[0].configKeys = new KeyboardMenuInputSet.ConfigKeys(
                GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.CursorHorizontal, Pole.Positive),
                GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.CursorHorizontal, Pole.Negative),
                GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.CursorVertical, Pole.Positive),
                GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.CursorVertical, Pole.Negative),
                GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.Accept),
                GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.Cancel),
                GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.Edit),
                GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.ScrollLeft),
                GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.ScrollRight),
                GetKeyboardConfigAEM(controllerMap, (int)MirrorOfDuskButton.Pause)
                );

        ControllerMap controllerMap2 = PlayerManager.GetPlayerInput(PlayerId.PlayerTwo).controllers.maps.GetMap(controller, 2, 2);

        this.keyboardMenuInputSet[1].configKeys = new KeyboardMenuInputSet.ConfigKeys(
                GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.CursorHorizontal, Pole.Positive),
                GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.CursorHorizontal, Pole.Negative),
                GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.CursorVertical, Pole.Positive),
                GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.CursorVertical, Pole.Negative),
                GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.Accept),
                GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.Cancel),
                GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.Edit),
                GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.ScrollLeft),
                GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.ScrollRight),
                GetKeyboardConfigAEM(controllerMap2, (int)MirrorOfDuskButton.Pause)
            );


        for (int i = 0; i < 2; i++)
        {
            this.currentItems[3].button.options[i] = keyboardMenuInputSet[i].configKeys.configKeySet[0].keyElementChar;
            this.currentItems[4].button.options[i] = keyboardMenuInputSet[i].configKeys.configKeySet[1].keyElementChar;
            this.currentItems[5].button.options[i] = keyboardMenuInputSet[i].configKeys.configKeySet[2].keyElementChar;
            this.currentItems[6].button.options[i] = keyboardMenuInputSet[i].configKeys.configKeySet[3].keyElementChar;
            this.currentItems[7].button.options[i] = keyboardMenuInputSet[i].configKeys.configKeySet[4].keyElementChar;
            this.currentItems[8].button.options[i] = keyboardMenuInputSet[i].configKeys.configKeySet[5].keyElementChar;
            this.currentItems[9].button.options[i] = keyboardMenuInputSet[i].configKeys.configKeySet[6].keyElementChar;
            this.currentItems[10].button.options[i] = keyboardMenuInputSet[i].configKeys.configKeySet[7].keyElementChar;
            this.currentItems[11].button.options[i] = keyboardMenuInputSet[i].configKeys.configKeySet[8].keyElementChar;
            this.currentItems[12].button.options[i] = keyboardMenuInputSet[i].configKeys.configKeySet[9].keyElementChar;
        }
        this.currentItems[3].button.updateNonSelection(0);
        this.currentItems[4].button.updateNonSelection(0);
        this.currentItems[5].button.updateNonSelection(0);
        this.currentItems[6].button.updateNonSelection(0);
        this.currentItems[7].button.updateNonSelection(0);
        this.currentItems[8].button.updateNonSelection(0);
        this.currentItems[9].button.updateNonSelection(0);
        this.currentItems[10].button.updateNonSelection(0);
        this.currentItems[11].button.updateNonSelection(0);
        this.currentItems[12].button.updateNonSelection(0);
        this.cmapInstance = controllerMap;
        this.cmapInstance2 = controllerMap2;
    }

    private ActionElementMap GetKeyboardConfigAEM(ControllerMap cmap, int button)
    {
        ActionElementMap aem = cmap.GetFirstButtonMapWithAction(button);
        return aem;
    }

    private ActionElementMap GetKeyboardConfigAEM(ControllerMap cmap, int button, Pole pole)
    {
        foreach (ActionElementMap aem in cmap.GetElementMapsWithAction(button))
        {
            if (aem != null)
            {
                if (pole == aem.axisContribution)
                {
                    return aem;
                }
            }
        }
        return null;
    }

    private ActionElementMap GetControllerConfigAEM(ControllerMap cmap, int button)
    {
        ActionElementMap aem = cmap.GetFirstButtonMapWithAction(button);
        return aem;
    }

    /*private string GetKeyboardConfigCode(ControllerMap cmap, int button)
    {
        ActionElementMap aem = cmap.GetFirstButtonMapWithAction(button);
        if (aem != null)
            return Keyboard.GetKeyName(aem.keyCode);
        return "--";
    }

    private string GetKeyboardConfigCode(ControllerMap cmap, int button, Pole pole)
    {
        foreach (ActionElementMap aem in cmap.GetElementMapsWithAction(button))
        {
            if (aem != null)
            {
                if (pole == aem.axisContribution)
                {
                    return Keyboard.GetKeyName(aem.keyCode);
                }
            }
        }
        return "--";
    }*/

    private void PollKeyboardForAssignment(out ControllerPollingInfo pollingInfo, out string keyLabel, out KeyCode keyCodeLabel)
    {
        pollingInfo = default(ControllerPollingInfo);
        keyCodeLabel = KeyCode.None;
        keyLabel = String.Empty;
        ControllerPollingInfo controllerPollingInfo = default(ControllerPollingInfo);
        foreach(ControllerPollingInfo controllerPollingInfo3 in ReInput.controllers.Keyboard.PollForAllKeys())
        {
            KeyCode keyboardKey = controllerPollingInfo3.keyboardKey;
            if (keyboardKey != KeyCode.AltGr)
            {
                if (controllerPollingInfo.keyboardKey == KeyCode.None)
                {
                    controllerPollingInfo = controllerPollingInfo3;
                }
            }
        }
        if (!ReInput.controllers.Keyboard.GetKeyDown(controllerPollingInfo.keyboardKey))
        {
            return;
        }
        pollingInfo = controllerPollingInfo;
        keyLabel = Keyboard.GetKeyName(pollingInfo.keyboardKey);
        keyCodeLabel = pollingInfo.keyboardKey;
    }

    private void FindTextLocalizer(LocalizationHelper localizationHelper, TextMeshProUGUI tmp, string buttonText, LocalizationHelper.LocalizationSubtext[] subtext = null)
    {
        if (localizationHelper != null)
        {
            TranslationElement translationElement = Localization.Find(buttonText);
            if (translationElement != null)
            {
                localizationHelper.ApplyTranslation(translationElement, subtext);
                return;
            }
        }
        tmp.text = buttonText;
    }

    private void ScrollDetails()
    {
        if (DetailsScroller != null)
            this.StopCoroutine(this.DetailsScroller);
        this.DetailsScroller = scrollDetails_cr(264f);
        base.StartCoroutine(this.DetailsScroller);
    }

    private IEnumerator scrollDetails_cr(float offset)
    {
        float width = menuDetailsText.preferredWidth;
        Vector3 startPosition = menuDetailsText.rectTransform.position;

        float scrollPosition = 0f;
        float canvPosition = menuDetailsCanvas.rect.x;

        for (; ; )
        {
            menuDetailsText.rectTransform.position = new Vector3((-scrollPosition) + canvPosition + offset, startPosition.y, startPosition.z);
            /*if (textUpdated)
            {
                width = m_detailsTextObject.preferredWidth;
                textUpdated = false;
            }*/

            bool scrollNow = false;

            if ((width - (scrollPosition + 5f)) > (2060 - offset))
            {
                scrollNow = true;
            }

            yield return new WaitForSeconds(2);

            while (scrollNow)
            {
                menuDetailsText.rectTransform.position = new Vector3((-scrollPosition) + canvPosition + offset, startPosition.y, startPosition.z);
                scrollPosition += 5f;
                if ((width - (scrollPosition + 4f)) < (1920 - offset))
                {
                    scrollNow = false;
                    scrollPosition = 0f;
                    yield return new WaitForSeconds(2);
                    //menuDetailsText.rectTransform.position = new Vector3((-scrollPosition) + canvPosition + offset, startPosition.y, startPosition.z);
                }
                yield return null;
            }
        }
    }
}
