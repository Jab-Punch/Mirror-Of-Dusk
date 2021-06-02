using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerInput : MonoBehaviour {

    public bool enablePlayerInput = false;
    public bool haltPlayerInput = false;
    public int playerHierarchy = 0;
    public string playerInputMode = "";
    protected float padHorizontal;
    protected float padVertical;
    protected float prevPadHorizontal = 0.0f;
    protected float prevPadVertical = 0.0f;
    protected float padKeyHorizontal;
    protected float padKeyVertical;
    protected float prevPadKeyHorizontal = 0.0f;
    protected float prevPadKeyVertical = 0.0f;

    protected float scrollFrames = 0.0f;
    protected int scrollTime = 0;
    protected bool holdScroll = false;

    protected string playerCode;

    protected SystemInputData systemInputData;
    protected SystemInputReader systemInputReader;
    protected InputReader inputReader;
    protected SFXPlayer sfxPlayer;


    public int playerID = 0;
    protected Player r_player;
    protected Player s_player;
    protected Controller r_player_controller;
    protected Controller s_player_controller;
    protected Controller k_player_controller;


    // Use this for initialization
    protected void InitInput()
    {
        playerCode = gameObject.name.Substring(gameObject.name.Length - 1, 1);
        systemInputData = GameObject.Find("EventSystem").GetComponent<SystemInputData>();
        systemInputReader = GameObject.Find("EventSystem").GetComponent<SystemInputReader>();
        inputReader = GameObject.Find("EventSystem").GetComponent<InputReader>();
        sfxPlayer = GameObject.Find("SFX Player").GetComponent<SFXPlayer>();
    }

    public virtual void searchForKeyInputs()
    {
        if (Input.GetButtonDown("Start_" + playerCode))
        {
            inputReader.enterNewPlayerInput("Start", playerCode);
        }
        if (Input.GetButtonDown("Jump_" + playerCode))
        {
            inputReader.enterNewPlayerInput("D", playerCode);
        }
        if (Input.GetButtonUp("Jump_" + playerCode))
        {
            inputReader.releasePlayerInput("D", playerCode);
        }
        if (Input.GetButtonDown("SpecialAttack_" + playerCode))
        {
            inputReader.enterNewPlayerInput("C", playerCode);
        }
        if (Input.GetButtonUp("SpecialAttack_" + playerCode))
        {
            inputReader.releasePlayerInput("C", playerCode);
        }
        if (Input.GetButtonDown("HeavyAttack_" + playerCode))
        {
            inputReader.enterNewPlayerInput("B", playerCode);
        }
        if (Input.GetButtonUp("HeavyAttack_" + playerCode))
        {
            inputReader.releasePlayerInput("B", playerCode);
        }
        if (Input.GetButtonDown("LightAttack_" + playerCode))
        {
            inputReader.enterNewPlayerInput("A", playerCode);
        }
        if (Input.GetButtonUp("LightAttack_" + playerCode))
        {
            inputReader.releasePlayerInput("A", playerCode);
        }
        padHorizontal = Input.GetAxisRaw("Horizontal_" + playerCode);
        padVertical = Input.GetAxisRaw("Vertical_" + playerCode);
        padKeyHorizontal = Input.GetAxisRaw("KeyHorizontal_" + playerCode);
        padKeyVertical = Input.GetAxisRaw("KeyVertical_" + playerCode);
        //Compare to prev axis and stop if lower.

        if (padVertical > 0.05f && padHorizontal < 0.05f && padHorizontal > -0.05f && !inputReader.useHold("Up", playerCode) && !inputReader.useHold("K_Up", playerCode) && !inputReader.axisKeyIsOn(playerCode))
        {
            inputReader.enterNewPlayerInput("Up", playerCode);
        }
        else if (padKeyVertical > 0.05f && padKeyHorizontal < 0.05f && padKeyHorizontal > -0.05f && !inputReader.useHold("Up", playerCode) && !inputReader.useHold("K_Up", playerCode))
        {
            if (!inputReader.axisKeyIsOn(playerCode))
            {
                inputReader.undoDPad(playerCode);
                inputReader.turnAxisKey(playerCode, true);
            }
            inputReader.enterNewPlayerInput("Up", playerCode);
            inputReader.enterNewPlayerInput("K_Up", playerCode);
        }
        if (padVertical < prevPadVertical && inputReader.useHold("Up", playerCode) && !inputReader.axisKeyIsOn(playerCode))
        {
            inputReader.releasePlayerInput("Up", playerCode);
        }
        else if (inputReader.axisKeyIsOn(playerCode) && inputReader.useHold("Up", playerCode) && !inputReader.useHold("K_Up", playerCode))
        {
            inputReader.releasePlayerInput("Up", playerCode);
        }
        if (padKeyVertical < prevPadKeyVertical && inputReader.useHold("K_Up", playerCode))
        {
            inputReader.releasePlayerInput("Up", playerCode);
            inputReader.releasePlayerInput("K_Up", playerCode);
        }
        if (padVertical > 0.05f && padHorizontal < -0.05f && !inputReader.useHold("Up_Left", playerCode) && !inputReader.useHold("K_Up_Left", playerCode) && !inputReader.axisKeyIsOn(playerCode))
        {
            inputReader.enterNewPlayerInput("Up_Left", playerCode);
        }
        else if (padKeyVertical > 0.05f && padKeyHorizontal < -0.05f && !inputReader.useHold("Up_Left", playerCode) && !inputReader.useHold("K_Up_Left", playerCode))
        {
            if (!inputReader.axisKeyIsOn(playerCode))
            {
                inputReader.undoDPad(playerCode);
                inputReader.turnAxisKey(playerCode, true);
            }
            inputReader.enterNewPlayerInput("Up_Left", playerCode);
            inputReader.enterNewPlayerInput("K_Up_Left", playerCode);
        }
        if ((padVertical < prevPadVertical || padHorizontal > prevPadHorizontal) && inputReader.useHold("Up_Left", playerCode) && !inputReader.axisKeyIsOn(playerCode))
        {
            inputReader.releasePlayerInput("Up_Left", playerCode);
        }
        else if (inputReader.axisKeyIsOn(playerCode) && inputReader.useHold("Up_Left", playerCode) && !inputReader.useHold("K_Up_Left", playerCode))
        {
            inputReader.releasePlayerInput("Up_Left", playerCode);
        }
        if ((padKeyVertical < prevPadKeyVertical || padKeyHorizontal > prevPadKeyHorizontal) && inputReader.useHold("K_Up_Left", playerCode))
        {
            inputReader.releasePlayerInput("Up_Left", playerCode);
            inputReader.releasePlayerInput("K_Up_Left", playerCode);
        }
        if (padVertical > 0.05f && padHorizontal > 0.05f && !inputReader.useHold("Up_Right", playerCode) && !inputReader.useHold("K_Up_Right", playerCode) && !inputReader.axisKeyIsOn(playerCode))
        {
            inputReader.enterNewPlayerInput("Up_Right", playerCode);
        }
        else if (padKeyVertical > 0.05f && padKeyHorizontal > 0.05f && !inputReader.useHold("Up_Right", playerCode) && !inputReader.useHold("K_Up_Right", playerCode))
        {
            if (!inputReader.axisKeyIsOn(playerCode))
            {
                inputReader.undoDPad(playerCode);
                inputReader.turnAxisKey(playerCode, true);
            }
            inputReader.enterNewPlayerInput("Up_Right", playerCode);
            inputReader.enterNewPlayerInput("K_Up_Right", playerCode);
        }
        if ((padVertical < prevPadVertical || padHorizontal < prevPadHorizontal) && inputReader.useHold("Up_Right", playerCode) && !inputReader.axisKeyIsOn(playerCode))
        {
            inputReader.releasePlayerInput("Up_Right", playerCode);
        }
        else if (inputReader.axisKeyIsOn(playerCode) && inputReader.useHold("Up_Right", playerCode) && !inputReader.useHold("K_Up_Right", playerCode))
        {
            inputReader.releasePlayerInput("Up_Right", playerCode);
        }
        if ((padKeyVertical < prevPadKeyVertical || padKeyHorizontal < prevPadKeyHorizontal) && inputReader.useHold("K_Up_Right", playerCode))
        {
            inputReader.releasePlayerInput("Up_Right", playerCode);
            inputReader.releasePlayerInput("K_Up_Right", playerCode);
        }
        if (padVertical < -0.05f && padHorizontal < 0.05f && padHorizontal > -0.05f && !inputReader.useHold("Down", playerCode) && !inputReader.useHold("K_Down", playerCode) && !inputReader.axisKeyIsOn(playerCode))
        {
            inputReader.enterNewPlayerInput("Down", playerCode);
        }
        else if (padKeyVertical < -0.05f && padKeyHorizontal < 0.05f && padKeyHorizontal > -0.05f && !inputReader.useHold("Down", playerCode) && !inputReader.useHold("K_Down", playerCode))
        {
            if (!inputReader.axisKeyIsOn(playerCode))
            {
                inputReader.undoDPad(playerCode);
                inputReader.turnAxisKey(playerCode, true);
            }
            inputReader.enterNewPlayerInput("Down", playerCode);
            inputReader.enterNewPlayerInput("K_Down", playerCode);
        }
        if (padVertical > prevPadVertical && inputReader.useHold("Down", playerCode) && !inputReader.axisKeyIsOn(playerCode))
        {
            inputReader.releasePlayerInput("Down", playerCode);
        }
        else if (inputReader.axisKeyIsOn(playerCode) && inputReader.useHold("Down", playerCode) && !inputReader.useHold("K_Down", playerCode))
        {
            inputReader.releasePlayerInput("Down", playerCode);
        }
        if (padKeyVertical > prevPadKeyVertical && inputReader.useHold("K_Down", playerCode))
        {
            inputReader.releasePlayerInput("Down", playerCode);
            inputReader.releasePlayerInput("K_Down", playerCode);
        }
        if (padVertical < -0.05f && padHorizontal < -0.05f && !inputReader.useHold("Down_Left", playerCode) && !inputReader.useHold("K_Down_Left", playerCode) && !inputReader.axisKeyIsOn(playerCode))
        {
            inputReader.enterNewPlayerInput("Down_Left", playerCode);
        }
        else if (padKeyVertical < -0.05f && padKeyHorizontal < -0.05f && !inputReader.useHold("Down_Left", playerCode) && !inputReader.useHold("K_Down_Left", playerCode))
        {
            if (!inputReader.axisKeyIsOn(playerCode))
            {
                inputReader.undoDPad(playerCode);
                inputReader.turnAxisKey(playerCode, true);
            }
            inputReader.enterNewPlayerInput("Down_Left", playerCode);
            inputReader.enterNewPlayerInput("K_Down_Left", playerCode);
        }
        if ((padVertical > prevPadVertical || padHorizontal > prevPadHorizontal) && inputReader.useHold("Down_Left", playerCode) && !inputReader.axisKeyIsOn(playerCode))
        {
            inputReader.releasePlayerInput("Down_Left", playerCode);
        }
        else if (inputReader.axisKeyIsOn(playerCode) && inputReader.useHold("Down_Left", playerCode) && !inputReader.useHold("K_Down_Left", playerCode))
        {
            inputReader.releasePlayerInput("Down_Left", playerCode);
        }
        if ((padKeyVertical > prevPadKeyVertical || padKeyHorizontal > prevPadKeyHorizontal) && inputReader.useHold("K_Down_Left", playerCode))
        {
            inputReader.releasePlayerInput("Down_Left", playerCode);
            inputReader.releasePlayerInput("K_Down_Left", playerCode);
        }
        if (padVertical < -0.05f && padHorizontal > 0.05f && !inputReader.useHold("Down_Right", playerCode) && !inputReader.useHold("K_Down_Right", playerCode) && !inputReader.axisKeyIsOn(playerCode))
        {
            inputReader.enterNewPlayerInput("Down_Right", playerCode);
        }
        else if (padKeyVertical < -0.05f && padKeyHorizontal > 0.05f && !inputReader.useHold("Down_Right", playerCode) && !inputReader.useHold("K_Down_Right", playerCode))
        {
            if (!inputReader.axisKeyIsOn(playerCode))
            {
                inputReader.undoDPad(playerCode);
                inputReader.turnAxisKey(playerCode, true);
            }
            inputReader.enterNewPlayerInput("Down_Right", playerCode);
            inputReader.enterNewPlayerInput("K_Down_Right", playerCode);
        }
        if ((padVertical > prevPadVertical || padHorizontal < prevPadHorizontal) && inputReader.useHold("Down_Right", playerCode) && !inputReader.axisKeyIsOn(playerCode))
        {
            inputReader.releasePlayerInput("Down_Right", playerCode);
        }
        else if (inputReader.axisKeyIsOn(playerCode) && inputReader.useHold("Down_Right", playerCode) && !inputReader.useHold("K_Down_Right", playerCode))
        {
            inputReader.releasePlayerInput("Down_Right", playerCode);
        }
        if ((padKeyVertical > prevPadKeyVertical || padKeyHorizontal < prevPadKeyHorizontal) && inputReader.useHold("K_Down_Right", playerCode))
        {
            inputReader.releasePlayerInput("Down_Right", playerCode);
            inputReader.releasePlayerInput("K_Down_Right", playerCode);
        }
        if (padHorizontal < -0.05f && padVertical < 0.05f && padVertical > -0.05f && !inputReader.useHold("Left", playerCode) && !inputReader.useHold("K_Left", playerCode) && !inputReader.axisKeyIsOn(playerCode))
        {
            inputReader.enterNewPlayerInput("Left", playerCode);
        }
        else if (padKeyHorizontal < -0.05f && padKeyVertical < 0.05f && padKeyVertical > -0.05f && !inputReader.useHold("Left", playerCode) && !inputReader.useHold("K_Left", playerCode))
        {
            if (!inputReader.axisKeyIsOn(playerCode))
            {
                inputReader.undoDPad(playerCode);
                inputReader.turnAxisKey(playerCode, true);
            }
            inputReader.enterNewPlayerInput("Left", playerCode);
            inputReader.enterNewPlayerInput("K_Left", playerCode);
        }
        if (padHorizontal > prevPadHorizontal && inputReader.useHold("Left", playerCode) && !inputReader.axisKeyIsOn(playerCode))
        {
            inputReader.releasePlayerInput("Left", playerCode);
        }
        else if (inputReader.axisKeyIsOn(playerCode) && inputReader.useHold("Left", playerCode) && !inputReader.useHold("K_Left", playerCode))
        {
            inputReader.releasePlayerInput("Left", playerCode);
        }
        if (padKeyHorizontal > prevPadKeyHorizontal && inputReader.useHold("K_Left", playerCode))
        {
            inputReader.releasePlayerInput("Left", playerCode);
            inputReader.releasePlayerInput("K_Left", playerCode);
        }
        if (padHorizontal > 0.05f && padVertical < 0.05f && padVertical > -0.05f && !inputReader.useHold("Right", playerCode) && !inputReader.useHold("K_Right", playerCode) && !inputReader.axisKeyIsOn(playerCode))
        {
            inputReader.enterNewPlayerInput("Right", playerCode);
        }
        else if (padKeyHorizontal > 0.05f && padKeyVertical < 0.05f && padKeyVertical > -0.05f && !inputReader.useHold("Right", playerCode) && !inputReader.useHold("K_Right", playerCode))
        {
            if (!inputReader.axisKeyIsOn(playerCode))
            {
                inputReader.undoDPad(playerCode);
                inputReader.turnAxisKey(playerCode, true);
            }
            inputReader.enterNewPlayerInput("Right", playerCode);
            inputReader.enterNewPlayerInput("K_Right", playerCode);
        }
        if (padHorizontal < prevPadHorizontal && inputReader.useHold("Right", playerCode) && !inputReader.axisKeyIsOn(playerCode))
        {
            inputReader.releasePlayerInput("Right", playerCode);
        }
        else if (inputReader.axisKeyIsOn(playerCode) && inputReader.useHold("Right", playerCode) && !inputReader.useHold("K_Right", playerCode))
        {
            inputReader.releasePlayerInput("Right", playerCode);
        }
        if (padKeyHorizontal < prevPadKeyHorizontal && inputReader.useHold("K_Right", playerCode))
        {
            inputReader.releasePlayerInput("Right", playerCode);
            inputReader.releasePlayerInput("K_Right", playerCode);
        }
        if (inputReader.checkKeyReleased(playerCode))
        {
            inputReader.turnAxisKey(playerCode, false);
        }
        prevPadHorizontal = padHorizontal;
        prevPadVertical = padVertical;
        prevPadKeyHorizontal = padKeyHorizontal;
        prevPadKeyVertical = padKeyVertical;
    }
}
