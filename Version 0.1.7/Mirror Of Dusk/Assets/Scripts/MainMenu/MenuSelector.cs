using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;

public class MenuSelector : MonoBehaviour
{

    public int numSelectors;
    public TextMeshProUGUI mainTextPrefab;
    public GameObject mainLinePrefab;
    public TextMeshProUGUI mainDetailTextPrefab;
    public TextMeshProUGUI subDetailTextPrefab;
    public TextMeshProUGUI subTextPrefab;
    public GameObject subLinePrefab;
    public GameObject bigShardPrefab;
    GameObject mainMenuLine;
    TextMeshProUGUI mainMenuListText;
    TextMeshProUGUI mainDetailText;
    TextMeshProUGUI subDetailText;
    public GameObject[] subMenuListLine;
    TextMeshProUGUI[] subMenuListText;
    GameObject bigShardClone;
    public int currentSelector = 0;
    public List<string> mainMenuList;
    public Dictionary<string, string[]> mainMenuDict;
    public Dictionary<int, string> mainMenuDictKey;
    public Dictionary<string, Color32> mainMenuDictColors;
    public Dictionary<string, string> mainMenuDictMainDetails;
    public Dictionary<string, string> mainMenuDictSubDetails;
    public string currentMainMenuMode;
    public List<string> prevMainMenuMode;
    public string prevLeavingMainMenuMode;
    Vector3 setPos;
    private bool returnHand = false;
    private bool clearMDew = false;
    private bool clearMWhite = false;
    private bool restoreMDew = false;
    private bool restoreMWhite = false;
    public Stack<int> storedSelector;

    GameObject bigShardCloneB;
    RawImage tScreenRender;
    SpriteRenderer mScreenRender;
    private TextColorAlter textColorAlter;
    private BumpSubChoice bumpSubChoice;

    public enum mainMenuWords
    {
        ModeSelect,
        SoloMode,
        MultiplayerMode,
        OnlineMode,
        DataMode,
        Options
    };

    // Use this for initialization
    void Start()
    {
        textColorAlter = gameObject.GetComponent<TextColorAlter>();
        bumpSubChoice = gameObject.GetComponent<BumpSubChoice>();
        mainMenuDictKey = new Dictionary<int, string>();
        storedSelector = new Stack<int>();
        BuildMenuChoices();
        InstChoices();
        textColorAlter.StartCoroutine("textAlterColor");
        tScreenRender = (RawImage)GameObject.Find("BaseTint").GetComponent<RawImage>();
        mScreenRender = GameObject.Find("TintScreenSprite").GetComponent<SpriteRenderer>();
    }

    private void BuildMenuChoices()
    {
        mainMenuList = new List<string>();
        mainMenuDict = new Dictionary<string, string[]>();
        mainMenuList.Add("MODE SELECT_1");
        mainMenuDict.Add("MODE SELECT_1", new string[] { "SOLO_1", "MULTIPLAYER_1", "ONLINE_1", "DATA_1", "OPTIONS_1" });
        mainMenuList.Add("SOLO_1");
        mainMenuDict.Add("SOLO_1", new string[] { "ARCADE_1", "STORY_1", "TRAINING_1", "MISSION_1", "SETTINGS_1" });
        mainMenuList.Add("MULTIPLAYER_1");
        mainMenuDict.Add("MULTIPLAYER_1", new string[] { "VERSUS_1", "TOURNAMENT_1", "SETTINGS_2" });
        mainMenuList.Add("ONLINE_1");
        mainMenuDict.Add("ONLINE_1", new string[] { "RANKED_1", "CASUAL_1", "LOBBY_1", "RESULTS_1", "SETTINGS_3" });
        mainMenuList.Add("DATA_1");
        mainMenuDict.Add("DATA_1", new string[] { "RANKINGS_1", "CHARACTER USAGE_1", "PROFILE_1", "GALLERY_1", "SOUND TEST_1" });
        mainMenuList.Add("TRAINING_1");
        mainMenuDict.Add("TRAINING_1", new string[] { "ENTER_1" });
        //mainMenuList.Add("OPTIONS_1");
        //mainMenuDict.Add("OPTIONS_1", new string[] { "RANKINGS_2", "CHARACTER USAGE_2", "PROFILE_2", "GALLERY_2", "SOUND TEST_2" });
        addMenuColors();
        addMenuDetails();
        currentMainMenuMode = "MODE SELECT_1";
        prevMainMenuMode = new List<string>();
        numSelectors = mainMenuDict[currentMainMenuMode].Count();
        mainMenuDictKey.Clear();
        int dCount = 0;
        foreach (string dKey in mainMenuDict[currentMainMenuMode])
        {
            mainMenuDictKey.Add(dCount, dKey);
            dCount++;
        }
        subMenuListLine = new GameObject[numSelectors];
        subMenuListText = new TextMeshProUGUI[numSelectors];
        currentSelector = 0;
    }

