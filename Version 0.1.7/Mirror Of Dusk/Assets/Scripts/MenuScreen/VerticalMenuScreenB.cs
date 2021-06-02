using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEditor;
using TMPro;

public class VerticalMenuScreenB : NewMenuScreenRoot
{

    [System.Serializable]
    public class MenuItemCodes
    {
        public int menuItemMax;
        public int currentMenuHCode;
        [Header("Slot Type")]
        public ShowValueEnum SlotType = ShowValueEnum.None;
        [Header("Slot Is Access")]
        [DrawIf("SlotType", ShowValueEnum.Access)]
        public string menuSummonName;
        [Header("Slot Is Confirm")]
        [DrawIf("SlotType", ShowValueEnum.Confirm)]
        public int currentConfirm = 0;
        [DrawIf("SlotType", ShowValueEnum.Confirm)]
        public string confirmOption;
        [Header("Slot Is Single")]
        public string[] slotValueSingle;
        public NewMenuDataCollect m_MenuDataCollect = new NewMenuDataCollect();
        [Header("Slot Is Int")]
        [DrawIf("SlotType", ShowValueEnum.RangeInt)]
        public int slotValueInt = 0;
        [DrawIf("SlotType", ShowValueEnum.RangeInt)]
        public int slotIncrementInt;
        public string[] specialNumberName;
        [Header("Slot Is Float")]
        [DrawIf("SlotType", ShowValueEnum.RangeFloat)]
        public float slotValueFloat = 0f;
        [DrawIf("SlotType", ShowValueEnum.RangeFloat)]
        public float slotIncrementFloat;
        public MenuDetailSection menuDetailSection;
    }

    public MenuItemCodes[] menuItemCodes;
    protected int[] prevMenuItemCodes;
    public int currentMenuVCode = 0;
    protected int prevMenuVCode = 0;
    protected VerticalLayoutGroup mainLayout;
    protected int currentVSlot;
    public bool useSlotLimits = false;
    public int slotLimit = 1;
    protected float initialSlotPosY;
    protected float currentSlotPosY;
    protected float initialLayoutPosY;
    public bool overRunH = false;
    public bool overRun = false;
    public int scrollDownWait = 0;
    public int scrollUpWait = 0;
    protected bool coRountineActive = false;
    protected bool scriptOn = false;

    protected GameObject[] menuSlots;

