using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using TMPro;
using Rewired;

public enum GameMode
{
    Assignment,
    Gameplay,
    FreeSelect,
    UI
}

public class CSPlayerInput : PlayerInput
{
    private CharacterSelectManager characterSelectManager;

    private string playerMenuMode = "";
    float padHMoveLeft = 0.0f;
    float padHMoveRight = 0.0f;
    float padVMoveUp = 0.0f;
    float padVMoveDown = 0.0f;
    int padHTimeLeft = 0;
    int padHTimeRight = 0;
    int padVTimeUp = 0;
    int padVTimeDown = 0;

    public bool loadingBeforeInputDone = false;
    public bool holdingCursorStar = true;
    public bool characterSelected { get; private set; }
    private bool characterSelectedRandom = false;
    public int selectedGUI;
    public bool onUI = false;

    public GameObject rulesMenuPrefab;
    public GameObject colorMenuPrefab;
    public GameObject hpMenuPrefab;

    public enum CursorFound
    {
        None,
        CursorStar,
        CharacterStar,
        Rules,
        Back,
        MenuUserName,
        MenuColor,
        MenuHP,
        MenuShards,
        MenuConfig,
        GUIPlayer,
        GUICPU,
        GUINone
    }

    public enum CursorFoundFlags
    {
        CursorStar = (1 << 0),
        CharacterStar = (1 << 1),
        Rules = (1 << 2),
        Back = (1 << 3),
        MenuUserName = (1 << 4),
        MenuColor = (1 << 5),
        MenuHP = (1 << 6),
        MenuShards = (1 << 7),
        MenuConfig = (1 << 8),
        GUIPlayer = (1 << 9),
        GUICPU = (1 << 10),
        GUINone = (1 << 11)
    }

    public enum CStarPickFlags
    {
        P1 = (1 << 0),
        P2 = (1 << 1),
        P3 = (1 << 2),
        P4 = (1 << 3)
    }

    public enum GUIMenuPickFlags
    {
        P1 = (1 << 0),
        P2 = (1 << 1),
        P3 = (1 << 2),
        P4 = (1 << 3)
    }

    ActivePlayers activePlayers;
    GameObject playerCursor;
    CSCursor csCursor;
    public CSCursorStar initialCursorStar;
    public CSCursorStar currentCursorStar;
    public CSCursorStar[] csCursorStars;
    CSPlayerInput[] players;
    CSPlayerData[] csPlayerData;
    CSPlayerGUI csPlayerGUI;
    CSPlayerGUI[] csPlayerGUIs;
    CharacterSelectShard[] csShard;
    AnimateCSShard[] animateCSShard;
    SummonCursors summonCursors;
    Dictionary<string, GameObject> menuScreenRoots;
    NewMenuScreenRoot menuScreenRoot;
    GameObject rulesMenuScreenGO;
    NewMenuScreenRoot rulesMenuScreenRoot;
    NewMenuScreenRoot[] selectUserScreenRoot;
    NewMenuScreenRoot[] colorScreenRoot;
    NewMenuScreenRoot[] hpScreenRoot;
    NewMenuScreenRoot[] shardsScreenRoot;
    ColorTableData colorTableData;
    ShardsMenuControl shardsMenuControl;
    ToChosenScene toChosenScene;
    PressStartToJoinPlayerSelector pressStartToJoin;
    float pCursorPosX;
    float pCursorPosY;
    float pCursorPosZ;

    private GameMode currentGameMode;
    static Dictionary<GameMode, string> gameModeToMapCategory = new Dictionary<GameMode, string>()
    {
        { GameMode.Assignment, "Assignment" },
        { GameMode.Gameplay, "Gameplay" },
        { GameMode.FreeSelect, "FreeSelect" },
        { GameMode.UI, "UI" }
    };
    public CursorFound playerCursorFound = CursorFound.None;
    public CursorFoundFlags playerCursorFoundFlags = 0;
    public CStarPickFlags cStarPickFlags = 0;
    public GUIMenuPickFlags guiMenuPickFlags = 0;

    //private const int kUpdatesPerSecond = 60;
    //private const float kUpdateInterval = 1.0f / kUpdatesPerSecond; // how many seconds pass before an update should happen
    //private float _accumulation = 0.0f; // stores time elapsed

    // Use this for initialization