    public void BuildMenuChoices(string sMode)
    {
        numSelectors = mainMenuDict[currentMainMenuMode].Count();
        mainMenuDictKey.Clear();
        int dCount = 0;
        foreach (string dKey in mainMenuDict[currentMainMenuMode])
        {
            mainMenuDictKey.Add(dCount, dKey);
            dCount++;
        }
        subMenuListLine = new GameObject[numSelectors];
        subMenuListText = new TextMeshProUGUI[numSelectors];
        if (returnHand)
        {
            currentSelector = storedSelector.Pop();
        }
        else
        {
            currentSelector = 0;
        }
        InstChoices();
        textColorAlter.StopCoroutine("textAlterColor");
        textColorAlter.StartCoroutine("textAlterColor");
    }

    public void BuildHand()
    {
        if (returnHand)
        {
            bigShardClone = Instantiate(bigShardPrefab, new Vector3(-901f, -261f, 11), Quaternion.identity);
            bigShardClone.transform.localScale = new Vector3(9f, 9f, 1f);
            MoveMenuItem moveBigShard = bigShardClone.GetComponent<MoveMenuItem>();
            ScaleMenuItem scaleBigShard = bigShardClone.GetComponent<ScaleMenuItem>();
            moveBigShard.changeStartingSpeed(80f, 20f, 1280f, 320f);
            moveBigShard.GetComponent<MoveMenuItem>().changeDelay(0f);
            scaleBigShard.GetComponent<ScaleMenuItem>().changeStartingScale(-0.5f, -0.5f, -8f, -8f);
            scaleBigShard.GetComponent<ScaleMenuItem>().changeDelay(0f);
            bigShardCloneB = bigShardClone.transform.Find("BigShard-Inner").gameObject;
            bigShardCloneB.GetComponent<ClearMirrorDew>().restoreMDew();
            GameObject bigShardCloneC = bigShardClone.transform.Find("BigShard-White").gameObject;
            bigShardCloneC.GetComponent<ClearMirrorDew>().restoreMDew();
            restoreMDewNow();

            /*tScreen = GameObject.Find("BaseTint");
            tScreenRender = (RawImage)GameObject.Find("BaseTint").GetComponent<RawImage>();
            mScreen = GameObject.Find("TintScreenSprite");
            mScreenRender = mScreen.GetComponent<SpriteRenderer>();*/
            string currentMenuChoice = currentMainMenuMode;
            bool foundTOptionRender = false;
            foreach (KeyValuePair<string, Color32> list in mainMenuDictColors)
            {
                if (list.Key == currentMenuChoice)
                {
                    foundTOptionRender = true;
                }
            }
            if (!foundTOptionRender) { currentMenuChoice = "DEFAULT"; }
            tScreenRender.color = mainMenuDictColors[currentMenuChoice];
            tScreenRender.color = new Color(tScreenRender.color.r, tScreenRender.color.g, tScreenRender.color.b, 0);
            currentMenuChoice = prevLeavingMainMenuMode;
            bool foundOptionRender = false;
            foreach (KeyValuePair<string, Color32> list in mainMenuDictColors)
            {
                if (list.Key == currentMenuChoice)
                {
                    foundOptionRender = true;
                }
            }
            if (!foundOptionRender) { currentMenuChoice = "DEFAULT"; }
            mScreenRender.color = mainMenuDictColors[currentMenuChoice];
        }
    }