    // Use this for initialization
    void Start()
    {
        setUpMenu();
        initializeVerticalScreen();
        initializePlayerAssignment();
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
            if (menuDetailsRoot != null)
            {
                menuDetailsRoot.updateDetails(menuItemCodes[currentMenuVCode].menuDetailSection);
            }
            if (useScreenHighlighter)
            {
                float posX = ((menuSlots[currentMenuVCode].transform.parent.transform.position.x - (menuSlots[currentMenuVCode].transform.parent.GetComponent<RectTransform>().rect.width / 2)) - menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.x);
                float posY = ((menuSlots[currentMenuVCode].transform.parent.transform.position.y + (menuSlots[currentMenuVCode].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.y - (menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.height * currentMenuVCode));
                screenHighlighter[0].transform.position = new Vector3(posX, posY, screenHighlighter[0].transform.position.z);
            }
            StartCoroutine("summonMenu", endMoveY);
        }
    }

    public override void OnDisable()
    {

    }

    protected virtual void initializeVerticalScreen()
    {
        prevMenuItemCodes = new int[menuItemCodes.Length];
        for (int i = 0; i < menuItemCodes.Length; i++)
        {
            prevMenuItemCodes[i] = menuItemCodes[i].currentMenuHCode;
        }
        scrollDownWait = 0;
        scrollUpWait = 0;
        currentVSlot = currentMenuVCode;
        mainLayout = gameObject.GetComponentInChildren<VerticalLayoutGroup>();
        GameObject mainLayoutGM = mainLayout.gameObject;
        menuSlots = new GameObject[mainLayoutGM.transform.childCount];
        for (int i = 0; i < mainLayoutGM.transform.childCount; i++)
        {
            menuSlots[i] = mainLayoutGM.transform.GetChild(i).gameObject;
        }
        overRunH = false;
        overRun = false;

        //slotLimit = System.Convert.ToInt32(Mathf.Floor((gameObject.transform.Find("OptionMask").GetComponent<SpriteMask>().transform.localScale.y) / (menuSlots[0].GetComponent<RectTransform>().rect.height)));
        if (useSlotLimits)
        {
            initialSlotPosY = ((menuSlots[slotLimit - 1].transform.parent.transform.position.y + (menuSlots[slotLimit - 1].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.y - (menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.height * (slotLimit - 1)));
            currentSlotPosY = (menuSlots[slotLimit - 1].transform.parent.transform.position.y + (menuSlots[slotLimit - 1].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.y;
            initialLayoutPosY = gameObject.GetComponentInChildren<VerticalLayoutGroup>().gameObject.transform.position.y;
        }

        float posX = ((menuSlots[currentMenuVCode].transform.parent.transform.position.x - (menuSlots[currentMenuVCode].transform.parent.GetComponent<RectTransform>().rect.width / 2)) - menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.x);
        float posY = ((menuSlots[currentMenuVCode].transform.parent.transform.position.y + (menuSlots[currentMenuVCode].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.y - (menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.height * currentMenuVCode));

        int mICCount = 0;
        foreach (MenuItemCodes mIC in menuItemCodes)
        {
            if (mIC.m_MenuDataCollect != null)
            {
                mIC.m_MenuDataCollect.Invoke(menuName, mICCount);
            }
            mICCount++;
        }

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

        /*for (int i = 0; i < menuSlots.Length; i++)
        {
            foreach (Transform child in menuSlots[i].transform)
            {
                if (child.gameObject.name == "MenuScreenArrows")
                {
                    bool current = false;
                    if (i == currentMenuHCode)
                    {
                        current = true;
                    }
                    if (current)
                    {
                        //menuSlots[i].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().enabled = true;
                        //menuSlots[i].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().enabled = true;
                        menuSlots[i].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().setAnimation(1);
                        menuSlots[i].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().setAnimation(1);
                    }
                    if (menuItemCodes[i].currentMenuHCode <= 0)
                    {
                        //menuSlots[i].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().enabled = true;
                        menuSlots[i].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().setAnimation(0);
                    }
                    if (menuItemCodes[i].currentMenuHCode >= menuItemCodes[i].menuItemMax - 1)
                    {
                        //menuSlots[i].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().enabled = true;
                        menuSlots[i].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().setAnimation(0);
                    }
                }
            }
        }*/
    }

    protected void initializePlayerAssignment()
    {
        csPlayerInput = GameObject.Find("Player " + pMenuCode).GetComponent<CSPlayerInput>();
    }

    protected virtual void checkForVerticalScroll()
    {
        if (prevMenuVCode != currentMenuVCode)
        {
            updateVerticalScroll();
        }
    }

    protected virtual void checkForHorizontalScroll()
    {
        if (prevMenuItemCodes[currentMenuVCode] != menuItemCodes[currentMenuVCode].currentMenuHCode || overRunH)
        {
            updateHorizontalScroll();
        }
    }

    protected virtual void updateVerticalScroll()
    {
        summonUI();
        int prevVSlot = prevMenuVCode;
        prevMenuVCode = currentMenuVCode;
        
        if (useSlotLimits)
        {
            if ((prevVSlot < prevMenuVCode && !overRun) || (prevMenuVCode < prevVSlot && overRun))
            {
                currentVSlot++;
            }
            else if ((prevVSlot > prevMenuVCode && !overRun) || (prevMenuVCode > prevVSlot && overRun))
            {
                currentVSlot--;
            }
            if (currentVSlot >= slotLimit && !overRun)
            {
                currentVSlot = slotLimit - 1;
                if (coRountineActive)
                {
                    StopCoroutine("scrollSlots");
                    scrollDownWait--;
                    coRountineActive = false;
                }
                scrollDownWait++;
                StartCoroutine("scrollSlots", true);
            }
            else if (currentVSlot < 0 && !overRun)
            {
                currentVSlot = 0;
                if (coRountineActive)
                {
                    StopCoroutine("scrollSlots");
                    scrollUpWait--;
                    coRountineActive = false;
                }
                scrollUpWait++;
                StartCoroutine("scrollSlots", false);
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
            }
            else
            {
                if (prevVSlot < prevMenuVCode)
                {
                    currentSlotPosY -= menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.height;
                }
                else if (prevVSlot > prevMenuVCode)
                {
                    currentSlotPosY += menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.height;
                }
            }
        }
        
        overRun = false;
        if (menuDetailsRoot != null)
        {
            menuDetailsRoot.updateDetails(menuItemCodes[currentMenuVCode].menuDetailSection);
        }
    }

    protected virtual void updateHorizontalScroll()
    {
        
    }

    public virtual IEnumerator scrollSlots(bool down)
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
            curPos = gameObject.GetComponentInChildren<VerticalLayoutGroup>().gameObject.transform.position;
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
            curPos = gameObject.GetComponentInChildren<VerticalLayoutGroup>().gameObject.transform.position;
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
                gameObject.GetComponentInChildren<VerticalLayoutGroup>().gameObject.transform.position = new Vector3(curPos.x, posY, curPos.z);
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
                gameObject.GetComponentInChildren<VerticalLayoutGroup>().gameObject.transform.position = new Vector3(curPos.x, posY, curPos.z);
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

    protected virtual void summonReset(bool down)
    {
        if (down)
        {
            Vector3 curPos = gameObject.GetComponentInChildren<VerticalLayoutGroup>().gameObject.transform.position;
            float posY = curPos.y;
            gameObject.GetComponentInChildren<VerticalLayoutGroup>().gameObject.transform.position = new Vector3(curPos.x, initialLayoutPosY, curPos.z);
            float posYb = posY - initialLayoutPosY;
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
            currentSlotPosY = (menuSlots[slotLimit - 1].transform.parent.transform.position.y + (menuSlots[slotLimit - 1].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.y;
            scrollDownWait--;
        }
        else
        {
            Vector3 curPos = gameObject.GetComponentInChildren<VerticalLayoutGroup>().gameObject.transform.position;
            float posY = curPos.y;
            gameObject.GetComponentInChildren<VerticalLayoutGroup>().gameObject.transform.position = new Vector3(curPos.x, initialLayoutPosY + (menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.height * (menuItemCodes.Length - slotLimit)), curPos.z);
            float posYb = (initialLayoutPosY + (menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.height * (menuItemCodes.Length - slotLimit))) - posY;
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
            currentSlotPosY = (initialSlotPosY) + (menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.height * (slotLimit - 1)) - (menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.height * (menuItemCodes.Length - 1));
            scrollUpWait--;
        }
    }

    protected virtual void summonUI()
    {
        if (useScreenHighlighter)
        {
            summonHighlighter();
        }
    }

    protected void summonHighlighter()
    {
        float posX = ((menuSlots[currentMenuVCode].transform.parent.transform.position.x - (menuSlots[currentMenuVCode].transform.parent.GetComponent<RectTransform>().rect.width / 2)) - menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.x);
        float posY = ((menuSlots[currentMenuVCode].transform.parent.transform.position.y + (menuSlots[currentMenuVCode].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.y - (menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.height * currentMenuVCode + (currentMenuVCode * mainLayout.spacing)));

        screenHighlighter.Add(Instantiate(screenHighlighterPrefab, new Vector3(posX, posY, -2f), Quaternion.identity));
        screenHighlighter[screenHighlighter.Count - 1].transform.parent = gameObject.transform;
        screenHighlighter[screenHighlighter.Count - 1].GetComponent<ScreenHighlighter>().summonInstant = false;
        if (screenHighlighter.Count > 1)
        {
            screenHighlighter[screenHighlighter.Count - 2].GetComponent<ScreenHighlighter>().StartCoroutine("destroyHighlighter");
        }
        //screenHighlighter.RemoveAt(screenHighlighter.Count - 2);
    }

    protected void resetHighlighter()
    {
        for (int i = 0; i < screenHighlighter.Count; i++)
        {
            Destroy(screenHighlighter[i]);
        }
        screenHighlighter.Clear();
        float posX = ((menuSlots[currentMenuVCode].transform.parent.transform.position.x - (menuSlots[currentMenuVCode].transform.parent.GetComponent<RectTransform>().rect.width / 2)) - menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.x);
        float posY = ((menuSlots[currentMenuVCode].transform.parent.transform.position.y + (menuSlots[currentMenuVCode].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.y - (menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.height * currentMenuVCode + (currentMenuVCode * mainLayout.spacing)));
        screenHighlighter.Add(Instantiate(screenHighlighterPrefab, new Vector3(posX, posY, -2f), Quaternion.identity));
        screenHighlighter[0].transform.parent = gameObject.transform;
    }

    public override void removeHighlighter()
    {
        screenHighlighter.RemoveAt(0);
    }

    public override void selectOption()
    {
        csPlayerInput = GameObject.Find("Player " + pMenuCode).GetComponent<CSPlayerInput>();
        if (menuItemCodes[currentMenuVCode].SlotType == ShowValueEnum.Confirm || menuItemCodes[currentMenuVCode].SlotType == ShowValueEnum.Access)
        {
            EventSystem.current.SetSelectedGameObject(menuSlots[currentMenuVCode]);
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public override void directionalController()
    {
        padHorizontal = Input.GetAxisRaw("Horizontal_" + pMenuCode);
        padVertical = Input.GetAxisRaw("Vertical_" + pMenuCode);
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
        if (inputReader.useNewInput("MoveLeft", pMenuCode) && scrollFrames <= 0)
        {
            sfxPlayer.PlaySound("Scroll");
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
            scrollFrames = ((holdScroll) ? 5f : 20f);
            holdScroll = true;
        }
        else if (inputReader.useNewInput("MoveRight", pMenuCode) && scrollFrames <= 0)
        {
            sfxPlayer.PlaySound("Scroll");
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
            scrollFrames = ((holdScroll) ? 5f : 20f);
            holdScroll = true;
        }
        else if ((inputReader.useNewInput("MoveDown", pMenuCode) || inputReader.useNewInput("MoveDown_Left", pMenuCode) || inputReader.useNewInput("MoveDown_Right", pMenuCode)) && scrollFrames <= 0 && (!inputReader.useNewInput("MoveLeft", pMenuCode) && !inputReader.useNewInput("MoveRight", pMenuCode)) && scrollUpWait <= 0)
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
        else if ((inputReader.useNewInput("MoveUp", pMenuCode) || inputReader.useNewInput("MoveUp_Left", pMenuCode) || inputReader.useNewInput("MoveUp_Right", pMenuCode)) && scrollFrames <= 0 && (!inputReader.useNewInput("MoveLeft", pMenuCode) && !inputReader.useNewInput("MoveRight", pMenuCode)) && scrollDownWait <= 0)
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
            if (inputReader.checkReleased(pMenuCode))
            {
                scrollFrames = 0f;
                holdScroll = false;
            }
        }
    }

    public virtual void updatedData_Arrow(int setting)
    {
        
    }

    public virtual void updateMenu(string menuType, int mode)
    {

    }
}
