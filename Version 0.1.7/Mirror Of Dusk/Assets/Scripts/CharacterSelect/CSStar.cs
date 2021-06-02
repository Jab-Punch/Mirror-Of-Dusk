using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSStar : MonoBehaviour, IHurtboxResponder
{
    private CharacterSelectManager characterSelectManager;

    public int characterCode;
    int highlightCount = 0;
    public bool highlighted { get; private set; }
    public bool highlightStay { get; private set; }

    //List<CSCursor> csCursorList;
    private SpriteAnimator[] animateStars;
    private SpriteMaskAnimator animateMasks;
    private SpriteRenderer nameSprite;
    private CSPlayerInput[] csPlayerInput;
    private CSPlayerData[] csPlayerData;
    private CSCursor[] csCursor;
    private Hurtbox hurtboxSet;
    private ICSStar iCSStarFound;

    [Header("Manager Prefabs")]
    public GameObject nameStar;
    public GameObject _hurtboxSet;


    //private AnimateStars animateStars;

    private void Awake()
    {
        characterSelectManager = CharacterSelectManager.characterSelectManager;

        csPlayerInput = new CSPlayerInput[4];
        csPlayerData = new CSPlayerData[4];
        csCursor = new CSCursor[4];
        for (int i = 0; i < characterSelectManager.players.Length; i++)
        {
            csPlayerInput[i] = characterSelectManager.players[i].GetComponent<CSPlayerInput>();
            csPlayerData[i] = characterSelectManager.players[i].GetComponent<CSPlayerData>();
            csCursor[i] = characterSelectManager.csCursors[i].GetComponent<CSCursor>();
        }
        nameSprite = nameStar.GetComponent<SpriteRenderer>();
        nameStar.SetActive(false);
        animateStars = gameObject.GetComponentsInChildren<SpriteAnimator>();
        animateMasks = gameObject.GetComponentInChildren<SpriteMaskAnimator>();
        hurtboxSet = _hurtboxSet.transform.GetChild(0).GetComponent<Hurtbox>();
    }

    // Use this for initialization
    void Start()
    {
        hurtboxSet.useHurtResponder(this);
        animateMasks.Play("Birth");
    }

    // Update is called once per frame
    void Update()
    {
        /*for (int i = 0; i < csCursorList.Count; i++)
        {
            if (!System.Object.ReferenceEquals(csCursorList[i], null))
            {
                if (gameObject == csCursorList[i].highlightedStar)
                {
                    highlightCount++;
                }
            }
        }*/
        if (highlightCount > 0)
        {
            highlighted = true;
        }
        else
        {
            highlighted = false;
            if (highlightStay)
            {
                for (int i = 0; i < animateStars.Length; i++)
                {
                    animateStars[i].Play("Out");
                }
                animateMasks.Play("Out");
            }
            highlightStay = false;
        }
        if (highlighted && !highlightStay)
        {
            for (int i = 0; i < animateStars.Length; i++)
            {
                animateStars[i].Play("Over");
            }
            animateMasks.Play("Over");
            highlightStay = true;
        }
        highlightCount = 0;
    }

    public void HideShowName()
    {
        nameStar.SetActive(true);
        StartCoroutine("RevealName");
    }

    private IEnumerator RevealName()
    {
        Color nC = nameSprite.color;
        float alph = nC.a;
        while (alph < 1.0f)
        {
            alph += 0.05f;
            if (alph > 1.0f) { alph = 1.0f; }
            nameSprite.color = new Color(nC.r, nC.g, nC.b, alph);
            yield return 0f;
        }
        yield return null;
    }

    public void collisionDetected(Hitbox hitbox, Hurtbox hurtbox)
    {
        int pID = hitbox.hitboxData.playerId;
        if (csPlayerInput[pID].holdingCursorStar && !csCursor[pID].heldCursorStar.noCStarBelow) //If holding the selection marker
        {
            csPlayerData[csCursor[pID].heldCursorStar.cursorId].foundCharacterCode = characterCode;
            csPlayerInput[pID].playerCursorFoundFlags |= (CSPlayerInput.CursorFoundFlags)(1 << 1);
            useICSStar(csCursor[hitbox.hitboxData.playerId]);
            iCSStarFound.CSStarDetected(hitbox.hitboxData.playerId, characterCode);
            highlightCount++;
        }
    }

    public void useICSStar(ICSStar gm)
    {
        iCSStarFound = gm;
    }
}
