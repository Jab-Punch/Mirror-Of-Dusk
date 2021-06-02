using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEditor;
using TMPro;

public class VerticalMenuScreen : NewMenuScreenRoot {

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
    private int[] prevMenuItemCodes;
    public int currentMenuVCode = 0;
    private int prevMenuVCode = 0;
    private int currentHSlot;
    private int currentVSlot;
    private int slotLimit;
    private float initialSlotPosY;
    private float currentSlotPosY;
    private float initialLayoutPosY;
    public bool overRunH = false;
    public bool overRun = false;
    public int scrollDownWait = 0;
    public int scrollUpWait = 0;
    private bool coRountineActive = false;
    private bool scriptOn = false;

    private GameObject[] menuSlots;

    /*Transform searchSquare;
    Material[] coloredSquaresA;
    Material[] coloredSquaresB;
    Material[] coloredSquaresC;*/

    // Use this for initialization
    void Start () {
        setUpMenu();
        /*if (gameObject.name.Contains("ColorMenu"))
        {
            reinitializeColors();
        }*/
        prevMenuItemCodes = new int[menuItemCodes.Length];
        for (int i = 0; i < menuItemCodes.Length; i++)
        {
            prevMenuItemCodes[i] = menuItemCodes[i].currentMenuHCode;
        }
        scrollDownWait = 0;
        scrollUpWait = 0;
        currentVSlot = currentMenuVCode;
        GameObject mainLayout = gameObject.GetComponentInChildren<VerticalLayoutGroup>().gameObject;
        menuSlots = new GameObject[mainLayout.transform.childCount];
        for (int i = 0; i < mainLayout.transform.childCount; i++)
        {
            menuSlots[i] = mainLayout.transform.GetChild(i).gameObject;
        }
        overRunH = false;
        overRun = false;
        slotLimit = System.Convert.ToInt32(Mathf.Floor((gameObject.transform.Find("OptionMask").GetComponent<SpriteMask>().transform.localScale.y) / (menuSlots[0].GetComponent<RectTransform>().rect.height)));
        initialSlotPosY = ((menuSlots[slotLimit - 1].transform.parent.transform.position.y + (menuSlots[slotLimit - 1].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.y - (menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.height * (slotLimit - 1)));
        currentSlotPosY = (menuSlots[slotLimit - 1].transform.parent.transform.position.y + (menuSlots[slotLimit - 1].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.y;
        initialLayoutPosY = gameObject.GetComponentInChildren<VerticalLayoutGroup>().gameObject.transform.position.y;

        float posX = ((menuSlots[0].transform.parent.transform.position.x - (menuSlots[0].transform.parent.GetComponent<RectTransform>().rect.width / 2)) - menuSlots[0].GetComponent<RectTransform>().rect.x + (menuSlots[0].GetComponent<RectTransform>().rect.width * 0));
        float posY = ((menuSlots[0].transform.parent.transform.position.y + (menuSlots[0].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[0].GetComponent<RectTransform>().rect.y - (menuSlots[0].GetComponent<RectTransform>().rect.height * 0));

        int mICCount = 0;
        foreach (MenuItemCodes mIC in menuItemCodes)
        {
            if (mIC.m_MenuDataCollect != null)
            {
                mIC.m_MenuDataCollect.Invoke(menuName, mICCount);
            }
            mICCount++;
        }
        currentHSlot = menuItemCodes[currentMenuVCode].currentMenuHCode;
        screenHighlighter = new List<GameObject>();
        screenHighlighter.Add(Instantiate(screenHighlighterPrefab, new Vector3(posX, posY, -2f), Quaternion.identity));
        screenHighlighter[0].transform.parent = gameObject.transform;
        float curAlpha = 0;
        SpriteRenderer sprSH = screenHighlighter[0].GetComponent<SpriteRenderer>();
        SpriteRenderer sprSHC = screenHighlighter[0].GetComponentInChildren<SpriteRenderer>();
        sprSH.color = new Color(sprSH.color.r, sprSH.color.g, sprSH.color.b, curAlpha);
        sprSHC.color = new Color(sprSHC.color.r, sprSHC.color.g, sprSHC.color.b, curAlpha);
        gameObject.transform.Find("MenuScreenMoreUp").GetComponent<SpriteRenderer>().enabled = false;
        gameObject.transform.Find("MenuScreenMoreDown").GetComponent<SpriteRenderer>().enabled = true;
        for (int i = 0; i < menuSlots.Length; i++)
        {
            foreach (Transform child in menuSlots[i].transform)
            {
                if (child.gameObject.name == "MenuScreenArrows")
                {
                    bool current = false;
                    if (i == currentMenuVCode)
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
        }
        //StartCoroutine("summonMenu", endMoveY);
        csPlayerInput = GameObject.Find("Player " + pMenuCode).GetComponent<CSPlayerInput>();
        scriptOn = true;
        gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
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
            StartCoroutine("summonMenu", endMoveY);
        }
    }

    public override void OnDisable()
    {

    }

    private void checkForVerticalScroll()
    {
        if (prevMenuVCode != currentMenuVCode)
        {
            foreach (Transform child in menuSlots[prevMenuVCode].transform)
            {
                if (child.gameObject.name == "MenuScreenArrows")
                {
                    //child.gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().enabled = true;
                    //child.gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().enabled = true;
                    child.gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().setAnimation(2);
                    child.gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().setAnimation(2);
                    if (menuItemCodes[prevMenuVCode].currentMenuHCode <= 0)
                    {
                        //child.gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().enabled = true;
                        child.gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().setAnimation(0);
                    }
                    if (menuItemCodes[prevMenuVCode].currentMenuHCode >= menuItemCodes[prevMenuVCode].menuItemMax - 1)
                    {
                        //child.gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().enabled = true;
                        child.gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().setAnimation(0);
                    }
                }
            }
            summonHighlighter();
            int prevVSlot = prevMenuVCode;
            prevMenuVCode = currentMenuVCode;
            foreach (Transform child in menuSlots[prevMenuVCode].transform)
            {
                if (child.gameObject.name == "MenuScreenArrows")
                {
                    //child.gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().enabled = true;
                    //child.gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().enabled = true;
                    child.gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().setAnimation(1);
                    child.gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().setAnimation(1);
                    if (menuItemCodes[prevMenuVCode].currentMenuHCode <= 0)
                    {
                        //child.gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().enabled = true;
                        child.gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().setAnimation(0);
                    }
                    if (menuItemCodes[prevMenuVCode].currentMenuHCode >= menuItemCodes[prevMenuVCode].menuItemMax - 1)
                    {
                        //child.gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().enabled = true;
                        child.gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().setAnimation(0);
                    }
                }
            }
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

                gameObject.transform.Find("MenuScreenMoreUp").GetComponent<SpriteRenderer>().enabled = true;
                //Color sprColor = gameObject.GetComponent<SpriteRenderer>().color;
                gameObject.transform.Find("MenuScreenMoreUp").GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                if (currentMenuVCode >= menuItemCodes.Length - 1) {
                    gameObject.transform.Find("MenuScreenMoreDown").GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                    gameObject.transform.Find("MenuScreenMoreDown").GetComponent<SpriteRenderer>().enabled = false;
                }
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
                gameObject.transform.Find("MenuScreenMoreDown").GetComponent<SpriteRenderer>().enabled = true;
                gameObject.transform.Find("MenuScreenMoreDown").GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                if (currentMenuVCode <= 0) {
                    gameObject.transform.Find("MenuScreenMoreUp").GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                    gameObject.transform.Find("MenuScreenMoreUp").GetComponent<SpriteRenderer>().enabled = false;
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
                gameObject.transform.Find("MenuScreenMoreUp").GetComponent<SpriteRenderer>().enabled = true;
                gameObject.transform.Find("MenuScreenMoreUp").GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                gameObject.transform.Find("MenuScreenMoreDown").GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                gameObject.transform.Find("MenuScreenMoreDown").GetComponent<SpriteRenderer>().enabled = false;
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
                gameObject.transform.Find("MenuScreenMoreDown").GetComponent<SpriteRenderer>().enabled = true;
                gameObject.transform.Find("MenuScreenMoreDown").GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                gameObject.transform.Find("MenuScreenMoreUp").GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                gameObject.transform.Find("MenuScreenMoreUp").GetComponent<SpriteRenderer>().enabled = false;
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
            overRun = false;
            if (menuDetailsRoot != null)
            {
                menuDetailsRoot.updateDetails(menuItemCodes[currentMenuVCode].menuDetailSection);
            }
        }
    }

    private void checkForHorizontalScroll()
    {
        if (prevMenuItemCodes[currentMenuVCode] != menuItemCodes[currentMenuVCode].currentMenuHCode || overRunH)
        {
            int prevHSlot = prevMenuItemCodes[currentMenuVCode];
            currentHSlot = menuItemCodes[currentMenuVCode].currentMenuHCode;
            prevMenuItemCodes[currentMenuVCode] = menuItemCodes[currentMenuVCode].currentMenuHCode;
            if ((prevHSlot < prevMenuItemCodes[currentMenuVCode] && !overRunH) || (prevMenuItemCodes[currentMenuVCode] < prevHSlot && overRunH))
            {
                currentHSlot++;
            }
            else if ((prevHSlot > prevMenuItemCodes[currentMenuVCode] && !overRunH) || (prevMenuItemCodes[currentMenuVCode] > prevHSlot && overRunH))
            {
                currentHSlot--;
            }
            switch (menuItemCodes[currentMenuVCode].SlotType)
            {
                case ShowValueEnum.Access:
                    break;
                case ShowValueEnum.Confirm:
                    break;
                case ShowValueEnum.Single:
                    menuSlots[currentMenuVCode].transform.Find("OptionChoice").GetComponent<TextMeshProUGUI>().text = menuItemCodes[currentMenuVCode].slotValueSingle[menuItemCodes[currentMenuVCode].currentMenuHCode];
                    menuSlots[currentMenuVCode].GetComponent<UI_InputTrigger>().InvokeUpdateData();
                    break;
                case ShowValueEnum.RangeInt:
                    menuSlots[currentMenuVCode].transform.Find("OptionChoice").GetComponent<TextMeshProUGUI>().text = (menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt)).ToString();
                    fixNumberName(currentMenuVCode);
                    menuSlots[currentMenuVCode].GetComponent<UI_InputTrigger>().InvokeUpdateData();
                    break;
                case ShowValueEnum.RangeFloat:
                    menuSlots[currentMenuVCode].transform.Find("OptionChoice").GetComponent<TextMeshProUGUI>().text = (menuItemCodes[currentMenuVCode].slotValueFloat + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementFloat)).ToString("F1");
                    menuSlots[currentMenuVCode].GetComponent<UI_InputTrigger>().InvokeUpdateData();
                    break;
                default:
                    break;
            }
            foreach (Transform child in menuSlots[currentMenuVCode].transform)
            {
                if (child.gameObject.name == "MenuScreenArrows")
                {
                    if (menuSlots[currentMenuVCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().animState != 1)
                    {
                        //menuSlots[currentMenuVCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().enabled = true;
                        menuSlots[currentMenuVCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().setAnimation(1);
                        if (menuSlots[currentMenuVCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().animState == 1)
                        {
                            int tempFrame = menuSlots[currentMenuVCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().getFrame();
                            int tempSpeed = menuSlots[currentMenuVCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().getSpeed();
                            menuSlots[currentMenuVCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().setFrame(tempFrame, tempSpeed);
                        }
                    }
                    if (menuSlots[currentMenuVCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().animState != 1)
                    {
                        //menuSlots[currentMenuVCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().enabled = true;
                        menuSlots[currentMenuVCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().setAnimation(1);
                        if (menuSlots[currentMenuVCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().animState == 1)
                        {
                            int tempFrame = menuSlots[currentMenuVCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().getFrame();
                            int tempSpeed = menuSlots[currentMenuVCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().getSpeed();
                            menuSlots[currentMenuVCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().setFrame(tempFrame, tempSpeed);
                        }
                    }
                    if (menuItemCodes[currentMenuVCode].currentMenuHCode <= 0)
                    {
                        //menuSlots[currentMenuVCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().enabled = true;
                        menuSlots[currentMenuVCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().setAnimation(0);
                        //menuSlots[currentMenuVCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().StartCoroutine("setCoAnimation", 0);
                    }
                    if (menuItemCodes[currentMenuVCode].currentMenuHCode >= menuItemCodes[currentMenuVCode].menuItemMax - 1)
                    {
                        //menuSlots[currentMenuVCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().enabled = true;
                        menuSlots[currentMenuVCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().setAnimation(0);
                        //menuSlots[currentMenuVCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().StartCoroutine("setCoAnimation", 0);
                    }
                }
            }
            overRunH = false;
        }
    }

    public IEnumerator scrollSlots(bool down)
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

    private void summonReset(bool down)
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

    private void summonHighlighter()
    {
        float posX = ((menuSlots[currentMenuVCode].transform.parent.transform.position.x - (menuSlots[currentMenuVCode].transform.parent.GetComponent<RectTransform>().rect.width / 2)) - menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.x);
        float posY = ((menuSlots[currentMenuVCode].transform.parent.transform.position.y + (menuSlots[currentMenuVCode].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.y - (menuSlots[currentMenuVCode].GetComponent<RectTransform>().rect.height * currentMenuVCode));

        screenHighlighter.Add(Instantiate(screenHighlighterPrefab, new Vector3(posX, posY, -2f), Quaternion.identity));
        screenHighlighter[screenHighlighter.Count - 1].transform.parent = gameObject.transform;
        screenHighlighter[screenHighlighter.Count - 1].GetComponent<ScreenHighlighter>().summonInstant = false;
        screenHighlighter[screenHighlighter.Count - 2].GetComponent<ScreenHighlighter>().StartCoroutine("destroyHighlighter");
        //screenHighlighter.RemoveAt(screenHighlighter.Count - 2);
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
        if (menuItemCodes[currentMenuVCode].SlotType == ShowValueEnum.RangeInt)
        {
            if (currentMenuVCode == 7)
            {
                sfxPlayer.PlaySound("Confirm");
                CSPlayerData[] csPD = new CSPlayerData[4];
                for (int c = 0; c < 4; c++)
                {
                    csPD[c] = GameObject.Find("Player " + (c+1).ToString()).GetComponent<CSPlayerData>();
                    csPD[c].playerHealth = gameData.defaultInitialHealth;
                }
            }
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
        scrollTime++;
        if (scrollFrames <= 0)
        {
            scrollFrames = 0f;
        }
        if (inputReader.useNewInput("MoveLeft", pMenuCode) && scrollFrames <= 0)
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
        else if (inputReader.useNewInput("MoveRight", pMenuCode) && scrollFrames <= 0)
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
                scrollTime = 0;
            }
        }
    }

    public void updatedData_Arrow(int setting)
    {
        csPlayerInput = GameObject.Find("Player " + pMenuCode).GetComponent<CSPlayerInput>();
        if (setting == 0)
        {
            gameData.modeStyle = menuItemCodes[currentMenuVCode].currentMenuHCode;
        }
        else if (setting == 1)
        {
            gameData.modeReflection = menuItemCodes[currentMenuVCode].currentMenuHCode;
        }
        else if (setting == 2)
        {
            gameData.defaultTotalShards = (menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt));
        }
        else if (setting == 3)
        {
            gameData.defaultStockCount = (menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt));
        }
        else if (setting == 4)
        {
            gameData.timerSetting = (menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt));
        }
        else if (setting == 7)
        {
            gameData.defaultInitialHealth = (menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt));
            if (gameData.defaultInitialHealth > 9999) { gameData.defaultInitialHealth = 9999; }
        }
        else if (setting == 8)
        {
            gameData.defaultInitialShards = (menuItemCodes[currentMenuVCode].slotValueInt + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementInt));
        }
        else if (setting == 9)
        {
            gameData.damageRatio = (float)(menuItemCodes[currentMenuVCode].slotValueFloat + (menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementFloat));
        }
        else if (setting == 10)
        {
            gameData.barrierRatio = (float)(menuItemCodes[currentMenuVCode].slotValueFloat + ((float)menuItemCodes[currentMenuVCode].currentMenuHCode * menuItemCodes[currentMenuVCode].slotIncrementFloat));
        }
    }

    public void updateMenu(string menuType, int mode)
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
                else if (mode == 7)
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
