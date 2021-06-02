using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EditHealthGUI : AbstractMB
{
    private bool isInitialized = false;

    [NonSerialized] public CharacterSelectPlayer csPlayer;
    private Player input;
    private CharacterSelectPlayerGUI rootGUI;
    [NonSerialized] public int editedDefaultHealth;
    private int storedDefaultHealth = 0;
    [SerializeField] private EditHealthGUISlot[] menuObjectButtons;
    [SerializeField] private TextMeshProUGUI handicapHealthText;
    [SerializeField] private TextMeshProUGUI defaultHealthText;
    private CanvasGroup canvasGroup;
    private float horizAxis = 0f;
    private float vertAxis = 0f;

    public bool hpMenuOpen { get; private set; }
    public bool inputEnabled { get; private set; }
    private List<EditHealthGUISlot> currentItems;
    public EditHealthGUI.SelectingState selectingState { get; set; }
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

    [NonSerialized] public int hpMaxLimit = 9999;
    [NonSerialized] public int hpMinLimit = 1;

    public delegate void FadeHighlighterDelegate();
    public event EditHealthGUI.FadeHighlighterDelegate OnFadeHighlighterEvent;

    protected override void Awake()
    {
        base.Awake();
        this.hpMenuOpen = false;
        this.canvasGroup = base.GetComponent<CanvasGroup>();
        this.selectingState = SelectingState.Free;
        this.currentItems = new List<EditHealthGUISlot>(this.menuObjectButtons);
    }

    protected void OnEnable()
    {
        if (isInitialized)
        {
            this.horizAxis = 0f;
            this.vertAxis = 0f;
            this.horizontalSelection = 3;
            
            this.ResetCoroutine(_moveGUIUp, moveGUIUp);
        }
    }

    private string NumberToString(int num)
    {
        if (num > 999)
        {
            return num.ToString();
        } else if (num > 99)
        {
            return "0" + num.ToString();
        } else if (num > 9)
        {
            return "00" + num.ToString();
        } else
        {
            return "000" + num.ToString();
        }
    }

    public void EnableThisGUI(CharacterSelectPlayer player, int selectedCharacterId)
    {
        if (isInitialized)
        {
            this.csPlayer = player;
            this.input = player.Input;
            this.csPlayer.state = CharacterSelectPlayer.State.Options;
            this.storedDefaultHealth = CharacterSelectScene.Current.playerData[(int)this.rootGUI.id].playerHealth;
            this.editedDefaultHealth = this.storedDefaultHealth;
            this.defaultHealthText.text = NumberToString(BattleSettingsData.Data.battleModeSettings[CharacterSelectData.Data.CurrentMode].defaultInitialHealth);
            this.handicapHealthText.text = NumberToString(this.editedDefaultHealth);
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
            if (this.GetButtonDown(MirrorOfDuskButton.Pause) || this.GetButtonDown(MirrorOfDuskButton.Cancel))
            {
                AudioManager.Play("cancel1");
                CharacterSelectScene.Current.playerData[(int)this.rootGUI.id].playerHealth = this.storedDefaultHealth;
                this.DisableThisGUI();
                horizAxis = 0f;
                vertAxis = 0f;
                return;
            }
            if (this.GetButtonDown(MirrorOfDuskButton.Accept))
            {
                AudioManager.Play("confirm1");
                CharacterSelectScene.Current.playerData[(int)this.rootGUI.id].playerHealth = this.editedDefaultHealth;
                this.DisableThisGUI();
                horizAxis = 0f;
                vertAxis = 0f;
                return;
            }
            int verticalSelectionCount = 0;
            int horizontalSelectionCount = 0;
            float tempVertAxis = this.GetAxis(MirrorOfDuskButton.CursorVertical);
            float tempHorizAxis = this.GetAxis(MirrorOfDuskButton.CursorHorizontal);
            if (tempVertAxis > 0f && tempVertAxis >= vertAxis)
            {
                verticalSelectionCount++;
            }
            if (tempVertAxis < 0f && tempVertAxis <= vertAxis)
            {
                verticalSelectionCount--;
            }
            vertAxis = tempVertAxis;
            if (tempHorizAxis > 0f && tempHorizAxis >= horizAxis)
            {
                horizontalSelectionCount++;
            }
            if (tempHorizAxis < 0f && tempHorizAxis <= horizAxis)
            {
                horizontalSelectionCount--;
            }
            horizAxis = tempHorizAxis;
            if (verticalSelectionCount == 0 && horizontalSelectionCount == 0)
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
                    AudioManager.Play("menu_scroll");
                    this.horizontalSelection += horizontalSelectionCount;
                }
                else
                {
                    if (verticalSelectionCount != 0)
                    {
                        timeSincePress += 0.15f;
                        if (menuFirstPress == 0)
                            timeSincePress += 0.2f;
                        menuFirstPress = 3;
                        if (verticalSelectionCount > 0)
                        {
                            this.currentItems[this.horizontalSelection].incrementSelection();
                            this.UpdateVerticalSelection();
                        }
                        else if (verticalSelectionCount < 0)
                        {
                            this.currentItems[this.horizontalSelection].decrementSelection();
                            this.UpdateVerticalSelection();
                        }
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
    }

    private void UpdateVerticalSelection()
    {
        this.handicapHealthText.text = NumberToString(this.editedDefaultHealth);
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
}
