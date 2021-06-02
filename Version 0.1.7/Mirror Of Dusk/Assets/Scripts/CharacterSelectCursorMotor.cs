using System;
using System.Collections;
using UnityEngine;

public class CharacterSelectCursorMotor : AbstractPausableComponent
{
    public CharacterSelectCursorPlayerController player { get; private set; }
    public CharacterSelectCursorInput input { get; private set; }

    public Vector2 velocity { get; private set; }
    private Vector2 axis;
    private float prevMagnitudeX = 0f;
    private float prevMagnitudeY = 0f;
    private float forceX = 0f;
    private float forceY = 0f;
    private bool inputState = true;

    private float xPos(float pos)
    {
        float result = System.Math.Max(-920f, System.Math.Min(pos, 980f));
        return result;
    }

    private float yPos(float pos)
    {
        float result = System.Math.Max(-550f, System.Math.Min(pos, 500f));
        return result;
    }

    protected override void Awake()
    {
        base.Awake();
        this.player = base.GetComponent<CharacterSelectCursorPlayerController>();
        this.input = base.GetComponent<CharacterSelectCursorInput>();
        prevMagnitudeX = 0f;
        prevMagnitudeY = 0f;
        this.inputState = true;
    }

    private void Update()
    {
        if (PlayerManager.GetPlayerStatus(this.player.id, 4)) {
            return;
        }
        if (this.player.player.state != CharacterSelectPlayer.State.Selecting)
            return;
        if (!CharacterSelectCursorPlayerController.CanMove())
        {
            this.velocity = Vector2.zero;
            this.axis = Vector2.zero;
            //base.rigidBody2D.velocity = Vector2.zero;
            prevMagnitudeX = 0f;
            prevMagnitudeY = 0f;
            forceX = 0f;
            forceY = 0f;
            return;
        }
        this.TallyCollisionPriorities();
        if (this.player.stats.Holding != -1)
        {
            if (this.player.stats.characterSelectedId != -1)
            {
                CharacterSelectScene.Current.OnCharacterSelectEventTrigger(this.player.stats.Holding, this.player.stats.characterSelectedId, false);
            } else
            {
                CharacterSelectScene.Current.OnCharacterSelectEventTrigger(this.player.stats.Holding, -1, false);
            }
        }
        this.HandleInput();
        this.MoveCursor();
        this.player.stats.playerCursorFound = CursorHitboxPriority.None;
        this.player.stats.playerCursorFoundFlags = 0;
        this.player.stats.cStarPickFlags = 0;
        this.player.stats.guiPickFlags = 0;
        this.player.stats.characterSelectedId = -1;
    }

    private void TallyCollisionPriorities()
    {
        if (this.player.stats.playerCursorFoundFlags != 0)
        {
            for (int i = 0; i < this.player.stats.foundFlagsLength; i++)
            {
                if (this.player.stats.playerCursorFoundFlags.HasFlag((CharacterSelectCursorStatsManager.CursorFoundFlags)(1 << i)))
                {
                    this.player.stats.playerCursorFound = (CursorHitboxPriority)(i + 1);
                    break;
                }
            }
        }
    }

