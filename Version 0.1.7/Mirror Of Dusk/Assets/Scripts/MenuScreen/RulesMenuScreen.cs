using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEditor;
using TMPro;
using Rewired;

public class RulesMenuScreen : VerticalMenuScreenB {

    private CharacterSelectManager characterSelectManager;

    private CSPlayerGUI[] csPlayerGUI;
    private CSPlayerInput[] csPlayerInputs;
    private CSPlayerData[] csPlayerDatas;
    private class RulesMenuArrows
    {
        public bool exists = false;
        public AnimateGlowArrows animateLeft;
        public AnimateGlowArrows animateRight;
    }
    private List<RulesMenuArrows> rulesMenuArrows;
    private SpriteRenderer arrowMoreUp;
    private SpriteRenderer arrowMoreDown;
    private CSReadTitleText csRTT;
    private int slotMove = 1;
    private RulesMenuShardSettingsScreen shardSettingsMenu;

    [Header("Sprite Prefabs")]
    public GameObject _menuScreenMoreUp;
    public GameObject _menuScreenMoreDown;

    void Awake()
    {
        setUpMenu();
        initializeVerticalScreen();
        initializeRulesMenuScreen();
    }

    // Use this for initialization
    void Start()
    {
        scriptOn = true;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        checkForHorizontalScroll();
        checkForVerticalScroll();
    }

    public override void OnEnable()
    {
        if (scriptOn)
        {
            if (controlCode == 0)
            {
                if (menuDetailsRoot != null)
                {
                    menuDetailsRoot.updateDetails(menuItemCodes[currentMenuVCode].menuDetailSection);
                }
                resetHighlighter();
                if (currentMenuVCode <= 0)
                {
                    arrowMoreUp.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                    arrowMoreUp.enabled = false;
                }
                if (currentMenuVCode >= menuItemCodes.Length - 1)
                {
                    arrowMoreDown.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                    arrowMoreDown.enabled = false;
                }
                StartCoroutine("summonMenu", endMoveY);
            } else if (controlCode == 1)
            {

            }
        }
    }

    protected override void setUpMenu()
    {
        characterSelectManager = CharacterSelectManager.characterSelectManager;

        menuName = gameObject.name.Substring(0, gameObject.name.IndexOf("Screen"));
        sfxPlayer = characterSelectManager.sfxPlayer.GetComponent<SFXPlayer>();
        gameData = characterSelectManager.gameData.GetComponent<GameData>();
        try
        {
            menuDetailsRoot = gameObject.GetComponentInChildren<MenuDetailsRoot>();
        }
        catch (System.NullReferenceException)
        {
            
        }
        s_player = ReInput.players.GetPlayer(4);
        systemInputReader = characterSelectManager.eventSystem.GetComponent<SystemInputReader>();
        //initializeCharacterCodes();
        csPlayerInputs = new CSPlayerInput[4];
        csPlayerDatas = new CSPlayerData[4];
        for (int i = 0; i < 4; i++)
        {
            csPlayerInputs[i] = characterSelectManager.players[i].GetComponent<CSPlayerInput>();
            csPlayerDatas[i] = characterSelectManager.players[i].GetComponent<CSPlayerData>();
        }
    }

    private void initializeRulesMenuScreen()
    {
        csPlayerGUI = new CSPlayerGUI[4];
        for (int i = 0; i < 4; i++)
        {
            csPlayerGUI[i] = characterSelectManager.csPlayerGUI.transform.GetChild(i).GetComponent<CSPlayerGUI>();
        }
        shardSettingsMenu = characterSelectManager.shardSettingsRulesScreen.GetComponent<RulesMenuShardSettingsScreen>();
        shardSettingsMenu.assignRootMenu(this);
        shardSettingsMenu.gameObject.transform.position = new Vector3(0, 0, shardSettingsMenu.gameObject.transform.position.z);
        rulesMenuArrows = new List<RulesMenuArrows>();
        csRTT = characterSelectManager.csTitleBase.GetComponentInChildren<CSReadTitleText>();
        for (int i = 0; i < menuSlots.Length; i++)
        {
            rulesMenuArrows.Add(new RulesMenuArrows { exists = false, animateLeft = null, animateRight = null });
            foreach (Transform child in menuSlots[i].transform)
            {
                if (child.gameObject.name == "MenuScreenArrows")
                {
                    rulesMenuArrows[i].exists = true;
                    rulesMenuArrows[i].animateLeft = child.gameObject.transform.GetChild(0).GetComponent<AnimateGlowArrows>();
                    rulesMenuArrows[i].animateRight = child.gameObject.transform.GetChild(1).GetComponent<AnimateGlowArrows>();
                    bool current = false;
                    if (i == currentMenuVCode)
                    {
                        current = true;
                    }
                    if (current)
                    {
                        rulesMenuArrows[i].animateLeft.setAnimation(1);
                        rulesMenuArrows[i].animateRight.setAnimation(1);
                    }
                    if (menuItemCodes[i].currentMenuHCode <= 0)
                    {
                        rulesMenuArrows[i].animateLeft.setAnimation(0);
                    }
                    if (menuItemCodes[i].currentMenuHCode >= menuItemCodes[i].menuItemMax - 1)
                    {
                        rulesMenuArrows[i].animateRight.setAnimation(0);
                    }
                }
            }
        }
        arrowMoreUp = _menuScreenMoreUp.GetComponent<SpriteRenderer>();
        arrowMoreDown = _menuScreenMoreDown.GetComponent<SpriteRenderer>();
    }

    public override void assignPlayer(int playerCode)
    {
        currentPlayerInput = csPlayerInputs[playerCode];
    }

