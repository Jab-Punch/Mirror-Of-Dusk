using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EditShardsGUI : AbstractMB
{
    private bool isInitialized = false;

    [NonSerialized] public CharacterSelectPlayer csPlayer;
    private Player input;
    private CharacterSelectPlayerGUI rootGUI;
    [NonSerialized] public int editedDefaultShardsHeld;
    [NonSerialized] public int editedDefaultShardStrength;
    private int storedDefaultShardsHeld = 0;
    private int storedDefaultShardStrength = 0;
    [SerializeField] private EditShardsGUISlot[] menuObjectButtons;
    [SerializeField] private EditShardsGUISlot[] heldObjectButtons;
    [SerializeField] private EditShardsGUISlot[] strengthObjectButtons;
    [SerializeField] private TextMeshProUGUI shardsHeldText;
    [SerializeField] private TextMeshProUGUI shardStrengthText;
    [SerializeField] private TextMeshProUGUI shardsLeftText;
    private CanvasGroup canvasGroup;
    private float horizAxis = 0f;
    private float vertAxis = 0f;

    public bool shardsMenuOpen { get; private set; }
    public bool inputEnabled { get; private set; }
    private List<EditShardsGUISlot> currentItems;
    private List<EditShardsGUISlot> currentHeldItems;
    private List<EditShardsGUISlot> currentStrengthItems;
    public EditShardsGUI.SelectingState selectingState { get; set; }
    public enum SelectingState
    {
        Free,
        Busy
    }

    public EditShardsGUI.PhaseState phaseState { get; set; }
    public enum PhaseState
    {
        Select,
        Held,
        Strength
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

    private int _currentSelection;
    private int currentSelection
    {
        get
        {
            return this._currentSelection;
        }
        set
        {
            this._currentSelection = (value + this.currentItems.Count) % this.currentItems.Count;
            this.UpdateCurrentSelection();
        }
    }

    private int _currentHeldSelection;
    private int currentHeldSelection
    {
        get
        {
            return this._currentHeldSelection;
        }
        set
        {
            this._currentHeldSelection = (value + this.currentHeldItems.Count) % this.currentHeldItems.Count;
            this.UpdateCurrentHeldSelection();
        }
    }

    private int _currentStrengthSelection;
    private int currentStrengthSelection
    {
        get
        {
            return this._currentStrengthSelection;
        }
        set
        {
            this._currentStrengthSelection = (value + this.currentStrengthItems.Count) % this.currentStrengthItems.Count;
            this.UpdateCurrentStrengthSelection();
        }
    }

    public delegate void FadeHighlighterDelegate();
    public event EditShardsGUI.FadeHighlighterDelegate OnFadeHighlighterEvent;
    public event EditShardsGUI.FadeHighlighterDelegate OnFadeArrowsEvent;
    public delegate void UpdateArrowAnimationDelegate();
    public event EditShardsGUI.UpdateArrowAnimationDelegate OnUpdateArrowAnimationEvent;

    protected override void Awake()
    {
        base.Awake();
        this.shardsMenuOpen = false;
        this.canvasGroup = base.GetComponent<CanvasGroup>();
        this.selectingState = SelectingState.Free;
        this.currentItems = new List<EditShardsGUISlot>(this.menuObjectButtons);
        this.currentHeldItems = new List<EditShardsGUISlot>(this.heldObjectButtons);
        this.currentStrengthItems = new List<EditShardsGUISlot>(this.strengthObjectButtons);
    }

    protected void OnEnable()
    {
        if (isInitialized)
        {
            this.horizAxis = 0f;
            this.vertAxis = 0f;
            this.currentSelection = 0;

            this.ResetCoroutine(_moveGUIUp, moveGUIUp);
        }
    }

    private string NumberToString(int num)
    {
        if (num > 999)
        {
            return num.ToString();
        }
        else if (num > 99)
        {
            return "0" + num.ToString();
        }
        else if (num > 9)
        {
            return "00" + num.ToString();
        }
        else
        {
            return "000" + num.ToString();
        }
    }

    private string NumberToLargeString(int num)
    {
        if (num > 9999)
        {
            return num.ToString();
        }
        else if (num > 999)
        {
            return "0" + num.ToString();
        }
        else if (num > 99)
        {
            return "00" + num.ToString();
        }
        else if (num > 9)
        {
            return "000" + num.ToString();
        }
        else
        {
            return "0000" + num.ToString();
        }
    }

    public void EnableThisGUI(CharacterSelectPlayer player, int selectedCharacterId)
    {
        if (isInitialized)
        {
            this.csPlayer = player;
            this.input = player.Input;
            this.csPlayer.state = CharacterSelectPlayer.State.Options;
            this.storedDefaultShardsHeld = CharacterSelectScene.Current.playerData[(int)this.rootGUI.id].playerShardsHeld;
            this.storedDefaultShardStrength = CharacterSelectScene.Current.playerData[(int)this.rootGUI.id].playerShardStrength;
            this.editedDefaultShardsHeld = this.storedDefaultShardsHeld;
            this.editedDefaultShardStrength = this.storedDefaultShardStrength;
            this.shardsHeldText.text = NumberToString(this.editedDefaultShardsHeld);
            this.shardStrengthText.text = NumberToString(this.editedDefaultShardStrength);
            CharacterSelectScene.Current.UpdateShardsLeft(0);
            for (int i = 0; i < currentHeldItems.Count; i++)
            {
                this.currentHeldItems[i].FadeAllArrows();
                this.currentStrengthItems[i].FadeAllArrows();
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
            return;
        }
        if (this.csPlayer.state != CharacterSelectPlayer.State.Options)
            return;
        timeSincePress -= Time.deltaTime;
        timeSincePress = Mathf.Clamp(timeSincePress, 0f, 1000f);
        if (this.selectingState == SelectingState.Free)
        {
            switch (this.phaseState)
            {
                default:
                    if (this.GetButtonDown(MirrorOfDuskButton.Pause) || this.GetButtonDown(MirrorOfDuskButton.Cancel))
                    {
                        AudioManager.Play("cancel1");
                        this.DisableThisGUI();
                        horizAxis = 0f;
                        vertAxis = 0f;
                        return;
                    }
                    if (this.GetButtonDown(MirrorOfDuskButton.Accept))
                    {
                        AudioManager.Play("confirm1");
                        this.currentItems[currentSelection].holdHighlighter = true;
                        this.phaseState = (currentSelection == 0) ? PhaseState.Held : PhaseState.Strength;
                        if (phaseState == PhaseState.Held)
                        {
                            this.currentHeldSelection = 3;
                            this.storedDefaultShardsHeld = CharacterSelectScene.Current.playerData[(int)this.rootGUI.id].playerShardsHeld;
                        } else if (phaseState == PhaseState.Strength)
                        {
                            this.currentStrengthSelection = 3;
                            this.storedDefaultShardStrength = CharacterSelectScene.Current.playerData[(int)this.rootGUI.id].playerShardStrength;
                        }
                        //CharacterSelectScene.Current.playerData[(int)this.rootGUI.id].playerHealth = this.editedDefaultHealth;
                        horizAxis = 0f;
                        vertAxis = 0f;
                        return;
                    }
                    int verticalSelectionCount = 0;
                    float tempVertAxis = this.GetAxis(MirrorOfDuskButton.CursorVertical);
                    if (tempVertAxis > 0f && tempVertAxis >= vertAxis)
                    {
                        verticalSelectionCount++;
                    }
                    if (tempVertAxis < 0f && tempVertAxis <= vertAxis)
                    {
                        verticalSelectionCount--;
                    }
                    vertAxis = tempVertAxis;
                    if (verticalSelectionCount == 0)
                    {
                        menuFirstPress--;
                        menuFirstPress = Mathf.Clamp(menuFirstPress, 0, 10);
                        if (menuFirstPress <= 0)
                        {
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
                            AudioManager.Play("menu_scroll");
                            this.currentSelection += verticalSelectionCount;
                        }
                    }
                    break;
                case PhaseState.Held:
                    if (this.GetButtonDown(MirrorOfDuskButton.Pause))
                    {
                        AudioManager.Play("cancel1");
                        this.currentItems[currentSelection].holdHighlighter = false;
                        this.phaseState = PhaseState.Select;
                        for (int i = 0; i < currentHeldItems.Count; i++)
                        {
                            this.currentHeldItems[i].FadeAllArrows();
                        }
                        CharacterSelectScene.Current.playerData[(int)this.rootGUI.id].playerShardsHeld = this.storedDefaultShardsHeld;
                        this.editedDefaultShardsHeld = this.storedDefaultShardsHeld;
                        this.shardsHeldText.text = NumberToString(this.storedDefaultShardsHeld);
                        this.DisableThisGUI();
                        horizAxis = 0f;
                        vertAxis = 0f;
                        return;
                    }
                    if (this.GetButtonDown(MirrorOfDuskButton.Cancel))
                    {
                        AudioManager.Play("cancel1");
                        this.currentItems[currentSelection].holdHighlighter = false;
                        this.phaseState = PhaseState.Select;
                        for (int i = 0; i < currentHeldItems.Count; i++)
                        {
                            this.currentHeldItems[i].FadeAllArrows();
                        }
                        CharacterSelectScene.Current.playerData[(int)this.rootGUI.id].playerShardsHeld = this.storedDefaultShardsHeld;
                        this.editedDefaultShardsHeld = this.storedDefaultShardsHeld;
                        this.shardsHeldText.text = NumberToString(this.storedDefaultShardsHeld);
                        horizAxis = 0f;
                        vertAxis = 0f;
                        return;
                    }
                    if (this.GetButtonDown(MirrorOfDuskButton.Accept))
                    {
                        AudioManager.Play("confirm1");
                        this.currentItems[currentSelection].holdHighlighter = false;
                        this.phaseState = PhaseState.Select;
                        for (int i = 0; i < currentHeldItems.Count; i++)
                        {
                            this.currentHeldItems[i].FadeAllArrows();
                        }
                        CharacterSelectScene.Current.playerData[(int)this.rootGUI.id].playerShardsHeld = this.editedDefaultShardsHeld;
                        this.storedDefaultShardsHeld = this.editedDefaultShardsHeld;
                        horizAxis = 0f;
                        vertAxis = 0f;
                        return;
                    }
                    int verticalSelectionCountB = 0;
                    int horizontalSelectionCountB = 0;
                    float tempVertAxisB = this.GetAxis(MirrorOfDuskButton.CursorVertical);
                    float tempHorizAxisB = this.GetAxis(MirrorOfDuskButton.CursorHorizontal);
                    if (tempVertAxisB > 0f && tempVertAxisB >= vertAxis)
                    {
                        verticalSelectionCountB++;
                    }
                    if (tempVertAxisB < 0f && tempVertAxisB <= vertAxis)
                    {
                        verticalSelectionCountB--;
                    }
                    vertAxis = tempVertAxisB;
                    if (tempHorizAxisB > 0f && tempHorizAxisB >= horizAxis)
                    {
                        horizontalSelectionCountB++;
                    }
                    if (tempHorizAxisB < 0f && tempHorizAxisB <= horizAxis)
                    {
                        horizontalSelectionCountB--;
                    }
                    horizAxis = tempHorizAxisB;
                    if (verticalSelectionCountB == 0 && horizontalSelectionCountB == 0)
                    {
                        menuFirstPress--;
                        menuFirstPress = Mathf.Clamp(menuFirstPress, 0, 10);
                        if (menuFirstPress <= 0)
                        {
                            this.timeSincePress = 0f;
                        }
                    }
                    if (timeSincePress <= 0f)
                    {
                        if (horizontalSelectionCountB != 0)
                        {
                            timeSincePress += 0.15f;
                            if (menuFirstPress == 0)
                                timeSincePress += 0.2f;
                            menuFirstPress = 3;
                            AudioManager.Play("menu_scroll");
                            this.currentHeldSelection += horizontalSelectionCountB;
                        }
                        else
                        {
                            if (verticalSelectionCountB != 0)
                            {
                                timeSincePress += 0.15f;
                                if (menuFirstPress == 0)
                                    timeSincePress += 0.2f;
                                menuFirstPress = 3;
                                if (verticalSelectionCountB > 0)
                                {
                                    this.currentHeldItems[this.currentHeldSelection].incrementSelection();
                                    this.UpdateCurrentHeldNumber();
                                }
                                else if (verticalSelectionCountB < 0)
                                {
                                    this.currentHeldItems[this.currentHeldSelection].decrementSelection();
                                    this.UpdateCurrentHeldNumber();
                                }
                            }
                        }
                    }
                    break;
                case PhaseState.Strength:
                    if (this.GetButtonDown(MirrorOfDuskButton.Pause))
                    {
                        AudioManager.Play("cancel1");
                        this.currentItems[currentSelection].holdHighlighter = false;
                        this.phaseState = PhaseState.Select;
                        for (int i = 0; i < currentStrengthItems.Count; i++)
                        {
                            this.currentStrengthItems[i].FadeAllArrows();
                        }
                        CharacterSelectScene.Current.playerData[(int)this.rootGUI.id].playerShardStrength = this.storedDefaultShardStrength;
                        this.editedDefaultShardStrength = this.storedDefaultShardStrength;
                        this.shardStrengthText.text = NumberToString(this.storedDefaultShardStrength);
                        this.DisableThisGUI();
                        horizAxis = 0f;
                        vertAxis = 0f;
                        return;
                    }
                    if (this.GetButtonDown(MirrorOfDuskButton.Cancel))
                    {
                        AudioManager.Play("cancel1");
                        this.currentItems[currentSelection].holdHighlighter = false;
                        this.phaseState = PhaseState.Select;
                        for (int i = 0; i < currentStrengthItems.Count; i++)
                        {
                            this.currentStrengthItems[i].FadeAllArrows();
                        }
                        CharacterSelectScene.Current.playerData[(int)this.rootGUI.id].playerShardStrength = this.storedDefaultShardStrength;
                        this.editedDefaultShardStrength = this.storedDefaultShardStrength;
                        this.shardStrengthText.text = NumberToString(this.storedDefaultShardStrength);
                        horizAxis = 0f;
                        vertAxis = 0f;
                        return;
                    }
                    if (this.GetButtonDown(MirrorOfDuskButton.Accept))
                    {
                        AudioManager.Play("confirm1");
                        this.currentItems[currentSelection].holdHighlighter = false;
                        this.phaseState = PhaseState.Select;
                        for (int i = 0; i < currentStrengthItems.Count; i++)
                        {
                            this.currentStrengthItems[i].FadeAllArrows();
                        }
                        CharacterSelectScene.Current.playerData[(int)this.rootGUI.id].playerShardStrength = this.editedDefaultShardStrength;
                        this.storedDefaultShardStrength = this.editedDefaultShardStrength;
                        horizAxis = 0f;
                        vertAxis = 0f;
                        return;
                    }
                    int verticalSelectionCountC = 0;
                    int horizontalSelectionCountC = 0;
                    float tempVertAxisC = this.GetAxis(MirrorOfDuskButton.CursorVertical);
                    float tempHorizAxisC = this.GetAxis(MirrorOfDuskButton.CursorHorizontal);
                    if (tempVertAxisC > 0f && tempVertAxisC >= vertAxis)
                    {
                        verticalSelectionCountC++;
                    }
                    if (tempVertAxisC < 0f && tempVertAxisC <= vertAxis)
                    {
                        verticalSelectionCountC--;
                    }
                    vertAxis = tempVertAxisC;
                    if (tempHorizAxisC > 0f && tempHorizAxisC >= horizAxis)
                    {
                        horizontalSelectionCountC++;
                    }
                    if (tempHorizAxisC < 0f && tempHorizAxisC <= horizAxis)
                    {
                        horizontalSelectionCountC--;
                    }
                    horizAxis = tempHorizAxisC;
                    if (verticalSelectionCountC == 0 && horizontalSelectionCountC == 0)
                    {
                        menuFirstPress--;
                        menuFirstPress = Mathf.Clamp(menuFirstPress, 0, 10);
                        if (menuFirstPress <= 0)
                        {
                            this.timeSincePress = 0f;
                        }
                    }
                    if (timeSincePress <= 0f)
                    {
                        if (horizontalSelectionCountC != 0)
                        {
                            timeSincePress += 0.15f;
                            if (menuFirstPress == 0)
                                timeSincePress += 0.2f;
                            menuFirstPress = 3;
                            AudioManager.Play("menu_scroll");
                            this.currentStrengthSelection += horizontalSelectionCountC;
                        }
                        else
                        {
                            if (verticalSelectionCountC != 0)
                            {
                                timeSincePress += 0.15f;
                                if (menuFirstPress == 0)
                                    timeSincePress += 0.2f;
                                menuFirstPress = 3;
                                if (verticalSelectionCountC > 0)
                                {
                                    this.currentStrengthItems[this.currentStrengthSelection].incrementSelection();
                                    this.UpdateCurrentStrengthNumber();
                                }
                                else if (verticalSelectionCountC < 0)
                                {
                                    this.currentStrengthItems[this.currentStrengthSelection].decrementSelection();
                                    this.UpdateCurrentStrengthNumber();
                                }
                            }
                        }
                    }
                    break;
            }
        }
    }

    private void UpdateCurrentSelection()
    {
        if (OnFadeHighlighterEvent != null)
        {
            OnFadeHighlighterEvent();
        }
        else
        {
            for (int i = 0; i < this.currentItems.Count; i++)
            {
                this.currentItems[i].InstantFadeHighlighter();
            }
        }
        this.currentItems[currentSelection].SummonHighlighter();
    }

    private void UpdateCurrentHeldSelection()
    {
        if (OnFadeArrowsEvent != null)
        {
            OnFadeArrowsEvent();
        }
        this.currentHeldItems[currentHeldSelection].SummonArrows();
    }

    private void UpdateCurrentHeldNumber()
    {
        this.shardsHeldText.text = NumberToString(this.editedDefaultShardsHeld);
    }

    private void UpdateCurrentStrengthSelection()
    {
        if (OnFadeArrowsEvent != null)
        {
            OnFadeArrowsEvent();
        }
        this.currentStrengthItems[currentStrengthSelection].SummonArrows();
    }

    private void UpdateCurrentStrengthNumber()
    {
        this.shardStrengthText.text = NumberToString(this.editedDefaultShardStrength);
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

    public void UpdateShardsLeft(int shards)
    {
        this.shardsLeftText.text = this.NumberToLargeString(shards) + "/" + BattleSettingsData.Data.battleModeSettings[CharacterSelectData.Data.CurrentMode].defaultTotalShards;
    }
}
