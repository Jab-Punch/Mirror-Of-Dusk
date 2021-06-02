using System;
using UnityEngine;
using Rewired;

public class MirrorOfDuskInput
{
    public static readonly MirrorOfDuskInput.Pair[] pairs = new MirrorOfDuskInput.Pair[]
    {
        new MirrorOfDuskInput.Pair(MirrorOfDuskInput.InputSymbols.PS4_X, "<sprite=0>", "<sprite=1>"),
        new MirrorOfDuskInput.Pair(MirrorOfDuskInput.InputSymbols.PS4_O, "<sprite=2>", "<sprite=3>"),
        new MirrorOfDuskInput.Pair(MirrorOfDuskInput.InputSymbols.PS4_Square, "<sprite=4>", "<sprite=5>"),
        new MirrorOfDuskInput.Pair(MirrorOfDuskInput.InputSymbols.PS4_Triangle, "<sprite=6>", "<sprite=7>")
    };

    public enum InputDevice
    {
        Keyboard,
        Controller_1,
        Controller_2,
        Controller_3,
        Controller_4
    }

    public enum InputSymbols
    {
        PS4_NONE,
        PS4_Square,
        PS4_Triangle,
        PS4_O,
        PS4_X,
        PS4_RB,
        PS4_LB,
        PS4_RT,
        PS4_LT
    }

    public class Pair
    {
        public Pair(MirrorOfDuskInput.InputSymbols symbol, string first, string second)
        {
            this.symbol = symbol;
            this.first = first;
            this.second = second;
        }
        
        public readonly MirrorOfDuskInput.InputSymbols symbol;
        
        public readonly string first;
        public readonly string second;
    }

    public static Localization.Translation InputDisplayForButton(MirrorOfDuskButton button, int rewiredPlayerId = 0)
    {
        Player.ControllerHelper controllers = ReInput.players.GetPlayer(rewiredPlayerId).controllers;
        ActionElementMap firstElementMapWithAction;
        if (controllers != null && controllers.joystickCount > 0)
        {
            ControllerType controllerType = (ControllerType)2;
            Controller lastActiveController = ReInput.players.GetPlayer(rewiredPlayerId).controllers.GetLastActiveController();
            if (lastActiveController != null)
            {
                controllerType = lastActiveController.type;
            }
            firstElementMapWithAction = controllers.maps.GetFirstElementMapWithAction(controllerType, (int)button, true);
        } else
        {
            if (PlatformHelper.IsConsole)
            {
                return default(Localization.Translation);
            }
            firstElementMapWithAction = ReInput.players.GetPlayer(rewiredPlayerId).controllers.maps.GetFirstElementMapWithAction((int)button, true);
        }
        if (firstElementMapWithAction == null)
        {
            return new Localization.Translation
            {
                text = string.Empty
            };
        }
        string text = firstElementMapWithAction.elementIdentifierName;
        if (button == MirrorOfDuskButton.Dodge && text.Contains("Shift"))
        {
            text = "Shift";
        }
        string text2 = MirrorOfDuskInput.handleCustomGlyphs(text, rewiredPlayerId);
        Localization.Translation result = Localization.Translate(text);
        if (text2 == null)
        {
            if (!string.IsNullOrEmpty(result.text))
            {
                text = result.text;
            }
        } else
        {
            text = text2;
        }
        text = text.ToUpper();
        text = text.Replace(" SHOULDER", "B");
        text = text.Replace(" BUMPER", "B");
        text = text.Replace(" TRIGGER", "T");
        text = text.Replace("LEFT", "L");
        text = text.Replace("RIGHT", "R");
        text = text.Replace("R SHIFT", "SHIFT");
        text = text.Replace("L SHIFT", "SHIFT");
        text = text.Replace(" +", string.Empty);
        text = text.Replace(" -", string.Empty);
        result.text = text;
        return result;
    }

    private static string handleCustomGlyphs(string input, int rewiredPlayerId)
    {
        return null;
    }