    private void InstChoices()
    {
        mainMenuListText = Instantiate(mainTextPrefab, new Vector3(-1327f, 450f, -35), Quaternion.identity);
        mainMenuListText.transform.SetParent(GameObject.Find("Text Canvas").transform, false);
        mainMenuListText.name = "MainChoiceText";
        mainMenuListText.text = cleanText(currentMainMenuMode);
        if (currentMainMenuMode == "SOLO_1") { mainMenuListText.text += " MODE"; }
        mainMenuLine = Instantiate(mainLinePrefab, new Vector3(-1314f, 344f, -40), Quaternion.identity);
        mainMenuLine.name = "MainForm-WithGlow";
        mainDetailText = Instantiate(mainDetailTextPrefab, new Vector3(-1357f, 340f, -35), Quaternion.identity);
        mainDetailText.transform.SetParent(GameObject.Find("Text Canvas").transform, false);
        mainDetailText.name = "MainMenuMainDetail";
        mainDetailText.text = mainMenuDictMainDetails[mainMenuDict[currentMainMenuMode][currentSelector]];
        subDetailText = Instantiate(subDetailTextPrefab, new Vector3(-1357f, 340f, -35), Quaternion.identity);
        subDetailText.transform.SetParent(GameObject.Find("Text Canvas").transform, false);
        subDetailText.name = "MainMenuSubDetail";
        subDetailText.text = mainMenuDictSubDetails[mainMenuDict[currentMainMenuMode][currentSelector]];
        if (returnHand)
        {
            returnHand = false;
        }
        else
        {
            bigShardClone = Instantiate(bigShardPrefab, new Vector3(1879f, -1291f, 11), Quaternion.identity);
        }
        float xPlace = -1270f;
        float yPlace = 150f;
        float zPlace = -40f;
        float delayPlace = 3f;
        setPos = new Vector3(xPlace, yPlace, zPlace);
        for (int i = 0; i < numSelectors; i++)
        {
            subMenuListLine[i] = Instantiate(subLinePrefab, new Vector3(xPlace - 140f, yPlace - 27f, zPlace), Quaternion.identity);
            subMenuListLine[i].name = "SubForm-WithGlow" + i;
            subMenuListLine[i].GetComponent<MoveMenuItem>().startDelay = delayPlace;
            subMenuListText[i] = Instantiate(subTextPrefab, setPos, Quaternion.identity);
            subMenuListText[i].transform.SetParent(GameObject.Find("Text Canvas").transform, false);
            subMenuListText[i].name = "SubChoiceText" + i;
            subMenuListText[i].text = cleanText(mainMenuDict[currentMainMenuMode][i]);
            subMenuListText[i].GetComponent<MoveMenuItem>().startDelay = delayPlace;
            yPlace -= 90f;
            setPos = new Vector3(xPlace, yPlace, zPlace);
            delayPlace += 3f;
        }
        textColorAlter.refreshMenu();
        bumpSubChoice.refreshMenu();
    }

    private void addMenuColors()
    {
        mainMenuDictColors = new Dictionary<string, Color32>();
        mainMenuDictColors.Add("SOLO_1", new Color32(146, 149, 67, 76));
        mainMenuDictColors.Add("MULTIPLAYER_1", new Color32(140, 26, 27, 76));
        mainMenuDictColors.Add("ONLINE_1", new Color32(25, 32, 135, 76));
        mainMenuDictColors.Add("DATA_1", new Color32(29, 106, 92, 76));
        mainMenuDictColors.Add("DEFAULT", new Color32(35, 96, 31, 76));
    }

