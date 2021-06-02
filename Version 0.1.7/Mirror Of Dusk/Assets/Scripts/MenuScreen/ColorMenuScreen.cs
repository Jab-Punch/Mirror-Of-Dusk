using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rewired;

public class ColorMenuScreen : HorizontalMenuScreen, IMenuSelectOrb {

    private CharacterSelectManager characterSelectManager;

    public int playerId = 0;
    private int c_playerId;
    private Player playerControl;
    public GameObject menuSelectOrbPrefab;
    public List<GameObject> menuSelectOrb { get; set; }
    private SpriteRenderer menuSelectOrbSprite;
    private CSPlayerGUI csPlayerGUI;
    private CharacterSelectShard csShard;
    //private SpriteRenderer shardSprite;
    private int prevSelectedSlot;
    public GameObject _selectedColorText;
    private TextMeshProUGUI selectedColorText;
    private FighterDataCollection fighterDataCollection;

    void Awake()
    {
        setUpMenu();
        initializeHorizontalScreen();
        initializeColorMenuScreen();
    }

    // Use this for initialization
    void Start () {
        scriptOn = true;
        gameObject.SetActive(false);
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
        /*screenHighlighter = new List<GameObject>();
        screenHighlighter.Add(Instantiate(screenHighlighterPrefab, new Vector3(0, 0, 0), Quaternion.identity));
        Destroy(screenHighlighter[0]);
        screenHighlighter.Clear();*/
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
        /*initialSlotPosX = ((menuSlots[slotLimit - 1].transform.parent.transform.position.x + (menuSlots[slotLimit - 1].transform.parent.GetComponent<RectTransform>().rect.width / 2)) + menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.x - (menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.width * (slotLimit - 1)));
        currentSlotPosX = (menuSlots[slotLimit - 1].transform.parent.transform.position.x + (menuSlots[slotLimit - 1].transform.parent.GetComponent<RectTransform>().rect.width / 2)) + menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.x;
        initialLayoutPosX = gameObject.GetComponentInChildren<HorizontalLayoutGroup>().gameObject.transform.position.x;*/

        float posX = ((menuSlots[currentMenuHCode].transform.parent.transform.position.x - (menuSlots[currentMenuHCode].transform.parent.GetComponent<RectTransform>().rect.width / 2)) - menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.x + (menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.width * currentMenuHCode));
        float posY = ((menuSlots[currentMenuHCode].transform.parent.transform.position.y + (menuSlots[currentMenuHCode].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.y);
        
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

    private void initializeColorMenuScreen()
    {
        c_playerId = playerId;
        csPlayerGUI = characterSelectManager.csPlayerGUI.transform.GetChild(playerId).GetComponent<CSPlayerGUI>();
        csShard = characterSelectManager.csShards.transform.GetChild(playerId).GetComponent<CharacterSelectShard>();
        //shardSprite = csShard.gameObject.transform.Find("LargeIcon").GetComponent<SpriteRenderer>();
        fighterDataCollection = characterSelectManager.fighterDataCollection.GetComponent<FighterDataCollection>();
        selectedColorText = _selectedColorText.GetComponentInChildren<TextMeshProUGUI>();
        menuSelectOrb = new List<GameObject>();

        float posX = ((menuSlots[0].transform.parent.transform.position.x - (menuSlots[0].transform.parent.GetComponent<RectTransform>().rect.width / 2)) - menuSlots[0].GetComponent<RectTransform>().rect.x + (menuSlots[0].GetComponent<RectTransform>().rect.width * 0));
        float posY = ((menuSlots[0].transform.parent.transform.position.y + (menuSlots[0].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[0].GetComponent<RectTransform>().rect.y - (menuSlots[0].GetComponent<RectTransform>().rect.height * 0));

        menuSelectOrb.Add(Instantiate(menuSelectOrbPrefab, new Vector3(posX, posY + 44f, -3f), Quaternion.identity));
        menuSelectOrb[0].transform.parent = gameObject.transform;
        menuSelectOrb[0].GetComponent<MenuSelectOrb>().sortLayer("MenuSubA");
    }

    public override void assignCurrentPlayer(CSPlayerInput c_player)
    {
        currentPlayerInput = c_player;
        c_playerId = c_player.playerID;
        playerControl = ReInput.players.GetPlayer(c_playerId);
    }

    public override void OnEnable()
    {
        if (scriptOn)
        {
            if (menuDetailsRoot != null)
            {
                menuDetailsRoot.updateDetails(menuItemCodes[currentMenuHCode].menuDetailSection);
            }
            currentMenuHCode = csPlayerData.characterColorCode;
            resetHighlighter();
            resetMenuSelectOrb();
            csShard.prevSelectedColor = currentMenuHCode;
            prevSelectedSlot = currentMenuHCode;
            updateColorText();
            StartCoroutine("summonMenu", endMoveY);
        }
    }

    // Update is called once per frame
    void Update () {
        //checkForVerticalScroll();
        checkForHorizontalScroll();
    }

    protected override void checkForHorizontalScroll()
    {
        if (prevMenuHCode != currentMenuHCode)
        {
            //updateHorizontalScroll();
            summonUI();
            prevMenuHCode = currentMenuHCode;
            updateColorIcon();
            updateColorText();
        }
    }

    private void updateColorText()
    {
        string resultText = "Default";
        for (int i = 0; i < fighterDataCollection.fighterSelectData.Length; i++)
        {
            if (csPlayerData.characterCode == -1)
            {
                break;
            }
            if (fighterDataCollection.fighterSelectData[i].name == csPlayerData.selectedCharacter && fighterDataCollection.fighterSelectData[i].active)
            {
                FighterData fCData = fighterDataCollection.fighterSelectData[i].fighterData.GetComponent<FighterData>();
                for (int j = 0; j < fCData.fighterColorData.Length; j++)
                {
                    if (currentMenuHCode == j)
                    {
                        //selectedColorText.fontSize = 24;
                        resultText = System.Convert.ToString(fCData.fighterColorData[j].colorCode) + ": " + fCData.fighterColorData[j].name;
                        break;
                    }
                }
                break;
            }
        }
        selectedColorText.text = resultText;
    }

    private void updateColorIcon()
    {
        csShard.selectedColor = currentMenuHCode;
        csShard.UpdateShard();
        /*for (int i = 0; i < fighterDataCollection.fighterSelectData.Length; i++)
        {
            if (csPlayerData.characterCode == -1)
            {
                break;
            }
            if (fighterDataCollection.fighterSelectData[i].name == csPlayerData.selectedCharacter && fighterDataCollection.fighterSelectData[i].active)
            {
                FighterData fCData = fighterDataCollection.fighterSelectData[i].fighterData.GetComponent<FighterData>();
                for (int j = 0; j < fCData.fighterColorData.Length; j++)
                {
                    if (currentMenuHCode == j)
                    {
                        if (fCData.fighterColorData[j].selectPalettes[0].bustUpPalette != null)
                        {
                            shardSprite.sharedMaterial = fCData.fighterColorData[j].selectPalettes.bustUpPalette;
                        }
                        break;
                    }
                }
                break;
            }
        }*/
    }

    public override void selectOption()
    {
        sfxPlayer.PlaySound("Confirm");
        csPlayerData.characterColorCode = currentMenuHCode;
        csShard.selectedColor = currentMenuHCode;
        csPlayerGUI.updateColorText();
        currentPlayerInput.playerInputMode = "CharacterSelect";
        currentPlayerInput.enablePlayerInput = false;
        resetScrollFrames();
        StartCoroutine("disableMenu", true);
    }

    public override void closeMenu()
    {
        sfxPlayer.PlaySound("Cancel");
        csPlayerGUI.bufferGUI = 30;
        csPlayerData.characterColorCode = csShard.prevSelectedColor;
        csShard.selectedColor = csShard.prevSelectedColor;
        csShard.UpdateShard();
        currentPlayerInput.playerInputMode = "CharacterSelect";
        currentPlayerInput.enablePlayerInput = false;
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
        currentMenuHCode = 0;
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

    protected override void additionalCloseConditions(bool condition)
    {
        if (condition)
        {
            prevSelectedSlot = currentMenuHCode;
        } else
        {
            currentMenuHCode = prevSelectedSlot;
        }
    }

    public override void directionalController()
    {
        if (!currentPlayerInput.onUI)
        {
            padHorizontal = playerControl.GetAxisRaw("Move Horizontal");
            //padVertical = playerControl.GetAxisRaw("Move Vertical");
            //padHorizontal = Input.GetAxisRaw("Horizontal_" + pMenuCode);
            //padVertical = Input.GetAxisRaw("Vertical_" + pMenuCode);
            
            scrollFrames--;
            scrollTime++;
            if (scrollFrames <= 0)
            {
                scrollFrames = 0f;
            }
            if (inputReader.useNewInput("MoveRight", c_playerId) && scrollFrames <= 0 && (!inputReader.useNewInput("MoveUp", c_playerId) && !inputReader.useNewInput("MoveDown", c_playerId)))
            {
                sfxPlayer.PlaySound("Scroll");
                currentMenuHCode += 1;
                currentMenuHCode = ((currentMenuHCode >= menuItemCodes.Length) ? 0 : currentMenuHCode);
                scrollFrames = ((holdScroll) ? 5f : 20f);
                holdScroll = true;
            }
            else if (inputReader.useNewInput("MoveLeft", c_playerId) && scrollFrames <= 0 && (!inputReader.useNewInput("MoveUp", c_playerId) && !inputReader.useNewInput("MoveDown", c_playerId)))
            {
                sfxPlayer.PlaySound("Scroll");
                currentMenuHCode -= 1;
                currentMenuHCode = ((currentMenuHCode < 0) ? menuItemCodes.Length - 1 : currentMenuHCode);
                scrollFrames = ((holdScroll) ? 5f : 20f);
                holdScroll = true;
            }
            if (!inputReader.useNewInput("MoveRight", c_playerId) && !inputReader.useNewInput("MoveLeft", c_playerId))
            {
                scrollFrames = 0f;
                holdScroll = false;
                scrollTime = 0;
            }
        }
    }

    protected void summonMenuSelectOrb()
    {
        float posX = ((menuSlots[currentMenuHCode].transform.parent.transform.position.x - (menuSlots[currentMenuHCode].transform.parent.GetComponent<RectTransform>().rect.width / 2)) - menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.x + (menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.width * currentMenuHCode));
        float posY = ((menuSlots[currentMenuHCode].transform.parent.transform.position.y + (menuSlots[currentMenuHCode].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.y);

        menuSelectOrb.Add(Instantiate(menuSelectOrbPrefab, new Vector3(posX, posY + 44f, -3f), Quaternion.identity));
        menuSelectOrb[menuSelectOrb.Count - 1].transform.parent = gameObject.transform;
        menuSelectOrb[menuSelectOrb.Count - 1].GetComponent<MenuSelectOrb>().sortLayer("MenuSubA");
        menuSelectOrb[menuSelectOrb.Count - 1].GetComponent<MenuSelectOrb>().summonInstant = false;
        menuSelectOrb[menuSelectOrb.Count - 2].GetComponent<MenuSelectOrb>().StartCoroutine("destroyMenuSelectOrb");
        //screenHighlighter.RemoveAt(screenHighlighter.Count - 2);
    }

    protected override void summonUI()
    {
        summonHighlighter();
        summonMenuSelectOrb();
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
        float posX = ((menuSlots[currentMenuHCode].transform.parent.transform.position.x - (menuSlots[currentMenuHCode].transform.parent.GetComponent<RectTransform>().rect.width / 2)) - menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.x + (menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.width * currentMenuHCode));
        float posY = ((menuSlots[currentMenuHCode].transform.parent.transform.position.y + (menuSlots[currentMenuHCode].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.y);
        menuSelectOrb.Add(Instantiate(menuSelectOrbPrefab, new Vector3(posX, posY + 44f, -3f), Quaternion.identity));
        menuSelectOrb[0].transform.parent = gameObject.transform;
        menuSelectOrb[0].GetComponent<MenuSelectOrb>().sortLayer("MenuSubA");
    }

}