    public static MirrorOfDuskInput.InputSymbols InputSymbolForButton(MirrorOfDuskButton button)
    {
        MirrorOfDuskInput.InputSymbols result;
        switch (button)
        {
            case MirrorOfDuskButton.LightAttack:
                result = MirrorOfDuskInput.InputSymbols.PS4_Square;
                break;
            case MirrorOfDuskButton.HeavyAttack:
                result = MirrorOfDuskInput.InputSymbols.PS4_Triangle;
                break;
            case MirrorOfDuskButton.SpecialAttack:
                result = MirrorOfDuskInput.InputSymbols.PS4_O;
                break;
            case MirrorOfDuskButton.Jump:
                result = MirrorOfDuskInput.InputSymbols.PS4_X;
                break;
            case MirrorOfDuskButton.Dodge:
                result = MirrorOfDuskInput.InputSymbols.PS4_LT;
                break;
            case MirrorOfDuskButton.Block:
                result = MirrorOfDuskInput.InputSymbols.PS4_RT;
                break;
            default:
                if (button != MirrorOfDuskButton.None)
                {
                }
                result = MirrorOfDuskInput.InputSymbols.PS4_NONE;
                break;
            case MirrorOfDuskButton.Accept:
                result = MirrorOfDuskInput.InputSymbols.PS4_X;
                break;
            case MirrorOfDuskButton.Cancel:
                result = MirrorOfDuskInput.InputSymbols.PS4_O;
                break;
        }
        return result;
    }

    public static string DialogueStringFromButton(MirrorOfDuskButton button)
    {
        return " {" + button + "} ";
    }

    public static Joystick CheckForUnconnectedControllerPress()
    {
        foreach (Joystick joystick in ReInput.controllers.Joysticks)
        {
            if (!ReInput.controllers.IsJoystickAssigned(joystick))
            {
                if (joystick.GetAnyButtonDown())
                {
                    return joystick;
                }
            }
        }
        return null;
    }