    private void Awake()
    {
        characterSelectManager = CharacterSelectManager.characterSelectManager;
        GameObject csGUIs = characterSelectManager.csPlayerGUI;
        csPlayerGUIs = csGUIs.GetComponentsInChildren<CSPlayerGUI>();
        csPlayerGUI = csGUIs.transform.GetChild(playerID).GetComponent<CSPlayerGUI>();
        selectedGUI = playerID;
        GameObject aPlay = characterSelectManager.activePlayers;
        activePlayers = aPlay.GetComponent<ActivePlayers>();
        GameObject csControl = characterSelectManager.csControl;
        summonCursors = csControl.GetComponent<SummonCursors>();
        shardsMenuControl = csControl.GetComponent<ShardsMenuControl>();
        toChosenScene = csControl.GetComponent<ToChosenScene>();

        pressStartToJoin = aPlay.GetComponent<PressStartToJoinPlayerSelector>();
        r_player = ReInput.players.GetPlayer(playerID);
        s_player = ReInput.players.GetPlayer(4);
        setPlayerMenuMode("Assignment");

        ReInput.ControllerConnectedEvent += OnControllerConnected;
        ReInput.ControllerPreDisconnectEvent += OnControllerPreDisconnected;
        ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;
        //r_player.controllers.ControllerAddedEvent += OnControllerAdded;

        foreach (Controller cm in r_player.controllers.Controllers)
        {
            if (cm.type == ControllerType.Keyboard)
            {
                k_player_controller = cm;
            }
        }
        foreach (Joystick j in ReInput.controllers.Joysticks)
        {
            if (ReInput.controllers.IsJoystickAssigned(j)) continue;

            AssignJoystickToNextOpenPlayer(j);
        }
        ChangeGameMode(GameMode.Assignment);

        players = new CSPlayerInput[4];
        csPlayerData = new CSPlayerData[4];
        for (int i = 0; i < characterSelectManager.players.Length; i++)
        {
            players[i] = characterSelectManager.players[i].GetComponent<CSPlayerInput>();
            csPlayerData[i] = characterSelectManager.players[i].GetComponent<CSPlayerData>();
        }
        playerCursor = characterSelectManager.csCursors[playerID];
        csCursor = playerCursor.GetComponent<CSCursor>();
        csCursorStars = new CSCursorStar[4];
        for (int i = 0; i < csCursorStars.Length; i++)
        {
            csCursorStars[i] = characterSelectManager.csCursorStars[i].GetComponent<CSCursorStar>();
        }
        csPlayerGUIs = csGUIs.GetComponentsInChildren<CSPlayerGUI>();
        GameObject csSh = characterSelectManager.csShards;
        csShard = new CharacterSelectShard[4];
        animateCSShard = new AnimateCSShard[4];
        for (int i = 0; i < csShard.Length; i++)
        {
            csShard[i] = csSh.transform.GetChild(i).GetComponent<CharacterSelectShard>();
            animateCSShard[i] = csSh.transform.GetChild(i).GetComponent<AnimateCSShard>();
        }
        GameObject evSystem = characterSelectManager.eventSystem;
        systemInputData = evSystem.GetComponent<SystemInputData>();
        systemInputReader = evSystem.GetComponent<SystemInputReader>();
        inputReader = evSystem.GetComponent<InputReader>();
        sfxPlayer = characterSelectManager.sfxPlayer.GetComponent<SFXPlayer>();


        menuScreenRoots = new Dictionary<string, GameObject>();
        //menuScreenRoots.Add("RulesMenuScreen", characterSelectManager.rulesMenuScreen);
        rulesMenuScreenGO = characterSelectManager.rulesMenuScreen;
        rulesMenuScreenRoot = rulesMenuScreenGO.GetComponent<NewMenuScreenRoot>();
        rulesMenuScreenGO.transform.position = new Vector3(0, rulesMenuScreenGO.transform.position.y, rulesMenuScreenGO.transform.position.z);
        SpriteRenderer tempCSP2 = rulesMenuScreenGO.GetComponent<SpriteRenderer>();
        tempCSP2.color = new Color(tempCSP2.color.r, tempCSP2.color.g, tempCSP2.color.b, 0);
        SpriteRenderer[] tempCSP3 = rulesMenuScreenGO.transform.GetComponentsInChildren<SpriteRenderer>();
        for (int child = 0; child < tempCSP3.Length; child++)
        {
            tempCSP3[child].color = new Color(tempCSP3[child].color.r, tempCSP3[child].color.g, tempCSP3[child].color.b, 0);
        }
        TextMeshProUGUI[] tempCSP4 = rulesMenuScreenGO.transform.GetComponentsInChildren<TextMeshProUGUI>();
        for (int child = 0; child < tempCSP4.Length; child++)
        {
            tempCSP4[child].color = new Color(tempCSP4[child].color.r, tempCSP4[child].color.g, tempCSP4[child].color.b, 0);
        }
        selectUserScreenRoot = new NewMenuScreenRoot[4];
        colorScreenRoot = new NewMenuScreenRoot[4];
        hpScreenRoot = new NewMenuScreenRoot[4];
        shardsScreenRoot = new NewMenuScreenRoot[4];
        for (int i = 0; i < players.Length; i++)
        {
            menuScreenRoots.Add("SelectUserMenuScreen" + i.ToString(), characterSelectManager.selectUserMenuScreens.transform.GetChild(i).gameObject);
            selectUserScreenRoot[i] = menuScreenRoots["SelectUserMenuScreen" + i.ToString()].GetComponent<NewMenuScreenRoot>();
            menuScreenRoots.Add("ColorMenuScreen" + i.ToString(), characterSelectManager.colorMenuScreens.transform.GetChild(i).gameObject);
            colorScreenRoot[i] = menuScreenRoots["ColorMenuScreen" + i.ToString()].GetComponent<NewMenuScreenRoot>();
            menuScreenRoots.Add("HPMenuScreen" + i.ToString(), characterSelectManager.hpMenuScreens.transform.GetChild(i).gameObject);
            hpScreenRoot[i] = menuScreenRoots["HPMenuScreen" + i.ToString()].GetComponent<NewMenuScreenRoot>();
            menuScreenRoots.Add("ShardsMenuScreen" + i.ToString(), characterSelectManager.shardsMenuScreens.transform.GetChild(i).gameObject);
            shardsScreenRoot[i] = menuScreenRoots["ShardsMenuScreen" + i.ToString()].GetComponent<NewMenuScreenRoot>();
        }
        colorTableData = colorScreenRoot[playerID].GetComponentInChildren<ColorTableData>();
    }

    void Start()
    {
        characterSelected = false;
        playerCode = gameObject.name.Substring(gameObject.name.Length - 1, 1);
        playerInputMode = "Assignment";
    }

    public void ChangeGameMode(GameMode mode)
    {
        currentGameMode = mode; // store the new game mode
        if (inputReader != null)
        {
            inputReader.releaseAllHolds(playerID);
        }
        SetControllerMapsForCurrentGameMode(); // enable the correct Controller Maps for the game mode
    }

    /*void OnControllerAdded(ControllerAssignmentChangedEventArgs args)
    {
        SetControllerMapsForCurrentGameMode();
    }*/

    void SetControllerMapsForCurrentGameMode()
    {
        // Disable all controller maps first for all controllers of all types
        r_player.controllers.maps.SetAllMapsEnabled(false);

        // Enable maps for the current game mode for all controlllers of all types
        r_player.controllers.maps.SetMapsEnabled(true, gameModeToMapCategory[currentGameMode]);
    }

    void OnControllerConnected(ControllerStatusChangedEventArgs args)
    {
        Debug.Log("A controller was connected! Name = " + args.name + " Id = " + args.controllerId + " Type = " + args.controllerType);
        if (args.controllerType != ControllerType.Joystick)
        {
            return;
        }
        AssignJoystickToNextOpenPlayer(ReInput.controllers.GetJoystick(args.controllerId));
    }

    void OnControllerPreDisconnected(ControllerStatusChangedEventArgs args)
    {
        Debug.Log("A controller is about to disconnect! Name = " + args.name + " Id = " + args.controllerId + " Type = " + args.controllerType);
        UnassignJoystickFromPlayer(args.controllerId);
    }

    void OnControllerDisconnected(ControllerStatusChangedEventArgs args)
    {
        //Debug.Log("A controller was disconnected! Name = " + args.name + " Id = " + args.controllerId + " Type = " + args.controllerType);
        //Debug.Log(args.controllerId + " / " + r_player_controller.id);
        //UnassignJoystickFromPlayer(args.controllerId);
    }

    void AssignJoystickToNextOpenPlayer(Joystick j)
    {
        if (r_player.controllers.joystickCount <= 0 && !ReInput.controllers.IsJoystickAssigned(j))
        {
            r_player.controllers.AddController(j, true);
            s_player.controllers.AddController(j, false);
            r_player_controller = j;
            s_player_controller = j;
            pressStartToJoin.NewControllerFound(playerID, r_player_controller, activePlayers.playerOn[playerID]);
            if (loadingBeforeInputDone)
            {
                enablePlayerInput = true;
            }
        } else
        {
            foreach (Controller cm in r_player.controllers.Controllers)
            {
                if (cm.type == ControllerType.Joystick)
                {
                    s_player.controllers.AddController(j, false);
                    r_player_controller = cm;
                    s_player_controller = cm;
                    if (loadingBeforeInputDone)
                    {
                        enablePlayerInput = true;
                    }
                    break;
                }
            }
        }
    }

    void UnassignJoystickFromPlayer(int j)
    {
        if (System.Object.ReferenceEquals(r_player_controller, null))
        {
            return;
        }
        if (r_player_controller.id == j)
        {
            if (inputReader != null)
            {
                inputReader.releaseAllHolds(playerID);
            }
            r_player.controllers.RemoveController(r_player_controller);
            s_player.controllers.RemoveController(s_player_controller);
            if (k_player_controller == null)
            {
                activePlayers.playerOn[playerID] = false;
            }
        }
        r_player_controller = null;
        s_player_controller = null;
    }

    private void OnDestroy()
    {
        ReInput.ControllerConnectedEvent -= OnControllerConnected;
        ReInput.ControllerPreDisconnectEvent -= OnControllerPreDisconnected;
        ReInput.ControllerDisconnectedEvent -= OnControllerDisconnected;
        //r_player.controllers.ControllerAddedEvent -= OnControllerAdded;
    }

