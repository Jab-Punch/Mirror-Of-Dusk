using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rewired;

public class SelectUserNewNameScreen : HorizontalMenuScreen
{
    public enum LetterOption
    {
        RomaniUpper,
        RomaniLower,
        JapaneseHiragana,
        JapaneseKatakana,
        Symbol,
        Latin
    }

    public enum LetterFunction
    {
        Default,
        Constanant,
        Daku,
        Handaku
    }

    public class LetterChar
    {
        private char _letter;
        private char _constanantLetter;
        private char _dakuLetter;
        private char _handakuLetter;
        private LetterFunction _letterFunction;

        public LetterChar(char ch)
        {
            _letter = ch;
            _letterFunction = LetterFunction.Default;
        }

        public LetterChar(char ch, LetterFunction _LF)
        {
            _letter = ch;
            _letterFunction = _LF;
        }

        public LetterChar(char ch, LetterFunction _LF, char cch)
        {
            _letter = ch;
            _letterFunction = _LF;
            if (_letterFunction == LetterFunction.Constanant)
            {
                _constanantLetter = cch;
            }
            else if (_letterFunction == LetterFunction.Daku)
            {
                _dakuLetter = cch;
            }
        }

        public LetterChar(char ch, LetterFunction _LF, char ch1, char ch2)
        {
            _letter = ch;
            _letterFunction = _LF;
            if (_letterFunction == LetterFunction.Constanant)
            {
                _constanantLetter = ch1;
                _dakuLetter = ch2;
            } else if (_letterFunction == LetterFunction.Daku || _letterFunction == LetterFunction.Handaku)
            {
                _dakuLetter = ch1;
                _handakuLetter = ch2;
            }
        }

        public char Letter
        {
            get { return _letter; }
            set { _letter = value; }
        }

        public char ConstanantLetter
        {
            get {
                return _constanantLetter;
            }
        }

        public char DakuLetter
        {
            get
            {
                return _dakuLetter;
            }
        }

        public char HandakuLetter
        {
            get
            {
                return _handakuLetter;
            }
        }

        public LetterFunction letterFunction
        {
            get { return _letterFunction; }
        }

