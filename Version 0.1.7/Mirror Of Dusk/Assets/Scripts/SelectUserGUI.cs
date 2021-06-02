using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectUserGUI : AbstractMB
{
    private bool isInitialized = false;

    [NonSerialized] public CharacterSelectPlayer csPlayer;
    private Player input;
    private CharacterSelectPlayerGUI rootGUI;
    [SerializeField] private SelectUserGUISlot[] nameObjectButtons;
    [SerializeField] private SelectUserGUISlot[] editObjectButtons;
    private CanvasGroup canvasGroup;
    private float horizAxis = 0f;
    private float vertAxis = 0f;

    public bool selectUserMenuOpen { get; private set; }
    public bool inputEnabled { get; private set; }
    private List<SelectUserGUISlot> currentNameItems;
    private List<SelectUserGUISlot> currentEditItems;

    public SelectUserGUI.SelectingState selectingState { get; set; }
    public enum SelectingState
    {
        Free,
        Busy
    }

    public SelectUserGUI.PhaseState phaseState { get; set; }
    public enum PhaseState
    {
        Name,
        Edit
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

    private int _currentNameSelection;
    private int currentNameSelection
    {
        get
        {
            return this._currentNameSelection;
        }
        set
        {
            this._currentNameSelection = (value + this.currentNameItems.Count) % this.currentNameItems.Count;
            this.UpdateCurrentNameSelection();
        }
    }

    private int _currentEditSelection;
    private int currentEditSelection
    {
        get
        {
            return this._currentEditSelection;
        }
        set
        {
            this._currentEditSelection = (value + this.currentEditItems.Count) % this.currentEditItems.Count;
            this.UpdateCurrentEditSelection();
        }
    }

    protected override void Awake()
    {
        base.Awake();
        this.selectUserMenuOpen = false;
        this.canvasGroup = base.GetComponent<CanvasGroup>();
        this.selectingState = SelectingState.Free;
        this.currentNameItems = new List<SelectUserGUISlot>(this.nameObjectButtons);
        this.currentEditItems = new List<SelectUserGUISlot>(this.editObjectButtons);
    }

    protected void OnEnable()
    {
        if (isInitialized)
        {
            this.horizAxis = 0f;
            this.vertAxis = 0f;
            this.currentNameSelection = 0;
            this.currentEditSelection = 1;

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

    private void UpdateCurrentNameSelection()
    {
        if (OnFadeHighlighterEvent != null)
        {
            OnFadeHighlighterEvent();
        }
        /*else
        {
            for (int i = 0; i < this.currentNameItems.Count; i++)
            {
                this.currentNameItems[i].InstantFadeHighlighter();
            }
        }
        this.currentNameItems[currentNameSelection].SummonHighlighter();*/
    }

    private void UpdateCurrentEditSelection()
    {
        if (OnFadeHighlighterEvent != null)
        {
            OnFadeHighlighterEvent();
        }
        /*else
        {
            for (int i = 0; i < this.currentEditItems.Count; i++)
            {
                this.currentEditItems[i].InstantFadeHighlighter();
            }
        }
        this.currentEditItems[currentEditSelection].SummonHighlighter();*/
    }

    public delegate void FadeHighlighterDelegate();
    public event SelectUserGUI.FadeHighlighterDelegate OnFadeHighlighterEvent;

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
