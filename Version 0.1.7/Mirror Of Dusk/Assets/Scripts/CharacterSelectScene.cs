using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectScene : AbstractMB
{
    public static CharacterSelectScene Current { get; private set; }

    [Header("Player")]
    [SerializeField] private CharacterSelectPlayer[] player;
    [SerializeField] private Vector3 playerOnePosition = Vector3.zero;
    [SerializeField] private Vector3 playerTwoPosition = Vector3.zero;
    [SerializeField] private Vector3 playerThreePosition = Vector3.zero;
    [SerializeField] private Vector3 playerFourPosition = Vector3.zero;
    [NonSerialized] public PlayerMode[] playerModes;
    [NonSerialized] public PlayerData[] playerData;
    [NonSerialized] public CharacterSelectCursorPlayerController[] playerCursors;
    [NonSerialized] public CharacterSelectCursorStar[] playerCursorStars;
    [NonSerialized] public CharacterSelectPlayerGUI[] playerGUIs;
    [NonSerialized] public CharacterSelectCharacterShard[] playerCharacterShards;

    [Header("Items")]
    [SerializeField] public SpriteRenderer faderBackground;
    [SerializeField] private GameObject csTitleBorder;
    [SerializeField] private CanvasGroup csTitleCanvas;
    [SerializeField] private TextMeshProUGUI csTitleCanvasText;
    [SerializeField] private GameObject csRulesBorder;
    [SerializeField] private GameObject csBackBorder;
    public CharacterStarSlot[] characterSelectSlot;

    [Header("Resources")]
    [SerializeField] public CharacterSelectCursorPlayerController characterSelectCursorPlayerController;
    [SerializeField] public CharacterSelectCursorStar characterSelectCursorStar;
    [SerializeField] public CharacterSelectPlayerGUI characterSelectPlayerGUI;
    [SerializeField] public CharacterSelectCharacterShard characterSelectCharacterShard;
    [SerializeField] public CharacterIconResources characterIconData;

    [Space(10f)]
    [NonSerialized] public CharacterSelectScene.State state;
    private CharacterSelectScene.UserConfigDataStatus userConfigDataStatus;

    private System.Random r;
    private List<int> randomizedCharacterCount;
    private IEnumerator co_BeginSummoningStars;
    private IEnumerator co_FadeBlackness;
    private IEnumerator co_MoveTitleBorder;
    private IEnumerator co_MoveRulesBorder;
    private IEnumerator co_MoveBackBorder;
    private IEnumerator co_ReadTitleText;

    private class TTData
    {
        public int code;
        public int fontSize;
        public string info;
    }
    private Dictionary<BattleMode, List<TTData>> titleTextData;

    public enum State
    {
        Init,
        Opening,
        Setup,
        Selecting,
        OptionsBusy,
        OptionsSelecting,
        Exiting,
        Exit
    }

    private enum UserConfigDataStatus
    {
        Uninitialized,
        Received,
        Initialized
    }

    public enum PlayerMode
    {
        Offline,
        Player,
        CPU
    }

    [Serializable]
    public class PlayerData
    {
        public int characterSelectedId = -1;
        public int characterColorCode = 0;
        public int playerHealth = 3000;
        public int playerShardsHeld = 50;
        public int playerShardStrength = 150;

        public PlayerData()
        {
            this.characterSelectedId = -1;
            this.characterColorCode = 0;
            this.playerHealth = 2000;
            this.playerShardsHeld = 50;
            this.playerShardStrength = 150;
            if (BattleSettingsData.Data.initialized)
            {
                this.playerHealth = BattleSettingsData.Data.battleModeSettings[CharacterSelectData.Data.CurrentMode].defaultInitialHealth;
                this.playerShardsHeld = BattleSettingsData.Data.battleModeSettings[CharacterSelectData.Data.CurrentMode].defaultInitialShards;
                this.playerShardStrength = BattleSettingsData.Data.battleModeSettings[CharacterSelectData.Data.CurrentMode].defaultShardStrength;
            }
        }

        public int GetCharacterId
        {
            get
            {
                if (characterSelectedId >= CharacterSelectScene.Current.characterIconData.data.Length || characterSelectedId == 0 || characterSelectedId == -1)
                {
                    return 0;
                }
                return characterSelectedId;
            }
        }
    }

    [NonSerialized] public int totalShardsLeft;

    

    [Serializable]
    public class CharacterStarSlot
    {
        public CharacterSelectStarSlot starSlot;
        public bool unlocked;
    }

    public delegate void OnCursorStarSelectEventHandler(int csId, CharacterSelectCursorPlayerController cursor);
    public event OnCursorStarSelectEventHandler OnCursorStarSelectEvent;

    public delegate void OnCharacterSelectEventHandler(int csId, int selectedId, bool click);
    public event OnCharacterSelectEventHandler OnCharacterSelectEvent;

    public delegate void OnReviewTotalShardsLeftEventHandler(int amount);
    public event OnReviewTotalShardsLeftEventHandler OnReviewTotalShardsLeftEvent;

    protected override void Awake()
    {
        base.Awake();
        MirrorOfDusk.Init(false);
        CharacterSelectScene.Current = this;
        if (!CharacterSelectData.Initialized)
            CharacterSelectData.Init();
        if (DEBUG_AssetLoaderManager.debugWasFound)
        {
            CharacterSelectData.Data.CurrentMode = BattleMode.Training;
            CharacterSelectData.Data.AssignDefaultStartPlayerCursorPositions(CharacterSelectData.Data.CurrentMode);
            CharacterSelectData.Data.AssignStartPlayerCursorPositions();
        }
        SceneLoader.OnFadeOutStartEvent += this.OnLoaded;
        PlayerManager.OnPlayerLeaveEvent += this.OnPlayerLeaving;
    }

    private void OnLoaded(float eTime)
    {
        UserConfigData.Init(new UserConfigData.UserConfigDataInitHandler(this.OnUserConfigDataInitialized));
        
    }

    private void OnAllDataLoaded()
    {
        PlayerManager.allowAutomaticControllerSearch = true;
        this.CreatePlayerCursors();
        InitializeTitleText();

        for (int i = 0; i < CharacterSelectData.Data.GetPlayerSlotCount; i++)
        {
            this.player[i].OnStart();
        }
        InterruptingPrompt.SetCanInterrupt(false);
        randomizedCharacterCount = new List<int>();
        co_FadeBlackness = fadeBlackness_cr();
        co_MoveTitleBorder = moveCSTitleBorder_cr();
        co_MoveRulesBorder = moveCSRulesBorder_cr();
        co_MoveBackBorder = moveCSBackBorder_cr();
        co_ReadTitleText = changeTitleText_cr();
        this.state = CharacterSelectScene.State.Opening;
        this.BeginRandomizingStars();
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
            BattleSettingsData.LoadFromCloud(new BattleSettingsData.BattleSettingsDataLoadFromCloudHandler(this.OnBattleSettingsDataLoaded));
        }
        else
        {
            this.userConfigDataStatus = CharacterSelectScene.UserConfigDataStatus.Received;
        }
    }

    private void OnBattleSettingsDataLoaded(bool success)
    {
        if (!success)
        {
            BattleSettingsData.LoadFromCloud(new BattleSettingsData.BattleSettingsDataLoadFromCloudHandler(this.OnBattleSettingsDataLoaded));
            return;
        }
        BattleSettingsData.ApplySettingsOnStartup();
        base.StartCoroutine(this.allDataLoaded_cr());
    }

    private IEnumerator allDataLoaded_cr()
    {
        yield return null;
        this.OnAllDataLoaded();
        yield break;
    }

    private void OnPlayerLeaving(PlayerId player)
    {
        int pId = (int)player;
        this.playerGUIs[pId].DeselectCharacter();
        this.playerCursorStars[pId].DisableThisCursorStar();
        CharacterSelectData.Data.GetPlayerSlot(pId).joinState = CharacterSelectData.PlayerSlot.JoinState.NotJoining;
        this.playerModes[pId] = CharacterSelectScene.PlayerMode.Offline;
        this.player[pId].BeginDisableState(45);
        if (this.playerCharacterShards[pId].gameObject.activeSelf)
            this.playerCharacterShards[pId].DisableThisShard();
        if (this.playerCursors[pId].gameObject.activeSelf)
            this.playerCursors[pId].DisableThisCursor();
        PlayerManager.SetPlayerWaitForJoinRequest(player);
    }
    
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.userConfigDataStatus == CharacterSelectScene.UserConfigDataStatus.Received)
        {
            this.userConfigDataStatus = CharacterSelectScene.UserConfigDataStatus.Initialized;
            BattleSettingsData.LoadFromCloud(new BattleSettingsData.BattleSettingsDataLoadFromCloudHandler(this.OnBattleSettingsDataLoaded));
            //base.StartCoroutine(this.allDataLoaded_cr());
        }
        if (this.state == State.Exiting)
        {
            this.state = State.Exit;
            PlayerManager.allowAutomaticControllerSearch = false;
            base.StartCoroutine(exitScene_cr());
        }
    }

    private void CreatePlayerCursors()
    {
        this.playerModes = new PlayerMode[4];
        this.playerData = new PlayerData[4] {
            new PlayerData(),
            new PlayerData(),
            new PlayerData(),
            new PlayerData()
        };
        this.totalShardsLeft = BattleSettingsData.Data.battleModeSettings[CharacterSelectData.Data.CurrentMode].defaultTotalShards;
        this.TallyRemainingShards();
        this.playerCursors = new CharacterSelectCursorPlayerController[4];
        this.playerCursorStars = new CharacterSelectCursorStar[4];
        this.playerGUIs = new CharacterSelectPlayerGUI[4];
        this.playerCharacterShards = new CharacterSelectCharacterShard[4];
        this.playerModes[0] = PlayerMode.Player;
        this.playerModes[1] = PlayerMode.CPU;
        this.playerModes[2] = PlayerMode.Offline;
        this.playerModes[3] = PlayerMode.Offline;
        for (int i = 0; i < CharacterSelectData.Data.GetPlayerSlotCount; i++)
        {
            this.playerGUIs[i] = CharacterSelectPlayerGUI.Create(player[i], new CharacterSelectPlayerGUI.InitObject(
                    CharacterSelectData.Data.GetGuiRestPos(i), i));
            this.playerCursorStars[i] = CharacterSelectCursorStar.Create(player[i], new CharacterSelectCursorStar.InitObject(
                    CharacterSelectData.Data.GetPlayerSlot(i).cursorStarPosition, CharacterSelectData.Data.GetPlayerSlotProperties(i).cursorBodyColor, CharacterSelectData.Data.GetPlayerSlotProperties(i).cursorStarEdgeColor,
                    CharacterSelectData.Data.GetPlayerSlotProperties(i).cursorStarLabelColor, CharacterSelectData.Data.GetPlayerSlotProperties(4).cursorBodyColor, CharacterSelectData.Data.GetPlayerSlotProperties(4).cursorStarEdgeColor,
                    CharacterSelectData.Data.GetPlayerSlotProperties(4).cursorStarLabelColor, CharacterSelectData.Data.GetCursorStarHeldStatus(i)));
            this.playerCursors[i] = CharacterSelectCursorPlayerController.Create(player[i], new CharacterSelectCursorPlayerController.InitObject(
                    CharacterSelectData.Data.GetPlayerSlot(i).playerCursorPosition, CharacterSelectData.Data.GetPlayerSlotProperties(i).cursorBodyColor, CharacterSelectData.Data.GetPlayerSlotProperties(4).cursorBodyColor, CharacterSelectData.Data.GetInitialCursorStarHeldId(i, CharacterSelectData.Data.GetCursorStarHeldStatus(i))));
            this.playerCharacterShards[i] = CharacterSelectCharacterShard.Create(player[i], new CharacterSelectCharacterShard.InitObject(
                    CharacterSelectData.Data.GetCharacterShardRestPos(i)));
            /*if (CharacterSelectData.Data.GetPlayerSlot(i).joinState == CharacterSelectData.PlayerSlot.JoinState.Joined)
            {
                
            }*/
        }
        CharacterSelectData.Data.SetCursorStarHeldStatus(0, true);
        CharacterSelectData.Data.SetCursorStarHeldStatus(1, false);
        for (int i = 0; i < CharacterSelectData.Data.GetPlayerSlotCount; i++)
        {
            this.playerCursorStars[i].Held = CharacterSelectData.Data.GetCursorStarHeldStatus(i);
            this.playerCursors[i].stats.Holding = CharacterSelectData.Data.GetInitialCursorStarHeldId(i, CharacterSelectData.Data.GetCursorStarHeldStatus(i));
        }
    }

    private void OnDestroy()
    {
        if (CharacterSelectScene.Current == this)
        {
            CharacterSelectScene.Current = null;
        }
        for (int i = 0; i < CharacterSelectData.Data.GetPlayerSlotCount; i++)
        {
            this.player[i].OnCSAcceptEvent -= this.OnPressCSAccept;
        }
        SceneLoader.OnFadeOutStartEvent -= this.OnLoaded;
        PlayerManager.OnPlayerLeaveEvent -= this.OnPlayerLeaving;
    }

    public CharacterSelectPlayer GetPlayer(int i)
    {
        return player[i];
    }

    #region Opening Sequences
    private IEnumerator fadeBlackness_cr()
    {
        float alpha = 1;
        float fadeEndValue = 0;
        float fadeSpeed = 1f;
        while (alpha >= fadeEndValue)
        {
            faderBackground.color = new Color(faderBackground.color.r, faderBackground.color.g, faderBackground.color.b, alpha);
            alpha += Time.deltaTime * (fadeSpeed) * -1;
            yield return null;
        }
        faderBackground.enabled = false;
        yield break;
    }

    private void BeginRandomizingStars()
    {
        r = new System.Random();
        int randomIndex = 0;
        int characterStarCount = characterSelectSlot.Length;
        List<int> characterCount = new List<int>();
        for (int i = 0; i < characterStarCount; i++)
        {
            characterCount.Add(i);
        }
        while (characterCount.Count > 0)
        {
            randomIndex = r.Next(0, characterCount.Count);
            randomizedCharacterCount.Add(characterCount[randomIndex]);
            characterCount.RemoveAt(randomIndex);
        }
        co_BeginSummoningStars = beginSummoningStars_cr();
        StartCoroutine(co_BeginSummoningStars);
    }

    private void OnDisable()
    {
        /*if (OnCursorStarSelectEvent != null)
            CharacterSelectScene.Current.OnCursorStarSelectEvent -= this.OnCursorStarSelect;*/
    }

    private IEnumerator beginSummoningStars_cr()
    {
        yield return new WaitForSeconds(1);
        int starGrow = 0;
        this.GrowCSStar(randomizedCharacterCount[starGrow++], "Birth");
        yield return new WaitForSeconds(0.5f);
        this.GrowCSStar(randomizedCharacterCount[starGrow++], "Birth");
        yield return new WaitForSeconds(0.4f);
        this.GrowCSStar(randomizedCharacterCount[starGrow++], "Birth");
        yield return new WaitForSeconds(0.3f);
        this.GrowCSStar(randomizedCharacterCount[starGrow++], "Birth");
        yield return new WaitForSeconds(0.2f);
        while (starGrow < randomizedCharacterCount.Count)
        {
            this.GrowCSStar(randomizedCharacterCount[starGrow++], "Birth");
            yield return new WaitForSeconds(0.1f);
        }
        starGrow = 0;
        while (starGrow < randomizedCharacterCount.Count)
        {
            this.characterSelectSlot[randomizedCharacterCount[starGrow]].starSlot.RevealName();
            this.GrowCSStar(randomizedCharacterCount[starGrow++], "Grow");
            yield return new WaitForSeconds(0.1f);
            if (starGrow == 1)
            {
                StartCoroutine(co_FadeBlackness);
                StartCoroutine(co_MoveTitleBorder);
                StartCoroutine(co_MoveRulesBorder);
                StartCoroutine(co_MoveBackBorder);
                StartCoroutine(co_ReadTitleText);
            }
        }
        yield break;
    }

    private void GrowCSStar(int starNum, string state)
    {
        characterSelectSlot[starNum].starSlot.spriteAnimator.Play(state);
        characterSelectSlot[starNum].starSlot.spriteAnimator2.Play(state);
        characterSelectSlot[starNum].starSlot.maskAnimator.Play(state);
    }

    private void GrowCSStar(int starNum, string state, int startFrame)
    {
        characterSelectSlot[starNum].starSlot.spriteAnimator.Play(state, startFrame);
        characterSelectSlot[starNum].starSlot.spriteAnimator2.Play(state, startFrame);
        characterSelectSlot[starNum].starSlot.maskAnimator.Play(state, startFrame);
    }

    private IEnumerator moveCSTitleBorder_cr()
    {
        int wFrame = 20;
        while (wFrame > 0)
        {
            wFrame--;
            yield return null;
        }
        float posY = csTitleBorder.transform.position.y;
        while (posY > 466)
        {
            posY -= 10f;
            if (posY < 466) posY = 466f;
            csTitleBorder.transform.position = new Vector3(csTitleBorder.transform.position.x, posY, 0);
            yield return null;
        }
        this.state = CharacterSelectScene.State.Setup;
        yield return null;
        this.SetPlayerGUIs(CharacterSelectPlayerGUI.SummonState.Opening);
        this.SetPlayerShards(CharacterSelectCharacterShard.SummonState.Opening);
        this.SetCurrentJoiningCursors();
        base.FrameDelayedCallback(new Action(this.FinishingSetupState), 40);
        yield break;
    }

    private IEnumerator moveCSRulesBorder_cr()
    {
        int wFrame = 20;
        while (wFrame > 0)
        {
            wFrame--;
            yield return null;
        }
        float posX = csRulesBorder.transform.position.x;
        while (posX > 875.5f)
        {
            posX -= 10f;
            if (posX < 875.5f) posX = 875.5f;
            csRulesBorder.transform.position = new Vector3(posX, csRulesBorder.transform.position.y, 0);
            yield return null;
        }
        yield break;
    }

    private IEnumerator moveCSBackBorder_cr()
    {
        int wFrame = 20;
        while (wFrame > 0)
        {
            wFrame--;
            yield return null;
        }
        float posX = csBackBorder.transform.position.x;
        while (posX < -875.5f)
        {
            posX += 10f;
            if (posX > -875.5f) posX = -875.5f;
            csBackBorder.transform.position = new Vector3(posX, csBackBorder.transform.position.y, 0);
            yield return null;
        }
        yield break;
    }
    #endregion

    public IEnumerator changeTitleText_cr()
    {
        float alpha = 1f;
        int currentRead = 0;
        csTitleCanvasText.text = titleTextData[CharacterSelectData.Data.CurrentMode][currentRead].info;
        csTitleCanvasText.fontSize = titleTextData[CharacterSelectData.Data.CurrentMode][currentRead].fontSize;
        for (; ; )
        {
            yield return new WaitForSeconds(3.3f);
            currentRead = (currentRead + 1) % titleTextData[CharacterSelectData.Data.CurrentMode].Count;
            while (alpha > 0)
            {
                alpha -= 0.1f;
                if (alpha < 0) alpha = 0f;
                csTitleCanvas.alpha = alpha;
                yield return null;
            }
            //theText.text = ttInfo[currentRead];
            //theText.fontSize = ttFontSize[currentRead];
            csTitleCanvasText.text = titleTextData[CharacterSelectData.Data.CurrentMode][currentRead].info;
            csTitleCanvasText.fontSize = titleTextData[CharacterSelectData.Data.CurrentMode][currentRead].fontSize;
            while (alpha < 1)
            {
                alpha += 0.1f;
                if (alpha > 1) alpha = 1f;
                csTitleCanvas.alpha = alpha;
                yield return null;
            }
        }
        yield break;
    }

    private void InitializeTitleText()
    {
        titleTextData = new Dictionary<BattleMode, List<TTData>>();
        titleTextData.Add(BattleMode.Training, new List<TTData>() {
            new TTData { code = 0, fontSize = 46, info = "TRAINING MODE" },
            new TTData { code = 1, fontSize = 36, info = "CHOOSE THE DEMENTED SOUL" },
            new TTData { code = 2, fontSize = 46, info = "TIME - ENDLESS" }
        });
        titleTextData.Add(BattleMode.Arcade, new List<TTData>() {
            new TTData { code = 0, fontSize = 46, info = "ARCADE MODE" },
            new TTData { code = 1, fontSize = 36, info = "CHOOSE THE DEMENTED SOUL" },
            new TTData { code = 2, fontSize = 46, info = "DIFFICULTY - NORMAL" }
        });
        titleTextData.Add(BattleMode.Story, new List<TTData>() {
            new TTData { code = 0, fontSize = 46, info = "STORY MODE" },
            new TTData { code = 1, fontSize = 36, info = "CHOOSE THE DEMENTED SOUL" },
            new TTData { code = 2, fontSize = 46, info = "DIFFICULTY - NORMAL" }
        });
        titleTextData.Add(BattleMode.Multiplayer, new List<TTData>() {
            new TTData { code = 0, fontSize = 46, info = "VERSUS MODE" },
            new TTData { code = 1, fontSize = 36, info = "CHOOSE THE DEMENTED SOUL" },
            new TTData { code = 2, fontSize = 46, info = "TIME - ENDLESS" }
        });
        titleTextData.Add(BattleMode.Online, new List<TTData>() {
            new TTData { code = 0, fontSize = 46, info = "ONLINE MODE" },
            new TTData { code = 1, fontSize = 36, info = "CHOOSE THE DEMENTED SOUL" },
            new TTData { code = 2, fontSize = 46, info = "TIME - ENDLESS" }
        });
    }

    private void SetPlayerGUIs(CharacterSelectPlayerGUI.SummonState summoningState)
    {
        for (int i = 0; i < this.playerGUIs.Length; i++)
        {
            if (summoningState == CharacterSelectPlayerGUI.SummonState.Opening)
            {
                this.FrameDelayedCallbackIntTwo(new Action<int, int>(this.SetThisPlayerGUI), i, (int)summoningState, (i * 5));
            } else
            {
                this.playerGUIs[i].summonState = summoningState;
                this.playerGUIs[i].gameObject.SetActive(true);
            }
        }
    }

    private void SetPlayerShards(CharacterSelectCharacterShard.SummonState summoningState)
    {
        for (int i = 0; i < this.playerCharacterShards.Length; i++)
        {
            if (CharacterSelectData.Data.GetPlayerSlot(i).joinState == CharacterSelectData.PlayerSlot.JoinState.Joined && playerModes[i] != PlayerMode.Offline)
            {
                if (summoningState == CharacterSelectCharacterShard.SummonState.Opening)
                {
                    this.FrameDelayedCallbackIntTwo(new Action<int, int>(this.SetThisPlayerShard), i, (int)summoningState, (i * 5));
                }
                else
                {
                    this.playerCharacterShards[i].summonState = summoningState;
                    this.playerCharacterShards[i].gameObject.SetActive(true);
                }
            }
            else if (CharacterSelectData.Data.GetPlayerSlot(i).joinState != CharacterSelectData.PlayerSlot.JoinState.Joined && playerModes[i] == PlayerMode.CPU)
            {
                if (summoningState == CharacterSelectCharacterShard.SummonState.Opening)
                {
                    this.FrameDelayedCallbackIntTwo(new Action<int, int>(this.SetThisPlayerShard), i, (int)summoningState, (i * 5));
                }
                else
                {
                    this.playerCharacterShards[i].summonState = summoningState;
                    this.playerCharacterShards[i].gameObject.SetActive(true);
                }
            }
        }
    }

    private void SetThisPlayerGUI(int guiId, int summoningState)
    {
        this.playerGUIs[guiId].summonState = (CharacterSelectPlayerGUI.SummonState)summoningState;
        this.playerGUIs[guiId].gameObject.SetActive(true);
    }

    private void SetThisPlayerShard(int shardId, int summoningState)
    {
        this.playerCharacterShards[shardId].summonState = (CharacterSelectCharacterShard.SummonState)summoningState;
        this.playerCharacterShards[shardId].gameObject.SetActive(true);
    }

    private void SetCurrentJoiningCursors()
    {
        for (int i = 0; i < this.playerCursors.Length; i++)
        {
            if (CharacterSelectData.Data.GetPlayerSlot(i).joinState == CharacterSelectData.PlayerSlot.JoinState.Joined && playerModes[i] != PlayerMode.Offline)
            {
                this.playerCursors[i].gameObject.SetActive(true);
                this.playerCursorStars[i].gameObject.SetActive(true);
            }
            else if(CharacterSelectData.Data.GetPlayerSlot(i).joinState != CharacterSelectData.PlayerSlot.JoinState.Joined && playerModes[i] == PlayerMode.CPU)
            {
                this.playerCursorStars[i].Held = false;
                this.playerCursors[i].stats.Holding = -1;
                this.playerCursorStars[i].gameObject.SetActive(true);
                this.player[i].state = CharacterSelectPlayer.State.Disabled;
            } else
            {
                this.player[i].state = CharacterSelectPlayer.State.Disabled;
            }
        }
    }

    public void SetJoiningCursor(PlayerId player, bool IsPlayer)
    {
        int pId = (int)player;
        if (IsPlayer)
        {
            PlayerManager.SetPlayerJoinState(player, 2);
            CharacterSelectData.Data.GetPlayerSlot(pId).joinState = CharacterSelectData.PlayerSlot.JoinState.Joined;
            this.playerCursorStars[pId].Held = true;
            this.playerCursors[pId].stats.Holding = (int)player;
            this.playerCharacterShards[pId].gameObject.SetActive(true);
            this.playerCursors[pId].gameObject.SetActive(true);
            this.playerCursorStars[pId].gameObject.SetActive(true);
            this.playerGUIs[pId].ChangePlayerIconStateTrigger(CursorHitboxPriority.GUIPlayer);
        } else
        {
            this.playerCursorStars[pId].OnCursorStarRelease();
            this.playerCursors[pId].stats.Holding = -1;
            this.playerCharacterShards[pId].gameObject.SetActive(true);
            this.playerCursorStars[pId].gameObject.SetActive(true);
            this.playerGUIs[pId].DefaultCharacterSelect((int)player, -1);
            this.playerGUIs[pId].ChangePlayerIconStateTrigger(CursorHitboxPriority.GUICPU);
        }
    }

    public void SetJoiningCursorFromCPU(PlayerId player)
    {
        for (int i = 0; i < this.playerCursors.Length; i++)
        {
            if (i == (int)player)
                continue;
            if (this.playerCursors[i].gameObject.activeSelf)
            {
                if (this.playerCursors[i].stats.Holding == (int)player)
                {
                    this.playerCursorStars[(int)player].retreatToStart = true;
                    this.playerCursorStars[(int)player].OnCursorStarRelease();
                    this.playerCursors[i].stats.Holding = -1;
                    this.playerCursors[(int)player].stats.Holding = -1;
                    this.playerGUIs[(int)player].DefaultCharacterSelect((int)player, -1);
                    break;
                }
            }
        }
        this.playerCursorStars[(int)player].ChangeModeColor();
        PlayerManager.SetPlayerJoinState(player, 2);
        CharacterSelectData.Data.GetPlayerSlot((int)player).joinState = CharacterSelectData.PlayerSlot.JoinState.Joined;
        this.playerCursors[(int)player].gameObject.SetActive(true);
        this.playerGUIs[(int)player].ChangePlayerIconStateTrigger(CursorHitboxPriority.GUIPlayer);
    }

    private void FinishingSetupState()
    {
        this.state = CharacterSelectScene.State.Selecting;
        for (int i = 0; i < CharacterSelectData.Data.GetPlayerSlotCount; i++)
        {
            if (this.player[i].state != CharacterSelectPlayer.State.Disabled)
                this.player[i].state = CharacterSelectPlayer.State.Selecting;
        }
    }

    public void OnPressCSAccept(CharacterSelectPlayer player, int command)
    {
        if (this.state == CharacterSelectScene.State.Opening)
        {
            if (co_BeginSummoningStars != null)
            {
                this.StopCoroutine(co_BeginSummoningStars);
                this.StopCoroutine(co_FadeBlackness);
                faderBackground.enabled = false;
                for (int i = 0; i < characterSelectSlot.Length; i++)
                {
                    characterSelectSlot[i].starSlot.HaltRevealName();
                }
                int skipFrame = 0;
                for (int i = 0; i < randomizedCharacterCount.Count; i++)
                {
                    this.GrowCSStar(randomizedCharacterCount[i], "Out", skipFrame);
                    skipFrame = (skipFrame + 3) % 4;
                }
                this.StopCoroutine(co_MoveTitleBorder);
                this.StopCoroutine(co_MoveRulesBorder);
                this.StopCoroutine(co_MoveBackBorder);
                this.StopCoroutine(co_ReadTitleText);
                csTitleBorder.transform.position = new Vector3(csTitleBorder.transform.position.x, 466f, 0);
                csRulesBorder.transform.position = new Vector3(875.5f, csRulesBorder.transform.position.y, 0);
                csBackBorder.transform.position = new Vector3(-875.5f, csBackBorder.transform.position.y, 0);
                this.StartCoroutine(co_ReadTitleText);
                this.state = CharacterSelectScene.State.Setup;
                this.SetPlayerGUIs(CharacterSelectPlayerGUI.SummonState.Instant);
                this.SetPlayerShards(CharacterSelectCharacterShard.SummonState.Instant);
                this.SetCurrentJoiningCursors();
                base.FrameDelayedCallback(new Action(this.FinishingSetupState), 20);
                co_BeginSummoningStars = null;
            }
        }
    }

    public void OnCursorStarSelectEventTrigger(int csId, CharacterSelectCursorPlayerController cursor)
    {
        OnCursorStarSelectEvent(csId, cursor);
    }

    public void ReleaseCursorStar(int cursorStar)
    {
        this.playerCursorStars[cursorStar].OnCursorStarRelease();
    }

    public void OnCharacterSelectEventTrigger(int csId, int selectedId, bool click)
    {
        if (OnCharacterSelectEvent != null)
        {
            OnCharacterSelectEvent(csId, selectedId, click);
        }
    }

    public void SetPlayerJoinState(int id, int state)
    {
        PlayerManager.SetPlayerJoinState((PlayerId)id, state);
    }

    public void TallyRemainingShards()
    {
        for (int i = 0; i < this.playerData.Length; i++)
        {
            if (this.playerData[i].playerShardsHeld > 0)
                this.totalShardsLeft -= this.playerData[i].playerShardsHeld;
        }
    }

    public void UpdateShardsLeft(int amount)
    {
        this.totalShardsLeft += amount;
        if (OnReviewTotalShardsLeftEvent != null)
            this.OnReviewTotalShardsLeftEvent(this.totalShardsLeft);
    }

    private IEnumerator exitScene_cr()
    {
        yield return new WaitForSeconds(1.5f);
        SceneLoader.LoadScene("scene_main_menu", SceneLoader.Transition.Fade, SceneLoader.Transition.Fade, SceneLoader.Icon.Shard);
        yield return null;
        yield break;
    }
}
