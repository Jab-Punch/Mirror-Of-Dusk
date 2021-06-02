using System.Collections;
using System.Collections.Generic;
//using SpriteGlow;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Rendering.PostProcessing;


public class CSPlayerGUI : MonoBehaviour, IHurtboxResponder
{
    private CharacterSelectManager characterSelectManager;

    private bool _inUse = false; //Disable GUI input from other players if the menu is in use.
    public bool inUse {
        get { return _inUse; }
        set { _inUse = value; }
    }
    private int _bufferGUI = 0; //A frame countdown for when GUI must buffer before allowing input.
    public int bufferGUI
    {
        get { return _bufferGUI; }
        set { _bufferGUI = value; }
    }
    public enum MhFlags
    {
        P1 = (1 << 0),
        P2 = (1 << 1),
        P3 = (1 << 2),
        P4 = (1 << 3)
    }
    public class GuiSubHighlighted
    {
        public bool highlighted = false;
        public MhFlags mhFlags = 0;

        public void AddFlag(MhFlags flag)
        {
            highlighted = true;
            mhFlags |= flag;
        }
        public void ClearFlags()
        {
            highlighted = false;
            mhFlags = 0;
        }
    }
    public Dictionary<HighlightedSubSection, GuiSubHighlighted> highlightedSubSections;
    private Dictionary<HighlightedSubSection, bool> formerHighlightedSubSections;
    private List<int> currentHighlightCheck;
    private int currentHighlightFrame = 0;
    private int currentHighlightSelect = 0;
    public class ModeHighlighted
    {
        public MhFlags mhFlags;
        public MhFlags mhFlagsFound;
    }
    public enum HighlightedSubSection
    {
        UserName,
        Color,
        HP,
        Shards,
        Control
    }
    public enum GUIMode
    {
        Player,
        CPU,
        None
    }

    private ActivePlayers activePlayers;
    private SummonCursors summonCursors;
    public int guiID = 0;
    private string _playerIdentity;
    public string playerIdentity
    {
        get { return _playerIdentity; }
        set { _playerIdentity = value; }
    }
    private string _defaultPlayerIdentity;
    public string defaultPlayerIdentity
    {
        get { return _defaultPlayerIdentity; }
        set { _defaultPlayerIdentity = value; }
    }
    public int currentPlayerUserNameCode = 0;
    public GUIMode currentGUIMode = GUIMode.None;
    private Text playerGUIText;
    public PlayerGUITextSizeChecker playerGUITextSize;


    public class PlayerGUITextSizeChecker {
        public Text guiText;
        public string playerIdentity;
        public int FontSize = 30;

        public string PlayerIdentity
        {
            set {
                playerIdentity = value;
                guiText.text = playerIdentity;
                if (guiText.preferredWidth > 200)
                {
                    FontSize = 20;
                }
                else if (guiText.preferredWidth > 160)
                {
                    FontSize = 24;
                } else
                {
                    FontSize = 30;
                }
            }
        }
    }

    SpriteRenderer guiSprite;
    SpriteRenderer guiSubSprite;
    public Dictionary<int, ModeHighlighted> modeHighlighted = new Dictionary<int, ModeHighlighted> {
        { 0, new ModeHighlighted() },
        { 1, new ModeHighlighted() },
        { 2, new ModeHighlighted() }
    };
    SpriteRenderer[] iconSprites;
    private bool iconsOn = false;
    SpriteRenderer pGUILabel;
    SpriteRenderer pGUICharacterName;
    SpriteRenderer joinText;
    private bool joinTextOn = false;
    private bool joinCoroutineActive = false;
    private Color[] guiColors;
    GameObject pGUIAlterHP;
    GameObject pGUIAlterShards;
    private bool _enableModes = true;
    public bool enableModes
    {
        get { return _enableModes; }
        set { _enableModes = value; }
    }

    public GameObject gleamName;
    /*SpriteRenderer pGUICharacterNameGleam;
    SpriteGlowEffect spriteGlowEffect;
    PostProcessVolume sgeVolume;
    Bloom bloomLayer = null;*/

    private bool scriptOn = false;
    public bool refreshGUIOnce = false;

    private class TempFDC   //Instance of FighterDataCollection (Data of character colors and statuses)
    {
        public string name;
        public int id;
        public bool active = true;
        public GameObject gm;
        private FighterData _fighterData;

