using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rewired;

public class HPMenuScreen : HorizontalMenuScreen, IMenuSelectArrowUp, IMenuSelectArrowDown {

    private CharacterSelectManager characterSelectManager;

    public int playerId = 0;
    private int c_playerId;
    private Player playerControl;
    public GameObject menuSelectArrowUpPrefab;
    public GameObject menuSelectArrowDownPrefab;
    public List<GameObject> menuSelectArrowUp { get; set; }
    public List<GameObject> menuSelectArrowDown { get; set; }
    private SpriteRenderer menuSelectArrowUpSprite;
    private SpriteRenderer menuSelectArrowDownSprite;
    private CSPlayerGUI csPlayerGUI;
    private TextMeshProUGUI handicapHealthText;
    private TextMeshProUGUI defaultHealthText;
    private bool atMax = false;
    private bool atMin = false;
    private int prevHandicapHealth;

    [Header("Manager Prefabs")]
    public GameObject _defaultHealthText;
    public GameObject _handicapHealthText;

    void Awake()
    {
        setUpMenu();
        initializeHorizontalScreen();
        initializeHPMenuScreen();
    }

    // Use this for initialization
    void Start () {
        scriptOn = true;
        initializeHPMenuStyle();
        gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        checkForVerticalScroll();
        checkForHorizontalScroll();
    }

