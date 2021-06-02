using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Rewired;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Rewired.UI.ControlMapper;

public class MainMenuPlayer : AbstractMB
{
    private const float START_DELAY = 1f;
    [SerializeField] private PlayerId player;
    private Player input;
    private bool _checkUpdate = false;

    public bool CheckUpdate
    {
        get { return _checkUpdate; }
        set { _checkUpdate = value; }
    }
    private bool _directionBusy = false;
    private bool _commandBusy = false;

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

    public MainMenuPlayer.State state;
    private bool playerLeft;

    public delegate void PressDelegate(MainMenuPlayer player, int num);

    public event MainMenuPlayer.PressDelegate OnMenuUpDownEvent;
    public event MainMenuPlayer.PressDelegate OnMenuAcceptEvent;
    public event MainMenuPlayer.PressDelegate OnMenuCancelEvent;
    public event MainMenuPlayer.PressDelegate OnOptionsConfigEvent;

    public enum State
    {
        Init,
        Selecting,
        Options,
        Viewing,
        Exiting,
        Exited
    }

    protected override void Awake()
    {
        base.Awake();

    }
    
    void Start()
    {
        PlayerManager.SetPlayerMustJoin(player, true, false);
        OnMenuUpDownEvent += MainMenuScene.Current.OnPressMenuUpDown;
        OnMenuAcceptEvent += MainMenuScene.Current.OnPressMenuAccept;
        OnMenuCancelEvent += MainMenuScene.Current.OnPressMenuCancel;
        OnOptionsConfigEvent += MainMenuScene.Current.OnPressOptionsConfig;
        PlayerManager.OnPlayerLeaveEvent += this.OnPlayerLeft;
    }
    
    void Update()
    {
        //this.Busy = false;
        this.CheckUpdate = true;
        if (InterruptingPrompt.IsInterrupting())
        {
            return;
        }
        switch (this.state)
        {
            case MainMenuPlayer.State.Selecting:
                if (MainMenuScene.Current.Items.Count > 0 && MainMenuScene.Current.CurrentItem.state != MainMenuItem.State.Ready)
                {
                    return;
                }
                if (MainMenuScene.Current.Items.Count > 0 && this.input.GetButton((int)MirrorOfDuskButton.Accept))
                {
                    InputAction action = ReInput.mapping.GetAction("Accept");
                    /*ControllerPollingInfo pollingInfo;
                    pollingInfo = ReInput.controllers.polling.PollControllerForFirstButtonDown(ControllerType.Joystick, PlayerManager.GetPlayerJoystick(this.player).id);
                    UnityEngine.Debug.Log("ElementIdentifierId:" + pollingInfo.elementIdentifierId
                        + "ElementType:" + pollingInfo.elementType
                        + "KeyboardKey:" + pollingInfo.keyboardKey
                        );*/
                    this.OnMenuAcceptEvent(this, (int)MirrorOfDuskButton.Accept);
                }
                if (MainMenuScene.Current.Items.Count > 0 && this.input.GetButton((int)MirrorOfDuskButton.Cancel))
                {
                    this.OnMenuCancelEvent(this, (int)MirrorOfDuskButton.Cancel);
                }
                if (MainMenuScene.Current.Items.Count > 0 && (this.input.GetAxis((int)MirrorOfDuskButton.CursorVertical) < 0f))
                {
                    this.OnMenuUpDownEvent(this, 1);
                    return;
                }
                else if (MainMenuScene.Current.Items.Count > 0 && (this.input.GetAxis((int)MirrorOfDuskButton.CursorVertical) > 0f))
                {
                    this.OnMenuUpDownEvent(this, -1);
                    return;
                }
                break;
            case MainMenuPlayer.State.Options:

                break;
        }
    }

    private void OnDestroy()
    {
        //OnMenuUpDownEvent -= MainMenuScene.Current.OnPressMenuDown;
        PlayerManager.OnPlayerLeaveEvent -= this.OnPlayerLeft;
    }

    public void OnStart()
    {
        if (!base.gameObject.activeInHierarchy)
        {
            return;
        }
        this.input = PlayerManager.GetPlayerInput(this.player);
        if (DEBUG_AssetLoaderManager.debugWasFound)
        {
            /*ControlMapper controlMapper = MirrorOfDusk.Current.controlMapper;
            for (int i = 0; i < controlMapper.GetMappingSet.Length; i++)
            {
                for (int j = 0; j < controlMapper.GetMappingSet[i].actionIds.Count; j++)
                {
                    UnityEngine.Debug.Log(controlMapper.GetMappingSet[i].actionIds[j]);
                }
            }*/
            /*if (input.controllers.joystickCount > 0)
            {
                IEnumerable<ControllerMap> km = input.controllers.maps.GetAllMaps();

                foreach(ControllerMap cm in km)
                {
                    UnityEngine.Debug.Log("ControllerMapCategoryId:" + cm.categoryId
                        + " Controller:" + cm.controller
                        + " LayoutId:" + cm.layoutId
                        + " SourceMapId:" + cm.sourceMapId
                        + " Id:" + cm.id
                        );
                }
                    
                //ControllerMap testMap = input.controllers.maps.GetMap(ControllerType.Joystick, 1, "Gameplay", "Customized");
                ControllerMap testMap2 = input.controllers.maps.GetMap(PlayerManager.GetPlayerJoystick(this.player), "Gameplay", "Customized");
                ControllerMap testMap3 = ReInput.mapping.GetControllerMap(3);
                for (int k = 0; k < testMap2.GetElementMaps().Length; k++)
                {
                    UnityEngine.Debug.Log("ActionId:" + testMap2.GetElementMaps()[k].actionId
                        + " ActionDesc:" + testMap2.GetElementMaps()[k].actionDescriptiveName
                        + " ElementIndex:" + testMap2.GetElementMaps()[k].elementIndex
                        + " ElementType:" + testMap2.GetElementMaps()[k].elementType
                        + " Id:" + testMap2.GetElementMaps()[k].id
                        );
                }
                UnityEngine.Debug.Log(PlayerManager.GetPlayerJoystick(this.player).hardwareTypeGuid);
            }*/
        }
        this.state = MainMenuPlayer.State.Selecting;
    }

    private void OnPlayerLeft(PlayerId playerId)
    {
        if (playerId == this.player)
        {
            this.playerLeft = true;
        }
    }
}
