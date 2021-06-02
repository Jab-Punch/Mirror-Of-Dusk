using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorMenuGUI : AbstractMB
{
    private bool isInitialized = false;

    [NonSerialized] public CharacterSelectPlayer csPlayer;
    private Player input;
    private CharacterSelectPlayerGUI rootGUI;
    private int selectedCharacterId;
    private int storedCharacterColorCode = 0;
    [SerializeField] private ColorMenuGUISlot[] menuObjectButtons;
    [SerializeField] private TextMeshProUGUI colorNameText;
    [SerializeField] private LocalizationHelper colorNameLocalizationHelper;
    private CanvasGroup canvasGroup;
    private float horizAxis = 0f;

    public bool colorMenuOpen { get; private set; }
    public bool inputEnabled { get; private set; }
    private List<ColorMenuGUISlot> currentItems;
    public ColorMenuGUI.SelectingState selectingState { get; set; }
    public enum SelectingState
    {
        Free,
        Busy
    }

    private Vector2 restPosition;
    private Vector2 activePosition;
    private IEnumerator _moveGUIUp;
    private IEnumerator _moveGUIDown;

    public IEnumerator moveGUIUp
    {
        get
        {
            this._moveGUIUp = moveBase_cr(this.restPosition, this.activePosition, true);
            return _moveGUIUp;
        }
    }

    public IEnumerator moveGUIDown
    {
        get
        {
            this._moveGUIDown = moveBase_cr(this.activePosition, this.restPosition, false);
            return _moveGUIDown;
        }
    }

    private float timeSincePress = 0f;
    private int menuFirstPress = 0;
    private float horizontalHoldTime = 0f;

    private int _horizontalSelection;
    private int horizontalSelection
    {
        get
        {
            return this._horizontalSelection;
        }
        set
        {
            this._horizontalSelection = (value + this.currentItems.Count) % this.currentItems.Count;
            this.UpdateHorizontalSelection();
        }
    }

    public delegate void FadeHighlighterDelegate();
    public event ColorMenuGUI.FadeHighlighterDelegate OnFadeHighlighterEvent;
    public delegate void UpdateArrowAnimationDelegate();
    public event ColorMenuGUI.UpdateArrowAnimationDelegate OnUpdateArrowAnimationEvent;
    public delegate void UpdateHorizontalScrollDelegate(int currentHorizontalPos);
    public event ColorMenuGUI.UpdateHorizontalScrollDelegate OnUpdateHorizontalScrollEvent;

    protected override void Awake()
    {
        base.Awake();
        this.colorMenuOpen = false;
        this.canvasGroup = base.GetComponent<CanvasGroup>();
        this.selectingState = SelectingState.Free;
        this.currentItems = new List<ColorMenuGUISlot>(this.menuObjectButtons);
    }

    protected void OnEnable()
    {
        if (isInitialized)
        {
            this.horizAxis = 0f;
            int horizSel = 0;
            for (int i = 0; i < this.currentItems.Count; i++)
            {
                if (this.currentItems[i].button.colorCode == CharacterSelectScene.Current.playerData[(int)this.rootGUI.id].characterColorCode)
                {
                    horizSel = CharacterSelectScene.Current.playerData[(int)this.rootGUI.id].characterColorCode;
                    break;
                }
            }
            this.horizontalSelection = horizSel;
            this.storedCharacterColorCode = this.horizontalSelection;
            int foundCharId = CharacterSelectScene.Current.playerCharacterShards[(int)this.rootGUI.id].GetCharacterId(CharacterSelectScene.Current.playerCharacterShards[(int)this.rootGUI.id].characterSelectedId);
            if (foundCharId != 0)
            {
                this.FindTextLocalizer(CharacterSelectScene.Current.characterIconData.data[foundCharId].normal.selectPalettes[CharacterSelectScene.Current.playerCharacterShards[(int)this.rootGUI.id].characterColorCode].name);
                this.colorNameText.text = ((CharacterSelectScene.Current.playerCharacterShards[(int)this.rootGUI.id].characterColorCode + 1).ToString()) + ": " + this.colorNameText.text;
            }
            this.ResetCoroutine(_moveGUIUp, moveGUIUp);
        }
    }

    public void EnableThisGUI(CharacterSelectPlayer player, int selectedCharacterId)
    {
        if (isInitialized)
        {
            this.csPlayer = player;
            this.input = player.Input;
            this.csPlayer.state = CharacterSelectPlayer.State.Options;
            this.selectedCharacterId = selectedCharacterId;
            this.currentItems = new List<ColorMenuGUISlot>();
            for (int i = 0; i < CharacterSelectScene.Current.characterIconData.data[selectedCharacterId].normal.selectPalettes.Length; i++)
            {
                this.currentItems.Add(this.menuObjectButtons[i]);
                this.currentItems[i].button.colorCode = CharacterSelectScene.Current.characterIconData.data[selectedCharacterId].normal.selectPalettes[i].colorCode;
                this.currentItems[i].button.ColorA.color = CharacterSelectScene.Current.characterIconData.data[selectedCharacterId].normal.selectPalettes[this.currentItems[i].button.colorCode].colorA;
                this.currentItems[i].button.ColorB.color = CharacterSelectScene.Current.characterIconData.data[selectedCharacterId].normal.selectPalettes[this.currentItems[i].button.colorCode].colorB;
                this.currentItems[i].button.ColorC.color = CharacterSelectScene.Current.characterIconData.data[selectedCharacterId].normal.selectPalettes[this.currentItems[i].button.colorCode].colorC;
            }
            this.gameObject.SetActive(true);
        }
    }

    public void DisableThisGUI()
    {
        this.ResetCoroutine(_moveGUIDown, moveGUIDown);
    }

    public void Init(CharacterSelectPlayer player, CharacterSelectPlayerGUI rootGUI)
    {
        this.rootGUI = rootGUI;
        this.restPosition = this.gameObject.transform.localPosition;
        this.activePosition = new Vector2(this.restPosition.x, this.restPosition.y + 460f);
        this.isInitialized = true;
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!this.inputEnabled || CharacterSelectScene.Current.state != CharacterSelectScene.State.Selecting || this.csPlayer == null)
        {
            this.timeSincePress = 0f;
            menuFirstPress = 0;
            horizontalHoldTime = 0;
            return;
        }
        if (this.csPlayer.state != CharacterSelectPlayer.State.Options)
            return;
        timeSincePress -= Time.deltaTime;
        timeSincePress = Mathf.Clamp(timeSincePress, 0f, 1000f);
        if (this.selectingState == SelectingState.Free)
        {
            if (this.GetButtonDown(MirrorOfDuskButton.Pause) || this.GetButtonDown(MirrorOfDuskButton.Cancel))
            {
                AudioManager.Play("cancel1");
                CharacterSelectScene.Current.playerCharacterShards[(int)this.rootGUI.id].UpdateCharacterColor(this.currentItems[this.storedCharacterColorCode].button.colorCode);
                this.DisableThisGUI();
                horizAxis = 0f;
                return;
            }
            if (this.GetButtonDown(MirrorOfDuskButton.Accept))
            {
                AudioManager.Play("confirm1");
                this.DisableThisGUI();
                horizAxis = 0f;
                return;
            }
            int horizontalSelectionCount = 0;
            float tempHorizAxis = this.GetAxis(MirrorOfDuskButton.CursorHorizontal);
            if (tempHorizAxis > 0f && tempHorizAxis >= horizAxis)
            {
                horizontalSelectionCount++;
            }
            if (tempHorizAxis < 0f && tempHorizAxis <= horizAxis)
            {
                horizontalSelectionCount--;
            }
            horizAxis = tempHorizAxis;
            if (horizontalSelectionCount == 0)
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
                /*if (this.GetButton(MirrorOfDuskButton.MenuRight) && this.currentItems[this.verticalSelection].options.Length > 0)
                {
                    this.currentItems[this.verticalSelection].incrementSelection();
                    this.UpdateHorizontalSelection();
                }
                if (this.GetButton(MirrorOfDuskButton.MenuLeft) && this.currentItems[this.verticalSelection].options.Length > 0)
                {
                    this.currentItems[this.verticalSelection].decrementSelection();
                    this.UpdateHorizontalSelection();
                }*/
                if (horizontalSelectionCount != 0)
                {
                    timeSincePress += 0.15f;
                    if (menuFirstPress == 0)
                        timeSincePress += 0.2f;
                    menuFirstPress = 3;
                    horizontalHoldTime = 0f;
                    AudioManager.Play("menu_scroll");
                    this.horizontalSelection += horizontalSelectionCount;
                    CharacterSelectScene.Current.playerCharacterShards[(int)this.rootGUI.id].ShiftCharacterColor(horizontalSelectionCount);
                    int foundCharId = CharacterSelectScene.Current.playerCharacterShards[(int)this.rootGUI.id].GetCharacterId(CharacterSelectScene.Current.playerCharacterShards[(int)this.rootGUI.id].characterSelectedId);
                    if (foundCharId != 0)
                    {
                        this.FindTextLocalizer(CharacterSelectScene.Current.characterIconData.data[foundCharId].normal.selectPalettes[CharacterSelectScene.Current.playerCharacterShards[(int)this.rootGUI.id].characterColorCode].name);
                        this.colorNameText.text = ((CharacterSelectScene.Current.playerCharacterShards[(int)this.rootGUI.id].characterColorCode + 1).ToString()) + ": " + this.colorNameText.text;
                    }
                }
            }
        }
    }

    private void UpdateHorizontalSelection()
    {
        if (OnFadeHighlighterEvent != null)
        {
            OnFadeHighlighterEvent();
        } else
        {
            for (int i = 0; i < this.currentItems.Count; i++)
            {
                this.currentItems[i].InstantFadeHighlighter();
            }
        }
        this.currentItems[horizontalSelection].SummonHighlighter();
        if (OnUpdateHorizontalScrollEvent != null)
        {
            OnUpdateHorizontalScrollEvent(this._horizontalSelection);
        }
    }

    protected bool GetButtonDown(MirrorOfDuskButton button)
    {
        if (this.input.GetButtonDown((int)button))
        {
            return true;
        }
        return false;
    }

    protected float GetAxis(MirrorOfDuskButton button)
    {
        return this.input.GetAxis((int)button);
    }

    private void FindTextLocalizer(string colorText, LocalizationHelper.LocalizationSubtext[] subtext = null)
    {
        if (this.colorNameLocalizationHelper != null)
        {
            TranslationElement translationElement = Localization.Find(colorText);
            if (translationElement != null)
            {
                this.colorNameLocalizationHelper.ApplyTranslation(translationElement, subtext);
                return;
            }
        }
        this.colorNameText.text = colorText;
    }

    private IEnumerator moveBase_cr(Vector2 startPos, Vector2 endPos, bool summon)
    {
        this.inputEnabled = false;
        yield return base.TweenLocalPosition(startPos, endPos, 20f, EaseUtils.EaseType.linear);
        if (summon)
        {
            this.inputEnabled = true;
        }
        else
        {
            this.inputEnabled = false;
            rootGUI.actionState = CharacterSelectPlayerGUI.ActionState.Free;
            this.csPlayer.state = CharacterSelectPlayer.State.Selecting;
            this.csPlayer = null;
            this.gameObject.SetActive(false);
        }
        yield break;
    }

    private void StopThisCoroutine(IEnumerator cr)
    {
        if (cr != null)
            this.StopCoroutine(cr);
    }

    private void ResetCoroutine(IEnumerator stop_cr, IEnumerator start_cr)
    {
        this.StopThisCoroutine(stop_cr);
        this.StartCoroutine(start_cr);
    }
}
