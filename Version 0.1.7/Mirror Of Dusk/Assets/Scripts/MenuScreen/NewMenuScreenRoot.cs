using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEditor;
using TMPro;
using Rewired;

[System.Serializable]
public class NewMenuDataCollect : UnityEvent<string, int>
{

}

public class NewMenuScreenRoot : MonoBehaviour {

    public enum ShowValueEnum
    {
        Access,
        Confirm,
        Single,
        RangeInt,
        RangeFloat,
        None
    }

    protected string pMenuCode;

    [SerializeField] public string appearMode = "Move";
    [SerializeField] public float endMoveY;
    [SerializeField] public float destroyEndMoveY;
    protected int controlCode = 0;
    [System.Serializable]
    public class ScreenHighlighterSize
    {
        public float width = 50.0f;
        public float height = 50.0f;
    }
    [System.Serializable]
    public class MenuDetailSection
    {
        public float pos = 100f;
        public string currentDetails;
    }
    protected string menuName;
    public bool useScreenHighlighter = true;
    public ScreenHighlighterSize screenHighlighterSize;
    public Dictionary<int, string> characterName;
    public GameObject screenHighlighterPrefab;
    protected List<GameObject> screenHighlighter;
    public GameObject _boxHighlight;

    protected CSPlayerInput csPlayerInput;
    protected CSPlayerInput currentPlayerInput;
    protected Player s_player;
    protected SystemInputReader systemInputReader;
    protected InputReader inputReader;
    protected CSPlayerData csPlayerData;
    protected FighterData fighterData;
    protected GameData gameData;
    protected SFXPlayer sfxPlayer;
    public MenuDetailsRoot menuDetailsRoot;
    protected NewMenuScreenRoot rootMenu;

    //Directional Input Controls
    protected float padHorizontal;
    protected float padVertical;
    protected float prevPadHorizontal = 0.0f;
    protected float prevPadVertical = 0.0f;
    protected float padHMoveLeft = 0.0f;
    protected float padHMoveRight = 0.0f;
    protected float padVMoveUp = 0.0f;
    protected float padVMoveDown = 0.0f;
    protected int padHTime = 0;
    protected int padHTimeLeft = 0;
    protected int padHTimeRight = 0;
    protected int padVTime = 0;
    protected int padVTimeUp = 0;
    protected int padVTimeDown = 0;
    protected bool padReadDown = false;
    protected bool padReadRight = false;
    protected float scrollFrames = 0.0f;
    protected int scrollTime = 0;
    protected bool holdScroll = false;


    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void OnEnable()
    {

    }

    public virtual void OnDisable()
    {

    }

    public virtual void assignPlayer(int playerCode)
    {
        //pMenuCode = playerCode;
    }

    public virtual void assignCurrentPlayer(CSPlayerInput c_player)
    {
        currentPlayerInput = c_player;
    }

    protected virtual void setUpMenu()
    {
        menuName = gameObject.name.Substring(0, gameObject.name.IndexOf("Screen"));
        sfxPlayer = GameObject.Find("SFX Player").GetComponent<SFXPlayer>();
        gameData = GameObject.Find("Game Data").GetComponent<GameData>();
        pMenuCode = gameObject.name.Substring(gameObject.name.Length - 1, 1);
        try
        {
            menuDetailsRoot = gameObject.GetComponentInChildren<MenuDetailsRoot>();
        } catch (System.NullReferenceException)
        {

        }
        inputReader = GameObject.Find("EventSystem").GetComponent<InputReader>();
        initializeCharacterCodes();
        csPlayerData = GameObject.Find("Player " + pMenuCode).GetComponent<CSPlayerData>();
    }

    protected void initializeCharacterCodes()
    {
        characterName = new Dictionary<int, string> {
            { 0, "Wymond" },
            { 1, "Tilly" },
            { 2, "Celinda" },
            { 3, "Leatrice" },
            { 4, "Pompion" },
            { 5, "Mugo" },
            { 6, "Faithe" },
            { 7, "Sigmund" },
            { 8, "Deimos" },
            { 9, "Akita" },
            { 10, "Aldric" },
            { 11, "Random" },
            { -1, "Random" }
        };
    }

    public void assignRootMenu(NewMenuScreenRoot rootMen)
    {
        rootMenu = rootMen;
    }

    public void updateControlCode(int cc)
    {
        controlCode = cc;
    }

    public IEnumerator summonMenu(float eMY)
    {
        if (appearMode == "Fade")
        {
            float curAlpha = 0;
            SpriteRenderer sprSH = screenHighlighter[0].GetComponent<SpriteRenderer>();
            SpriteRenderer sprSHC = screenHighlighter[0].GetComponentInChildren<SpriteRenderer>();
            sprSH.color = new Color(sprSH.color.r, sprSH.color.g, sprSH.color.b, curAlpha);
            sprSHC.color = new Color(sprSHC.color.r, sprSHC.color.g, sprSHC.color.b, curAlpha);
            while (curAlpha < 1.0f)
            {
                curAlpha += 0.075f;
                if (curAlpha > 1.0f)
                {
                    curAlpha = 1.0f;
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
            while (curPosY < eMY)
            {
                curPosY += 20f;
                if (curPosY > eMY)
                {
                    curPosY = eMY;
                }
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, curPosY, gameObject.transform.position.z);
                yield return null;
            }
        }
        yield return new WaitForSeconds(0.4f);
        currentPlayerInput.enablePlayerInput = true;
        yield return null;
    }

    public virtual IEnumerator disableMenu(bool selectLock)
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
        additionalCloseConditions(selectLock);
        gameObject.SetActive(false);
        yield return null;
    }

    protected virtual void additionalCloseConditions(bool condition)
    {

    }

    public virtual void closeMenu()
    {
        csPlayerInput = GameObject.Find("Player " + pMenuCode).GetComponent<CSPlayerInput>();
        sfxPlayer.PlaySound("Cancel");
        csPlayerInput.playerInputMode = "CharacterSelect";
        csPlayerInput.enablePlayerInput = false;
        resetScrollFrames();
        StartCoroutine("disableMenu", false);
    }

    public virtual void forceCloseMenu()
    {

    }

    public virtual void selectOption()
    {
        
    }

    public virtual void directionalController()
    {

    }

    public void resetScrollFrames()
    {
        scrollFrames = 0f;
        holdScroll = false;
        scrollTime = 0;
    }

    public virtual void removeHighlighter()
    {
        screenHighlighter.RemoveAt(0);
    }

    /*public void updateDetails(string dText)
    {
        gameObject.transform.Find("MenuDetails").gameObject.transform.Find("MenuDetails_Canvas").gameObject.transform.Find("MenuDetails_Text").gameObject.GetComponent<TextMeshProUGUI>().text = dText;
    }*/
}
