using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class CharacterSelectPlayer : AbstractMB
{
    [SerializeField] private PlayerId player;
    private Player input;

    public CharacterSelectPlayer.State state;

    public delegate void PressDelegate(CharacterSelectPlayer player, int num);

    public event CharacterSelectPlayer.PressDelegate OnCSAcceptEvent;

    public enum State
    {
        Init,
        Disabled,
        Enabling,
        Selecting,
        Options,
        Disabling,
        Exiting,
        Exited
    }

    public PlayerId playerId
    {
        get { return player; }
    }

    public int Id
    {
        get { return (int)player; }
    }

    public Player Input
    {
        get { return this.input; }
    }

    protected override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerManager.SetPlayerCanJoin(player, true, true);
        PlayerManager.SetPlayerWaitForJoinRequest(player);
        OnCSAcceptEvent += CharacterSelectScene.Current.OnPressCSAccept;
    }

    // Update is called once per frame
    void Update()
    {
        if (InterruptingPrompt.IsInterrupting() || this.input == null)
        {
            return;
        }
        switch (this.state)
        {
            case CharacterSelectPlayer.State.Init:
                if (CharacterSelectScene.Current.state == CharacterSelectScene.State.Opening)
                {
                    if (this.input.GetButtonDown((int)MirrorOfDuskButton.Accept))
                    {
                        this.OnCSAcceptEvent(this, (int)MirrorOfDuskButton.Accept);
                        return;
                    }
                }
                break;
            case CharacterSelectPlayer.State.Disabled:
                if (CharacterSelectScene.Current.state == CharacterSelectScene.State.Selecting)
                {
                    if (this.input.GetButtonDown((int)MirrorOfDuskButton.Accept) || this.input.GetButtonDown((int)MirrorOfDuskButton.Pause))
                    {
                        if (CharacterSelectScene.Current.playerModes[(int)this.player] == CharacterSelectScene.PlayerMode.CPU)
                        {
                            CharacterSelectScene.Current.playerModes[(int)this.player] = CharacterSelectScene.PlayerMode.Player;
                            CharacterSelectScene.Current.SetJoiningCursorFromCPU(this.player);
                        } else
                        {
                            CharacterSelectScene.Current.playerModes[(int)this.player] = CharacterSelectScene.PlayerMode.Player;
                            CharacterSelectScene.Current.SetJoiningCursor(this.player, true);
                        }
                        this.state = CharacterSelectPlayer.State.Enabling;
                        base.FrameDelayedCallback(this.SelectingState, 25);
                        return;
                    }
                }
                break;
            case CharacterSelectPlayer.State.Selecting:
                if (CharacterSelectScene.Current.state == CharacterSelectScene.State.Selecting)
                {
                    if (CharacterSelectScene.Current.playerModes[(int)this.player] != CharacterSelectScene.PlayerMode.Offline && CharacterSelectScene.Current.playerCharacterShards[(int)this.player].enabled && CharacterSelectScene.Current.playerGUIs[(int)this.player].actionState == CharacterSelectPlayerGUI.ActionState.Free)
                    {
                        if (this.input.GetButtonDown((int)MirrorOfDuskButton.ScrollRight))
                        {
                            CharacterSelectScene.Current.playerCharacterShards[(int)this.player].ShiftCharacterColor(1);
                            return;
                        }
                        if (this.input.GetButtonDown((int)MirrorOfDuskButton.ScrollLeft))
                        {
                            CharacterSelectScene.Current.playerCharacterShards[(int)this.player].ShiftCharacterColor(-1);
                            return;
                        }
                    }
                }
                break;
        }
    }

    public void OnStart()
    {
        if (!base.gameObject.activeInHierarchy)
        {
            return;
        }
        this.input = PlayerManager.GetPlayerInput(this.player);

        //this.state = CharacterSelectPlayer.State.Enabling;
    }

    public void BeginDisableState(int amount)
    {
        this.state = CharacterSelectPlayer.State.Disabling;
        base.FrameDelayedCallback(this.DisableState, amount);
    }

    private void DisableState()
    {
        this.state = CharacterSelectPlayer.State.Disabled;
    }

    public void SelectingState()
    {
        this.state = CharacterSelectPlayer.State.Selecting;
    }
}