    private void addMenuDetails()
    {
        mainMenuDictMainDetails = new Dictionary<string, string>
        {
            { "SOLO_1" , "SOLO MODE" } ,
            { "MULTIPLAYER_1" , "MULTIPLAYER MODE" } ,
            { "ONLINE_1" , "ONLINE MODE" } ,
            { "DATA_1" , "DATA" } ,
            { "OPTIONS_1" , "OPTIONS" } ,
            { "ARCADE_1" , "ARCADE MODE" } ,
            { "STORY_1" , "STORY MODE" } ,
            { "TRAINING_1" , "TRAINING" } ,
            { "MISSION_1" , "MISSION" } ,
            { "SETTINGS_1" , "SETTINGS" } ,
            { "SETTINGS_2" , "SETTINGS" } ,
            { "SETTINGS_3" , "SETTINGS" } ,
            { "VERSUS_1" , "VERSUS MODE" } ,
            { "TOURNAMENT_1" , "TOURNAMENT MODE" } ,
            { "RANKED_1" , "RANKED MATCHES" } ,
            { "CASUAL_1" , "CASUAL MATCHES" } ,
            { "LOBBY_1" , "LOBBY" } ,
            { "RESULTS_1" , "RESULTS" } ,
            { "RANKINGS_1" , "RANKINGS" } ,
            { "CHARACTER USAGE_1" , "CHARACTER USAGE" } ,
            { "PROFILE_1" , "PROFILE" } ,
            { "GALLERY_1" , "GALLERY" } ,
            { "SOUND TEST_1" , "SOUND TEST" } ,
            { "DEFAULT" , "???" }
        };

        mainMenuDictSubDetails = new Dictionary<string, string>
        {
            { "SOLO_1" , "Attend several types of single player activities to take a challenge or build up experience." } ,
            { "MULTIPLAYER_1" , "Have guests? Spar or join them in the dark mayhem through battles of many kinds." } ,
            { "ONLINE_1" , "Face your peers and threats from far away in the worldwide calamity." } ,
            { "DATA_1" , "A library where all information of the unfortunate can be accessed." } ,
            { "OPTIONS_1" , "Overall settings of the game and its features." } ,
            { "ARCADE_1" , "Seek after independent foes one-by-one to grow a fearsome record." } ,
            { "STORY_1" , "Play each of the twisted seekers in their personal chronicle." } ,
            { "TRAINING_1" , "For learning the characters' powers and everything the game can offer." } ,
            { "MISSION_1" , "Improve your seeker's skills by partaking unique challenges." } ,
            { "SETTINGS_1" , "Modify the settings of the current mode." } ,
            { "SETTINGS_2" , "Modify the settings of the current mode." } ,
            { "SETTINGS_3" , "Modify the settings of the current mode." } ,
            { "VERSUS_1" , "Fight off players or CPUs in battles of your choice." } ,
            { "TOURNAMENT_1" , "Create a local competition to decide whose future prevails." } ,
            { "RANKED_1" , "Face competitive opponents in a serious feud where your overall skills are compared." } ,
            { "CASUAL_1" , "Face any type of opponent regardless of rank. Filtering of opponents can apply." } ,
            { "LOBBY_1" , "Create or find rooms where players can gather in their own skirmish." } ,
            { "RESULTS_1" , "Look up the rankings and statistics from your and others' battles." } ,
            { "RANKINGS_1" , "Read results of characters' efforts by a ranked order." } ,
            { "CHARACTER USAGE_1" , "Find out who has been journeying the most in any mode." } ,
            { "PROFILE_1" , "Learn the backstory of each playable character." } ,
            { "GALLERY_1" , "A collection of artworks to share." } ,
            { "SOUND TEST_1" , "Toy around with sounds and voices stored within the game." } ,
            { "DEFAULT" , "???" }
        };
    }

    public void returnTheHand()
    {
        returnHand = true;
    }

    private string cleanText(string aText)
    {
        int codelin = aText.IndexOf('_');
        //codelin != null && 
        if (codelin != 0)
        {
            aText = aText.Substring(0, codelin);
        }
        return aText;
    }

    public void clearMDewNow()
    {
        clearMDew = true;
        clearMWhite = true;
    }

    public void clearMDewOff()
    {
        clearMDew = false;
    }
    public void clearMWhiteOff()
    {
        clearMWhite = false;
    }

    public bool clearMDewCheck()
    {
        if (clearMDew || clearMWhite)
        {
            return true;
        }
        return false;
    }

    public void restoreMDewNow()
    {
        restoreMDew = true;
        restoreMWhite = true;
    }

    public bool restoreMDewCheck()
    {
        if (restoreMDew || restoreMWhite)
        {
            return true;
        }
        return false;
    }
}