        public bool ConstanantExists
        {
            get
            {
                if (_constanantLetter != '\0')
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool DakuExists
        {
            get { if (_dakuLetter != '\0')
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool HandakuExists
        {
            get
            {
                if (_handakuLetter != '\0')
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }

    public class LetterSet
    {
        Dictionary<LetterOption, char> letterChoice;
        LetterOption _currentLetterOption;

        public LetterSet(char _RU, char _RL, char _JH, char _JK, char _S, char _L)
        {
            letterChoice = new Dictionary<LetterOption, char>();
            letterChoice.Add(LetterOption.RomaniUpper, _RU);
            letterChoice.Add(LetterOption.RomaniLower, _RL);
            letterChoice.Add(LetterOption.JapaneseHiragana, _JH);
            letterChoice.Add(LetterOption.JapaneseKatakana, _JK);
            letterChoice.Add(LetterOption.Symbol, _S);
            letterChoice.Add(LetterOption.Latin, _L);
            _currentLetterOption = LetterOption.RomaniUpper;
        }

        private char _selectedChoice
        {
            get { return letterChoice[_currentLetterOption]; }
        }

        public char SelectedChoice(LetterOption _LO)
        {
            _currentLetterOption = _LO;
            return _selectedChoice;
        }
    }

    [System.Serializable]
    public class LetterOptionShiftStyles
    {
        public LetterOption _letterOption;
        public GameObject _gm;
    }

    private CharacterSelectManager characterSelectManager;
    private MenuEventManager menuEventManagerInstance;

    public int playerId = 0;
    private int c_playerId;
    private Player playerControl;
    private CSPlayerGUI csPlayerGUI;

    public enum NameEditMode
    {
        EnterName,
        EditName
    }
    private NameEditMode nameEditMode = NameEditMode.EnterName;
    private int currentSection;
    private int currentColumn;
    private int currentRow;
    private LetterOption currentLetterOption;
    private bool capsOn = true;
    private bool hiraganaOn = true;
    private Dictionary<int, Dictionary<int, LetterSet>> letterMenuSlots;
    private MenuSlot letterSelectSlot;
    public MenuSlot[] letterOptionSlots;
    public MenuSlot[] menuSettingSlots;
    [SerializeField] private LetterOptionShiftStyles[] _letterOptionShiftStyles;
    private Dictionary<LetterOption, GameObject> letterOptionShiftStyles;
    
    private SpriteRenderer letterBoxHighlight;
    private SpriteRenderer boxHighlight;
    private SpriteRenderer background;
    private bool bufferOn = false;

    private SelectUserMenuScreen newNameRootMenu;
    private string nameTextInput;
    private string nameTextNext;
    private TextMeshProUGUI nameTextField;

    [SerializeField] private GameObject[] typeSectionSet;
    private GameObject currentTypeSection = null;
    private Dictionary<char, LetterChar> letterCollection;

    [Header("Manager Prefabs")]
    public GameObject _nameTextField;

    [Header("Sprite Prefabs")]
    public GameObject _typeGlyphSection;
    public GameObject _letterSelectSlot;
    public GameObject _letterBoxHighlight;
    public GameObject _background;

    private void Awake()
    {
        menuEventManagerInstance = gameObject.GetComponent<MenuEventManager>();
        setUpMenu();
        initializeHorizontalScreen();
        initializeNewNameScreen();
    }

    void Start()
    {
        scriptOn = true;
        gameObject.SetActive(false);
    }

    void Update()
    {

    }

    public override void OnEnable()
    {
        if (nameEditMode == NameEditMode.EnterName)
        {
            nameTextInput = "";
            nameTextField.text = nameTextInput + nameTextNext;
        } else if (nameEditMode == NameEditMode.EditName)
        {
            nameTextInput = gameData.userNameReference[newNameRootMenu.SelectedUserId].Name;
            nameTextField.text = nameTextInput + ((nameTextInput.Length < 10) ? nameTextNext : "");
        }
        currentSection = 1;
        currentColumn = 0;
        currentRow = 0;
        for (int i = 0; i < letterOptionSlots.Length; i++)
        {
            (letterOptionSlots[i] as EventMenuSlot).deselectUpdate();
        }
        for (int i = 0; i < letterOptionSlots.Length; i++)
        {
            letterOptionSlots[i].enabled = false;
        }
        for (int i = 0; i < menuSettingSlots.Length; i++)
        {
            menuSettingSlots[i].enabled = false;
        }
        for (int i = 0; i < _letterOptionShiftStyles.Length; i++)
        {
            letterOptionShiftStyles[_letterOptionShiftStyles[i]._letterOption].SetActive(false);
        }
        switch (currentLetterOption)
        {
            case LetterOption.RomaniUpper:
            case LetterOption.RomaniLower:
                (letterOptionSlots[0] as EventMenuSlot).selectUpdate();
                letterOptionShiftStyles[LetterOption.RomaniUpper].SetActive(true);
                break;
            case LetterOption.JapaneseHiragana:
                (letterOptionSlots[1] as EventMenuSlot).selectUpdate();
                letterOptionShiftStyles[LetterOption.JapaneseKatakana].SetActive(true);
                break;
            case LetterOption.JapaneseKatakana:
                (letterOptionSlots[1] as EventMenuSlot).selectUpdate();
                letterOptionShiftStyles[LetterOption.JapaneseHiragana].SetActive(true);
                break;
            case LetterOption.Symbol:
                (letterOptionSlots[2] as EventMenuSlot).selectUpdate();
                break;
            case LetterOption.Latin:
                (letterOptionSlots[3] as EventMenuSlot).selectUpdate();
                break;
            default:
                break;
        }
        _boxHighlight.SetActive(false);

        MoveLetterSelectSlot();

    }

    public override void OnDisable()
    {
        //horizontalMenuSlots[currentHorizSlot].subMenuSlots[horizontalMenuSlots[currentHorizSlot].currentMenuSlot].enabled = false;
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
    }

    protected override void initializeHorizontalScreen()
    {
        for (int i = 0; i < letterOptionSlots.Length; i++)
        {
            letterOptionSlots[i].enabled = false;
        }
        for (int i = 0; i < menuSettingSlots.Length; i++)
        {
            menuSettingSlots[i].enabled = false;
        }

        currentSection = 1;
        currentColumn = 0;
        currentRow = 0;
        currentLetterOption = LetterOption.RomaniUpper;

        InitializeLetters();

        letterMenuSlots = new Dictionary<int, Dictionary<int, LetterSet>>();
        letterMenuSlots.Add(0, new Dictionary<int, LetterSet> {
            { 0, new LetterSet('A', 'a', 'あ', 'ア', '!', 'À') },
            { 1, new LetterSet('B', 'b', 'か', 'カ', '?', 'à') },
            { 2, new LetterSet('C', 'c', 'さ', 'サ', '.', 'Ò') },
            { 3, new LetterSet('D', 'd', 'た', 'タ', ',', 'ò') },
            { 4, new LetterSet('E', 'e', 'な', 'ナ', '\'', 'È') },
            { 5, new LetterSet('F', 'f', 'は', 'ハ', '"', 'è') },
            { 6, new LetterSet('G', 'g', 'ま', 'マ', '`', 'Ì') },
            { 7, new LetterSet('H', 'h', 'や', 'ヤ', '~', 'ì') },
            { 8, new LetterSet('I', 'i', 'ら', 'ラ', ':', 'Ù') },
            { 9, new LetterSet('J', 'j', 'わ', 'ワ', ';', 'ù') },
            { 10, new LetterSet('K', 'k', 'ー', 'ー', '_', 'Ñ') },
            { 11, new LetterSet('L', 'l', ' ', ' ', ' ', 'ñ') }});
        letterMenuSlots.Add(1, new Dictionary<int, LetterSet> {
            { 0, new LetterSet('M', 'm', 'い', 'イ', '#', 'Á') },
            { 1, new LetterSet('N', 'n', 'き', 'キ', '+', 'á') },
            { 2, new LetterSet('O', 'o', 'し', 'シ', '-', 'Ó') },
            { 3, new LetterSet('P', 'p', 'ち', 'チ', '*', 'ó') },
            { 4, new LetterSet('Q', 'q', 'に', 'ニ', '÷', 'É') },
            { 5, new LetterSet('R', 'r', 'ひ', 'ヒ', '/', 'é') },
            { 6, new LetterSet('S', 's', 'み', 'ミ', '=', 'Í') },
            { 7, new LetterSet('T', 't', 'ゆ', 'ユ', '%', 'í') },
            { 8, new LetterSet('U', 'u', 'り', 'リ', '^', 'Ú') },
            { 9, new LetterSet('V', 'v', 'を', 'ヲ', '°', 'ú') },
            { 10, new LetterSet('W', 'w', '・', '・', '±', 'Ç') },
            { 11, new LetterSet('X', 'x', ' ', ' ', ' ', 'ç') }});
        letterMenuSlots.Add(2, new Dictionary<int, LetterSet> {
            { 0, new LetterSet('Y', 'y', 'う', 'ウ', '@', 'Â') },
            { 1, new LetterSet('Z', 'z', 'く', 'ク', '&', 'â') },
            { 2, new LetterSet('.', '.', 'す', 'ス', '<', 'Ô') },
            { 3, new LetterSet(',', ',', 'つ', 'ツ', '>', 'ô') },
            { 4, new LetterSet('/', '/', 'ぬ', 'ヌ', '(', 'Ê') },
            { 5, new LetterSet('~', '~', 'ふ', 'フ', ')', 'ê') },
            { 6, new LetterSet('!', '!', 'む', 'ム', '{', 'Î') },
            { 7, new LetterSet('?', '?', 'よ', 'ヨ', '}', 'î') },
            { 8, new LetterSet('&', '&', 'る', 'ル', '[', 'Û') },
            { 9, new LetterSet('-', '-', 'ん', 'ン', ']', 'û') },
            { 10, new LetterSet(':', ':', ' ', ' ', '●', 'Þ') },
            { 11, new LetterSet(';', ';', 'っ', 'ッ', '▲', 'þ') }});
        letterMenuSlots.Add(3, new Dictionary<int, LetterSet> {
            { 0, new LetterSet('0', '0', 'え', 'エ', '¡', 'Ä') },
            { 1, new LetterSet('1', '1', 'け', 'ケ', '¿', 'ä') },
            { 2, new LetterSet('2', '2', 'せ', 'セ', '\\', 'Ö') },
            { 3, new LetterSet('3', '3', 'て', 'テ', '|', 'ö') },
            { 4, new LetterSet('4', '4', 'ね', 'ネ', '$', 'Ë') },
            { 5, new LetterSet('5', '5', 'へ', 'へ', '¢', 'ë') },
            { 6, new LetterSet('6', '6', 'め', 'メ', '£', 'Ï') },
            { 7, new LetterSet('7', '7', '「', '「', '¥', 'ï') },
            { 8, new LetterSet('8', '8', 'れ', 'レ', '「', 'Ü') },
            { 9, new LetterSet('9', '9', '、', '、', '」', 'ü') },
            { 10, new LetterSet('#', '#', ' ', ' ', '◀', 'ß') },
            { 11, new LetterSet('*', '*', '゛', '゛', '▶', ' ') }});
        letterMenuSlots.Add(4, new Dictionary<int, LetterSet> {
            { 0, new LetterSet('_', '_', 'お', 'オ', '⇦', 'Ã') },
            { 1, new LetterSet('@', '@', 'こ', 'コ', '⇧', 'ã') },
            { 2, new LetterSet('^', '^', 'そ', 'ソ', '⇨', 'Õ') },
            { 3, new LetterSet('\'', '\'', 'と', 'ト', '⇩', 'õ') },
            { 4, new LetterSet('"', '"', 'の', 'ノ', '♂', 'Æ') },
            { 5, new LetterSet('(', '(', 'ほ', 'ホ', '♀', 'æ') },
            { 6, new LetterSet(')', ')', 'も', 'モ', '♠', ' ') },
            { 7, new LetterSet('[', '[', '」', '」', '♣', ' ') },
            { 8, new LetterSet(']', ']', 'ろ', 'ロ', '♥', ' ') },
            { 9, new LetterSet('°', '°', '。', '。', '♦', ' ') },
            { 10, new LetterSet('|', '|', ' ', ' ', '♪', ' ') },
            { 11, new LetterSet('\\', '\\', '゜', '゜', '♫', ' ') }});

        letterOptionShiftStyles = new Dictionary<LetterOption, GameObject>();
        for (int i = 0; i < _letterOptionShiftStyles.Length; i++)
        {
            letterOptionShiftStyles.Add(_letterOptionShiftStyles[i]._letterOption, _letterOptionShiftStyles[i]._gm);
            letterOptionShiftStyles[_letterOptionShiftStyles[i]._letterOption].SetActive(false);
        }
        currentTypeSection = typeSectionSet[(int)currentLetterOption];
        typeSectionSet[(int)currentLetterOption].SetActive(true);
        nameTextNext = "<color=red>_</color>";
    }

    private void initializeNewNameScreen()
    {
        c_playerId = playerId;
        csPlayerGUI = characterSelectManager.csPlayerGUI.transform.GetChild(playerId).GetComponent<CSPlayerGUI>();
        letterSelectSlot = _letterSelectSlot.GetComponent<MenuSlot>();
        letterBoxHighlight = _letterBoxHighlight.GetComponent<SpriteRenderer>();
        boxHighlight = _boxHighlight.GetComponent<SpriteRenderer>();
        background = _background.GetComponent<SpriteRenderer>();
        nameTextField = _nameTextField.GetComponent<TextMeshProUGUI>();
    }

    public void assignNewNameMenu(SelectUserMenuScreen rootMen)
    {
        newNameRootMenu = rootMen;
    }

    public void SetNameMode(NameEditMode _NEM)
    {
        nameEditMode = _NEM;
    }

    public override void directionalController()
    {
        if (!currentPlayerInput.onUI && !bufferOn)
        {
            padHorizontal = playerControl.GetAxisRaw("Move Horizontal");
            padVertical = playerControl.GetAxisRaw("Move Vertical");
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
            if ((inputReader.useNewInput("MoveRight", c_playerId) || inputReader.useNewInput("MoveDown_Right", c_playerId) || inputReader.useNewInput("MoveUp_Right", c_playerId)) && scrollFrames <= 0)
            {
                if (currentSection == 0)
                {
                    sfxPlayer.PlaySound("Scroll");
                    letterOptionSlots[currentColumn].enabled = false;
                    currentColumn = (currentColumn >= 3) ? 0 : ++currentColumn;
                    letterOptionSlots[currentColumn].enabled = true;
                }
                else if (currentSection == 1)
                {
                    sfxPlayer.PlaySound("Scroll");
                    currentColumn = ((currentColumn >= 11) ? 0 : ++currentColumn);
                    MoveLetterSelectSlot();
                }
                else if (currentSection == 2)
                {
                    sfxPlayer.PlaySound("Scroll");
                    menuSettingSlots[currentColumn].enabled = false;
                    currentColumn = (currentColumn >= 4) ? 0 : ++currentColumn;
                    menuSettingSlots[currentColumn].enabled = true;
                }
                scrollFrames = ((holdScroll) ? 5f : 20f);
                holdScroll = true;
            }
            else if ((inputReader.useNewInput("MoveLeft", c_playerId) || inputReader.useNewInput("MoveDown_Left", c_playerId) || inputReader.useNewInput("MoveUp_Left", c_playerId)) && scrollFrames <= 0)
            {
                if (currentSection == 0)
                {
                    sfxPlayer.PlaySound("Scroll");
                    letterOptionSlots[currentColumn].enabled = false;
                    currentColumn = (currentColumn <= 0) ? 3 : --currentColumn;
                    letterOptionSlots[currentColumn].enabled = true;
                }
                else if (currentSection == 1)
                {
                    sfxPlayer.PlaySound("Scroll");
                    currentColumn = (currentColumn <= 0) ? 11 : --currentColumn;
                    MoveLetterSelectSlot();
                }
                else if (currentSection == 2)
                {
                    sfxPlayer.PlaySound("Scroll");
                    menuSettingSlots[currentColumn].enabled = false;
                    currentColumn = (currentColumn <= 0) ? 4 : --currentColumn;
                    menuSettingSlots[currentColumn].enabled = true;
                }
                scrollFrames = ((holdScroll) ? 5f : 20f);
                holdScroll = true;
            }
            else if ((inputReader.useNewInput("MoveDown", c_playerId)) && scrollFrames <= 0 && (!inputReader.useNewInput("MoveLeft", c_playerId) && !inputReader.useNewInput("MoveRight", c_playerId)))
            {
                if (currentSection < 2)
                {
                    if (currentSection == 1)
                    {
                        sfxPlayer.PlaySound("Scroll");
                        if (currentRow >= 4)
                        {
                            currentSection = 2;
                            _boxHighlight.SetActive(true);
                            _letterBoxHighlight.SetActive(false);
                            if (currentColumn >= 0 && currentColumn <= 1)
                            {
                                currentColumn = 0;
                            }
                            else if (currentColumn >= 2 && currentColumn <= 4)
                            {
                                currentColumn = 1;
                            }
                            else if (currentColumn >= 5 && currentColumn <= 6)
                            {
                                currentColumn = 2;
                            }
                            else if (currentColumn >= 7 && currentColumn <= 9)
                            {
                                currentColumn = 3;
                            }
                            else if (currentColumn >= 10 && currentColumn <= 11)
                            {
                                currentColumn = 4;
                            }
                            menuSettingSlots[currentColumn].enabled = true;
                        }
                        else
                        {
                            currentRow++;
                            MoveLetterSelectSlot();
                        }
                    } else
                    {
                        sfxPlayer.PlaySound("Scroll");
                        letterOptionSlots[currentColumn].enabled = false;
                        _boxHighlight.SetActive(false);
                        _letterBoxHighlight.SetActive(true);
                        currentSection++;
                        switch (currentColumn)
                        {
                            case 0:
                                currentColumn = 0;
                                break;
                            case 1:
                                currentColumn = 4;
                                break;
                            case 2:
                                currentColumn = 7;
                                break;
                            case 3:
                                currentColumn = 11;
                                break;
                            default:
                                currentColumn = 0;
                                break;
                        }
                        MoveLetterSelectSlot();
                    }
                }
                scrollFrames = ((holdScroll) ? 5f : 20f);
                holdScroll = true;
            }
            else if (inputReader.useNewInput("MoveUp", c_playerId) && scrollFrames <= 0 && (!inputReader.useNewInput("MoveLeft", c_playerId) && !inputReader.useNewInput("MoveRight", c_playerId)))
            {
                if (currentSection < 2)
                {
                    if (currentSection == 1)
                    {
                        if (currentRow > 0)
                        {
                            sfxPlayer.PlaySound("Scroll");
                            currentRow--;
                            MoveLetterSelectSlot();
                        } else
                        {
                            sfxPlayer.PlaySound("Scroll");
                            currentSection = 0;
                            _boxHighlight.SetActive(true);
                            _letterBoxHighlight.SetActive(false);
                            if (currentColumn >= 0 && currentColumn <= 2)
                            {
                                currentColumn = 0;
                            }
                            else if (currentColumn >= 3 && currentColumn <= 5)
                            {
                                currentColumn = 1;
                            }
                            else if (currentColumn >= 6 && currentColumn <= 8)
                            {
                                currentColumn = 2;
                            }
                            else if (currentColumn >= 9 && currentColumn <= 11)
                            {
                                currentColumn = 3;
                            }
                            letterOptionSlots[currentColumn].enabled = true;
                        }
                    }
                }
                else
                {
                    sfxPlayer.PlaySound("Scroll");
                    menuSettingSlots[currentColumn].enabled = false;
                    _boxHighlight.SetActive(false);
                    _letterBoxHighlight.SetActive(true);
                    currentSection--;
                    switch (currentColumn)
                    {
                        case 0:
                            currentColumn = 0;
                            break;
                        case 1:
                            currentColumn = 3;
                            break;
                        case 2:
                            currentColumn = 5;
                            break;
                        case 3:
                            currentColumn = 8;
                            break;
                        case 4:
                            currentColumn = 11;
                            break;
                        default:
                            currentColumn = 0;
                            break;
                    }
                    MoveLetterSelectSlot();
                }
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
        bufferOn = false;
    }

    public override void selectOption()
    {
        bufferOn = true;
        if (currentSection == 0)
        {
            sfxPlayer.PlaySound("Select_1");
            menuEventManagerInstance.TriggerEvent("ActivateSlot", null);
        }
        else if (currentSection == 1)
        {
            if (letterMenuSlots[currentRow][currentColumn].SelectedChoice(currentLetterOption) != ' ')
            {
                if ((letterMenuSlots[currentRow][currentColumn].SelectedChoice(currentLetterOption) == 'っ' || letterMenuSlots[currentRow][currentColumn].SelectedChoice(currentLetterOption) == 'ッ') && nameTextInput.Length > 0)
                {
                    char prevLetter = nameTextInput[nameTextInput.Length - 1];
                    if (letterCollection[prevLetter].ConstanantExists)
                    {
                        sfxPlayer.PlaySound("Select_2");
                        nameTextInput = nameTextInput.Substring(0, nameTextInput.Length - 1);
                        nameTextInput += letterCollection[prevLetter].ConstanantLetter;
                        nameTextField.text = nameTextInput + ((nameTextInput.Length < 10) ? nameTextNext : "");
                    }
                }
                else if (letterMenuSlots[currentRow][currentColumn].SelectedChoice(currentLetterOption) == '゛' && nameTextField.text.Length > 0)
                {
                    char prevLetter = nameTextInput[nameTextInput.Length - 1];
                    if (letterCollection[prevLetter].DakuExists)
                    {
                        sfxPlayer.PlaySound("Select_2");
                        nameTextInput = nameTextInput.Substring(0, nameTextInput.Length - 1);
                        nameTextInput += letterCollection[prevLetter].DakuLetter;
                        nameTextField.text = nameTextInput + ((nameTextInput.Length < 10) ? nameTextNext : "");
                    }
                }
                else if (letterMenuSlots[currentRow][currentColumn].SelectedChoice(currentLetterOption) == '゜' && nameTextField.text.Length > 0)
                {
                    char prevLetter = nameTextInput[nameTextInput.Length - 1];
                    if (letterCollection[prevLetter].HandakuExists)
                    {
                        sfxPlayer.PlaySound("Select_2");
                        nameTextInput = nameTextInput.Substring(0, nameTextInput.Length - 1);
                        nameTextInput += letterCollection[prevLetter].HandakuLetter;
                        nameTextField.text = nameTextInput + ((nameTextInput.Length < 10) ? nameTextNext : "");
                    }
                }
                else
                {
                    if (nameTextInput.Length < 10)
                    {
                        sfxPlayer.PlaySound("Select_2");
                        nameTextInput += letterCollection[letterMenuSlots[currentRow][currentColumn].SelectedChoice(currentLetterOption)].Letter;
                        nameTextField.text = nameTextInput + ((nameTextInput.Length < 10) ? nameTextNext : "");
                    }
                }
            }
        }
        else if (currentSection == 2)
        {
            menuEventManagerInstance.TriggerEvent("ActivateSlot", null);
        }
    }

    public override void closeMenu()
    {
        sfxPlayer.PlaySound("Cancel");
        csPlayerGUI.bufferGUI = 30;
        newNameRootMenu.unlockMenu();
        gameObject.SetActive(false);
    }

    public override void forceCloseMenu()
    {
        resetScrollFrames();
        newNameRootMenu.unlockMenu();
        gameObject.SetActive(false);
    }

    public void ConfirmEntry()
    {
        bool cancel = false;
        string trimmedName = TrimInput(nameTextInput);
        //Check is new name is blank
        if (trimmedName.Length == 0)
        {
            cancel = true;
        }
        //Check if user name already exists
        for (int i = 0; i < gameData.idCounter; i++)
        {
            if (trimmedName == gameData.userNameReference[i].Name)
            {
                if (nameEditMode == NameEditMode.EnterName || (nameEditMode == NameEditMode.EditName && i != newNameRootMenu.SelectedUserId))
                {
                    cancel = true;
                    break;
                }
            }
        }
        if (!cancel)
        {
            sfxPlayer.PlaySound("Confirm");
            csPlayerGUI.bufferGUI = 30;
            newNameRootMenu.unlockMenu();
            if (nameEditMode == NameEditMode.EnterName)
            {
                newNameRootMenu.AddNewUser(trimmedName);
            } else
            {
                newNameRootMenu.EditUser(trimmedName);
            }
            gameObject.SetActive(false);
        }
    }

    public void MoveLetterSelectSlot()
    {
        _letterBoxHighlight.SetActive(true);
        _letterSelectSlot.transform.position = new Vector3(_typeGlyphSection.transform.position.x - 132 + (24 * currentColumn), _typeGlyphSection.transform.position.y + 70 - (35 * currentRow), _letterSelectSlot.transform.position.z);
        _letterBoxHighlight.transform.position = new Vector3(_letterSelectSlot.transform.position.x, _letterSelectSlot.transform.position.y, _letterBoxHighlight.transform.position.z);
        //boxHighlight.size = new Vector2(48f, 48f);
    }

    public void ChangeLetterOption(int _LO)
    {
        for (int i = 0; i < letterOptionSlots.Length; i++)
        {
            (letterOptionSlots[i] as EventMenuSlot).deselectUpdate();
        }
        for (int i = 0; i < _letterOptionShiftStyles.Length; i++)
        {
            letterOptionShiftStyles[_letterOptionShiftStyles[i]._letterOption].SetActive(false);
        }
        currentTypeSection.SetActive(false);
        currentLetterOption = (LetterOption)_LO;
        if (currentLetterOption == LetterOption.RomaniUpper && !capsOn)
        {
            currentLetterOption = LetterOption.RomaniLower;
        }
        if (currentLetterOption == LetterOption.JapaneseHiragana && !hiraganaOn)
        {
            currentLetterOption = LetterOption.JapaneseKatakana;
        }
        currentTypeSection = typeSectionSet[(int)currentLetterOption];
        switch (currentLetterOption)
        {
            case LetterOption.RomaniUpper:
            case LetterOption.RomaniLower:
                (letterOptionSlots[0] as EventMenuSlot).selectUpdate();
                letterOptionShiftStyles[LetterOption.RomaniUpper].SetActive(true);
                break;
            case LetterOption.JapaneseHiragana:
                (letterOptionSlots[1] as EventMenuSlot).selectUpdate();
                letterOptionShiftStyles[LetterOption.JapaneseKatakana].SetActive(true);
                break;
            case LetterOption.JapaneseKatakana:
                (letterOptionSlots[1] as EventMenuSlot).selectUpdate();
                letterOptionShiftStyles[LetterOption.JapaneseHiragana].SetActive(true);
                break;
            case LetterOption.Symbol:
                (letterOptionSlots[2] as EventMenuSlot).selectUpdate();
                break;
            case LetterOption.Latin:
                (letterOptionSlots[3] as EventMenuSlot).selectUpdate();
                break;
            default:
                break;
        }
        currentTypeSection.SetActive(true);
    }

    public void ShiftLetterOption()
    {
        currentTypeSection.SetActive(false);
        int _LO = (int)currentLetterOption;
        int _TLO;
        switch (_LO)
        {
            case 0:
                sfxPlayer.PlaySound("Select_1");
                _TLO = _LO + 1;
                capsOn = false;
                break;
            case 2:
                sfxPlayer.PlaySound("Select_1");
                _TLO = _LO + 1;
                hiraganaOn = false;
                letterOptionShiftStyles[LetterOption.JapaneseHiragana].SetActive(true);
                letterOptionShiftStyles[LetterOption.JapaneseKatakana].SetActive(false);
                break;
            case 1:
                sfxPlayer.PlaySound("Select_1");
                _TLO = _LO - 1;
                capsOn = true;
                break;
            case 3:
                sfxPlayer.PlaySound("Select_1");
                _TLO = _LO - 1;
                hiraganaOn = true;
                letterOptionShiftStyles[LetterOption.JapaneseHiragana].SetActive(false);
                letterOptionShiftStyles[LetterOption.JapaneseKatakana].SetActive(true);
                break;
            default:
                _TLO = _LO;
                break;
        }
        currentLetterOption = (LetterOption)_TLO;
        currentTypeSection = typeSectionSet[(int)currentLetterOption];
        currentTypeSection.SetActive(true);
    }

    public void AddSpace()
    {
        if (nameTextInput.Length < 10)
        {
            sfxPlayer.PlaySound("Select_2");
            nameTextInput += ' ';
            nameTextField.text = nameTextInput + ((nameTextInput.Length < 10) ? nameTextNext : "");
        }
    }

    public void DeleteLetter()
    {
        if (nameTextInput.Length > 0)
        {
            sfxPlayer.PlaySound("Select_2");
            nameTextInput = nameTextInput.Substring(0, nameTextInput.Length-1);
            nameTextField.text = nameTextInput + ((nameTextInput.Length < 10) ? nameTextNext : "");
        }
    }

    public string TrimInput(string name)
    {
        string trimmedName = name;
        //Remove the spaces at the beginning
        int spaces = 0;
        for (int i = 0; i < trimmedName.Length; i++)
        {
            if (trimmedName[i] == ' ')
            {
                spaces++;
            } else
            {
                break;
            }
        }
        if (spaces > 0)
        {
            trimmedName = trimmedName.Substring(spaces);
        }
        //Remove the spaces at the end
        spaces = 0;
        if (trimmedName.Length > 0)
        {
            for (int i = trimmedName.Length - 1; i >= 0; i--)
            {
                if (trimmedName[i] == ' ')
                {
                    spaces++;
                }
                else
                {
                    break;
                }
            }
            if (spaces > 0)
            {
                trimmedName = trimmedName.Substring(0, trimmedName.Length - spaces);
            }
        }
        return trimmedName;
    }

    #region[InitializeLetters()]
    private void InitializeLetters()
    {
        letterCollection = new Dictionary<char, LetterChar>();
        letterCollection.Add('A', new LetterChar('A'));
        letterCollection.Add('B', new LetterChar('B'));
        letterCollection.Add('C', new LetterChar('C'));
        letterCollection.Add('D', new LetterChar('D'));
        letterCollection.Add('E', new LetterChar('E'));
        letterCollection.Add('F', new LetterChar('F'));
        letterCollection.Add('G', new LetterChar('G'));
        letterCollection.Add('H', new LetterChar('H'));
        letterCollection.Add('I', new LetterChar('I'));
        letterCollection.Add('J', new LetterChar('J'));
        letterCollection.Add('K', new LetterChar('K'));
        letterCollection.Add('L', new LetterChar('L'));
        letterCollection.Add('M', new LetterChar('M'));
        letterCollection.Add('N', new LetterChar('N'));
        letterCollection.Add('O', new LetterChar('O'));
        letterCollection.Add('P', new LetterChar('P'));
        letterCollection.Add('Q', new LetterChar('Q'));
        letterCollection.Add('R', new LetterChar('R'));
        letterCollection.Add('S', new LetterChar('S'));
        letterCollection.Add('T', new LetterChar('T'));
        letterCollection.Add('U', new LetterChar('U'));
        letterCollection.Add('V', new LetterChar('V'));
        letterCollection.Add('W', new LetterChar('W'));
        letterCollection.Add('X', new LetterChar('X'));
        letterCollection.Add('Y', new LetterChar('Y'));
        letterCollection.Add('Z', new LetterChar('Z'));
        letterCollection.Add('a', new LetterChar('a'));
        letterCollection.Add('b', new LetterChar('b'));
        letterCollection.Add('c', new LetterChar('c'));
        letterCollection.Add('d', new LetterChar('d'));
        letterCollection.Add('e', new LetterChar('e'));
        letterCollection.Add('f', new LetterChar('f'));
        letterCollection.Add('g', new LetterChar('g'));
        letterCollection.Add('h', new LetterChar('h'));
        letterCollection.Add('i', new LetterChar('i'));
        letterCollection.Add('j', new LetterChar('j'));
        letterCollection.Add('k', new LetterChar('k'));
        letterCollection.Add('l', new LetterChar('l'));
        letterCollection.Add('m', new LetterChar('m'));
        letterCollection.Add('n', new LetterChar('n'));
        letterCollection.Add('o', new LetterChar('o'));
        letterCollection.Add('p', new LetterChar('p'));
        letterCollection.Add('q', new LetterChar('q'));
        letterCollection.Add('r', new LetterChar('r'));
        letterCollection.Add('s', new LetterChar('s'));
        letterCollection.Add('t', new LetterChar('t'));
        letterCollection.Add('u', new LetterChar('u'));
        letterCollection.Add('v', new LetterChar('v'));
        letterCollection.Add('w', new LetterChar('w'));
        letterCollection.Add('x', new LetterChar('x'));
        letterCollection.Add('y', new LetterChar('y'));
        letterCollection.Add('z', new LetterChar('z'));
        letterCollection.Add('0', new LetterChar('0'));
        letterCollection.Add('1', new LetterChar('1'));
        letterCollection.Add('2', new LetterChar('2'));
        letterCollection.Add('3', new LetterChar('3'));
        letterCollection.Add('4', new LetterChar('4'));
        letterCollection.Add('5', new LetterChar('5'));
        letterCollection.Add('6', new LetterChar('6'));
        letterCollection.Add('7', new LetterChar('7'));
        letterCollection.Add('8', new LetterChar('8'));
        letterCollection.Add('9', new LetterChar('9'));
        letterCollection.Add(' ', new LetterChar(' '));
        letterCollection.Add('.', new LetterChar('.'));
        letterCollection.Add(',', new LetterChar(','));
        letterCollection.Add('/', new LetterChar('/'));
        letterCollection.Add('~', new LetterChar('~'));
        letterCollection.Add('!', new LetterChar('!'));
        letterCollection.Add('?', new LetterChar('?'));
        letterCollection.Add('&', new LetterChar('&'));
        letterCollection.Add('-', new LetterChar('-'));
        letterCollection.Add(':', new LetterChar(':'));
        letterCollection.Add(';', new LetterChar(';'));
        letterCollection.Add('#', new LetterChar('#'));
        letterCollection.Add('*', new LetterChar('*'));
        letterCollection.Add('_', new LetterChar('_'));
        letterCollection.Add('@', new LetterChar('@'));
        letterCollection.Add('^', new LetterChar('^'));
        letterCollection.Add('\'', new LetterChar('\''));
        letterCollection.Add('"', new LetterChar('"'));
        letterCollection.Add('(', new LetterChar('('));
        letterCollection.Add(')', new LetterChar(')'));
        letterCollection.Add('[', new LetterChar('['));
        letterCollection.Add(']', new LetterChar(']'));
        letterCollection.Add('°', new LetterChar('°'));
        letterCollection.Add('|', new LetterChar('|'));
        letterCollection.Add('\\', new LetterChar('\\'));
        letterCollection.Add('`', new LetterChar('`'));
        letterCollection.Add('+', new LetterChar('+'));
        letterCollection.Add('÷', new LetterChar('÷'));
        letterCollection.Add('=', new LetterChar('='));
        letterCollection.Add('%', new LetterChar('%'));
        letterCollection.Add('±', new LetterChar('±'));
        letterCollection.Add('<', new LetterChar('<'));
        letterCollection.Add('>', new LetterChar('>'));
        letterCollection.Add('{', new LetterChar('{'));
        letterCollection.Add('}', new LetterChar('}'));
        letterCollection.Add('●', new LetterChar('●'));
        letterCollection.Add('▲', new LetterChar('▲'));
        letterCollection.Add('¡', new LetterChar('¡'));
        letterCollection.Add('¿', new LetterChar('¿'));
        letterCollection.Add('$', new LetterChar('$'));
        letterCollection.Add('¢', new LetterChar('¢'));
        letterCollection.Add('£', new LetterChar('£'));
        letterCollection.Add('¥', new LetterChar('¥'));
        letterCollection.Add('「', new LetterChar('「'));
        letterCollection.Add('」', new LetterChar('」'));
        letterCollection.Add('◀', new LetterChar('◀'));
        letterCollection.Add('▶', new LetterChar('▶'));
        letterCollection.Add('⇦', new LetterChar('⇦'));
        letterCollection.Add('⇧', new LetterChar('⇧'));
        letterCollection.Add('⇨', new LetterChar('⇨'));
        letterCollection.Add('⇩', new LetterChar('⇩'));
        letterCollection.Add('♂', new LetterChar('♂'));
        letterCollection.Add('♀', new LetterChar('♀'));
        letterCollection.Add('♠', new LetterChar('♠'));
        letterCollection.Add('♣', new LetterChar('♣'));
        letterCollection.Add('♥', new LetterChar('♥'));
        letterCollection.Add('♦', new LetterChar('♦'));
        letterCollection.Add('♪', new LetterChar('♪'));
        letterCollection.Add('♫', new LetterChar('♫'));
        letterCollection.Add('À', new LetterChar('À'));
        letterCollection.Add('à', new LetterChar('à'));
        letterCollection.Add('Ò', new LetterChar('Ò'));
        letterCollection.Add('ò', new LetterChar('ò'));
        letterCollection.Add('È', new LetterChar('È'));
        letterCollection.Add('è', new LetterChar('è'));
        letterCollection.Add('Ì', new LetterChar('Ì'));
        letterCollection.Add('ì', new LetterChar('ì'));
        letterCollection.Add('Ù', new LetterChar('Ù'));
        letterCollection.Add('ù', new LetterChar('ù'));
        letterCollection.Add('Ñ', new LetterChar('Ñ'));
        letterCollection.Add('ñ', new LetterChar('ñ'));
        letterCollection.Add('Á', new LetterChar('Á'));
        letterCollection.Add('á', new LetterChar('á'));
        letterCollection.Add('Ó', new LetterChar('Ó'));
        letterCollection.Add('ó', new LetterChar('ó'));
        letterCollection.Add('É', new LetterChar('É'));
        letterCollection.Add('é', new LetterChar('é'));
        letterCollection.Add('Í', new LetterChar('Í'));
        letterCollection.Add('í', new LetterChar('í'));
        letterCollection.Add('Ú', new LetterChar('Ú'));
        letterCollection.Add('ú', new LetterChar('ú'));
        letterCollection.Add('Ç', new LetterChar('Ç'));
        letterCollection.Add('ç', new LetterChar('ç'));
        letterCollection.Add('Â', new LetterChar('Â'));
        letterCollection.Add('â', new LetterChar('â'));
        letterCollection.Add('Ô', new LetterChar('Ô'));
        letterCollection.Add('ô', new LetterChar('ô'));
        letterCollection.Add('Ê', new LetterChar('Ê'));
        letterCollection.Add('ê', new LetterChar('ê'));
        letterCollection.Add('Î', new LetterChar('Î'));
        letterCollection.Add('î', new LetterChar('î'));
        letterCollection.Add('Û', new LetterChar('Û'));
        letterCollection.Add('û', new LetterChar('û'));
        letterCollection.Add('Þ', new LetterChar('Þ'));
        letterCollection.Add('þ', new LetterChar('þ'));
        letterCollection.Add('Ä', new LetterChar('Ä'));
        letterCollection.Add('ä', new LetterChar('ä'));
        letterCollection.Add('Ö', new LetterChar('Ö'));
        letterCollection.Add('ö', new LetterChar('ö'));
        letterCollection.Add('Ë', new LetterChar('Ë'));
        letterCollection.Add('ë', new LetterChar('ë'));
        letterCollection.Add('Ï', new LetterChar('Ï'));
        letterCollection.Add('ï', new LetterChar('ï'));
        letterCollection.Add('Ü', new LetterChar('Ü'));
        letterCollection.Add('ü', new LetterChar('ü'));
        letterCollection.Add('ß', new LetterChar('ß'));
        letterCollection.Add('Ã', new LetterChar('Ã'));
        letterCollection.Add('ã', new LetterChar('ã'));
        letterCollection.Add('Õ', new LetterChar('Õ'));
        letterCollection.Add('õ', new LetterChar('õ'));
        letterCollection.Add('Æ', new LetterChar('Æ'));
        letterCollection.Add('æ', new LetterChar('æ'));
        letterCollection.Add('ぁ', new LetterChar('ぁ'));
        letterCollection.Add('あ', new LetterChar('あ', LetterFunction.Constanant, 'ぁ'));
        letterCollection.Add('ぃ', new LetterChar('ぃ'));
        letterCollection.Add('い', new LetterChar('い', LetterFunction.Constanant, 'ぃ'));
        letterCollection.Add('ぅ', new LetterChar('ぅ'));
        letterCollection.Add('う', new LetterChar('う', LetterFunction.Constanant, 'ぅ', 'ゔ'));
        letterCollection.Add('ぇ', new LetterChar('ぇ'));
        letterCollection.Add('え', new LetterChar('え', LetterFunction.Constanant, 'ぇ'));
        letterCollection.Add('ぉ', new LetterChar('ぉ'));
        letterCollection.Add('お', new LetterChar('お', LetterFunction.Constanant, 'ぉ'));
        letterCollection.Add('か', new LetterChar('か', LetterFunction.Constanant, 'ゕ', 'が'));
        letterCollection.Add('が', new LetterChar('が'));
        letterCollection.Add('き', new LetterChar('き', LetterFunction.Daku, 'ぎ'));
        letterCollection.Add('ぎ', new LetterChar('ぎ'));
        letterCollection.Add('く', new LetterChar('く', LetterFunction.Daku, 'ぐ'));
        letterCollection.Add('ぐ', new LetterChar('ぐ'));
        letterCollection.Add('け', new LetterChar('け', LetterFunction.Constanant, 'ゖ', 'げ'));
        letterCollection.Add('げ', new LetterChar('げ'));
        letterCollection.Add('こ', new LetterChar('こ', LetterFunction.Daku, 'ご'));
        letterCollection.Add('ご', new LetterChar('ご'));
        letterCollection.Add('さ', new LetterChar('さ', LetterFunction.Daku, 'ざ'));
        letterCollection.Add('ざ', new LetterChar('ざ'));
        letterCollection.Add('し', new LetterChar('し', LetterFunction.Daku, 'じ'));
        letterCollection.Add('じ', new LetterChar('じ'));
        letterCollection.Add('す', new LetterChar('す', LetterFunction.Daku, 'ず'));
        letterCollection.Add('ず', new LetterChar('ず'));
        letterCollection.Add('せ', new LetterChar('せ', LetterFunction.Daku, 'ぜ'));
        letterCollection.Add('ぜ', new LetterChar('ぜ'));
        letterCollection.Add('そ', new LetterChar('そ', LetterFunction.Daku, 'ぞ'));
        letterCollection.Add('ぞ', new LetterChar('ぞ'));
        letterCollection.Add('た', new LetterChar('た', LetterFunction.Daku, 'だ'));
        letterCollection.Add('だ', new LetterChar('だ'));
        letterCollection.Add('ち', new LetterChar('ち', LetterFunction.Daku, 'ぢ'));
        letterCollection.Add('ぢ', new LetterChar('ぢ'));
        letterCollection.Add('っ', new LetterChar('っ'));
        letterCollection.Add('つ', new LetterChar('つ', LetterFunction.Constanant, 'っ', 'づ'));
        letterCollection.Add('づ', new LetterChar('づ'));
        letterCollection.Add('て', new LetterChar('て', LetterFunction.Daku, 'で'));
        letterCollection.Add('で', new LetterChar('で'));
        letterCollection.Add('と', new LetterChar('と', LetterFunction.Daku, 'ど'));
        letterCollection.Add('ど', new LetterChar('ど'));
        letterCollection.Add('な', new LetterChar('な'));
        letterCollection.Add('に', new LetterChar('に'));
        letterCollection.Add('ぬ', new LetterChar('ぬ'));
        letterCollection.Add('ね', new LetterChar('ね'));
        letterCollection.Add('の', new LetterChar('の'));
        letterCollection.Add('は', new LetterChar('は', LetterFunction.Handaku, 'ば', 'ぱ'));
        letterCollection.Add('ば', new LetterChar('ば'));
        letterCollection.Add('ぱ', new LetterChar('ぱ'));
        letterCollection.Add('ひ', new LetterChar('ひ', LetterFunction.Handaku, 'び', 'ぴ'));
        letterCollection.Add('び', new LetterChar('び'));
        letterCollection.Add('ぴ', new LetterChar('ぴ'));
        letterCollection.Add('ふ', new LetterChar('ふ', LetterFunction.Handaku, 'ぶ', 'ぷ'));
        letterCollection.Add('ぶ', new LetterChar('ぶ'));
        letterCollection.Add('ぷ', new LetterChar('ぷ'));
        letterCollection.Add('へ', new LetterChar('へ', LetterFunction.Handaku, 'べ', 'ぺ'));
        letterCollection.Add('べ', new LetterChar('べ'));
        letterCollection.Add('ぺ', new LetterChar('ぺ'));
        letterCollection.Add('ほ', new LetterChar('ほ', LetterFunction.Handaku, 'ぼ', 'ぽ'));
        letterCollection.Add('ぼ', new LetterChar('ぼ'));
        letterCollection.Add('ぽ', new LetterChar('ぽ'));
        letterCollection.Add('ま', new LetterChar('ま'));
        letterCollection.Add('み', new LetterChar('み'));
        letterCollection.Add('む', new LetterChar('む'));
        letterCollection.Add('め', new LetterChar('め'));
        letterCollection.Add('も', new LetterChar('も'));
        letterCollection.Add('ゃ', new LetterChar('ゃ'));
        letterCollection.Add('や', new LetterChar('や', LetterFunction.Constanant, 'ゃ'));
        letterCollection.Add('ゅ', new LetterChar('ゅ'));
        letterCollection.Add('ゆ', new LetterChar('ゆ', LetterFunction.Constanant, 'ゅ'));
        letterCollection.Add('ょ', new LetterChar('ょ'));
        letterCollection.Add('よ', new LetterChar('よ', LetterFunction.Constanant, 'ょ'));
        letterCollection.Add('ら', new LetterChar('ら'));
        letterCollection.Add('り', new LetterChar('り'));
        letterCollection.Add('る', new LetterChar('る'));
        letterCollection.Add('れ', new LetterChar('れ'));
        letterCollection.Add('ろ', new LetterChar('ろ'));
        letterCollection.Add('ゎ', new LetterChar('ゎ'));
        letterCollection.Add('わ', new LetterChar('わ', LetterFunction.Constanant, 'ゎ'));
        letterCollection.Add('を', new LetterChar('を'));
        letterCollection.Add('ん', new LetterChar('ん'));
        letterCollection.Add('ゔ', new LetterChar('ゔ'));
        letterCollection.Add('ゕ', new LetterChar('ゕ'));
        letterCollection.Add('ゖ', new LetterChar('ゖ'));
        letterCollection.Add('ァ', new LetterChar('ァ'));
        letterCollection.Add('ア', new LetterChar('ア', LetterFunction.Constanant, 'ァ'));
        letterCollection.Add('ィ', new LetterChar('ィ'));
        letterCollection.Add('イ', new LetterChar('イ', LetterFunction.Constanant, 'ィ'));
        letterCollection.Add('ゥ', new LetterChar('ゥ'));
        letterCollection.Add('ウ', new LetterChar('ウ', LetterFunction.Constanant, 'ゥ', 'ヴ'));
        letterCollection.Add('ェ', new LetterChar('ェ'));
        letterCollection.Add('エ', new LetterChar('エ', LetterFunction.Constanant, 'ェ'));
        letterCollection.Add('ォ', new LetterChar('ォ'));
        letterCollection.Add('オ', new LetterChar('オ', LetterFunction.Constanant, 'ォ'));
        letterCollection.Add('カ', new LetterChar('カ', LetterFunction.Constanant, 'ヵ', 'ガ'));
        letterCollection.Add('ガ', new LetterChar('ガ'));
        letterCollection.Add('キ', new LetterChar('キ', LetterFunction.Daku, 'ギ'));
        letterCollection.Add('ギ', new LetterChar('ギ'));
        letterCollection.Add('ク', new LetterChar('ク', LetterFunction.Daku, 'グ'));
        letterCollection.Add('グ', new LetterChar('グ'));
        letterCollection.Add('ケ', new LetterChar('ケ', LetterFunction.Constanant, 'ヶ', 'ゲ'));
        letterCollection.Add('ゲ', new LetterChar('ゲ'));
        letterCollection.Add('コ', new LetterChar('コ', LetterFunction.Daku, 'ゴ'));
        letterCollection.Add('ゴ', new LetterChar('ゴ'));
        letterCollection.Add('サ', new LetterChar('サ', LetterFunction.Daku, 'ザ'));
        letterCollection.Add('ザ', new LetterChar('ザ'));
        letterCollection.Add('シ', new LetterChar('シ', LetterFunction.Daku, 'ジ'));
        letterCollection.Add('ジ', new LetterChar('ジ'));
        letterCollection.Add('ス', new LetterChar('ス', LetterFunction.Daku, 'ズ'));
        letterCollection.Add('ズ', new LetterChar('ズ'));
        letterCollection.Add('セ', new LetterChar('セ', LetterFunction.Daku, 'ゼ'));
        letterCollection.Add('ゼ', new LetterChar('ゼ'));
        letterCollection.Add('ソ', new LetterChar('ソ', LetterFunction.Daku, 'ゾ'));
        letterCollection.Add('ゾ', new LetterChar('ゾ'));
        letterCollection.Add('タ', new LetterChar('タ', LetterFunction.Daku, 'ダ'));
        letterCollection.Add('ダ', new LetterChar('ダ'));
        letterCollection.Add('チ', new LetterChar('チ', LetterFunction.Daku, 'ヂ'));
        letterCollection.Add('ヂ', new LetterChar('ヂ'));
        letterCollection.Add('ッ', new LetterChar('ッ'));
        letterCollection.Add('ツ', new LetterChar('ツ', LetterFunction.Constanant, 'ッ', 'ヅ'));
        letterCollection.Add('ヅ', new LetterChar('ヅ'));
        letterCollection.Add('テ', new LetterChar('テ', LetterFunction.Daku, 'デ'));
        letterCollection.Add('デ', new LetterChar('デ'));
        letterCollection.Add('ト', new LetterChar('ト', LetterFunction.Daku, 'ド'));
        letterCollection.Add('ド', new LetterChar('ド'));
        letterCollection.Add('ナ', new LetterChar('ナ'));
        letterCollection.Add('ニ', new LetterChar('ニ'));
        letterCollection.Add('ヌ', new LetterChar('ヌ'));
        letterCollection.Add('ネ', new LetterChar('ネ'));
        letterCollection.Add('ノ', new LetterChar('ノ'));
        letterCollection.Add('ハ', new LetterChar('ハ', LetterFunction.Handaku, 'バ', 'パ'));
        letterCollection.Add('バ', new LetterChar('バ'));
        letterCollection.Add('パ', new LetterChar('パ'));
        letterCollection.Add('ヒ', new LetterChar('ヒ', LetterFunction.Handaku, 'ビ', 'ピ'));
        letterCollection.Add('ビ', new LetterChar('ビ'));
        letterCollection.Add('ピ', new LetterChar('ピ'));
        letterCollection.Add('フ', new LetterChar('フ', LetterFunction.Handaku, 'ブ', 'プ'));
        letterCollection.Add('ブ', new LetterChar('ブ'));
        letterCollection.Add('プ', new LetterChar('プ'));
        letterCollection.Add('ヘ', new LetterChar('ヘ', LetterFunction.Handaku, 'ベ', 'ペ'));
        letterCollection.Add('ベ', new LetterChar('ベ'));
        letterCollection.Add('ペ', new LetterChar('ペ'));
        letterCollection.Add('ホ', new LetterChar('ホ', LetterFunction.Handaku, 'ボ', 'ポ'));
        letterCollection.Add('ボ', new LetterChar('ボ'));
        letterCollection.Add('ポ', new LetterChar('ポ'));
        letterCollection.Add('マ', new LetterChar('マ'));
        letterCollection.Add('ミ', new LetterChar('ミ'));
        letterCollection.Add('ム', new LetterChar('ム'));
        letterCollection.Add('メ', new LetterChar('メ'));
        letterCollection.Add('モ', new LetterChar('モ'));
        letterCollection.Add('ャ', new LetterChar('ャ'));
        letterCollection.Add('ヤ', new LetterChar('ヤ', LetterFunction.Constanant, 'ャ'));
        letterCollection.Add('ュ', new LetterChar('ュ'));
        letterCollection.Add('ユ', new LetterChar('ユ', LetterFunction.Constanant, 'ュ'));
        letterCollection.Add('ョ', new LetterChar('ョ'));
        letterCollection.Add('ヨ', new LetterChar('ヨ', LetterFunction.Constanant, 'ョ'));
        letterCollection.Add('ラ', new LetterChar('ラ'));
        letterCollection.Add('リ', new LetterChar('リ'));
        letterCollection.Add('ル', new LetterChar('ル'));
        letterCollection.Add('レ', new LetterChar('レ'));
        letterCollection.Add('ロ', new LetterChar('ロ'));
        letterCollection.Add('ヮ', new LetterChar('ヮ'));
        letterCollection.Add('ワ', new LetterChar('ワ', LetterFunction.Constanant, 'ヮ', 'ヷ'));
        letterCollection.Add('ヲ', new LetterChar('ヲ', LetterFunction.Daku, 'ヺ'));
        letterCollection.Add('ン', new LetterChar('ン'));
        letterCollection.Add('ヴ', new LetterChar('ヴ'));
        letterCollection.Add('ヵ', new LetterChar('ヵ'));
        letterCollection.Add('ヶ', new LetterChar('ヶ'));
        letterCollection.Add('ヷ', new LetterChar('ヷ'));
        letterCollection.Add('ヺ', new LetterChar('ヺ'));
        letterCollection.Add('ー', new LetterChar('ー'));
        letterCollection.Add('・', new LetterChar('・'));
        letterCollection.Add('、', new LetterChar('、'));
        letterCollection.Add('。', new LetterChar('。'));
        letterCollection.Add('゛', new LetterChar('゛'));
        letterCollection.Add('゜', new LetterChar('゜'));
    }
    #endregion
}