    protected override void updateVerticalScroll()
    {
        if (rulesMenuArrows[prevMenuVCode].exists)
        {
            rulesMenuArrows[prevMenuVCode].animateLeft.setAnimation(2);
            rulesMenuArrows[prevMenuVCode].animateRight.setAnimation(2);
            if (menuItemCodes[prevMenuVCode].currentMenuHCode <= 0)
            {
                rulesMenuArrows[prevMenuVCode].animateLeft.setAnimation(0);
            }
            if (menuItemCodes[prevMenuVCode].currentMenuHCode >= menuItemCodes[prevMenuVCode].menuItemMax - 1)
            {
                rulesMenuArrows[prevMenuVCode].animateRight.setAnimation(0);
            }
        }
        summonUI();
        int prevVSlot = prevMenuVCode;
        prevMenuVCode = currentMenuVCode;
        if (rulesMenuArrows[currentMenuVCode].exists)
        {
            rulesMenuArrows[currentMenuVCode].animateLeft.setAnimation(1);
            rulesMenuArrows[currentMenuVCode].animateRight.setAnimation(1);
            if (menuItemCodes[currentMenuVCode].currentMenuHCode <= 0)
            {
                rulesMenuArrows[currentMenuVCode].animateLeft.setAnimation(0);
            }
            if (menuItemCodes[currentMenuVCode].currentMenuHCode >= menuItemCodes[currentMenuVCode].menuItemMax - 1)
            {
                rulesMenuArrows[currentMenuVCode].animateRight.setAnimation(0);
            }
        }
        if (useSlotLimits)
        {
            int prevCVSlot = currentVSlot;
            if ((prevVSlot < prevMenuVCode && !overRun) || (prevMenuVCode < prevVSlot && overRun))
            {
                currentVSlot = currentVSlot + slotMove;
            }
            else if ((prevVSlot > prevMenuVCode && !overRun) || (prevMenuVCode > prevVSlot && overRun))
            {
                currentVSlot = currentVSlot - slotMove;
            }
            if (currentVSlot >= slotLimit && !overRun)
            {
                currentSlotPosY -= (menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.height * (slotLimit - prevCVSlot - 1));
                currentVSlot = slotLimit - 1;
                if (coRountineActive)
                {
                    StopCoroutine("scrollSlots");
                    scrollDownWait--;
                    coRountineActive = false;
                }
                scrollDownWait++;
                StartCoroutine("scrollSlots", true);

                arrowMoreUp.enabled = true;
                arrowMoreUp.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                if (currentMenuVCode >= menuItemCodes.Length - 1)
                {
                    arrowMoreDown.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                    arrowMoreDown.enabled = false;
                }
            }
            else if (currentVSlot < 0 && !overRun)
            {
                currentSlotPosY += (menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.height * (slotLimit - (slotLimit - prevCVSlot)));
                currentVSlot = 0;
                if (coRountineActive)
                {
                    StopCoroutine("scrollSlots");
                    scrollUpWait--;
                    coRountineActive = false;
                }
                scrollUpWait++;
                StartCoroutine("scrollSlots", false);
                arrowMoreDown.enabled = true;
                arrowMoreDown.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                if (currentMenuVCode <= 0)
                {
                    arrowMoreUp.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                    arrowMoreUp.enabled = false;
                }
            }
            else if (currentVSlot <= 0 && overRun)
            {
                currentVSlot = slotLimit - 1;
                if (coRountineActive)
                {
                    StopCoroutine("scrollSlots");
                    scrollUpWait--;
                    coRountineActive = false;
                }
                scrollUpWait++;
                summonReset(false);
                arrowMoreUp.enabled = true;
                arrowMoreUp.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                arrowMoreDown.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                arrowMoreDown.enabled = false;
            }
            else if (currentVSlot > 0 && overRun)
            {
                currentVSlot = 0;
                if (coRountineActive)
                {
                    StopCoroutine("scrollSlots");
                    scrollDownWait--;
                    coRountineActive = false;
                }
                scrollDownWait++;
                summonReset(true);
                arrowMoreDown.enabled = true;
                arrowMoreDown.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                arrowMoreUp.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                arrowMoreUp.enabled = false;
            }
            else
            {
                if (prevVSlot < prevMenuVCode)
                {
                    //currentSlotPosY -= ((menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.height * slotMove) + ((slotLimit) - currentVSlot));
                    currentSlotPosY -= (menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.height * slotMove);
                }
                else if (prevVSlot > prevMenuVCode)
                {
                    //currentSlotPosY += ((menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.height * (slotMove - ((currentVSlot + slotLimit) - (slotLimit)))));
                    currentSlotPosY += (menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.height * slotMove);
                }
            }
        }
        overRun = false;
        if (menuDetailsRoot != null)
        {
            menuDetailsRoot.updateDetails(menuItemCodes[currentMenuVCode].menuDetailSection);
        }
    }