    // Update is called once per frame
    void Update()
    {
        searchForKeyInputs();

        if (playerCursorFoundFlags != 0)
        {
            for (int i = 0; i < System.Enum.GetNames(typeof(CursorFoundFlags)).Length; i++)
            {
                if (playerCursorFoundFlags.HasFlag((CursorFoundFlags)(1 << i)))
                {
                    playerCursorFound = (CursorFound)(i + 1);
                    break;
                }
            }
        }
        

        if (!onUI)
        {
            if (playerInputMode == "Assignment" && loadingBeforeInputDone)
            {
                if (r_player_controller != null)
                {
                    if (r_player.controllers.maps.GetMap(r_player_controller, "FreeSelect", "Default").enabled && !csPlayerGUIs[playerID].inUse && csPlayerGUIs[playerID].bufferGUI <= 0)
                    {
                        activePlayers.playerOn[playerID] = true;
                        playerInputMode = "CharacterSelect";
                        csPlayerGUI.bufferGUI = 30;
                        ChangeGameMode(GameMode.FreeSelect);
                        //csPlayerGUI.currentGUIMode = CSPlayerGUI.GUIMode.Player;
                        HitboxEventManager.TriggerEvent("UpdateGUISettings", csPlayerGUIs[playerID].guiID, 0, playerID);
                        enablePlayerInput = false;
                        summonCursors.StartCoroutine("beginASummon", playerID);
                        if (!initialCursorStar.gameObject.activeSelf)
                        {
                            StartCoroutine(summonCursors.beginSummonCStar(playerID, activePlayers.playerCursorPos[playerID + 1].posX - 40f, activePlayers.playerCursorPos[playerID + 1].posY + 40f, activePlayers.playerStarPos[playerID + 1].posZ));
                            summonCursors.StartCoroutine("beginSummonAPlayerGUI", playerID);
                        } else
                        {
                            initialCursorStar.ChangeCursorStarMode(0);
                            if (initialCursorStar.isHeld)
                            {
                                players[initialCursorStar.followID].csCursor.heldCursorStar = null;
                                players[initialCursorStar.followID].holdingCursorStar = false;
                                initialCursorStar.followID = playerID;
                                csCursor.heldCursorStar = initialCursorStar;
                                currentCursorStar = initialCursorStar;
                                holdingCursorStar = true;
                            }
                        }
                        return;
                    }
                }
                else
                {
                    if (k_player_controller != null)
                    {
                        if (playerID == 0)
                        {
                            if (r_player.controllers.maps.GetMap(k_player_controller, "FreeSelect", "Player1").enabled && !csPlayerGUIs[playerID].inUse && csPlayerGUIs[playerID].bufferGUI <= 0)
                            {
                                activePlayers.playerOn[playerID] = true;
                                playerInputMode = "CharacterSelect";
                                csPlayerGUI.bufferGUI = 30;
                                ChangeGameMode(GameMode.FreeSelect);
                                //csPlayerGUI.currentGUIMode = CSPlayerGUI.GUIMode.Player;
                                HitboxEventManager.TriggerEvent("UpdateGUISettings", csPlayerGUIs[playerID].guiID, 0, playerID);
                                enablePlayerInput = false;
                                summonCursors.StartCoroutine("beginASummon", playerID);
                                if (!initialCursorStar.gameObject.activeSelf)
                                {
                                    StartCoroutine(summonCursors.beginSummonCStar(playerID, activePlayers.playerCursorPos[playerID + 1].posX - 40f, activePlayers.playerCursorPos[playerID + 1].posY + 40f, activePlayers.playerStarPos[playerID + 1].posZ));
                                    summonCursors.StartCoroutine("beginSummonAPlayerGUI", playerID);
                                } else
                                {
                                    initialCursorStar.ChangeCursorStarMode(0);
                                    if (initialCursorStar.isHeld)
                                    {
                                        players[initialCursorStar.followID].csCursor.heldCursorStar = null;
                                        players[initialCursorStar.followID].holdingCursorStar = false;
                                        initialCursorStar.followID = playerID;
                                        csCursor.heldCursorStar = initialCursorStar;
                                        currentCursorStar = initialCursorStar;
                                        holdingCursorStar = true;
                                    }
                                }
                                return;
                            }
                        }
                        else if (playerID == 1)
                        {
                            if (r_player.controllers.maps.GetMap(k_player_controller, "FreeSelect", "Player2").enabled && !csPlayerGUIs[playerID].inUse && csPlayerGUIs[playerID].bufferGUI <= 0)
                            {
                                activePlayers.playerOn[playerID] = true;
                                playerInputMode = "CharacterSelect";
                                csPlayerGUI.bufferGUI = 30;
                                ChangeGameMode(GameMode.FreeSelect);
                                //csPlayerGUI.currentGUIMode = CSPlayerGUI.GUIMode.Player;
                                HitboxEventManager.TriggerEvent("UpdateGUISettings", csPlayerGUIs[playerID].guiID, 0, playerID);
                                enablePlayerInput = false;
                                summonCursors.StartCoroutine("beginASummon", playerID);
                                if (!initialCursorStar.gameObject.activeSelf)
                                {
                                    StartCoroutine(summonCursors.beginSummonCStar(playerID, activePlayers.playerCursorPos[playerID + 1].posX - 40f, activePlayers.playerCursorPos[playerID + 1].posY + 40f, activePlayers.playerStarPos[playerID + 1].posZ));
                                    summonCursors.StartCoroutine("beginSummonAPlayerGUI", playerID);
                                } else
                                {
                                    initialCursorStar.ChangeCursorStarMode(0);
                                    if (initialCursorStar.isHeld)
                                    {
                                        players[initialCursorStar.followID].csCursor.heldCursorStar = null;
                                        players[initialCursorStar.followID].holdingCursorStar = false;
                                        initialCursorStar.followID = playerID;
                                        csCursor.heldCursorStar = initialCursorStar;
                                        currentCursorStar = initialCursorStar;
                                        holdingCursorStar = true;
                                    }
                                }
                                return;
                            }
                        }
                    }
                }
            }
        }

        if (enablePlayerInput && playerHierarchy >= activePlayers.playerHierarchyCheck && loadingBeforeInputDone)
        {
            if (!onUI)
            {
                if (playerInputMode == "CharacterSelect")
                {
                    selectedGUI = playerID;
                    if (!activePlayers.playerOn[playerID])
                    {
                        playerInputMode = "Assignment";
                        ChangeGameMode(GameMode.Assignment);
                        //csPlayerGUI.currentGUIMode = CSPlayerGUI.GUIMode.None;
                        HitboxEventManager.TriggerEvent("UpdateGUISettings", csPlayerGUIs[playerID].guiID, 2, playerID);
                        enablePlayerInput = false;
                        removeOpenMenus();
                        csPlayerData[playerID].initializePlayerData();
                        shardsMenuControl.DeclareShardsLeft();
                        summonCursors.StartCoroutine("beginADestroy", playerID);
                    }
                    else
                    {
                        if (inputReader.useNewInput("LightOffLong", playerID))
                        {
                            SpriteRenderer tempPC = playerCursor.GetComponent<SpriteRenderer>();
                            tempPC.color = new Color(0.40f, 0.40f, 0.80f, 1.0f);
                        }
                        if (inputReader.useNewInput("LightOff", playerID))
                        {
                            SpriteRenderer tempPC = playerCursor.GetComponent<SpriteRenderer>();
                            tempPC.color = new Color(0.85f, 0.45f, 0.45f, 1.0f);
                        }
                        if (inputReader.useNewInput("LightOn", playerID))
                        {
                            SpriteRenderer tempPC = playerCursor.GetComponent<SpriteRenderer>();
                            tempPC.color = new Color(0.85f, 0.85f, 0.85f, 1.0f);
                        }
                        if (inputReader.useNewInput("Piledriver", playerID))
                        {
                            sfxPlayer.PlaySound("EnterMain");
                        }
                        if (inputReader.useNewInput("Hidden2", playerID))
                        {
                            sfxPlayer.PlaySound("Cancel");
                        }
                        if (inputReader.useNewInput("Hidden", playerID))
                        {
                            sfxPlayer.PlaySound("EnterMain");
                        }
                        if (inputReader.useNewInput("Unknown", playerID))
                        {
                            sfxPlayer.PlaySound("Twinkle");
                        }
                        if (inputReader.useNewInput("Confirm", playerID))
                        {
                            if (playerCursorFound == CursorFound.CursorStar)
                            {

                                if (cStarPickFlags.HasFlag((CStarPickFlags)(1 << playerID)) && !csCursorStars[playerID].noSelect)
                                {
                                    currentCursorStar = csCursorStars[playerID];
                                } else
                                {
                                    for (int i = 0; i < System.Enum.GetNames(typeof(CStarPickFlags)).Length; i++)
                                    {
                                        if (cStarPickFlags.HasFlag((CStarPickFlags)(1 << i)) && !csCursorStars[i].noSelect)
                                        {
                                            currentCursorStar = csCursorStars[i];
                                            break;
                                        }
                                    }
                                }
                                if (!currentCursorStar.noSelect)
                                {
                                    csCursor.heldCursorStar = currentCursorStar;
                                    currentCursorStar.followID = csCursor.cursorId;
                                    currentCursorStar.isHolding(true);
                                    currentCursorStar.iconFollowRate = 0.8f;
                                    removeOpenMenus();
                                    csPlayerData[currentCursorStar.cursorId].characterColorCode = 0;
                                    csShard[currentCursorStar.cursorId].selectedColor = 0;
                                    csPlayerData[currentCursorStar.cursorId].deselectStar();
                                    players[currentCursorStar.cursorId].csPlayerGUI.refreshGUIOnce = true;
                                    players[currentCursorStar.cursorId].characterSelected = false;
                                    players[currentCursorStar.cursorId].characterSelectedRandom = false;
                                    holdingCursorStar = true;
                                }
                            } else
                            {
                                if (playerCursorFound == CursorFound.CharacterStar || (holdingCursorStar && !csCursor.heldCursorStar.noCStarBelow))
                                {
                                    currentCursorStar.isHolding(true);
                                    sfxPlayer.PlaySound("Confirm");
                                    csPlayerGUIs[currentCursorStar.cursorId].StartCoroutine("nameBurst");
                                    animateCSShard[currentCursorStar.cursorId].changeState(9);
                                    if (csCursor.currentHighlightedStar == 0)
                                    {
                                        players[currentCursorStar.cursorId].characterSelectedRandom = true;
                                    }
                                    csPlayerData[currentCursorStar.cursorId].selectStar();
                                    players[currentCursorStar.cursorId].colorTableData.updateTable();
                                    players[currentCursorStar.cursorId].characterSelected = true;
                                    holdingCursorStar = false;
                                    currentCursorStar.followID = currentCursorStar.cursorId;
                                    currentCursorStar = initialCursorStar;
                                    playerCursorFound = CursorFound.None;
                                    playerCursorFoundFlags = 0;
                                }
                            }
                            if (playerCursorFound == CursorFound.Rules)
                            {
                                sfxPlayer.PlaySound("Confirm");
                                playerInputMode = "MenuSelect";
                                enablePlayerInput = false;
                                playerHierarchy = 2;
                                enableOtherPlayers(false);
                                ChangeGameMode(GameMode.UI);
                                for (int p = 0; p < players.Length; p++)
                                {
                                    players[p].onUI = true;
                                }
                                //menuScreenRoot = rulesMenuScreenRoot;
                                rulesMenuScreenRoot.assignPlayer(playerID);
                                rulesMenuScreenGO.SetActive(true);
                                playerMenuMode = "Rules";
                            }
                            if (playerCursorFound == CursorFound.Back)
                            {
                                sfxPlayer.PlaySound("Cancel");
                                toChosenScene.selectScene("MainMenuScene", -1000);
                                toChosenScene.enableFadeScene = true;
                                playerHierarchy = 5;
                                enablePlayerInput = false;
                            }
                            if (playerCursorFound == CursorFound.MenuUserName)
                            {
                                for (int i = 0; i < System.Enum.GetNames(typeof(GUIMenuPickFlags)).Length; i++)
                                {
                                    if (guiMenuPickFlags.HasFlag((GUIMenuPickFlags)(1 << i)) && !csPlayerGUIs[i].inUse && csPlayerGUIs[i].bufferGUI <= 0)
                                    {
                                        sfxPlayer.PlaySound("Confirm");
                                        playerInputMode = "MenuSelect";
                                        enablePlayerInput = false;
                                        csPlayerGUIs[i].bufferGUI = 30;
                                        csPlayerGUIs[i].inUse = true;
                                        menuScreenRoot = selectUserScreenRoot[i];
                                        selectedGUI = i;
                                        menuScreenRoots["SelectUserMenuScreen" + i.ToString()].SetActive(true);
                                        playerMenuMode = "SelectUser";
                                        break;
                                    }
                                }
                            }
                            if (playerCursorFound == CursorFound.MenuColor)
                            {
                                for (int i = 0; i < System.Enum.GetNames(typeof(GUIMenuPickFlags)).Length; i++)
                                {
                                    if (guiMenuPickFlags.HasFlag((GUIMenuPickFlags)(1 << i)) && !csPlayerGUIs[i].inUse && csPlayerGUIs[i].bufferGUI <= 0 && players[i].characterSelected && !players[i].characterSelectedRandom)
                                    {
                                        sfxPlayer.PlaySound("Confirm");
                                        playerInputMode = "MenuSelect";
                                        enablePlayerInput = false;
                                        csPlayerGUIs[i].bufferGUI = 30;
                                        csPlayerGUIs[i].inUse = true;
                                        menuScreenRoot = colorScreenRoot[i];
                                        selectedGUI = i;
                                        menuScreenRoot.assignCurrentPlayer(this);
                                        menuScreenRoots["ColorMenuScreen" + i.ToString()].SetActive(true);
                                        playerMenuMode = "Color";
                                        break;
                                    }
                                }
                            }
                            if (playerCursorFound == CursorFound.MenuHP)
                            {
                                for (int i = 0; i < System.Enum.GetNames(typeof(GUIMenuPickFlags)).Length; i++)
                                {
                                    if (guiMenuPickFlags.HasFlag((GUIMenuPickFlags)(1 << i)) && !csPlayerGUIs[i].inUse && csPlayerGUIs[i].bufferGUI <= 0)
                                    {
                                        sfxPlayer.PlaySound("Confirm");
                                        playerInputMode = "MenuSelect";
                                        enablePlayerInput = false;
                                        csPlayerGUIs[i].bufferGUI = 30;
                                        csPlayerGUIs[i].inUse = true;
                                        menuScreenRoot = hpScreenRoot[i];
                                        selectedGUI = i;
                                        menuScreenRoot.assignCurrentPlayer(this);
                                        menuScreenRoots["HPMenuScreen" + i.ToString()].SetActive(true);
                                        playerMenuMode = "HP";
                                        break;
                                    }
                                }
                            }
                            if (playerCursorFound == CursorFound.MenuShards)
                            {
                                for (int i = 0; i < System.Enum.GetNames(typeof(GUIMenuPickFlags)).Length; i++)
                                {
                                    if (guiMenuPickFlags.HasFlag((GUIMenuPickFlags)(1 << i)) && !csPlayerGUIs[i].inUse && csPlayerGUIs[i].bufferGUI <= 0)
                                    {
                                        sfxPlayer.PlaySound("Confirm");
                                        playerInputMode = "MenuSelect";
                                        enablePlayerInput = false;
                                        csPlayerGUIs[i].bufferGUI = 30;
                                        csPlayerGUIs[i].inUse = true;
                                        menuScreenRoot = shardsScreenRoot[i];
                                        selectedGUI = i;
                                        menuScreenRoot.assignCurrentPlayer(this);
                                        menuScreenRoots["ShardsMenuScreen" + i.ToString()].SetActive(true);
                                        playerMenuMode = "Shards";
                                        break;
                                    }
                                }
                            }
                            if (playerCursorFound == CursorFound.GUIPlayer)
                            {
                                for (int i = 0; i < csPlayerGUIs.Length; i++)
                                {
                                    //CSGUIMode csMode = csPlayerGUI.gameObject.transform.Find("GUIModeIcons").gameObject.transform.Find("GUIModeIcon0").GetComponent<CSGUIMode>();
                                    if (csPlayerGUIs[i].modeHighlighted[0].mhFlags.HasFlag((CSPlayerGUI.MhFlags)(1 << playerID)) && csPlayerGUIs[i].enableModes && !csPlayerGUIs[i].inUse && csPlayerGUIs[i].bufferGUI <= 0)
                                    {
                                        for (int j = 0; j < players.Length; j++)
                                        {
                                            if (i == players[j].playerID)
                                            {
                                                if (players[j].currentGameMode == GameMode.Assignment)
                                                {
                                                    if (players[j].initialCursorStar.gameObject.activeSelf)
                                                    {
                                                        players[j].initialCursorStar.ChangeCursorStarMode(0);
                                                        if (players[j].initialCursorStar.isHeld)
                                                        {
                                                            players[players[j].initialCursorStar.followID].csCursor.heldCursorStar = null;
                                                            players[players[j].initialCursorStar.followID].holdingCursorStar = false;
                                                            players[j].initialCursorStar.followID = j;
                                                            players[j].csCursor.heldCursorStar = initialCursorStar;
                                                            players[j].currentCursorStar = initialCursorStar;
                                                            players[j].holdingCursorStar = true;
                                                        }
                                                    }
                                                    pressStartToJoin.forceActivePlayer(i);
                                                    //csPlayerGUIs[i].currentGUIMode = CSPlayerGUI.GUIMode.Player;
                                                    HitboxEventManager.TriggerEvent("UpdateGUISettings", csPlayerGUIs[i].guiID, 0, playerID);
                                                } else
                                                {
                                                    csPlayerGUIs[i].bufferGUI = 5;
                                                    players[j].initialCursorStar.ChangeCursorStarMode(0);
                                                    if (players[j].initialCursorStar.isHeld && (players[j].initialCursorStar.followID != players[j].playerID))
                                                    {
                                                        players[players[j].initialCursorStar.followID].csCursor.heldCursorStar = null;
                                                        players[players[j].initialCursorStar.followID].holdingCursorStar = false;
                                                        players[j].initialCursorStar.followID = j;
                                                        players[j].csCursor.heldCursorStar = initialCursorStar;
                                                        players[j].currentCursorStar = initialCursorStar;
                                                        players[j].holdingCursorStar = true;
                                                    }
                                                    //csPlayerGUIs[i].currentGUIMode = CSPlayerGUI.GUIMode.Player;
                                                    HitboxEventManager.TriggerEvent("UpdateGUISettings", csPlayerGUIs[i].guiID, 0, playerID);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (playerCursorFound == CursorFound.GUICPU)
                            {
                                for (int i = 0; i < csPlayerGUIs.Length; i++)
                                {
                                    //CSGUIMode csMode = csPlayerGUI.gameObject.transform.Find("GUIModeIcons").gameObject.transform.Find("GUIModeIcon0").GetComponent<CSGUIMode>();
                                    if (csPlayerGUIs[i].modeHighlighted[1].mhFlags.HasFlag((CSPlayerGUI.MhFlags)(1 << playerID)) && csPlayerGUIs[i].enableModes && !csPlayerGUIs[i].inUse && csPlayerGUIs[i].bufferGUI <= 0)
                                    {
                                        if (i == playerID)
                                        {
                                            csPlayerGUIs[i].bufferGUI = 5;
                                            initialCursorStar.ChangeCursorStarMode(1);
                                            //csPlayerGUIs[i].currentGUIMode = CSPlayerGUI.GUIMode.CPU;
                                            HitboxEventManager.TriggerEvent("UpdateGUISettings", csPlayerGUIs[i].guiID, 1, playerID);
                                        } else
                                        {
                                            if (!activePlayers.playerOn[i])
                                            {
                                                if (!players[i].initialCursorStar.gameObject.activeSelf)
                                                {
                                                    csPlayerGUIs[i].bufferGUI = 30;
                                                    StartCoroutine(summonCursors.beginSummonCStar(i, activePlayers.playerStarPos[i + 1].posX, activePlayers.playerStarPos[i + 1].posY, activePlayers.playerStarPos[i + 1].posZ));
                                                    summonCursors.StartCoroutine("beginSummonAPlayerGUI", i);
                                                }
                                                HitboxEventManager.TriggerEvent("UpdateGUISettings", csPlayerGUIs[i].guiID, 1, playerID);
                                            } else
                                            {
                                                csPlayerGUIs[i].bufferGUI = 5;
                                                players[i].initialCursorStar.ChangeCursorStarMode(1);
                                                HitboxEventManager.TriggerEvent("UpdateGUISettings", csPlayerGUIs[i].guiID, 1, playerID);
                                            }
                                        }
                                    }
                                }
                            }
                            if (playerCursorFound == CursorFound.GUINone)
                            {
                                for (int i = 0; i < csPlayerGUIs.Length; i++)
                                {
                                    if (csPlayerGUIs[i].modeHighlighted[2].mhFlags.HasFlag((CSPlayerGUI.MhFlags)(1 << playerID)) && csPlayerGUIs[i].enableModes && csPlayerGUIs[i].bufferGUI <= 0)
                                    {
                                        csPlayerGUIs[i].bufferGUI = 30;
                                        activePlayers.playerOn[i] = false;
                                        if (players[i].initialCursorStar.isHeld && (players[i].initialCursorStar.followID != players[i].playerID))
                                        {
                                            players[players[i].initialCursorStar.followID].csCursor.heldCursorStar = null;
                                            players[players[i].initialCursorStar.followID].holdingCursorStar = false;
                                            players[i].initialCursorStar.followID = i;
                                            players[i].csCursor.heldCursorStar = players[i].initialCursorStar;
                                            players[i].currentCursorStar = players[i].initialCursorStar;
                                        }
                                        if (players[i].holdingCursorStar && players[i].currentCursorStar != players[i].initialCursorStar)
                                        {
                                            players[i].currentCursorStar.followID = players[i].currentCursorStar.cursorId;
                                            players[i].currentCursorStar.UpdatePosToCPU();
                                            players[i].currentCursorStar = players[i].initialCursorStar;
                                        }
                                        HitboxEventManager.TriggerEvent("UpdateGUISettings", csPlayerGUIs[i].guiID, 2, playerID);
                                    }
                                }
                            }
                        }
                        if (inputReader.useNewInput("Cancel", playerID))
                        {
                            if (!holdingCursorStar)
                            {
                                currentCursorStar = initialCursorStar;
                                if (!currentCursorStar.noSelect && (csPlayerGUI.currentGUIMode == CSPlayerGUI.GUIMode.Player || (csPlayerGUI.currentGUIMode == CSPlayerGUI.GUIMode.CPU && !initialCursorStar.isHeld)))
                                {
                                    currentCursorStar = initialCursorStar;
                                    csCursor.heldCursorStar = currentCursorStar;
                                    currentCursorStar.isHolding(true);
                                    currentCursorStar.iconFollowRate = 0.8f;
                                    removeOpenMenus();
                                    csPlayerData[playerID].characterColorCode = 0;
                                    csShard[playerID].selectedColor = 0;
                                    csPlayerData[playerID].deselectStar();
                                    csPlayerGUI.refreshGUIOnce = true;
                                    characterSelected = false;
                                    characterSelectedRandom = false;
                                    holdingCursorStar = true;
                                }
                            }
                        }
                    }
                }
                else if (playerInputMode == "MenuSelect" && !haltPlayerInput)
                {
                    if (!activePlayers.playerOn[playerID])
                    {
                        //csPlayerGUI.currentGUIMode = CSPlayerGUI.GUIMode.None;
                        HitboxEventManager.TriggerEvent("UpdateGUISettings", csPlayerGUIs[playerID].guiID, 2, playerID);
                        enablePlayerInput = false;
                        csPlayerData[playerID].initializePlayerData();
                        shardsMenuControl.DeclareShardsLeft();
                        removeOpenMenus();
                        playerInputMode = "Assignment";
                        ChangeGameMode(GameMode.Assignment);
                        summonCursors.StartCoroutine("beginADestroy", playerID);
                        summonCursors.StartCoroutine("beginDestroyCStar", playerID);
                        summonCursors.StartCoroutine("beginDestroyAPlayerGUI", playerID);
                    }
                    else
                    {
                        if (inputReader.useNewInput("Confirm", playerID))
                        {
                            menuScreenRoot.selectOption();
                        }
                        if (inputReader.useNewInput("Cancel", playerID))
                        {
                            menuScreenRoot.closeMenu();
                        }
                    }
                }

                if (playerInputMode == "CharacterSelect")
                {
                    pCursorPosX = playerCursor.transform.position.x;
                    pCursorPosY = playerCursor.transform.position.y;
                    pCursorPosZ = playerCursor.transform.position.z;
                    padHorizontal = r_player.GetAxisRaw("Move Horizontal");
                    padVertical = r_player.GetAxisRaw("Move Vertical");
                    //padKeyHorizontal = Input.GetAxisRaw("KeyHorizontal_" + playerCode);
                    //padKeyVertical = Input.GetAxisRaw("KeyVertical_" + playerCode);
                    if (inputReader.useNewInput("MoveRight", playerID) || inputReader.useNewInput("MoveUp_Right", playerID) || inputReader.useNewInput("MoveDown_Right", playerID))
                    {
                        padHTimeRight = ((padHTimeRight < 15) ? (padHTimeRight + 1) : padHTimeRight);
                        padHMoveRight = (padHTimeRight * Mathf.Ceil((inputReader.axisKeyIsOn(playerCode)) ? padKeyHorizontal : padHorizontal));
                        float newPosX = pCursorPosX + padHMoveRight;
                        if (csPlayerGUIs[currentCursorStar.cursorId].currentGUIMode == CSPlayerGUI.GUIMode.CPU && holdingCursorStar && csCursor.heldCursorStar.cursorId != playerID)
                        {
                            if (newPosX > 790)
                            {
                                newPosX = 790;
                            }
                            if (newPosX < -710)
                            {
                                newPosX = -710;
                            }
                        } else
                        {
                            if (newPosX > 980)
                            {
                                newPosX = 980;
                            }
                            if (newPosX < -920)
                            {
                                newPosX = -920;
                            }
                        }
                        pCursorPosX = newPosX;
                        playerCursor.transform.position = new Vector3(Mathf.Round(pCursorPosX), Mathf.Round(pCursorPosY), pCursorPosZ);
                        padHMoveLeft = 0.0f;
                        padHTimeLeft = 0;
                    }
                    else if (inputReader.useNewInput("MoveLeft", playerID) || inputReader.useNewInput("MoveUp_Left", playerID) || inputReader.useNewInput("MoveDown_Left", playerID))
                    {
                        padHTimeLeft = ((padHTimeLeft < 15) ? (padHTimeLeft + 1) : padHTimeLeft);
                        padHMoveLeft = (padHTimeLeft * Mathf.Ceil((inputReader.axisKeyIsOn(playerCode)) ? padKeyHorizontal : padHorizontal));
                        float newPosX = pCursorPosX + padHMoveLeft;
                        if (csPlayerGUIs[currentCursorStar.cursorId].currentGUIMode == CSPlayerGUI.GUIMode.CPU && holdingCursorStar && csCursor.heldCursorStar.cursorId != playerID)
                        {
                            if (newPosX > 790)
                            {
                                newPosX = 790;
                            }
                            if (newPosX < -710)
                            {
                                newPosX = -710;
                            }
                        }
                        else
                        {
                            if (newPosX > 980)
                            {
                                newPosX = 980;
                            }
                            if (newPosX < -920)
                            {
                                newPosX = -920;
                            }
                        }
                        pCursorPosX = newPosX;
                        playerCursor.transform.position = new Vector3(Mathf.Round(pCursorPosX), Mathf.Round(pCursorPosY), pCursorPosZ);
                        padHMoveRight = 0.0f;
                        padHTimeRight = 0;
                    }
                    else
                    {
                        padHMoveLeft = 0.0f;
                        padHMoveRight = 0.0f;
                        padHTimeLeft = 0;
                        padHTimeRight = 0;
                    }
                    if (inputReader.useNewInput("MoveUp", playerID) || inputReader.useNewInput("MoveUp_Left", playerID) || inputReader.useNewInput("MoveUp_Right", playerID))
                    {
                        padVTimeUp = ((padVTimeUp < 15) ? (padVTimeUp + 1) : padVTimeUp);
                        padVMoveUp = (padVTimeUp * Mathf.Ceil((inputReader.axisKeyIsOn(playerCode)) ? padKeyVertical : padVertical));
                        float newPosY = pCursorPosY + padVMoveUp;
                        if (csPlayerGUIs[currentCursorStar.cursorId].currentGUIMode == CSPlayerGUI.GUIMode.CPU && holdingCursorStar && csCursor.heldCursorStar.cursorId != playerID)
                        {
                            if (newPosY > 340)
                            {
                                newPosY = 340;
                            }
                            if (newPosY < -40)
                            {
                                newPosY = -40;
                            }
                        }
                        else
                        {
                            if (newPosY > 500)
                            {
                                newPosY = 500;
                            }
                            if (newPosY < -550)
                            {
                                newPosY = -550;
                            }
                        }
                        pCursorPosY = newPosY;
                        playerCursor.transform.position = new Vector3(Mathf.Round(pCursorPosX), Mathf.Round(pCursorPosY), pCursorPosZ);
                        padVMoveDown = 0.0f;
                        padVTimeDown = 0;
                    }
                    else if (inputReader.useNewInput("MoveDown", playerID) || inputReader.useNewInput("MoveDown_Left", playerID) || inputReader.useNewInput("MoveDown_Right", playerID))
                    {
                        padVTimeDown = ((padVTimeDown < 15) ? (padVTimeDown + 1) : padVTimeDown);
                        padVMoveDown = (padVTimeDown * Mathf.Ceil((inputReader.axisKeyIsOn(playerCode)) ? padKeyVertical : padVertical));
                        float newPosY = pCursorPosY + padVMoveDown;
                        if (csPlayerGUIs[currentCursorStar.cursorId].currentGUIMode == CSPlayerGUI.GUIMode.CPU && holdingCursorStar && csCursor.heldCursorStar.cursorId != playerID)
                        {
                            if (newPosY > 340)
                            {
                                newPosY = 340;
                            }
                            if (newPosY < -40)
                            {
                                newPosY = -40;
                            }
                        }
                        else
                        {
                            if (newPosY > 500)
                            {
                                newPosY = 500;
                            }
                            if (newPosY < -550)
                            {
                                newPosY = -550;
                            }
                        }
                        pCursorPosY = newPosY;
                        playerCursor.transform.position = new Vector3(Mathf.Round(pCursorPosX), Mathf.Round(pCursorPosY), pCursorPosZ);
                        padVMoveUp = 0.0f;
                        padVTimeUp = 0;
                    }
                    else
                    {
                        padVMoveUp = 0.0f;
                        padVMoveDown = 0.0f;
                        padVTimeUp = 0;
                        padVTimeDown = 0;
                    }
                }
                else if (playerInputMode == "MenuSelect" && !haltPlayerInput)
                {
                    if (currentGameMode == GameMode.UI)
                    {
                        rulesMenuScreenRoot.directionalController();
                    } else
                    {
                        menuScreenRoot.directionalController();
                    }
                }
            } else
            {
                if (currentGameMode == GameMode.UI)
                {
                    if (systemInputReader.useNewInput("Confirm"))
                    {
                        rulesMenuScreenRoot.selectOption();
                    }
                    if (systemInputReader.useNewInput("Cancel"))
                    {
                        rulesMenuScreenRoot.closeMenu();
                    }
                    rulesMenuScreenRoot.directionalController();
                } else
                {
                    if (systemInputReader.useNewInput("Confirm"))
                    {
                        rulesMenuScreenRoot.selectOption();
                    }
                    if (systemInputReader.useNewInput("Cancel"))
                    {
                        rulesMenuScreenRoot.closeMenu();
                    }
                    rulesMenuScreenRoot.directionalController();
                }
                
            }
            
        }
        playerCursorFound = CursorFound.None;
        playerCursorFoundFlags = 0;
        cStarPickFlags = 0;
        guiMenuPickFlags = 0;
        haltPlayerInput = false;
    }

    public override void searchForKeyInputs()
    {
        foreach (KeyValuePair<string, SystemInputData.InputClass> keyName in SystemInputData.systemInputPressList)
        {
            if (SystemInputData.systemInputPressList[keyName.Key]._isHold == 0 && s_player.GetButtonDown(keyName.Key))
            {
                SystemInputData.systemInputPressList[keyName.Key]._used++;
            }
        }
        foreach (KeyValuePair<string, SystemInputData.InputClass> keyName in SystemInputData.systemInputReleaseList)
        {
            if (SystemInputData.systemInputPressList[keyName.Key]._isHold == 0 && s_player.GetButtonUp(keyName.Key))
            {
                SystemInputData.systemInputReleaseList[keyName.Key]._used++;
            }
        }
        padHorizontal = s_player.GetAxisRaw("UI_Horizontal");
        padVertical = s_player.GetAxisRaw("UI_Vertical");
        if (padVertical >= 0.5f)
        {
            SystemInputData.systemInputPressList["UI_Up"]._used++;
        }
        if (padVertical <= -0.5f)
        {
            SystemInputData.systemInputPressList["UI_Down"]._used++;
        }
        if (padHorizontal >= 0.5f)
        {
            SystemInputData.systemInputPressList["UI_Right"]._used++;
        }
        if (padHorizontal <= -0.5f)
        {
            SystemInputData.systemInputPressList["UI_Left"]._used++;
        }
        systemInputData.playerFoundCounter++;
        
        if (r_player.GetButtonDown("Start"))
        {
            inputReader.enterNewPlayerInput("Start", playerID);
        }
        if (r_player.GetButtonUp("Start"))
        {
            inputReader.releasePlayerInput("Start", playerID);
        }
        if (r_player.GetButtonDown("Jump"))
        {
            inputReader.enterNewPlayerInput("D", playerID);
        }
        if (r_player.GetButtonUp("Jump"))
        {
            inputReader.releasePlayerInput("D", playerID);
        }
        if (r_player.GetButtonDown("Special Attack"))
        {
            inputReader.enterNewPlayerInput("C", playerID);
        }
        if (r_player.GetButtonUp("Special Attack"))
        {
            inputReader.releasePlayerInput("C", playerID);
        }
        if (r_player.GetButtonDown("Heavy Attack"))
        {
            inputReader.enterNewPlayerInput("B", playerID);
        }
        if (r_player.GetButtonUp("Heavy Attack"))
        {
            inputReader.releasePlayerInput("B", playerID);
        }
        if (r_player.GetButtonDown("Light Attack"))
        {
            inputReader.enterNewPlayerInput("A", playerID);
        }
        if (r_player.GetButtonUp("Light Attack"))
        {
            inputReader.releasePlayerInput("A", playerID);
        }
        padHorizontal = r_player.GetAxisRaw("Move Horizontal");
        padVertical = r_player.GetAxisRaw("Move Vertical");
        //padKeyHorizontal = r_player.GetAxisRaw("KeyHorizontal_" + playerCode);
        //padKeyVertical = r_player.GetAxisRaw("KeyVertical" + playerCode);

        //Compare to prev axis and stop if lower.
        
        if (padVertical > 0.05f && padHorizontal < 0.05f && padHorizontal > -0.05f && !inputReader.useHold("Up", playerID))
        {
            inputReader.enterNewPlayerInput("Up", playerID);
        }
        if (padVertical < prevPadVertical && inputReader.useHold("Up", playerID))
        {
            inputReader.releasePlayerInput("Up", playerID);
        }
        if (padVertical > 0.05f && padHorizontal < -0.05f && !inputReader.useHold("Up_Left", playerID))
        {
            inputReader.enterNewPlayerInput("Up_Left", playerID);
        }
        if ((padVertical < prevPadVertical || padHorizontal > prevPadHorizontal) && inputReader.useHold("Up_Left", playerID))
        {
            inputReader.releasePlayerInput("Up_Left", playerID);
        }
        if (padVertical > 0.05f && padHorizontal > 0.05f && !inputReader.useHold("Up_Right", playerID))
        {
            inputReader.enterNewPlayerInput("Up_Right", playerID);
        }
        if ((padVertical < prevPadVertical || padHorizontal < prevPadHorizontal) && inputReader.useHold("Up_Right", playerID))
        {
            inputReader.releasePlayerInput("Up_Right", playerID);
        }
        if (padVertical < -0.05f && padHorizontal < 0.05f && padHorizontal > -0.05f && !inputReader.useHold("Down", playerID))
        {
            inputReader.enterNewPlayerInput("Down", playerID);
        }
        if (padVertical > prevPadVertical && inputReader.useHold("Down", playerID))
        {
            inputReader.releasePlayerInput("Down", playerID);
        }
        if (padVertical < -0.05f && padHorizontal < -0.05f && !inputReader.useHold("Down_Left", playerID))
        {
            inputReader.enterNewPlayerInput("Down_Left", playerID);
        }
        if ((padVertical > prevPadVertical || padHorizontal > prevPadHorizontal) && inputReader.useHold("Down_Left", playerID))
        {
            inputReader.releasePlayerInput("Down_Left", playerID);
        }
        if (padVertical < -0.05f && padHorizontal > 0.05f && !inputReader.useHold("Down_Right", playerID))
        {
            inputReader.enterNewPlayerInput("Down_Right", playerID);
        }
        if ((padVertical > prevPadVertical || padHorizontal < prevPadHorizontal) && inputReader.useHold("Down_Right", playerID))
        {
            inputReader.releasePlayerInput("Down_Right", playerID);
        }
        if (padHorizontal < -0.05f && padVertical < 0.05f && padVertical > -0.05f && !inputReader.useHold("Left", playerID))
        {
            inputReader.enterNewPlayerInput("Left", playerID);
        }
        if (padHorizontal > prevPadHorizontal && inputReader.useHold("Left", playerID))
        {
            inputReader.releasePlayerInput("Left", playerID);
        }
        if (padHorizontal > 0.05f && padVertical < 0.05f && padVertical > -0.05f && !inputReader.useHold("Right", playerID))
        {
            inputReader.enterNewPlayerInput("Right", playerID);
        }
        if (padHorizontal < prevPadHorizontal && inputReader.useHold("Right", playerID))
        {
            inputReader.releasePlayerInput("Right", playerID);
        }
        prevPadHorizontal = padHorizontal;
        prevPadVertical = padVertical;
        //prevPadKeyHorizontal = padKeyHorizontal;
        //prevPadKeyVertical = padKeyVertical;

        confirmSystemInputs();
    }

    private void confirmSystemInputs()
    {
        if (systemInputData.playerFoundCounter >= 4)
        {
            int horiz = 0;
            int vert = 0;
            foreach (KeyValuePair<string, SystemInputData.InputClass> keyName in SystemInputData.systemInputPressList)
            {
                if (SystemInputData.systemInputPressList[keyName.Key]._isHold == 2)
                {
                    if (SystemInputData.systemInputPressList[keyName.Key]._used > 0)
                    {
                        horiz += (SystemInputData.systemInputPressList[keyName.Key]._used * SystemInputData.systemInputPressList[keyName.Key]._negative);
                    }
                } else if (SystemInputData.systemInputPressList[keyName.Key]._isHold == 1)
                {
                    if (SystemInputData.systemInputPressList[keyName.Key]._used > 0)
                    {
                        vert += (SystemInputData.systemInputPressList[keyName.Key]._used * SystemInputData.systemInputPressList[keyName.Key]._negative);
                    }
                }
                else
                {
                    if (SystemInputData.systemInputPressList[keyName.Key]._used > 0)
                    {
                        systemInputReader.enterNewPlayerInput(keyName.Key);
                    }
                }
                SystemInputData.systemInputPressList[keyName.Key]._used = 0;
            }
            if (vert > 0 && !systemInputReader.useHold("UI_Up"))
            {
                systemInputReader.enterNewPlayerInput("UI_Up");
            }
            if (vert <= 0 && systemInputReader.useHold("UI_Up"))
            {
                systemInputReader.releasePlayerInput("UI_Up");
            }
            if (vert < 0 && !systemInputReader.useHold("UI_Down"))
            {
                systemInputReader.enterNewPlayerInput("UI_Down");
            }
            if (vert >= 0 && systemInputReader.useHold("UI_Down"))
            {
                systemInputReader.releasePlayerInput("UI_Down");
            }
            if (horiz > 0 && !systemInputReader.useHold("UI_Right"))
            {
                systemInputReader.enterNewPlayerInput("UI_Right");
            }
            if (horiz <= 0 && systemInputReader.useHold("UI_Right"))
            {
                systemInputReader.releasePlayerInput("UI_Right");
            }
            if (horiz < 0 && !systemInputReader.useHold("UI_Left"))
            {
                systemInputReader.enterNewPlayerInput("UI_Left");
            }
            if (horiz >= 0 && systemInputReader.useHold("UI_Left"))
            {
                systemInputReader.releasePlayerInput("UI_Left");
            }
            foreach (KeyValuePair<string, SystemInputData.InputClass> keyName in SystemInputData.systemInputReleaseList)
            {
                if (SystemInputData.systemInputReleaseList[keyName.Key]._isHold == 0 && SystemInputData.systemInputReleaseList[keyName.Key]._used > 0)
                {
                    systemInputReader.releasePlayerInput(keyName.Key);
                }
                SystemInputData.systemInputReleaseList[keyName.Key]._used = 0;
            }
            systemInputData.playerFoundCounter = 0;
        }
    }

    public void setPlayerMenuMode(string mode)
    {
        playerMenuMode = mode;
    }

    public bool getPlayerMenuMode(string mode)
    {
        if (playerMenuMode == mode)
        {
            return true;
        }
        return false;
    }

    public void AutoCharSelection()
    {
        csCursor.currentHighlightedStar = 0;
        characterSelectedRandom = true;
        csPlayerData[playerID].characterCode = 0;
        csPlayerData[playerID].selectStar();
        colorTableData.updateTable();
        characterSelected = true;
        holdingCursorStar = false;
    }

    public void unSelectCharacters()
    {
        currentCursorStar.iconFollowRate = 0.8f;
        csPlayerData[playerID].characterColorCode = 0;
        csShard[playerID].selectedColor = 0;
        csPlayerData[playerID].deselectStar();
        characterSelected = false;
        characterSelectedRandom = false;
    }

    private void removeOpenMenus()
    {
        if (menuScreenRoot != null)
        {
            menuScreenRoot.forceCloseMenu();
        }
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].selectedGUI == playerID && activePlayers.playerOn[i])
            {
                if (csPlayerGUI.inUse && players[i].menuScreenRoot != null)
                {
                    players[i].menuScreenRoot.forceCloseMenu();
                    players[i].enablePlayerInput = true;
                    players[i].setPlayerMenuMode("");
                    players[i].playerInputMode = "CharacterSelect";
                }
            }
        }
        //colorScreenRoot.forceCloseMenu();
        //hpScreenRoot[playerID].forceCloseMenu();
        //shardsScreenRoot.forceCloseMenu();
    }

    public void exitUI()
    {
        ChangeGameMode(GameMode.FreeSelect);
        for (int p = 0; p < players.Length; p++)
        {
            players[p].onUI = false;
        }
    }

    public void enableOtherPlayers(bool state)
    {
        for (int p = 0; p < players.Length; p++)
        {
            try
            {
                if (players[p].playerCode != playerCode)
                {
                    players[p].enablePlayerInput = state;
                }
            }
            catch (System.NullReferenceException)
            {
                Debug.Log("Player code is invalid.");
            }
        }
    }
}