    public override void OnEnable()
    {
        if (scriptOn)
        {
            if (menuDetailsRoot != null)
            {
                menuDetailsRoot.updateDetails(menuItemCodes[currentMenuHCode].menuDetailSection);
            }
            prevMenuHCode = 3;
            currentMenuHCode = 3;
            prevHandicapHealth = csPlayerData.playerHealth;
            handicapHealthText.text = HealthText(csPlayerData.playerHealth);
            resetMenuSelectArrowUp();
            resetMenuSelectArrowDown();
            defaultHealthText.text = HealthText(gameData.defaultInitialHealth);
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
        //initializeCharacterCodes();
        csPlayerData = characterSelectManager.players[playerId].GetComponent<CSPlayerData>();
        /*pMenuCode = gameObject.name.Substring(gameObject.name.Length - 1, 1);
        try
        {
            menuDetailsRoot = gameObject.GetComponentInChildren<MenuDetailsRoot>();
        }
        catch (System.NullReferenceException)
        {

        }
        inputReader = GameObject.Find("EventSystem").GetComponent<InputReader>();
        initializeCharacterCodes();
        csPlayerData = GameObject.Find("Player " + pMenuCode).GetComponent<CSPlayerData>();*/
    }

    protected override void initializeHorizontalScreen()
    {
        prevMenuItemCodes = new int[menuItemCodes.Length];
        for (int i = 0; i < menuItemCodes.Length; i++)
        {
            prevMenuItemCodes[i] = menuItemCodes[i].currentMenuHCode;
        }
        currentHSlot = currentMenuHCode;
        GameObject mainLayout = gameObject.GetComponentInChildren<HorizontalLayoutGroup>().gameObject;
        menuSlots = new GameObject[mainLayout.transform.childCount];
        for (int i = 0; i < mainLayout.transform.childCount; i++)
        {
            menuSlots[i] = mainLayout.transform.GetChild(i).gameObject;
        }
        slotLimit = System.Convert.ToInt32(Mathf.Floor((optionMask.GetComponent<SpriteMask>().transform.localScale.x) / (menuSlots[0].GetComponent<RectTransform>().rect.width)));

    }

    private void initializeHPMenuScreen()
    {
        c_playerId = playerId;
        csPlayerGUI = characterSelectManager.csPlayerGUI.transform.GetChild(playerId).GetComponent<CSPlayerGUI>();
        defaultHealthText = _defaultHealthText.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        handicapHealthText = _handicapHealthText.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        menuSelectArrowUp = new List<GameObject>();
        menuSelectArrowDown = new List<GameObject>();
    }

    private void initializeHPMenuStyle()
    {
        defaultHealthText.text = HealthText(gameData.defaultInitialHealth);
        handicapHealthText.text = HealthText(csPlayerData.playerHealth);

        float posX = ((menuSlots[currentMenuHCode].transform.parent.transform.position.x - (menuSlots[currentMenuHCode].transform.parent.GetComponent<RectTransform>().rect.width / 2)) - menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.x + (menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.width * currentMenuHCode));
        float posY = ((menuSlots[currentMenuHCode].transform.parent.transform.position.y + (menuSlots[currentMenuHCode].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.y);
        menuSelectArrowUp.Add(Instantiate(menuSelectArrowUpPrefab, new Vector3(posX, posY + 20f, -5f), Quaternion.identity));
        menuSelectArrowUp[0].transform.parent = gameObject.transform;
        menuSelectArrowUp[0].GetComponent<MenuSelectArrowUp>().sortLayer("MenuSubA");
        if (csPlayerData.playerHealth >= 9999)
        {
            menuSelectArrowUp[0].GetComponent<MenuSelectArrowUp>().arrowActive = false;
            atMax = true;
        }
        menuSelectArrowDown.Add(Instantiate(menuSelectArrowDownPrefab, new Vector3(posX, posY - 20f, -5f), Quaternion.identity));
        menuSelectArrowDown[0].transform.parent = gameObject.transform;
        menuSelectArrowDown[0].GetComponent<MenuSelectArrowDown>().sortLayer("MenuSubA");
        if (csPlayerData.playerHealth <= 1)
        {
            menuSelectArrowDown[0].GetComponent<MenuSelectArrowDown>().arrowActive = false;
            atMin = true;
        }
    }

    public override void assignCurrentPlayer(CSPlayerInput c_player)
    {
        currentPlayerInput = c_player;
        c_playerId = c_player.playerID;
        playerControl = ReInput.players.GetPlayer(c_playerId);
    }

    protected override void checkForHorizontalScroll()
    {
        if (prevMenuHCode != currentMenuHCode)
        {
            updateHorizontalScroll();
        }
    }

    protected override void checkForVerticalScroll()
    {
        if (prevMenuItemCodes[currentMenuHCode] != menuItemCodes[currentMenuHCode].currentMenuHCode || overRunH)
        {
            int prevHSlot = prevMenuItemCodes[currentMenuHCode];
            currentVSlot = menuItemCodes[currentMenuHCode].currentMenuHCode;
            prevMenuItemCodes[currentMenuHCode] = menuItemCodes[currentMenuHCode].currentMenuHCode;
            if (menuItemCodes[currentMenuHCode].SlotType == ShowValueEnum.RangeInt)
            {
                if ((prevHSlot < prevMenuItemCodes[currentMenuHCode] && !overRunH) || (prevMenuItemCodes[currentMenuHCode] < prevHSlot && overRunH))
                {
                    updateHP(currentMenuHCode, 1);
                }
                else if ((prevHSlot > prevMenuItemCodes[currentMenuHCode] && !overRunH) || (prevMenuItemCodes[currentMenuHCode] > prevHSlot && overRunH))
                {
                    updateHP(currentMenuHCode, -1);
                }
            }
            overRunH = false;
        }
    }

    public override void selectOption()
    {
        sfxPlayer.PlaySound("Confirm");
        currentPlayerInput.playerInputMode = "CharacterSelect";
        currentPlayerInput.enablePlayerInput = false;
        csPlayerGUI.refreshGUIOnce = true;
        prevMenuHCode = 3;
        currentMenuHCode = 3;
        resetScrollFrames();
        StartCoroutine("disableMenu", false);
    }

    public override void directionalController()
    {
        if (!currentPlayerInput.onUI)
        {
            padHorizontal = playerControl.GetAxisRaw("Move Horizontal");
            padVertical = playerControl.GetAxisRaw("Move Vertical");
            //padHorizontal = Input.GetAxisRaw("Horizontal_" + pMenuCode);
            //padVertical = Input.GetAxisRaw("Vertical_" + pMenuCode);
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
                if (!(menuItemCodes[currentMenuHCode].SlotType == ShowValueEnum.Access || menuItemCodes[currentMenuHCode].SlotType == ShowValueEnum.Confirm || menuItemCodes[currentMenuHCode].SlotType == ShowValueEnum.None))
                {
                    sfxPlayer.PlaySound("Scroll");
                }
                menuItemCodes[currentMenuHCode].currentMenuHCode -= 1;
                if (menuItemCodes[currentMenuHCode].currentMenuHCode < 0)
                {
                    menuItemCodes[currentMenuHCode].currentMenuHCode = 9;
                    overRunH = true;
                }
                scrollFrames = ((holdScroll) ? 5f : 20f);
                holdScroll = true;
            }
            else if ((inputReader.useNewInput("MoveUp", c_playerId) || inputReader.useNewInput("MoveUp_Left", c_playerId) || inputReader.useNewInput("MoveUp_Right", c_playerId)) && scrollFrames <= 0)
            {
                if (!(menuItemCodes[currentMenuHCode].SlotType == ShowValueEnum.Access || menuItemCodes[currentMenuHCode].SlotType == ShowValueEnum.Confirm || menuItemCodes[currentMenuHCode].SlotType == ShowValueEnum.None))
                {
                    sfxPlayer.PlaySound("Scroll");
                }
                menuItemCodes[currentMenuHCode].currentMenuHCode += 1;
                if (menuItemCodes[currentMenuHCode].currentMenuHCode > 9)
                {
                    menuItemCodes[currentMenuHCode].currentMenuHCode = 0;
                    overRunH = true;
                }
                scrollFrames = ((holdScroll) ? 5f : 20f);
                holdScroll = true;
            }
            else if (inputReader.useNewInput("MoveRight", c_playerId) && scrollFrames <= 0 && (!inputReader.useNewInput("MoveUp", c_playerId) && !inputReader.useNewInput("MoveDown", c_playerId)) && scrollUpWait <= 0)
            {
                sfxPlayer.PlaySound("Scroll");
                currentMenuHCode += 1;
                if (currentMenuHCode >= menuItemCodes.Length)
                {
                    overRun = true;
                }
                currentMenuHCode = ((currentMenuHCode >= menuItemCodes.Length) ? 0 : currentMenuHCode);
                scrollFrames = ((holdScroll) ? 5f : 20f);
                holdScroll = true;
            }
            else if (inputReader.useNewInput("MoveLeft", c_playerId) && scrollFrames <= 0 && (!inputReader.useNewInput("MoveUp", c_playerId) && !inputReader.useNewInput("MoveDown", c_playerId)) && scrollDownWait <= 0)
            {
                sfxPlayer.PlaySound("Scroll");
                currentMenuHCode -= 1;
                if (currentMenuHCode < 0)
                {
                    overRun = true;
                }
                currentMenuHCode = ((currentMenuHCode < 0) ? menuItemCodes.Length - 1 : currentMenuHCode);
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

    public void updateHP(int mode, int add)
    {
        if (mode == 0) //Digit 4
        {
            csPlayerData.playerHealth += add * 1000;
        }
        else if (mode == 1) //Digit 3
        {
            csPlayerData.playerHealth += add * 100;
        }
        else if (mode == 2) //Digit 2
        {
            csPlayerData.playerHealth += add * 10;
        }
        else if (mode == 3) //Digit 1
        {
            csPlayerData.playerHealth += add;
        }

        if (csPlayerData.playerHealth >= 9999)
        {
            csPlayerData.playerHealth = 9999;
            if (!atMax)
            {
                updateArrowUp(true);
            }
            atMax = true;
        } else
        {
            if (atMax)
            {
                updateArrowUp(false);
            }
            atMax = false;
        }
        if (csPlayerData.playerHealth <= 1)
        {
            csPlayerData.playerHealth = 1;
            if (!atMin)
            {
                updateArrowDown(true);
            }
            atMin = true;
        } else
        {
            if (atMin)
            {
                updateArrowDown(false);
            }
            atMin = false;
        }
        handicapHealthText.text = HealthText(csPlayerData.playerHealth);
    }

    public string HealthText(int health)
    {
        string result = "";
        if (health < 1000)
        {
            result += "0";
        }
        if (health < 100)
        {
            result += "0";
        }
        if (health < 10)
        {
            result += "0";
        }

        result += health.ToString();
        
        return result;
    }

    public override void closeMenu()
    {
        sfxPlayer.PlaySound("Cancel");
        csPlayerGUI.bufferGUI = 30;
        currentPlayerInput.playerInputMode = "CharacterSelect";
        currentPlayerInput.enablePlayerInput = false;
        prevMenuHCode = 3;
        currentMenuHCode = 3;
        csPlayerData.playerHealth = prevHandicapHealth;
        csPlayerGUI.refreshGUIOnce = true;
        resetScrollFrames();
        StartCoroutine("disableMenu", false);
    }

    public override void forceCloseMenu()
    {
        resetScrollFrames();
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, destroyEndMoveY, gameObject.transform.position.z);
        currentPlayerInput.playerHierarchy = 0;
        currentPlayerInput.setPlayerMenuMode("");
        csPlayerGUI.inUse = false;
        prevMenuHCode = 3;
        currentMenuHCode = 3;
        csPlayerData.playerHealth = prevHandicapHealth;
        csPlayerGUI.refreshGUIOnce = true;
        gameObject.SetActive(false);
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
                    if (child.gameObject.name.Contains("MenuScreenMore"))
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

    protected void summonMenuSelectArrowUp()
    {
        if (menuSelectArrowUp.Count > 0)
        {
            for (int i = 0; i < menuSelectArrowUp.Count; i++)
            {
                Destroy(menuSelectArrowUp[i]);
            }
            menuSelectArrowUp.Clear();
        }
        float posX = ((menuSlots[currentMenuHCode].transform.parent.transform.position.x - (menuSlots[currentMenuHCode].transform.parent.GetComponent<RectTransform>().rect.width / 2)) - menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.x + (menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.width * currentMenuHCode));
        float posY = ((menuSlots[currentMenuHCode].transform.parent.transform.position.y + (menuSlots[currentMenuHCode].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.y);

        menuSelectArrowUp.Add(Instantiate(menuSelectArrowUpPrefab, new Vector3(posX, posY + 20f, -5f), Quaternion.identity));
        if (csPlayerData.playerHealth >= 9999)
        {
            menuSelectArrowUp[menuSelectArrowUp.Count - 1].GetComponent<MenuSelectArrowUp>().arrowActive = false;
        }
        menuSelectArrowUp[menuSelectArrowUp.Count - 1].transform.parent = gameObject.transform;
        menuSelectArrowUp[menuSelectArrowUp.Count - 1].GetComponent<MenuSelectArrowUp>().sortLayer("MenuSubA");
        menuSelectArrowUp[menuSelectArrowUp.Count - 1].GetComponent<MenuSelectArrowUp>().summonInstant = true;
    }

    protected void summonMenuSelectArrowDown()
    {
        if (menuSelectArrowDown.Count > 0)
        {
            for (int i = 0; i < menuSelectArrowDown.Count; i++)
            {
                Destroy(menuSelectArrowDown[i]);
            }
            menuSelectArrowDown.Clear();
        }
        float posX = ((menuSlots[currentMenuHCode].transform.parent.transform.position.x - (menuSlots[currentMenuHCode].transform.parent.GetComponent<RectTransform>().rect.width / 2)) - menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.x + (menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.width * currentMenuHCode));
        float posY = ((menuSlots[currentMenuHCode].transform.parent.transform.position.y + (menuSlots[currentMenuHCode].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.y);

        menuSelectArrowDown.Add(Instantiate(menuSelectArrowDownPrefab, new Vector3(posX, posY - 20f, -5f), Quaternion.identity));
        if (csPlayerData.playerHealth <= 1)
        {
            menuSelectArrowDown[menuSelectArrowDown.Count - 1].GetComponent<MenuSelectArrowDown>().arrowActive = false;
        }
        menuSelectArrowDown[menuSelectArrowDown.Count - 1].transform.parent = gameObject.transform;
        menuSelectArrowDown[menuSelectArrowDown.Count - 1].GetComponent<MenuSelectArrowDown>().sortLayer("MenuSubA");
        menuSelectArrowDown[menuSelectArrowDown.Count - 1].GetComponent<MenuSelectArrowDown>().summonInstant = true;
    }

    protected override void summonUI()
    {
        summonMenuSelectArrowUp();
        summonMenuSelectArrowDown();
    }

    public void updateArrowUp(bool on)
    {
        if (on)
        {
            float posY = ((menuSlots[currentMenuHCode].transform.parent.transform.position.y + (menuSlots[currentMenuHCode].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.y);
            menuSelectArrowUp[0].GetComponent<MenuSelectArrowUp>().turnOffArrow(posY + 20f);
            menuSelectArrowDown[0].GetComponent<MenuSelectArrowDown>().resetArrow(posY - 20f);
        } else
        {
            float posY = ((menuSlots[currentMenuHCode].transform.parent.transform.position.y + (menuSlots[currentMenuHCode].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.y);
            menuSelectArrowUp[0].GetComponent<MenuSelectArrowUp>().turnOnArrow();
            menuSelectArrowUp[0].GetComponent<MenuSelectArrowUp>().resetArrow(posY + 20f);
            menuSelectArrowDown[0].GetComponent<MenuSelectArrowDown>().resetArrow(posY - 20f);
        }
    }

    public void updateArrowDown(bool on)
    {
        if (on)
        {
            float posY = ((menuSlots[currentMenuHCode].transform.parent.transform.position.y + (menuSlots[currentMenuHCode].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.y);
            menuSelectArrowDown[0].GetComponent<MenuSelectArrowDown>().turnOffArrow(posY - 20f);
            menuSelectArrowUp[0].GetComponent<MenuSelectArrowUp>().resetArrow(posY + 20f);
        }
        else
        {
            float posY = ((menuSlots[currentMenuHCode].transform.parent.transform.position.y + (menuSlots[currentMenuHCode].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.y);
            menuSelectArrowDown[0].GetComponent<MenuSelectArrowDown>().turnOnArrow();
            menuSelectArrowUp[0].GetComponent<MenuSelectArrowUp>().resetArrow(posY + 20f);
            menuSelectArrowDown[0].GetComponent<MenuSelectArrowDown>().resetArrow(posY - 20f);
        }
    }

    public void removeMenuSelectArrowUp()
    {
        menuSelectArrowUp.RemoveAt(0);
    }

    public void removeMenuSelectArrowDown()
    {
        menuSelectArrowDown.RemoveAt(0);
    }

    public void resetMenuSelectArrowUp()
    {
        for (int i = 0; i < menuSelectArrowUp.Count; i++)
        {
            Destroy(menuSelectArrowUp[i]);
        }
        menuSelectArrowUp.Clear();
        float posX = ((menuSlots[currentMenuHCode].transform.parent.transform.position.x - (menuSlots[currentMenuHCode].transform.parent.GetComponent<RectTransform>().rect.width / 2)) - menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.x + (menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.width * currentMenuHCode));
        float posY = ((menuSlots[currentMenuHCode].transform.parent.transform.position.y + (menuSlots[currentMenuHCode].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.y);
        menuSelectArrowUp.Add(Instantiate(menuSelectArrowUpPrefab, new Vector3(posX, posY + 20f, -5f), Quaternion.identity));
        menuSelectArrowUp[0].transform.parent = gameObject.transform;
        menuSelectArrowUp[0].GetComponent<MenuSelectArrowUp>().sortLayer("MenuSubA");
        if (csPlayerData.playerHealth >= 9999)
        {
            updateArrowUp(true);
            atMax = true;
        }
        else
        {
            menuSelectArrowUp[0].GetComponent<MenuSelectArrowUp>().resetArrow(posY + 20f);
            atMax = false;
        }
    }

    public void resetMenuSelectArrowDown()
    {
        for (int i = 0; i < menuSelectArrowDown.Count; i++)
        {
            Destroy(menuSelectArrowDown[i]);
        }
        menuSelectArrowDown.Clear();
        float posX = ((menuSlots[currentMenuHCode].transform.parent.transform.position.x - (menuSlots[currentMenuHCode].transform.parent.GetComponent<RectTransform>().rect.width / 2)) - menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.x + (menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.width * currentMenuHCode));
        float posY = ((menuSlots[currentMenuHCode].transform.parent.transform.position.y + (menuSlots[currentMenuHCode].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.y);
        menuSelectArrowDown.Add(Instantiate(menuSelectArrowDownPrefab, new Vector3(posX, posY - 20f, -5f), Quaternion.identity));
        menuSelectArrowDown[0].transform.parent = gameObject.transform;
        menuSelectArrowDown[0].GetComponent<MenuSelectArrowDown>().sortLayer("MenuSubA");
        if (csPlayerData.playerHealth <= 1)
        {
            updateArrowDown(true);
            atMin = true;
        }
        else
        {
            menuSelectArrowDown[0].GetComponent<MenuSelectArrowDown>().resetArrow(posY - 20f);
            atMin = false;
        }
    }
}
