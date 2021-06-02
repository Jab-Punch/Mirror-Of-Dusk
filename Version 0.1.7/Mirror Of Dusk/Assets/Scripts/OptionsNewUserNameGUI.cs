using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsNewUserNameGUI : AbstractMB
{
    private bool isInitialized = false;
    public bool isOpen { get; private set; }
    public bool inputEnabled { get; private set; }
    private MirrorOfDuskInput.AnyPlayerInput input;
    private CanvasGroup canvasGroup;
    private OptionsGUI rootOptions;

    private string nameTextInput;
    private string nameTextNext;
    [SerializeField] private TextMeshProUGUI nameTextField;

    private NameEditMode nameEditMode = NameEditMode.EnterName;
    public OptionsNewUserNameGUISlotSections[] slotSections;
    private int _currentSection;
    private int _currentColumn;
    private int _currentRow;
    private LetterOption currentLetterOption;
    private bool capsOn = true;
    private bool hiraganaOn = true;
    private Dictionary<int, Dictionary<int, LetterSet>> letterMenuSlots;
    private Dictionary<char, LetterChar> letterCollection;
    [SerializeField] private LetterOptionShiftStyles[] _letterOptionShiftStyles;
    private Dictionary<LetterOption, GameObject> letterOptionShiftStyles;
    [SerializeField] private GameObject[] typeSectionSet;
    private GameObject currentTypeSection = null;

    private float timeSincePress = 0f;
    private int menuFirstPress = 0;
    private float horizontalHoldTime = 0f;

    private int currentSection
    {
        get
        {
            return this._currentSection;
        }
        set
        {
            int num = value;
            if (num > _currentSection)
            {
                this._currentRow = 0;
                if (_currentSection < 2)
                {
                    if (_currentSection < 1)
                    {
                        switch (_currentColumn)
                        {
                            case 0:
                                _currentColumn = 0;
                                break;
                            case 1:
                                _currentColumn = 4;
                                break;
                            case 2:
                                _currentColumn = 7;
                                break;
                            case 3:
                                _currentColumn = 11;
                                break;
                            default:
                                _currentColumn = 0;
                                break;
                        }
                    } else
                    {
                        if (_currentColumn >= 0 && _currentColumn <= 1)
                        {
                            _currentColumn = 0;
                        }
                        else if (_currentColumn >= 2 && _currentColumn <= 4)
                        {
                            _currentColumn = 1;
                        }
                        else if (_currentColumn >= 5 && _currentColumn <= 6)
                        {
                            _currentColumn = 2;
                        }
                        else if (_currentColumn >= 7 && _currentColumn <= 9)
                        {
                            _currentColumn = 3;
                        }
                        else if (_currentColumn >= 10 && _currentColumn <= 11)
                        {
                            _currentColumn = 4;
                        }
                    }
                }
                else
                {
                    if (_currentColumn > 1)
                    {
                        _currentColumn -= 1;
                    }
                }
            } else if (num < _currentSection)
            {
                this._currentRow = this.slotSections[(num + this.slotSections.Length) % this.slotSections.Length].slots.Length - 1;
                if (_currentSection < 2)
                {
                    if (_currentSection < 1)
                    {
                        if (_currentColumn > 1)
                        {
                            _currentColumn += 1;
                        }
                    } else
                    {
                        if (_currentColumn >= 0 && _currentColumn <= 2)
                        {
                            _currentColumn = 0;
                        }
                        else if (_currentColumn >= 3 && _currentColumn <= 5)
                        {
                            _currentColumn = 1;
                        }
                        else if (_currentColumn >= 6 && _currentColumn <= 8)
                        {
                            _currentColumn = 2;
                        }
                        else if (_currentColumn >= 9 && _currentColumn <= 11)
                        {
                            _currentColumn = 3;
                        }
                    }
                } else
                {
                    switch (_currentColumn)
                    {
                        case 0:
                            _currentColumn = 0;
                            break;
                        case 1:
                            _currentColumn = 3;
                            break;
                        case 2:
                            _currentColumn = 5;
                            break;
                        case 3:
                            _currentColumn = 8;
                            break;
                        case 4:
                            _currentColumn = 11;
                            break;
                        default:
                            _currentColumn = 0;
                            break;
                    }
                }
            }
            this._currentSection = (value + this.slotSections.Length) % this.slotSections.Length;
            this.UpdateSelection();
        }
    }

    private int currentColumn
    {
        get
        {
            return this._currentColumn;
        }
        set
        {
            _currentColumn = (value + this.slotSections[_currentSection].slots[_currentRow].slot.Length) % this.slotSections[_currentSection].slots[_currentRow].slot.Length;
            this.UpdateSelection();
        }
    }

    private int currentRow
    {
        get
        {
            return this._currentRow;
        }
        set
        {
            this._currentRow = (value + this.slotSections[_currentSection].slots.Length) % this.slotSections[_currentSection].slots.Length;
            this.UpdateSelection();
        }
    }

    public delegate void FadeNameHighlighterDelegate();
    public event OptionsNewUserNameGUI.FadeNameHighlighterDelegate OnFadeNameHighlighterEvent;
    public delegate void UpdateNameHighlighterAnimationDelegate();
    public event OptionsNewUserNameGUI.UpdateNameHighlighterAnimationDelegate OnUpdateNameHighlighterAnimationEvent;

    [Serializable]
    public class OptionsNewUserNameGUISlotSections
    {
        public OptionsNewUserNameGUISlots[] slots;

        [Serializable]
        public class OptionsNewUserNameGUISlots
        {
            [SerializeField] public OptionsNewUserNameGUISlot[] slot;
        }
    }

    public enum NameEditMode
    {
        EnterName,
        EditName
    }

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
            }
            else if (_letterFunction == LetterFunction.Daku || _letterFunction == LetterFunction.Handaku)
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
            get
            {
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
            get
            {
                if (_dakuLetter != '\0')
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

    [Serializable]
    public class LetterOptionShiftStyles
    {
        public LetterOption _letterOption;
        public GameObject _gm;
    }

    protected override void Awake()
    {
        base.Awake();
        this.canvasGroup = base.GetComponent<CanvasGroup>();
        this.InitializeLetterSlots();
        letterOptionShiftStyles = new Dictionary<LetterOption, GameObject>();
        for (int i = 0; i < _letterOptionShiftStyles.Length; i++)
        {
            letterOptionShiftStyles.Add(_letterOptionShiftStyles[i]._letterOption, _letterOptionShiftStyles[i]._gm);
            letterOptionShiftStyles[_letterOptionShiftStyles[i]._letterOption].SetActive(false);
        }

        nameTextNext = "<color=red>_</color>";
    }

    private void OnEnable()
    {
        if (!this.isInitialized)
        {
            return;
        }
        if (nameEditMode == NameEditMode.EnterName)
        {
            nameTextInput = "";
            nameTextField.text = nameTextInput + nameTextNext;
        }
        else if (nameEditMode == NameEditMode.EditName)
        {
            nameTextInput = UserConfigDataManager.availableUserProfiles[SettingsData.Data.currentUserConfigProfile].userProfileName;
            nameTextField.text = nameTextInput + ((nameTextInput.Length < 10) ? nameTextNext : "");
        }
        this._currentSection = 1;
        this._currentColumn = 0;
        this._currentRow = 0;
        this.currentLetterOption = LetterOption.RomaniUpper;

        for (int i = 0; i < _letterOptionShiftStyles.Length; i++)
        {
            letterOptionShiftStyles[_letterOptionShiftStyles[i]._letterOption].SetActive(false);
        }
        switch (currentLetterOption)
        {
            case LetterOption.RomaniUpper:
            case LetterOption.RomaniLower:
                letterOptionShiftStyles[LetterOption.RomaniUpper].SetActive(true);
                break;
            case LetterOption.JapaneseHiragana:
                letterOptionShiftStyles[LetterOption.JapaneseKatakana].SetActive(true);
                break;
            case LetterOption.JapaneseKatakana:
                letterOptionShiftStyles[LetterOption.JapaneseHiragana].SetActive(true);
                break;
        }
        for (int i = 0; i < typeSectionSet.Length; i++)
        {
            typeSectionSet[i].SetActive(false);
        }
        currentTypeSection = typeSectionSet[(int)currentLetterOption];
        typeSectionSet[(int)currentLetterOption].SetActive(true);
        this.ShowScreen(this.canvasGroup);
    }

    private void OnDisable()
    {
        this.isOpen = false;
        if (!this.isInitialized)
        {
            this.isInitialized = true;
            return;
        }
    }

    private void OnDestroy()
    {
        this.isInitialized = false;
    }
    
    public void Init(OptionsGUI rootOp, int mode)
    {
        this.input = new MirrorOfDuskInput.AnyPlayerInput(false);
        this.rootOptions = rootOp;
        this.nameEditMode = (NameEditMode)mode;
        this.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!this.inputEnabled || MainMenuScene.Current.state != MainMenuScene.State.OptionsSelecting)
        {
            this.timeSincePress = 0f;
            menuFirstPress = 0;
            horizontalHoldTime = 0;
            return;
        }
        timeSincePress -= Time.deltaTime;
        timeSincePress = Mathf.Clamp(timeSincePress, 0f, 1000f);
        if (this.GetButtonDown(MirrorOfDuskButton.Pause))
        {
            MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
            this.MenuCancelSound();
            this.HideScreen(this.canvasGroup, OptionsGUI.State.UserConfig);
            return;
        }
        if (this.GetButtonDown(MirrorOfDuskButton.Cancel))
        {
            if (nameTextInput.Length > 0)
            {
                AudioManager.Play("select2");
                nameTextInput = nameTextInput.Substring(0, nameTextInput.Length - 1);
                nameTextField.text = nameTextInput + ((nameTextInput.Length < 10) ? nameTextNext : "");
                return;
            } else
            {
                MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                this.MenuCancelSound();
                this.HideScreen(this.canvasGroup, OptionsGUI.State.UserConfig);
                return;
            }
        }
        if (this.GetButtonDown(MirrorOfDuskButton.Accept))
        {
            if (currentSection == 0)
            {
                switch (currentColumn)
                {
                    case 0:
                        ChangeLetterOption(0);
                        break;
                    case 1:
                        ChangeLetterOption(2);
                        break;
                    case 2:
                        ChangeLetterOption(4);
                        break;
                    case 3:
                        ChangeLetterOption(5);
                        break;
                }
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
                            AudioManager.Play("select2");
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
                            AudioManager.Play("select2");
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
                            AudioManager.Play("select2");
                            nameTextInput = nameTextInput.Substring(0, nameTextInput.Length - 1);
                            nameTextInput += letterCollection[prevLetter].HandakuLetter;
                            nameTextField.text = nameTextInput + ((nameTextInput.Length < 10) ? nameTextNext : "");
                        }
                    }
                    else
                    {
                        if (nameTextInput.Length < 10)
                        {
                            AudioManager.Play("select2");
                            nameTextInput += letterCollection[letterMenuSlots[currentRow][currentColumn].SelectedChoice(currentLetterOption)].Letter;
                            nameTextField.text = nameTextInput + ((nameTextInput.Length < 10) ? nameTextNext : "");
                        }
                    }
                }
            } else if (currentSection == 2)
            {
                switch (currentColumn)
                {
                    case 0:
                        ShiftLetterOption();
                        break;
                    case 1:
                        AddSpace();
                        break;
                    case 2:
                        DeleteLetter();
                        break;
                    case 3:
                        MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                        this.MenuCancelSound();
                        this.HideScreen(this.canvasGroup, OptionsGUI.State.UserConfig);
                        break;
                    case 4:
                        //this.MenuSelectSound();
                        if (this.ConfirmEntry())
                        {
                            MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
                            OptionsGUI.Current.UpdateUserConfigProfileSelection();
                            this.HideScreen(this.canvasGroup, OptionsGUI.State.UserConfig);
                        } else
                        {
                            this.MenuCancelSound();
                        }
                        break;
                }
            }
            return;
        }
        if (this.GetButtonDown(MirrorOfDuskButton.ScrollLeft))
        {
            switch (currentLetterOption)
            {
                case LetterOption.RomaniUpper:
                case LetterOption.RomaniLower:
                    ChangeLetterOption(5);
                    break;
                case LetterOption.JapaneseHiragana:
                case LetterOption.JapaneseKatakana:
                    ChangeLetterOption(0);
                    break;
                case LetterOption.Symbol:
                    ChangeLetterOption(2);
                    break;
                case LetterOption.Latin:
                    ChangeLetterOption(4);
                    break;
            }
            return;
        }
        if (this.GetButtonDown(MirrorOfDuskButton.ScrollRight))
        {
            switch (currentLetterOption)
            {
                case LetterOption.RomaniUpper:
                case LetterOption.RomaniLower:
                    ChangeLetterOption(2);
                    break;
                case LetterOption.JapaneseHiragana:
                case LetterOption.JapaneseKatakana:
                    ChangeLetterOption(4);
                    break;
                case LetterOption.Symbol:
                    ChangeLetterOption(5);
                    break;
                case LetterOption.Latin:
                    ChangeLetterOption(0);
                    break;
            }
            return;
        }
        int horizontalSelectionCount = 0;
        int verticalSelectionCount = 0;
        if (this.GetButton(MirrorOfDuskButton.MenuRight))
        {
            horizontalSelectionCount++;
        }
        if (this.GetButton(MirrorOfDuskButton.MenuLeft))
        {
            horizontalSelectionCount--;
        }
        if (this.GetButton(MirrorOfDuskButton.MenuDown))
        {
            verticalSelectionCount++;
        }
        if (this.GetButton(MirrorOfDuskButton.MenuUp))
        {
            verticalSelectionCount--;
        }
        if (verticalSelectionCount == 0 && horizontalSelectionCount == 0)
        {
            menuFirstPress--;
            menuFirstPress = Mathf.Clamp(menuFirstPress, 0, 10);
            if (menuFirstPress <= 0)
            {
                horizontalHoldTime = 0f;
                this.timeSincePress = 0f;
            }
        }
        if (timeSincePress <= 0f)
        {
            if (verticalSelectionCount != 0)
            {
                timeSincePress += 0.15f;
                if (menuFirstPress == 0)
                    timeSincePress += 0.2f;
                menuFirstPress = 3;
                horizontalHoldTime = 0f;
                AudioManager.Play("menu_scroll");
                if (verticalSelectionCount > 0)
                {
                    if (this._currentRow >= this.slotSections[_currentSection].slots.Length - 1)
                    {
                        this.currentSection += verticalSelectionCount;
                    } else
                    {
                        this.currentRow += verticalSelectionCount;
                    }
                } else if (verticalSelectionCount < 0)
                {
                    if (this._currentRow <= 0)
                    {
                        this.currentSection += verticalSelectionCount;
                    }
                    else
                    {
                        this.currentRow += verticalSelectionCount;
                    }
                }
            }
            else
            {
                if (horizontalSelectionCount != 0)
                {
                    timeSincePress += 0.15f;
                    if (menuFirstPress == 0)
                        timeSincePress += 0.2f;
                    menuFirstPress = 3;
                    this.horizontalHoldTime += Time.deltaTime;
                    AudioManager.Play("menu_scroll");
                    this.currentColumn += horizontalSelectionCount;
                }
            }
        }
    }

    private void UpdateSelection()
    {
        OnFadeNameHighlighterEvent();
        this.slotSections[_currentSection].slots[_currentRow].slot[_currentColumn].UpdateNameHighlighterAnimation();
        /*OnFadeHighlighterEvent();
        this.currentItems[verticalSelection].SummonHighlighter();
        if (OnUpdateVerticalScrollEvent != null)
        {
            OnUpdateVerticalScrollEvent(this._verticalSelection);
        }
        UpdateDetails();
        if (OnUpdateArrowAnimationEvent != null)
            OnUpdateArrowAnimationEvent();
        this.currentItems[verticalSelection].UpdateArrowsVertical();*/
    }

    public void ShowScreen(CanvasGroup canvasGroup)
    {
        base.StartCoroutine(this.showScreen_cr(canvasGroup));
    }

    private IEnumerator showScreen_cr(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0;
        yield return null;
        yield return null;
        this.UpdateSelection();
        //this.verticalSelection = 0;
        //this.currentItems[verticalSelection].button.highlighterCanvasGroup.alpha = 1f;
        float alph = 0;
        while (alph < 1f)
        {
            alph += 0.05f;
            canvasGroup.alpha = alph;
            yield return null;
        }
        this.isOpen = true;
        base.FrameDelayedCallback(new Action(this.Interactable), 1);
        MainMenuScene.Current.state = MainMenuScene.State.OptionsSelecting;
        yield break;
    }

    public void HideScreen(CanvasGroup canvasGroup, OptionsGUI.State state)
    {
        MainMenuScene.Current.state = MainMenuScene.State.OptionsBusy;
        /*if (SettingsData.Data.canAutoSave)
        {
            SettingsData.Save();
            if (PlatformHelper.IsConsole)
            {
                SettingsData.SaveToCloud();
            }
            else
            {
                SettingsData.SaveToCloud();
            }*/
        /*if (this.savePlayerData)
        {
            PlayerData.SaveCurrentFile();
        }*/
        //}
        this.canvasGroup.interactable = false;
        this.canvasGroup.blocksRaycasts = false;
        this.inputEnabled = false;
        base.StartCoroutine(this.hideScreen_cr(canvasGroup, state));
    }

    private IEnumerator hideScreen_cr(CanvasGroup canvasGroup, OptionsGUI.State state)
    {
        float alph = canvasGroup.alpha;
        while (alph > 0f)
        {
            alph -= 0.05f;
            canvasGroup.alpha = alph;
            yield return null;
        }
        MainMenuScene.Current.state = MainMenuScene.State.OptionsSelecting;
        if (this.rootOptions != null)
            this.rootOptions.SetToggle(state);
        this.gameObject.SetActive(false);
        yield break;
    }

    private void Interactable()
    {
        this.canvasGroup.interactable = true;
        this.canvasGroup.blocksRaycasts = true;
        this.inputEnabled = true;
    }

    protected bool GetButtonDown(MirrorOfDuskButton button)
    {
        if (this.input.GetButtonDown(button))
        {
            return true;
        }
        return false;
    }

    protected bool GetButton(MirrorOfDuskButton button)
    {
        return this.input.GetButton(button);
    }

    protected void MenuSelectSound()
    {
        AudioManager.Play("confirm1");
    }

    protected void MenuCancelSound()
    {
        AudioManager.Play("cancel1");
    }

    private void ChangeLetterOption(int _LO)
    {
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
                letterOptionShiftStyles[LetterOption.RomaniUpper].SetActive(true);
                break;
            case LetterOption.JapaneseHiragana:
                letterOptionShiftStyles[LetterOption.JapaneseKatakana].SetActive(true);
                break;
            case LetterOption.JapaneseKatakana:
                letterOptionShiftStyles[LetterOption.JapaneseHiragana].SetActive(true);
                break;
            case LetterOption.Symbol:
                break;
            case LetterOption.Latin:
                break;
            default:
                break;
        }
        currentTypeSection.SetActive(true);
    }

    private void ShiftLetterOption()
    {
        currentTypeSection.SetActive(false);
        int _LO = (int)currentLetterOption;
        int _TLO;
        switch (_LO)
        {
            case 0:
                AudioManager.Play("select1");
                _TLO = _LO + 1;
                capsOn = false;
                break;
            case 2:
                AudioManager.Play("select1");
                _TLO = _LO + 1;
                hiraganaOn = false;
                letterOptionShiftStyles[LetterOption.JapaneseHiragana].SetActive(true);
                letterOptionShiftStyles[LetterOption.JapaneseKatakana].SetActive(false);
                break;
            case 1:
                AudioManager.Play("select1");
                _TLO = _LO - 1;
                capsOn = true;
                break;
            case 3:
                AudioManager.Play("select1");
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

    private void AddSpace()
    {
        if (nameTextInput.Length < 10)
        {
            AudioManager.Play("select2");
            nameTextInput += ' ';
            nameTextField.text = nameTextInput + ((nameTextInput.Length < 10) ? nameTextNext : "");
        }
    }

    private void DeleteLetter()
    {
        if (nameTextInput.Length > 0)
        {
            AudioManager.Play("select2");
            nameTextInput = nameTextInput.Substring(0, nameTextInput.Length - 1);
            nameTextField.text = nameTextInput + ((nameTextInput.Length < 10) ? nameTextNext : "");
        }
    }

    private bool ConfirmEntry()
    {
        string trimmedName = TrimInput(nameTextInput);
        //Check is new name is blank
        if (trimmedName.Length == 0)
        {
            return false;
        }
        //Check if user name already exists
        for (int i = 0; i < UserConfigDataManager.availableUserProfiles.Count; i++)
        {
            if (trimmedName == UserConfigDataManager.availableUserProfiles[i].userProfileName)
            {
                if (nameEditMode == NameEditMode.EnterName || (nameEditMode == NameEditMode.EditName && i != SettingsData.Data.currentUserConfigProfile))
                {
                    return false;
                }
            }
        }
        this.MenuSelectSound();
        if (nameEditMode == NameEditMode.EnterName)
        {
            UserConfigDataManager.AddNewProfile(trimmedName);
        }
        else
        {
            UserConfigDataManager.EditCurrentProfile(trimmedName);
        }
        return true;
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
            }
            else
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

    private void InitializeLetterSlots()
    {
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