    private void HandleInput()
    {
        if (this.inputState)
            this.BufferInputs();
        this.axis = new Vector2((float)this.input.GetAxisIntX(NewPlayerInput.Axis.X, false), (float)this.input.GetAxisIntY(NewPlayerInput.Axis.Y, false));
        //float magnitude = this.axis.magnitude;
        float magnitudeX = (float)Math.Sqrt(this.axis.x * this.axis.x);
        if (magnitudeX < 0.0001f || (prevMagnitudeX >= 0.0001f && magnitudeX < prevMagnitudeX - 0.05f))
        {
            if (magnitudeX < 0.0001f)
            {
                this.axis.x = 0f;
            }
            else
            {
                this.axis.x /= magnitudeX;
            }
            prevMagnitudeX = magnitudeX;
            this.forceX = ((this.forceX <= 0f) ? 0f : --this.forceX);
        }
        else
        {
            this.axis.x /= magnitudeX;
            prevMagnitudeX = magnitudeX;
            this.forceX = ((this.forceX >= 14f) ? 14f : (this.forceX + 0.5f));
        }
        float magnitudeY = (float)Math.Sqrt(this.axis.y * this.axis.y);
        if (magnitudeY < 0.0001f || (prevMagnitudeY >= 0.0001f && magnitudeY < prevMagnitudeY - 0.05f))
        {
            if (magnitudeY < 0.0001f)
            {
                this.axis.y = 0f;
            } else
            {
                this.axis.y /= magnitudeY;
            }
            prevMagnitudeY = magnitudeY;
            this.forceY = ((this.forceY <= 0f) ? 0f : --this.forceY);
        }
        else
        {
            this.axis.y /= magnitudeY;
            prevMagnitudeY = magnitudeY;
            this.forceY = ((this.forceY >= 14f) ? 14f : (this.forceY + 0.5f));
        }
        /*if (magnitude < 0.0001f || (prevMagnitude > 0.5f && this.axis.magnitude < prevMagnitude))
        {
            this.axis = Vector2.zero;
            prevMagnitude = 0f;
        } else
        {
            this.axis /= magnitude;
            prevMagnitude = magnitude;
        }*/
    }

    private void MoveCursor()
    {
        this.velocity = Vector2.Lerp(this.velocity, new Vector2(this.axis.x * forceX, this.axis.y * forceY), 1f);
        //this.GetComponent<Rigidbody2D>().velocity = this.velocity;
        this.gameObject.transform.position = new Vector2(xPos((float)Math.Round(this.gameObject.transform.position.x + this.velocity.x)), yPos((float)Math.Round(this.gameObject.transform.position.y + this.velocity.y)));
    }

