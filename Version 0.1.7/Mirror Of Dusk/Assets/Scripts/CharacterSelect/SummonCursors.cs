using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonCursors : MonoBehaviour
{
    private CharacterSelectManager characterSelectManager;

    public GameObject cursorPrefab;
    public GameObject cursorBurstPrefab;
    public GameObject cursorStarPrefab;
    public GameObject playerGUIPrefab;
    public GameObject playerShardPrefab;

    private ActivePlayers activePlayers;
    public CSPlayerInput[] csPlayerInput;
    private GameObject[] csCursorG;
    private GameObject[] csPlayerGUI_go;
    private GameObject[] csShard_go;

    private void Awake()
    {
        characterSelectManager = CharacterSelectManager.characterSelectManager;
        activePlayers = characterSelectManager.activePlayers.GetComponent<ActivePlayers>();
        csPlayerInput = new CSPlayerInput[4];
        for (int i = 0; i < characterSelectManager.players.Length; i++)
        {
            csPlayerInput[i] = characterSelectManager.players[i].GetComponent<CSPlayerInput>();
        }
        csPlayerGUI_go = new GameObject[4];
        csShard_go = new GameObject[4];
        for (int a = 0; a < activePlayers.playerOn.Length; a++)
        {
            csPlayerGUI_go[a] = characterSelectManager.csPlayerGUI.transform.GetChild(a).gameObject;
            csShard_go[a] = characterSelectManager.csShards.transform.GetChild(a).gameObject;
        }
    }

    public IEnumerator beginASummon(int id)
    {
        int frame = 0;
        activePlayers.csCursorG[id].transform.position = new Vector3(activePlayers.playerCursorPos[id + 1].posX, activePlayers.playerCursorPos[id + 1].posY, activePlayers.playerCursorPos[id + 1].posZ);
        if (activePlayers.playerOn[id])
        {
            activePlayers.csCursorB[id].playBurst();
        }
        while (frame < 8)
        {
            frame++;
            yield return null;
        }
        if (activePlayers.playerOn[id])
        {
            activePlayers.csCursorBS[id].enabled = true;
            activePlayers.csCursorBG[id].enabled = true;
            activePlayers.csCursorBSN[id].enabled = true;
            activePlayers.csCursorB[id].enableCollisions = true;
            csPlayerInput[id].enablePlayerInput = true;
        }
        yield return new WaitForSeconds(0.2f);
    }

    public IEnumerator beginSummonCStar(int id, float posX, float posY, float posZ)
    {
        int frame = 0;
        activePlayers.csCursorStar[id].transform.position = new Vector3(posX, posY, posZ);
        activePlayers.csCursorStar[id].SetActive(true);
        activePlayers._csCursorStar[id].HideShowLabel(0);
        while (frame < 8)
        {
            frame++;
            yield return null;
        }
        activePlayers._csCursorStar[id].animateOn = true;
        yield return new WaitForSeconds(0.2f);
    }

    public IEnumerator beginADestroy(int id)
    {
        int frame = 0;
        activePlayers.csCursorB[id].playBurst();
        activePlayers.csCursorB[id].enableCollisions = false;
        activePlayers.csCursorB[id].resetCursor();
        csPlayerInput[id].holdingCursorStar = true;
        csPlayerInput[id].enablePlayerInput = false;
        while (frame < 8)
        {
            frame++;
            yield return null;
        }
        activePlayers.csCursorBS[id].enabled = false;
        activePlayers.csCursorBG[id].enabled = false;
        activePlayers.csCursorBSN[id].enabled = false;
        yield return new WaitForSeconds(0.2f);
    }

    public IEnumerator beginDestroyCStar(int id)
    {
        int frame = 0;
        activePlayers._csCursorStar[id].animateOn = false;
        activePlayers._csCursorStar[id].noCStarBelow = true;
        activePlayers._csCursorStar[id].destroyCSStar();
        while (frame < 8)
        {
            frame++;
            yield return null;
        }
        activePlayers.csCursorB[id].resetCursor();
        activePlayers.csCursorStar[id].SetActive(false);
        yield return new WaitForSeconds(0.2f);
    }

    public IEnumerator beginSummonPlayerGUI()
    {
        for (int a = 0; a < activePlayers.playerOn.Length; a++)
        {
            csPlayerGUI_go[a].SetActive(true);
        }
        yield return null;
    }

    public IEnumerator beginSummonAPlayerGUI(int id)
    {
        csShard_go[id].SetActive(true);
        yield return new WaitForSeconds(0.1f);
        yield return null;
    }

    public IEnumerator beginDestroyAPlayerGUI(int id)
    {
        csShard_go[id].GetComponent<CharacterSelectShard>().StartCoroutine("destroyPShard");
        yield return new WaitForSeconds(0.1f);
        yield return null;
    }
}
