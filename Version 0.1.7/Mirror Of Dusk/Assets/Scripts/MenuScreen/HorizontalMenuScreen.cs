using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEditor;
using TMPro;

public class HorizontalMenuScreen : NewMenuScreenRoot {

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
    public int currentMenuHCode = 0;
    protected int prevMenuHCode = 0;
    protected int currentHSlot;
    protected int currentVSlot;
    protected int slotLimit;
    protected float initialSlotPosX;
    protected float currentSlotPosX;
    protected float initialLayoutPosX;
    public bool scrollOn = true;
    public bool overRunH = false;
    public bool overRun = false;
    public int scrollDownWait = 0;
    public int scrollUpWait = 0;
    protected bool coRountineActive = false;
    protected bool scriptOn = false;

    protected GameObject[] menuSlots;
    public GameObject optionMask;

    /*Transform searchSquare;
    Material[] coloredSquaresA;
    Material[] coloredSquaresB;
    Material[] coloredSquaresC;*/

    // Use this for initialization
    void Start()
    {
        setUpMenu();
        initializeHorizontalScreen();
        scriptOn = true;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
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
            float posX = ((menuSlots[currentMenuHCode].transform.parent.transform.position.x - (menuSlots[currentMenuHCode].transform.parent.GetComponent<RectTransform>().rect.width / 2)) - menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.x + (menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.width * currentMenuHCode));
            float posY = ((menuSlots[currentMenuHCode].transform.parent.transform.position.y + (menuSlots[currentMenuHCode].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.y);
            screenHighlighter[0].transform.position = new Vector3(posX, posY, screenHighlighter[0].transform.position.z);

            StartCoroutine("summonMenu", endMoveY);
        }
    }

    public override void OnDisable()
    {

    }

    protected virtual void initializeHorizontalScreen()
    {
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
        currentHSlot = currentMenuHCode;
        GameObject mainLayout = gameObject.GetComponentInChildren<HorizontalLayoutGroup>().gameObject;
        menuSlots = new GameObject[mainLayout.transform.childCount];
        for (int i = 0; i < mainLayout.transform.childCount; i++)
        {
            menuSlots[i] = mainLayout.transform.GetChild(i).gameObject;
        }
        overRunH = false;
        overRun = false;
        slotLimit = System.Convert.ToInt32(Mathf.Floor((gameObject.transform.Find("OptionMask").GetComponent<SpriteMask>().transform.localScale.x) / (menuSlots[0].GetComponent<RectTransform>().rect.width)));
        initialSlotPosX = ((menuSlots[slotLimit - 1].transform.parent.transform.position.x + (menuSlots[slotLimit - 1].transform.parent.GetComponent<RectTransform>().rect.width / 2)) + menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.x - (menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.width * (slotLimit - 1)));
        currentSlotPosX = (menuSlots[slotLimit - 1].transform.parent.transform.position.x + (menuSlots[slotLimit - 1].transform.parent.GetComponent<RectTransform>().rect.width / 2)) + menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.x;
        initialLayoutPosX = gameObject.GetComponentInChildren<HorizontalLayoutGroup>().gameObject.transform.position.x;

        float posX = ((menuSlots[currentMenuHCode].transform.parent.transform.position.x - (menuSlots[currentMenuHCode].transform.parent.GetComponent<RectTransform>().rect.width / 2)) - menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.x + (menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.width * currentMenuHCode));
        float posY = ((menuSlots[currentMenuHCode].transform.parent.transform.position.y + (menuSlots[currentMenuHCode].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.y);

        int mICCount = 0;
        foreach (MenuItemCodes mIC in menuItemCodes)
        {
            if (mIC.m_MenuDataCollect != null)
            {
                mIC.m_MenuDataCollect.Invoke(menuName, mICCount);
            }
            mICCount++;
        }
        currentVSlot = menuItemCodes[currentMenuHCode].currentMenuHCode;
        screenHighlighter = new List<GameObject>();
        screenHighlighter.Add(Instantiate(screenHighlighterPrefab, new Vector3(posX, posY, -2f), Quaternion.identity));
        screenHighlighter[0].transform.parent = gameObject.transform;
        float curAlpha;
        if (appearMode == "Fade")
        {
            curAlpha = 0;
        } else
        {
            curAlpha = 0.5f;
        }
        SpriteRenderer sprSH = screenHighlighter[0].GetComponent<SpriteRenderer>();
        SpriteRenderer sprSHC = screenHighlighter[0].GetComponentInChildren<SpriteRenderer>();
        sprSH.color = new Color(sprSH.color.r, sprSH.color.g, sprSH.color.b, curAlpha);
        sprSHC.color = new Color(sprSHC.color.r, sprSHC.color.g, sprSHC.color.b, curAlpha);
        //gameObject.transform.Find("MenuScreenMoreUp").GetComponent<SpriteRenderer>().enabled = false;
        //gameObject.transform.Find("MenuScreenMoreDown").GetComponent<SpriteRenderer>().enabled = true;
        for (int i = 0; i < menuSlots.Length; i++)
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
        }
        //StartCoroutine("summonMenu", endMoveY);
        csPlayerInput = GameObject.Find("Player " + pMenuCode).GetComponent<CSPlayerInput>();
    }

    protected virtual void checkForHorizontalScroll()
    {
        if (prevMenuHCode != currentMenuHCode)
        {
            updateHorizontalScroll();
        }
    }

    protected virtual void checkForVerticalScroll()
    {
        if (prevMenuItemCodes[currentMenuHCode] != menuItemCodes[currentMenuHCode].currentMenuHCode || overRunH)
        {
            int prevHSlot = prevMenuItemCodes[currentMenuHCode];
            currentVSlot = menuItemCodes[currentMenuHCode].currentMenuHCode;
            prevMenuItemCodes[currentMenuHCode] = menuItemCodes[currentMenuHCode].currentMenuHCode;
            if ((prevHSlot < prevMenuItemCodes[currentMenuHCode] && !overRunH) || (prevMenuItemCodes[currentMenuHCode] < prevHSlot && overRunH))
            {
                currentVSlot++;
            }
            else if ((prevHSlot > prevMenuItemCodes[currentMenuHCode] && !overRunH) || (prevMenuItemCodes[currentMenuHCode] > prevHSlot && overRunH))
            {
                currentVSlot--;
            }
            switch (menuItemCodes[currentMenuHCode].SlotType)
            {
                case ShowValueEnum.Access:
                    break;
                case ShowValueEnum.Confirm:
                    break;
                case ShowValueEnum.Single:
                    menuSlots[currentMenuHCode].transform.Find("OptionChoice").GetComponent<TextMeshProUGUI>().text = menuItemCodes[currentMenuHCode].slotValueSingle[menuItemCodes[currentMenuHCode].currentMenuHCode];
                    menuSlots[currentMenuHCode].GetComponent<UI_InputTrigger>().InvokeUpdateData();
                    break;
                case ShowValueEnum.RangeInt:
                    menuSlots[currentMenuHCode].transform.Find("OptionChoice").GetComponent<TextMeshProUGUI>().text = (menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)).ToString();
                    fixNumberName(currentMenuHCode);
                    menuSlots[currentMenuHCode].GetComponent<UI_InputTrigger>().InvokeUpdateData();
                    break;
                case ShowValueEnum.RangeFloat:
                    menuSlots[currentMenuHCode].transform.Find("OptionChoice").GetComponent<TextMeshProUGUI>().text = (menuItemCodes[currentMenuHCode].slotValueFloat + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementFloat)).ToString("F1");
                    menuSlots[currentMenuHCode].GetComponent<UI_InputTrigger>().InvokeUpdateData();
                    break;
                default:
                    break;
            }
            foreach (Transform child in menuSlots[currentMenuHCode].transform)
            {
                if (child.gameObject.name == "MenuScreenArrows")
                {
                    if (menuSlots[currentMenuHCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().animState != 1)
                    {
                        //menuSlots[currentMenuHCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().enabled = true;
                        menuSlots[currentMenuHCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().setAnimation(1);
                        if (menuSlots[currentMenuHCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().animState == 1)
                        {
                            int tempFrame = menuSlots[currentMenuHCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().getFrame();
                            int tempSpeed = menuSlots[currentMenuHCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().getSpeed();
                            menuSlots[currentMenuHCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().setFrame(tempFrame, tempSpeed);
                        }
                    }
                    if (menuSlots[currentMenuHCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().animState != 1)
                    {
                        //menuSlots[currentMenuHCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().enabled = true;
                        menuSlots[currentMenuHCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().setAnimation(1);
                        if (menuSlots[currentMenuHCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().animState == 1)
                        {
                            int tempFrame = menuSlots[currentMenuHCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().getFrame();
                            int tempSpeed = menuSlots[currentMenuHCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().getSpeed();
                            menuSlots[currentMenuHCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().setFrame(tempFrame, tempSpeed);
                        }
                    }
                    if (menuItemCodes[currentMenuHCode].currentMenuHCode <= 0)
                    {
                        //menuSlots[currentMenuHCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().enabled = true;
                        menuSlots[currentMenuHCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().setAnimation(0);
                        //menuSlots[currentMenuHCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().StartCoroutine("setCoAnimation", 0);
                    }
                    if (menuItemCodes[currentMenuHCode].currentMenuHCode >= menuItemCodes[currentMenuHCode].menuItemMax - 1)
                    {
                        //menuSlots[currentMenuHCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().enabled = true;
                        menuSlots[currentMenuHCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().setAnimation(0);
                        //menuSlots[currentMenuHCode].transform.Find("MenuScreenArrows").gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().StartCoroutine("setCoAnimation", 0);
                    }
                }
            }
            overRunH = false;
        }
    }

    protected void updateHorizontalScroll()
    {
        foreach (Transform child in menuSlots[prevMenuHCode].transform)
        {
            if (child.gameObject.name == "MenuScreenArrows")
            {
                //child.gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().enabled = true;
                //child.gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().enabled = true;
                child.gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().setAnimation(2);
                child.gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().setAnimation(2);
                if (menuItemCodes[prevMenuHCode].currentMenuHCode <= 0)
                {
                    //child.gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().enabled = true;
                    child.gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().setAnimation(0);
                }
                if (menuItemCodes[prevMenuHCode].currentMenuHCode >= menuItemCodes[prevMenuHCode].menuItemMax - 1)
                {
                    //child.gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().enabled = true;
                    child.gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().setAnimation(0);
                }
            }
        }
        summonUI();
        int prevHSlot = prevMenuHCode;
        prevMenuHCode = currentMenuHCode;
        foreach (Transform child in menuSlots[prevMenuHCode].transform)
        {
            if (child.gameObject.name == "MenuScreenArrows")
            {
                //child.gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().enabled = true;
                //child.gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().enabled = true;
                child.gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().setAnimation(1);
                child.gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().setAnimation(1);
                if (menuItemCodes[prevMenuHCode].currentMenuHCode <= 0)
                {
                    //child.gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().enabled = true;
                    child.gameObject.transform.Find("MenuScreenArrowLeft").GetComponent<AnimateGlowArrows>().setAnimation(0);
                }
                if (menuItemCodes[prevMenuHCode].currentMenuHCode >= menuItemCodes[prevMenuHCode].menuItemMax - 1)
                {
                    //child.gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().enabled = true;
                    child.gameObject.transform.Find("MenuScreenArrowRight").GetComponent<AnimateGlowArrows>().setAnimation(0);
                }
            }
        }
        if ((prevHSlot < prevMenuHCode && !overRun) || (prevMenuHCode < prevHSlot && overRun))
        {
            currentHSlot++;
        }
        else if ((prevHSlot > prevMenuHCode && !overRun) || (prevMenuHCode > prevHSlot && overRun))
        {
            currentHSlot--;
        }
        if (currentHSlot >= slotLimit && !overRun)
        {
            currentHSlot = slotLimit - 1;
            if (coRountineActive)
            {
                StopCoroutine("scrollSlots");
                scrollDownWait--;
                coRountineActive = false;
            }
            if (scrollOn)
            {
                scrollDownWait++;
                StartCoroutine("scrollSlots", true);
            }

            /*gameObject.transform.Find("MenuScreenMoreUp").GetComponent<SpriteRenderer>().enabled = true;
            //Color sprColor = gameObject.GetComponent<SpriteRenderer>().color;
            gameObject.transform.Find("MenuScreenMoreUp").GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            if (currentMenuHCode >= menuItemCodes.Length - 1)
            {
                gameObject.transform.Find("MenuScreenMoreDown").GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                gameObject.transform.Find("MenuScreenMoreDown").GetComponent<SpriteRenderer>().enabled = false;
            }*/
        }
        else if (currentHSlot < 0 && !overRun)
        {
            currentHSlot = 0;
            if (coRountineActive)
            {
                StopCoroutine("scrollSlots");
                scrollUpWait--;
                coRountineActive = false;
            }
            if (scrollOn)
            {
                scrollUpWait++;
                StartCoroutine("scrollSlots", false);
            }
            /*gameObject.transform.Find("MenuScreenMoreDown").GetComponent<SpriteRenderer>().enabled = true;
            gameObject.transform.Find("MenuScreenMoreDown").GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            if (currentMenuHCode <= 0)
            {
                gameObject.transform.Find("MenuScreenMoreUp").GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                gameObject.transform.Find("MenuScreenMoreUp").GetComponent<SpriteRenderer>().enabled = false;
            }*/
        }
        else if (currentHSlot <= 0 && overRun)
        {
            currentHSlot = slotLimit - 1;
            if (coRountineActive)
            {
                StopCoroutine("scrollSlots");
                scrollUpWait--;
                coRountineActive = false;
            }
            if (scrollOn)
            {
                scrollUpWait++;
                summonReset(false);
            }
            /*gameObject.transform.Find("MenuScreenMoreUp").GetComponent<SpriteRenderer>().enabled = true;
            gameObject.transform.Find("MenuScreenMoreUp").GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            gameObject.transform.Find("MenuScreenMoreDown").GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            gameObject.transform.Find("MenuScreenMoreDown").GetComponent<SpriteRenderer>().enabled = false;*/
        }
        else if (currentHSlot > 0 && overRun)
        {
            currentHSlot = 0;
            if (coRountineActive)
            {
                StopCoroutine("scrollSlots");
                scrollDownWait--;
                coRountineActive = false;
            }
            if (scrollOn)
            {
                scrollDownWait++;
                summonReset(true);
            }
            /*gameObject.transform.Find("MenuScreenMoreDown").GetComponent<SpriteRenderer>().enabled = true;
            gameObject.transform.Find("MenuScreenMoreDown").GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            gameObject.transform.Find("MenuScreenMoreUp").GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            gameObject.transform.Find("MenuScreenMoreUp").GetComponent<SpriteRenderer>().enabled = false;*/
        }
        else
        {
            if (prevHSlot < prevMenuHCode)
            {
                currentSlotPosX -= menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.width;
            }
            else if (prevHSlot > prevMenuHCode)
            {
                currentSlotPosX += menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.width;
            }
        }
        overRun = false;
        if (menuDetailsRoot != null)
        {
            menuDetailsRoot.updateDetails(menuItemCodes[currentMenuHCode].menuDetailSection);
        }
    }

    public IEnumerator scrollSlots(bool down)
    {
        coRountineActive = true;
        int safeCount = 0;
        float destination;
        float moveSpeedX;
        float posX;
        Vector3 curPos;
        if (down)
        {
            float dX1 = (initialSlotPosX - (initialSlotPosX - currentSlotPosX) - ((menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.width * (currentMenuHCode - (slotLimit - 1)))) - (currentSlotPosX - initialSlotPosX));
            float dX2 = currentSlotPosX;
            destination = dX1 - dX2;
            curPos = gameObject.GetComponentInChildren<HorizontalLayoutGroup>().gameObject.transform.position;
            posX = curPos.x;
            destination = destination + currentSlotPosX;
            moveSpeedX = (currentSlotPosX - destination) / 3f;
        }
        else
        {
            //float upInit = initialSlotPosX + ((menuItemCodes.Length - 1) * menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.height) - (menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.height * (slotLimit - 1));
            //float upInit = ((menuSlots[slotLimit - 1].transform.parent.transform.position.y + (menuSlots[slotLimit - 1].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.y - (menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.height * (slotLimit - 1)));
            float upInit = (initialSlotPosX + (menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.width * (slotLimit - 1))) - (menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.width * (menuItemCodes.Length - 1)) + (menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.width * (slotLimit - 1));
            float dX1 = (upInit + (upInit - currentSlotPosX) + ((menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.width * (menuItemCodes.Length - (slotLimit) - currentMenuHCode))) + (currentSlotPosX - upInit));
            float dX2 = currentSlotPosX;
            destination = dX1 - dX2;
            curPos = gameObject.GetComponentInChildren<HorizontalLayoutGroup>().gameObject.transform.position;
            posX = curPos.x;
            destination = currentSlotPosX + destination;
            moveSpeedX = (currentSlotPosX - destination) / 3f;
        }
        if (down)
        {
            while (currentSlotPosX > destination && safeCount < 100)
            {
                int tempMoveSpeedX = Mathf.RoundToInt(moveSpeedX);
                moveSpeedX = System.Convert.ToSingle(tempMoveSpeedX);
                posX = posX + moveSpeedX;
                currentSlotPosX = currentSlotPosX - moveSpeedX;
                if (moveSpeedX < 1.0f)
                {
                    moveSpeedX = 1.0f;
                }
                float sHSpeedX = moveSpeedX;
                moveSpeedX = moveSpeedX * 0.9f;
                if (currentSlotPosX <= destination)
                {
                    sHSpeedX += (currentSlotPosX - destination);
                    posX += (currentSlotPosX - destination);
                    currentSlotPosX = destination;
                }
                gameObject.GetComponentInChildren<HorizontalLayoutGroup>().gameObject.transform.position = new Vector3(posX, curPos.y, curPos.z);
                foreach (GameObject sH in screenHighlighter)
                {
                    try
                    {
                        sH.transform.position = new Vector3(sH.transform.position.x + sHSpeedX, sH.transform.position.y, sH.transform.position.z);
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
            while (currentSlotPosX < destination && safeCount < 100)
            {
                int tempMoveSpeedX = Mathf.RoundToInt(moveSpeedX);
                moveSpeedX = System.Convert.ToSingle(tempMoveSpeedX);
                posX = posX + moveSpeedX;
                currentSlotPosX = currentSlotPosX - moveSpeedX;
                if (moveSpeedX > -1.0f)
                {
                    moveSpeedX = -1.0f;
                }
                float sHSpeedX = moveSpeedX;
                moveSpeedX = moveSpeedX * 0.9f;
                if (currentSlotPosX >= destination)
                {
                    sHSpeedX -= (destination - currentSlotPosX);
                    posX -= (destination - currentSlotPosX);
                    currentSlotPosX = destination;
                }
                gameObject.GetComponentInChildren<HorizontalLayoutGroup>().gameObject.transform.position = new Vector3(posX, curPos.y, curPos.z);
                foreach (GameObject sH in screenHighlighter)
                {
                    try
                    {
                        sH.transform.position = new Vector3(sH.transform.position.x + sHSpeedX, sH.transform.position.y, sH.transform.position.z);
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

    protected void summonReset(bool down)
    {
        if (down)
        {
            Vector3 curPos = gameObject.GetComponentInChildren<HorizontalLayoutGroup>().gameObject.transform.position;
            float posX = curPos.x;
            gameObject.GetComponentInChildren<HorizontalLayoutGroup>().gameObject.transform.position = new Vector3(initialLayoutPosX, curPos.y, curPos.z);
            float posXb = posX - initialLayoutPosX;
            foreach (GameObject sH in screenHighlighter)
            {
                try
                {
                    float destination = sH.transform.position.x - posXb;
                    sH.transform.position = new Vector3(destination, sH.transform.position.y, sH.transform.position.z);
                }
                catch (MissingReferenceException)
                {

                }
            }
            currentSlotPosX = (menuSlots[slotLimit - 1].transform.parent.transform.position.x + (menuSlots[slotLimit - 1].transform.parent.GetComponent<RectTransform>().rect.width / 2)) + menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.x;
            scrollDownWait--;
        }
        else
        {
            Vector3 curPos = gameObject.GetComponentInChildren<HorizontalLayoutGroup>().gameObject.transform.position;
            float posX = curPos.x;
            gameObject.GetComponentInChildren<HorizontalLayoutGroup>().gameObject.transform.position = new Vector3(initialLayoutPosX + (menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.width * (menuItemCodes.Length - slotLimit)), curPos.y, curPos.z);
            float posXb = (initialLayoutPosX + (menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.width * (menuItemCodes.Length - slotLimit))) - posX;
            foreach (GameObject sH in screenHighlighter)
            {
                try
                {
                    float destination = sH.transform.position.x + posXb;
                    sH.transform.position = new Vector3(destination, sH.transform.position.y, sH.transform.position.z);
                }
                catch (MissingReferenceException)
                {

                }
            }
            currentSlotPosX = (initialSlotPosX) + (menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.width * (slotLimit - 1)) - (menuSlots[slotLimit - 1].GetComponent<RectTransform>().rect.width * (menuItemCodes.Length - 1));
            scrollUpWait--;
        }
    }

    protected virtual void summonUI()
    {
        summonHighlighter();
    }

    protected void summonHighlighter()
    {
        float posX = ((menuSlots[currentMenuHCode].transform.parent.transform.position.x - (menuSlots[currentMenuHCode].transform.parent.GetComponent<RectTransform>().rect.width / 2)) - menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.x + (menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.width * currentMenuHCode));
        float posY = ((menuSlots[currentMenuHCode].transform.parent.transform.position.y + (menuSlots[currentMenuHCode].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.y);

        screenHighlighter.Add(Instantiate(screenHighlighterPrefab, new Vector3(posX, posY, -2f), Quaternion.identity));
        screenHighlighter[screenHighlighter.Count - 1].transform.parent = gameObject.transform;
        screenHighlighter[screenHighlighter.Count - 1].GetComponent<ScreenHighlighter>().summonInstant = false;
        screenHighlighter[screenHighlighter.Count - 2].GetComponent<ScreenHighlighter>().StartCoroutine("destroyHighlighter");
        //screenHighlighter.RemoveAt(screenHighlighter.Count - 2);
    }

    protected void resetHighlighter()
    {
        for (int i = 0; i < screenHighlighter.Count; i++)
        {
            Destroy(screenHighlighter[i]);
        }
        screenHighlighter.Clear();
        float posX = ((menuSlots[currentMenuHCode].transform.parent.transform.position.x - (menuSlots[currentMenuHCode].transform.parent.GetComponent<RectTransform>().rect.width / 2)) - menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.x + (menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.width * currentMenuHCode));
        float posY = ((menuSlots[currentMenuHCode].transform.parent.transform.position.y + (menuSlots[currentMenuHCode].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + menuSlots[currentMenuHCode].GetComponent<RectTransform>().rect.y);
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
        if (menuItemCodes[currentMenuHCode].SlotType == ShowValueEnum.Confirm || menuItemCodes[currentMenuHCode].SlotType == ShowValueEnum.Access)
        {
            EventSystem.current.SetSelectedGameObject(menuSlots[currentMenuHCode]);
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
        scrollTime++;
        if (scrollFrames <= 0)
        {
            scrollFrames = 0f;
        }
        if ((inputReader.useNewInput("MoveDown", pMenuCode) || inputReader.useNewInput("MoveDown_Left", pMenuCode) || inputReader.useNewInput("MoveDown_Right", pMenuCode)) && scrollFrames <= 0)
        {
            if (!(menuItemCodes[currentMenuHCode].SlotType == ShowValueEnum.Access || menuItemCodes[currentMenuHCode].SlotType == ShowValueEnum.Confirm || menuItemCodes[currentMenuHCode].SlotType == ShowValueEnum.None))
            {
                sfxPlayer.PlaySound("Scroll");
            }
            if (menuItemCodes[currentMenuHCode].SlotType == ShowValueEnum.RangeInt)
            {
                if (menuItemCodes[currentMenuHCode].currentMenuHCode == 0)
                {
                    menuItemCodes[currentMenuHCode].currentMenuHCode = 0;
                }
                else
                {
                    if (scrollTime > 60)
                    {
                        if (scrollTime > 120 && menuItemCodes[currentMenuHCode].menuItemMax >= 500)
                        {
                            if (menuItemCodes[currentMenuHCode].currentMenuHCode >= menuItemCodes[currentMenuHCode].menuItemMax - 1)
                            {
                                int fixCount = 0;
                                float numDec = 0;
                                bool multFixed = false;
                                if (menuItemCodes[currentMenuHCode].slotIncrementInt >= 10)
                                {
                                    while ((((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) - numDec) % 1000 != 0) && fixCount < 1000)
                                    {
                                        if ((((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) - numDec) % 100 == 0))
                                        {
                                            numDec += 100f;
                                        }
                                        else if ((((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) - numDec) % 10 == 0))
                                        {
                                            numDec += 10f;
                                        }
                                        else
                                        {
                                            numDec += 1f;
                                        }
                                        if ((((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) - numDec) % 1000 == 0))
                                        {
                                            multFixed = true;
                                        }
                                        fixCount++;
                                    }
                                    if (multFixed)
                                    {
                                        menuItemCodes[currentMenuHCode].currentMenuHCode -= (int)numDec / 10;
                                    }
                                    else
                                    {
                                        menuItemCodes[currentMenuHCode].currentMenuHCode -= 100;
                                    }
                                }
                                else
                                {
                                    while ((((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) - numDec) % 100 != 0) && fixCount < 100)
                                    {
                                        if ((((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) - numDec) % 10 == 0))
                                        {
                                            numDec += 10f;
                                        }
                                        else
                                        {
                                            numDec++;
                                        }
                                        if ((((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) - numDec) % 100 == 0))
                                        {
                                            multFixed = true;
                                        }
                                        fixCount++;
                                    }
                                    if (multFixed)
                                    {
                                        menuItemCodes[currentMenuHCode].currentMenuHCode -= (int)numDec;
                                    }
                                    else
                                    {
                                        menuItemCodes[currentMenuHCode].currentMenuHCode -= 100;
                                    }
                                }
                            }
                            else if ((menuItemCodes[currentMenuHCode].slotIncrementInt >= 10 && ((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) % 1000 == 0)) || (menuItemCodes[currentMenuHCode].slotIncrementInt < 10 && ((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) % 100 == 0)))
                            {
                                menuItemCodes[currentMenuHCode].currentMenuHCode -= 100;
                            }
                            else
                            {
                                menuItemCodes[currentMenuHCode].currentMenuHCode -= 10;
                            }
                        }
                        else
                        {
                            if (menuItemCodes[currentMenuHCode].currentMenuHCode >= menuItemCodes[currentMenuHCode].menuItemMax - 1)
                            {
                                int fixCount = 0;
                                int numDec = 0;
                                bool multFixed = false;
                                if (menuItemCodes[currentMenuHCode].slotIncrementInt >= 10)
                                {
                                    while ((((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) - numDec) % 100 != 0) && fixCount < 100)
                                    {
                                        if ((((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) - numDec) % 10 == 0))
                                        {
                                            numDec += 10;
                                        }
                                        else
                                        {
                                            numDec++;
                                        }
                                        if ((((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) - numDec) % 100 == 0))
                                        {
                                            multFixed = true;
                                        }
                                        fixCount++;
                                    }
                                    if (multFixed)
                                    {
                                        menuItemCodes[currentMenuHCode].currentMenuHCode -= (numDec / 10);
                                    }
                                    else
                                    {
                                        menuItemCodes[currentMenuHCode].currentMenuHCode -= 1;
                                    }
                                }
                                else
                                {
                                    while ((((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) - numDec) % 10 != 0) && fixCount < 20)
                                    {
                                        numDec++;
                                        if ((((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) - numDec) % 10 == 0))
                                        {
                                            multFixed = true;
                                        }
                                        fixCount++;
                                    }
                                    if (multFixed)
                                    {
                                        menuItemCodes[currentMenuHCode].currentMenuHCode -= numDec;
                                    }
                                    else
                                    {
                                        menuItemCodes[currentMenuHCode].currentMenuHCode -= 10;
                                    }
                                }
                            }
                            else if ((menuItemCodes[currentMenuHCode].slotIncrementInt >= 10 && ((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) % 100 == 0)) || (menuItemCodes[currentMenuHCode].slotIncrementInt < 10 && ((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) % 10 == 0)))
                            {
                                menuItemCodes[currentMenuHCode].currentMenuHCode -= 10;
                            }
                            else
                            {
                                menuItemCodes[currentMenuHCode].currentMenuHCode -= 1;
                            }
                        }
                    }
                    else
                    {
                        menuItemCodes[currentMenuHCode].currentMenuHCode -= 1;
                    }
                    if (menuItemCodes[currentMenuHCode].currentMenuHCode <= 0)
                    {
                        menuItemCodes[currentMenuHCode].currentMenuHCode = 0;
                    }
                }
            }
            else
            {
                if (menuItemCodes[currentMenuHCode].currentMenuHCode == 0)
                {
                    menuItemCodes[currentMenuHCode].currentMenuHCode = 0;
                }
                else
                {
                    menuItemCodes[currentMenuHCode].currentMenuHCode -= 1;
                }
                if (menuItemCodes[currentMenuHCode].currentMenuHCode < 0)
                {
                    menuItemCodes[currentMenuHCode].currentMenuHCode = 0;
                }
            }
            scrollFrames = ((holdScroll) ? 5f : 20f);
            holdScroll = true;
        }
        else if ((inputReader.useNewInput("MoveUp", pMenuCode) || inputReader.useNewInput("MoveUp_Left", pMenuCode) || inputReader.useNewInput("MoveUp_Right", pMenuCode)) && scrollFrames <= 0)
        {
            if (!(menuItemCodes[currentMenuHCode].SlotType == ShowValueEnum.Access || menuItemCodes[currentMenuHCode].SlotType == ShowValueEnum.Confirm || menuItemCodes[currentMenuHCode].SlotType == ShowValueEnum.None))
            {
                sfxPlayer.PlaySound("Scroll");
            }
            if (menuItemCodes[currentMenuHCode].SlotType == ShowValueEnum.RangeInt)
            {
                if (menuItemCodes[currentMenuHCode].currentMenuHCode == menuItemCodes[currentMenuHCode].menuItemMax - 1)
                {
                    menuItemCodes[currentMenuHCode].currentMenuHCode = menuItemCodes[currentMenuHCode].menuItemMax - 1;
                }
                else
                {
                    if (scrollTime > 60)
                    {
                        if (scrollTime > 120 && menuItemCodes[currentMenuHCode].menuItemMax >= 500)
                        {
                            if (menuItemCodes[currentMenuHCode].currentMenuHCode <= 0)
                            {
                                int fixCount = 0;
                                float numDec = 0;
                                bool multFixed = false;
                                if (menuItemCodes[currentMenuHCode].slotIncrementInt >= 10)
                                {
                                    while ((((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) + numDec) % 1000 != 0) && fixCount < 1000)
                                    {
                                        if ((((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) + numDec) % 100 == 0))
                                        {
                                            numDec += 100f;
                                        }
                                        else if ((((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) + numDec) % 10 == 0))
                                        {
                                            numDec += 10f;
                                        }
                                        else
                                        {
                                            numDec += 1f;
                                        }
                                        if ((((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) + numDec) % 1000 == 0))
                                        {
                                            multFixed = true;
                                        }
                                        fixCount++;
                                    }
                                    if (multFixed)
                                    {
                                        menuItemCodes[currentMenuHCode].currentMenuHCode += (int)numDec / 10;
                                    }
                                    else
                                    {
                                        menuItemCodes[currentMenuHCode].currentMenuHCode += 100;
                                    }
                                }
                                else
                                {
                                    while ((((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) + numDec) % 100 != 0) && fixCount < 100)
                                    {
                                        if ((((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) + numDec) % 10 == 0))
                                        {
                                            numDec += 10f;
                                        }
                                        else
                                        {
                                            numDec++;
                                        }
                                        if ((((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) + numDec) % 100 == 0))
                                        {
                                            multFixed = true;
                                        }
                                        fixCount++;
                                    }
                                    if (multFixed)
                                    {
                                        menuItemCodes[currentMenuHCode].currentMenuHCode += (int)numDec;
                                    }
                                    else
                                    {
                                        menuItemCodes[currentMenuHCode].currentMenuHCode += 100;
                                    }
                                }
                            }
                            else if ((menuItemCodes[currentMenuHCode].slotIncrementInt >= 10 && ((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) % 1000 == 0)) || (menuItemCodes[currentMenuHCode].slotIncrementInt < 10 && ((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) % 100 == 0)))
                            {
                                menuItemCodes[currentMenuHCode].currentMenuHCode += 100;
                            }
                            else
                            {
                                menuItemCodes[currentMenuHCode].currentMenuHCode += 10;
                            }
                        }
                        else
                        {
                            if (menuItemCodes[currentMenuHCode].currentMenuHCode <= 0)
                            {
                                int fixCount = 0;
                                int numDec = 0;
                                bool multFixed = false;
                                if (menuItemCodes[currentMenuHCode].slotIncrementInt >= 10)
                                {
                                    while ((((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) + numDec) % 100 != 0) && fixCount < 100)
                                    {
                                        if ((((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) + numDec) % 10 == 0))
                                        {
                                            numDec += 10;
                                        }
                                        else
                                        {
                                            numDec++;
                                        }
                                        if ((((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) + numDec) % 100 == 0))
                                        {
                                            multFixed = true;
                                        }
                                        fixCount++;
                                    }
                                    if (multFixed)
                                    {
                                        menuItemCodes[currentMenuHCode].currentMenuHCode += (numDec / 10);
                                    }
                                    else
                                    {
                                        menuItemCodes[currentMenuHCode].currentMenuHCode += 1;
                                    }
                                }
                                else
                                {
                                    while ((((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) + numDec) % 10 != 0) && fixCount < 20)
                                    {
                                        numDec++;
                                        if ((((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) + numDec) % 10 == 0))
                                        {
                                            multFixed = true;
                                        }
                                        fixCount++;
                                    }
                                    if (multFixed)
                                    {
                                        menuItemCodes[currentMenuHCode].currentMenuHCode += numDec;
                                    }
                                    else
                                    {
                                        menuItemCodes[currentMenuHCode].currentMenuHCode += 10;
                                    }
                                }
                            }
                            else if ((menuItemCodes[currentMenuHCode].slotIncrementInt >= 10 && ((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) % 100 == 0)) || (menuItemCodes[currentMenuHCode].slotIncrementInt < 10 && ((menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt)) % 10 == 0)))
                            {
                                menuItemCodes[currentMenuHCode].currentMenuHCode += 10;
                            }
                            else
                            {
                                menuItemCodes[currentMenuHCode].currentMenuHCode += 1;
                            }
                        }
                    }
                    else
                    {
                        menuItemCodes[currentMenuHCode].currentMenuHCode += 1;
                    }
                    if (menuItemCodes[currentMenuHCode].currentMenuHCode >= menuItemCodes[currentMenuHCode].menuItemMax)
                    {
                        menuItemCodes[currentMenuHCode].currentMenuHCode = menuItemCodes[currentMenuHCode].menuItemMax - 1;
                    }
                }
            }
            else
            {
                if (menuItemCodes[currentMenuHCode].currentMenuHCode == menuItemCodes[currentMenuHCode].menuItemMax - 1)
                {
                    menuItemCodes[currentMenuHCode].currentMenuHCode = menuItemCodes[currentMenuHCode].menuItemMax - 1;
                }
                else
                {
                    menuItemCodes[currentMenuHCode].currentMenuHCode += 1;
                }
                if (menuItemCodes[currentMenuHCode].currentMenuHCode >= menuItemCodes[currentMenuHCode].menuItemMax)
                {
                    menuItemCodes[currentMenuHCode].currentMenuHCode = menuItemCodes[currentMenuHCode].menuItemMax - 1;
                }
            }
            scrollFrames = ((holdScroll) ? 5f : 20f);
            holdScroll = true;
        }
        else if (inputReader.useNewInput("MoveRight", pMenuCode) && scrollFrames <= 0 && (!inputReader.useNewInput("MoveUp", pMenuCode) && !inputReader.useNewInput("MoveDown", pMenuCode)) && scrollUpWait <= 0)
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
        else if (inputReader.useNewInput("MoveLeft", pMenuCode) && scrollFrames <= 0 && (!inputReader.useNewInput("MoveUp", pMenuCode) && !inputReader.useNewInput("MoveDown", pMenuCode)) && scrollDownWait <= 0)
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
        /*if (setting == 0)
        {
            gameData.modeStyle = menuItemCodes[currentMenuHCode].currentMenuHCode;
        }
        else if (setting == 1)
        {
            gameData.modeReflection = menuItemCodes[currentMenuHCode].currentMenuHCode;
        }
        else if (setting == 2)
        {
            gameData.defaultTotalShards = (menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt));
        }
        else if (setting == 3)
        {
            gameData.defaultStockCount = (menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt));
        }
        else if (setting == 4)
        {
            gameData.timerSetting = (menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt));
        }
        else if (setting == 7)
        {
            gameData.defaultInitialHealth = (menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt));
            if (gameData.defaultInitialHealth > 9999) { gameData.defaultInitialHealth = 9999; }
        }
        else if (setting == 8)
        {
            gameData.defaultInitialShards = (menuItemCodes[currentMenuHCode].slotValueInt + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementInt));
        }
        else if (setting == 9)
        {
            gameData.damageRatio = (float)(menuItemCodes[currentMenuHCode].slotValueFloat + (menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementFloat));
        }
        else if (setting == 10)
        {
            gameData.barrierRatio = (float)(menuItemCodes[currentMenuHCode].slotValueFloat + ((float)menuItemCodes[currentMenuHCode].currentMenuHCode * menuItemCodes[currentMenuHCode].slotIncrementFloat));
        }*/
    }

    public void updateMenu(string menuType, int mode)
    {
        /*switch (menuType)
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
        }*/
    }

    public void fixNumberName(int vCode)
    {
        /*if (menuItemCodes[vCode].specialNumberName.Length > 0)
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
        }*/
    }
}
