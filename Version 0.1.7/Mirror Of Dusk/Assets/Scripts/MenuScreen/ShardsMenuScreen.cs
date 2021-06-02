using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rewired;

public class ShardsMenuScreen : VerticalMenuScreenB, IMenuSelectOrb {

    private CharacterSelectManager characterSelectManager;

    public int playerId = 0;
    private int c_playerId;
    private Player playerControl;
    public GameObject menuSelectOrbPrefab;
    public List<GameObject> menuSelectOrb { get; set; }
    private SpriteRenderer menuSelectOrbSprite;
    private CSPlayerGUI csPlayerGUI;
    private CSPlayerData[] otherPlayers;
    private CSPlayerInput[] otherPlayerInput;
    private MenuEditNumber[] menuEditNumber;
    private ShardsMenuControl shardsMenuControl;
    private TextMeshProUGUI shardsHeldText;
    private TextMeshProUGUI shardStrengthText;
    private TextMeshProUGUI shardsLeftText;
    private TextMeshProUGUI totalShardsText;
    private int menuPhase = 0;
    private int[] prevShards;

    [Header("Manager Prefabs")]
    public GameObject _shardsLeftCanvas;

    void Awake()
    {
        setUpMenu();
        initializeVerticalScreen();
        initializeShardsMenuScreen();
    }

    // Use this for initialization
    void Start () {
        scriptOn = true;
        initializeShardsMenuStyle();
        gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        menuEditNumber[0].setMax(shardsMenuControl.totalShardsLeft + csPlayerData.playerInitShards);
        shardsLeftText.text = updateNumber(shardsMenuControl.totalShardsLeft, 5);
        if (menuPhase == 1 && currentMenuVCode == 0)
        {
            menuEditNumber[0].reviewMaxLimit();
        }
        checkForVerticalScroll();
    }

