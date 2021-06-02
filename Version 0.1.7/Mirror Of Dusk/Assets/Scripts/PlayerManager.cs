using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Rewired;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class PlayerManager
{
    private class PlayerSlot
    {
        public enum JoinState
        {
            NotJoining,
            JoinPromptDisplayed,
            JoinRequested,
            Joined,
            Leaving,
            LeaveRequested
        }

        public enum ControllerState
        {
            NoController,
            UsingController,
            Disconnected,
            ReconnectPromptDisplayed,
            WaitingForReconnect
        }

        public bool canJoin;
        public bool mustJoin;
        public bool waitForJoinRequest;
        public PlayerManager.PlayerSlot.JoinState joinState;
        public PlayerManager.PlayerSlot.ControllerState controllerState;
        public bool canSwitch;
        public bool requestedSwitch;
        public bool promptBeforeJoin;
        public int controllerId;
        public bool shouldAssignController;
        public bool controllerDisconnectFromPlm;

        public ControllerType lastController = (ControllerType)20;
    }

    public delegate void PlayerChangedDelegate(PlayerId playerId);

    private static PlayerManager.PlayerSlot[] playerSlots = new PlayerManager.PlayerSlot[]
    {
        new PlayerManager.PlayerSlot(),
        new PlayerManager.PlayerSlot(),
        new PlayerManager.PlayerSlot(),
        new PlayerManager.PlayerSlot()
    };

    public static bool Multiplayer;

    private static bool shouldGoToSlotSelect = false;
    private static bool shouldGoToStartScreen = false;
    private static bool pausedDueToPlm = false;
    public static int player1DisconnectedControllerId;
    public static bool allowAutomaticControllerSearch = false;

    private static Dictionary<int, Player> playerInputs;

    private static Dictionary<int, AbstractPlayerController> players;

    private static PlayerId currentId;
    
    private static SignInEventHandler f__m0;
    private static SignOutEventHandler f__m1;
    private static Action<ControllerStatusChangedEventArgs> f__m2;
    private static Action<ControllerStatusChangedEventArgs> f__m3;
    private static OnUnconstrainedHandler f__m4;
    private static OnResumeHandler f__m5;
    private static OnSuspendHandler f__m6;
    private static InitializeCloudStoreHandler f__m7;

    public static event PlayerManager.PlayerChangedDelegate OnPlayerJoinedEvent;
    public static event PlayerManager.PlayerChangedDelegate OnPlayerLeaveEvent;
    public static event PlayerManager.PlayerChangedDelegate OnPlayerDisconnectedEvent;
    public static event Action OnControlsChanged;

    public static bool ShouldShowJoinPrompt
    {
        get
        {
            return PlayerManager.playerSlots[1].joinState == PlayerSlot.JoinState.JoinPromptDisplayed;
        }
    }

    public static void Awake()
    {
        PlayerManager.Multiplayer = false;
        PlayerManager.players = new Dictionary<int, AbstractPlayerController>();
        PlayerManager.players.Add(0, null);
        PlayerManager.players.Add(1, null);
        PlayerManager.players.Add(2, null);
        PlayerManager.players.Add(3, null);
        PlayerManager.playerInputs = new Dictionary<int, Player>();
        PlayerManager.playerInputs.Add(0, ReInput.players.GetPlayer(0));
        PlayerManager.playerInputs.Add(1, ReInput.players.GetPlayer(1));
        PlayerManager.playerInputs.Add(2, ReInput.players.GetPlayer(2));
        PlayerManager.playerInputs.Add(3, ReInput.players.GetPlayer(3));
    }

    public static void Init()
    {
        OnlineInterface @interface = OnlineManager.Instance.Interface;
        if (PlayerManager.f__m0 == null)
        {
            PlayerManager.f__m0 = new SignInEventHandler(PlayerManager.OnUserSignedIn);
        }
        @interface.OnUserSignedIn += PlayerManager.f__m0;
        OnlineInterface interface2 = OnlineManager.Instance.Interface;
        if (PlayerManager.f__m1 == null)
        {
            PlayerManager.f__m1 = new SignOutEventHandler(PlayerManager.OnUserSignedOut);
        }
        interface2.OnUserSignedOut += PlayerManager.f__m1;
        if (PlayerManager.f__m2 == null)
        {
            PlayerManager.f__m2 = new Action<ControllerStatusChangedEventArgs>(PlayerManager.OnControllerConnected);
        }
        ReInput.ControllerConnectedEvent += f__m2;
        if (PlayerManager.f__m3 == null)
        {
            PlayerManager.f__m3 = new Action<ControllerStatusChangedEventArgs>(PlayerManager.OnControllerDisconnected);
        }
        ReInput.ControllerDisconnectedEvent += f__m3;
        PlmInterface interface3 = PlmManager.Instance.Interface;
        if (PlayerManager.f__m4 == null)
        {
            PlayerManager.f__m4 = new OnUnconstrainedHandler(PlayerManager.OnUnconstrained);
        }
        interface3.OnUnconstrained += PlayerManager.f__m4;
        PlmInterface interface4 = PlmManager.Instance.Interface;
        if (PlayerManager.f__m5 == null)
		{
            PlayerManager.f__m5 = new OnResumeHandler(PlayerManager.OnResume);
        }
        interface4.OnResume += PlayerManager.f__m5;
        PlmInterface interface5 = PlmManager.Instance.Interface;
        if (PlayerManager.f__m6 == null)
		{
            PlayerManager.f__m6 = new OnSuspendHandler(PlayerManager.OnSuspend);
        }
        interface5.OnSuspend += PlayerManager.f__m6;
    }

    public static void SetPlayerCanJoin(PlayerId player, bool canJoin, bool promptBeforeJoin)
    {
        PlayerManager.PlayerSlot playerSlot;
        switch (player)
        {
            case PlayerId.PlayerOne:
                playerSlot = PlayerManager.playerSlots[0];
                break;
            case PlayerId.PlayerTwo:
                playerSlot = PlayerManager.playerSlots[1];
                break;
            case PlayerId.PlayerThree:
                playerSlot = PlayerManager.playerSlots[2];
                break;
            case PlayerId.PlayerFour:
                playerSlot = PlayerManager.playerSlots[3];
                break;
            default:
                playerSlot = PlayerManager.playerSlots[0];
                break;
        }
        playerSlot.canJoin = canJoin;
        playerSlot.mustJoin = false;
        playerSlot.promptBeforeJoin = promptBeforeJoin;
        if (!canJoin && playerSlot.joinState == PlayerManager.PlayerSlot.JoinState.JoinPromptDisplayed)
        {
            playerSlot.joinState = PlayerManager.PlayerSlot.JoinState.NotJoining;
        }
    }

    public static void SetPlayerMustJoin(PlayerId player, bool mustJoin, bool promptBeforeJoin)
    {
        PlayerManager.PlayerSlot playerSlot;
        switch (player)
        {
            case PlayerId.PlayerOne:
                playerSlot = PlayerManager.playerSlots[0];
                break;
            case PlayerId.PlayerTwo:
                playerSlot = PlayerManager.playerSlots[1];
                break;
            case PlayerId.PlayerThree:
                playerSlot = PlayerManager.playerSlots[2];
                break;
            case PlayerId.PlayerFour:
                playerSlot = PlayerManager.playerSlots[3];
                break;
            default:
                playerSlot = PlayerManager.playerSlots[0];
                break;
        }
        playerSlot.mustJoin = mustJoin;
        playerSlot.canJoin = false;
        playerSlot.waitForJoinRequest = false;
        playerSlot.promptBeforeJoin = promptBeforeJoin;
        if (!mustJoin && playerSlot.joinState == PlayerManager.PlayerSlot.JoinState.JoinPromptDisplayed)
        {
            playerSlot.joinState = PlayerManager.PlayerSlot.JoinState.NotJoining;
        }
    }

    public static void SetPlayerWaitForJoinRequest(PlayerId player)
    {
        PlayerManager.PlayerSlot playerSlot;
        switch (player)
        {
            case PlayerId.PlayerOne:
                playerSlot = PlayerManager.playerSlots[0];
                break;
            case PlayerId.PlayerTwo:
                playerSlot = PlayerManager.playerSlots[1];
                break;
            case PlayerId.PlayerThree:
                playerSlot = PlayerManager.playerSlots[2];
                break;
            case PlayerId.PlayerFour:
                playerSlot = PlayerManager.playerSlots[3];
                break;
            default:
                playerSlot = PlayerManager.playerSlots[0];
                break;
        }
        playerSlot.waitForJoinRequest = true;
        playerSlot.mustJoin = false;
        if (!playerSlot.canJoin && playerSlot.joinState == PlayerManager.PlayerSlot.JoinState.JoinPromptDisplayed)
        {
            playerSlot.joinState = PlayerManager.PlayerSlot.JoinState.NotJoining;
        }
    }

    public static void PlayerLeave(PlayerId player)
    {
        PlayerManager.PlayerSlot playerSlot;
        switch (player)
        {
            case PlayerId.PlayerOne:
                playerSlot = PlayerManager.playerSlots[0];
                break;
            case PlayerId.PlayerTwo:
                playerSlot = PlayerManager.playerSlots[1];
                break;
            case PlayerId.PlayerThree:
                playerSlot = PlayerManager.playerSlots[2];
                break;
            case PlayerId.PlayerFour:
                playerSlot = PlayerManager.playerSlots[3];
                break;
            default:
                playerSlot = PlayerManager.playerSlots[0];
                break;
        }
        playerSlot.joinState = PlayerManager.PlayerSlot.JoinState.Leaving;
    }

    public static void Update()
    {
        if (InterruptingPrompt.IsInterrupting())
        {
            for (int i = 0; i < PlayerManager.playerSlots.Length; i++)
            {
                if (PlayerManager.playerSlots[i].joinState == PlayerManager.PlayerSlot.JoinState.Joined && PlayerManager.playerSlots[i].controllerState == 
                    PlayerManager.PlayerSlot.ControllerState.ReconnectPromptDisplayed)
                {
                    PlayerId playerId;
                    switch (i)
                    {
                        case 0:
                            playerId = PlayerId.PlayerOne;
                            break;
                        case 1:
                            playerId = PlayerId.PlayerTwo;
                            break;
                        case 2:
                            playerId = PlayerId.PlayerThree;
                            break;
                        case 3:
                            playerId = PlayerId.PlayerFour;
                            break;
                        default:
                            playerId = PlayerId.None;
                            break;
                    }
                    Joystick joystick = MirrorOfDuskInput.CheckForUnconnectedControllerPress();
                    Player playerInput = PlayerManager.GetPlayerInput(playerId);
                    if (joystick != null)
                    {
                        PlayerManager.playerSlots[i].controllerState = PlayerManager.PlayerSlot.ControllerState.UsingController;
                        PlayerManager.playerSlots[i].controllerId = joystick.id;
                        PlayerManager.playerSlots[i].controllerDisconnectFromPlm = false;
                        PlayerManager.playerSlots[i].lastController = (ControllerType)2;
                        playerInput.controllers.AddController(joystick, true);
                        ReInput.userDataStore.LoadControllerData(PlayerManager.playerInputs[(int)playerId].id, (ControllerType)2, PlayerManager.playerSlots[i].controllerId);
                        PlayerManager.ControlsChanged();
                    }
                    if (!PlatformHelper.IsConsole && playerInput.GetAnyButtonDown())
                    {
                        PlayerManager.playerSlots[i].controllerState = PlayerManager.PlayerSlot.ControllerState.NoController;
                        PlayerManager.playerSlots[i].controllerDisconnectFromPlm = false;
                        PlayerManager.ControlsChanged();
                        PlayerManager.playerSlots[i].lastController = 0;
                    }
                }
            }
            return;
        }
        for (int j = 0; j < PlayerManager.playerSlots.Length; j++)
        {
            //Check if controllers are set to be automatically assigned upon connection.
            if (PlayerManager.allowAutomaticControllerSearch && PlayerManager.playerSlots[j].canJoin && PlayerManager.playerSlots[j].promptBeforeJoin)
            {
                PlayerId playerId2;
                switch (j)
                {
                    case 0:
                        playerId2 = PlayerId.PlayerOne;
                        break;
                    case 1:
                        playerId2 = PlayerId.PlayerTwo;
                        break;
                    case 2:
                        playerId2 = PlayerId.PlayerThree;
                        break;
                    case 3:
                        playerId2 = PlayerId.PlayerFour;
                        break;
                    default:
                        playerId2 = PlayerId.None;
                        break;
                }
                bool flag = false;
                Joystick joystick2 = null;
                if (playerId2 == PlayerId.PlayerOne && SettingsData.Data.buttonGuide != SettingsData.ButtonGuide.Keyboard_Only)
                {
                    joystick2 = MirrorOfDuskInput.AutomaticallyAssignJoystick(playerInputs[j]);
                }
                else if (playerId2 == PlayerId.PlayerTwo && SettingsData.Data.buttonGuide2 != SettingsData.ButtonGuide.Keyboard_Only)
                {
                    joystick2 = MirrorOfDuskInput.AutomaticallyAssignJoystick(playerInputs[j]);
                } else
                {
                    joystick2 = MirrorOfDuskInput.AutomaticallyAssignJoystick(playerInputs[j]);
                }
                Player playerInput2 = PlayerManager.GetPlayerInput(playerId2);
                if (joystick2 != null)
                {
                    flag = true;
                    PlayerManager.playerSlots[j].controllerState = PlayerManager.PlayerSlot.ControllerState.UsingController;
                    PlayerManager.playerSlots[j].controllerId = joystick2.id;
                }
                else 
                {
                    flag = true;
                    PlayerManager.playerSlots[j].controllerState = PlayerManager.PlayerSlot.ControllerState.NoController;
                }
                if (flag)
                {
                    if (PlayerManager.playerSlots[j].joinState == PlayerManager.PlayerSlot.JoinState.NotJoining && PlayerManager.playerSlots[j].promptBeforeJoin)
                    {
                        PlayerManager.playerSlots[j].joinState = PlayerManager.PlayerSlot.JoinState.JoinPromptDisplayed;
                    }
                    bool flag2 = false;
                    if (OnlineManager.Instance.Interface.SupportsMultipleUsers)
                    {
                        ulong value = (ulong)joystick2.systemId.Value;
                        OnlineUser userForController = OnlineManager.Instance.Interface.GetUserForController(value);
                        if (userForController != null && ((j == 0 && userForController.Equals(OnlineManager.Instance.Interface.MainUser)) || (j == 1 && userForController.Equals(OnlineManager.Instance.Interface.SecondaryUser)) || (j == 2 && userForController.Equals(OnlineManager.Instance.Interface.ThirdUser)) || (j == 3 && userForController.Equals(OnlineManager.Instance.Interface.FourthUser))))
                        {
                            OnlineManager.Instance.Interface.SetUser(playerId2, userForController);
                            flag2 = true;
                        }
                        else
                        {
                            OnlineManager.Instance.Interface.SignInUser(false, playerId2, value);
                        }
                    }
                    else if (OnlineManager.Instance.Interface.SupportsUserSignIn && playerId2 == PlayerId.PlayerOne)
                    {
                        OnlineManager.Instance.Interface.SignInUser(false, playerId2, 0UL);
                    }
                    else
                    {
                        flag2 = true;
                    }
                    if (flag2)
                    {
                        if (joystick2 != null)
                        {
                            playerInput2.controllers.AddController(joystick2, true);
                            ReInput.userDataStore.LoadControllerData(PlayerManager.playerInputs[(int)playerId2].id, (ControllerType)2, PlayerManager.playerSlots[j].controllerId);
                        }
                    }
                }
            }
            //Check if a player can join while their status is not currently joined or is joined while having no active controller.
            if (!PlayerManager.allowAutomaticControllerSearch && PlayerManager.playerSlots[j].canJoin && (PlayerManager.playerSlots[j].joinState != PlayerManager.PlayerSlot.JoinState.JoinRequested && PlayerManager.playerSlots[j].joinState != PlayerManager.PlayerSlot.JoinState.Joined || (PlayerManager.playerSlots[j].joinState == PlayerManager.PlayerSlot.JoinState.Joined && PlayerManager.playerSlots[j].controllerState == PlayerManager.PlayerSlot.ControllerState.NoController)))
            {
                PlayerId playerId2;
                switch (j)
                {
                    case 0:
                        playerId2 = PlayerId.PlayerOne;
                        break;
                    case 1:
                        playerId2 = PlayerId.PlayerTwo;
                        break;
                    case 2:
                        playerId2 = PlayerId.PlayerThree;
                        break;
                    case 3:
                        playerId2 = PlayerId.PlayerFour;
                        break;
                    default:
                        playerId2 = PlayerId.None;
                        break;
                }
                bool flag = false;
                Joystick joystick2 = MirrorOfDuskInput.CheckForUnconnectedControllerPress();
                Player playerInput2 = PlayerManager.GetPlayerInput(playerId2);
                if (joystick2 != null)
                {
                    flag = true;
                    PlayerManager.playerSlots[j].controllerState = PlayerManager.PlayerSlot.ControllerState.UsingController;
                    PlayerManager.playerSlots[j].controllerId = joystick2.id;
                }
                else if (!PlatformHelper.IsConsole && ((!(SceneManager.GetActiveScene().name == "scene_title")) ? (playerInput2.GetAnyButtonDown() && PlayerManager.playerSlots[j].joinState != PlayerManager.PlayerSlot.JoinState.Joined) : (playerInput2.controllers.Keyboard.GetAnyButtonDown() && 
                    PlayerManager.playerSlots[j].joinState == PlayerManager.PlayerSlot.JoinState.NotJoining)))
                {
                    flag = true;
                    PlayerManager.playerSlots[j].controllerState = PlayerManager.PlayerSlot.ControllerState.NoController;
                }
                if (flag)
                {
                    if (PlayerManager.playerSlots[j].joinState == PlayerManager.PlayerSlot.JoinState.NotJoining && PlayerManager.playerSlots[j].promptBeforeJoin)
                    {
                        PlayerManager.playerSlots[j].joinState = PlayerManager.PlayerSlot.JoinState.JoinPromptDisplayed;
                    } else
                    {
                        bool flag2 = false;
                        PlayerManager.playerSlots[j].joinState = PlayerManager.PlayerSlot.JoinState.JoinRequested;
                        if (OnlineManager.Instance.Interface.SupportsMultipleUsers)
                        {
                            ulong value = (ulong)joystick2.systemId.Value;
                            OnlineUser userForController = OnlineManager.Instance.Interface.GetUserForController(value);
                            if (userForController != null && ((j == 0 && userForController.Equals(OnlineManager.Instance.Interface.MainUser)) || (j == 1 && userForController.Equals(OnlineManager.Instance.Interface.SecondaryUser)) || (j == 2 && userForController.Equals(OnlineManager.Instance.Interface.ThirdUser)) || (j == 3 && userForController.Equals(OnlineManager.Instance.Interface.FourthUser))))
                            {
                                OnlineManager.Instance.Interface.SetUser(playerId2, userForController);
                                flag2 = true;
                            }
                            else
                            {
                                OnlineManager.Instance.Interface.SignInUser(false, playerId2, value);
                            }
                        }
                        else if (OnlineManager.Instance.Interface.SupportsUserSignIn && playerId2 == PlayerId.PlayerOne)
                        {
                            OnlineManager.Instance.Interface.SignInUser(false, playerId2, 0UL);
                        }
                        else
                        {
                            flag2 = true;
                        }
                        if (flag2)
                        {
                            if (joystick2 != null)
                            {
                                playerInput2.controllers.AddController(joystick2, true);
                                ReInput.userDataStore.LoadControllerData(PlayerManager.playerInputs[(int)playerId2].id, (ControllerType)2, PlayerManager.playerSlots[j].controllerId);
                            }
                            PlayerManager.playerSlots[j].joinState = PlayerManager.PlayerSlot.JoinState.Joined;
                            if (playerId2 == PlayerId.PlayerTwo || playerId2 == PlayerId.PlayerThree || playerId2 == PlayerId.PlayerFour)
                            {
                                PlayerManager.Multiplayer = true;
                            }
                            if (PlayerManager.OnPlayerJoinedEvent != null)
                            {
                                PlayerManager.OnPlayerJoinedEvent(playerId2);
                            }
                            if (PlayerManager.playerSlots[j].controllerState == PlayerManager.PlayerSlot.ControllerState.UsingController && SceneManager.GetActiveScene().name == "scene_title")
                            {
                                AudioManager.Play("select2");
                            }
                        }
                    }
                }
            }
            //Check if a player MUST join while their status is not currently joined or is joined while having no active controller.
            if (PlayerManager.playerSlots[j].mustJoin && (PlayerManager.playerSlots[j].joinState != PlayerManager.PlayerSlot.JoinState.Joined || (PlayerManager.playerSlots[j].joinState == PlayerManager.PlayerSlot.JoinState.Joined && PlayerManager.playerSlots[j].controllerState == PlayerManager.PlayerSlot.ControllerState.NoController)))
            {
                PlayerId playerId2;
                switch (j)
                {
                    case 0:
                        playerId2 = PlayerId.PlayerOne;
                        break;
                    case 1:
                        playerId2 = PlayerId.PlayerTwo;
                        break;
                    case 2:
                        playerId2 = PlayerId.PlayerThree;
                        break;
                    case 3:
                        playerId2 = PlayerId.PlayerFour;
                        break;
                    default:
                        playerId2 = PlayerId.None;
                        break;
                }
                bool flag = false;
                Joystick joystick2 = null;
                foreach (Joystick joystick in ReInput.controllers.Joysticks)
                {
                    if (!ReInput.controllers.IsJoystickAssigned(joystick))
                    {
                        joystick2 = joystick;
                        break;
                    }
                }
                Player playerInput2 = PlayerManager.GetPlayerInput(playerId2);
                if (joystick2 != null)
                {
                    flag = true;
                    PlayerManager.playerSlots[j].controllerState = PlayerManager.PlayerSlot.ControllerState.UsingController;
                    PlayerManager.playerSlots[j].controllerId = joystick2.id;
                }
                else
                {
                    flag = true;
                    PlayerManager.playerSlots[j].controllerState = PlayerManager.PlayerSlot.ControllerState.NoController;
                }
                if (flag)
                {
                    if (PlayerManager.playerSlots[j].joinState == PlayerManager.PlayerSlot.JoinState.NotJoining && PlayerManager.playerSlots[j].promptBeforeJoin)
                    {
                        PlayerManager.playerSlots[j].joinState = PlayerManager.PlayerSlot.JoinState.JoinPromptDisplayed;
                    }
                    else
                    {
                        bool flag2 = false;
                        PlayerManager.playerSlots[j].joinState = PlayerManager.PlayerSlot.JoinState.JoinRequested;
                        if (OnlineManager.Instance.Interface.SupportsMultipleUsers)
                        {
                            ulong value = (ulong)joystick2.systemId.Value;
                            OnlineUser userForController = OnlineManager.Instance.Interface.GetUserForController(value);
                            if (userForController != null && ((j == 0 && userForController.Equals(OnlineManager.Instance.Interface.MainUser)) || (j == 1 && userForController.Equals(OnlineManager.Instance.Interface.SecondaryUser)) || (j == 2 && userForController.Equals(OnlineManager.Instance.Interface.ThirdUser)) || (j == 3 && userForController.Equals(OnlineManager.Instance.Interface.FourthUser))))
                            {
                                OnlineManager.Instance.Interface.SetUser(playerId2, userForController);
                                flag2 = true;
                            }
                            else
                            {
                                OnlineManager.Instance.Interface.SignInUser(false, playerId2, value);
                            }
                        }
                        else if (OnlineManager.Instance.Interface.SupportsUserSignIn && playerId2 == PlayerId.PlayerOne)
                        {
                            OnlineManager.Instance.Interface.SignInUser(false, playerId2, 0UL);
                        }
                        else
                        {
                            flag2 = true;
                        }
                        if (flag2)
                        {
                            if (joystick2 != null)
                            {
                                playerInput2.controllers.AddController(joystick2, true);
                                ReInput.userDataStore.LoadControllerData(PlayerManager.playerInputs[(int)playerId2].id, (ControllerType)2, PlayerManager.playerSlots[j].controllerId);
                            }
                            PlayerManager.playerSlots[j].joinState = PlayerManager.PlayerSlot.JoinState.Joined;
                            if (playerId2 == PlayerId.PlayerTwo || playerId2 == PlayerId.PlayerThree || playerId2 == PlayerId.PlayerFour)
                            {
                                PlayerManager.Multiplayer = true;
                            }
                            if (PlayerManager.OnPlayerJoinedEvent != null)
                            {
                                PlayerManager.OnPlayerJoinedEvent(playerId2);
                            }
                        }
                    }
                }
            }
            //Check if a player is currently requesting to join while their status is not currently joined or is joined while having no active controller.
            if (PlayerManager.playerSlots[j].waitForJoinRequest && (PlayerManager.playerSlots[j].joinState == PlayerManager.PlayerSlot.JoinState.JoinRequested))
            {
                PlayerId playerId2;
                switch (j)
                {
                    case 0:
                        playerId2 = PlayerId.PlayerOne;
                        break;
                    case 1:
                        playerId2 = PlayerId.PlayerTwo;
                        break;
                    case 2:
                        playerId2 = PlayerId.PlayerThree;
                        break;
                    case 3:
                        playerId2 = PlayerId.PlayerFour;
                        break;
                    default:
                        playerId2 = PlayerId.None;
                        break;
                }
                PlayerManager.playerSlots[j].joinState = PlayerManager.PlayerSlot.JoinState.Joined;
                PlayerManager.playerSlots[j].waitForJoinRequest = false;
                if (PlayerManager.OnPlayerJoinedEvent != null)
                {
                    PlayerManager.OnPlayerJoinedEvent(playerId2);
                }
            }
        }
        //Check if a player wants to switch with another user.
        for (int k = 0; k < PlayerManager.playerSlots.Length; k++)
        {
            if (OnlineManager.Instance.Interface.SupportsUserSignIn && PlayerManager.playerSlots[k].canSwitch && PlayerManager.playerSlots[k].joinState == PlayerManager.PlayerSlot.JoinState.Joined)
            {
                if (OnlineManager.Instance.Interface.SupportsMultipleUsers || k == 0)
                {
                    PlayerId playerId3;
                    switch (k)
                    {
                        case 0:
                            playerId3 = PlayerId.PlayerOne;
                            break;
                        case 1:
                            playerId3 = PlayerId.PlayerTwo;
                            break;
                        case 2:
                            playerId3 = PlayerId.PlayerThree;
                            break;
                        case 3:
                            playerId3 = PlayerId.PlayerFour;
                            break;
                        default:
                            playerId3 = PlayerId.None;
                            break;
                    }
                    Player playerInput3 = PlayerManager.GetPlayerInput(playerId3);
                    if (playerInput3.GetButtonDown(11))
                    {
                        PlayerManager.playerSlots[k].requestedSwitch = true;
                        PlayerManager.playerSlots[(k + 1) % 2].requestedSwitch = false;
                        ulong controllerId = 0UL;
                        if (playerInput3.controllers.joystickCount > 0)
                        {
                            controllerId = (ulong)playerInput3.controllers.Joysticks[0].systemId.Value;
                        }
                        OnlineManager.Instance.Interface.SwitchUser(playerId3, controllerId);
                    }
                }
            }
        }
        //Check if a player is cancelling from joining.
        for (int l = 0; l < PlayerManager.playerSlots.Length; l++)
        {
            if (PlayerManager.playerSlots[l].joinState == PlayerManager.PlayerSlot.JoinState.LeaveRequested)
            {
                PlayerId playerId4;
                switch (l)
                {
                    case 0:
                        playerId4 = PlayerId.PlayerOne;
                        break;
                    case 1:
                        playerId4 = PlayerId.PlayerTwo;
                        break;
                    case 2:
                        playerId4 = PlayerId.PlayerThree;
                        break;
                    case 3:
                        playerId4 = PlayerId.PlayerFour;
                        break;
                    default:
                        playerId4 = PlayerId.None;
                        break;
                }
                Player playerInput4 = PlayerManager.GetPlayerInput(playerId4);
                //playerInput4.controllers.ClearControllersOfType<Joystick>();
                if (PlayerManager.OnPlayerLeaveEvent != null)
                {
                    PlayerManager.OnPlayerLeaveEvent(playerId4);
                }
                PlayerManager.playerSlots[l].joinState = PlayerManager.PlayerSlot.JoinState.NotJoining;
                OnlineManager.Instance.Interface.SetRichPresenceActive(playerId4, false);
                OnlineManager.Instance.Interface.SetUser(playerId4, null);
            }
        }
        //Check if a player is leaving.
        for (int l = 0; l < PlayerManager.playerSlots.Length; l++)
        {
            if (SceneLoader.CurrentlyLoading)
            {
                break;
            }
            if (PlayerManager.playerSlots[l].joinState == PlayerManager.PlayerSlot.JoinState.Leaving)
            {
                PlayerId playerId4;
                switch (l)
                {
                    case 0:
                        playerId4 = PlayerId.PlayerOne;
                        break;
                    case 1:
                        playerId4 = PlayerId.PlayerTwo;
                        break;
                    case 2:
                        playerId4 = PlayerId.PlayerThree;
                        break;
                    case 3:
                        playerId4 = PlayerId.PlayerFour;
                        break;
                    default:
                        playerId4 = PlayerId.None;
                        break;
                }
                Player playerInput4 = PlayerManager.GetPlayerInput(playerId4);
                playerInput4.controllers.ClearControllersOfType<Joystick>();
                PlayerManager.playerSlots[l].joinState = PlayerManager.PlayerSlot.JoinState.NotJoining;
                if (playerId4 == PlayerId.PlayerTwo || playerId4 == PlayerId.PlayerThree || playerId4 == PlayerId.PlayerFour)
                {
                    PlayerManager.Multiplayer = false;
                }
                OnlineManager.Instance.Interface.SetRichPresenceActive(playerId4, false);
                OnlineManager.Instance.Interface.SetUser(playerId4, null);
                if (playerId4 == PlayerId.PlayerOne)
                {
                    PlayerManager.shouldGoToStartScreen = true;
                }
                else if (PlayerManager.OnPlayerLeaveEvent != null)
                {
                    PlayerManager.OnPlayerLeaveEvent(playerId4);
                    AudioManager.Play("player_despawn");
                }
            }
        }
        //Check if a player should have a controller assigned.
        for (int m = 0; m < PlayerManager.playerSlots.Length; m++)
        {
            if (PlayerManager.playerSlots[m].shouldAssignController)
            {
                PlayerId playerId5;
                switch (m)
                {
                    case 0:
                        playerId5 = PlayerId.PlayerOne;
                        break;
                    case 1:
                        playerId5 = PlayerId.PlayerTwo;
                        break;
                    case 2:
                        playerId5 = PlayerId.PlayerThree;
                        break;
                    case 3:
                        playerId5 = PlayerId.PlayerFour;
                        break;
                    default:
                        playerId5 = PlayerId.None;
                        break;
                }
                Player playerInput5 = PlayerManager.GetPlayerInput(playerId5);
                playerInput5.controllers.AddController<Joystick>(PlayerManager.playerSlots[m].controllerId, true);
                ReInput.userDataStore.LoadControllerData(PlayerManager.playerInputs[(int)playerId5].id, (ControllerType)2, PlayerManager.playerSlots[m].controllerId);
                PlayerManager.playerSlots[m].shouldAssignController = false;
            }
        }
        //Check if a message regarding a controller disconnection is allowed to appear.
        if (ControllerDisconnectedPrompt.Instance != null && !ControllerDisconnectedPrompt.Instance.Visible && ControllerDisconnectedPrompt.Instance.allowedToShow)
        {
            for (int n = 0; n < 4; n++)
            {
                PlayerId playerId6;
                switch (n)
                {
                    case 0:
                        playerId6 = PlayerId.PlayerOne;
                        break;
                    case 1:
                        playerId6 = PlayerId.PlayerTwo;
                        break;
                    case 2:
                        playerId6 = PlayerId.PlayerThree;
                        break;
                    case 3:
                        playerId6 = PlayerId.PlayerFour;
                        break;
                    default:
                        playerId6 = PlayerId.None;
                        break;
                }
                if (PlayerManager.IsControllerDisconnected(playerId6, false))
                {
                    ControllerDisconnectedPrompt.Instance.Show(playerId6);
                    break;
                }
            }
        }
        //Check if a controller disconnection has occurred overall.
        if (ControllerDisconnectedPrompt.Instance == null || (ControllerDisconnectedPrompt.Instance != null && !ControllerDisconnectedPrompt.Instance.allowedToShow))
        {
            for (int n = 0; n < 4; n++)
            {
                PlayerId playerId6;
                switch (n)
                {
                    case 0:
                        playerId6 = PlayerId.PlayerOne;
                        break;
                    case 1:
                        playerId6 = PlayerId.PlayerTwo;
                        break;
                    case 2:
                        playerId6 = PlayerId.PlayerThree;
                        break;
                    case 3:
                        playerId6 = PlayerId.PlayerFour;
                        break;
                    default:
                        playerId6 = PlayerId.None;
                        break;
                }
                if (PlayerManager.IsControllerDisconnected(playerId6, false))
                {
                    if (PlayerManager.OnPlayerDisconnectedEvent != null)
                    {
                        PlayerManager.OnPlayerDisconnectedEvent(playerId6);
                    }
                    PlayerManager.playerSlots[n].controllerState = PlayerManager.PlayerSlot.ControllerState.NoController;
                    break;
                }
            }
        }
        if (PlmManager.Instance.Interface.IsConstrained())
        {
            if (InterruptingPrompt.CanInterrupt() && PauseManager.state != PauseManager.State.Paused)
            {
                PauseManager.Pause();
                PlayerManager.pausedDueToPlm = true;
            }
        }
        else if (PlayerManager.pausedDueToPlm)
        {
            PauseManager.Unpause();
            PlayerManager.pausedDueToPlm = false;
        }
        if (PlayerManager.shouldGoToSlotSelect)
        {
            PlayerManager.goToSlotSelect();
            PlayerManager.shouldGoToSlotSelect = false;
        }
        if (PlayerManager.shouldGoToStartScreen)
        {
            PlayerManager.goToStartScreen();
            PlayerManager.shouldGoToStartScreen = false;
        }
        //Check for a player's most recently active controller.
        for (int num = 0; num < 4; num++)
        {
            PlayerId id;
            switch (num)
            {
                case 0:
                    id = PlayerId.PlayerOne;
                    break;
                case 1:
                    id = PlayerId.PlayerTwo;
                    break;
                case 2:
                    id = PlayerId.PlayerThree;
                    break;
                case 3:
                    id = PlayerId.PlayerFour;
                    break;
                default:
                    id = PlayerId.None;
                    break;
            }
            Controller lastActiveController = PlayerManager.GetPlayerInput(id).controllers.GetLastActiveController();
            if (lastActiveController != null && lastActiveController.type != PlayerManager.playerSlots[num].lastController)
            {
                PlayerManager.playerSlots[num].lastController = lastActiveController.type;
                PlayerManager.ControlsChanged();
            }
        }
    }

    public static void ControllerRemapped(PlayerId playerId, bool usingController, int controllerId)
    {
        int num;
        switch (playerId)
        {
            case PlayerId.PlayerOne:
                num = 0;
                break;
            case PlayerId.PlayerTwo:
                num = 1;
                break;
            case PlayerId.PlayerThree:
                num = 2;
                break;
            case PlayerId.PlayerFour:
                num = 3;
                break;
            default:
                num = 2147483647;
                break;
        }
        PlayerManager.playerSlots[num].controllerState = ((!usingController) ? PlayerManager.PlayerSlot.ControllerState.NoController : PlayerManager.PlayerSlot.ControllerState.UsingController);
        PlayerManager.playerSlots[num].controllerId = controllerId;
    }

    public static void ControlsChanged()
    {
        if (PlayerManager.OnControlsChanged != null)
        {
            PlayerManager.OnControlsChanged();
        }
    }

    private static void OnUserSignedIn(OnlineUser user)
    {
        for (int i = 0; i < PlayerManager.playerSlots.Length; i++)
        {
            if (PlayerManager.playerSlots[i].canJoin && PlayerManager.playerSlots[i].joinState == PlayerManager.PlayerSlot.JoinState.JoinRequested)
            {
                OnlineManager.Instance.Interface.UpdateControllerMapping();
                if (user == null || (i == 0 && (user.Equals(OnlineManager.Instance.Interface.SecondaryUser) || user.Equals(OnlineManager.Instance.Interface.ThirdUser) || user.Equals(OnlineManager.Instance.Interface.FourthUser))) || (i > 0 && user.Equals(OnlineManager.Instance.Interface.MainUser)))
                {
                    PlayerManager.playerSlots[i].joinState = PlayerManager.PlayerSlot.JoinState.NotJoining;
                } else
                {
                    PlayerId playerId;
                    switch (i)
                    {
                        case 0:
                            playerId = PlayerId.PlayerOne;
                            break;
                        case 1:
                            playerId = PlayerId.PlayerTwo;
                            break;
                        case 2:
                            playerId = PlayerId.PlayerThree;
                            break;
                        case 3:
                            playerId = PlayerId.PlayerFour;
                            break;
                        default:
                            playerId = PlayerId.None;
                            break;
                    }
                    OnlineManager.Instance.Interface.SetUser(playerId, user);
                    if (PlayerManager.playerSlots[i].controllerState == PlayerManager.PlayerSlot.ControllerState.UsingController)
                    {
                        PlayerManager.playerSlots[i].shouldAssignController = true;
                    }
                    PlayerManager.playerSlots[i].joinState = PlayerManager.PlayerSlot.JoinState.Joined;
                    if (playerId == PlayerId.PlayerTwo)
                    {
                        PlayerManager.Multiplayer = true;
                    }
                    PlayerManager.OnPlayerJoinedEvent(playerId);
                }
            }
        }
        for (int j = 0; j < PlayerManager.playerSlots.Length; j++)
        {
            if (PlayerManager.playerSlots[j].canSwitch && PlayerManager.playerSlots[j].requestedSwitch && PlayerManager.playerSlots[j].joinState == PlayerManager.PlayerSlot.JoinState.Joined)
            {
                OnlineManager.Instance.Interface.UpdateControllerMapping();
                PlayerManager.playerSlots[j].requestedSwitch = false;
                PlayerId player;
                switch (j)
                {
                    case 0:
                        player = PlayerId.PlayerOne;
                        break;
                    case 1:
                        player = PlayerId.PlayerTwo;
                        break;
                    case 2:
                        player = PlayerId.PlayerThree;
                        break;
                    case 3:
                        player = PlayerId.PlayerFour;
                        break;
                    default:
                        player = PlayerId.None;
                        break;
                }
                if (user != null && !user.Equals(OnlineManager.Instance.Interface.MainUser) && !user.Equals(OnlineManager.Instance.Interface.SecondaryUser) && !user.Equals(OnlineManager.Instance.Interface.ThirdUser) && !user.Equals(OnlineManager.Instance.Interface.FourthUser))
                {
                    OnlineManager.Instance.Interface.SetUser(player, user);
                    if (j == 0)
                    {
                        PlayerManager.shouldGoToSlotSelect = true;
                    }
                }
            }
        }
    }

    private static void OnUserSignedOut(PlayerId player, string name)
    {
        if (PlmManager.Instance.Interface.IsConstrained())
        {
            return;
        }
        PlayerManager.PlayerSlot playerSlot;
        switch (player) {
            case PlayerId.PlayerOne:
                playerSlot = PlayerManager.playerSlots[0];
                break;
            case PlayerId.PlayerTwo:
                playerSlot = PlayerManager.playerSlots[1];
                break;
            case PlayerId.PlayerThree:
                playerSlot = PlayerManager.playerSlots[2];
                break;
            case PlayerId.PlayerFour:
                playerSlot = PlayerManager.playerSlots[3];
                break;
            default:
                playerSlot = PlayerManager.playerSlots[0];
                break;
        }
        if (playerSlot.requestedSwitch)
        {
            return;
        }
        PlayerManager.PlayerLeave(player);
    }
    
    private static void OnControllerConnected(ControllerStatusChangedEventArgs args)
    {

    }

    private static void OnControllerDisconnected(ControllerStatusChangedEventArgs args)
    {
        if (PlmManager.Instance.Interface.IsConstrained())
        {
            return;
        }
        for (int i = 0; i < PlayerManager.playerSlots.Length; i++)
        {
            PlayerId playerId;
            switch (i)
            {
                case 0:
                    playerId = PlayerId.PlayerOne;
                    break;
                case 1:
                    playerId = PlayerId.PlayerTwo;
                    break;
                case 2:
                    playerId = PlayerId.PlayerThree;
                    break;
                case 3:
                    playerId = PlayerId.PlayerFour;
                    break;
                default:
                    playerId = PlayerId.None;
                    break;
            }
            if (PlayerManager.playerSlots[i].controllerState == PlayerManager.PlayerSlot.ControllerState.UsingController && PlayerManager.playerSlots[i].controllerId == args.controllerId &&
                PlayerManager.playerSlots[i].joinState == PlayerManager.PlayerSlot.JoinState.Joined)
            {
                PlayerManager.playerInputs[(int)playerId].controllers.RemoveController<Joystick>(args.controllerId);
                PlayerManager.playerSlots[i].controllerState = PlayerManager.PlayerSlot.ControllerState.Disconnected;
                if (playerId == PlayerId.PlayerOne)
                {
                    PlayerManager.player1DisconnectedControllerId = args.controllerId;
                }
            }
        }
    }

    private static void OnSuspend()
    {
    }

    private static void OnResume()
    {
    }

    private static void OnUnconstrained()
    {
        PlayerManager.CheckForPairingsChanges();
    }

    private static void CheckForPairingsChanges()
    {
        bool flag = OnlineManager.Instance.Interface.ControllerMappingChanged();
        for (int i = 0; i < PlayerManager.playerSlots.Length; i++)
        {
            PlayerId playerId;
            switch (i)
            {
                case 0:
                    playerId = PlayerId.PlayerOne;
                    break;
                case 1:
                    playerId = PlayerId.PlayerTwo;
                    break;
                case 2:
                    playerId = PlayerId.PlayerThree;
                    break;
                case 3:
                    playerId = PlayerId.PlayerFour;
                    break;
                default:
                    playerId = PlayerId.None;
                    break;
            }
            if (PlayerManager.playerSlots[i].joinState == PlayerManager.PlayerSlot.JoinState.Joined)
            {
                if (!OnlineManager.Instance.Interface.IsUserSignedIn(playerId))
                {
                    PlayerManager.PlayerLeave(playerId);
                    if (playerId == PlayerId.PlayerOne)
                    {
                        PlayerManager.PlayerLeave(PlayerId.PlayerTwo);
                        PlayerManager.PlayerLeave(PlayerId.PlayerThree);
                        PlayerManager.PlayerLeave(PlayerId.PlayerFour);
                    }
                } else if (!flag)
                {
                    if (PlayerManager.playerSlots[i].controllerState == PlayerManager.PlayerSlot.ControllerState.UsingController && PlayerManager.playerInputs[(int)playerId].controllers.joystickCount == 0)
                    {
                        PlayerManager.playerInputs[(int)playerId].controllers.AddController<Joystick>(PlayerManager.playerSlots[i].controllerId, true);
                        ReInput.userDataStore.LoadControllerData(PlayerManager.playerInputs[(int)playerId].id, (ControllerType)2, PlayerManager.playerSlots[i].controllerId);
                    }
                } else
                {
                    List<ulong> controllersForUser = OnlineManager.Instance.Interface.GetControllersForUser(playerId);
                    if (controllersForUser == null || controllersForUser.Count != 1)
                    {
                        PlayerManager.playerInputs[(int)playerId].controllers.ClearControllersOfType<Joystick>();
                        PlayerManager.playerSlots[i].controllerState = PlayerManager.PlayerSlot.ControllerState.Disconnected;
                        PlayerManager.playerSlots[i].controllerDisconnectFromPlm = true;
                    } else
                    {
                        ulong num = controllersForUser[0];
                        foreach (Joystick joystick in ReInput.controllers.Joysticks)
                        {
                            if (joystick.systemId.Value == (long)num)
                            {
                                if (PlayerManager.playerInputs[(int)playerId].controllers.joystickCount > 0)
                                {

                                }
                                PlayerManager.playerInputs[(int)playerId].controllers.ClearControllersOfType<Joystick>();
                                PlayerManager.playerInputs[(int)playerId].controllers.AddController(joystick, true);
                                ReInput.userDataStore.LoadControllerData(PlayerManager.playerInputs[(int)playerId].id, (ControllerType)2, PlayerManager.playerSlots[i].controllerId);
                                PlayerManager.playerSlots[i].controllerId = joystick.id;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    public static void LoadControllerMappings(PlayerId player)
    {
        int num;
        switch (player)
        {
            case PlayerId.PlayerOne:
                num = 0;
                break;
            case PlayerId.PlayerTwo:
                num = 1;
                break;
            case PlayerId.PlayerThree:
                num = 2;
                break;
            case PlayerId.PlayerFour:
                num = 3;
                break;
            default:
                num = 2147483647;
                break;
        }
        ReInput.userDataStore.LoadControllerData(PlayerManager.playerInputs[(int)player].id, (ControllerType)2, PlayerManager.playerSlots[num].controllerId);
    }

    public static bool IsControllerDisconnected(PlayerId playerId, bool countWaitingForReconnectAsDisconnected = true)
    {
        int num;
        switch (playerId)
        {
            case PlayerId.PlayerOne:
                num = 0;
                break;
            case PlayerId.PlayerTwo:
                num = 1;
                break;
            case PlayerId.PlayerThree:
                num = 2;
                break;
            case PlayerId.PlayerFour:
                num = 3;
                break;
            default:
                num = 2147483647;
                break;
        }
        return PlayerManager.playerSlots[num].joinState == PlayerManager.PlayerSlot.JoinState.Joined && (PlayerManager.playerSlots[num].controllerState == PlayerManager.PlayerSlot.ControllerState.Disconnected || 
            PlayerManager.playerSlots[num].controllerState == PlayerManager.PlayerSlot.ControllerState.ReconnectPromptDisplayed || (countWaitingForReconnectAsDisconnected && 
            PlayerManager.playerSlots[num].controllerState == PlayerManager.PlayerSlot.ControllerState.WaitingForReconnect));
    }

    public static void OnDisconnectPromptDisplayed(PlayerId playerId)
    {
        int num;
        switch (playerId)
        {
            case PlayerId.PlayerOne:
                num = 0;
                break;
            case PlayerId.PlayerTwo:
                num = 1;
                break;
            case PlayerId.PlayerThree:
                num = 2;
                break;
            case PlayerId.PlayerFour:
                num = 3;
                break;
            default:
                num = 2147483647;
                break;
        }
        PlayerManager.playerSlots[num].controllerState = PlayerManager.PlayerSlot.ControllerState.ReconnectPromptDisplayed;
    }

    private static void goToSlotSelect()
    {
        MirrorOfDusk.Current.controlMapper.Close(true);
        PlayerManager.playerSlots[0].canSwitch = false;
        PlayerManager.playerSlots[0].requestedSwitch = false;
        PlayerManager.playerSlots[0].canJoin = false;
        PlayerManager.GetPlayerInput(PlayerId.PlayerTwo).controllers.ClearControllersOfType<Joystick>();
        PlayerManager.playerSlots[1] = new PlayerManager.PlayerSlot();
        PlayerManager.Multiplayer = false;
        OnlineManager.Instance.Interface.SetUser(PlayerId.PlayerTwo, null);
        SceneLoader.LoadScene(Scenes.scene_main_menu, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris, SceneLoader.Icon.Hourglass);
    }

    private static void goToStartScreen()
    {
        MirrorOfDusk.Current.controlMapper.Close(true);
        PlayerManager.ResetPlayers();
        if (StartScreenAudio.Instance != null)
        {
            UnityEngine.Object.Destroy(StartScreenAudio.Instance.gameObject);
        }
        SceneLoader.LoadScene(Scenes.scene_title, SceneLoader.Transition.Fade, SceneLoader.Transition.Fade, SceneLoader.Icon.Hourglass);
    }

    public static void ResetPlayers()
    {
        PlayerManager.playerSlots[0] = new PlayerManager.PlayerSlot();
        PlayerManager.playerSlots[1] = new PlayerManager.PlayerSlot();
        PlayerManager.playerSlots[2] = new PlayerManager.PlayerSlot();
        PlayerManager.playerSlots[3] = new PlayerManager.PlayerSlot();
        PlayerManager.GetPlayerInput(PlayerId.PlayerOne).controllers.ClearControllersOfType<Joystick>();
        PlayerManager.GetPlayerInput(PlayerId.PlayerTwo).controllers.ClearControllersOfType<Joystick>();
        PlayerManager.GetPlayerInput(PlayerId.PlayerThree).controllers.ClearControllersOfType<Joystick>();
        PlayerManager.GetPlayerInput(PlayerId.PlayerFour).controllers.ClearControllersOfType<Joystick>();
        PlayerManager.Multiplayer = false;
        if (OnlineManager.Instance.Interface.SupportsMultipleUsers)
        {
            OnlineManager.Instance.Interface.SetUser(PlayerId.PlayerOne, null);
            OnlineManager.Instance.Interface.SetUser(PlayerId.PlayerTwo, null);
            OnlineManager.Instance.Interface.SetUser(PlayerId.PlayerThree, null);
            OnlineManager.Instance.Interface.SetUser(PlayerId.PlayerFour, null);
        }
    }

    public static Player GetPlayerInput(PlayerId id)
    {
        return PlayerManager.playerInputs[(int)id];
    }

    public static Controller GetPlayerJoystick(PlayerId id)
    {
        foreach (Controller joystick in ReInput.controllers.Controllers)
        {
            if (joystick.type == ControllerType.Joystick)
            {
                if (PlayerManager.playerInputs[(int)id].controllers.ContainsController(joystick))
                {
                    return joystick;
                }
            }
        }
        return null;
    }

    public static AbstractPlayerController Current
    {
        get
        {
            return PlayerManager.GetPlayer(PlayerManager.currentId);
        }
    }

    public static void SetPlayer(PlayerId id, AbstractPlayerController player)
    {
        PlayerManager.players[(int)id] = player;
    }
    
    public static void ClearPlayer(PlayerId id)
    {
        PlayerManager.players[(int)id] = null;
    }
    
    public static void ClearPlayers()
    {
        PlayerManager.currentId = PlayerId.PlayerOne;
        PlayerManager.players[0] = null;
        PlayerManager.players[1] = null;
        PlayerManager.players[2] = null;
        PlayerManager.players[3] = null;
    }

    public static AbstractPlayerController GetPlayer(PlayerId id)
    {
        return PlayerManager.players[(int)id];
    }

    public static T GetPlayer<T>(PlayerId id) where T : AbstractPlayerController
    {
        return PlayerManager.GetPlayer(id) as T;
    }

    public static AbstractPlayerController GetRandom()
    {
        if (!PlayerManager.Multiplayer || !(PlayerManager.DoesPlayerExist(PlayerId.PlayerTwo) && PlayerManager.DoesPlayerExist(PlayerId.PlayerThree) && PlayerManager.DoesPlayerExist(PlayerId.PlayerFour)))
        {
            return PlayerManager.players[0];
        }
        return PlayerManager.GetPlayer(EnumUtils.Random<PlayerId>());
    }

    public static AbstractPlayerController GetNext()
    {
        if (!PlayerManager.Multiplayer || !(PlayerManager.DoesPlayerExist(PlayerId.PlayerTwo) && PlayerManager.DoesPlayerExist(PlayerId.PlayerThree) && PlayerManager.DoesPlayerExist(PlayerId.PlayerFour)))
        {
            return PlayerManager.players[0];
        }
        if (!PlayerManager.DoesPlayerExist(PlayerId.PlayerOne))
        {
            if (!PlayerManager.DoesPlayerExist(PlayerId.PlayerTwo))
            {
                if (!PlayerManager.DoesPlayerExist(PlayerId.PlayerThree))
                {
                    return PlayerManager.players[3];
                }
                return PlayerManager.players[2];
            }
            return PlayerManager.players[1];
        }
        AbstractPlayerController result = PlayerManager.Current;
        PlayerId playerId = PlayerManager.currentId;
        if (playerId == PlayerId.PlayerOne)
        {
            PlayerManager.currentId = PlayerId.PlayerTwo;
        }
        else if (playerId == PlayerId.PlayerTwo)
        {
            PlayerManager.currentId = PlayerId.PlayerThree;
        }
        else if (playerId == PlayerId.PlayerThree)
        {
            PlayerManager.currentId = PlayerId.PlayerFour;
        }
        else
        {
            PlayerManager.currentId = PlayerId.PlayerOne;
        }
        return result;
    }

    private static bool DoesPlayerExist(PlayerId player)
    {
        return !(PlayerManager.players[(int)player] == null) && !PlayerManager.players[(int)player].IsKOed;
    }

    public static AbstractPlayerController GetFirst()
    {
        if (!PlayerManager.DoesPlayerExist(PlayerId.PlayerOne))
        {
            if (!PlayerManager.DoesPlayerExist(PlayerId.PlayerTwo))
            {
                if (!PlayerManager.DoesPlayerExist(PlayerId.PlayerThree))
                {
                    return PlayerManager.players[3];
                }
                return PlayerManager.players[2];
            }
            return PlayerManager.players[1];
        }
        return PlayerManager.players[0];
    }

    public static Dictionary<int, AbstractPlayerController>.ValueCollection GetAllPlayers()
    {
        return PlayerManager.players.Values;
    }

    public static int Count
    {
        get
        {
            int num = 0;
            foreach (int num2 in PlayerManager.players.Keys)
            {
                if (PlayerManager.DoesPlayerExist((PlayerId)num2) && !PlayerManager.GetPlayer((PlayerId)num2).IsKOed)
                {
                    num++;
                }
            }
            return num;
        }
    }

    public static bool IsPlayerUsingController(PlayerId player)
    {
        PlayerManager.PlayerSlot playerSlot;
        switch (player)
        {
            case PlayerId.PlayerOne:
                playerSlot = PlayerManager.playerSlots[0];
                break;
            case PlayerId.PlayerTwo:
                playerSlot = PlayerManager.playerSlots[1];
                break;
            case PlayerId.PlayerThree:
                playerSlot = PlayerManager.playerSlots[2];
                break;
            case PlayerId.PlayerFour:
                playerSlot = PlayerManager.playerSlots[3];
                break;
            default:
                playerSlot = PlayerManager.playerSlots[0];
                break;
        }
        if (playerSlot.controllerState == PlayerSlot.ControllerState.UsingController)
            return true;
        return false;
    }

    public static bool GetPlayerStatus(PlayerId player, int status)
    {
        PlayerManager.PlayerSlot playerSlot;
        switch (player)
        {
            case PlayerId.PlayerOne:
                playerSlot = PlayerManager.playerSlots[0];
                break;
            case PlayerId.PlayerTwo:
                playerSlot = PlayerManager.playerSlots[1];
                break;
            case PlayerId.PlayerThree:
                playerSlot = PlayerManager.playerSlots[2];
                break;
            case PlayerId.PlayerFour:
                playerSlot = PlayerManager.playerSlots[3];
                break;
            default:
                playerSlot = PlayerManager.playerSlots[0];
                break;
        }
        if (playerSlot.joinState == (PlayerSlot.JoinState)status)
            return true;
        return false;
    }

    public static void SetPlayerJoinState(PlayerId player, int status)
    {
        PlayerManager.PlayerSlot playerSlot;
        switch (player)
        {
            case PlayerId.PlayerOne:
                playerSlot = PlayerManager.playerSlots[0];
                break;
            case PlayerId.PlayerTwo:
                playerSlot = PlayerManager.playerSlots[1];
                break;
            case PlayerId.PlayerThree:
                playerSlot = PlayerManager.playerSlots[2];
                break;
            case PlayerId.PlayerFour:
                playerSlot = PlayerManager.playerSlots[3];
                break;
            default:
                return;
        }
        playerSlot.joinState = (PlayerManager.PlayerSlot.JoinState)status;
    }

    /*public static Vector2 Center
    {
        get
        {
            if (!PlayerManager.Multiplayer || PlayerManager.Count < 2)
            {
                return PlayerManager.GetFirst().center;
            }
            return (PlayerManager.players[0].center + PlayerManager.players[1].center) / 2f;
        }
    }

    public static Vector2 CameraCenter
    {
        get
        {
            if (!PlayerManager.Multiplayer || PlayerManager.Count < 2)
            {
                return PlayerManager.GetFirst().CameraCenter;
            }
            return (PlayerManager.players[0].center + PlayerManager.players[1].CameraCenter) / 2f;
        }
    }*/
}