    private void BufferInputs()
    {
        if (player.input.actions.GetButtonDown((int)MirrorOfDuskButton.Accept))
        {
            if (player.stats.Holding != -1 && CharacterSelectScene.Current.playerCursorStars[player.stats.Holding].nearStars)
            {
                if (player.stats.playerCursorFound == CursorHitboxPriority.CharacterStar)
                {
                    CharacterSelectScene.Current.playerData[player.stats.Holding].characterSelectedId = this.player.stats.characterSelectedId;
                    CharacterSelectScene.Current.OnCharacterSelectEventTrigger(this.player.stats.Holding, this.player.stats.characterSelectedId, true);
                    /*animateCSShard[currentCursorStar.cursorId].changeState(9);
                    csPlayerData[currentCursorStar.cursorId].selectStar();
                    players[currentCursorStar.cursorId].colorTableData.updateTable();
                    players[currentCursorStar.cursorId].characterSelected = true;
                    holdingCursorStar = false;
                    currentCursorStar.followID = currentCursorStar.cursorId;
                    currentCursorStar = initialCursorStar;
                    playerCursorFound = CursorFound.None;
                    playerCursorFoundFlags = 0;*/
                } else
                {
                    CharacterSelectScene.Current.playerData[player.stats.Holding].characterSelectedId = -1;
                    CharacterSelectScene.Current.OnCharacterSelectEventTrigger(this.player.stats.Holding, -1, true);
                    //players[currentCursorStar.cursorId].characterSelectedRandom = true;
                }
                CharacterSelectScene.Current.ReleaseCursorStar(this.player.stats.Holding);
                AudioManager.Play("confirm1");
                player.stats.Holding = -1;
                this.inputState = false;
                base.FrameDelayedCallback(new Action(this.RestoreInput), 5);
                return;
            } else
            {
                if (player.stats.playerCursorFound == CursorHitboxPriority.CursorStar)
                {
                    if (player.stats.cStarPickFlags.HasFlag((CharacterSelectCursorStatsManager.CStarPickFlags)(1 << (int)player.id)) && CharacterSelectScene.Current.playerGUIs[(int)player.id].actionState == CharacterSelectPlayerGUI.ActionState.Free)
                    {
                        CharacterSelectScene.Current.OnCursorStarSelectEventTrigger((int)player.id, player);
                        this.inputState = false;
                        base.FrameDelayedCallback(new Action(this.RestoreInput), 5);
                        return;
                    }
                    else
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (i != (int)player.id)
                            {
                                if (player.stats.cStarPickFlags.HasFlag((CharacterSelectCursorStatsManager.CStarPickFlags)(1 << i)) && CharacterSelectScene.Current.playerGUIs[i].actionState == CharacterSelectPlayerGUI.ActionState.Free)
                                {
                                    CharacterSelectScene.Current.OnCursorStarSelectEventTrigger(i, player);
                                    this.inputState = false;
                                    base.FrameDelayedCallback(new Action(this.RestoreInput), 5);
                                    return;
                                }
                            }
                        }
                    }
                }
                if (player.stats.playerCursorFound == CursorHitboxPriority.Back) //When selecting to exit the character select screen
                {
                    AudioManager.Play("cancel1");
                    CharacterSelectScene.Current.state = CharacterSelectScene.State.Exiting;
                    this.inputState = false;
                    base.FrameDelayedCallback(new Action(this.RestoreInput), 5);
                    return;
                }
                if (player.stats.playerCursorFound == CursorHitboxPriority.MenuColor) //When selecting to open the character color menu
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (player.stats.guiPickFlags.HasFlag((CharacterSelectCursorStatsManager.GuiPickFlags)(1 << i)))
                        {
                            if (CharacterSelectScene.Current.playerGUIs[i].guiActive && CharacterSelectScene.Current.playerGUIs[i].actionState == CharacterSelectPlayerGUI.ActionState.Free && CharacterSelectScene.Current.playerCursorStars[i].Held == false && CharacterSelectScene.Current.playerModes[i] != CharacterSelectScene.PlayerMode.Offline && CharacterSelectScene.Current.playerData[i].GetCharacterId != 0 && ((CharacterSelectScene.Current.playerModes[i] == CharacterSelectScene.PlayerMode.Player && i == player.player.Id) || (CharacterSelectScene.Current.playerModes[i] == CharacterSelectScene.PlayerMode.CPU)))
                            {
                                CharacterSelectScene.Current.playerGUIs[i].actionState = CharacterSelectPlayerGUI.ActionState.Busy;
                                CharacterSelectScene.Current.playerGUIs[i].OpenColorMenu(player.player, CharacterSelectScene.Current.playerCursorStars[i].characterSelectedId);
                                AudioManager.Play("confirm1");
                                CharacterSelectScene.Current.playerGUIs[i].DelayInput();
                                this.inputState = false;
                                base.FrameDelayedCallback(new Action(this.RestoreInput), 5);
                                return;
                            }
                        }
                    }
                }
                if (player.stats.playerCursorFound == CursorHitboxPriority.MenuHP) //When selecting to open the edit health menu
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (player.stats.guiPickFlags.HasFlag((CharacterSelectCursorStatsManager.GuiPickFlags)(1 << i)))
                        {
                            if (CharacterSelectScene.Current.playerGUIs[i].guiActive && CharacterSelectScene.Current.playerGUIs[i].actionState == CharacterSelectPlayerGUI.ActionState.Free && ((CharacterSelectScene.Current.playerModes[i] == CharacterSelectScene.PlayerMode.Player && i == player.player.Id) || (CharacterSelectScene.Current.playerModes[i] == CharacterSelectScene.PlayerMode.CPU)))
                            {
                                CharacterSelectScene.Current.playerGUIs[i].actionState = CharacterSelectPlayerGUI.ActionState.Busy;
                                CharacterSelectScene.Current.playerGUIs[i].OpenHPMenu(player.player, CharacterSelectScene.Current.playerCursorStars[i].characterSelectedId);
                                AudioManager.Play("confirm1");
                                CharacterSelectScene.Current.playerGUIs[i].DelayInput();
                                this.inputState = false;
                                base.FrameDelayedCallback(new Action(this.RestoreInput), 5);
                                return;
                            }
                        }
                    }
                }
                if (player.stats.playerCursorFound == CursorHitboxPriority.MenuShards) //When selecting to open the edit shard stats menu
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (player.stats.guiPickFlags.HasFlag((CharacterSelectCursorStatsManager.GuiPickFlags)(1 << i)))
                        {
                            if (CharacterSelectScene.Current.playerGUIs[i].guiActive && CharacterSelectScene.Current.playerGUIs[i].actionState == CharacterSelectPlayerGUI.ActionState.Free && ((CharacterSelectScene.Current.playerModes[i] == CharacterSelectScene.PlayerMode.Player && i == player.player.Id) || (CharacterSelectScene.Current.playerModes[i] == CharacterSelectScene.PlayerMode.CPU)))
                            {
                                CharacterSelectScene.Current.playerGUIs[i].actionState = CharacterSelectPlayerGUI.ActionState.Busy;
                                CharacterSelectScene.Current.playerGUIs[i].OpenShardsMenu(player.player, CharacterSelectScene.Current.playerCursorStars[i].characterSelectedId);
                                AudioManager.Play("confirm1");
                                CharacterSelectScene.Current.playerGUIs[i].DelayInput();
                                this.inputState = false;
                                base.FrameDelayedCallback(new Action(this.RestoreInput), 5);
                                return;
                            }
                        }
                    }
                }
                if (player.stats.playerCursorFound == CursorHitboxPriority.GUIPlayer) //When selecting to summon or set a player as playable
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (player.stats.guiPickFlags.HasFlag((CharacterSelectCursorStatsManager.GuiPickFlags)(1 << i)))
                        {
                            if (PlayerManager.GetPlayerStatus((PlayerId)i, 1) && CharacterSelectScene.Current.GetPlayer(i).state == CharacterSelectPlayer.State.Disabled && CharacterSelectScene.Current.playerGUIs[i].guiActive)
                            {
                                if (CharacterSelectScene.Current.playerModes[i] == CharacterSelectScene.PlayerMode.CPU)
                                {
                                    CharacterSelectScene.Current.playerGUIs[i].ChangePlayerIconStateTrigger(player.stats.playerCursorFound);
                                    CharacterSelectScene.Current.playerModes[i] = CharacterSelectScene.PlayerMode.Player;
                                    CharacterSelectScene.Current.SetJoiningCursorFromCPU((PlayerId)i);
                                    CharacterSelectScene.Current.GetPlayer(i).state = CharacterSelectPlayer.State.Enabling;
                                    base.FrameDelayedCallback(new Action(CharacterSelectScene.Current.GetPlayer(i).SelectingState), 25);
                                    CharacterSelectScene.Current.playerGUIs[i].DelayInput();
                                    this.inputState = false;
                                    base.FrameDelayedCallback(new Action(this.RestoreInput), 5);
                                    return;
                                } else if (CharacterSelectScene.Current.playerModes[i] == CharacterSelectScene.PlayerMode.Offline)
                                {
                                    CharacterSelectScene.Current.playerGUIs[i].ChangePlayerIconStateTrigger(player.stats.playerCursorFound);
                                    CharacterSelectScene.Current.playerModes[i] = CharacterSelectScene.PlayerMode.Player;
                                    CharacterSelectScene.Current.SetJoiningCursor((PlayerId)i, true);
                                    CharacterSelectScene.Current.GetPlayer(i).state = CharacterSelectPlayer.State.Enabling;
                                    base.FrameDelayedCallback(new Action(CharacterSelectScene.Current.GetPlayer(i).SelectingState), 25);
                                    CharacterSelectScene.Current.playerGUIs[i].DelayInput();
                                    this.inputState = false;
                                    base.FrameDelayedCallback(new Action(this.RestoreInput), 5);
                                    return;
                                }
                            }
                            if (PlayerManager.GetPlayerStatus((PlayerId)i, 3) && CharacterSelectScene.Current.GetPlayer(i).state == CharacterSelectPlayer.State.Selecting && CharacterSelectScene.Current.playerGUIs[i].guiActive)
                            {
                                if (CharacterSelectScene.Current.playerModes[i] == CharacterSelectScene.PlayerMode.CPU)
                                {
                                    CharacterSelectScene.Current.playerGUIs[i].ChangePlayerIconStateTrigger(player.stats.playerCursorFound);
                                    CharacterSelectScene.Current.playerModes[i] = CharacterSelectScene.PlayerMode.Player;
                                    for (int j = 0; j < CharacterSelectScene.Current.playerCursors.Length; j++)
                                    {
                                        if (j == i)
                                            continue;
                                        if (CharacterSelectScene.Current.playerCursors[j].gameObject.activeSelf)
                                        {
                                            if (CharacterSelectScene.Current.playerCursors[j].stats.Holding == i)
                                            {
                                                CharacterSelectScene.Current.playerCursorStars[i].retreatToStart = true;
                                                CharacterSelectScene.Current.playerCursorStars[i].OnCursorStarRelease();
                                                CharacterSelectScene.Current.playerCursors[j].stats.Holding = -1;
                                                CharacterSelectScene.Current.playerGUIs[i].DefaultCharacterSelect(i, -1);
                                                break;
                                            }
                                        }
                                    }
                                    CharacterSelectScene.Current.playerCursors[i].ChangeModeColor();
                                    CharacterSelectScene.Current.playerCursorStars[i].ChangeModeColor();
                                    CharacterSelectScene.Current.playerGUIs[i].DelayInput();
                                    this.inputState = false;
                                    base.FrameDelayedCallback(new Action(this.RestoreInput), 5);
                                    return;
                                }
                            }
                        }
                    }
                }
                if (player.stats.playerCursorFound == CursorHitboxPriority.GUICPU) //When selecting to summon or set a player as a CPU
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (player.stats.guiPickFlags.HasFlag((CharacterSelectCursorStatsManager.GuiPickFlags)(1 << i)))
                        {
                            if (PlayerManager.GetPlayerStatus((PlayerId)i, 1) && CharacterSelectScene.Current.GetPlayer(i).state == CharacterSelectPlayer.State.Disabled && CharacterSelectScene.Current.playerGUIs[i].guiActive)
                            {
                                CharacterSelectScene.Current.playerGUIs[i].ChangePlayerIconStateTrigger(player.stats.playerCursorFound);
                                CharacterSelectScene.Current.playerModes[i] = CharacterSelectScene.PlayerMode.CPU;
                                CharacterSelectScene.Current.SetJoiningCursor((PlayerId)i, false);
                                //CharacterSelectScene.Current.GetPlayer(i).state = CharacterSelectPlayer.State.Enabling;
                                //base.FrameDelayedCallback(new Action(CharacterSelectScene.Current.GetPlayer(i).SelectingState), 25);
                                CharacterSelectScene.Current.playerGUIs[i].DelayInput();
                                this.inputState = false;
                                base.FrameDelayedCallback(new Action(this.RestoreInput), 5);
                                return;
                            }
                            if (PlayerManager.GetPlayerStatus((PlayerId)i, 3) && CharacterSelectScene.Current.GetPlayer(i).state == CharacterSelectPlayer.State.Selecting && CharacterSelectScene.Current.playerGUIs[i].guiActive)
                            {
                                if (CharacterSelectScene.Current.playerModes[i] == CharacterSelectScene.PlayerMode.Player)
                                {
                                    CharacterSelectScene.Current.playerGUIs[i].ChangePlayerIconStateTrigger(player.stats.playerCursorFound);
                                    CharacterSelectScene.Current.playerModes[i] = CharacterSelectScene.PlayerMode.CPU;
                                    CharacterSelectScene.Current.playerCursors[i].ChangeModeColor();
                                    CharacterSelectScene.Current.playerCursorStars[i].ChangeModeColor();
                                    CharacterSelectScene.Current.playerGUIs[i].DelayInput();
                                    this.inputState = false;
                                    base.FrameDelayedCallback(new Action(this.RestoreInput), 5);
                                    return;
                                }
                            }
                        }
                    }
                }
                if (player.stats.playerCursorFound == CursorHitboxPriority.GUINone) //When selecting to disable a player
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (player.stats.guiPickFlags.HasFlag((CharacterSelectCursorStatsManager.GuiPickFlags)(1 << i)))
                        {
                            if (PlayerManager.GetPlayerStatus((PlayerId)i, 3) && CharacterSelectScene.Current.GetPlayer(i).state == CharacterSelectPlayer.State.Selecting && CharacterSelectScene.Current.playerGUIs[i].guiActive)
                            {
                                CharacterSelectScene.Current.playerGUIs[i].ChangePlayerIconStateTrigger(player.stats.playerCursorFound);
                                //CharacterSelectScene.Current.playerGUIs[i].DeselectCharacter();
                                //CharacterSelectScene.Current.playerCursorStars[i].DisableThisCursorStar();
                                CharacterSelectScene.Current.SetPlayerJoinState(i, 5);
                                /*CharacterSelectData.Data.GetPlayerSlot(i).joinState = CharacterSelectData.PlayerSlot.JoinState.NotJoining;
                                CharacterSelectScene.Current.playerModes[i] = CharacterSelectScene.PlayerMode.Offline;
                                PlayerManager.SetPlayerWaitForJoinRequest((PlayerId)i);*/
                                CharacterSelectScene.Current.playerGUIs[i].DelayInput();
                                this.inputState = false;
                                base.FrameDelayedCallback(new Action(this.RestoreInput), 5);
                                return;
                            }
                            if (PlayerManager.GetPlayerStatus((PlayerId)i, 1) && CharacterSelectScene.Current.GetPlayer(i).state == CharacterSelectPlayer.State.Disabled && CharacterSelectScene.Current.playerGUIs[i].guiActive)
                            {
                                CharacterSelectScene.Current.playerGUIs[i].ChangePlayerIconStateTrigger(player.stats.playerCursorFound);
                                CharacterSelectScene.Current.playerGUIs[i].DeselectCharacter();
                                CharacterSelectScene.Current.playerCursorStars[i].DisableThisCursorStar();
                                CharacterSelectScene.Current.playerCharacterShards[i].DisableThisShard();
                                CharacterSelectScene.Current.playerModes[i] = CharacterSelectScene.PlayerMode.Offline;
                                CharacterSelectScene.Current.playerGUIs[i].DelayInput();
                                this.inputState = false;
                                base.FrameDelayedCallback(new Action(this.RestoreInput), 5);
                                return;
                            }
                        }
                    }
                }
            }
        }
        if (player.input.actions.GetButtonDown((int)MirrorOfDuskButton.Cancel))
        {
            if (!CharacterSelectScene.Current.playerCursorStars[(int)player.id].Held)
            {
                CharacterSelectScene.Current.playerCursorStars[(int)player.id].OnCursorStarSelect((int)player.id, player);
                this.inputState = false;
                base.FrameDelayedCallback(new Action(this.RestoreInput), 5);
                return;
            }
        }
    }

    public void RestoreInput()
    {
        this.inputState = true;
    }
}