        public TempFDC(string name, int id, bool active, GameObject gm)
        {
            this.name = name;
            this.id = id;
            this.active = active;
            this.gm = gm;
            _fighterData = gm.GetComponent<FighterData>();
        }

        public FighterData fighterData
        {
            get { return _fighterData; }
        }
    }

    CSPlayerInput csPlayerInput;
    CSPlayerInput[] csPlayerInputs;
    CSPlayerData csPlayerData;
    CharacterSelectShard csShard;
    CSPlayerGUI[] guis;
    GameData gameData;
    FighterDataCollection fighterDataCollection;
    private TempFDC[] fighterSelectData;
    private SpriteRenderer[] guiBack;
    private MaterialPropertyBlock[] guiBackMat;
    private Color[] guiBackShad;
    private Color guiTintOff = new Color(0f, 0f, 0f, 1.0f);
    private Color guiTintOn = new Color(110f/255f, 20f/255f, 205f/255f, 1.0f);
    private Hurtbox[] hurtboxSet;

    [Header("Manager Prefabs")]
    public GameObject _joinText;
    public GameObject _pGUILabel;
    public GameObject _pGUISub;
    public GameObject _pGUICharacterName;
    public GameObject _pGUIAlterHP;
    public GameObject _pGUIAlterShards;
    public GameObject _playerGUIText;
    public GameObject[] _guiBack;
    public GameObject _hurtboxSet;

    [Header("Sprite Prefabs")]
    public Sprite[] _pGUILabelNo;
    public Sprite[] sprName;

