using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rewired;

public class SelectUserMenuScreen : HorizontalMenuScreen
{
    private CharacterSelectManager characterSelectManager;

    public int playerId = 0;
    private int c_playerId;
    private Player playerControl;
    private CSPlayerGUI csPlayerGUI;

    public SubMenuSlot[] horizontalMenuSlots;
    private SelectUserMenuScreen[] userMenuScreens;
    private int selectedUserId;
    public int SelectedUserId
    {
        get { return selectedUserId; }
    }
    private int currentNameReference;
    private int currentNameSetMin;
    private int currentNameSetMax;

    private int currentHorizSlot;
    private int horizontalSlotLimit;
    private bool bufferOn = false;

    private SelectUserNewNameScreen newNameScreen;
    private SelectUserDeleteNameScreen deleteUserScreen;
    private SpriteRenderer arrowUp;
    private SpriteRenderer arrowDown;

    [Header("Sprite Prefabs")]
    public GameObject _arrowUp;
    public GameObject _arrowDown;

    private void Awake()
    {
        setUpMenu();
        initializeHorizontalScreen();
        initializeSelectUserMenuScreen();
    }
    
    void Start () {
        scriptOn = true;
        gameObject.SetActive(false);
    }

    void Update()
    {

    }

    public override void OnEnable()
    {
        Canvas.ForceUpdateCanvases();
        unlockMenu();
        currentHorizSlot = 0;
        horizontalMenuSlots[currentHorizSlot].subMenuSlots[horizontalMenuSlots[currentHorizSlot].currentMenuSlot].enabled = true;
        if (scriptOn)
        {
            for (int i = 0; i < horizontalMenuSlots[0].subMenuSlots.Length; i++)
            {
                (horizontalMenuSlots[0].subMenuSlots[i] as UserSelectSlot).deselectUpdate();
            }
            if ((((selectedUserId - currentNameReference) + horizontalMenuSlots[0].currentMenuSlot) < horizontalMenuSlots[0].slotLimit && ((selectedUserId - currentNameReference) + horizontalMenuSlots[0].currentMenuSlot) > 0) || (currentNameReference == 0))
            {
                if (selectedUserId == 0)
                {
                    (horizontalMenuSlots[0].subMenuSlots[0] as UserSelectSlot).selectUpdate();
                } else if (selectedUserId >= currentNameSetMin && selectedUserId <= currentNameSetMax)
                {
                    (horizontalMenuSlots[0].subMenuSlots[(3 - currentNameSetMax + selectedUserId)] as UserSelectSlot).selectUpdate();
                }
            }
            UpdateArrows();
            /*if (menuDetailsRoot != null)
            {
                menuDetailsRoot.updateDetails(menuItemCodes[currentMenuHCode].menuDetailSection);
            }*/
            StartCoroutine("summonThisMenu", endMoveY);
        }
    }

    public override void OnDisable()
    {
        horizontalMenuSlots[currentHorizSlot].subMenuSlots[horizontalMenuSlots[currentHorizSlot].currentMenuSlot].enabled = false;
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
        userMenuScreens = new SelectUserMenuScreen[4];
        for (int i = 0; i < userMenuScreens.Length; i++)
        {
            userMenuScreens[i] = characterSelectManager.selectUserMenuScreens.transform.GetChild(i).GetComponent<SelectUserMenuScreen>();
        }
    }

    protected override void initializeHorizontalScreen()
    {
        currentHorizSlot = 0;
        currentNameReference = 0;
        currentNameSetMin = 1;
        currentNameSetMax = 3;
        for (int i = 0; i < horizontalMenuSlots.Length; i++)
        {
            horizontalMenuSlots[i].currentMenuSlot = 0;
        }
        horizontalSlotLimit = horizontalMenuSlots.Length;
    }

    private void initializeSelectUserMenuScreen()
    {
        c_playerId = playerId;
        csPlayerGUI = characterSelectManager.csPlayerGUI.transform.GetChild(playerId).GetComponent<CSPlayerGUI>();
        selectedUserId = csPlayerGUI.currentPlayerUserNameCode;
        newNameScreen = characterSelectManager.selectUserNewNameScreens.transform.GetChild(playerId).GetComponent<SelectUserNewNameScreen>();
        newNameScreen.assignRootMenu(this);
        deleteUserScreen = characterSelectManager.selectUserDeleteNameScreens.transform.GetChild(playerId).GetComponent<SelectUserDeleteNameScreen>();
        deleteUserScreen.assignRootMenu(this);
        arrowUp = _arrowUp.GetComponent<SpriteRenderer>();
        arrowDown = _arrowDown.GetComponent<SpriteRenderer>();
        for (int i = 1; i < horizontalMenuSlots[currentHorizSlot].slotLimit; i++)
        {
            if (i < gameData.userNameReference.Length)
            {
                (horizontalMenuSlots[currentHorizSlot].subMenuSlots[i] as UserSelectSlot).nameText.text = gameData.userNameReference[currentNameReference + i].Name;
            } else
            {
                (horizontalMenuSlots[currentHorizSlot].subMenuSlots[i] as UserSelectSlot).nameText.text = "";
            }
        }
    }

