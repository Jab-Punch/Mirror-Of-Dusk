using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectCursorStar : AbstractCollidableObject
{
    private CharacterSelectPlayer player;
    public PlayerId id { get; private set; }
    [NonSerialized] public int characterSelectedId = -1;
    [SerializeField] private CharacterSelectCursorStarAnimationController animationController;
    public CharacterSelectCursorPlayerController assignedCursor;
    [NonSerialized] public CursorHitboxPriority cursorHitboxPriority = CursorHitboxPriority.CursorStar;

    [Header("Sprites")]
    [SerializeField] SpriteRenderer cursorStarBodySprite;
    [SerializeField] SpriteRenderer cursorStarEdgeSprite;
    [SerializeField] SpriteRenderer cursorStarNumberSprite;
    [SerializeField] GameSpriteAnimator cursorStarBodyAnimator;
    [SerializeField] GameSpriteAnimator cursorStarEdgeAnimator;
    [SerializeField] Sprite[] cursorStarNumberSet;
    [SerializeField] Sprite cursorStarCpuNumber;
    [NonSerialized] Color currentColor;
    [NonSerialized] Color currentEdgeColor;
    [NonSerialized] Color currentLabelColor;
    [NonSerialized] Color cpuColor;
    [NonSerialized] Color cpuEdgeColor;
    [NonSerialized] Color cpuLabelColor;

    private bool held = false;
    private bool hitboxActive = false;
    [NonSerialized] public bool retreatToStart = false;

    private bool _initialized = false;
    private Vector3 startPosition;
    private bool starGrowingCrActive;
    private bool starShrinkingCrActive;
    [NonSerialized] public bool nearStars = false;

    public bool Held
    {
        get { return this.held; }
        set { this.held = value; }
    }

    [Serializable]
    public class InitObject
    {
        public Vector2 position;
        public Color currentColor;
        public Color currentEdgeColor;
        public Color currentLabelColor;
        public Color cpuColor;
        public Color cpuEdgeColor;
        public Color cpuLabelColor;
        public bool held;

        public InitObject(Vector2 position, Color currentColor, Color currentEdgeColor, Color currentLabelColor, Color cpuColor, Color cpuEdgeColor, Color cpuLabelColor, bool held)
        {
            this.position = position;
            this.currentColor = currentColor;
            this.currentEdgeColor = currentEdgeColor;
            this.currentLabelColor = currentLabelColor;
            this.cpuColor = cpuColor;
            this.cpuEdgeColor = cpuEdgeColor;
            this.cpuLabelColor = cpuLabelColor;
            this.held = held;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        this.SetLayer(base.GetComponent<SpriteRenderer>(), 0);
        int sortOrder = -16;
        foreach (SpriteRenderer layer in base.GetComponentsInChildren<SpriteRenderer>())
        {
            this.SetLayer(layer, 0, sortOrder++);
        }
        base.tag = "Player_Cursor";
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (_initialized)
        {
            if (CharacterSelectScene.Current.playerModes[(int)id] == CharacterSelectScene.PlayerMode.Player)
            {
                if (CharacterSelectScene.Current.playerCursors[(int)id].gameObject.activeSelf)
                {
                    base.transform.position = CharacterSelectScene.Current.playerCursors[(int)id].gameObject.transform.position;
                }
                this.nearStars = false;
                this.cursorStarBodySprite.color = this.currentColor;
                this.cursorStarEdgeSprite.color = this.currentEdgeColor;
                this.cursorStarNumberSprite.color = this.currentLabelColor;
            }
            else if (CharacterSelectScene.Current.playerModes[(int)id] == CharacterSelectScene.PlayerMode.CPU)
            {
                base.transform.position = startPosition;
                this.nearStars = true;
                this.cursorStarBodySprite.color = this.cpuColor;
                this.cursorStarEdgeSprite.color = this.cpuEdgeColor;
                this.cursorStarNumberSprite.color = this.cpuLabelColor;
                CharacterSelectScene.Current.playerGUIs[(int)id].DefaultCharacterSelect((int)id, -1);
                this.characterSelectedId = -1;
            }
            this.StartCoroutine(reEnableSprites_cr(held));
            CharacterSelectScene.Current.OnCursorStarSelectEvent += this.OnCursorStarSelect;
            CharacterSelectScene.Current.OnCharacterSelectEvent += this.OnCharacterSelect;
        }
    }

    protected void OnDisable()
    {
        if (_initialized)
        {
            if (CharacterSelectScene.Current != null)
            {
                CharacterSelectScene.Current.OnCursorStarSelectEvent -= this.OnCursorStarSelect;
                CharacterSelectScene.Current.OnCharacterSelectEvent -= this.OnCharacterSelect;
            }
        }
    }

    public void DisableThisCursorStar()
    {
        for (int j = 0; j < CharacterSelectScene.Current.playerCursors.Length; j++)
        {
            if (j == (int)this.id)
                continue;
            if (CharacterSelectScene.Current.playerCursors[j].gameObject.activeSelf)
            {
                if (CharacterSelectScene.Current.playerCursors[j].stats.Holding == (int)this.id)
                {
                    this.OnCursorStarRelease();
                    CharacterSelectScene.Current.playerCursors[j].stats.Holding = -1;
                    break;
                }
            }
        }
        this.StartCoroutine(reDisableSprites_cr());
    }

    protected void SetLayer(SpriteRenderer renderer, int player_id)
    {
        if (renderer == null) return;
        renderer.sortingLayerName = "Foreground_3";
        renderer.sortingOrder = (-16 - (3 * player_id));
    }

    protected void SetLayer(SpriteRenderer renderer, int player_id, int sortOrder)
    {
        if (renderer == null) return;
        renderer.sortingLayerName = "Foreground_3";
        renderer.sortingOrder = (sortOrder - (3 * player_id));
    }

    protected override void Update()
    {
        Vector3 position = base.transform.position;
        base.transform.position = new Vector3(position.x, position.y, position.z);
        base.Update();
        this.MoveCursorStar(this.gameObject);
    }

    public static CharacterSelectCursorStar Create(CharacterSelectPlayer player, CharacterSelectCursorStar.InitObject init)
    {
        CharacterSelectCursorStar characterSelectCursorStar = UnityEngine.Object.Instantiate<CharacterSelectCursorStar>
            (CharacterSelectScene.Current.characterSelectCursorStar);
        characterSelectCursorStar.Init(player, init);
        return characterSelectCursorStar;
    }

    private void Init(CharacterSelectPlayer player, CharacterSelectCursorStar.InitObject init)
    {
        this.player = player;
        base.gameObject.name = "CursorStar_" + player.playerId.ToString();
        this.id = player.playerId;
        this.SetLayer(base.GetComponent<SpriteRenderer>(), (int)this.id);
        int sortOrder = -16;
        foreach (SpriteRenderer layer in base.GetComponentsInChildren<SpriteRenderer>())
        {
            this.SetLayer(layer, (int)this.id, sortOrder++);
        }
        this.startPosition = init.position;
        base.transform.position = startPosition;
        this.currentColor = init.currentColor;
        this.currentEdgeColor = init.currentEdgeColor;
        this.currentLabelColor = init.currentLabelColor;
        this.cpuColor = init.cpuColor;
        this.cpuEdgeColor = init.cpuEdgeColor;
        this.cpuLabelColor = init.cpuLabelColor;
        this.cursorStarBodySprite.color = this.currentColor;
        this.cursorStarEdgeSprite.color = this.currentEdgeColor;
        this.cursorStarNumberSprite.color = this.currentLabelColor;
        this.cursorStarNumberSprite.sprite = this.cursorStarNumberSet[(int)this.id];
        this.held = init.held;
        if (!this.held)
            this.nearStars = true;
        _initialized = true;
        this.gameObject.SetActive(false);
    }

    private IEnumerator reEnableSprites_cr(bool holding)
    {
        cursorStarBodySprite.enabled = false;
        cursorStarEdgeSprite.enabled = false;
        cursorStarNumberSprite.enabled = false;
        if (holding)
        {
            cursorStarBodySprite.enabled = true;
            cursorStarEdgeSprite.enabled = true;
            this.GrowCSCursorStar("Sleep");
            assignedCursor = CharacterSelectScene.Current.playerCursors[(int)this.id];
            held = true;
            assignedCursor.stats.Holding = (int)this.id;
            yield return null;
            this.hitboxActive = true;
        } else
        {
            yield return null;
            int wFrame = 6;
            while (wFrame > 0)
            {
                wFrame--;
                yield return null;
            }
            cursorStarBodySprite.enabled = true;
            cursorStarEdgeSprite.enabled = true;
            this.GrowCSCursorStar("Birth");
            wFrame = 8;
            while (wFrame > 0)
            {
                wFrame--;
                yield return null;
            }
            this.RevealNumber();
            this.hitboxActive = true;
        }
        this.animationController.enabled = true;
        yield break;
    }

    private IEnumerator reDisableSprites_cr()
    {
        animationController.hitboxActive = false;
        if (this.nearStars)
        {
            yield return reShrinkSprites_cr();
        }
        yield return null;
        cursorStarBodySprite.enabled = false;
        cursorStarEdgeSprite.enabled = false;
        cursorStarNumberSprite.enabled = false;
        this.hitboxActive = false;
        this.animationController.enabled = false;
        this.gameObject.SetActive(false);
        yield break;
    }

    private void GrowCSCursorStar(string state)
    {
        cursorStarBodyAnimator.Play(state);
        cursorStarEdgeAnimator.Play(state);
    }

    private void GrowCSCursorStar(string state, int startFrame)
    {
        cursorStarBodyAnimator.Play(state, startFrame);
        cursorStarEdgeAnimator.Play(state, startFrame);
    }

    public IEnumerator reGrowSprites_cr()
    {
        while (starShrinkingCrActive)
        {
            yield return null;
        }
        starGrowingCrActive = true;
        this.GrowCSCursorStar("Birth");
        int wFrame = 8;
        while (wFrame > 0)
        {
            wFrame--;
            yield return null;
        }
        this.RevealNumber();
        yield return new WaitForSeconds(0.1f);
        starGrowingCrActive = false;
        yield break;
    }

    public IEnumerator reShrinkSprites_cr()
    {
        while (starGrowingCrActive)
        {
            yield return null;
        }
        starShrinkingCrActive = true;
        this.GrowCSCursorStar("Fade");
        int wFrame = 4;
        while (wFrame > 0)
        {
            wFrame--;
            yield return null;
        }
        cursorStarNumberSprite.enabled = false;
        yield return new WaitForSeconds(0.1f);
        starShrinkingCrActive = false;
        yield break;
    }

    private void RevealNumber()
    {
        cursorStarNumberSprite.enabled = true;
    }

    public void ChangeModeColor()
    {
        if (CharacterSelectScene.Current.playerModes[(int)id] == CharacterSelectScene.PlayerMode.Player)
        {
            this.cursorStarBodySprite.color = this.currentColor;
            this.cursorStarEdgeSprite.color = this.currentEdgeColor;
            this.cursorStarNumberSprite.color = this.currentLabelColor;
        }
        else if (CharacterSelectScene.Current.playerModes[(int)id] == CharacterSelectScene.PlayerMode.CPU)
        {
            this.cursorStarBodySprite.color = this.cpuColor;
            this.cursorStarEdgeSprite.color = this.cpuEdgeColor;
            this.cursorStarNumberSprite.color = this.cpuLabelColor;
        }
    }

    private void MoveCursorStar(GameObject curPos)
    {
        if (assignedCursor != null)
        {
            retreatToStart = false;
            Vector2 setNewPos = new Vector2(assignedCursor.gameObject.transform.position.x - 40f, assignedCursor.gameObject.transform.position.y + 40f);
            curPos.transform.position = Vector2.Lerp(curPos.transform.position, setNewPos, 0.8f);
            curPos.transform.position = new Vector3(curPos.transform.position.x, curPos.transform.position.y, 0);
            return;
        }
        if (retreatToStart)
        {
            Vector2 setNewPos = startPosition;
            curPos.transform.position = Vector2.Lerp(curPos.transform.position, startPosition, 0.8f);
            curPos.transform.position = new Vector3(curPos.transform.position.x, curPos.transform.position.y, 0);
            if (curPos.transform.position == startPosition)
                retreatToStart = false;
        }
    }

    protected override void OnCollisionCursor(GameObject hit, CollisionPhase phase)
    {
        if (hitboxActive)
        {
            if (phase == CollisionPhase.Stay)
            {
                CharacterSelectCursorStatsManager stats = hit.GetComponent<CharacterSelectCursorStatsManager>();
                if (stats.Holding != (int)this.id)
                {
                    if (stats.baseCursor.id == this.id && !CharacterSelectScene.Current.playerCursorStars[(int)this.id].Held)
                    {
                        stats.playerCursorFoundFlags |= (CharacterSelectCursorStatsManager.CursorFoundFlags)(1 << 0);
                        stats.cStarPickFlags |= (CharacterSelectCursorStatsManager.CStarPickFlags)(1 << (int)id);
                    }
                    else
                    {
                        if (CharacterSelectScene.Current.playerModes[(int)this.id] == CharacterSelectScene.PlayerMode.CPU && !CharacterSelectScene.Current.playerCursorStars[(int)this.id].Held)
                        {
                            stats.playerCursorFoundFlags |= (CharacterSelectCursorStatsManager.CursorFoundFlags)(1 << 0);
                            stats.cStarPickFlags |= (CharacterSelectCursorStatsManager.CStarPickFlags)(1 << (int)id);
                        }
                    }
                }
            }
        }
    }

    public void OnCursorStarSelect(int csId, CharacterSelectCursorPlayerController cursor)
    {
        if (csId == (int)this.id)
        {
            assignedCursor = cursor;
            held = true;
            cursor.stats.Holding = csId;
            CharacterSelectScene.Current.playerData[csId].characterSelectedId = -1;
        }
    }

    public void OnCursorStarRelease()
    {
        assignedCursor = null;
        held = false;
    }

    public void OnCharacterSelect(int csId, int selectedId, bool click)
    {
        if (csId == (int)this.id)
        {
            this.characterSelectedId = selectedId;
        }
    }
}