    // Use this for initialization
    void Awake()
    {
        characterSelectManager = CharacterSelectManager.characterSelectManager;
        inUse = false;
        activePlayers = characterSelectManager.activePlayers.GetComponent<ActivePlayers>();
        gameData = characterSelectManager.gameData.GetComponent<GameData>();
        csPlayerInput = characterSelectManager.players[guiID].GetComponent<CSPlayerInput>();
        csPlayerInputs = new CSPlayerInput[4];
        for (int i = 0; i < csPlayerInputs.Length; i++)
        {
            csPlayerInputs[i] = characterSelectManager.players[i].GetComponent<CSPlayerInput>();
        }
        csPlayerData = characterSelectManager.players[guiID].GetComponent<CSPlayerData>();
        csShard = characterSelectManager.csShards.transform.GetChild(guiID).GetComponent<CharacterSelectShard>();
        guis = new CSPlayerGUI[4];
        for (int i = 0; i < guis.Length; i++)
        {
            guis[i] = characterSelectManager.csPlayerGUI.transform.GetChild(i).GetComponent<CSPlayerGUI>();
        }
        summonCursors = characterSelectManager.csControl.GetComponent<SummonCursors>();
        fighterDataCollection = characterSelectManager.fighterDataCollection.GetComponent<FighterDataCollection>();
        fighterSelectData = new TempFDC[fighterDataCollection.fighterSelectData.Length];
        for (int i = 0; i < fighterSelectData.Length; i++)
        {
            fighterSelectData[i] = new TempFDC(fighterDataCollection.fighterSelectData[i].name, fighterDataCollection.fighterSelectData[i].id, fighterDataCollection.fighterSelectData[i].active, fighterDataCollection.fighterSelectData[i].fighterData);
        }
        highlightedSubSections = new Dictionary<HighlightedSubSection, GuiSubHighlighted>
        {
            { HighlightedSubSection.UserName, new GuiSubHighlighted() },
            { HighlightedSubSection.Color, new GuiSubHighlighted() },
            { HighlightedSubSection.HP, new GuiSubHighlighted() },
            { HighlightedSubSection.Shards, new GuiSubHighlighted() },
            { HighlightedSubSection.Control, new GuiSubHighlighted() }
        };
        formerHighlightedSubSections = new Dictionary<HighlightedSubSection, bool>
        {
            { HighlightedSubSection.UserName, false },
            { HighlightedSubSection.Color, false },
            { HighlightedSubSection.HP, false },
            { HighlightedSubSection.Shards, false },
            { HighlightedSubSection.Control, false }
        };
        currentHighlightCheck = new List<int>();
        currentHighlightFrame = 0;

        guiColors = new Color[] {
            new Color(1f, 1f, 1f, 1f),
            new Color(0.5f, 0.7f, 0.75f, 1f),
            new Color(0.2f, 0.2f, 0.2f, 1f)
        };

        joinText = _joinText.GetComponent<SpriteRenderer>();
        guiSprite = _pGUILabel.GetComponent<SpriteRenderer>();
        guiSubSprite = _pGUISub.GetComponent<SpriteRenderer>();
        pGUILabel = _pGUILabel.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>();
        pGUILabel.sprite = _pGUILabelNo[guiID];
        pGUICharacterName = _pGUICharacterName.GetComponent<SpriteRenderer>();
        guiBack = new SpriteRenderer[_guiBack.Length];
        guiBackMat = new MaterialPropertyBlock[_guiBack.Length];
        for (int i = 0; i < _guiBack.Length; i++)
        {
            guiBackMat[i] = new MaterialPropertyBlock();
            guiBack[i] = _guiBack[i].transform.GetChild(0).GetComponent<SpriteRenderer>();
            guiBack[i].GetPropertyBlock(guiBackMat[i]);
            //guiBackMat[i] = guiBack[i].material;
            //guiBackShad[i] = guiBack[i].sharedMaterial.color;
        }
        pGUIAlterHP = _pGUIAlterHP.transform.GetChild(1).gameObject;
        pGUIAlterShards = _pGUIAlterShards.transform.GetChild(1).gameObject;
        defaultPlayerIdentity = "PLAYER " + (guiID + 1).ToString();
        if (currentPlayerUserNameCode != 0 && currentPlayerUserNameCode < gameData.userNameReference.Length)
        {
            playerIdentity = gameData.userNameReference[currentPlayerUserNameCode].Name;
        } else
        {
            playerIdentity = defaultPlayerIdentity;
        }
        playerGUIText = _playerGUIText.transform.GetChild(0).GetComponent<Text>();
        playerGUITextSize = new PlayerGUITextSizeChecker();
        playerGUITextSize.guiText = _playerGUIText.transform.GetChild(1).GetComponent<Text>();
        iconSprites = new SpriteRenderer[4];
        var tempCSP = Instantiate(gleamName, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 145, gameObject.transform.position.z - 3), Quaternion.identity);
        Destroy(tempCSP);
        int iconNum = 0;
        foreach (Transform ch in gameObject.transform)
        {
            if (ch.gameObject.name.Contains("CSPlayerGUIIcon_"))
            {
                iconSprites[iconNum] = ch.GetComponent<SpriteRenderer>();
                iconNum++;
            }
        }
        hurtboxSet = _hurtboxSet.GetComponentsInChildren<Hurtbox>();
    }

    void Start()
    {
        joinText.enabled = false;
        //pGUICharacterNameBurst = gameObject.transform.Find("CharacterSelectCharacterNameLight").GetComponentInChildren<SpriteRenderer>();
        //pGUICharacterNameGleam = gameObject.transform.Find("CharacterSelectCharacterNameBright").GetComponentInChildren<SpriteRenderer>();
        //spriteGlowEffect = gameObject.transform.Find("CharacterSelectCharacterNameBright").GetComponentInChildren<SpriteGlowEffect>();
        //spriteGlowEffect.enabled = false;
        //sgeVolume = gameObject.transform.Find("CharacterSelectCharacterNameBright").GetComponent<PostProcessVolume>();
        //Sprite[] spr = Resources.LoadAll<Sprite>("CharacterSelect/" + "CharacterSelectPlayerGNo");

        playerGUITextSize.PlayerIdentity = playerIdentity;
        playerGUIText.fontSize = playerGUITextSize.FontSize;
        playerGUIText.text = playerIdentity;
        /*if (playerGUIText.rectTransform.rect.width > 200)
        {
            playerGUIText.fontSize = 20;
        }
        else if (playerGUIText.rectTransform.rect.width > 160 && playerGUIText.rectTransform.rect.width <= 200)
        {
            playerGUIText.fontSize = 24;
        }*/
        refreshGUIOnce = false;
        setIcons(false);

        for (int i = 0; i < hurtboxSet.Length; i++)
        {
            hurtboxSet[i].useHurtResponder(this);
        }
        scriptOn = true;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (joinTextOn)
        {
            if (!csShard.gameObject.activeSelf)
            {
                if (!joinCoroutineActive)
                {
                    StartCoroutine("JoinFlash");
                }
            }
            else
            {
                if (joinCoroutineActive)
                {
                    StopCoroutine("JoinFlash");
                    joinText.enabled = false;
                    joinCoroutineActive = false;
                }
            }
        }
        for (int i = 0; i < guiBack.Length; i++)
        {
            guiBackMat[i].SetColor("_Color", guiTintOff);
            guiBack[i].SetPropertyBlock(guiBackMat[i]);
        }
        if (currentGUIMode != GUIMode.None)
        {
            if (!iconsOn)
            {
                setIcons(true);
            }
            iconsOn = true;
            int hCount = 0;
            for (int key = 0; key < highlightedSubSections.Count; key++)
            {
                if (highlightedSubSections[(HighlightedSubSection)key].highlighted)
                {
                    MhFlags hFlags = highlightedSubSections[(HighlightedSubSection)key].mhFlags;
                    
                    for (int i = 0; i < csPlayerInputs.Length; i++)
                    {
                        if (!HasGUISubFlags(csPlayerInputs[i].playerCursorFoundFlags))
                        {
                            if (hFlags.HasFlag((MhFlags)(1 << i)))
                            {
                                guiBackMat[key].SetColor("_Color", guiTintOn);
                                guiBack[key].SetPropertyBlock(guiBackMat[key]);
                                csPlayerInputs[i].playerCursorFoundFlags |= (CSPlayerInput.CursorFoundFlags)(1 << (key + 4));
                            }
                        }
                    }
                    //guiBack.sprite = new Color(1.0f);
                    hCount++;
                }
            }
            UpdateHighlighted(hCount);
            for (int key = 0; key < highlightedSubSections.Count; key++)
            {
                highlightedSubSections[(HighlightedSubSection)key].ClearFlags();
            }
            pGUICharacterName.sprite = sprName[csPlayerData.characterCode];
            float pGUIAlpha = 1.0f;
            if (csPlayerData.characterCode == 0 && !csPlayerInput.characterSelected)
            {
                pGUIAlpha = 0.0f;
            }
            pGUICharacterName.color = new Color(pGUICharacterName.color.r, pGUICharacterName.color.g, pGUICharacterName.color.b, pGUIAlpha);
            if (csPlayerData.playerHealth == gameData.defaultInitialHealth)
            {
                pGUIAlterHP.SetActive(false);
            }
            else
            {
                pGUIAlterHP.SetActive(true);
            }
            if (csPlayerData.playerInitShards == gameData.defaultInitialShards && csPlayerData.playerShardStrength == gameData.defaultShardStrength)
            {
                pGUIAlterShards.SetActive(false);
            }
            else
            {
                pGUIAlterShards.SetActive(true);
            }
            refreshGUIOnce = false;
        }
        else
        {
            if (iconsOn)
            {
                setIcons(false);
            }
            iconsOn = false;
        }
        bufferGUI = (bufferGUI > 0) ? bufferGUI - 1 : 0;
    }

    void OnEnable()
    {
        if (scriptOn)
        {
            bufferGUI = 30;
            guiSprite.color = guiColors[2];
            guiSubSprite.color = guiColors[2];
            StartCoroutine("summonPGUI");
            refreshGUIOnce = true;
        }
        HitboxEventManager.StartListening("UpdateGUISettings", UpdateGUISettings);
    }

    void OnDisable()
    {
        HitboxEventManager.StopListening("UpdateGUISettings", UpdateGUISettings);
    }

    public IEnumerator summonPGUI()
    {
        float endMoveY = -420f;
        float curPosY = gameObject.transform.position.y;
        while (curPosY < endMoveY)
        {
            curPosY += 30f;
            if (curPosY > endMoveY)
            {
                curPosY = endMoveY;
            }
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, curPosY, gameObject.transform.position.z);
            yield return null;
        }
        joinTextOn = true;
        yield return null;
    }

    public IEnumerator destroyPGUI()
    {
        joinTextOn = false;
        float endMoveY = -1050f;
        float curPosY = gameObject.transform.position.y;
        while (curPosY > endMoveY)
        {
            curPosY -= 30f;
            if (curPosY < endMoveY)
            {
                curPosY = endMoveY;
            }
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, curPosY, gameObject.transform.position.z);
            yield return null;
        }
        this.gameObject.SetActive(false);
        yield return null;
    }

    public IEnumerator nameBurst()
    {
        var tempCSP = Instantiate(gleamName, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 145, gameObject.transform.position.z - 3), Quaternion.identity);
        SpriteRenderer pGUICharacterNameBurst = tempCSP.GetComponent<SpriteRenderer>();
        pGUICharacterNameBurst.sprite = sprName[csPlayerData.characterCode];
        float scal = 1.0f;
        float alpha = 1.0f;
        pGUICharacterNameBurst.transform.localScale = new Vector3(scal, scal, 1.0f);
        pGUICharacterNameBurst.color = new Color(pGUICharacterNameBurst.color.r, pGUICharacterNameBurst.color.g, pGUICharacterNameBurst.color.b, alpha);
        yield return null;
        for (int j = 0; j < 8; j++)
        {
            pGUICharacterNameBurst.transform.localScale += new Vector3(0.015f, 0.04f, 0.0f);
            yield return null;
        }
        while (alpha > 0)
        {
            pGUICharacterNameBurst.transform.localScale += new Vector3(0.015f, 0.04f, 0.0f);
            pGUICharacterNameBurst.color = new Color(pGUICharacterNameBurst.color.r, pGUICharacterNameBurst.color.g, pGUICharacterNameBurst.color.b, alpha);
            alpha -= 0.05f;
            yield return null;
        }
        pGUICharacterNameBurst.color = new Color(pGUICharacterNameBurst.color.r, pGUICharacterNameBurst.color.g, pGUICharacterNameBurst.color.b, 0.0f);
        yield return null;
        Destroy(tempCSP);
    }

    /*public IEnumerator nameGleam()
    {
        spriteGlowEffect.enabled = true;
        float scal = 1.0f;
        pGUICharacterNameGleam.transform.localScale = new Vector3(scal, scal, 1.0f);
        float alpha = 1.0f;
        pGUICharacterNameGleam.color = new Color(pGUICharacterNameGleam.color.r, pGUICharacterNameGleam.color.g, pGUICharacterNameGleam.color.b, alpha);
        spriteGlowEffect.GlowColor = new Color(spriteGlowEffect.GlowColor.r, spriteGlowEffect.GlowColor.g, spriteGlowEffect.GlowColor.b, alpha);
        Color tmpA = pGUICharacterNameGleam.color;
        Color tmpB = spriteGlowEffect.GlowColor;
        tmpA.a = 1.0f;
        tmpB.a = 1.0f;
        yield return null;
        sgeVolume.profile.TryGetSettings(out bloomLayer);
        bloomLayer.enabled.value = true;
        bloomLayer.intensity.value = 5;
        for (int j = 0; j < 8; j++)
        {
            pGUICharacterNameGleam.transform.localScale += new Vector3(0.015f, 0.04f, 0.0f);
            bloomLayer.enabled.value = true;
            bloomLayer.intensity.value += 1;
            yield return null;
        }
        float gAlpha = 1.0f;
        while (alpha > 0)
        {
            gAlpha -= 0.05f;
            bloomLayer.intensity.value += 1;
            pGUICharacterNameGleam.transform.localScale += new Vector3(0.015f, 0.04f, 0.0f);
            pGUICharacterNameGleam.color = new Color(pGUICharacterNameGleam.color.r, pGUICharacterNameGleam.color.g, pGUICharacterNameGleam.color.b, alpha);
            spriteGlowEffect.GlowColor = new Color(spriteGlowEffect.GlowColor.r, spriteGlowEffect.GlowColor.g, spriteGlowEffect.GlowColor.b, gAlpha);
            alpha -= 0.05f;
            yield return null;
        }
        pGUICharacterNameGleam.color = new Color(pGUICharacterNameGleam.color.r, pGUICharacterNameGleam.color.g, pGUICharacterNameGleam.color.b, 0.0f);
        spriteGlowEffect.enabled = false;
        yield return null;
    }*/

    public IEnumerator JoinFlash()
    {
        joinCoroutineActive = true;
        while (true)
        {
            joinText.enabled = true;
            yield return new WaitForSeconds(0.75f);
            joinText.enabled = false;
            yield return new WaitForSeconds(0.25f);
        }
    }

    public void setIcons(bool on)
    {
        for (int i = 0; i < iconSprites.Length; i++)
        {
            iconSprites[i].enabled = on;
            pGUICharacterName.enabled = on;
            playerGUIText.enabled = on;
        }
        if (!on)
        {
            pGUIAlterHP.SetActive(false);
            pGUIAlterShards.SetActive(false);
        }
    }

    public void updateColorText()
    {
        string resultText = "No Color";
        for (int i = 0; i < fighterSelectData.Length; i++)
        {
            if (csPlayerData.characterCode == 0)
            {
                break;
            }
            if (fighterSelectData[i].name == csPlayerData.selectedCharacter && fighterSelectData[i].active)
            {
                for (int j = 0; j < fighterSelectData[i].fighterData.fighterColorData.Length; j++)
                {
                    if (csPlayerData.characterColorCode == j)
                    {
                        playerGUIText.fontSize = 24;
                        resultText = System.Convert.ToString(fighterSelectData[i].fighterData.fighterColorData[j].colorCode) + ": " + fighterSelectData[i].fighterData.fighterColorData[j].name;
                        break;
                    }
                }
                break;
            }
        }
        playerGUIText.text = resultText;
    }

    public void UpdateGUIColor(int mode)
    {
        guiSprite.color = guiColors[mode];
        guiSubSprite.color = guiColors[mode];
    }

    void UpdateHighlighted(int hCount)
    {
        if (hCount > 0)
        {
            for (int i = 0; i < formerHighlightedSubSections.Count; i++)
            {
                if (formerHighlightedSubSections[(HighlightedSubSection)i] != highlightedSubSections[(HighlightedSubSection)i].highlighted)
                {
                    if (!formerHighlightedSubSections[(HighlightedSubSection)i])
                    {
                        formerHighlightedSubSections[(HighlightedSubSection)i] = true;
                        currentHighlightCheck.Add(i);
                    } else
                    {
                        formerHighlightedSubSections[(HighlightedSubSection)i] = false;
                        currentHighlightCheck.Remove(i);
                    }
                    currentHighlightCheck.Sort();
                    currentHighlightFrame = 0;
                    refreshGUIOnce = true;
                    for (int j = 0; j < currentHighlightCheck.Count; j++)
                    {
                        if (currentHighlightCheck[j] == i)
                        {
                            currentHighlightSelect = j;
                            break;
                        }
                    }
                }
            }
            currentHighlightFrame++;
            if (currentHighlightFrame >= 120 || refreshGUIOnce)
            {
                if (currentHighlightFrame >= 120)
                {
                    currentHighlightSelect++;
                    currentHighlightFrame = 0;
                }
                if (currentHighlightSelect >= currentHighlightCheck.Count)
                {
                    currentHighlightSelect = 0;
                }
                switch ((HighlightedSubSection)currentHighlightCheck[currentHighlightSelect])
                {
                    case HighlightedSubSection.Color:
                        playerGUIText.fontSize = 30;
                        updateColorText();
                        break;
                    case HighlightedSubSection.HP:
                        playerGUIText.fontSize = 30;
                        playerGUIText.text = csPlayerData.playerHealth.ToString() + " HP";
                        break;
                    case HighlightedSubSection.Shards:
                        playerGUIText.fontSize = 30;
                        playerGUIText.text = csPlayerData.playerInitShards + " / " + gameData.defaultTotalShards;
                        break;
                    case HighlightedSubSection.Control:
                        playerGUIText.fontSize = 30;
                        playerGUIText.text = "CONTROLS";
                        break;
                    default:
                        if (currentGUIMode == GUIMode.Player)
                        {
                            playerGUIText.fontSize = playerGUITextSize.FontSize;
                            playerGUIText.text = playerIdentity;
                        }
                        else if (currentGUIMode == GUIMode.CPU)
                        {
                            playerGUIText.fontSize = 30;
                            playerGUIText.text = "CPU";
                        }
                        else
                        {
                            playerGUIText.fontSize = 30;
                            playerGUIText.text = "";
                        }
                        break;
                }
            }
        } else
        {
            currentHighlightFrame = 120;
            currentHighlightSelect = 0;
            currentHighlightCheck.Clear();
            for (int i = 0; i < formerHighlightedSubSections.Count; i++)
            {
                formerHighlightedSubSections[(HighlightedSubSection)i] = false;
            }
            if (currentGUIMode == GUIMode.Player)
            {
                playerGUIText.fontSize = playerGUITextSize.FontSize;
                playerGUIText.text = playerIdentity;
            }
            else if (currentGUIMode == GUIMode.CPU)
            {
                playerGUIText.fontSize = 30;
                playerGUIText.text = "CPU";
            }
            else
            {
                playerGUIText.fontSize = 30;
                playerGUIText.text = "";
            }
        }
        
    }

    void UpdateGUISettings(System.Object arg0, System.Object arg1, System.Object arg2)
    {
        int gID = (int)arg0;
        int gMode = (int)arg1;
        if (gID == guiID)
        {
            currentGUIMode = (GUIMode)gMode;
            UpdateGUIColor(gMode);
            if (csPlayerInput.currentCursorStar.gameObject.activeSelf && currentGUIMode == CSPlayerGUI.GUIMode.None)
            {
                summonCursors.StartCoroutine("beginDestroyCStar", gID);
                summonCursors.StartCoroutine("beginDestroyAPlayerGUI", gID);
            }
            refreshGUIOnce = true;
            StartCoroutine("StallModes");
        }
    }

    private IEnumerator StallModes()
    {
        enableModes = false;
        yield return new WaitForSeconds(0.6f);
        enableModes = true;
        yield return null;
    }

    public void collisionDetected(Hitbox hitbox, Hurtbox hurtbox)
    {
        if (hitbox.hitboxData.playerId == guiID || currentGUIMode == GUIMode.CPU)
        {
            switch (hurtbox.hurtboxData.name)
            {
                case "CSPlayerGUIBackLargeCollider":
                    //guis[hitbox.hitboxData.playerId].highlightedSubSection = HighlightedSubSection.Color;
                    highlightedSubSections[HighlightedSubSection.UserName].AddFlag((MhFlags)(1 << hitbox.hitboxData.playerId));
                    csPlayerInputs[hitbox.hitboxData.playerId].guiMenuPickFlags |= (CSPlayerInput.GUIMenuPickFlags)(1 << guiID);
                    break;
                case "CSPlayerGUIBackSmall_Color":
                    //guis[hitbox.hitboxData.playerId].highlightedSubSection = HighlightedSubSection.Color;
                    highlightedSubSections[HighlightedSubSection.Color].AddFlag((MhFlags)(1 << hitbox.hitboxData.playerId));
                    csPlayerInputs[hitbox.hitboxData.playerId].guiMenuPickFlags |= (CSPlayerInput.GUIMenuPickFlags)(1 << guiID);
                    break;
                case "CSPlayerGUIBackSmall_HP":
                    //guis[hitbox.hitboxData.playerId].highlightedSubSection = HighlightedSubSection.HP;
                    highlightedSubSections[HighlightedSubSection.HP].AddFlag((MhFlags)(1 << hitbox.hitboxData.playerId));
                    csPlayerInputs[hitbox.hitboxData.playerId].guiMenuPickFlags |= (CSPlayerInput.GUIMenuPickFlags)(1 << guiID);
                    break;
                case "CSPlayerGUIBackSmall_Shard":
                    //guis[hitbox.hitboxData.playerId].highlightedSubSection = HighlightedSubSection.Shards;
                    highlightedSubSections[HighlightedSubSection.Shards].AddFlag((MhFlags)(1 << hitbox.hitboxData.playerId));
                    csPlayerInputs[hitbox.hitboxData.playerId].guiMenuPickFlags |= (CSPlayerInput.GUIMenuPickFlags)(1 << guiID);
                    break;
                case "CSPlayerGUIBackSmall_Control":
                    //guis[hitbox.hitboxData.playerId].highlightedSubSection = HighlightedSubSection.Control;
                    highlightedSubSections[HighlightedSubSection.Control].AddFlag((MhFlags)(1 << hitbox.hitboxData.playerId));
                    csPlayerInputs[hitbox.hitboxData.playerId].guiMenuPickFlags |= (CSPlayerInput.GUIMenuPickFlags)(1 << guiID);
                    break;
                default:
                    //highlightedSubSection = HighlightedSubSection.Default;
                    break;
            }
        }
    }

    private bool HasGUISubFlags(CSPlayerInput.CursorFoundFlags plyr)
    {
        for (int i = 0; i < 5; i++)
        {
            if (plyr.HasFlag((CSPlayerInput.CursorFoundFlags)(1 << (i + 4))))
            {
                return true;
            }
        }
        return false;
    }
}
