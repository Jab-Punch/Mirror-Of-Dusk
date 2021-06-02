using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Rewired;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsGUIPlayer : AbstractMB
{
    [SerializeField] private OptionsGUI optionsRoot;
    private const float START_DELAY = 1f;
    [SerializeField] private PlayerId player;
    private Player input;
    private Controller playerController;
    private ControllerMap playerControllerMap;
    private bool _checkUpdate = false;

    public bool CheckUpdate
    {
        get { return _checkUpdate; }
        set { _checkUpdate = value; }
    }
    private bool _directionBusy = false;
    private bool _commandBusy = false;

    public Controller PlayerController
    {
        get { if (playerController != null)
            {
                return playerController;
            }
            return null;
        }
    }

    public bool DirectionBusy
    {
        get { return _directionBusy; }
        set { _directionBusy = value; }
    }

    public bool CommandBusy
    {
        get { return _commandBusy; }
        set { _commandBusy = value; }
    }

    public PlayerId OG_PlayerId
    {
        get { return this.player; }
    }

    public OptionsGUIPlayer.State state;
    public OptionsGUIPlayer.SelectedState selectedState;
    private bool playerLeft;
    private int pollingElement = -1;

    public delegate void PressDelegate();
    public delegate void PressDelegateSel(int selection);
    public delegate void PressDelegateButton(MirrorOfDuskButton foundButton);
    public delegate void PressDelegateButtonId(string foundButton, int id);
    public delegate void PressDelegateMove(bool down, bool up, bool right);

    public event OptionsGUIPlayer.PressDelegateMove OnOptionsUpDownEvent;
    public event OptionsGUIPlayer.PressDelegateSel OnOptionsAcceptEvent;
    public event OptionsGUIPlayer.PressDelegate OnOptionsCancelEvent;
    public event OptionsGUIPlayer.PressDelegateButton OnOptionsConfigEvent;
    public event OptionsGUIPlayer.PressDelegateButtonId OnOptionsConfigIdEvent;

    public enum State
    {
        Init,
        Selecting,
        Menu
    }

    public enum SelectedState
    {
        Off,
        On
    }

    protected override void Awake()
    {
        base.Awake();

    }

    private void OnEnable()
    {
        if (this.state != OptionsGUIPlayer.State.Init)
        {
            OnOptionsUpDownEvent += optionsRoot.OnPressOptionsUpDown;
            OnOptionsAcceptEvent += optionsRoot.OnPressOptionsAccept;
            OnOptionsCancelEvent += optionsRoot.OnPressOptionsCancel;
            OnOptionsConfigEvent += optionsRoot.OnPressOptionsConfig;
            OnOptionsConfigIdEvent += optionsRoot.OnPressOptionsConfigId;
            PlayerManager.OnPlayerDisconnectedEvent += this.OnPlayerLeft;
            if (input.controllers.joystickCount > 0)
            {
                foreach (Controller cont in input.controllers.Controllers)
                {
                    if (cont.type == ControllerType.Joystick && cont.isConnected)
                    {
                        playerController = input.controllers.GetController(ControllerType.Joystick, cont.id);
                        if (optionsRoot.state == OptionsGUI.State.UserControllerConfig)
                        {
                            playerControllerMap = input.controllers.maps.GetMap(playerController, 1, 1);
                        } else if (optionsRoot.state == OptionsGUI.State.ControllerMenuConfig)
                        {
                            playerControllerMap = input.controllers.maps.GetMap(playerController, 2, 1);
                        } else
                        {
                            playerControllerMap = null;
                        }
                    }
                }
            } else
            {
                playerController = ReInput.controllers.GetController(ControllerType.Keyboard, 0);
                playerControllerMap = null;
            }
            this.pollingElement = -1;
            this.playerLeft = false;
        }
    }

    private void OnDisable()
    {
        if (OnOptionsUpDownEvent != null)
        {
            OnOptionsUpDownEvent -= optionsRoot.OnPressOptionsUpDown;
            OnOptionsAcceptEvent -= optionsRoot.OnPressOptionsAccept;
            OnOptionsCancelEvent -= optionsRoot.OnPressOptionsCancel;
            OnOptionsConfigEvent -= optionsRoot.OnPressOptionsConfig;
            OnOptionsConfigIdEvent -= optionsRoot.OnPressOptionsConfigId;
        }
        PlayerManager.OnPlayerDisconnectedEvent -= this.OnPlayerLeft;
        this.pollingElement = -1;
    }

    void Start()
    {
        PlayerManager.SetPlayerMustJoin(player, true, false);
    }

    void Update()
    {
        //this.Busy = false;
        this.CheckUpdate = true;
        if (InterruptingPrompt.IsInterrupting() || MainMenuScene.Current.state == MainMenuScene.State.OptionsBusy)
        {
            return;
        }
        if (this.playerLeft)
        {
            this.playerLeft = false;
            this.OnOptionsCancelEvent();
            return;
        }
        switch (this.state)
        {
            case OptionsGUIPlayer.State.Selecting:
                if (optionsRoot.state != OptionsGUI.State.UserControllerConfig && optionsRoot.state != OptionsGUI.State.ControllerMenuConfig)
                {
                    return;
                }
                if (optionsRoot.state == OptionsGUI.State.UserControllerConfig)
                {
                    //MirrorOfDuskButton testButton = MirrorOfDuskButton.None;
                    string testButton = "--";
                    int elemId = -1;
                    switch (this.optionsRoot.VerticalSelection)
                    {
                        case 0:
                            if (this.GetButtonDown(MirrorOfDuskButton.Cancel) || this.GetButtonDown(MirrorOfDuskButton.Pause))
                            {
                                this.OnOptionsCancelEvent();
                                return;
                            }
                            if (this.GetButtonDown(MirrorOfDuskButton.Accept))
                            {
                                this.OnOptionsAcceptEvent(this.optionsRoot.VerticalSelection);
                                return;
                            }
                            break;
                        case 1:
                            if (this.GetButtonDown(MirrorOfDuskButton.Pause))
                            {
                                this.OnOptionsCancelEvent();
                                return;
                            }
                            if (this.GetButtonDown(MirrorOfDuskButton.Cancel))
                            {
                                return;
                            }
                            if (this.GetButtonDown(MirrorOfDuskButton.Accept))
                            {
                                this.OnOptionsAcceptEvent(this.optionsRoot.VerticalSelection);
                                return;
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
                            if (this.GetButtonDown(MirrorOfDuskButton.Pause))
                            {
                                this.OnOptionsCancelEvent();
                                return;
                            }
                            if (SettingsData.Data.currentUserConfigProfile != 0)
                            {
                                /*if (GetAndReturnJoystickActionButtonDown(ref testButton))
                                {
                                    if (testButton != MirrorOfDuskButton.None)
                                    {
                                        this.OnOptionsConfigEvent(testButton);
                                        return;
                                    }
                                }*/
                                if (FindJoystickElementDownUser(ref testButton, out elemId))
                                {
                                    MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                                    base.FrameDelayedCallback(new Action(this.ClosingDelay), 12);
                                    this.OnOptionsConfigIdEvent(testButton, elemId);
                                    return;
                                }
                                if (FindKeyboardElementDownUser(ref testButton, out elemId))
                                {
                                    MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                                    base.FrameDelayedCallback(new Action(this.ClosingDelay), 12);
                                    this.OnOptionsConfigIdEvent(testButton, elemId);
                                    return;
                                }
                                /*if (GetAndReturnKeyboardActionButtonDown(ref testButton))
                                {
                                    if (testButton != MirrorOfDuskButton.None)
                                    {
                                        this.OnOptionsConfigEvent(testButton);
                                        return;
                                    }
                                }*/
                            }
                            if (this.GetButtonDown(MirrorOfDuskButton.Cancel))
                            {
                                return;
                            }
                            break;
                    }
                    bool opDown = this.input.GetAxis((int)MirrorOfDuskButton.CursorVertical) < 0f;
                    bool opUp = this.input.GetAxis((int)MirrorOfDuskButton.CursorVertical) > 0f;
                    bool opRight = this.input.GetAxis((int)MirrorOfDuskButton.CursorHorizontal) > 0f;
                    this.OnOptionsUpDownEvent(opDown, opUp, opRight);
                }
                if (optionsRoot.state == OptionsGUI.State.ControllerMenuConfig)
                {
                    string testButton = "--";
                    int elemId = -1;
                    switch (this.optionsRoot.VerticalSelection)
                    {
                        case 0:
                            if (this.GetButtonDown(MirrorOfDuskButton.Cancel) || this.GetButtonDown(MirrorOfDuskButton.Pause))
                            {
                                this.OnOptionsCancelEvent();
                                return;
                            }
                            if (this.GetButtonDown(MirrorOfDuskButton.Accept))
                            {
                                this.OnOptionsAcceptEvent(this.optionsRoot.VerticalSelection);
                                return;
                            }
                            break;
                        case 1:
                            if (this.GetButtonDown(MirrorOfDuskButton.Pause))
                            {
                                this.OnOptionsCancelEvent();
                                return;
                            }
                            if (this.GetButtonDown(MirrorOfDuskButton.Cancel))
                            {
                                return;
                            }
                            if (this.GetButtonDown(MirrorOfDuskButton.Accept))
                            {
                                this.OnOptionsAcceptEvent(this.optionsRoot.VerticalSelection);
                                return;
                            }
                            break;
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                            if (this.GetButtonDown(MirrorOfDuskButton.Pause))
                            {
                                this.OnOptionsCancelEvent();
                                return;
                            }
                            if (FindJoystickElementDown(ref testButton, out elemId))
                            {
                                MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                                base.FrameDelayedCallback(new Action(this.ClosingDelay), 12);
                                this.OnOptionsConfigIdEvent(testButton, elemId);
                                return;
                            }
                            break;
                    }
                    bool opDown = this.input.GetAxis((int)MirrorOfDuskButton.CursorVertical) < 0f;
                    bool opUp = this.input.GetAxis((int)MirrorOfDuskButton.CursorVertical) > 0f;
                    bool opRight = this.input.GetAxis((int)MirrorOfDuskButton.CursorHorizontal) > 0f;
                    this.OnOptionsUpDownEvent(opDown, opUp, opRight);
                }
                break;
        }
    }

    private void OnDestroy()
    {
        //OnMenuUpDownEvent -= MainMenuScene.Current.OnPressMenuDown;
    }

    public void OnStart()
    {
        if (!base.gameObject.activeInHierarchy)
        {
            return;
        }
        this.input = PlayerManager.GetPlayerInput(this.player);
        this.state = OptionsGUIPlayer.State.Selecting;
        this.enabled = false;
    }

    private void OnPlayerLeft(PlayerId playerId)
    {
        if (playerId == this.player)
        {
            if (optionsRoot.state == OptionsGUI.State.UserControllerConfig && !input.controllers.hasKeyboard)
            {
                this.playerLeft = true;
            }
            if (optionsRoot.state == OptionsGUI.State.ControllerMenuConfig)
            {
                this.playerLeft = true;
            }
        }
    }

    protected bool GetButtonDown(MirrorOfDuskButton button)
    {
        if (this.input.GetButtonDown((int)button))
        {
            return true;
        }
        return false;
    }

    protected bool GetButton(MirrorOfDuskButton button)
    {
        if (this.input.GetButton((int)button))
        {
            return true;
        }
        return false;
    }

    private bool GetAndReturnJoystickActionButtonDown(ref MirrorOfDuskButton testButton)
    {
        if (FindJoystickActionButtonDown(ref testButton))
        {
            AudioManager.Play("select2");
            return true;
        }
        return false;
    }

    private bool FindJoystickActionButtonDown(ref MirrorOfDuskButton button)
    {
        if (InterruptingPrompt.IsInterrupting())
        {
            return false;
        }
        if (input.controllers.joystickCount > 0)
        {
            foreach (Controller cont in input.controllers.Controllers)
            {
                if (cont.type == ControllerType.Joystick && cont.isConnected)
                {
                    if (optionsRoot.state == OptionsGUI.State.UserControllerConfig)
                    {
                        Controller currentJoystick = input.controllers.GetController(ControllerType.Joystick, cont.id);
                        ControllerMap cmp = input.controllers.maps.GetMap(currentJoystick, 1, 0);
                        ControllerPollingInfo pollingInfo = ReInput.controllers.polling.PollControllerForFirstElementDown(ControllerType.Joystick, currentJoystick.id);
                        if (cmp.player.GetButtonDown("LightAttack"))
                        {
                            button = MirrorOfDuskButton.LightAttack;
                            return true;
                        }
                        else if (cmp.player.GetButtonDown("HeavyAttack"))
                        {
                            button = MirrorOfDuskButton.HeavyAttack;
                            return true;
                        }
                        else if (cmp.player.GetButtonDown("SpecialAttack"))
                        {
                            button = MirrorOfDuskButton.SpecialAttack;
                            return true;
                        }
                        else if (cmp.player.GetButtonDown("Jump"))
                        {
                            button = MirrorOfDuskButton.Jump;
                            return true;
                        }
                        else if (cmp.player.GetButtonDown("Block"))
                        {
                            button = MirrorOfDuskButton.Block;
                            return true;
                        }
                        else if (cmp.player.GetButtonDown("Dodge"))
                        {
                            button = MirrorOfDuskButton.Dodge;
                            return true;
                        }
                        else if (cmp.player.GetButtonDown("Grab"))
                        {
                            button = MirrorOfDuskButton.Grab;
                            return true;
                        }
                        else if (cmp.player.GetButtonDown("UseMirror"))
                        {
                            button = MirrorOfDuskButton.UseMirror;
                            return true;
                        }
                        /*if (player.controllers.GetController(ControllerType.Joystick, cont.id).GetButtonDown(5))
                        {
                            button = MirrorOfDuskButton.Jump;
                            return true;
                        }*/
                        /*Controller currentJoystick = player.controllers.GetController(ControllerType.Joystick, cont.id);
                        ControllerPollingInfo pollingInfo = ReInput.controllers.polling.PollControllerForFirstElementDown(ControllerType.Joystick, currentJoystick.id);
                        if (pollingInfo.elementIndex != -1)
                        {
                            Debug.Log(pollingInfo.elementIndex + " " + pollingInfo.elementType);
                            Debug.Log(pollingInfo.elementIdentifier.id + " " + pollingInfo.elementIdentifier.name);
                            Debug.Log(pollingInfo.elementIdentifierName + " " + pollingInfo.elementIdentifierId);
                            ControllerMap cmp = player.controllers.maps.GetMap(currentJoystick, 1, 0);
                            //ControllerElementTarget cet = new ControllerElementTarget(pollingInfo.elementIdentifier);
                            foreach (ActionElementMap am in cmp.AllMaps)
                            {
                                Debug.Log(am.id);
                                Debug.Log(am.elementIdentifierName);
                                Debug.Log(am.elementIdentifierId);
                            }
                            Debug.Log(cmp.GetElementMap(201));
                            Debug.Log("FirstElMap: " + cmp.GetFirstElementMapWithAction(3, true));


                            //Debug.Log(cmp.GetElementMapsWithAction(pollingInfo.));
                            //Debug.Log(cmp.GetFirstElementMapWithElementTarget(pollingInfo.);
                            if (pollingInfo.elementIndex == 0)
                            {

                            }
                            //ElementAssignment _e_a = new ElementAssignment(currentJoystick.type, pollingInfo.elementType, pollingInfo.elementIdentifierId, AxisRange.Positive, pollingInfo.keyboardKey, ModifierKeyFlags.None, this.fieldInfo.actionId, (this.fieldInfo.axisRange != 2) ? 0 : 1, false, (this.aem == null) ? -1 : this.aem.id);
                            return true;
                        }*/
                        /*if ((player.GetButtonDown(13) || player.GetButtonDown(14) || player.GetButtonDown(7) || player.GetButtonDown(15) || player.GetButtonDown(2) || player.GetButtonDown(6) || player.GetButtonDown(8) || player.GetButtonDown(3) || player.GetButtonDown(4) || player.GetButtonDown(5)) && (!this.checkIfKOed || !this.IsKOed(player)))
                        {
                            return true;
                        }*/
                    } else
                    {
                        Controller currentJoystick = input.controllers.GetController(ControllerType.Joystick, cont.id);
                        ControllerMap cmp = input.controllers.maps.GetMap(currentJoystick, 2, 1);
                        ControllerPollingInfo pollingInfo = ReInput.controllers.polling.PollControllerForFirstElementDown(ControllerType.Joystick, currentJoystick.id);
                        if (cmp.player.GetButtonDown("Accept"))
                        {
                            button = MirrorOfDuskButton.Accept;
                            return true;
                        }
                        else if (cmp.player.GetButtonDown("Cancel"))
                        {
                            button = MirrorOfDuskButton.Cancel;
                            return true;
                        }
                        else if (cmp.player.GetButtonDown("Edit"))
                        {
                            button = MirrorOfDuskButton.Edit;
                            return true;
                        }
                        else if (cmp.player.GetButtonDown("ScrollLeft"))
                        {
                            button = MirrorOfDuskButton.ScrollLeft;
                            return true;
                        }
                        else if (cmp.player.GetButtonDown("ScrollRight"))
                        {
                            button = MirrorOfDuskButton.ScrollRight;
                            return true;
                        }
                        /*if (player.controllers.GetController(ControllerType.Joystick, cont.id).GetButtonDown(5))
                        {
                            button = MirrorOfDuskButton.Jump;
                            return true;
                        }*/
                        /*Controller currentJoystick = player.controllers.GetController(ControllerType.Joystick, cont.id);
                        ControllerPollingInfo pollingInfo = ReInput.controllers.polling.PollControllerForFirstElementDown(ControllerType.Joystick, currentJoystick.id);
                        if (pollingInfo.elementIndex != -1)
                        {
                            Debug.Log(pollingInfo.elementIndex + " " + pollingInfo.elementType);
                            Debug.Log(pollingInfo.elementIdentifier.id + " " + pollingInfo.elementIdentifier.name);
                            Debug.Log(pollingInfo.elementIdentifierName + " " + pollingInfo.elementIdentifierId);
                            ControllerMap cmp = player.controllers.maps.GetMap(currentJoystick, 1, 0);
                            //ControllerElementTarget cet = new ControllerElementTarget(pollingInfo.elementIdentifier);
                            foreach (ActionElementMap am in cmp.AllMaps)
                            {
                                Debug.Log(am.id);
                                Debug.Log(am.elementIdentifierName);
                                Debug.Log(am.elementIdentifierId);
                            }
                            Debug.Log(cmp.GetElementMap(201));
                            Debug.Log("FirstElMap: " + cmp.GetFirstElementMapWithAction(3, true));


                            //Debug.Log(cmp.GetElementMapsWithAction(pollingInfo.));
                            //Debug.Log(cmp.GetFirstElementMapWithElementTarget(pollingInfo.);
                            if (pollingInfo.elementIndex == 0)
                            {

                            }
                            //ElementAssignment _e_a = new ElementAssignment(currentJoystick.type, pollingInfo.elementType, pollingInfo.elementIdentifierId, AxisRange.Positive, pollingInfo.keyboardKey, ModifierKeyFlags.None, this.fieldInfo.actionId, (this.fieldInfo.axisRange != 2) ? 0 : 1, false, (this.aem == null) ? -1 : this.aem.id);
                            return true;
                        }*/
                        /*if ((player.GetButtonDown(13) || player.GetButtonDown(14) || player.GetButtonDown(7) || player.GetButtonDown(15) || player.GetButtonDown(2) || player.GetButtonDown(6) || player.GetButtonDown(8) || player.GetButtonDown(3) || player.GetButtonDown(4) || player.GetButtonDown(5)) && (!this.checkIfKOed || !this.IsKOed(player)))
                        {
                            return true;
                        }*/
                    }

                }
            }
        }
        return false;
    }

    private bool FindJoystickElementDown(ref string button, out int elemId)
    {
        int elementId = -1;
        if (InterruptingPrompt.IsInterrupting())
        {
            elemId = elementId;
            return false;
        }
        if (input.controllers.joystickCount > 0)
        {
            foreach (Controller cont in input.controllers.Controllers)
            {
                if (cont.type == ControllerType.Joystick && cont.isConnected)
                {
                    Controller currentJoystick = input.controllers.GetController(ControllerType.Joystick, cont.id);
                    ControllerMap cmp = input.controllers.maps.GetMap(currentJoystick, 2, 1);
                    ControllerPollingInfo pollingInfo = ReInput.controllers.polling.PollControllerForFirstElementDown(ControllerType.Joystick, currentJoystick.id);
                    if (pollingInfo.elementIndex == -1)
                    {
                        this.pollingElement = -1;
                        //UnityEngine.Debug.Log("Element:" + pollingInfo.elementIdentifierId + " " + pollingInfo.elementIdentifierName);
                    }
                    if (this.pollingElement != pollingInfo.elementIndex && pollingInfo.elementIndex != -1 && (pollingInfo.elementType == ControllerElementType.Button || (pollingInfo.elementType == ControllerElementType.Axis && ((pollingInfo.elementIdentifierName[0] == 'R' || pollingInfo.elementIdentifierName[0] == 'L') && pollingInfo.elementIdentifierName.Length == 2))) && !pollingInfo.elementIdentifierName.Contains("Hat") && !pollingInfo.elementIdentifierName.Contains("D-Pad"))
                    {
                        button = pollingInfo.elementIdentifierName;
                        elemId = pollingInfo.elementIdentifierId;
                        this.pollingElement = elemId;
                        return true;
                    }
                    /*if (player.controllers.GetController(ControllerType.Joystick, cont.id).GetButtonDown(5))
                    {
                        button = MirrorOfDuskButton.Jump;
                        return true;
                    }*/
                    /*Controller currentJoystick = player.controllers.GetController(ControllerType.Joystick, cont.id);
                    ControllerPollingInfo pollingInfo = ReInput.controllers.polling.PollControllerForFirstElementDown(ControllerType.Joystick, currentJoystick.id);
                    if (pollingInfo.elementIndex != -1)
                    {
                        Debug.Log(pollingInfo.elementIndex + " " + pollingInfo.elementType);
                        Debug.Log(pollingInfo.elementIdentifier.id + " " + pollingInfo.elementIdentifier.name);
                        Debug.Log(pollingInfo.elementIdentifierName + " " + pollingInfo.elementIdentifierId);
                        ControllerMap cmp = player.controllers.maps.GetMap(currentJoystick, 1, 0);
                        //ControllerElementTarget cet = new ControllerElementTarget(pollingInfo.elementIdentifier);
                        foreach (ActionElementMap am in cmp.AllMaps)
                        {
                            Debug.Log(am.id);
                            Debug.Log(am.elementIdentifierName);
                            Debug.Log(am.elementIdentifierId);
                        }
                        Debug.Log(cmp.GetElementMap(201));
                        Debug.Log("FirstElMap: " + cmp.GetFirstElementMapWithAction(3, true));


                        //Debug.Log(cmp.GetElementMapsWithAction(pollingInfo.));
                        //Debug.Log(cmp.GetFirstElementMapWithElementTarget(pollingInfo.);
                        if (pollingInfo.elementIndex == 0)
                        {

                        }
                        //ElementAssignment _e_a = new ElementAssignment(currentJoystick.type, pollingInfo.elementType, pollingInfo.elementIdentifierId, AxisRange.Positive, pollingInfo.keyboardKey, ModifierKeyFlags.None, this.fieldInfo.actionId, (this.fieldInfo.axisRange != 2) ? 0 : 1, false, (this.aem == null) ? -1 : this.aem.id);
                        return true;
                    }*/
                    /*if ((player.GetButtonDown(13) || player.GetButtonDown(14) || player.GetButtonDown(7) || player.GetButtonDown(15) || player.GetButtonDown(2) || player.GetButtonDown(6) || player.GetButtonDown(8) || player.GetButtonDown(3) || player.GetButtonDown(4) || player.GetButtonDown(5)) && (!this.checkIfKOed || !this.IsKOed(player)))
                    {
                        return true;
                    }*/
                }
            }
        }
        elemId = elementId;
        return false;
    }

    private bool FindJoystickElementDownUser(ref string button, out int elemId)
    {
        int elementId = -1;
        if (InterruptingPrompt.IsInterrupting())
        {
            elemId = elementId;
            return false;
        }
        if (playerController != null && playerControllerMap != null)
        {
            ControllerPollingInfo pollingInfo = ReInput.controllers.polling.PollControllerForFirstElementDown(ControllerType.Joystick, playerController.id);
            if (pollingInfo.elementIndex == -1)
            {
                this.pollingElement = -1;
                //UnityEngine.Debug.Log("Element:" + pollingInfo.elementIdentifierId + " " + pollingInfo.elementIdentifierName);
            }
            if (this.pollingElement != pollingInfo.elementIndex && pollingInfo.elementIndex != -1 && (pollingInfo.elementType == ControllerElementType.Button || (pollingInfo.elementType == ControllerElementType.Axis && ((pollingInfo.elementIdentifierName[0] == 'R' || pollingInfo.elementIdentifierName[0] == 'L') && pollingInfo.elementIdentifierName.Length == 2))) && !pollingInfo.elementIdentifierName.Contains("Hat") && !pollingInfo.elementIdentifierName.Contains("D-Pad"))
            {
                button = UserConfigData.GetUserConfigActionElementName((playerController as Joystick), pollingInfo.elementIdentifierId);
                elemId = pollingInfo.elementIdentifierId;
                this.pollingElement = elemId;
                return true;
            }
        }
        elemId = elementId;
        return false;
    }

    private bool FindKeyboardElementDownUser(ref string button, out int elemId)
    {
        int elementId = -1;
        if (InterruptingPrompt.IsInterrupting())
        {
            elemId = elementId;
            return false;
        }
        if (playerController != null && playerControllerMap != null)
        {
            ControllerPollingInfo pollingInfo = ReInput.controllers.polling.PollControllerForFirstElementDown(ControllerType.Joystick, playerController.id);
            if (pollingInfo.elementIndex == -1)
            {
                this.pollingElement = -1;
                //UnityEngine.Debug.Log("Element:" + pollingInfo.elementIdentifierId + " " + pollingInfo.elementIdentifierName);
            }
            if (this.pollingElement != pollingInfo.elementIndex && pollingInfo.elementIndex != -1 && (pollingInfo.elementType == ControllerElementType.Button || (pollingInfo.elementType == ControllerElementType.Axis && ((pollingInfo.elementIdentifierName[0] == 'R' || pollingInfo.elementIdentifierName[0] == 'L') && pollingInfo.elementIdentifierName.Length == 2))) && !pollingInfo.elementIdentifierName.Contains("Hat") && !pollingInfo.elementIdentifierName.Contains("D-Pad"))
            {
                button = UserConfigData.GetUserConfigActionElementName((playerController as Joystick), pollingInfo.elementIdentifierId);
                elemId = pollingInfo.elementIdentifierId;
                this.pollingElement = elemId;
                return true;
            }
        }
        elemId = elementId;
        return false;
    }

    private bool GetAndReturnKeyboardActionButtonDown(ref MirrorOfDuskButton testButton)
    {
        if (FindKeyboardActionButtonDown(ref testButton))
        {
            AudioManager.Play("select2");
            return true;
        }
        return false;
    }

    private bool FindKeyboardActionButtonDown(ref MirrorOfDuskButton button)
    {
        if (InterruptingPrompt.IsInterrupting())
        {
            return false;
        }
        if (this.input.controllers.hasKeyboard)
        {
            foreach (Controller cont in this.input.controllers.Controllers)
            {
                if (cont.type == ControllerType.Keyboard && cont.isConnected)
                {
                    Controller currentKeyboard = this.input.controllers.GetController(ControllerType.Keyboard, cont.id);
                    ControllerMap cmp = this.input.controllers.maps.GetMap(currentKeyboard, 0, ((player == PlayerId.PlayerOne) ? 1 : 2));
                    ControllerPollingInfo pollingInfo = ReInput.controllers.polling.PollControllerForFirstElementDown(ControllerType.Keyboard, currentKeyboard.id);
                    if (cmp.player.GetButtonDown("LightAttack"))
                    {
                        button = MirrorOfDuskButton.LightAttack;
                        return true;
                    }
                    else if (cmp.player.GetButtonDown("HeavyAttack"))
                    {
                        button = MirrorOfDuskButton.HeavyAttack;
                        return true;
                    }
                    else if (cmp.player.GetButtonDown("SpecialAttack"))
                    {
                        button = MirrorOfDuskButton.SpecialAttack;
                        return true;
                    }
                    else if (cmp.player.GetButtonDown("Jump"))
                    {
                        button = MirrorOfDuskButton.Jump;
                        return true;
                    }
                    else if (cmp.player.GetButtonDown("Block"))
                    {
                        button = MirrorOfDuskButton.Block;
                        return true;
                    }
                    else if (cmp.player.GetButtonDown("Dodge"))
                    {
                        button = MirrorOfDuskButton.Dodge;
                        return true;
                    }
                    else if (cmp.player.GetButtonDown("Grab"))
                    {
                        button = MirrorOfDuskButton.Grab;
                        return true;
                    }
                    else if (cmp.player.GetButtonDown("UseMirror"))
                    {
                        button = MirrorOfDuskButton.UseMirror;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void ClosingDelay()
    {
        MainMenuScene.Current.state = MainMenuScene.State.OptionsSelecting;
    }
}
