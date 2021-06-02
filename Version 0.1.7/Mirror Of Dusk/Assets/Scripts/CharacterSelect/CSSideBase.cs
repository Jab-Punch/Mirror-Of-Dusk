using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSSideBase : MonoBehaviour, IHurtboxResponder {

    private CharacterSelectManager characterSelectManager;
    private CSPlayerInput[] csPlayers;

    public string mainName;

    public GameObject sideBackground;
    public GameObject sideIcon;
    public GameObject hurtboxSet;
    private Hurtbox _hurtboxSet;

    private SpriteAnimator baseBack;
    private bool found = false;
    public int highlightCount = 0;
    private Vector3 iconPos;
    public string hurtBoxId = "SideBase";

    private void Awake()
    {
        characterSelectManager = CharacterSelectManager.characterSelectManager;
        csPlayers = new CSPlayerInput[4];
        for (int i = 0; i < characterSelectManager.players.Length; i++)
        {
            csPlayers[i] = characterSelectManager.players[i].GetComponent<CSPlayerInput>();
        }
        baseBack = sideBackground.GetComponent<SpriteAnimator>();
        iconPos = new Vector3(sideIcon.transform.position.x, sideIcon.transform.position.y, sideIcon.transform.position.z);
        _hurtboxSet = hurtboxSet.GetComponent<Hurtbox>();
    }

    private void Start()
    {
        _hurtboxSet.useHurtResponder(this);
    }

    // Update is called once per frame
    void Update () {
        //this.gameObject.transform.GetChild(3).gameObject.transform.GetChild(0).GetComponent<Hurtbox>().useResponder(this);
        if (highlightCount > 0)
        {
            if (!found)
            {
                found = true;
                iconPos = new Vector3(sideIcon.transform.position.x, sideIcon.transform.position.y, sideIcon.transform.position.z);
                StartCoroutine("jingleIcon");
            }
            baseBack.Play("On");
        } else
        {
            if (found)
            {
                StopCoroutine("jingleIcon");
                sideIcon.transform.position = iconPos;
                baseBack.Play("Off");
            }
            found = false;
        }
        highlightCount = 0;
    }

    private IEnumerator jingleIcon()
    {
        float moveLimit = 0f;
        float moveNum = -2f;
        while (found)
        {
            if (sideIcon.name.Contains("Rules"))
            {
                sideIcon.transform.position = new Vector3(sideIcon.transform.position.x, sideIcon.transform.position.y + moveNum, sideIcon.transform.position.z);
            }
            if (sideIcon.name.Contains("Back"))
            {
                sideIcon.transform.position = new Vector3(sideIcon.transform.position.x + moveNum, sideIcon.transform.position.y, sideIcon.transform.position.z);
            }
            moveLimit += moveNum;
            if (moveLimit >= 10f || moveLimit <= -10f)
            {
                moveNum *= -1;
            }
            yield return null;
        }
        //sideIcon.transform.position = iconPos;
        yield return null;
    }

    public void collisionDetected(Hitbox hitbox, Hurtbox hurtbox)
    {
        highlightCount++;
        if (sideIcon.name.Contains("Rules"))
        {
            //csPlayers[hitbox.hitboxData.playerId].playerCursorFound = CSPlayerInput.CursorFound.Rules;
            csPlayers[hitbox.hitboxData.playerId].playerCursorFoundFlags |= (CSPlayerInput.CursorFoundFlags)(1 << 2);
        }
        else if (sideIcon.name.Contains("Back"))
        {
            //csPlayers[hitbox.hitboxData.playerId].playerCursorFound = CSPlayerInput.CursorFound.Back;
            csPlayers[hitbox.hitboxData.playerId].playerCursorFoundFlags |= (CSPlayerInput.CursorFoundFlags)(1 << 3);
        }
    }
}
