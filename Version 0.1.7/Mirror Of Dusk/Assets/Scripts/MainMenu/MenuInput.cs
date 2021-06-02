using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MenuInput : PlayerInput
{
    
    MenuSelector menuSelector;
    TextColorAlter textColorAlter;
    DelayInputMainMenu delayInput;
    FlashTheGleam flashTheGleam;
    BumpSubChoice bumpSubChoice;
    ToChosenScene toChosenScene;
    public bool enableMenuInput = false;
    private GameObject[] menuObjects;
    private MoveMenuItem[] menuObjectsComp;
    private ScaleMenuItem[] menuObjectsScale;
    float padAim;
    float padRead = 0.0f;
    bool padReadDown = false;

    // Use this for initialization
    void Start()
    {
        InitInput();
        playerCode = "1";
        menuSelector = GameObject.Find("MenuSelector").GetComponent<MenuSelector>();
        textColorAlter = GameObject.Find("MenuSelector").GetComponent<TextColorAlter>();
        delayInput = GameObject.Find("MenuSelector").GetComponent<DelayInputMainMenu>();
        flashTheGleam = gameObject.GetComponent<FlashTheGleam>();
        bumpSubChoice = gameObject.GetComponent<BumpSubChoice>();
        toChosenScene = GameObject.Find("MenuSelector").GetComponent<ToChosenScene>();
        scrollFrames = 0.0f;
        padRead = 0.0f;
    }

    void Update()
    {
        searchForKeyInputs();

        if (enableMenuInput)
        {
            directionalController();
            /*padAim = Input.GetAxisRaw("Vertical_1");
            if (padRead < 0.02f || padRead > -0.02f)
            {
                padRead = padAim;
                if (padRead > 0)
                {
                    padReadDown = false;
                }
                else if (padRead < 0)
                {
                    padReadDown = true;
                }
            }
            else
            {
                if (!padReadDown && (padAim < padRead))
                {
                    padRead = 0;
                }
                if (padReadDown && (padAim > padRead))
                {
                    padRead = 0;
                }
            }
            scrollFrames--;
            if (scrollFrames <= 0)
            {
                scrollFrames = 0f;
            }
            if (padAim < -0.05 && scrollFrames <= 0)
            {
                menuSelector.currentSelector = (menuSelector.currentSelector + 1);
                menuSelector.currentSelector = ((menuSelector.currentSelector >= menuSelector.numSelectors) ? 0 : menuSelector.currentSelector);
                textColorAlter.upCount = 0;
                scrollFrames = ((holdScroll) ? 10f : 30f);
                holdScroll = true;
            }
            else if (padAim > 0.05 && scrollFrames <= 0)
            {
                menuSelector.currentSelector = (menuSelector.currentSelector - 1);
                menuSelector.currentSelector = ((menuSelector.currentSelector < 0) ? menuSelector.numSelectors - 1 : menuSelector.currentSelector);
                textColorAlter.upCount = 0;
                scrollFrames = ((holdScroll) ? 10f : 30f);
                holdScroll = true;
            }
            else
            {
                if (padAim >= -0.05 && padAim <= 0.05 || padRead == 0)
                {
                    scrollFrames = 0f;
                    holdScroll = false;
                }
            }*/
            if (inputReader.useNewInput("Confirm", playerCode) && enableMenuInput)
            {
                enableMenuInput = false;
                StartCoroutine("SelectThisOption");
            }
            if (inputReader.useNewInput("Cancel", playerCode) && enableMenuInput)
            {
                if (menuSelector.prevMainMenuMode.Count() > 0)
                {
                    sfxPlayer.PlaySound("Cancel");
                    menuSelector.prevLeavingMainMenuMode = menuSelector.currentMainMenuMode;
                    menuSelector.currentMainMenuMode = menuSelector.prevMainMenuMode[menuSelector.prevMainMenuMode.Count() - 1];
                    menuSelector.prevMainMenuMode.Remove(menuSelector.prevMainMenuMode[menuSelector.prevMainMenuMode.Count() - 1]);
                    //if (menuSelector.currentMainMenuMode == "SOLO") { menuSelector.currentMainMenuMode += " MODE"; }
                    menuSelector.returnTheHand();
                    menuObjects = GameObject.FindGameObjectsWithTag("MainMenuItem");
                    menuObjectsComp = new MoveMenuItem[menuObjects.Length];
                    menuObjectsScale = new ScaleMenuItem[menuObjects.Length];
                    for (int i = 0; i < menuObjects.Length; i++)
                    {
                        menuObjectsComp[i] = (MoveMenuItem)menuObjects[i].GetComponent<MoveMenuItem>();
                        menuObjectsScale[i] = (ScaleMenuItem)menuObjects[i].GetComponent<ScaleMenuItem>();
                        if (menuObjectsComp[i] != null)
                        {
                            if (menuObjectsComp[i].name == "BigShard(Clone)")
                            {
                                menuObjectsComp[i].changeEndingSpeed(35f, -30f, 1050f, -900f);
                                menuObjectsComp[i].changeDelay(0f);
                            }
                            menuObjectsComp[i].removeChoiceItems();
                        }
                        if (menuObjectsScale[i] != null)
                        {
                            if (menuObjectsScale[i].name == "BigShard(Clone)")
                            {
                                menuObjectsScale[i].changeEndingScale(0f, 0f, 0f, 0f);
                                menuObjectsScale[i].changeDelay(0f);
                            }
                            menuObjectsScale[i].removeChoiceItems();
                        }
                    }
                    delayInput.delayTime = 90.0f;
                    delayInput.startMovingTrees(false);
                    delayInput.refreshTheMenu();
                    delayInput.buildTheHand();
                    enableMenuInput = false;
                }
                else
                {
                    sfxPlayer.PlaySound("Cancel");
                    delayInput.delayTime = 100.0f;
                    delayInput.nowLeaving();
                    toChosenScene.selectScene("TitleScene");
                    toChosenScene.enableFadeScene = true;
                    enableMenuInput = false;
                }
            }
        }
    }

    public void directionalController()
    {
        //padHorizontal = Input.GetAxisRaw("Horizontal_" + playerCode);
        //padVertical = Input.GetAxisRaw("Vertical_" + playerCode);
        padAim = Input.GetAxisRaw("Vertical_1");
        if (padRead < 0.02f || padRead > -0.02f)
        {
            padRead = padAim;
            if (padRead > 0)
            {
                padReadDown = false;
            }
            else if (padRead < 0)
            {
                padReadDown = true;
            }
        }
        else
        {
            if (!padReadDown && (padAim < padRead))
            {
                padRead = 0;
            }
            if (padReadDown && (padAim > padRead))
            {
                padRead = 0;
            }
        }
        scrollFrames--;
        if (scrollFrames <= 0)
        {
            scrollFrames = 0f;
        }
        if ((inputReader.useNewInput("MoveDown", playerCode) || inputReader.useNewInput("MoveDown_Left", playerCode) || inputReader.useNewInput("MoveDown_Right", playerCode)) && scrollFrames <= 0)
        {
            menuSelector.currentSelector = (menuSelector.currentSelector + 1);
            menuSelector.currentSelector = ((menuSelector.currentSelector >= menuSelector.numSelectors) ? 0 : menuSelector.currentSelector);
            textColorAlter.StopCoroutine("textAlterColor");
            textColorAlter.StartCoroutine("textAlterColor");
            scrollFrames = ((holdScroll) ? 10f : 30f);
            holdScroll = true;
        }
        else if ((inputReader.useNewInput("MoveUp", playerCode) || inputReader.useNewInput("MoveUp_Left", playerCode) || inputReader.useNewInput("MoveUp_Right", playerCode)) && scrollFrames <= 0)
        {
            menuSelector.currentSelector = (menuSelector.currentSelector - 1);
            menuSelector.currentSelector = ((menuSelector.currentSelector < 0) ? menuSelector.numSelectors - 1 : menuSelector.currentSelector);
            textColorAlter.StopCoroutine("textAlterColor");
            textColorAlter.StartCoroutine("textAlterColor");
            scrollFrames = ((holdScroll) ? 10f : 30f);
            holdScroll = true;
        }
        else
        {
            if (inputReader.checkReleased(playerCode))
            {
                scrollFrames = 0f;
                holdScroll = false;
            }
        }
    }

    private IEnumerator SelectThisOption()
    {
        while (flashTheGleam.flashGleaming)
        {
            yield return null;
        }
        foreach (string list in menuSelector.mainMenuList)
        {
            if (list == menuSelector.mainMenuDictKey[menuSelector.currentSelector])
            {
                sfxPlayer.PlaySound("Confirm");
                string cMainMenuMode = menuSelector.currentMainMenuMode;
                menuSelector.storedSelector.Push(menuSelector.currentSelector);
                menuSelector.prevMainMenuMode.Add(cMainMenuMode);
                menuSelector.currentMainMenuMode = menuSelector.mainMenuDictKey[menuSelector.currentSelector];
                bool leaveMenu = false;
                foreach (string chce in menuSelector.mainMenuDict[menuSelector.currentMainMenuMode])
                {
                    if (chce == "ENTER_1")
                    {
                        leaveMenu = true;
                        textColorAlter.enabled = false;
                        bumpSubChoice.enabled = false;
                        menuSelector.currentSelector = 0;
                    }
                }
                menuObjects = GameObject.FindGameObjectsWithTag("MainMenuItem");
                menuObjectsComp = new MoveMenuItem[menuObjects.Length];
                menuObjectsScale = new ScaleMenuItem[menuObjects.Length];
                for (int i = 0; i < menuObjects.Length; i++)
                {
                    menuObjectsComp[i] = (MoveMenuItem)menuObjects[i].GetComponent<MoveMenuItem>();
                    menuObjectsScale[i] = (ScaleMenuItem)menuObjects[i].GetComponent<ScaleMenuItem>();
                    if (!System.Object.ReferenceEquals(menuObjectsComp[i], null))
                    {
                        if (menuObjectsComp[i].name == "BigShard(Clone)")
                        {
                            menuObjectsComp[i].changeDelay(25f);
                        }
                        menuObjectsComp[i].removeChoiceItems();
                    }
                    if (!System.Object.ReferenceEquals(menuObjectsScale[i], null))
                    {
                        if (menuObjectsScale[i].name == "BigShard(Clone)")
                        {
                            menuObjectsScale[i].changeDelay(25f);
                        }
                        menuObjectsScale[i].removeChoiceItems();
                    }
                }
                flashTheGleam.StartCoroutine("FlashGleam", false);
                menuSelector.clearMDewNow();
                delayInput.delayTime = 100.0f;
                delayInput.startMovingTrees(true);
                if (leaveMenu)
                {
                    delayInput.nowLeaving();
                    toChosenScene.selectScene("CharacterSelectScene");
                    toChosenScene.enableFadeScene = true;
                }
                else
                {
                    delayInput.refreshTheMenu();
                }
                enableMenuInput = false;
            }
        }
        yield return null;
    }
}