    protected override void updateHorizontalScroll()
    {
        prevMenuItemCodes[currentMenuVCode] = menuItemCodes[currentMenuVCode].currentMenuHCode;
        switch (menuItemCodes[currentMenuVCode].SlotType)
        {
            case ShowValueEnum.Access:
                break;
            case ShowValueEnum.Confirm:
                break;
            case ShowValueEnum.Single:
                menuSlots[currentMenuVCode].transform.Find("OptionChoice").GetComponent<TextMeshProUGUI>().text = menuItemCodes[currentMenuVCode].slotValueSingle[menuItemCodes[currentMenuVCode].currentMenuHCode];
                //menuSlots[currentMenuVCode].GetComponent<UI_InputTrigger>().InvokeUpdateData();
                break;
            case ShowValueEnum.RangeInt:
                menuSlots[currentMenuVCode].transform.Find("OptionChoice").GetComponent<TextMeshProUGUI>().text = (menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)).ToString();
                fixNumberName(currentMenuVCode);
                try {
                    menuSlots[currentMenuVCode].GetComponent<UI_InputTrigger>().InvokeUpdateData();
                } catch (System.NullReferenceException)
                {

                }
                break;
            case ShowValueEnum.RangeFloat:
                menuSlots[currentMenuVCode].transform.Find("OptionChoice").GetComponent<TextMeshProUGUI>().text = (menuItemCodes[currentMenuVCode].slotValueFloat + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementFloat)).ToString("F1");
                //menuSlots[currentMenuVCode].GetComponent<UI_InputTrigger>().InvokeUpdateData();
                break;
            default:
                break;
        }
        if (rulesMenuArrows[currentMenuVCode].exists)
        {

        }
        if (rulesMenuArrows[currentMenuVCode].animateLeft.animState != 1)
        {
            rulesMenuArrows[currentMenuVCode].animateLeft.setAnimation(1);
            if (rulesMenuArrows[currentMenuVCode].animateRight.animState == 1)
            {
                int tempFrame = rulesMenuArrows[currentMenuVCode].animateRight.getFrame();
                int tempSpeed = rulesMenuArrows[currentMenuVCode].animateRight.getSpeed();
                rulesMenuArrows[currentMenuVCode].animateLeft.setFrame(tempFrame, tempSpeed);
            }
        }
        if (rulesMenuArrows[currentMenuVCode].animateRight.animState != 1)
        {
            rulesMenuArrows[currentMenuVCode].animateRight.setAnimation(1);
            if (rulesMenuArrows[currentMenuVCode].animateLeft.animState == 1)
            {
                int tempFrame = rulesMenuArrows[currentMenuVCode].animateLeft.getFrame();
                int tempSpeed = rulesMenuArrows[currentMenuVCode].animateLeft.getSpeed();
                rulesMenuArrows[currentMenuVCode].animateRight.setFrame(tempFrame, tempSpeed);
            }
        }
        if (menuItemCodes[currentMenuVCode].currentMenuHCode <= 0)
        {
            rulesMenuArrows[currentMenuVCode].animateLeft.setAnimation(0);
        }
        if (menuItemCodes[currentMenuVCode].currentMenuHCode >= menuItemCodes[currentMenuVCode].menuItemMax - 1)
        {
            rulesMenuArrows[currentMenuVCode].animateRight.setAnimation(0);
        }
        overRunH = false;
    }

    public override IEnumerator scrollSlots(bool down)
    {
        coRountineActive = true;
        int safeCount = 0;
        float destination;
        float moveSpeedY;
        float posY;
        Vector3 curPos;
        if (down)
        {
            float dY1 = (initialSlotPosY - (initialSlotPosY - currentSlotPosY) - ((menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.height * (currentMenuVCode - (slotLimit - 1)))) - (currentSlotPosY - initialSlotPosY));
            float dY2 = currentSlotPosY;
            destination = dY1 - dY2;
            curPos = mainLayout.gameObject.transform.position;
            posY = curPos.y;
            destination = destination + currentSlotPosY;
            moveSpeedY = (currentSlotPosY - destination) / 3f;
        }
        else
        {
            //float upInit = initialSlotPosY + ((menuItemCodes.Length - 1) * menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.height) - (menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.height * (slotLimit - 1));
            //float upInit = ((menuSlots[slotLimit - 1].transform.parent.transform.position.y + (menuSlots[slotLimit - 1].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.y - (menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.height * (slotLimit - 1)));
            float upInit = (initialSlotPosY + (menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.height * (slotLimit - 1))) - (menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.height * (menuItemCodes.Length - 1)) + (menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.height * (slotLimit - 1));
            float dY1 = (upInit + (upInit - currentSlotPosY) + ((menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.height * (menuItemCodes.Length - (slotLimit) - currentMenuVCode))) + (currentSlotPosY - upInit));
            float dY2 = currentSlotPosY;
            destination = dY1 - dY2;
            curPos = mainLayout.gameObject.transform.position;
            posY = curPos.y;
            destination = currentSlotPosY + destination;
            moveSpeedY = (currentSlotPosY - destination) / 3f;
        }
        if (down)
        {
            while (currentSlotPosY > destination && safeCount < 100)
            {
                int tempMoveSpeedY = Mathf.RoundToInt(moveSpeedY);
                moveSpeedY = System.Convert.ToSingle(tempMoveSpeedY);
                posY = posY + moveSpeedY;
                currentSlotPosY = currentSlotPosY - moveSpeedY;
                if (moveSpeedY < 1.0f)
                {
                    moveSpeedY = 1.0f;
                }
                float sHSpeedY = moveSpeedY;
                moveSpeedY = moveSpeedY * 0.9f;
                if (currentSlotPosY <= destination)
                {
                    sHSpeedY += (currentSlotPosY - destination);
                    posY += (currentSlotPosY - destination);
                    currentSlotPosY = destination;
                }
                mainLayout.gameObject.transform.position = new Vector3(curPos.x, posY, curPos.z);
                foreach (GameObject sH in screenHighlighter)
                {
                    try
                    {
                        sH.transform.position = new Vector3(sH.transform.position.x, sH.transform.position.y + sHSpeedY, sH.transform.position.z);
                    }
                    catch (MissingReferenceException)
                    {

                    }
                }
                safeCount++;
                yield return null;
            }
            scrollDownWait--;
        }
        else
        {
            while (currentSlotPosY < destination && safeCount < 100)
            {
                int tempMoveSpeedY = Mathf.RoundToInt(moveSpeedY);
                moveSpeedY = System.Convert.ToSingle(tempMoveSpeedY);
                posY = posY + moveSpeedY;
                currentSlotPosY = currentSlotPosY - moveSpeedY;
                if (moveSpeedY > -1.0f)
                {
                    moveSpeedY = -1.0f;
                }
                float sHSpeedY = moveSpeedY;
                moveSpeedY = moveSpeedY * 0.9f;
                if (currentSlotPosY >= destination)
                {
                    sHSpeedY -= (destination - currentSlotPosY);
                    posY -= (destination - currentSlotPosY);
                    currentSlotPosY = destination;
                }
                mainLayout.gameObject.transform.position = new Vector3(curPos.x, posY, curPos.z);
                foreach (GameObject sH in screenHighlighter)
                {
                    try
                    {
                        sH.transform.position = new Vector3(sH.transform.position.x, sH.transform.position.y + sHSpeedY, sH.transform.position.z);
                    }
                    catch (MissingReferenceException)
                    {

                    }
                }
                safeCount++;
                yield return null;
            }
            scrollUpWait--;
        }
        coRountineActive = false;
        yield return null;
    }

    protected override void summonReset(bool down)
    {
        if (down)
        {
            Vector3 curPos = mainLayout.gameObject.transform.position;
            float posY = curPos.y;
            mainLayout.gameObject.transform.position = new Vector3(curPos.x, initialLayoutPosY, curPos.z);
            float posYb = posY - initialLayoutPosY;
            if (useScreenHighlighter)
            {
                foreach (GameObject sH in screenHighlighter)
                {
                    try
                    {
                        float destination = sH.transform.position.y - posYb;
                        sH.transform.position = new Vector3(sH.transform.position.x, destination, sH.transform.position.z);
                    }
                    catch (MissingReferenceException)
                    {

                    }
                }
            }
            currentSlotPosY = (menuSlots[slotLimit - 1].transform.parent.transform.position.y + (menuSlots[slotLimit - 1].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.y;
            scrollDownWait--;
        }
        else
        {
            Vector3 curPos = mainLayout.gameObject.transform.position;
            float posY = curPos.y;
            mainLayout.gameObject.transform.position = new Vector3(curPos.x, initialLayoutPosY + (menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.height * (menuItemCodes.Length - slotLimit)), curPos.z);
            float posYb = (initialLayoutPosY + (menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.height * (menuItemCodes.Length - slotLimit))) - posY;
            if (useScreenHighlighter)
            {
                foreach (GameObject sH in screenHighlighter)
                {
                    try
                    {
                        float destination = sH.transform.position.y + posYb;
                        sH.transform.position = new Vector3(sH.transform.position.x, destination, sH.transform.position.z);
                    }
                    catch (MissingReferenceException)
                    {

                    }
                }
            }
            currentSlotPosY = (initialSlotPosY) + (menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.height * (slotLimit - 1)) - (menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.height * (menuItemCodes.Length - 1));
            scrollUpWait--;
        }
    }

    public override void selectOption()
    {
        if (controlCode == 0)
        {
            if (menuItemCodes[currentMenuVCode].SlotType == ShowValueEnum.Confirm || menuItemCodes[currentMenuVCode].SlotType == ShowValueEnum.Access)
            {
                /*EventSystem.current.SetSelectedGameObject(menuSlots[currentMenuVCode]);
                EventSystem.current.SetSelectedGameObject(null);*/
                if (currentMenuVCode == 7)
                {
                    sfxPlayer.PlaySound("Confirm");
                    currentPlayerInput.enablePlayerInput = false;
                    controlCode = 1;
                    StartCoroutine("lockMenu");
                    shardSettingsMenu.assignPlayer(currentPlayerInput.playerID);
                    shardSettingsMenu.assignRulesRootMenu(this);
                    shardSettingsMenu.gameObject.SetActive(true);
                }
                else if (currentMenuVCode == 15)
                {
                    closeMenu();
                }
            }
            else if (menuItemCodes[currentMenuVCode].SlotType == ShowValueEnum.RangeInt)
            {
                if (currentMenuVCode == 6)
                {
                    sfxPlayer.PlaySound("Confirm");
                    for (int i = 0; i < csPlayerDatas.Length; i++)
                    {
                        csPlayerDatas[i].playerHealth = gameData.defaultInitialHealth;
                    }
                }
            }
        }
        else if (controlCode == 1)
        {
            shardSettingsMenu.selectOption();
        }
    }

    public override void closeMenu()
    {
        if (controlCode == 0)
        {
            sfxPlayer.PlaySound("Cancel");
            currentPlayerInput.playerInputMode = "CharacterSelect";
            currentPlayerInput.enablePlayerInput = false;
            resetScrollFrames();
            StartCoroutine("disableMenu", false);
        } else if (controlCode == 1)
        {
            shardSettingsMenu.closeMenu();
        }
    }

    public override void directionalController()
    {
        if (controlCode == 0)
        {
            //padHorizontal = Input.GetAxisRaw("Horizontal_" + pMenuCode);
            //padVertical = Input.GetAxisRaw("Vertical_" + pMenuCode);
            padHorizontal = s_player.GetAxisRaw("UI_Horizontal");
            padVertical = s_player.GetAxisRaw("UI_Vertical");
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
            scrollTime++;
            if (scrollFrames <= 0)
            {
                scrollFrames = 0f;
            }
            if (systemInputReader.useNewInput("MoveLeft") && scrollFrames <= 0)
            {
                sfxPlayer.PlaySound("Scroll");
                if (menuItemCodes[currentMenuVCode].SlotType == ShowValueEnum.RangeInt)
                {
                    if (menuItemCodes[currentMenuVCode].currentMenuHCode == 0)
                    {
                        menuItemCodes[currentMenuVCode].currentMenuHCode = 0;
                    }
                    else
                    {
                        if (scrollTime > 60)
                        {
                            if (scrollTime > 120 && menuItemCodes[currentMenuVCode].menuItemMax >= 500)
                            {
                                if (menuItemCodes[currentMenuVCode].currentMenuHCode >= menuItemCodes[currentMenuVCode].menuItemMax - 1)
                                {
                                    int fixCount = 0;
                                    float numDec = 0;
                                    bool multFixed = false;
                                    if (menuItemCodes[currentMenuVCode].slotIncrementInt >= 10)
                                    {
                                        while ((((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) - numDec) % 1000 != 0) && fixCount < 1000)
                                        {
                                            if ((((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) - numDec) % 100 == 0))
                                            {
                                                numDec += 100f;
                                            }
                                            else if ((((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) - numDec) % 10 == 0))
                                            {
                                                numDec += 10f;
                                            }
                                            else
                                            {
                                                numDec += 1f;
                                            }
                                            if ((((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) - numDec) % 1000 == 0))
                                            {
                                                multFixed = true;
                                            }
                                            fixCount++;
                                        }
                                        if (multFixed)
                                        {
                                            menuItemCodes[currentMenuVCode].currentMenuHCode -= (int)numDec / 10;
                                        }
                                        else
                                        {
                                            menuItemCodes[currentMenuVCode].currentMenuHCode -= 100;
                                        }
                                    }
                                    else
                                    {
                                        while ((((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) - numDec) % 100 != 0) && fixCount < 100)
                                        {
                                            if ((((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) - numDec) % 10 == 0))
                                            {
                                                numDec += 10f;
                                            }
                                            else
                                            {
                                                numDec++;
                                            }
                                            if ((((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) - numDec) % 100 == 0))
                                            {
                                                multFixed = true;
                                            }
                                            fixCount++;
                                        }
                                        if (multFixed)
                                        {
                                            menuItemCodes[currentMenuVCode].currentMenuHCode -= (int)numDec;
                                        }
                                        else
                                        {
                                            menuItemCodes[currentMenuVCode].currentMenuHCode -= 100;
                                        }
                                    }
                                }
                                else if ((menuItemCodes[currentMenuVCode].slotIncrementInt >= 10 && ((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) % 1000 == 0)) || (menuItemCodes[currentMenuVCode].slotIncrementInt < 10 && ((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) % 100 == 0)))
                                {
                                    menuItemCodes[currentMenuVCode].currentMenuHCode -= 100;
                                }
                                else
                                {
                                    menuItemCodes[currentMenuVCode].currentMenuHCode -= 10;
                                }
                            }
                            else
                            {
                                if (menuItemCodes[currentMenuVCode].currentMenuHCode >= menuItemCodes[currentMenuVCode].menuItemMax - 1)
                                {
                                    int fixCount = 0;
                                    int numDec = 0;
                                    bool multFixed = false;
                                    if (menuItemCodes[currentMenuVCode].slotIncrementInt >= 10)
                                    {
                                        while ((((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) - numDec) % 100 != 0) && fixCount < 100)
                                        {
                                            if ((((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) - numDec) % 10 == 0))
                                            {
                                                numDec += 10;
                                            }
                                            else
                                            {
                                                numDec++;
                                            }
                                            if ((((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) - numDec) % 100 == 0))
                                            {
                                                multFixed = true;
                                            }
                                            fixCount++;
                                        }
                                        if (multFixed)
                                        {
                                            menuItemCodes[currentMenuVCode].currentMenuHCode -= (numDec / 10);
                                        }
                                        else
                                        {
                                            menuItemCodes[currentMenuVCode].currentMenuHCode -= 1;
                                        }
                                    }
                                    else
                                    {
                                        while ((((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) - numDec) % 10 != 0) && fixCount < 20)
                                        {
                                            numDec++;
                                            if ((((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) - numDec) % 10 == 0))
                                            {
                                                multFixed = true;
                                            }
                                            fixCount++;
                                        }
                                        if (multFixed)
                                        {
                                            menuItemCodes[currentMenuVCode].currentMenuHCode -= numDec;
                                        }
                                        else
                                        {
                                            menuItemCodes[currentMenuVCode].currentMenuHCode -= 10;
                                        }
                                    }
                                }
                                else if ((menuItemCodes[currentMenuVCode].slotIncrementInt >= 10 && ((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) % 100 == 0)) || (menuItemCodes[currentMenuVCode].slotIncrementInt < 10 && ((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) % 10 == 0)))
                                {
                                    menuItemCodes[currentMenuVCode].currentMenuHCode -= 10;
                                }
                                else
                                {
                                    menuItemCodes[currentMenuVCode].currentMenuHCode -= 1;
                                }
                            }
                        }
                        else
                        {
                            menuItemCodes[currentMenuVCode].currentMenuHCode -= 1;
                        }
                        if (menuItemCodes[currentMenuVCode].currentMenuHCode <= 0)
                        {
                            menuItemCodes[currentMenuVCode].currentMenuHCode = 0;
                        }
                    }
                }
                else
                {
                    if (menuItemCodes[currentMenuVCode].currentMenuHCode == 0)
                    {
                        menuItemCodes[currentMenuVCode].currentMenuHCode = 0;
                    }
                    else
                    {
                        menuItemCodes[currentMenuVCode].currentMenuHCode -= 1;
                    }
                    if (menuItemCodes[currentMenuVCode].currentMenuHCode < 0)
                    {
                        menuItemCodes[currentMenuVCode].currentMenuHCode = 0;
                    }
                }
                scrollFrames = ((holdScroll) ? 5f : 20f);
                holdScroll = true;
            }
            else if (systemInputReader.useNewInput("MoveRight") && scrollFrames <= 0)
            {
                sfxPlayer.PlaySound("Scroll");
                if (menuItemCodes[currentMenuVCode].SlotType == ShowValueEnum.RangeInt)
                {
                    if (menuItemCodes[currentMenuVCode].currentMenuHCode == menuItemCodes[currentMenuVCode].menuItemMax - 1)
                    {
                        menuItemCodes[currentMenuVCode].currentMenuHCode = menuItemCodes[currentMenuVCode].menuItemMax - 1;
                    }
                    else
                    {
                        if (scrollTime > 60)
                        {
                            if (scrollTime > 120 && menuItemCodes[currentMenuVCode].menuItemMax >= 500)
                            {
                                if (menuItemCodes[currentMenuVCode].currentMenuHCode <= 0)
                                {
                                    int fixCount = 0;
                                    float numDec = 0;
                                    bool multFixed = false;
                                    if (menuItemCodes[currentMenuVCode].slotIncrementInt >= 10)
                                    {
                                        while ((((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) + numDec) % 1000 != 0) && fixCount < 1000)
                                        {
                                            if ((((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) + numDec) % 100 == 0))
                                            {
                                                numDec += 100f;
                                            }
                                            else if ((((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) + numDec) % 10 == 0))
                                            {
                                                numDec += 10f;
                                            }
                                            else
                                            {
                                                numDec += 1f;
                                            }
                                            if ((((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) + numDec) % 1000 == 0))
                                            {
                                                multFixed = true;
                                            }
                                            fixCount++;
                                        }
                                        if (multFixed)
                                        {
                                            menuItemCodes[currentMenuVCode].currentMenuHCode += (int)numDec / 10;
                                        }
                                        else
                                        {
                                            menuItemCodes[currentMenuVCode].currentMenuHCode += 100;
                                        }
                                    }
                                    else
                                    {
                                        while ((((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) + numDec) % 100 != 0) && fixCount < 100)
                                        {
                                            if ((((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) + numDec) % 10 == 0))
                                            {
                                                numDec += 10f;
                                            }
                                            else
                                            {
                                                numDec++;
                                            }
                                            if ((((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) + numDec) % 100 == 0))
                                            {
                                                multFixed = true;
                                            }
                                            fixCount++;
                                        }
                                        if (multFixed)
                                        {
                                            menuItemCodes[currentMenuVCode].currentMenuHCode += (int)numDec;
                                        }
                                        else
                                        {
                                            menuItemCodes[currentMenuVCode].currentMenuHCode += 100;
                                        }
                                    }
                                }
                                else if ((menuItemCodes[currentMenuVCode].slotIncrementInt >= 10 && ((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) % 1000 == 0)) || (menuItemCodes[currentMenuVCode].slotIncrementInt < 10 && ((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) % 100 == 0)))
                                {
                                    menuItemCodes[currentMenuVCode].currentMenuHCode += 100;
                                }
                                else
                                {
                                    menuItemCodes[currentMenuVCode].currentMenuHCode += 10;
                                }
                            }
                            else
                            {
                                if (menuItemCodes[currentMenuVCode].currentMenuHCode <= 0)
                                {
                                    int fixCount = 0;
                                    int numDec = 0;
                                    bool multFixed = false;
                                    if (menuItemCodes[currentMenuVCode].slotIncrementInt >= 10)
                                    {
                                        while ((((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) + numDec) % 100 != 0) && fixCount < 100)
                                        {
                                            if ((((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) + numDec) % 10 == 0))
                                            {
                                                numDec += 10;
                                            }
                                            else
                                            {
                                                numDec++;
                                            }
                                            if ((((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) + numDec) % 100 == 0))
                                            {
                                                multFixed = true;
                                            }
                                            fixCount++;
                                        }
                                        if (multFixed)
                                        {
                                            menuItemCodes[currentMenuVCode].currentMenuHCode += (numDec / 10);
                                        }
                                        else
                                        {
                                            menuItemCodes[currentMenuVCode].currentMenuHCode += 1;
                                        }
                                    }
                                    else
                                    {
                                        while ((((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) + numDec) % 10 != 0) && fixCount < 20)
                                        {
                                            numDec++;
                                            if ((((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) + numDec) % 10 == 0))
                                            {
                                                multFixed = true;
                                            }
                                            fixCount++;
                                        }
                                        if (multFixed)
                                        {
                                            menuItemCodes[currentMenuVCode].currentMenuHCode += numDec;
                                        }
                                        else
                                        {
                                            menuItemCodes[currentMenuVCode].currentMenuHCode += 10;
                                        }
                                    }
                                }
                                else if ((menuItemCodes[currentMenuVCode].slotIncrementInt >= 10 && ((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) % 100 == 0)) || (menuItemCodes[currentMenuVCode].slotIncrementInt < 10 && ((menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)) % 10 == 0)))
                                {
                                    menuItemCodes[currentMenuVCode].currentMenuHCode += 10;
                                }
                                else
                                {
                                    menuItemCodes[currentMenuVCode].currentMenuHCode += 1;
                                }
                            }
                        }
                        else
                        {
                            menuItemCodes[currentMenuVCode].currentMenuHCode += 1;
                        }
                        if (menuItemCodes[currentMenuVCode].currentMenuHCode >= menuItemCodes[currentMenuVCode].menuItemMax)
                        {
                            menuItemCodes[currentMenuVCode].currentMenuHCode = menuItemCodes[currentMenuVCode].menuItemMax - 1;
                        }
                    }
                }
                else
                {
                    if (menuItemCodes[currentMenuVCode].currentMenuHCode == menuItemCodes[currentMenuVCode].menuItemMax - 1)
                    {
                        menuItemCodes[currentMenuVCode].currentMenuHCode = menuItemCodes[currentMenuVCode].menuItemMax - 1;
                    }
                    else
                    {
                        menuItemCodes[currentMenuVCode].currentMenuHCode += 1;
                    }
                    if (menuItemCodes[currentMenuVCode].currentMenuHCode >= menuItemCodes[currentMenuVCode].menuItemMax)
                    {
                        menuItemCodes[currentMenuVCode].currentMenuHCode = menuItemCodes[currentMenuVCode].menuItemMax - 1;
                    }
                }
                scrollFrames = ((holdScroll) ? 5f : 20f);
                holdScroll = true;
            }
            else if ((systemInputReader.useNewInput("MoveDown")) && scrollFrames <= 0 && (!systemInputReader.useNewInput("MoveLeft") && !systemInputReader.useNewInput("MoveRight")) && scrollUpWait <= 0)
            {
                sfxPlayer.PlaySound("Scroll");
                currentMenuVCode += 1;
                if (currentMenuVCode >= 8 && currentMenuVCode <= 10)
                {
                    currentMenuVCode = 11;
                    slotMove = 4;
                }
                else
                {
                    slotMove = 1;
                }
                if (currentMenuVCode >= menuItemCodes.Length)
                {
                    overRun = true;
                }
                currentMenuVCode = ((currentMenuVCode >= menuItemCodes.Length) ? 0 : currentMenuVCode);
                scrollFrames = ((holdScroll) ? 5f : 20f);
                holdScroll = true;
            }
            else if ((systemInputReader.useNewInput("MoveUp")) && scrollFrames <= 0 && (!systemInputReader.useNewInput("MoveLeft") && !systemInputReader.useNewInput("MoveRight")) && scrollDownWait <= 0)
            {
                sfxPlayer.PlaySound("Scroll");
                currentMenuVCode -= 1;
                if (currentMenuVCode >= 8 && currentMenuVCode <= 10)
                {
                    currentMenuVCode = 7;
                    slotMove = 4;
                }
                else
                {
                    slotMove = 1;
                }
                if (currentMenuVCode < 0)
                {
                    overRun = true;
                }
                currentMenuVCode = ((currentMenuVCode < 0) ? menuItemCodes.Length - 1 : currentMenuVCode);
                scrollFrames = ((holdScroll) ? 5f : 20f);
                holdScroll = true;
            }
            else
            {
                if (systemInputReader.checkReleased())
                {
                    scrollFrames = 0f;
                    holdScroll = false;
                    scrollTime = 0;
                }
            }
        }
        else if (controlCode == 1)
        {
            shardSettingsMenu.directionalController();
        }
    }

    public override IEnumerator disableMenu(bool selectLock)
    {
        if (appearMode == "Fade")
        {
            float curAlpha = 1.0f;
            while (curAlpha > 0f)
            {
                curAlpha -= 0.075f;
                if (curAlpha < 0f)
                {
                    curAlpha = 0f;
                }
                SpriteRenderer spr = gameObject.GetComponent<SpriteRenderer>();
                spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, curAlpha);
                SpriteRenderer[] children = GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer child in children)
                {
                    if (child.sprite.name.Contains("ScreenHighlighter"))
                    {
                        float highlightAlpha = curAlpha;
                        if (highlightAlpha >= 0.5f)
                        {
                            highlightAlpha = 0.5f;
                        }
                        child.color = new Color(child.color.r, child.color.g, child.color.b, highlightAlpha);
                    }
                    else if (child.gameObject.name.Contains("MenuScreenMore"))
                    {
                        if (child.gameObject.GetComponent<SpriteRenderer>().enabled)
                        {
                            child.color = new Color(child.color.r, child.color.g, child.color.b, curAlpha);
                        }
                    }
                    else
                    {
                        child.color = new Color(child.color.r, child.color.g, child.color.b, curAlpha);
                    }
                }
                TextMeshProUGUI[] childText = GetComponentsInChildren<TextMeshProUGUI>();
                foreach (TextMeshProUGUI child in childText)
                {
                    child.color = new Color(child.color.r, child.color.g, child.color.b, curAlpha);
                }
                yield return null;
            }
        }
        else if (appearMode == "Move")
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
        }
        yield return new WaitForSeconds(0.4f);
        currentPlayerInput.exitUI();
        currentPlayerInput.playerHierarchy = 0;
        currentPlayerInput.enablePlayerInput = true;
        currentPlayerInput.enableOtherPlayers(true);
        currentPlayerInput.setPlayerMenuMode("");
        additionalCloseConditions(selectLock);
        gameObject.SetActive(false);
        yield return null;
    }

    public IEnumerator lockMenu()
    {
        float curTint = 1.0f;
        SpriteRenderer sprSH = screenHighlighter[0].GetComponent<SpriteRenderer>();
        SpriteRenderer sprSHC = screenHighlighter[0].GetComponentInChildren<SpriteRenderer>();
        sprSH.color = new Color(sprSH.color.r, sprSH.color.g, sprSH.color.b, sprSH.color.a);
        sprSHC.color = new Color(sprSHC.color.r, sprSHC.color.g, sprSHC.color.b, sprSH.color.a);
        //menuDetailsRoot.gameObject.SetActive(false);
        while (curTint > 0.5f)
        {
            curTint -= 0.075f;
            if (curTint < 0.5f)
            {
                curTint = 0.5f;
            }
            SpriteRenderer spr = gameObject.GetComponent<SpriteRenderer>();
            spr.color = new Color(curTint, curTint, curTint, spr.color.a);
            SpriteRenderer[] children = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer child in children)
            {
                if (child.sprite.name.Contains("ScreenHighlighter"))
                {
                    float highlightTint = curTint;
                    if (highlightTint <= 0.5f)
                    {
                        highlightTint = 0.5f;
                    }
                    child.color = new Color(child.color.r - 0.075f, child.color.g - 0.075f, child.color.b - 0.075f, child.color.a);
                }
                else if (child.gameObject.name.Contains("MenuScreenMore"))
                {
                    if (child.gameObject.GetComponent<SpriteRenderer>().enabled)
                    {
                        child.color = new Color(curTint, curTint, curTint, child.color.a);
                    }
                }
                else
                {
                    if (!child.gameObject.name.Contains("MenuDetails"))
                    {
                        child.color = new Color(curTint, curTint, curTint, child.color.a);
                    }
                }
            }
            TextMeshProUGUI[] childText = GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI child in childText)
            {
                if (child.text.Contains("Edit"))
                {
                    child.color = new Color(child.color.r - 0.075f, child.color.g - 0.075f, child.color.b - 0.075f, child.color.a);
                } else
                {
                    if (!child.gameObject.name.Contains("MenuDetails"))
                    {
                        child.color = new Color(child.color.r - 0.075f, child.color.g - 0.075f, child.color.b - 0.075f, child.color.a);
                    }
                }
            }
            yield return null;
        }
        yield return new WaitForSeconds(0.4f);
        //csPlayerInput.enablePlayerInput = true;
        yield return null;
    }

    public IEnumerator unlockMenu()
    {
        float curTint = 0.5f;
        SpriteRenderer sprSH = screenHighlighter[0].GetComponent<SpriteRenderer>();
        SpriteRenderer sprSHC = screenHighlighter[0].GetComponentInChildren<SpriteRenderer>();
        sprSH.color = new Color(sprSH.color.r, sprSH.color.g, sprSH.color.b, sprSH.color.a);
        sprSHC.color = new Color(sprSHC.color.r, sprSHC.color.g, sprSHC.color.b, sprSH.color.a);
        //menuDetailsRoot.gameObject.SetActive(true);
        menuDetailsRoot.updateDetails(menuItemCodes[currentMenuVCode].menuDetailSection);
        while (curTint < 1.0f)
        {
            curTint += 0.075f;
            if (curTint > 1.0f)
            {
                curTint = 1.0f;
            }
            SpriteRenderer spr = gameObject.GetComponent<SpriteRenderer>();
            spr.color = new Color(curTint, curTint, curTint, spr.color.a);
            SpriteRenderer[] children = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer child in children)
            {
                if (child.sprite.name.Contains("ScreenHighlighter"))
                {
                    float highlightTint = curTint;
                    if (highlightTint >= 1.0f)
                    {
                        highlightTint = 1.0f;
                    }
                    child.color = new Color(child.color.r + 0.075f, child.color.g + 0.075f, child.color.b + 0.075f, child.color.a);
                }
                else if (child.gameObject.name.Contains("MenuScreenMore"))
                {
                    if (child.gameObject.GetComponent<SpriteRenderer>().enabled)
                    {
                        child.color = new Color(curTint, curTint, curTint, child.color.a);
                    }
                }
                else
                {
                    if (!child.gameObject.name.Contains("MenuDetails"))
                    {
                        child.color = new Color(curTint, curTint, curTint, child.color.a);
                    }
                }
            }
            TextMeshProUGUI[] childText = GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI child in childText)
            {
                if (child.text.Contains("Edit"))
                {
                    child.color = new Color(child.color.r + 0.075f, child.color.g + 0.075f, child.color.b + 0.075f, child.color.a);
                }
                else
                {
                    if (!child.gameObject.name.Contains("MenuDetails"))
                    {
                        child.color = new Color(child.color.r + 0.075f, child.color.g + 0.075f, child.color.b + 0.075f, child.color.a);
                    }
                }
            }
            yield return null;
        }
        yield return new WaitForSeconds(0.4f);
        while (shardSettingsMenu.gameObject.activeSelf == true)
        {
            yield return null;
        }
        controlCode = 0;
        currentPlayerInput.enablePlayerInput = true;
        yield return null;
    }

    public override void updatedData_Arrow(int setting)
    {
        if (setting == 0)
        {
            gameData.modeStyle = menuItemCodes[currentMenuVCode].currentMenuHCode;
        }
        else if (setting == 1)
        {
            gameData.modeReflection = menuItemCodes[currentMenuVCode].currentMenuHCode;
        }
        else if (setting == 4)
        {
            gameData.timerSetting = (menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt));
            csRTT.updateTimerInfo(gameData.timerSetting);
        }
        else if (setting == 5)
        {
            gameData.defaultStockCount = (menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt));
        }
        else if (setting == 6)
        {
            gameData.defaultInitialHealth = (menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt));
            if (gameData.defaultInitialHealth > 9999) { gameData.defaultInitialHealth = 9999; }
        }
        else if (setting == 11)
        {
            gameData.damageRatio = (float)(menuItemCodes[currentMenuVCode].slotValueFloat + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementFloat));
        }
        else if (setting == 12)
        {
            gameData.barrierRatio = (float)(menuItemCodes[currentMenuVCode].slotValueFloat + ((float)menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementFloat));
        }
    }

    public override void updateMenu(string menuType, int mode)
    {
        switch (menuType)
        {
            case "RulesMenu":
                if (mode == 0)
                {
                    menuItemCodes[0].currentMenuHCode = gameData.modeStyle;
                    menuSlots[0].transform.Find("OptionChoice").GetComponent<TextMeshProUGUI>().text = menuItemCodes[0].slotValueSingle[menuItemCodes[0].currentMenuHCode];
                }
                else if (mode == 1)
                {
                    menuItemCodes[1].currentMenuHCode = gameData.modeReflection;
                    menuSlots[1].transform.Find("OptionChoice").GetComponent<TextMeshProUGUI>().text = menuItemCodes[1].slotValueSingle[menuItemCodes[1].currentMenuHCode];
                }
                else if (mode == 2)
                {
                    menuItemCodes[2].currentMenuHCode = ((gameData.defaultTotalShards - menuItemCodes[2].slotValueInt) / menuItemCodes[2].slotIncrementInt);
                    menuSlots[2].transform.Find("OptionChoice").GetComponent<TextMeshProUGUI>().text = (menuItemCodes[2].slotValueInt + (menuItemCodes[2].currentMenuHCode * menuItemCodes[2].slotIncrementInt)).ToString();
                    fixNumberName(2);
                }
                else if (mode == 3)
                {
                    menuItemCodes[3].currentMenuHCode = ((gameData.defaultStockCount - menuItemCodes[3].slotValueInt) / menuItemCodes[3].slotIncrementInt);
                    menuSlots[3].transform.Find("OptionChoice").GetComponent<TextMeshProUGUI>().text = (menuItemCodes[3].slotValueInt + (menuItemCodes[3].currentMenuHCode * menuItemCodes[3].slotIncrementInt)).ToString();
                }
                else if (mode == 4)
                {
                    menuItemCodes[4].currentMenuHCode = ((gameData.timerSetting - menuItemCodes[4].slotValueInt) / menuItemCodes[4].slotIncrementInt);
                    menuSlots[4].transform.Find("OptionChoice").GetComponent<TextMeshProUGUI>().text = (menuItemCodes[4].slotValueInt + (menuItemCodes[4].currentMenuHCode * menuItemCodes[4].slotIncrementInt)).ToString();
                    fixNumberName(4);
                }
                else if (mode == 6)
                {
                    float newNum = Mathf.Round(((float)gameData.defaultInitialHealth - (float)menuItemCodes[7].slotValueInt) / (float)menuItemCodes[7].slotIncrementInt);
                    menuItemCodes[7].currentMenuHCode = (int)newNum;
                    menuSlots[7].transform.Find("OptionChoice").GetComponent<TextMeshProUGUI>().text = (menuItemCodes[7].slotValueInt + (menuItemCodes[7].currentMenuHCode * menuItemCodes[7].slotIncrementInt)).ToString();
                    fixNumberName(7);
                }
                else if (mode == 8)
                {
                    menuItemCodes[8].currentMenuHCode = ((gameData.defaultInitialShards - menuItemCodes[8].slotValueInt) / menuItemCodes[8].slotIncrementInt);
                    menuSlots[8].transform.Find("OptionChoice").GetComponent<TextMeshProUGUI>().text = (menuItemCodes[8].slotValueInt + (menuItemCodes[8].currentMenuHCode * menuItemCodes[8].slotIncrementInt)).ToString();
                }
                else if (mode == 9)
                {
                    menuItemCodes[9].currentMenuHCode = (int)Mathf.Round((gameData.damageRatio - menuItemCodes[9].slotValueFloat) / menuItemCodes[9].slotIncrementFloat);
                    menuSlots[9].transform.Find("OptionChoice").GetComponent<TextMeshProUGUI>().text = (menuItemCodes[9].slotValueFloat + (menuItemCodes[9].currentMenuHCode * menuItemCodes[9].slotIncrementFloat)).ToString("F1");
                }
                else if (mode == 10)
                {
                    menuItemCodes[10].currentMenuHCode = (int)Mathf.Round((gameData.barrierRatio - menuItemCodes[10].slotValueFloat) / menuItemCodes[10].slotIncrementFloat);
                    menuSlots[10].transform.Find("OptionChoice").GetComponent<TextMeshProUGUI>().text = (menuItemCodes[10].slotValueFloat + (menuItemCodes[10].currentMenuHCode * menuItemCodes[10].slotIncrementFloat)).ToString("F1");
                }
                break;
            default:
                break;
        }
    }

    public void updateShardSettings(int held, int total, int strength)
    {
        menuSlots[8].transform.Find("OptionChoice").GetComponent<TextMeshProUGUI>().text = held.ToString();
        menuSlots[9].transform.Find("OptionChoice").GetComponent<TextMeshProUGUI>().text = total.ToString();
        menuSlots[10].transform.Find("OptionChoice").GetComponent<TextMeshProUGUI>().text = strength.ToString();
    }

    protected override void summonUI()
    {
        if (useScreenHighlighter)
        {
            summonHighlighter();
        }
    }

    public void fixNumberName(int vCode)
    {
        if (menuItemCodes[vCode].specialNumberName.Length > 0)
        {
            foreach (string numberName in menuItemCodes[vCode].specialNumberName)
            {
                if (numberName.Contains("_"))
                {
                    int index = numberName.IndexOf("_");
                    string startCode = numberName.Substring(0, index);
                    if (startCode.Contains("-"))
                    {
                        int indexB = numberName.IndexOf("-");
                        int startNum = System.Convert.ToInt32(startCode.Substring(0, indexB));
                        int endNum = System.Convert.ToInt32(startCode.Substring(indexB + 1));
                        if (menuItemCodes[vCode].currentMenuHCode >= startNum && menuItemCodes[vCode].currentMenuHCode <= endNum)
                        {
                            string result = numberName.Substring(index + 1);
                            result = result.Replace("#", (menuItemCodes[vCode].slotValueInt + (menuItemCodes[vCode].currentMenuHCode * menuItemCodes[vCode].slotIncrementInt)).ToString());
                            menuSlots[vCode].transform.Find("OptionChoice").GetComponent<TextMeshProUGUI>().text = result;
                        }
                    }
                    else
                    {
                        int startNum = System.Convert.ToInt32(startCode);
                        if (menuItemCodes[vCode].currentMenuHCode == startNum)
                        {
                            string result = numberName.Substring(index + 1);
                            result = result.Replace("#", (menuItemCodes[vCode].slotValueInt + (menuItemCodes[vCode].currentMenuHCode * menuItemCodes[vCode].slotIncrementInt)).ToString());
                            menuSlots[vCode].transform.Find("OptionChoice").GetComponent<TextMeshProUGUI>().text = result;
                        }
                    }
                }
            }
        }
    }

}
