using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class CharacterSelectCursorPlayerController : CharacterSelectCursorSprite
{
    private CharacterSelectPlayer _player;
    public PlayerId id { get; private set; }
    public CharacterSelectCursorInput input { get; private set; }
    public CharacterSelectCursorMotor motor { get; private set; }

    [Header("Components")]
    [SerializeField] public CharacterSelectCursorStatsManager stats;

    [Header("Sprites")]
    [SerializeField] SpriteRenderer cursorBodySprite;
    [SerializeField] SpriteRenderer cursorGlowSprite;
    [SerializeField] SpriteRenderer cursorNumberSprite;
    [SerializeField] SpriteRenderer cursorNumberBurst;
    [SerializeField] GameSpriteAnimator cursorNumberBurstAnimation;
    [NonSerialized] Color currentColor;
    [NonSerialized] Color cpuColor;
    [SerializeField] Sprite[] cursorNumberSet;

    private bool _initialized = false;
    private Vector3 startPosition;

    public CharacterSelectPlayer player
    {
        get { return _player; }
    }

    public Vector2 Velocity
    {
        get { return this.motor.velocity; }
    }

    [Serializable]
    public class InitObject
    {
        public Vector2 position;
        public Color currentColor;
        public Color cpuColor;
        public int holding;

        public InitObject(Vector2 position, Color currentColor, Color cpuColor, int holding)
        {
            this.position = position;
            this.currentColor = currentColor;
            this.cpuColor = cpuColor;
            this.holding = holding;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        base.tag = "Player_Cursor";
        this.input = base.GetComponent<CharacterSelectCursorInput>();
        this.motor = base.GetComponent<CharacterSelectCursorMotor>();
    }

    private void OnEnable()
    {
        if (_initialized)
        {
            base.transform.position = this.startPosition;
            if (CharacterSelectScene.Current.playerModes[(int)id] == CharacterSelectScene.PlayerMode.Player)
            {
                this.cursorBodySprite.color = this.currentColor;
                this.cursorNumberSprite.color = this.currentColor;
                this.cursorNumberBurst.color = this.currentColor;
            } else if (CharacterSelectScene.Current.playerModes[(int)id] == CharacterSelectScene.PlayerMode.CPU)
            {
                this.cursorBodySprite.color = this.cpuColor;
                this.cursorNumberSprite.color = this.cpuColor;
                this.cursorNumberBurst.color = this.cpuColor;
            }
            this.cursorNumberBurstAnimation.Play("Burst");
            this.StartCoroutine(reEnableSprites_cr());
        }
    }

    public void DisableThisCursor()
    {
        this.StartCoroutine(reDisableSprites_cr());
    }

    public static CharacterSelectCursorPlayerController Create(CharacterSelectPlayer csPlayer, CharacterSelectCursorPlayerController.InitObject init)
    {
        CharacterSelectCursorPlayerController characterSelectCursorPlayerController = UnityEngine.Object.Instantiate<CharacterSelectCursorPlayerController>
            (CharacterSelectScene.Current.characterSelectCursorPlayerController);
        characterSelectCursorPlayerController.Init(csPlayer, init);
        return characterSelectCursorPlayerController;
    }

    private void Init(CharacterSelectPlayer player, CharacterSelectCursorPlayerController.InitObject init)
    {
        this._player = player;
        base.gameObject.name = "Cursor_" + _player.playerId.ToString();
        this.id = _player.playerId;
        this.ResetLayers((int)this.id);
        this.input.Init(this.id);
        this.startPosition = init.position;
        base.transform.position = this.startPosition;
        this.currentColor = init.currentColor;
        this.cpuColor = init.cpuColor;
        this.cursorBodySprite.color = this.currentColor;
        this.cursorNumberSprite.color = this.currentColor;
        this.cursorNumberBurst.color = this.currentColor;
        this.cursorNumberSprite.sprite = this.cursorNumberSet[(int)this.id];
        this.stats.Holding = init.holding;
        _initialized = true;
        this.gameObject.SetActive(false);
    }

    private IEnumerator reEnableSprites_cr()
    {
        cursorBodySprite.enabled = false;
        cursorGlowSprite.enabled = false;
        cursorNumberSprite.enabled = false;
        yield return null;
        int wFrame = 6;
        while (wFrame > 0)
        {
            wFrame--;
            yield return null;
        }
        cursorBodySprite.enabled = true;
        cursorGlowSprite.enabled = true;
        cursorNumberSprite.enabled = true;
        yield break;
    }

    private IEnumerator reDisableSprites_cr()
    {
        this.cursorNumberBurstAnimation.Play("Burst");
        yield return null;
        int wFrame = 6;
        while (wFrame > 0)
        {
            wFrame--;
            yield return null;
        }
        cursorBodySprite.enabled = false;
        cursorGlowSprite.enabled = false;
        cursorNumberSprite.enabled = false;
        wFrame = 10;
        while (wFrame > 0)
        {
            wFrame--;
            yield return null;
        }
        this.gameObject.SetActive(false);
        yield break;
    }

    public void ChangeModeColor()
    {
        if (CharacterSelectScene.Current.playerModes[(int)id] == CharacterSelectScene.PlayerMode.Player)
        {
            this.cursorBodySprite.color = this.currentColor;
            this.cursorNumberSprite.color = this.currentColor;
            this.cursorNumberBurst.color = this.currentColor;
        }
        else if (CharacterSelectScene.Current.playerModes[(int)id] == CharacterSelectScene.PlayerMode.CPU)
        {
            this.cursorBodySprite.color = this.cpuColor;
            this.cursorNumberSprite.color = this.cpuColor;
            this.cursorNumberBurst.color = this.cpuColor;
        }
    }

    public static bool CanMove()
    {
        return CharacterSelectScene.Current.state == CharacterSelectScene.State.Selecting;
    }
}