    public override void OnEnable()
    {
        if (scriptOn)
        {
            prevMenuVCode = 0;
            currentMenuVCode = 0;
            menuPhase = 0;
            for (int i = 0; i < menuEditNumber.Length; i++)
            {
                menuEditNumber[i].activateText();
                menuEditNumber[i].resetUI();
            }
            menuEditNumber[0].updateNumber(csPlayerData.playerInitShards);
            menuEditNumber[1].updateNumber(csPlayerData.playerShardStrength);
            prevShards[0] = csPlayerData.playerInitShards;
            prevShards[1] = csPlayerData.playerShardStrength;
            //prevHandicapHealth = csPlayerData.playerHealth;
            //handicapHealthText.text = HealthText(csPlayerData.playerHealth);
            if (useScreenHighlighter)
            {
                resetHighlighter();
                /*float posX = ((menuSlots[currentMenuVCode].transform.parent.transform.position.x - (menuSlots[currentMenuVCode].transform.parent.GetComponent<RectTransform>().rect.width / 2)) - menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.x);
                float posY = ((menuSlots[currentMenuVCode].transform.parent.transform.position.y + (menuSlots[currentMenuVCode].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.y - (menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.height * currentMenuVCode + (currentMenuVCode * mainLayout.spacing)));
                screenHighlighter[0].transform.position = new Vector3(posX, posY, screenHighlighter[0].transform.position.z);*/
            }
            resetMenuSelectOrb();
            shardsLeftText.text = updateNumber(shardsMenuControl.totalShardsLeft, 5);
            totalShardsText.text = updateNumber(gameData.defaultTotalShards, 5);
            StartCoroutine("summonMenu", endMoveY);
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

    protected override void initializeVerticalScreen()
    {
        prevMenuItemCodes = new int[menuItemCodes.Length];
        for (int i = 0; i < menuItemCodes.Length; i++)
        {
            prevMenuItemCodes[i] = menuItemCodes[i].currentMenuHCode;
        }
        mainLayout = gameObject.GetComponentInChildren<VerticalLayoutGroup>();
        GameObject mainLayoutGM = mainLayout.gameObject;
        menuSlots = new GameObject[mainLayoutGM.transform.childCount];
        for (int i = 0; i < mainLayoutGM.transform.childCount; i++)
        {
            menuSlots[i] = mainLayoutGM.transform.GetChild(i).gameObject;
        }
        overRun = false;

        if (useSlotLimits)
        {
            initialSlotPosY = ((menuSlots[slotLimit - 1].transform.parent.transform.position.y + (menuSlots[slotLimit - 1].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.y - (menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.height * (slotLimit - 1)));
            currentSlotPosY = (menuSlots[slotLimit - 1].transform.parent.transform.position.y + (menuSlots[slotLimit - 1].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.y;
            initialLayoutPosY = gameObject.GetComponentInChildren<VerticalLayoutGroup>().gameObject.transform.position.y;
        }

        float posX = ((menuSlots[currentMenuVCode].transform.parent.transform.position.x - (menuSlots[currentMenuVCode].transform.parent.GetComponent<RectTransform>().rect.width / 2)) - menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.x);
        float posY = ((menuSlots[currentMenuVCode].transform.parent.transform.position.y + (menuSlots[currentMenuVCode].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.y - (menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.height * currentMenuVCode));

        if (useScreenHighlighter)
        {
            screenHighlighter = new List<GameObject>();
            screenHighlighter.Add(Instantiate(screenHighlighterPrefab, new Vector3(posX, posY, -2f), Quaternion.identity));
            screenHighlighter[0].transform.parent = gameObject.transform;
            float curAlpha;
            if (appearMode == "Fade")
            {
                curAlpha = 0;
            }
            else
            {
                curAlpha = 0.5f;
            }
            SpriteRenderer sprSH = screenHighlighter[0].GetComponent<SpriteRenderer>();
            SpriteRenderer sprSHC = screenHighlighter[0].GetComponentInChildren<SpriteRenderer>();
            sprSH.color = new Color(sprSH.color.r, sprSH.color.g, sprSH.color.b, curAlpha);
            sprSHC.color = new Color(sprSHC.color.r, sprSHC.color.g, sprSHC.color.b, curAlpha);
        }
    }

    private void initializeShardsMenuScreen()
    {
        c_playerId = playerId;
        csPlayerGUI = characterSelectManager.csPlayerGUI.transform.GetChild(playerId).GetComponent<CSPlayerGUI>();
        otherPlayers = new CSPlayerData[3];
        otherPlayerInput = new CSPlayerInput[3];
        int otherInt = 0;
        for (int i = 0; i < 4; i++)
        {
            if (playerId != i)
            {
                otherPlayers[otherInt] = characterSelectManager.players[i].GetComponent<CSPlayerData>();
                otherPlayerInput[otherInt] = characterSelectManager.players[i].GetComponent<CSPlayerInput>();
                otherInt++;
            }
        }
        shardsLeftText = _shardsLeftCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        totalShardsText = _shardsLeftCanvas.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        menuEditNumber = new MenuEditNumber[2];
        menuEditNumber = gameObject.transform.GetComponentsInChildren<MenuEditNumber>();
        for (int i = 0; i < menuEditNumber.Length; i++)
        {
            menuEditNumber[i].updateSortLayer("MenuSubA");
        }
        prevShards = new int[2];
        //totalShardsText = gameObject.transform.Find("CanvasHandicapHealth").gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        menuSelectOrb = new List<GameObject>();

        float posX = ((menuSlots[currentMenuVCode].transform.parent.transform.position.x - (menuSlots[currentMenuVCode].transform.parent.GetComponent<RectTransform>().rect.width / 2)) - menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.x);
        float posY = ((menuSlots[currentMenuVCode].transform.parent.transform.position.y + (menuSlots[currentMenuVCode].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.y - (menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.height * currentMenuVCode + (currentMenuVCode * mainLayout.spacing)));

        menuSelectOrb.Add(Instantiate(menuSelectOrbPrefab, new Vector3(posX - 67f, posY, -10f), Quaternion.identity));
        menuSelectOrb[0].GetComponent<MenuSelectOrb>().sortLayer("MenuSubA");
        menuSelectOrb[0].transform.parent = gameObject.transform;
    }

    private void initializeShardsMenuStyle()
    {
        shardsMenuControl = characterSelectManager.csControl.GetComponent<ShardsMenuControl>();
        menuEditNumber[0].setMinMax(0, ShardsLeft());
        menuEditNumber[1].setMinMax(0, gameData.defaultTotalShards);
    }

    public override void assignCurrentPlayer(CSPlayerInput c_player)
    {
        currentPlayerInput = c_player;
        c_playerId = c_player.playerID;
        playerControl = ReInput.players.GetPlayer(c_playerId);
    }

    private int ShardsLeft()
    {
        int dTS = gameData.defaultTotalShards;
        for (int i = 0; i < otherPlayers.Length; i++)
        {
            dTS -= otherPlayers[i].playerInitShards;
        }
        return dTS;
    }

    public string updateNumber(int editedNumber, int numlength)
    {
        string fmt = new System.String('0', numlength);
        return editedNumber.ToString(fmt);
    }

    protected override void checkForVerticalScroll()
    {
        if (prevMenuVCode != currentMenuVCode)
        {
            summonUI();
            prevMenuVCode = currentMenuVCode;
            overRun = false;
        }
    }

    public override void selectOption()
    {
        if (menuPhase == 0)
        {
            sfxPlayer.PlaySound("Confirm");
            menuPhase++;
            if (currentMenuVCode == 0)
            {
                menuEditNumber[0].setMax(ShardsLeft());
                //prevShards[currentMenuVCode] = csPlayerData.playerInitShards;
            } else if (currentMenuVCode == 1)
            {
                //prevShards[currentMenuVCode] = csPlayerData.playerShardStrength;
            }
            menuEditNumber[currentMenuVCode].slotPos = menuEditNumber[currentMenuVCode].horizLen - 1;
            menuEditNumber[currentMenuVCode].summonUI();
        } else if (menuPhase == 1)
        {
            sfxPlayer.PlaySound("Confirm");
            if (currentMenuVCode == 0)
            {
                prevShards[currentMenuVCode] = csPlayerData.playerInitShards;
                //csPlayerData.playerInitShards = prevShards[currentMenuVCode];
            } else if (currentMenuVCode == 1)
            {
                prevShards[currentMenuVCode] = csPlayerData.playerShardStrength;
                //csPlayerData.playerShardStrength = prevShards[currentMenuVCode];
            }
            menuEditNumber[currentMenuVCode].destroyMenuSelectArrowUp();
            menuEditNumber[currentMenuVCode].destroyMenuSelectArrowDown();
            menuPhase--;
        }
        /*csPlayerInput.playerInputMode = "CharacterSelect";
        csPlayerInput.enablePlayerInput = false;
        csPlayerGUI.refreshGUIOnce = true;
        resetScrollFrames();
        StartCoroutine("disableMenu", false);*/
    }

    public override void closeMenu()
    {
        if (menuPhase == 0)
        {
            sfxPlayer.PlaySound("Cancel");
            csPlayerGUI.bufferGUI = 30;
            currentPlayerInput.playerInputMode = "CharacterSelect";
            currentPlayerInput.enablePlayerInput = false;
            csPlayerGUI.refreshGUIOnce = true;
            resetScrollFrames();
            StartCoroutine("disableMenu", false);
        }
        else if (menuPhase == 1)
        {
            sfxPlayer.PlaySound("Cancel");
            if (currentMenuVCode == 0)
            {
                csPlayerData.playerInitShards = prevShards[0];
                menuEditNumber[0].updateNumber(csPlayerData.playerInitShards);
                shardsMenuControl.DeclareShardsLeft();
                shardsLeftText.text = updateNumber(shardsMenuControl.totalShardsLeft, 5);
                for (int i = 0; i < otherPlayerInput.Length; i++)
                {
                    otherPlayerInput[i].haltPlayerInput = true;
                }
            }
            else if (currentMenuVCode == 1)
            {
                csPlayerData.playerShardStrength = prevShards[1];
                menuEditNumber[1].updateNumber(csPlayerData.playerShardStrength);
            }
            menuEditNumber[currentMenuVCode].destroyMenuSelectArrowUp();
            menuEditNumber[currentMenuVCode].destroyMenuSelectArrowDown();
            menuPhase--;
        }
    }

    public override void forceCloseMenu()
    {
        resetScrollFrames();
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, destroyEndMoveY, gameObject.transform.position.z);
        currentPlayerInput.playerHierarchy = 0;
        currentPlayerInput.setPlayerMenuMode("");
        csPlayerGUI.inUse = false;
        //menuEditNumber[currentMenuVCode].destroyMenuSelectArrowUp();
        //menuEditNumber[currentMenuVCode].destroyMenuSelectArrowDown();
        csPlayerData.playerInitShards = prevShards[0];
        menuEditNumber[0].updateNumber(csPlayerData.playerInitShards);
        shardsMenuControl.DeclareShardsLeft();
        shardsLeftText.text = updateNumber(shardsMenuControl.totalShardsLeft, 5);
        csPlayerData.playerShardStrength = prevShards[1];
        menuEditNumber[1].updateNumber(csPlayerData.playerShardStrength);
        currentMenuVCode = 0;
        menuPhase = 0;
        menuEditNumber[0].setMinMax(0, ShardsLeft());
        menuEditNumber[1].setMinMax(0, gameData.defaultTotalShards);
        csPlayerGUI.refreshGUIOnce = true;
        gameObject.SetActive(false);
    }

    public override void directionalController()
    {
        if (!currentPlayerInput.onUI)
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
            if (menuPhase <= 0)
            {
                if ((inputReader.useNewInput("MoveDown", c_playerId) || inputReader.useNewInput("MoveDown_Left", c_playerId) || inputReader.useNewInput("MoveDown_Right", c_playerId)) && scrollFrames <= 0 && (!inputReader.useNewInput("MoveLeft", c_playerId) && !inputReader.useNewInput("MoveRight", c_playerId)))
                {
                    sfxPlayer.PlaySound("Scroll");
                    currentMenuVCode += 1;
                    if (currentMenuVCode >= menuItemCodes.Length)
                    {
                        overRun = true;
                    }
                    currentMenuVCode = ((currentMenuVCode >= menuItemCodes.Length) ? 0 : currentMenuVCode);
                    scrollFrames = ((holdScroll) ? 5f : 20f);
                    holdScroll = true;
                }
                else if ((inputReader.useNewInput("MoveUp", c_playerId) || inputReader.useNewInput("MoveUp_Left", c_playerId) || inputReader.useNewInput("MoveUp_Right", c_playerId)) && scrollFrames <= 0 && (!inputReader.useNewInput("MoveLeft", c_playerId) && !inputReader.useNewInput("MoveRight", c_playerId)))
                {
                    sfxPlayer.PlaySound("Scroll");
                    currentMenuVCode -= 1;
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
                    if (inputReader.checkReleased(c_playerId))
                    {
                        scrollFrames = 0f;
                        holdScroll = false;
                    }
                }
            }
            else if (menuPhase == 1)
            {
                if (inputReader.useNewInput("MoveRight", c_playerId) && scrollFrames <= 0 && (!inputReader.useNewInput("MoveUp", c_playerId) && !inputReader.useNewInput("MoveDown", c_playerId)))
                {
                    if (menuEditNumber[currentMenuVCode].ScrollRight())
                    {
                        //sfxPlayer.PlaySound("Scroll");
                    }
                    scrollFrames = ((holdScroll) ? 5f : 20f);
                    holdScroll = true;
                }
                else if (inputReader.useNewInput("MoveLeft", c_playerId) && scrollFrames <= 0 && (!inputReader.useNewInput("MoveUp", c_playerId) && !inputReader.useNewInput("MoveDown", c_playerId)))
                {
                    if (menuEditNumber[currentMenuVCode].ScrollLeft())
                    {
                        //sfxPlayer.PlaySound("Scroll");
                    }
                    scrollFrames = ((holdScroll) ? 5f : 20f);
                    holdScroll = true;
                }
                else if ((inputReader.useNewInput("MoveDown", c_playerId) || inputReader.useNewInput("MoveDown_Left", c_playerId) || inputReader.useNewInput("MoveDown_Right", c_playerId)) && scrollFrames <= 0)
                {
                    if (currentMenuVCode == 0)
                    {
                        csPlayerData.playerInitShards = menuEditNumber[currentMenuVCode].ScrollDown(csPlayerData.playerInitShards, menuEditNumber[currentMenuVCode].slotPos);
                        shardsMenuControl.DeclareShardsLeft();
                        shardsLeftText.text = updateNumber(shardsMenuControl.totalShardsLeft, 5);
                        for (int i = 0; i < otherPlayerInput.Length; i++)
                        {
                            otherPlayerInput[i].haltPlayerInput = true;
                        }
                    }
                    else if (currentMenuVCode == 1)
                    {
                        csPlayerData.playerShardStrength = menuEditNumber[currentMenuVCode].ScrollDown(csPlayerData.playerShardStrength, menuEditNumber[currentMenuVCode].slotPos);
                    }
                    scrollFrames = ((holdScroll) ? 5f : 20f);
                    holdScroll = true;
                }
                else if ((inputReader.useNewInput("MoveUp", c_playerId) || inputReader.useNewInput("MoveUp_Left", c_playerId) || inputReader.useNewInput("MoveUp_Right", c_playerId)) && scrollFrames <= 0)
                {
                    if (currentMenuVCode == 0)
                    {
                        csPlayerData.playerInitShards = menuEditNumber[currentMenuVCode].ScrollUp(csPlayerData.playerInitShards, menuEditNumber[currentMenuVCode].slotPos);
                        shardsMenuControl.DeclareShardsLeft();
                        shardsLeftText.text = updateNumber(shardsMenuControl.totalShardsLeft, 5);
                        for (int i = 0; i < otherPlayerInput.Length; i++)
                        {
                            otherPlayerInput[i].haltPlayerInput = true;
                        }
                    }
                    else if (currentMenuVCode == 1)
                    {
                        csPlayerData.playerShardStrength = menuEditNumber[currentMenuVCode].ScrollUp(csPlayerData.playerShardStrength, menuEditNumber[currentMenuVCode].slotPos);
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
            }
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
        currentPlayerInput.playerHierarchy = 0;
        currentPlayerInput.enablePlayerInput = true;
        currentPlayerInput.enableOtherPlayers(true);
        currentPlayerInput.setPlayerMenuMode("");
        csPlayerGUI.inUse = false;
        additionalCloseConditions(selectLock);
        gameObject.SetActive(false);
        yield return null;
    }

    public void updateAllText ()
    {
        menuEditNumber[0].updateNumber(gameData.defaultInitialShards);
        menuEditNumber[1].updateNumber(gameData.defaultShardStrength);
        menuEditNumber[0].setMinMax(0, ShardsLeft());
        menuEditNumber[1].setMinMax(0, gameData.defaultTotalShards);
        shardsLeftText.text = updateNumber(shardsMenuControl.totalShardsLeft, 5);
        totalShardsText.text = updateNumber(gameData.defaultTotalShards, 5);
    }

    protected void summonMenuSelectOrb()
    {
        if (menuPhase <= 0)
        {
            float posX = ((menuSlots[currentMenuVCode].transform.parent.transform.position.x - (menuSlots[currentMenuVCode].transform.parent.GetComponent<RectTransform>().rect.width / 2)) - menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.x);
            float posY = ((menuSlots[currentMenuVCode].transform.parent.transform.position.y + (menuSlots[currentMenuVCode].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.y - (menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.height * currentMenuVCode + (currentMenuVCode * mainLayout.spacing)));

            menuSelectOrb.Add(Instantiate(menuSelectOrbPrefab, new Vector3(posX - 67f, posY, -10f), Quaternion.identity));
            menuSelectOrb[menuSelectOrb.Count - 1].transform.parent = gameObject.transform;
            menuSelectOrb[menuSelectOrb.Count - 1].GetComponent<MenuSelectOrb>().sortLayer("MenuSubA");
            menuSelectOrb[menuSelectOrb.Count - 1].GetComponent<MenuSelectOrb>().summonInstant = true;
            menuSelectOrb[menuSelectOrb.Count - 1].GetComponent<MenuSelectOrb>().StartCoroutine("animateOrb","Left");
            menuSelectOrb[menuSelectOrb.Count - 2].GetComponent<MenuSelectOrb>().StartCoroutine("destroyMenuSelectOrb");
        }
        //screenHighlighter.RemoveAt(screenHighlighter.Count - 2);
    }

    protected override void summonUI()
    {
        if (menuPhase <= 0)
        {
            summonHighlighter();
            summonMenuSelectOrb();
        }
    }

    public void removeMenuSelectOrb()
    {
        menuSelectOrb.RemoveAt(0);
    }

    public void resetMenuSelectOrb()
    {
        for (int i = 0; i < menuSelectOrb.Count; i++)
        {
            Destroy(menuSelectOrb[i]);
        }
        menuSelectOrb.Clear();
        float posX = ((menuSlots[currentMenuVCode].transform.parent.transform.position.x - (menuSlots[currentMenuVCode].transform.parent.GetComponent<RectTransform>().rect.width / 2)) - menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.x);
        float posY = ((menuSlots[currentMenuVCode].transform.parent.transform.position.y + (menuSlots[currentMenuVCode].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.y - (menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.height * currentMenuVCode + (currentMenuVCode * mainLayout.spacing)));
        menuSelectOrb.Add(Instantiate(menuSelectOrbPrefab, new Vector3(posX - 67f, posY, -10f), Quaternion.identity));
        menuSelectOrb[0].transform.parent = gameObject.transform;
        menuSelectOrb[0].GetComponent<MenuSelectOrb>().sortLayer("MenuSubA");
        menuSelectOrb[0].GetComponent<MenuSelectOrb>().StartCoroutine("animateOrb", "Left");
    }
}