    public override void directionalController()
    {
        if (!currentPlayerInput.onUI && !bufferOn)
        {
            if (controlCode == 0)
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
                if ((inputReader.useNewInput("MoveDown", c_playerId) || inputReader.useNewInput("MoveDown_Left", c_playerId) || inputReader.useNewInput("MoveDown_Right", c_playerId)) && scrollFrames <= 0)
                {
                    if (horizontalMenuSlots[currentHorizSlot].currentMenuSlot < (horizontalMenuSlots[currentHorizSlot].slotLimit - 1))
                    {
                        horizontalMenuSlots[currentHorizSlot].subMenuSlots[horizontalMenuSlots[currentHorizSlot].currentMenuSlot].enabled = false;
                        if (currentHorizSlot == 0 && currentNameReference < gameData.idCounter - 1)
                        {
                            if (horizontalMenuSlots[currentHorizSlot].currentMenuSlot < 2 || (horizontalMenuSlots[currentHorizSlot].currentMenuSlot == 2 && currentNameReference == gameData.idCounter - 2))
                            {
                                sfxPlayer.PlaySound("Scroll");
                                if ((((selectedUserId - currentNameReference) + horizontalMenuSlots[currentHorizSlot].currentMenuSlot) < horizontalMenuSlots[currentHorizSlot].slotLimit && ((selectedUserId - currentNameReference) + horizontalMenuSlots[currentHorizSlot].currentMenuSlot) > 0))
                                {
                                    (horizontalMenuSlots[currentHorizSlot].subMenuSlots[(selectedUserId - currentNameReference) + horizontalMenuSlots[currentHorizSlot].currentMenuSlot] as UserSelectSlot).deselectUpdate();
                                }
                                if (horizontalMenuSlots[currentHorizSlot].currentMenuSlot == 0)
                                {
                                    currentNameReference = currentNameSetMin;
                                } else
                                {
                                    currentNameReference++;
                                }
                                horizontalMenuSlots[currentHorizSlot].currentMenuSlot += 1;
                                if ((((selectedUserId - currentNameReference) + horizontalMenuSlots[currentHorizSlot].currentMenuSlot) < horizontalMenuSlots[currentHorizSlot].slotLimit && ((selectedUserId - currentNameReference) + horizontalMenuSlots[currentHorizSlot].currentMenuSlot) > 0))
                                {
                                    (horizontalMenuSlots[currentHorizSlot].subMenuSlots[(selectedUserId - currentNameReference) + horizontalMenuSlots[currentHorizSlot].currentMenuSlot] as UserSelectSlot).selectUpdate();
                                }
                            }
                            else if (horizontalMenuSlots[currentHorizSlot].currentMenuSlot == 2 && currentNameReference < gameData.idCounter - 2)
                            {
                                sfxPlayer.PlaySound("Scroll");
                                if ((((selectedUserId - currentNameReference) + horizontalMenuSlots[currentHorizSlot].currentMenuSlot) < horizontalMenuSlots[currentHorizSlot].slotLimit && ((selectedUserId - currentNameReference) + horizontalMenuSlots[currentHorizSlot].currentMenuSlot) > 0) || currentNameReference == 0)
                                {
                                    (horizontalMenuSlots[currentHorizSlot].subMenuSlots[(selectedUserId - currentNameReference) + horizontalMenuSlots[currentHorizSlot].currentMenuSlot] as UserSelectSlot).deselectUpdate();
                                }
                                if (currentNameReference < gameData.idCounter - 2)
                                {
                                    for (int i = 1; i < horizontalMenuSlots[currentHorizSlot].slotLimit; i++)
                                    {
                                        (horizontalMenuSlots[currentHorizSlot].subMenuSlots[i] as UserSelectSlot).nameText.text = gameData.userNameReference[currentNameReference - 1 + i].Name;
                                    }
                                }
                                currentNameReference++;
                                currentNameSetMin++;
                                currentNameSetMax++;
                                if ((((selectedUserId - currentNameReference) + horizontalMenuSlots[currentHorizSlot].currentMenuSlot) < horizontalMenuSlots[currentHorizSlot].slotLimit && ((selectedUserId - currentNameReference) + horizontalMenuSlots[currentHorizSlot].currentMenuSlot) > 0) || currentNameReference == 0)
                                {
                                    (horizontalMenuSlots[currentHorizSlot].subMenuSlots[(selectedUserId - currentNameReference) + horizontalMenuSlots[currentHorizSlot].currentMenuSlot] as UserSelectSlot).selectUpdate();
                                }
                            }
                            UpdateArrows();
                        }
                        else if (currentHorizSlot == 1)
                        {
                            sfxPlayer.PlaySound("Scroll");
                            horizontalMenuSlots[currentHorizSlot].currentMenuSlot += 1;
                        }
                        horizontalMenuSlots[currentHorizSlot].subMenuSlots[horizontalMenuSlots[currentHorizSlot].currentMenuSlot].enabled = true;
                    }
                    scrollFrames = ((holdScroll) ? 5f : 20f);
                    holdScroll = true;
                }
                else if ((inputReader.useNewInput("MoveUp", c_playerId) || inputReader.useNewInput("MoveUp_Left", c_playerId) || inputReader.useNewInput("MoveUp_Right", c_playerId)) && scrollFrames <= 0)
                {
                    if (horizontalMenuSlots[currentHorizSlot].currentMenuSlot > 0)
                    {
                        horizontalMenuSlots[currentHorizSlot].subMenuSlots[horizontalMenuSlots[currentHorizSlot].currentMenuSlot].enabled = false;
                        if (currentHorizSlot == 0 && currentNameReference > 0)
                        {
                            if (horizontalMenuSlots[currentHorizSlot].currentMenuSlot > 1 || (horizontalMenuSlots[currentHorizSlot].currentMenuSlot == 1 && currentNameReference == 1))
                            {
                                sfxPlayer.PlaySound("Scroll");
                                if ((((selectedUserId - currentNameReference) + horizontalMenuSlots[currentHorizSlot].currentMenuSlot) < horizontalMenuSlots[currentHorizSlot].slotLimit && ((selectedUserId - currentNameReference) + horizontalMenuSlots[currentHorizSlot].currentMenuSlot) > 0))
                                {
                                    (horizontalMenuSlots[currentHorizSlot].subMenuSlots[(selectedUserId - currentNameReference) + horizontalMenuSlots[currentHorizSlot].currentMenuSlot] as UserSelectSlot).deselectUpdate();
                                }
                                horizontalMenuSlots[currentHorizSlot].currentMenuSlot -= 1;
                                currentNameReference--;
                                if ((((selectedUserId - currentNameReference) + horizontalMenuSlots[currentHorizSlot].currentMenuSlot) < horizontalMenuSlots[currentHorizSlot].slotLimit && ((selectedUserId - currentNameReference) + horizontalMenuSlots[currentHorizSlot].currentMenuSlot) > 0))
                                {
                                    (horizontalMenuSlots[currentHorizSlot].subMenuSlots[(selectedUserId - currentNameReference) + horizontalMenuSlots[currentHorizSlot].currentMenuSlot] as UserSelectSlot).selectUpdate();
                                }
                            }
                            else if (horizontalMenuSlots[currentHorizSlot].currentMenuSlot == 1 && currentNameReference > 1)
                            {
                                sfxPlayer.PlaySound("Scroll");
                                if ((((selectedUserId - currentNameReference) + horizontalMenuSlots[currentHorizSlot].currentMenuSlot) < horizontalMenuSlots[currentHorizSlot].slotLimit && ((selectedUserId - currentNameReference) + horizontalMenuSlots[currentHorizSlot].currentMenuSlot) > 0) || currentNameReference == 1)
                                {
                                    (horizontalMenuSlots[currentHorizSlot].subMenuSlots[(selectedUserId - currentNameReference) + horizontalMenuSlots[currentHorizSlot].currentMenuSlot] as UserSelectSlot).deselectUpdate();
                                }
                                for (int i = 1; i < horizontalMenuSlots[currentHorizSlot].slotLimit; i++)
                                {
                                    (horizontalMenuSlots[currentHorizSlot].subMenuSlots[i] as UserSelectSlot).nameText.text = gameData.userNameReference[currentNameReference - 2 + i].Name;
                                }
                                currentNameReference--;
                                currentNameSetMin--;
                                currentNameSetMax--;
                                if ((((selectedUserId - currentNameReference) + horizontalMenuSlots[currentHorizSlot].currentMenuSlot) < horizontalMenuSlots[currentHorizSlot].slotLimit && ((selectedUserId - currentNameReference) + horizontalMenuSlots[currentHorizSlot].currentMenuSlot) > 0))
                                {
                                    (horizontalMenuSlots[currentHorizSlot].subMenuSlots[(selectedUserId - currentNameReference) + horizontalMenuSlots[currentHorizSlot].currentMenuSlot] as UserSelectSlot).selectUpdate();
                                }
                            }
                            UpdateArrows();
                        }
                        else if (currentHorizSlot == 1)
                        {
                            sfxPlayer.PlaySound("Scroll");
                            horizontalMenuSlots[currentHorizSlot].currentMenuSlot -= 1;
                        }
                        horizontalMenuSlots[currentHorizSlot].subMenuSlots[horizontalMenuSlots[currentHorizSlot].currentMenuSlot].enabled = true;
                    }
                    scrollFrames = ((holdScroll) ? 5f : 20f);
                    holdScroll = true;
                }
                else if (inputReader.useNewInput("MoveRight", c_playerId) && scrollFrames <= 0 && (!inputReader.useNewInput("MoveUp", c_playerId) && !inputReader.useNewInput("MoveDown", c_playerId)))
                {
                    if (currentHorizSlot < (horizontalSlotLimit - 1))
                    {
                        sfxPlayer.PlaySound("Scroll");
                        horizontalMenuSlots[currentHorizSlot].subMenuSlots[horizontalMenuSlots[currentHorizSlot].currentMenuSlot].enabled = false;
                        if (horizontalMenuSlots[currentHorizSlot].currentMenuSlot == 0)
                        {
                            currentHorizSlot += 1;
                            horizontalMenuSlots[currentHorizSlot].currentMenuSlot = 0;
                        }
                        else
                        {
                            currentHorizSlot += 1;
                            horizontalMenuSlots[currentHorizSlot].currentMenuSlot = 1;
                        }
                        horizontalMenuSlots[currentHorizSlot].subMenuSlots[horizontalMenuSlots[currentHorizSlot].currentMenuSlot].enabled = true;
                    }
                    scrollFrames = ((holdScroll) ? 5f : 20f);
                    holdScroll = true;
                }
                else if (inputReader.useNewInput("MoveLeft", c_playerId) && scrollFrames <= 0 && (!inputReader.useNewInput("MoveUp", c_playerId) && !inputReader.useNewInput("MoveDown", c_playerId)))
                {
                    if (currentHorizSlot > 0)
                    {
                        sfxPlayer.PlaySound("Scroll");
                        horizontalMenuSlots[currentHorizSlot].subMenuSlots[horizontalMenuSlots[currentHorizSlot].currentMenuSlot].enabled = false;
                        currentHorizSlot -= 1;
                        horizontalMenuSlots[currentHorizSlot].subMenuSlots[horizontalMenuSlots[currentHorizSlot].currentMenuSlot].enabled = true;
                    }
                    scrollFrames = ((holdScroll) ? 5f : 20f);
                    holdScroll = true;
                }
                else
                {
                    if (inputReader.checkReleased(c_playerId))
                    {
                        scrollFrames = 0f;
                        holdScroll = false;
                    }
                }
            } else if (controlCode == 1)
            {
                newNameScreen.directionalController();
            }
            else if (controlCode == 2)
            {
                deleteUserScreen.directionalController();
            }
        }
        bufferOn = false;
    }

    public override void selectOption()
    {
        if (!bufferOn)
        {
            if (controlCode == 0)
            {
                bufferOn = true;
                if (currentHorizSlot == 0)
                {
                    bool skip = false;
                    if (currentNameReference != 0)
                    {
                        for (int i = 0; i < userMenuScreens.Length; i++)
                        {
                            if (userMenuScreens[i] != this)
                            {
                                if (userMenuScreens[i].selectedUserId == currentNameReference)
                                {
                                    skip = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (!skip)
                    {
                        sfxPlayer.PlaySound("Confirm");
                        for (int i = 0; i < horizontalMenuSlots[0].subMenuSlots.Length; i++)
                        {
                            (horizontalMenuSlots[0].subMenuSlots[i] as UserSelectSlot).deselectUpdate();
                        }
                        selectedUserId = currentNameReference;
                        csPlayerGUI.currentPlayerUserNameCode = selectedUserId;
                        if (selectedUserId != 0)
                        {
                            csPlayerGUI.playerIdentity = gameData.userNameReference[selectedUserId].Name;
                            csPlayerGUI.playerGUITextSize.PlayerIdentity = gameData.userNameReference[selectedUserId].Name;
                        }
                        else
                        {
                            csPlayerGUI.playerIdentity = csPlayerGUI.defaultPlayerIdentity;
                            csPlayerGUI.playerGUITextSize.PlayerIdentity = csPlayerGUI.defaultPlayerIdentity;
                        }
                        csPlayerGUI.refreshGUIOnce = true;
                        if ((((selectedUserId - currentNameReference) + horizontalMenuSlots[0].currentMenuSlot) < horizontalMenuSlots[0].slotLimit && ((selectedUserId - currentNameReference) + horizontalMenuSlots[0].currentMenuSlot) > 0) || currentNameReference == 0)
                        {
                            (horizontalMenuSlots[0].subMenuSlots[(selectedUserId - currentNameReference) + horizontalMenuSlots[0].currentMenuSlot] as UserSelectSlot).selectUpdate();
                        }
                        for (int i = 0; i < userMenuScreens.Length; i++)
                        {
                            if (userMenuScreens[i] != this)
                            {
                                userMenuScreens[i].bufferOn = true;
                            }
                        }
                    }
                }
                else if (currentHorizSlot == 1)
                {
                    if (horizontalMenuSlots[1].currentMenuSlot == 0)
                    {
                        sfxPlayer.PlaySound("Confirm");
                        controlCode = 1;
                        lockMenu();
                        newNameScreen.assignNewNameMenu(this);
                        newNameScreen.SetNameMode(SelectUserNewNameScreen.NameEditMode.EnterName);
                        newNameScreen.gameObject.SetActive(true);
                    }
                    else if (horizontalMenuSlots[1].currentMenuSlot == 1)
                    {
                        if (selectedUserId != 0)
                        {
                            sfxPlayer.PlaySound("Confirm");
                            controlCode = 1;
                            lockMenu();
                            newNameScreen.assignNewNameMenu(this);
                            newNameScreen.SetNameMode(SelectUserNewNameScreen.NameEditMode.EditName);
                            newNameScreen.gameObject.SetActive(true);
                        }
                    }
                    else if (horizontalMenuSlots[1].currentMenuSlot == 2)
                    {
                        if (selectedUserId != 0)
                        {
                            sfxPlayer.PlaySound("Confirm");
                            controlCode = 2;
                            lockMenu();
                            deleteUserScreen.assignYesNoMenu(this, 0);
                            deleteUserScreen.UpdateNameText(gameData.userNameReference[selectedUserId].Name);
                            deleteUserScreen.gameObject.SetActive(true);
                        }
                    }
                }
            }
            else if (controlCode == 1)
            {
                newNameScreen.selectOption();
            }
            else if (controlCode == 2)
            {
                deleteUserScreen.selectOption();
                if (controlCode == 0)
                {
                    unlockMenu();
                }
            }
        }
    }

    public override void closeMenu()
    {
        if (controlCode == 0)
        {
            sfxPlayer.PlaySound("Cancel");
            csPlayerGUI.bufferGUI = 30;
            currentPlayerInput.playerInputMode = "CharacterSelect";
            currentPlayerInput.enablePlayerInput = false;
            csPlayerGUI.refreshGUIOnce = true;
            resetScrollFrames();
            StartCoroutine("disableThisMenu", false);
        } else if (controlCode == 1)
        {
            newNameScreen.closeMenu();
        } else if (controlCode == 2)
        {
            deleteUserScreen.closeMenu();
            unlockMenu();
        }
    }

    public override void forceCloseMenu()
    {
        resetScrollFrames();
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, destroyEndMoveY, gameObject.transform.position.z);
        currentPlayerInput.playerHierarchy = 0;
        currentPlayerInput.setPlayerMenuMode("");
        csPlayerGUI.inUse = false;
        csPlayerGUI.refreshGUIOnce = true;
        if (controlCode == 1)
        {
            newNameScreen.forceCloseMenu();
        }
        if (controlCode == 2)
        {
            deleteUserScreen.forceCloseMenu();
        }
        controlCode = 0;
        gameObject.SetActive(false);
    }

    public IEnumerator summonThisMenu(float eMY)
    {
        float curPosY = gameObject.transform.position.y;
        while (curPosY < eMY)
        {
            curPosY += 20f;
            if (curPosY > eMY)
            {
                curPosY = eMY;
            }
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, curPosY, gameObject.transform.position.z);
            yield return null;
        }
        yield return new WaitForSeconds(0.4f);
        currentPlayerInput.enablePlayerInput = true;
        yield return null;
    }

    public IEnumerator disableThisMenu(bool selectLock)
    {
        float curPosY = gameObject.transform.position.y;
        while (curPosY > destroyEndMoveY)
        {
            curPosY -= 20f;
            if (curPosY < destroyEndMoveY)
            {
                curPosY = destroyEndMoveY;
            }
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, curPosY, gameObject.transform.position.z);
            yield return null;
        }
        yield return new WaitForSeconds(0.4f);
        currentPlayerInput.playerHierarchy = 0;
        currentPlayerInput.enablePlayerInput = true;
        currentPlayerInput.enableOtherPlayers(true);
        currentPlayerInput.setPlayerMenuMode("");
        csPlayerGUI.inUse = false;
        additionalCloseConditions(selectLock);
        gameObject.SetActive(false);
        yield return null;
    }

    public void lockMenu()
    {
        float curTint = 0.5f;
        SpriteRenderer spr = gameObject.GetComponent<SpriteRenderer>();
        spr.color = new Color(curTint, curTint, curTint, spr.color.a);
        TextMeshProUGUI[] children = GetComponentsInChildren<TextMeshProUGUI>();
        for (int i = 0; i < children.Length; i++)
        {
            children[i].color = new Color(0.5f, 0.5f, 0.5f, spr.color.a);
        }
    }

    public void unlockMenu()
    {
        float curTint = 1.0f;
        SpriteRenderer spr = gameObject.GetComponent<SpriteRenderer>();
        spr.color = new Color(curTint, curTint, curTint, spr.color.a);
        TextMeshProUGUI[] children = GetComponentsInChildren<TextMeshProUGUI>();
        for (int i = 0; i < children.Length; i++)
        {
            children[i].color = new Color(1f, 1f, 1f, spr.color.a);
        }
        controlCode = 0;
    }

    private void UpdateArrows()
    {
        arrowUp.enabled = (currentNameSetMin == 1) ? false : true;
        arrowDown.enabled = (currentNameSetMax >= gameData.idCounter - 1) ? false : true;
    }

    public void AddNewUser(string name)
    {
        System.Array.Resize<GameData.UserNameReference>(ref gameData.userNameReference, gameData.idCounter + 1);
        gameData.userNameReference[gameData.idCounter] = new GameData.UserNameReference(gameData.idCounter, name);
        gameData.UpdateIdCounter(1);
        gameData.SaveGame();
        selectedUserId = gameData.idCounter - 1;
        csPlayerGUI.playerIdentity = gameData.userNameReference[selectedUserId].Name;
        csPlayerGUI.playerGUITextSize.PlayerIdentity = gameData.userNameReference[selectedUserId].Name;
        for (int i = 0; i < horizontalMenuSlots[0].subMenuSlots.Length; i++)
        {
            (horizontalMenuSlots[0].subMenuSlots[i] as UserSelectSlot).deselectUpdate();
        }
        if (gameData.idCounter == 2)
        {
            horizontalMenuSlots[0].currentMenuSlot = 1;
            currentNameReference = 1;
            (horizontalMenuSlots[0].subMenuSlots[1] as UserSelectSlot).nameText.text = gameData.userNameReference[currentNameReference].Name;
            (horizontalMenuSlots[0].subMenuSlots[1] as UserSelectSlot).selectUpdate();
        } else if (gameData.idCounter == 3)
        {
            horizontalMenuSlots[0].currentMenuSlot = 2;
            currentNameReference = 2;
            (horizontalMenuSlots[0].subMenuSlots[2] as UserSelectSlot).nameText.text = gameData.userNameReference[currentNameReference].Name;
            (horizontalMenuSlots[0].subMenuSlots[2] as UserSelectSlot).selectUpdate();
        } else
        {
            horizontalMenuSlots[0].currentMenuSlot = 3;
            currentNameSetMax = gameData.idCounter - 1;
            currentNameSetMin = currentNameSetMax - 2;
            currentNameReference = currentNameSetMax;
            for (int i = 1; i < horizontalMenuSlots[0].slotLimit; i++)
            {
                (horizontalMenuSlots[0].subMenuSlots[i] as UserSelectSlot).nameText.text = gameData.userNameReference[currentNameSetMin - 1 + i].Name;
            }
            (horizontalMenuSlots[0].subMenuSlots[3] as UserSelectSlot).selectUpdate();
        }
        UpdateArrows();
        for (int i = 0; i < userMenuScreens.Length; i++)
        {
            if (userMenuScreens[i] != this)
            {
                userMenuScreens[i].bufferOn = true;
                userMenuScreens[i].UpdateMenu();
            }
        }
    }

    public void EditUser(string name)
    {
        gameData.userNameReference[selectedUserId].Name = name;
        gameData.SaveGame();
        csPlayerGUI.playerIdentity = gameData.userNameReference[selectedUserId].Name;
        csPlayerGUI.playerGUITextSize.PlayerIdentity = gameData.userNameReference[selectedUserId].Name;
        for (int i = 1; i < horizontalMenuSlots[0].slotLimit; i++)
        {
            if ((currentNameSetMin - 1 + i) < gameData.idCounter)
            {
                (horizontalMenuSlots[0].subMenuSlots[i] as UserSelectSlot).nameText.text = gameData.userNameReference[currentNameSetMin - 1 + i].Name;
            }
            else
            {
                (horizontalMenuSlots[0].subMenuSlots[i] as UserSelectSlot).nameText.text = "";
            }
        }
        for (int i = 0; i < userMenuScreens.Length; i++)
        {
            if (userMenuScreens[i] != this)
            {
                userMenuScreens[i].bufferOn = true;
                userMenuScreens[i].UpdateAfterEdit();
            }
        }
    }

    public void UpdateMenu()
    {
        if (this.gameObject.activeSelf == true)
        {
            if (gameData.idCounter == 2)
            {
                (horizontalMenuSlots[0].subMenuSlots[1] as UserSelectSlot).nameText.text = gameData.userNameReference[gameData.idCounter - 1].Name;
            }
            else if (gameData.idCounter == 3)
            {
                (horizontalMenuSlots[0].subMenuSlots[2] as UserSelectSlot).nameText.text = gameData.userNameReference[gameData.idCounter - 1].Name;
            }
            else if (gameData.idCounter == 4)
            {
                (horizontalMenuSlots[0].subMenuSlots[3] as UserSelectSlot).nameText.text = gameData.userNameReference[gameData.idCounter - 1].Name;
            }
            else
            {
                if (currentNameSetMax == gameData.idCounter - 2 && horizontalMenuSlots[0].currentMenuSlot == 3)
                {
                    horizontalMenuSlots[0].currentMenuSlot = 2;
                    currentNameSetMax = gameData.idCounter - 1;
                    currentNameSetMin = currentNameSetMax - 2;
                    currentNameReference = currentNameSetMax - 1;
                    for (int i = 0; i < horizontalMenuSlots[0].subMenuSlots.Length; i++)
                    {
                        (horizontalMenuSlots[0].subMenuSlots[i] as UserSelectSlot).deselectUpdate();
                    }
                    if (selectedUserId == 0)
                    {
                        (horizontalMenuSlots[0].subMenuSlots[0] as UserSelectSlot).selectUpdate();
                    }
                    else if (selectedUserId >= currentNameSetMin && selectedUserId <= currentNameSetMax)
                    {
                        (horizontalMenuSlots[0].subMenuSlots[(3 - currentNameSetMax + selectedUserId)] as UserSelectSlot).selectUpdate();
                    }
                    for (int i = 1; i < horizontalMenuSlots[0].slotLimit; i++)
                    {
                        if ((currentNameSetMin - 1 + i) < gameData.idCounter)
                        {
                            (horizontalMenuSlots[0].subMenuSlots[i] as UserSelectSlot).nameText.text = gameData.userNameReference[currentNameSetMin - 1 + i].Name;
                        }
                        else
                        {
                            (horizontalMenuSlots[0].subMenuSlots[i] as UserSelectSlot).nameText.text = "";
                        }
                    }
                    if (currentHorizSlot == 0)
                    {
                        horizontalMenuSlots[0].subMenuSlots[3].enabled = false;
                        horizontalMenuSlots[0].subMenuSlots[2].enabled = true;
                    }
                }
            }
            UpdateArrows();
        }
        for (int i = 1; i < horizontalMenuSlots[0].slotLimit; i++)
        {
            if ((currentNameSetMin - 1 + i) < gameData.idCounter)
            {
                (horizontalMenuSlots[0].subMenuSlots[i] as UserSelectSlot).nameText.text = gameData.userNameReference[currentNameSetMin - 1 + i].Name;
            }
            else
            {
                (horizontalMenuSlots[0].subMenuSlots[i] as UserSelectSlot).nameText.text = "";
            }
        }
    }

    private void UpdateAfterEdit()
    {
        for (int i = 1; i < horizontalMenuSlots[0].slotLimit; i++)
        {
            if ((currentNameSetMin - 1 + i) < gameData.idCounter)
            {
                (horizontalMenuSlots[0].subMenuSlots[i] as UserSelectSlot).nameText.text = gameData.userNameReference[currentNameSetMin - 1 + i].Name;
            }
            else
            {
                (horizontalMenuSlots[0].subMenuSlots[i] as UserSelectSlot).nameText.text = "";
            }
        }
    }

    public void UpdateAfterDelete(int removedId)
    {
        if (selectedUserId > removedId)
        {
            selectedUserId--;
        }
        if (this.gameObject.activeSelf == true)
        {
            for (int i = 0; i < horizontalMenuSlots[0].subMenuSlots.Length; i++)
            {
                (horizontalMenuSlots[0].subMenuSlots[i] as UserSelectSlot).deselectUpdate();
            }
            if (currentHorizSlot == 0)
            {
                horizontalMenuSlots[0].subMenuSlots[horizontalMenuSlots[0].currentMenuSlot].enabled = false;
                horizontalMenuSlots[0].subMenuSlots[0].enabled = true;
            }
            horizontalMenuSlots[0].currentMenuSlot = 0;
            currentNameSetMax = 3;
            currentNameSetMin = 1;
            currentNameReference = currentNameSetMax;
            currentNameReference = 0;
            if (selectedUserId == 0)
            {
                (horizontalMenuSlots[0].subMenuSlots[0] as UserSelectSlot).selectUpdate();
            }
            else if (selectedUserId >= currentNameSetMin && selectedUserId <= currentNameSetMax)
            {
                (horizontalMenuSlots[0].subMenuSlots[(3 - currentNameSetMax + selectedUserId)] as UserSelectSlot).selectUpdate();
            }
            for (int i = 1; i < horizontalMenuSlots[0].slotLimit; i++)
            {
                if (i < gameData.userNameReference.Length)
                {
                    (horizontalMenuSlots[0].subMenuSlots[i] as UserSelectSlot).nameText.text = gameData.userNameReference[i].Name;
                }
                else
                {
                    (horizontalMenuSlots[0].subMenuSlots[i] as UserSelectSlot).nameText.text = "";
                }
            }
            UpdateArrows();
        }
        UpdateAfterEdit();
    }

    public void DeleteUser()
    {
        int removedId = selectedUserId;
        gameData.userNameReference[selectedUserId] = null;
        for (int i = selectedUserId; i < gameData.userNameReference.Length - 1; i++)
        {
            gameData.userNameReference[i] = new GameData.UserNameReference(i, gameData.userNameReference[i+1].Name);
        }
        gameData.userNameReference[gameData.userNameReference.Length - 1] = null;
        System.Array.Resize<GameData.UserNameReference>(ref gameData.userNameReference, gameData.idCounter - 1);
        gameData.UpdateIdCounter(-1);
        gameData.SaveGame();
        selectedUserId = 0;
        csPlayerGUI.playerIdentity = csPlayerGUI.defaultPlayerIdentity;
        csPlayerGUI.playerGUITextSize.PlayerIdentity = gameData.userNameReference[selectedUserId].Name;
        for (int i = 0; i < horizontalMenuSlots[0].subMenuSlots.Length; i++)
        {
            (horizontalMenuSlots[0].subMenuSlots[i] as UserSelectSlot).deselectUpdate();
        }
        horizontalMenuSlots[0].currentMenuSlot = 0;
        currentNameSetMax = 3;
        currentNameSetMin = 1;
        currentNameReference = currentNameSetMax;
        currentNameReference = 0;
        (horizontalMenuSlots[0].subMenuSlots[0] as UserSelectSlot).selectUpdate();
        for (int i = 1; i < horizontalMenuSlots[0].slotLimit; i++)
        {
            if (i < gameData.userNameReference.Length)
            {
                (horizontalMenuSlots[0].subMenuSlots[i] as UserSelectSlot).nameText.text = gameData.userNameReference[i].Name;
            } else
            {
                (horizontalMenuSlots[0].subMenuSlots[i] as UserSelectSlot).nameText.text = "";
            }
        }
        UpdateArrows();
        for (int i = 0; i < userMenuScreens.Length; i++)
        {
            if (userMenuScreens[i] != this)
            {
                userMenuScreens[i].bufferOn = true;
                userMenuScreens[i].UpdateAfterDelete(removedId);
            }
        }
    }
}
