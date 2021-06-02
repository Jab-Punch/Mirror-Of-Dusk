using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSCursorStar : MonoBehaviour, IHurtboxResponder
{
    private CharacterSelectManager characterSelectManager;

    public int cursorId;
    private float prevPosX;
    private float prevPosY;
    private float prevPosZ;
    private Vector3 prevPos;
    private Vector3[] cpuPos;
    public bool isHeld { get; private set; }
    public bool cpuOn = false;
    //public bool cpuOnCheck = false;
    public float iconFollowRate = 1.0f;
    private bool _noCStarBelow = true;        //Enable to remove the player selection marker from the cursor when below the character select icons.
    public bool noCStarBelow
    {
        get { return _noCStarBelow; }
        set { _noCStarBelow = value; }
    }
    private bool _animateOn = false;
    public bool animateOn
    {
        get { return _animateOn; }
        set { _animateOn = value; }
    }
    private bool stopFadeStart = true;
    private bool _noSelect = false;
    public bool noSelect
    {
        get { return _noSelect; }
        set { _noSelect = value; }
    }

    GameObject[] parentCursor;
    CSCursor assignedCursor;
    private SpriteAnimator[] animateStars;
    //private SpriteRenderer labelRenderer;
    //HitboxCollection hitboxCollection;

    private SpriteRenderer _sprA;
    private SpriteAnimator _sprA_Anim;
    private SpriteRenderer _sprB;
    private SpriteAnimator _sprB_Anim;
    private SpriteRenderer _sprC;
    private Color[] playerColors;
    private Color[] cpuColors;
    private Sprite[] labelSprites;
    private bool playerColorsAssigned = false;
    private bool cpuColorsAssigned = false;
    private ActivePlayers activePlayers;
    CSPlayerInput csPlayerInput;
    CSPlayerInput[] players;
    CSCursor csCursor;
    public int followID;
    private Hurtbox hurtboxSet;

    [Header("Manager Prefabs")]
    public GameObject _hurtboxSet;

    [Header("Sprite Prefabs")]
    public GameObject cursorStarEdge;
    public GameObject cursorStarBase;
    public GameObject cursorStarLabel;

    void Awake()
    {
        characterSelectManager = CharacterSelectManager.characterSelectManager;
        activePlayers = characterSelectManager.activePlayers.GetComponent<ActivePlayers>();
        
        cursorId = activePlayers.assignmentNumberCheck;
        followID = cursorId;
        csPlayerInput = characterSelectManager.players[cursorId].GetComponent<CSPlayerInput>();
        players = new CSPlayerInput[4];
        for (int i = 0; i < players.Length; i++)
        {
            players[i] = characterSelectManager.players[i].GetComponent<CSPlayerInput>();
        }
        csCursor = characterSelectManager.csCursors[cursorId].GetComponent<CSCursor>();

        animateStars = gameObject.GetComponentsInChildren<SpriteAnimator>();
        labelSprites = new Sprite[2];
        playerColors = new Color[3];
        cpuColors = new Color[3];

        _sprA = cursorStarEdge.GetComponent<SpriteRenderer>();
        _sprA_Anim = _sprA.gameObject.GetComponent<SpriteAnimator>();
        _sprB = cursorStarBase.GetComponent<SpriteRenderer>();
        _sprB_Anim = _sprB.gameObject.GetComponent<SpriteAnimator>();
        _sprC = cursorStarLabel.GetComponent<SpriteRenderer>();

        hurtboxSet = _hurtboxSet.transform.GetChild(0).GetComponent<Hurtbox>();
        parentCursor = new GameObject[4];
        for (int i = 0; i < parentCursor.Length; i++)
        {
            parentCursor[i] = characterSelectManager.csCursors[i];
        }
    }

    // Use this for initialization
    void Start()
    {
        hurtboxSet.useHurtResponder(this);
        _sprC.sprite = labelSprites[0];
        iconFollowRate = 1.0f;
    }

    void OnEnable()
    {
        assignedCursor = csCursor;
        prevPos = gameObject.transform.position;
        cpuPos = new Vector3[]
        {
            new Vector3(-150, 50, prevPos.z),
            new Vector3(-75, 30, prevPos.z),
            new Vector3(75, 30, prevPos.z),
            new Vector3(150, 50, prevPos.z)
        };
        if (activePlayers.playerOn[cursorId])
        {
            isHeld = true;
            csCursor.heldCursorStar = this;
            stopFadeStart = true;
            ChangeCursorStarMode(0);
        }
        else
        {
            isHeld = false;
            csCursor.heldCursorStar = null;
            stopFadeStart = false;
            ChangeCursorStarMode(1);
        }
        checkEnableBelow();
        HitboxEventManager.StartListening("UpdateGUISettings", UpdateGUISettings);
    }

    void OnDisable()
    {
        HitboxEventManager.StopListening("UpdateGUISettings", UpdateGUISettings);
    }

    // Update is called once per frame
    void Update()
    {
        /*if (playerColorsAssigned & !cpuOn)
        {
            _sprA.color = AlterColor(_sprA.color, playerColors[0]);
            _sprB.color = AlterColor(_sprB.color, playerColors[1]);
            _sprC.color = AlterColor(_sprC.color, playerColors[2]);
            _sprA_Anim.sprColor = AlterColor(_sprA_Anim.sprColor, playerColors[0]);
            _sprB_Anim.sprColor = AlterColor(_sprB_Anim.sprColor, playerColors[1]);
        }*/
        if (isHeld)
        {
            moveCursorStar(gameObject, parentCursor[followID].transform.position, -40f, 40f, true);
        }
        else
        {
            if (noSelect)
            {
                moveCursorStar(gameObject, prevPos, 0, 0, true);
            } else
            {
                moveCursorStar(gameObject, prevPos, 0, 0, false);
            }
        }
        checkEnableBelow();
        if (animateOn)
        {
            if (noCStarBelow)
            {
                if (stopFadeStart)
                {
                    for (int i = 0; i < animateStars.Length; i++)
                    {
                        if (!animateStars[i].IsPlaying("Fade") && !animateStars[i].IsPlayingNull())
                        {
                            animateStars[i].Play("Fade", false, 4);
                        }
                    }
                } else
                {
                    for (int i = 0; i < animateStars.Length; i++)
                    {
                        if (!animateStars[i].IsPlaying("Fade") && !animateStars[i].IsPlayingNull())
                        {
                            animateStars[i].Play("Fade", false, 0);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < animateStars.Length; i++)
                {
                    if (!animateStars[i].IsPlaying("Birth") && !animateStars[i].IsPlaying("Born"))
                    {
                        animateStars[i].Play("Birth");
                    }
                }
            }
            stopFadeStart = false;
        }
    }

    private void moveCursorStar(GameObject curPos, Vector3 newPos, float offsetX, float offsetY, bool follow)
    {
        float cPosZ = curPos.transform.position.z;
        if (curPos.transform.position.x == (newPos.x + offsetX) && curPos.transform.position.y == (newPos.y + offsetY))
        {
            iconFollowRate = 1.0f;
        }
        Vector2 setNewPos = new Vector2(newPos.x + offsetX, newPos.y + offsetY);
        if (follow)
        {
            curPos.transform.position = Vector2.Lerp(curPos.transform.position, setNewPos, iconFollowRate);
            curPos.transform.position = new Vector3(curPos.transform.position.x, curPos.transform.position.y, cPosZ);
        }
    }

    public void isHolding(bool B)
    {
        if (isHeld)
        {
            isHeld = false;
        }
        else
        {
            isHeld = true;
            if (B)
            {
                prevPos = gameObject.transform.position;
            }
        }
    }

    public void HideShowLabel(float alpha)
    {
        _sprC.color = new Color(_sprC.color.r, _sprC.color.g, _sprC.color.b, alpha);
    }

    public void destroyCSStar()
    {
        for (int i = 0; i < animateStars.Length; i++)
        {
            if (!animateStars[i].IsPlaying("Fade") && !animateStars[i].IsPlayingNull())
            {
                animateStars[i].Play("Fade", false, 0);
            }
        }
    }

    //Check to remove the selection marker from the cursor if said cursor is below the character select icons margin
    private void checkEnableBelow()
    {
        if ((gameObject.transform.position.y < 0 || gameObject.transform.position.y > 380 || ((gameObject.transform.position.y <= 380 && gameObject.transform.position.y >= 0) && (gameObject.transform.position.x > 750 || gameObject.transform.position.x < -750))) && csPlayerInput.holdingCursorStar)
        {
            noCStarBelow = true;
        }
        else
        {
            noCStarBelow = false;
        }
    }

    public void AssignPlayerColors (Color c1, Color c2, Color c3)
    {
        playerColors[0] = c1;
        playerColors[1] = c2;
        playerColors[2] = c3;
        playerColorsAssigned = true;
    }

    public void AssignCPUColors(Color c1, Color c2, Color c3)
    {
        cpuColors[0] = c1;
        cpuColors[1] = c2;
        cpuColors[2] = c3;
        cpuColorsAssigned = true;
    }

    private Color AlterColor(Color current, Color c)
    {
        return new Color(c.r, c.g, c.b, current.a);
    }

    public void AssignLabels(Sprite _playerLabel, Sprite _cpuLabel)
    {
        labelSprites[0] = _playerLabel;
        labelSprites[1] = _cpuLabel;
    }

    public void ChangeCursorStarMode(int mode)
    {
        if (mode == 0)
        {
            _sprA.color = AlterColor(_sprA.color, playerColors[0]);
            _sprB.color = AlterColor(_sprB.color, playerColors[1]);
            _sprC.color = AlterColor(_sprC.color, playerColors[2]);
            _sprA_Anim.sprColor = AlterColor(_sprA_Anim.sprColor, playerColors[0]);
            _sprB_Anim.sprColor = AlterColor(_sprB_Anim.sprColor, playerColors[1]);
        } else if (mode == 1)
        {
            _sprA.color = AlterColor(_sprA.color, cpuColors[0]);
            _sprB.color = AlterColor(_sprB.color, cpuColors[1]);
            _sprC.color = AlterColor(_sprC.color, cpuColors[2]);
            _sprA_Anim.sprColor = AlterColor(_sprA_Anim.sprColor, cpuColors[0]);
            _sprB_Anim.sprColor = AlterColor(_sprB_Anim.sprColor, cpuColors[1]);
        }
    }

    public void UpdatePosToCPU()
    {
        prevPos = cpuPos[cursorId];
        isHeld = false;
        iconFollowRate = 0.6f;
        StartCoroutine("NoSelectTime");
        csPlayerInput.AutoCharSelection();
    }

    void UpdateGUISettings(System.Object arg0, System.Object arg1, System.Object arg2)
    {
        int gID = (int)arg0;
        int gMode = (int)arg1;
        int pID = (int)arg2;
        if (gID == cursorId && pID != cursorId && !activePlayers.playerOn[gID])
        {
            if (gMode == 1)
            {
                prevPos = cpuPos[gID];
                isHeld = false;
                iconFollowRate = 0.6f;
                StartCoroutine("NoSelectTime");
                csPlayerInput.AutoCharSelection();
            }
        }
        if (gID == cursorId)
        {
            if (gMode == 1)
            {
                cpuOn = true;
            } else
            {
                cpuOn = false;
            }
        }
    }

    private IEnumerator NoSelectTime()
    {
        noSelect = true;
        yield return new WaitForSeconds(1f);
        noSelect = false;
        yield return null;
    }

    public void collisionDetected(Hitbox hitbox, Hurtbox hurtbox)
    {
        if ((hitbox.hitboxData.playerId == cursorId || cpuOn) && !isHeld && !noCStarBelow)
        {
            if (!players[hitbox.hitboxData.playerId].holdingCursorStar)
            {
                players[hitbox.hitboxData.playerId].playerCursorFoundFlags |= (CSPlayerInput.CursorFoundFlags)(1 << 0);
                players[hitbox.hitboxData.playerId].cStarPickFlags |= (CSPlayerInput.CStarPickFlags)(1 << cursorId);
            }
        }
    }
}
