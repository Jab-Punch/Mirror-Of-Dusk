using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rewired;

public class YesNoMenuScreen : HorizontalMenuScreen
{
    protected CharacterSelectManager characterSelectManager;
    protected MenuEventManager menuEventManagerInstance;

    public int playerId = 0;
    protected int c_playerId;
    protected Player playerControl;
    protected CSPlayerGUI csPlayerGUI;

    public MenuSlot[] selectionSlots;
    protected int currentSlot;
    protected int closeControlId;

    protected bool bufferOn = false;
    protected NewMenuScreenRoot selectedRootMenu;
    protected TextMeshProUGUI nameTextField;

    protected virtual void Awake()
    {
        menuEventManagerInstance = gameObject.GetComponent<MenuEventManager>();
        setUpMenu();
        initializeHorizontalScreen();
        initializeYesNoScreen();
    }

    void Start()
    {
        scriptOn = true;
        gameObject.SetActive(false);
    }

    void Update()
    {

    }

    public override void OnEnable()
    {
        currentSlot = 1;
        if (scriptOn)
        {
            for (int i = 0; i < selectionSlots.Length; i++)
            {
                (selectionSlots[i] as EventMenuSlot).Instantly = true;
                selectionSlots[i].enabled = false;
            }
            (selectionSlots[currentSlot] as EventMenuSlot).Instantly = true;
            selectionSlots[currentSlot].enabled = true;
        }
    }

    public override void OnDisable()
    {
        for (int i = 0; i < selectionSlots.Length; i++)
        {
            selectionSlots[i].enabled = false;
            (selectionSlots[i] as EventMenuSlot).InstantDestroyHighlighter();
        }
    }

    protected override void setUpMenu()
    {
        characterSelectManager = CharacterSelectManager.characterSelectManager;

        sfxPlayer = characterSelectManager.sfxPlayer.GetComponent<SFXPlayer>();
        gameData = characterSelectManager.gameData.GetComponent<GameData>();
        csPlayerInput = characterSelectManager.players[playerId].GetComponent<CSPlayerInput>();
        currentPlayerInput = csPlayerInput;
        playerControl = ReInput.players.GetPlayer(playerId);

        try
        {
            menuDetailsRoot = gameObject.GetComponentInChildren<MenuDetailsRoot>();
        }
        catch (System.NullReferenceException)
        {

        }
        inputReader = characterSelectManager.eventSystem.GetComponent<InputReader>();
        csPlayerData = characterSelectManager.players[playerId].GetComponent<CSPlayerData>();
    }

    protected override void initializeHorizontalScreen()
    {
        currentSlot = 1;
    }

    private void initializeYesNoScreen()
    {
        c_playerId = playerId;
        csPlayerGUI = characterSelectManager.csPlayerGUI.transform.GetChild(playerId).GetComponent<CSPlayerGUI>();
    }

    public void assignYesNoMenu(NewMenuScreenRoot rootMen, int ccid)
    {
        selectedRootMenu = rootMen;
        closeControlId = ccid;
    }

    public override void directionalController()
    {
        if (!currentPlayerInput.onUI && !bufferOn)
        {
            padHorizontal = playerControl.GetAxisRaw("Move Horizontal");
            padVertical = playerControl.GetAxisRaw("Move Vertical");
            if (padVTime < 0.02f || padVTime > -0.02f || padHTime < 0.02f || padHTime > -0.02f)
            {
                if (padVTime < 0.02f || padVTime > -0.02f)
                {
                    padVTime = (int)padVertical;
                    if (padVTime > 0)
                    {
                        padReadDown = false;
                    }
                    else if (padVTime < 0)
                    {
                        padReadDown = true;
                    }
                }
                if (padHTime < 0.02f || padHTime > -0.02f)
                {
                    padHTime = (int)padHorizontal;
                    if (padHTime > 0)
                    {
                        padReadRight = false;
                    }
                    else if (padHTime < 0)
                    {
                        padReadRight = true;
                    }
                }
            }
            else
            {
                if (!padReadRight && (padHorizontal < padHTime))
                {
                    padHTime = 0;
                }
                if (!padReadDown && (padVertical < padVTime))
                {
                    padVTime = 0;
                }
                if (padReadRight && (padHorizontal > padHTime))
                {
                    padHTime = 0;
                }
                if (padReadDown && (padVertical > padVTime))
                {
                    padVTime = 0;
                }
            }
            scrollFrames--;
            if (scrollFrames <= 0)
            {
                scrollFrames = 0f;
            }
            if ((inputReader.useNewInput("MoveRight", c_playerId) || inputReader.useNewInput("MoveDown_Right", c_playerId) || inputReader.useNewInput("MoveUp_Right", c_playerId)) && scrollFrames <= 0)
            {
                if (currentSlot == 0)
                {
                    sfxPlayer.PlaySound("Scroll");
                    selectionSlots[currentSlot].enabled = false;
                    currentSlot++;
                    selectionSlots[currentSlot].enabled = true;
                }
            }
            else if ((inputReader.useNewInput("MoveLeft", c_playerId) || inputReader.useNewInput("MoveDown_Left", c_playerId) || inputReader.useNewInput("MoveUp_Left", c_playerId)) && scrollFrames <= 0)
            {
                if (currentSlot == 1)
                {
                    sfxPlayer.PlaySound("Scroll");
                    selectionSlots[currentSlot].enabled = false;
                    currentSlot--;
                    selectionSlots[currentSlot].enabled = true;
                }
            }
        }
        bufferOn = false;
    }

    public override void selectOption()
    {
        bufferOn = true;
        menuEventManagerInstance.TriggerEvent("ActivateSlot", null);
    }

    public void confirmOption()
    {
        sfxPlayer.PlaySound("Confirm");
        csPlayerGUI.bufferGUI = 30;
        //selectedRootMenu.unlockMenu();
        selectedRootMenu.updateControlCode(closeControlId);
        gameObject.SetActive(false);
    }

    public override void closeMenu()
    {
        sfxPlayer.PlaySound("Cancel");
        csPlayerGUI.bufferGUI = 30;
        //selectedRootMenu.unlockMenu();
        selectedRootMenu.updateControlCode(closeControlId);
        gameObject.SetActive(false);
    }

    
}