    public static bool AutoAssignController(int rewiredPlayerId)
    {
        foreach (Joystick joystick in ReInput.controllers.Joysticks)
        {
            if (!ReInput.controllers.IsJoystickAssigned(joystick))
            {
                Player player = ReInput.players.GetPlayer(rewiredPlayerId);
                if (player != null)
                {
                    if (player.controllers.joystickCount <= 0)
                    {
                        player.controllers.AddController(joystick, true);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public static Joystick AutomaticallyAssignJoystick(Player rewiredPlayer)
    {
        foreach (Joystick joystick in ReInput.controllers.Joysticks)
        {
            if (!ReInput.controllers.IsJoystickAssigned(joystick))
            {
                Player player = rewiredPlayer;
                if (player != null)
                {
                    if (player.controllers.joystickCount <= 0)
                    {
                        player.controllers.AddController(joystick, true);
                        return joystick;
                    }
                }
            }
        }
        return null;
    }

    public class AnyPlayerInput
    {
        private Player[] players;
        public bool checkIfKOed;

        public AnyPlayerInput(bool checkIfKOed = false)
        {
            this.checkIfKOed = checkIfKOed;
            this.players = new Player[]
            {
                PlayerManager.GetPlayerInput(PlayerId.PlayerOne),
                PlayerManager.GetPlayerInput(PlayerId.PlayerTwo),
                PlayerManager.GetPlayerInput(PlayerId.PlayerThree),
                PlayerManager.GetPlayerInput(PlayerId.PlayerFour),
            };
        }

        public float GetAxis(MirrorOfDuskButton button)
        {
            foreach (Player player in this.players)
            {
                if (player.GetAxis((int)button) != 0f && (!this.checkIfKOed || !this.IsKOed(player)))
                {
                    return player.GetAxis((int)button);
                }
            }
            return 0f;
        }

        public bool GetButton(MirrorOfDuskButton button)
        {
            foreach (Player player in this.players)
            {
                if (player.GetButton((int)button) && (!this.checkIfKOed || !this.IsKOed(player)))
                {
                    return true;
                }
            }
            return false;
        }

        public bool GetButtonDown(MirrorOfDuskButton button)
        {
            if (InterruptingPrompt.IsInterrupting())
            {
                return false;
            }
            foreach (Player player in this.players)
            {
                if (player.GetButtonDown((int)button) && (!this.checkIfKOed || !this.IsKOed(player)))
                {
                    return true;
                }
            }
            return false;
        }

        public int GetButtonDownAndReturnPlayer(MirrorOfDuskButton button)
        {
            if (InterruptingPrompt.IsInterrupting())
            {
                return -1;
            }
            foreach (Player player in this.players)
            {
                if (player.GetButtonDown((int)button) && (!this.checkIfKOed || !this.IsKOed(player)))
                {
                    return player.id;
                }
            }
            return -1;
        }

        public bool GetActionButtonDown()
        {
            if (InterruptingPrompt.IsInterrupting())
            {
                return false;
            }
            foreach (Player player in this.players)
            {
                if ((player.GetButtonDown(13) || player.GetButtonDown(14) || player.GetButtonDown(7) || player.GetButtonDown(15) || player.GetButtonDown(2) || player.GetButtonDown(6) || player.GetButtonDown(8) || player.GetButtonDown(3) || player.GetButtonDown(4) || player.GetButtonDown(5)) && (!this.checkIfKOed || !this.IsKOed(player)))
                {
                    return true;
                }
            }
            return false;
        }

        public bool GetAndReturnJoystickActionButtonDown(ref MirrorOfDuskButton button)
        {
            if (InterruptingPrompt.IsInterrupting())
            {
                return false;
            }
            foreach (Player player in this.players)
            {
                if (player.controllers.joystickCount > 0)
                {
                    foreach (Controller cont in player.controllers.Controllers)
                    {
                        if (cont.type == ControllerType.Joystick && cont.isConnected)
                        {
                            Controller currentJoystick = player.controllers.GetController(ControllerType.Joystick, cont.id);
                            ControllerMap cmp = player.controllers.maps.GetMap(currentJoystick, 1, 0);
                            ControllerPollingInfo pollingInfo = ReInput.controllers.polling.PollControllerForFirstElementDown(ControllerType.Joystick, currentJoystick.id);
                            if (cmp.player.GetButtonDown("LightAttack"))
                            {
                                button = MirrorOfDuskButton.LightAttack;
                                return true;
                            } else if (cmp.player.GetButtonDown("HeavyAttack"))
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
                        }
                    }
                }
            }
            return false;
        }

        public bool GetAndReturnKeyboardActionButtonDown(ref MirrorOfDuskButton button)
        {
            if (InterruptingPrompt.IsInterrupting())
            {
                return false;
            }
            foreach (Player player in this.players)
            {
                if ((player.id == 0 || player.id == 1) && player.controllers.hasKeyboard)
                {
                    foreach (Controller cont in player.controllers.Controllers)
                    {
                        if (cont.type == ControllerType.Keyboard && cont.isConnected)
                        {
                            Controller currentKeyboard = player.controllers.GetController(ControllerType.Keyboard, cont.id);
                            ControllerMap cmp = player.controllers.maps.GetMap(currentKeyboard, 0, ((player.id == 0) ? 1 : 2));
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

        public bool GetAnyButtonDown()
        {
            if (InterruptingPrompt.IsInterrupting())
            {
                return false;
            }
            foreach (Player player in this.players)
            {
                foreach (Controller controller in player.controllers.Controllers)
                {
                    if (controller.GetAnyButtonDown() && (!this.checkIfKOed || !this.IsKOed(player)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool GetAnyButtonHeld()
        {
            if (InterruptingPrompt.IsInterrupting())
            {
                return false;
            }
            foreach (Player player in this.players)
            {
                foreach (Controller controller in player.controllers.Controllers)
                {
                    if (controller.GetAnyButton() && (!this.checkIfKOed || !this.IsKOed(player)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool GetButtonUp(MirrorOfDuskButton button)
        {
            foreach (Player player in this.players)
            {
                if (player.GetButtonUp((int)button) && (!this.checkIfKOed || !this.IsKOed(player)))
                {
                    return true;
                }
            }
            return false;
        }
        
        private bool IsKOed(Player player)
        {
            PlayerId id;
            if (player == this.players[0])
            {
                id = PlayerId.PlayerOne;
            } else if (player == this.players[1])
            {
                id = PlayerId.PlayerTwo;
            } else if (player == this.players[2])
            {
                id = PlayerId.PlayerThree;
            } else if (player == this.players[3])
            {
                id = PlayerId.PlayerFour;
            } else
            {
                id = PlayerId.None;
            }
            AbstractPlayerController player2 = PlayerManager.GetPlayer(id);
            return player2 == null || player2.IsKOed;
        }
    }
}
