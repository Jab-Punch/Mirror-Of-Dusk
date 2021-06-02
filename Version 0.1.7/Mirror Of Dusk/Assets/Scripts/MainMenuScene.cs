using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScene : AbstractMB
{

    [SerializeField] private MainMenuPlayer playerOne;
    [SerializeField] private MainMenuPlayer playerTwo;
    [SerializeField] private MainMenuPlayer playerThree;
    [SerializeField] private MainMenuPlayer playerFour;

    [Header("Items")]
    [SerializeField] private MainMenuItem mainMenuTitleItem;
    [SerializeField] private List<MainMenuItems> mainMenuModes;
    [SerializeField] private List<MainMenuItem> mainMenuSubItems;
    [SerializeField] private MainMenuHand mainMenuHand;
    [SerializeField] private OptionsGUI optionsPrefab;
    [SerializeField] private RectTransform optionsRoot;
    [SerializeField] public RawImage tintScreen;
    [SerializeField] public SpriteRenderer dewOne;
    [SerializeField] public SpriteRenderer dewTwo;
    private List<MainMenuItem> items;
    private OptionsGUI options;

    public List<MainMenuItem> Items
    {
        get { return items; }
    }

    [Space(10f)]
    [Header("Background Groups")]
    [SerializeField] public MainMenuSections currentSection;
    private MainMenuSections previousSection;
    private MainMenuSections formerCurrentSection;
    [SerializeField] public MainMenuBackgroundGroup[] mainMenuBackgroundGroups;

    public MainMenuSections CurrentSection {
        get {
            return this.currentSection;
        }
        set
        {
            this.currentSection = value;
        }
    }

    public MainMenuSections PreviousSection
    {
        get
        {
            return this.previousSection;
        }
        set
        {
            this.previousSection = value;
        }
    }

    [Space(10f)]
    [SerializeField] private TextMeshProUGUI displayNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private LocalizationHelper mainDetailLocalizationHelper;
    [SerializeField] private LocalizationHelper subDetailLocalizationHelper;

    private Player input;
    public MainMenuScene.State state;
    private int mainTitleIndex = 0;
    private int subTitleIndex;
    private bool firstStart = true;
    private bool menuFirstPress = false;
    private int setMenuDirection = 0;
    private float timeSincePress = 0f;
    //private MainMenuPlayer playerCommandBusy;
    private PlayerCommandBusy playerCommandBusy;
    
    public class PlayerCommandBusy
    {
        public MainMenuPlayer player = null;
        public int command = -1;

        public PlayerCommandBusy()
        {
            this.player = null;
            this.command = -1;
        }

        public PlayerCommandBusy(MainMenuPlayer player, int command)
        {
            this.player = player;
            this.command = command;
        }
    }

    private bool sceneLoaderExists = false;
    private MainMenuScene.UserConfigDataStatus userConfigDataStatus;

    public enum State
    {
        Init,
        Selecting,
        OptionsBusy,
        OptionsSelecting,
        Forwarding,
        Backing,
        Exiting
    }

    private enum UserConfigDataStatus
    {
        Uninitialized,
        Received,
        Initialized
    }

    [Serializable]
    public class MainMenuItems
    {
        public MainMenuItemMainType mainMenuMainType;
        public MainMenuItemSubType mainMenuSubType;
        public MainMenuSections mainMenuSection;
        public MainMenuSections mainMenuPreviousSection;
        public List<MainMenuSubItems> mainMenuSubItems;
    }

    [Serializable]
    public class MainMenuSubItems
    {
        public MainMenuItemSubType mainMenuSubType;
        public MainMenuSections mainMenuSection;
        public Scenes mainMenuEnterScene = Scenes.scene_main_menu;
    }

    public static MainMenuScene Current { get; private set; }

    public MainMenuItem CurrentItem
    {
        get
        {
            this.subTitleIndex = (subTitleIndex + this.items.Count) % this.items.Count;
            //this.subTitleIndex = Mathf.Clamp(this.subTitleIndex, 0, this.items.Count - 1);
            return this.items[this.subTitleIndex];
        }
    }

    public delegate void GroupTravelDelegate();
    public event GroupTravelDelegate OnGroupTravelEvent;
    public event GroupTravelDelegate OnGroupRetreatEvent;

    public bool OnGroupTravelEventFilled
    {
        get
        {
            return OnGroupTravelEvent == null;
        }
    }

    [SerializeField] private LocalizationHelper[] fontTest;

    protected override void Awake()
    {
        base.Awake();
        MirrorOfDusk.Init(false);
        MainMenuScene.Current = this;
        if (!MainMenuData.Initialized)
            MainMenuData.Init();
        /*this.options = this.optionsPrefab.InstantiatePrefab<OptionsGUI>();
        this.options.rectTransform.SetParent(this.optionsRoot, false);
        this.options.Init(false);*/
        for (int i = 0; i < mainMenuBackgroundGroups.Length; i++)
        {
            mainMenuBackgroundGroups[i].gameObject.SetActive(true);
            mainMenuBackgroundGroups[i].Init();
        }
        CreateSection();
        this.mainTitleIndex = 0;
        if (mainMenuTitleItem.mainMenuType != MainMenuItem.MainMenuItemType.Main)
        {
            mainMenuTitleItem = null;
        }
        /*if (mainMenuModes[mainTitleIndex].mainMenuTitleItem.mainMenuType != MainMenuItem.MainMenuItemType.Main)
        {
            mainMenuModes[mainTitleIndex] = null;
        }*/
        for (int i = 0; i < this.mainMenuModes.Count; i++)
        {
            MainMenuItem mainMenuItem = null;
            if (mainMenuModes[i].mainMenuMainType != MainMenuItemMainType.None)
            {
                MainMenuItem.MainMenuItemType itemType = this.mainMenuTitleItem.mainMenuType;
                if (itemType == MainMenuItem.MainMenuItemType.Main)
                {
                    mainMenuItem = this.mainMenuTitleItem;
                }
            }
            if (mainMenuItem != null)
            {
                if (mainMenuModes[i].mainMenuSection == MainMenuData.Data.CurrentSectionData.sectionPlacement)
                {
                    this.mainMenuTitleItem.mainMenuItemMainType = mainMenuModes[i].mainMenuMainType;
                    this.mainMenuTitleItem.mainMenuItemSection = mainMenuModes[i].mainMenuSection;
                    this.mainMenuTitleItem.mainMenuPreviousSection = mainMenuModes[i].mainMenuPreviousSection;
                    this.mainTitleIndex = i;
                    break;
                }
            }
        }
        items = new List<MainMenuItem>();
        /*for (int i = 0; i < this.mainMenuSubItems.Count; i++)
        {
            items.Add(this.mainMenuSubItems[i]);
        }*/
        this.subTitleIndex = 0;
        for (int i = 0; i < this.mainMenuModes[mainTitleIndex].mainMenuSubItems.Count; i++)
        {
            MainMenuItem mainMenuItem = null;
            if (mainMenuModes[mainTitleIndex].mainMenuSubItems[i].mainMenuSubType != MainMenuItemSubType.None)
            {
                MainMenuItem.MainMenuItemType itemType = this.mainMenuSubItems[i].mainMenuType;
                if (itemType == MainMenuItem.MainMenuItemType.Sub)
                {
                    mainMenuItem = this.mainMenuSubItems[i];
                    this.subTitleIndex++;
                }
            }
            if (mainMenuItem != null)
            {
                this.mainMenuSubItems[i].mainMenuItemSubType = mainMenuModes[mainTitleIndex].mainMenuSubItems[i].mainMenuSubType;
                this.mainMenuSubItems[i].mainMenuItemSection = mainMenuModes[mainTitleIndex].mainMenuSubItems[i].mainMenuSection;
                this.mainMenuSubItems[i].mainMenuEnterScene = mainMenuModes[mainTitleIndex].mainMenuSubItems[i].mainMenuEnterScene;
                this.items.Add(this.mainMenuSubItems[i]);
            }
        }
        for (int i = 0; i < mainMenuHand.shardBackgroundCollection.Count; i++)
        {
            if (this.mainMenuModes[mainTitleIndex].mainMenuSubType == mainMenuHand.shardBackgroundCollection[i].key)
            {
                this.tintScreen.color = mainMenuHand.shardBackgroundCollection[i].tint;
                break;
            }
        }
        AssignPrevSection();
        if (SceneLoader.Exists)
        {
            sceneLoaderExists = true;
            SceneLoader.OnFadeOutEndEvent += this.OnLoaded;
        } else
        {
            //this.OnLoaded();
        }
        if (MainMenuScene.Current.OnGroupTravelEvent != null)
        {

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        this.subTitleIndex = MainMenuData.Data.ItemPosition;
        playerCommandBusy = new PlayerCommandBusy();
        mainMenuHand.SetShardBackground(CurrentItem.mainMenuItemSubType);
        UserConfigData.Init(new UserConfigData.UserConfigDataInitHandler(this.OnUserConfigDataInitialized));
        //Localization.ExportCsv("D:\\Jesse's Portfolio\\Mirror Of Dusk Content\\Main Game\\Builds\\LocalizAsset\\LocAsset.csv");
    }

    // Update is called once per frame
    void Update()
    {
        if (this.userConfigDataStatus == MainMenuScene.UserConfigDataStatus.Received)
        {
            this.userConfigDataStatus = MainMenuScene.UserConfigDataStatus.Initialized;
            base.StartCoroutine(this.allDataLoaded_cr());
        }
        timeSincePress -= Time.deltaTime;
        timeSincePress = Mathf.Clamp(timeSincePress, 0f, 1000f);
        ResetTimeSincePress();
        if (this.state == MainMenuScene.State.Selecting)
        {
            if (this.playerOne.CheckUpdate && this.playerTwo.CheckUpdate && this.playerThree.CheckUpdate && this.playerFour.CheckUpdate)
            {
                ExecuteMenuAcceptInput();
                ExecuteMenuCancelInput();
                ExecuteMenuUpDownInput();
                playerOne.DirectionBusy = false;
                playerTwo.DirectionBusy = false;
                playerThree.DirectionBusy = false;
                playerFour.DirectionBusy = false;
                playerCommandBusy = new PlayerCommandBusy();
                playerOne.CheckUpdate = false;
                playerTwo.CheckUpdate = false;
                playerThree.CheckUpdate = false;
                playerFour.CheckUpdate = false;
            }
        }
        else if(this.state == MainMenuScene.State.OptionsSelecting)
        {
            if (this.playerOne.CheckUpdate && this.playerTwo.CheckUpdate && this.playerThree.CheckUpdate && this.playerFour.CheckUpdate)
            {
                ExecuteOptionConfigInput();
                /*ExecuteMenuAcceptInput();
                ExecuteMenuCancelInput();
                ExecuteMenuUpDownInput();*/
                playerOne.DirectionBusy = false;
                playerTwo.DirectionBusy = false;
                playerThree.DirectionBusy = false;
                playerFour.DirectionBusy = false;
                playerCommandBusy = new PlayerCommandBusy();
                playerOne.CheckUpdate = false;
                playerTwo.CheckUpdate = false;
                playerThree.CheckUpdate = false;
                playerFour.CheckUpdate = false;
            }
        }
        else
        {
            playerOne.DirectionBusy = false;
            playerTwo.DirectionBusy = false;
            playerThree.DirectionBusy = false;
            playerFour.DirectionBusy = false;
            playerCommandBusy = new PlayerCommandBusy();
            playerOne.CheckUpdate = false;
            playerTwo.CheckUpdate = false;
            playerThree.CheckUpdate = false;
            playerFour.CheckUpdate = false;
        }
        if (this.state == MainMenuScene.State.Exiting)
        {

        }
    }

    private void OnDestroy()
    {
        if (MainMenuScene.Current == this)
        {
            MainMenuScene.Current = null;
        }
        if (SceneLoader.Exists)
        {
            sceneLoaderExists = true;
        }
        if (sceneLoaderExists)
        {
            SceneLoader.OnFadeOutEndEvent -= this.OnLoaded;
        }
        this.playerOne.OnMenuUpDownEvent -= this.OnPressMenuUpDown;
        this.playerTwo.OnMenuUpDownEvent -= this.OnPressMenuUpDown;
        this.playerThree.OnMenuUpDownEvent -= this.OnPressMenuUpDown;
        this.playerFour.OnMenuUpDownEvent -= this.OnPressMenuUpDown;
        this.playerOne.OnMenuAcceptEvent -= this.OnPressMenuAccept;
        this.playerTwo.OnMenuAcceptEvent -= this.OnPressMenuAccept;
        this.playerThree.OnMenuAcceptEvent -= this.OnPressMenuAccept;
        this.playerFour.OnMenuAcceptEvent -= this.OnPressMenuAccept;
        this.playerOne.OnMenuCancelEvent -= this.OnPressMenuCancel;
        this.playerTwo.OnMenuCancelEvent -= this.OnPressMenuCancel;
        this.playerThree.OnMenuCancelEvent -= this.OnPressMenuCancel;
        this.playerFour.OnMenuCancelEvent -= this.OnPressMenuCancel;
    }

    private void OnUserConfigDataLoaded()
    {
        /*this.pig.OnStart();
        this.playerOne.OnStart();
        this.playerTwo.OnStart();*/
        base.StartCoroutine(menuItemsAppear_cr());
        this.playerOne.OnStart();
        //this.playerOne.OnPressDownEvent += this.OnPressMenuDown;
        this.playerTwo.OnStart();
        //this.playerTwo.OnPressDownEvent += this.OnPressMenuDown;
        this.playerThree.OnStart();
        //this.playerThree.OnPressDownEvent += this.OnPressMenuDown;
        this.playerFour.OnStart();
        //this.playerFour.OnPressDownEvent += this.OnPressMenuDown;
        InterruptingPrompt.SetCanInterrupt(false);
    }

    private void OnLoaded()
    {
        /*this.pig.OnStart();
        this.playerOne.OnStart();
        this.playerTwo.OnStart();*/
        /*base.StartCoroutine(menuItemsAppear_cr());
        this.playerOne.OnStart();
        //this.playerOne.OnPressDownEvent += this.OnPressMenuDown;
        this.playerTwo.OnStart();
        //this.playerTwo.OnPressDownEvent += this.OnPressMenuDown;
        this.playerThree.OnStart();
        //this.playerThree.OnPressDownEvent += this.OnPressMenuDown;
        this.playerFour.OnStart();
        //this.playerFour.OnPressDownEvent += this.OnPressMenuDown;
        InterruptingPrompt.SetCanInterrupt(true);*/
    }

    private void OnUserConfigDataInitialized(bool success)
    {
        if (!success)
        {
            UserConfigData.Init(new UserConfigData.UserConfigDataInitHandler(this.OnUserConfigDataInitialized));
            return;
        }
        if (PlatformHelper.IsConsole && !PlatformHelper.PreloadSettingsData)
        {
            SettingsData.LoadFromCloud(new SettingsData.SettingsDataLoadFromCloudHandler(this.OnSettingsDataLoaded));
        }
        else
        {
            this.userConfigDataStatus = MainMenuScene.UserConfigDataStatus.Received;
        }
    }

    private void OnSettingsDataLoaded(bool success)
    {
        if (!success)
        {
            SettingsData.LoadFromCloud(new SettingsData.SettingsDataLoadFromCloudHandler(this.OnSettingsDataLoaded));
            return;
        }
        SettingsData.ApplySettingsOnStartup();
        base.StartCoroutine(this.allDataLoaded_cr());
    }

    private IEnumerator allDataLoaded_cr()
    {
        yield return null;
        //ControllerDisconnectedPrompt.Instance.allowedToShow = true;
        this.options = this.optionsPrefab.InstantiatePrefab<OptionsGUI>();
        this.options.rectTransform.SetParent(this.optionsRoot, false);
        this.options.Init(false);
        this.OnUserConfigDataLoaded();
        /*if (PlatformHelper.ShowAchievements)
        {
            this.achievements = this.achievementsPrefab.InstantiatePrefab<AchievementsGUI>();
            this.achievements.rectTransform.SetParent(this.achievementsRoot, false);
            this.achievements.Init(false);
        }
        if (PlatformHelper.IsConsole)
        {
            PlayerManager.LoadControllerMappings(PlayerId.PlayerOne);
        }
        this.SetRichPresence();*/
        yield break;
    }

    private void CreateSection()
    {
        if (!MainMenuData.Data.CurrentSectionData.sessionStarted)
        {
            MainMenuData.Data.CurrentSectionData.sessionStarted = true;
            MainMenuData.Data.CurrentSectionData.sectionPlacement = MainMenuSections.Default;
        }
        CurrentSection = MainMenuData.Data.CurrentSectionData.sectionPlacement;
    }

    private void AssignPrevSection()
    {
        if (this.mainMenuTitleItem == null)
        {
            PreviousSection = MainMenuSections.None;
            return;
        }
        PreviousSection = this.mainMenuTitleItem.mainMenuPreviousSection;
    }

    public void SetPlayerStates(MainMenuPlayer.State state)
    {
        playerOne.state = state;
        playerTwo.state = state;
        playerThree.state = state;
        playerFour.state = state;
    }

    public void UpdateMenuItems(bool setInitialSlot = false)
    {
        for (int i = 0; i < mainMenuModes.Count; i++)
        {
            if (mainMenuModes[i].mainMenuSection == CurrentSection)
            {
                mainTitleIndex = i;
                break;
            }
        }
        this.mainMenuTitleItem.mainMenuItemMainType = mainMenuModes[mainTitleIndex].mainMenuMainType;
        this.mainMenuTitleItem.mainMenuItemSection = mainMenuModes[mainTitleIndex].mainMenuSection;
        this.mainMenuTitleItem.mainMenuPreviousSection = mainMenuModes[mainTitleIndex].mainMenuPreviousSection;
        items = new List<MainMenuItem>();
        this.subTitleIndex = 0;
        int tempNextSubTitleIndex = 0;
        for (int i = 0; i < this.mainMenuModes[mainTitleIndex].mainMenuSubItems.Count; i++)
        {
            MainMenuItem mainMenuItem = null;
            if (mainMenuModes[mainTitleIndex].mainMenuSubItems[i].mainMenuSubType != MainMenuItemSubType.None)
            {
                MainMenuItem.MainMenuItemType itemType = this.mainMenuSubItems[i].mainMenuType;
                if (itemType == MainMenuItem.MainMenuItemType.Sub)
                {
                    mainMenuItem = this.mainMenuSubItems[i];
                    if (setInitialSlot)
                    {
                        if (this.formerCurrentSection == mainMenuModes[mainTitleIndex].mainMenuSubItems[i].mainMenuSection)
                        {
                            tempNextSubTitleIndex = i;
                        }
                    }
                    this.subTitleIndex++;
                }
            }
            if (mainMenuItem != null)
            {
                this.mainMenuSubItems[i].mainMenuItemSubType = mainMenuModes[mainTitleIndex].mainMenuSubItems[i].mainMenuSubType;
                this.mainMenuSubItems[i].mainMenuItemSection = mainMenuModes[mainTitleIndex].mainMenuSubItems[i].mainMenuSection;
                this.mainMenuSubItems[i].mainMenuEnterScene = mainMenuModes[mainTitleIndex].mainMenuSubItems[i].mainMenuEnterScene;
                this.items.Add(this.mainMenuSubItems[i]);
            }
        }
        this.subTitleIndex = tempNextSubTitleIndex;
        AssignPrevSection();
        mainMenuHand.SetShardBackground(CurrentItem.mainMenuItemSubType);
        if (!setInitialSlot)
            mainMenuHand.TriggerEnterHand();
        base.StartCoroutine(menuItemsAppear_cr());
    }

    private IEnumerator menuItemsAppear_cr()
    {
        if (mainDetailLocalizationHelper != null)
        {
            mainDetailLocalizationHelper.currentID = (CurrentItem.DisplayId != -1) ? CurrentItem.DisplayId : mainDetailLocalizationHelper.currentID;
        }
        if (subDetailLocalizationHelper != null)
        {
            subDetailLocalizationHelper.currentID = (CurrentItem.DescriptionId != -1) ? CurrentItem.DescriptionId : subDetailLocalizationHelper.currentID;
        }
        displayNameText.text = CurrentItem.DescName;
        descriptionText.text = CurrentItem.Description;
        mainMenuTitleItem.Init();
        yield return new WaitForSeconds(0.1f);
        this.CurrentItem.selectState = MainMenuItem.SelectState.Selected;
        foreach (MainMenuItem mainMenuItem2 in this.items)
        {
            mainMenuItem2.Init();
            yield return new WaitForSeconds(0.05f);
        }
        base.StartCoroutine(itemsReady_cr());
        yield return null;
        yield break;
    }

    private IEnumerator menuItemsDisappear_cr()
    {
        mainMenuTitleItem.SendItemBack();
        yield return new WaitForSeconds(0.1f);
        //this.CurrentItem.selectState = MainMenuItem.SelectState.Selected;
        foreach (MainMenuItem mainMenuItem2 in this.items)
        {
            mainMenuItem2.SendItemBack();
            yield return new WaitForSeconds(0.05f);
        }
        yield return null;
        yield break;
    }

    private IEnumerator itemsReady_cr()
    {
        bool allReady = false;
        bool[] eachReady = new bool[this.items.Count];
        for (int b = 0; b < eachReady.Length; b++)
        {
            eachReady[b] = false;
        }
        while (!allReady)
        {
            for (int i = 0; i < this.items.Count; i++)
            {
                if (this.items[i].state == MainMenuItem.State.Ready)
                {
                    eachReady[i] = true;
                }
            }
            allReady = true;
            for (int j = 0; j < eachReady.Length; j++)
            {
                if (!eachReady[j])
                {
                    allReady = false;
                }
            }
            if (this.mainMenuHand.state != MainMenuHand.State.Ready)
            {
                allReady = false;
            }
            yield return null;
        }
        this.state = MainMenuScene.State.Selecting;
        yield return null;
        yield break;
    }

    public void OnPressMenuUpDown(MainMenuPlayer player, int dir)
    {
        if ((!this.playerOne.DirectionBusy && !this.playerTwo.DirectionBusy && !this.playerThree.DirectionBusy && !this.playerFour.DirectionBusy))
        {
            setMenuDirection = dir;
        }
        player.DirectionBusy = true;
    }

    public void OnPressMenuAccept(MainMenuPlayer player, int command)
    {
        /*if ((!this.playerOne.CommandBusy && !this.playerTwo.CommandBusy && !this.playerThree.CommandBusy && !this.playerFour.CommandBusy))
        {
            setMenuDirection = dir;
        }
        player.CommandBusy = true;*/
        if (playerCommandBusy.player == null)
        {
            playerCommandBusy = new PlayerCommandBusy(player, command);
        }
    }

    public void OnPressMenuCancel(MainMenuPlayer player, int command)
    {
        if (playerCommandBusy.player == null)
        {
            playerCommandBusy = new PlayerCommandBusy(player, command);
        }
    }

    public void OnPressOptionsConfig(MainMenuPlayer player, int command)
    {
        if (playerCommandBusy.player == null)
        {
            playerCommandBusy = new PlayerCommandBusy(player, command);
        }
    }

    private void ExecuteMenuUpDownInput()
    {
        if (setMenuDirection != 0 && timeSincePress <= 0f && this.state == MainMenuScene.State.Selecting)
        {
            this.mainMenuHand.SetShardBackground(this.CurrentItem.mainMenuItemSubType);
            this.subTitleIndex += setMenuDirection;
            timeSincePress += 0.15f;
            if (!menuFirstPress)
                timeSincePress += 0.2f;
            for (int i = 0; i < this.items.Count; i++)
            {
                this.items[i].selectState = MainMenuItem.SelectState.NotSelected;
            }
            this.CurrentItem.selectState = MainMenuItem.SelectState.Selected;
            if (mainDetailLocalizationHelper != null)
            {
                mainDetailLocalizationHelper.currentID = (CurrentItem.DisplayId != -1) ? CurrentItem.DisplayId : mainDetailLocalizationHelper.currentID;
            }
            if (subDetailLocalizationHelper != null)
            {
                subDetailLocalizationHelper.currentID = (CurrentItem.DescriptionId != -1) ? CurrentItem.DescriptionId : subDetailLocalizationHelper.currentID;
            }
            displayNameText.text = CurrentItem.DescName;
            descriptionText.text = CurrentItem.Description;
            this.mainMenuHand.PlayGleamScroll();
            menuFirstPress = true;
        } else
        {
            if (timeSincePress <= 0f || this.state != MainMenuScene.State.Selecting)
            {
                menuFirstPress = false;
            }
        }
        setMenuDirection = 0;
    }

    private void ExecuteMenuAcceptInput()
    {
        if (playerCommandBusy.player != null)
        {
            if (playerCommandBusy.command == (int)MirrorOfDuskButton.Accept && this.state == MainMenuScene.State.Selecting && this.mainMenuSubItems[subTitleIndex].mainMenuItemSection != MainMenuSections.None)
            {
                AudioManager.Play("confirm1");
                if (CurrentItem.mainMenuItemSection == MainMenuSections.Options)
                {
                    this.state = MainMenuScene.State.OptionsBusy;
                    SetPlayerStates(MainMenuPlayer.State.Options);
                    this.options.ShowMainOptionMenu();
                    return;
                }
                if (CurrentItem.mainMenuItemSection == MainMenuSections.Enter)
                {
                    MainMenuData.Data.CurrentSection = this.mainMenuSubItems[subTitleIndex].mainMenuEnterScene;
                    if (!MainMenuData.Data.CurrentSectionData.sessionStarted)
                    {
                        MainMenuData.Data.CurrentSectionData.sessionStarted = true;
                        MainMenuData.Data.CurrentSectionData.sectionPlacement = CurrentSection;
                    }
                    MainMenuData.Data.ItemPosition = this.subTitleIndex;
                }
                CurrentSection = CurrentItem.mainMenuItemSection;
                this.state = MainMenuScene.State.Forwarding;
                this.mainMenuHand.PlayGleamSelect();
                base.StartCoroutine(menuItemsDisappear_cr());
            }
        }
    }

    private void ExecuteMenuCancelInput()
    {
        if (playerCommandBusy.player != null)
        {
            if (playerCommandBusy.command == (int)MirrorOfDuskButton.Cancel && this.state == MainMenuScene.State.Selecting)
            {
                AudioManager.Play("cancel1");
                formerCurrentSection = CurrentSection;
                CurrentSection = mainMenuTitleItem.mainMenuPreviousSection;
                if (mainMenuTitleItem.mainMenuPreviousSection != MainMenuSections.None)
                {
                    this.state = MainMenuScene.State.Backing;
                    this.mainMenuHand.TriggerExitHand();
                    base.StartCoroutine(menuItemsDisappear_cr());
                } else
                {
                    this.state = MainMenuScene.State.Exiting;
                    MainMenuData.Data.CurrentSection = Scenes.scene_title;
                    if (!MainMenuData.Data.CurrentSectionData.sessionStarted)
                    {
                        MainMenuData.Data.CurrentSectionData.sessionStarted = true;
                        MainMenuData.Data.CurrentSectionData.sectionPlacement = CurrentSection;
                    }
                    MainMenuData.Data.ItemPosition = 0;
                    base.StartCoroutine(exitScene_cr());
                }
            }
        }
    }

    private void ExecuteOptionConfigInput()
    {
        if (playerCommandBusy.player != null)
        {
            if (playerCommandBusy.command == (int)MirrorOfDuskButton.Accept && this.state == MainMenuScene.State.Selecting && this.mainMenuSubItems[subTitleIndex].mainMenuItemSection != MainMenuSections.None)
            {
                AudioManager.Play("confirm1");
                if (CurrentItem.mainMenuItemSection == MainMenuSections.Options)
                {
                    this.state = MainMenuScene.State.OptionsBusy;
                    SetPlayerStates(MainMenuPlayer.State.Options);
                    this.options.ShowMainOptionMenu();
                    return;
                }
                if (CurrentItem.mainMenuItemSection == MainMenuSections.Enter)
                {
                    MainMenuData.Data.CurrentSection = this.mainMenuSubItems[subTitleIndex].mainMenuEnterScene;
                    if (!MainMenuData.Data.CurrentSectionData.sessionStarted)
                    {
                        MainMenuData.Data.CurrentSectionData.sessionStarted = true;
                        MainMenuData.Data.CurrentSectionData.sectionPlacement = CurrentSection;
                    }
                    MainMenuData.Data.ItemPosition = this.subTitleIndex;
                }
                CurrentSection = CurrentItem.mainMenuItemSection;
                this.state = MainMenuScene.State.Forwarding;
                this.mainMenuHand.PlayGleamSelect();
                base.StartCoroutine(menuItemsDisappear_cr());
            }
        }
    }

    public void ResetTimeSincePress()
    {
        if (!this.playerOne.DirectionBusy && !this.playerTwo.DirectionBusy && !this.playerThree.DirectionBusy && !this.playerFour.DirectionBusy)
        {
            this.timeSincePress = 0f;
        }
    }

    public void Travel()
    {
        if (this.OnGroupTravelEvent != null)
        {
            this.OnGroupTravelEvent();
        }
    }

    public void Retreat()
    {
        if (this.OnGroupRetreatEvent != null)
        {
            this.OnGroupRetreatEvent();
        }
    }

    public void SummonNextBackground()
    {
        for (int i = 0; i < mainMenuBackgroundGroups.Length; i++)
        {
            if (mainMenuBackgroundGroups[i].AssignedSectionFound)
            {
                mainMenuBackgroundGroups[i].ExecuteFinishTravel();
                break;
            }
        }
    }

    public void SummonPrevBackground()
    {
        for (int i = 0; i < mainMenuBackgroundGroups.Length; i++)
        {
            if (mainMenuBackgroundGroups[i].AssignedSectionFound)
            {
                mainMenuBackgroundGroups[i].ExecuteFinishRetreat(formerCurrentSection);
                break;
            }
        }
    }

    public void EnterNextScene()
    {
        base.StartCoroutine(this.enterScene_cr());
    }

    private IEnumerator enterScene_cr()
    {
        yield return new WaitForSeconds(0.3f);
        SceneLoader.LoadScene(MainMenuData.Data.CurrentSectionData.id, SceneLoader.Transition.Fade, SceneLoader.Transition.Fade, SceneLoader.Icon.Shard);
        yield return null;
        yield break;
    }

    private IEnumerator exitScene_cr()
    {
        yield return new WaitForSeconds(1.5f);
        SceneLoader.LoadScene("scene_title", SceneLoader.Transition.Fade, SceneLoader.Transition.Fade, SceneLoader.Icon.Shard);
        yield return null;
        yield break;
    }

    public MainMenuItemSubType RetrievePreviousSelection
    {
        get
        {
            for (int i = 0; i < mainMenuModes.Count; i++)
            {
                if (mainMenuModes[i].mainMenuSection == CurrentSection)
                {
                    for (int j = 0; i < mainMenuModes[i].mainMenuSubItems.Count; j++)
                    {
                        if (mainMenuModes[i].mainMenuSubItems[j].mainMenuSection == formerCurrentSection)
                        {
                            return mainMenuModes[i].mainMenuSubItems[j].mainMenuSubType;
                        }
                    }
                }
            }
            return MainMenuItemSubType.None;
        }
    }
}
