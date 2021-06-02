using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSCursor : MonoBehaviour, ICSStar
{
    private CharacterSelectManager characterSelectManager;

    public int cursorId;

    private int highlightedStar = 0;
    private int _currentHighlightedStar = 0;
    public int currentHighlightedStar
    {
        get { return _currentHighlightedStar; }
        set { _currentHighlightedStar = value; }
    }
    private SpriteRenderer burstControlRender;
    private SpriteAnimator burstControlAnimator;
    ActivePlayers activePlayers;
    CSPlayerInput csPlayerInput;
    CSPlayerGUI csPlayerGUI;
    CSCursorStar csCursorStar;
    private CSCursorStar _heldCursorStar = null;
    public CSCursorStar heldCursorStar {
        get { return _heldCursorStar; }
        set { _heldCursorStar = value; }
    }
    AnimateCSShard[] animateCSShard;
    CSSideBase[] csSideBase;

    HitboxCollection _hitboxCollection;
    Hitbox _hitbox;
    HurtboxCollection _hurtboxCollection;

    public bool enableCollisions = false;   //Enable and disable colliding.

    [Header("Sprite Prefabs")]
    [SerializeField] public GameObject cursorBurst;
    [SerializeField] public GameObject cursorGlow;
    [SerializeField] public GameObject cursorNumber;

    void Awake()
    {
        characterSelectManager = CharacterSelectManager.characterSelectManager;
        activePlayers = characterSelectManager.activePlayers.GetComponent<ActivePlayers>();
        _hitboxCollection = HitboxCollection.instance;
        _hurtboxCollection = HurtboxCollection.instance;
        burstControlRender = cursorBurst.GetComponent<SpriteRenderer>();
        burstControlAnimator = cursorBurst.GetComponent<SpriteAnimator>();

        cursorId = activePlayers.assignmentNumberCheck;
        csPlayerInput = characterSelectManager.players[cursorId].GetComponent<CSPlayerInput>();
        csPlayerGUI = characterSelectManager.csPlayerGUI.transform.GetChild(cursorId).GetComponent<CSPlayerGUI>();
        
        animateCSShard = new AnimateCSShard[4];
        for (int i = 0; i < animateCSShard.Length; i++)
        {
            animateCSShard[i] = characterSelectManager.csShards.transform.GetChild(i).GetComponent<AnimateCSShard>();
        }
        csSideBase = new CSSideBase[2];
        csSideBase[0] = characterSelectManager.csRulesBase.GetComponent<CSSideBase>();
        csSideBase[1] = characterSelectManager.csBackBase.GetComponent<CSSideBase>();
    }

    // Use this for initialization
    void Start()
    {
        csCursorStar = characterSelectManager.csCursorStars[cursorId].GetComponent<CSCursorStar>();
        for (int j = 0; j < _hitboxCollection.hitboxes.Count; j++)
        {
            if (gameObject.transform.root.gameObject == _hitboxCollection.hitboxes[j].owner && _hitboxCollection.hitboxes[j].hitbox.hitboxData.name == "CSCursor")
            {
                _hitbox = _hitboxCollection.hitboxes[j].hitbox;
                _hitbox.hitboxData.playerId = cursorId;
                _hitbox.gameObject.SetActive(true);
                break;
            }
        }
        highlightedStar = 0;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = new Vector3(Mathf.Round(gameObject.transform.position.x), Mathf.Round(gameObject.transform.position.y), gameObject.transform.position.z);

        //The following two lines help stabilize the position of the cursor to avoid a blur or distortion of the sprite
        gameObject.transform.position = new Vector3(gameObject.transform.position.x + 1f, gameObject.transform.position.y, gameObject.transform.position.z);
        gameObject.transform.position = new Vector3(gameObject.transform.position.x - 1f, gameObject.transform.position.y, gameObject.transform.position.z);

        if (enableCollisions)
        {
            for (int i = 0; i < _hurtboxCollection.hurtboxes.Count; i++)
            {
                _hurtboxCollection.hurtboxes[i].hurtbox.checkCollision(_hitbox.gameObject, _hitbox, _hitbox.gameObject.transform.position.x, _hitbox.gameObject.transform.position.y);
            }
            if (highlightedStar != currentHighlightedStar)
            {
                currentHighlightedStar = highlightedStar;
                if (currentHighlightedStar != 0 && heldCursorStar != null)
                {
                    animateCSShard[heldCursorStar.cursorId].changeState(3);
                }
            }
            highlightedStar = 0;
        }
    }

    public void setBurstColor(Color clr, bool alph)
    {
        burstControlRender.color = clr;
        if (alph)
        {
            burstControlRender.color = new Color(burstControlRender.color.r, burstControlRender.color.g, burstControlRender.color.b, 1.0f);
        } else
        {
            burstControlRender.color = new Color(burstControlRender.color.r, burstControlRender.color.g, burstControlRender.color.b, 0f);
        }
    }

    public void playBurst()
    {
        burstControlAnimator.gameObject.SetActive(true);
        burstControlRender.color = new Color(burstControlRender.color.r, burstControlRender.color.g, burstControlRender.color.b, 1.0f);
        burstControlAnimator.alterColor(burstControlRender.color);
        burstControlAnimator.Play("Burst", false, 0);
    }

    public void resetCursor()
    {
        csPlayerInput.holdingCursorStar = true;
        highlightedStar = 0;
        currentHighlightedStar = 0;
        csPlayerInput.unSelectCharacters();
        csPlayerGUI.refreshGUIOnce = true;
    }

    public void CSStarDetected(int playerId, int characterCode)
    {
        if (playerId == cursorId)
        {
            highlightedStar = characterCode;
        }
    }
}
