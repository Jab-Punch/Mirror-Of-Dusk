using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Rewired;

public class RulesMenuShardSettingsScreen : VerticalMenuScreenB {

    private CharacterSelectManager characterSelectManager;

    private CSPlayerGUI[] csPlayerGUI;
    private CSPlayerInput[] csPlayerInputs;
    private CSPlayerData[] csPlayerDatas;
    private ActivePlayers activePlayers;
    private ShardsMenuScreen[] shardsMenuScreen;
    private MenuEditNumber[] menuEditNumber;
    private ShardsMenuControl shardsMenuControl;
    private int selectedAmount;
    private int prevShardsHeld;
    private int prevShardTotal;
    private int prevShardStrength;
    private bool haltInput = false;
    private RulesMenuScreen rulesRootMenu;

    void Awake()
    {
        setUpMenu();
        initializeVerticalScreen();
        initializeRulesShardsMenuScreen();
    }

    // Use this for initialization
    void Start () {
        scriptOn = true;
        initializeRulesShardsMenuStyle();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnEnable()
    {
        if (scriptOn)
        {
            currentMenuVCode = 0;
            if (rulesRootMenu.menuDetailsRoot != null)
            {
                rulesRootMenu.menuDetailsRoot.updateDetails(menuItemCodes[currentMenuVCode].menuDetailSection);
            }
            for (int i = 0; i < menuEditNumber.Length; i++)
            {
                menuEditNumber[i].slotPos = menuEditNumber[i].horizLen - 1;
            }
            prevShardsHeld = gameData.defaultInitialShards;
            selectedAmount = prevShardsHeld;
            menuEditNumber[0].updateNumber(selectedAmount);
            menuEditNumber[0].activateText();
            menuEditNumber[1].deactivateText();
            menuEditNumber[2].deactivateText();
            menuEditNumber[0].summonUI();
            //prevHandicapHealth = csPlayerData.playerHealth;
            //handicapHealthText.text = HealthText(csPlayerData.playerHealth);
            if (useScreenHighlighter)
            {
                resetHighlighter();
                float posX = ((menuSlots[currentMenuVCode].transform.parent.transform.position.x - (menuSlots[currentMenuVCode].transform.parent.GetComponent<RectTransform>().rect.width / 2)) - menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.x);
                float posY = ((menuSlots[currentMenuVCode].transform.parent.transform.position.y + (menuSlots[currentMenuVCode].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.y - (menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.height * currentMenuVCode + (currentMenuVCode * mainLayout.spacing)));
                screenHighlighter[0].transform.position = new Vector3(posX, posY, screenHighlighter[0].transform.position.z);
            }
            //float posXO = ((menuSlots[currentMenuVCode].transform.parent.transform.position.x - (menuSlots[currentMenuVCode].transform.parent.GetComponent<RectTransform>().rect.width / 2)) - menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.x);
            //float posYO = ((menuSlots[currentMenuVCode].transform.parent.transform.position.y + (menuSlots[currentMenuVCode].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.y - (menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.height * currentMenuVCode + (currentMenuVCode * mainLayout.spacing)));
            StartCoroutine("summonMenu", endMoveY);
        }
    }

    public void assignRulesRootMenu(RulesMenuScreen rootMen)
    {
        rulesRootMenu = rootMen;
    }

    protected override void setUpMenu()
    {
        characterSelectManager = CharacterSelectManager.characterSelectManager;

        menuName = gameObject.name.Substring(0, gameObject.name.IndexOf("Screen"));
        activePlayers = characterSelectManager.activePlayers.GetComponent<ActivePlayers>();
        sfxPlayer = characterSelectManager.sfxPlayer.GetComponent<SFXPlayer>();
        gameData = characterSelectManager.gameData.GetComponent<GameData>();
        s_player = ReInput.players.GetPlayer(4);
        systemInputReader = characterSelectManager.eventSystem.GetComponent<SystemInputReader>();
        csPlayerInputs = new CSPlayerInput[4];
        csPlayerDatas = new CSPlayerData[4];
        csPlayerGUI = new CSPlayerGUI[4];
        shardsMenuScreen = new ShardsMenuScreen[4];
        for (int i = 0; i < 4; i++)
        {
            csPlayerInputs[i] = characterSelectManager.players[i].GetComponent<CSPlayerInput>();
            csPlayerDatas[i] = characterSelectManager.players[i].GetComponent<CSPlayerData>();
            csPlayerGUI[i] = characterSelectManager.csPlayerGUI.transform.GetChild(i).GetComponent<CSPlayerGUI>();
            shardsMenuScreen[i] = characterSelectManager.shardsMenuScreens.transform.GetChild(i).GetComponent<ShardsMenuScreen>();
        }
        shardsMenuControl = characterSelectManager.csControl.GetComponent<ShardsMenuControl>();
    }

    private void initializeRulesShardsMenuScreen()
    {
        menuEditNumber = new MenuEditNumber[3];
        menuEditNumber = gameObject.transform.GetComponentsInChildren<MenuEditNumber>();
        //csPlayerGUI = GameObject.Find("CSPlayerGUI_" + pMenuCode).GetComponent<CSPlayerGUI>();
        //shardsLeftText = gameObject.transform.Find("CanvasShardsLeft").gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }
    

    private void initializeRulesShardsMenuStyle()
    {
        menuEditNumber[0].setMinMax(0, 2500);
        menuEditNumber[1].setMinMax(0, 10000);
        menuEditNumber[2].setMinMax(0, 10000);
        menuEditNumber[0].updateNumber(gameData.defaultInitialShards);
        menuEditNumber[1].updateNumber('-');
        menuEditNumber[2].updateNumber('-');
    }

    public override void assignPlayer(int playerCode)
    {
        currentPlayerInput = csPlayerInputs[playerCode];
    }

    public override void selectOption()
    {
        //csPlayerInput = GameObject.Find("Player " + pMenuCode).GetComponent<CSPlayerInput>();
        sfxPlayer.PlaySound("Confirm");
        currentMenuVCode++;
        if (currentMenuVCode == 1)
        {
            prevShardsHeld = selectedAmount;
            if (useScreenHighlighter)
            {
                summonHighlighter();
            }
            menuEditNumber[0].destroyMenuSelectArrowUp();
            menuEditNumber[0].destroyMenuSelectArrowDown();
            selectedAmount = prevShardsHeld * TotalActivePlayers();
            menuEditNumber[1].setMin(selectedAmount);
            menuEditNumber[1].updateNumber(selectedAmount);
            menuEditNumber[1].activateText();
            menuEditNumber[1].summonUI();
            if (rulesRootMenu.menuDetailsRoot != null)
            {
                rulesRootMenu.menuDetailsRoot.updateDetails(menuItemCodes[currentMenuVCode].menuDetailSection);
            }
            haltInput = true;
        }
        if (currentMenuVCode == 2)
        {
            prevShardTotal = selectedAmount;
            if (useScreenHighlighter)
            {
                summonHighlighter();
            }
            menuEditNumber[1].destroyMenuSelectArrowUp();
            menuEditNumber[1].destroyMenuSelectArrowDown();
            selectedAmount = ((prevShardTotal < 150) ? prevShardTotal : 150);
            menuEditNumber[2].setMax(prevShardTotal);
            menuEditNumber[2].updateNumber(selectedAmount);
            menuEditNumber[2].activateText();
            menuEditNumber[2].summonUI();
            haltInput = true;
            if (rulesRootMenu.menuDetailsRoot != null)
            {
                rulesRootMenu.menuDetailsRoot.updateDetails(menuItemCodes[currentMenuVCode].menuDetailSection);
            }
        }
        if (currentMenuVCode == 3)
        {
            prevShardStrength = selectedAmount;
            menuEditNumber[2].destroyMenuSelectArrowUp();
            menuEditNumber[2].destroyMenuSelectArrowDown();

            gameData.defaultInitialShards = prevShardsHeld;
            gameData.defaultTotalShards = prevShardTotal;
            gameData.defaultShardStrength = prevShardStrength;
            for (int i = 0; i < 4; i++)
            {
                csPlayerDatas[i].playerInitShards = gameData.defaultInitialShards;
                csPlayerDatas[i].playerShardStrength = gameData.defaultShardStrength;
            }

            shardsMenuControl.DeclareShardsLeft();
            for (int i = 0; i < shardsMenuScreen.Length; i++)
            {
                shardsMenuScreen[i].updateAllText();
            }

            rulesRootMenu.updateShardSettings(gameData.defaultInitialShards, gameData.defaultTotalShards, gameData.defaultShardStrength);
            
            //csPlayerInput = GameObject.Find("Player " + pMenuCode).GetComponent<CSPlayerInput>();
            rootMenu.StartCoroutine("unlockMenu");
            currentPlayerInput.enablePlayerInput = false;
            resetScrollFrames();
            StartCoroutine("disableMenu", false);
        }
    }

    public override void closeMenu()
    {
        if (currentMenuVCode == 0)
        {
            //csPlayerInput = GameObject.Find("Player " + pMenuCode).GetComponent<CSPlayerInput>();
            sfxPlayer.PlaySound("Cancel");
            rootMenu.StartCoroutine("unlockMenu");
            currentPlayerInput.enablePlayerInput = false;
            resetScrollFrames();
            StartCoroutine("disableMenu", false);
        } else if (currentMenuVCode == 1)
        {
            sfxPlayer.PlaySound("Cancel");
            currentMenuVCode--;
            if (useScreenHighlighter)
            {
                summonHighlighter();
            }
            menuEditNumber[1].destroyMenuSelectArrowUp();
            menuEditNumber[1].destroyMenuSelectArrowDown();
            selectedAmount = prevShardsHeld;
            menuEditNumber[1].updateNumber('-');
            menuEditNumber[1].deactivateText();
            menuEditNumber[0].summonUI();
            if (rulesRootMenu.menuDetailsRoot != null)
            {
                rulesRootMenu.menuDetailsRoot.updateDetails(menuItemCodes[currentMenuVCode].menuDetailSection);
            }
            haltInput = true;
        } else if (currentMenuVCode == 2)
        {
            sfxPlayer.PlaySound("Cancel");
            currentMenuVCode--;
            if (useScreenHighlighter)
            {
                summonHighlighter();
            }
            menuEditNumber[2].destroyMenuSelectArrowUp();
            menuEditNumber[2].destroyMenuSelectArrowDown();
            selectedAmount = prevShardTotal;
            menuEditNumber[2].updateNumber('-');
            menuEditNumber[2].deactivateText();
            menuEditNumber[1].summonUI();
            if (rulesRootMenu.menuDetailsRoot != null)
            {
                rulesRootMenu.menuDetailsRoot.updateDetails(menuItemCodes[currentMenuVCode].menuDetailSection);
            }
            haltInput = true;
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
        //csPlayerInput.enablePlayerInput = true;
        additionalCloseConditions(selectLock);
        gameObject.SetActive(false);
        yield return null;
    }

    public override void directionalController()
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
        if (scrollFrames <= 0)
        {
            scrollFrames = 0f;
        }
        if (systemInputReader.useNewInput("MoveRight") && scrollFrames <= 0 && (!systemInputReader.useNewInput("MoveUp") && !systemInputReader.useNewInput("MoveDown")) && scrollUpWait <= 0 && !haltInput)
        {
            if (menuEditNumber[currentMenuVCode].ScrollRight())
            {
                //sfxPlayer.PlaySound("Scroll");
            }
            scrollFrames = ((holdScroll) ? 5f : 20f);
            holdScroll = true;
        }
        else if (systemInputReader.useNewInput("MoveLeft") && scrollFrames <= 0 && (!systemInputReader.useNewInput("MoveUp") && !systemInputReader.useNewInput("MoveDown")) && scrollDownWait <= 0 && !haltInput)
        {
            if (menuEditNumber[currentMenuVCode].ScrollLeft())
            {
                //sfxPlayer.PlaySound("Scroll");
            }
            scrollFrames = ((holdScroll) ? 5f : 20f);
            holdScroll = true;
        } else if ((systemInputReader.useNewInput("MoveDown")) && scrollFrames <= 0 && !haltInput)
        {
            selectedAmount = menuEditNumber[currentMenuVCode].ScrollDown(selectedAmount, menuEditNumber[currentMenuVCode].slotPos);
            scrollFrames = ((holdScroll) ? 5f : 20f);
            holdScroll = true;
        }
        else if ((systemInputReader.useNewInput("MoveUp")) && scrollFrames <= 0 && !haltInput)
        {
            selectedAmount = menuEditNumber[currentMenuVCode].ScrollUp(selectedAmount, menuEditNumber[currentMenuVCode].slotPos);
            scrollFrames = ((holdScroll) ? 5f : 20f);
            holdScroll = true;
        }
        else
        {
            if (systemInputReader.checkReleased())
            {
                scrollFrames = 0f;
                holdScroll = false;
            }
        }
        haltInput = false;
    }

    private int TotalActivePlayers()
    {
        int total = 0;
        for (int i = 0; i < activePlayers.playerOn.Length; i++)
        {
            if (activePlayers.playerOn[i])
            {
                total++;
            }
        }
        return total;
    }
}
