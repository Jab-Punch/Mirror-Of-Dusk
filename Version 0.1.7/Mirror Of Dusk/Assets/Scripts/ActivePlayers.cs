using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivePlayers : MonoBehaviour
{
    private CharacterSelectManager characterSelectManager;
    public int assignmentNumberCheck { get; private set; }
    
    /*[SerializeField] public bool player1On = true;
    [SerializeField] public bool player2On = true;
    [SerializeField] public bool player3On = true;
    [SerializeField] public bool player4On = true;*/
    [SerializeField] public bool[] playerOn = new bool[4] { false, false, false, false };
    public CSPlayerInput[] csPlayerInput;
    public int playerHierarchyCheck = 0;

    private SummonCursors summonCursors;
    public CSCursorStar[] _csCursorStar { get; private set; }
    public GameObject[] csCursorG { get; private set; }
    public CSCursor[] csCursorB { get; private set; }
    public SpriteRenderer[] csCursorBS { get; private set; }
    public SpriteRenderer[] csCursorBG { get; private set; }
    public SpriteRenderer[] csCursorBSN { get; private set; }
    public GameObject[] csCursorStar { get; private set; }

    public class PlayerStarPositions
    {
        public float posX;
        public float posY;
        public float posZ;
    }
    public class PlayerStarColors
    {
        public float r;
        public float g;
        public float b;
        public float a;
    }
    public Dictionary<int, PlayerStarPositions> playerStarPos = new Dictionary<int, PlayerStarPositions>() {
        { 1, new PlayerStarPositions { posX = -150f, posY = 50f, posZ = -21f } },
        { 2, new PlayerStarPositions { posX = -75f, posY = 30f, posZ = -19f } },
        { 3, new PlayerStarPositions { posX = 75f, posY = 30f, posZ = -17f } },
        { 4, new PlayerStarPositions { posX = 150f, posY = 50f, posZ = -15f } } };
    public Dictionary<int, PlayerStarPositions> playerCursorPos = new Dictionary<int, PlayerStarPositions>() {
        { 1, new PlayerStarPositions { posX = -645f, posY = -300f, posZ = -36f } },
        { 2, new PlayerStarPositions { posX = -215f, posY = -300f, posZ = -34f } },
        { 3, new PlayerStarPositions { posX = 215f, posY = -300f, posZ = -32f } },
        { 4, new PlayerStarPositions { posX = 645f, posY = -300f, posZ = -30f } } };
    public Dictionary<int, PlayerStarColors> playerCursorColors = new Dictionary<int, PlayerStarColors>() {
        { 0, new PlayerStarColors { r = 0.4f, g = 0.4f, b = 0.4f, a = 1.0f } },
        { 1, new PlayerStarColors { r = 0.85f, g = 0.45f, b = 0.45f, a = 1.0f } },
        { 2, new PlayerStarColors { r = 0.40f, g = 0.40f, b = 0.80f, a = 1.0f } },
        { 3, new PlayerStarColors { r = 0.75f, g = 0.40f, b = 0.80f, a = 1.0f } },
        { 4, new PlayerStarColors { r = 0.40f, g = 0.70f, b = 0.80f, a = 1.0f } } };
    public Dictionary<int, PlayerStarPositions> playerGUIPos = new Dictionary<int, PlayerStarPositions>() {
        { 1, new PlayerStarPositions { posX = -645f, posY = -1050f, posZ = -10f } },
        { 2, new PlayerStarPositions { posX = -215f, posY = -1050f, posZ = -10f } },
        { 3, new PlayerStarPositions { posX = 215f, posY = -1050f, posZ = -10f } },
        { 4, new PlayerStarPositions { posX = 645f, posY = -1050f, posZ = -10f } } };

    private void Awake()
    {
        characterSelectManager = CharacterSelectManager.characterSelectManager;
        assignmentNumberCheck = 0;
        
        summonCursors = characterSelectManager.csControl.GetComponent<SummonCursors>();
        csPlayerInput = new CSPlayerInput[4];
        for (int i = 0; i < characterSelectManager.players.Length; i++)
        {
            csPlayerInput[i] = characterSelectManager.players[i].GetComponent<CSPlayerInput>();
        }

        csCursorG = new GameObject[4];
        csCursorB = new CSCursor[4];
        _csCursorStar = new CSCursorStar[4];
        csCursorBG = new SpriteRenderer[4];
        csCursorBS = new SpriteRenderer[4];
        csCursorBSN = new SpriteRenderer[4];
        csCursorStar = new GameObject[4];
        Color[] starBaseColor = new Color[5] { new Color(0.85f, 0.45f, 0.45f, 1.0f), new Color(0.40f, 0.40f, 0.80f, 1.0f), new Color(0.75f, 0.40f, 0.80f, 1.0f), new Color(0.40f, 0.70f, 0.80f, 1.0f), new Color(0.4f, 0.4f, 0.4f, 1.0f) };
        Color[] starEdgeColor = new Color[5] { new Color(0.98f, 0.75f, 0.75f, 0.8f), new Color(0.50f, 0.50f, 0.90f, 0.8f), new Color(0.90f, 0.72f, 0.95f, 0.8f), new Color(0.72f, 0.87f, 0.95f, 0.8f), new Color(0.8f, 0.8f, 0.8f, 0.8f) };
        Color[] starLabelColor = new Color[5] { new Color(0.98f, 0.88f, 0.88f, 1.0f), new Color(0.80f, 0.80f, 0.95f, 1.0f), new Color(0.93f, 0.87f, 0.95f, 1.0f), new Color(0.87f, 0.93f, 0.95f, 1.0f), new Color(0.95f, 0.95f, 0.95f, 1.0f) };
        Sprite[] sprL = Resources.LoadAll<Sprite>("CharacterSelect/CharacterSelectCursorStarLabel");

        for (int a = 0; a < playerOn.Length; a++)
        {
            csCursorG[a] = Instantiate(summonCursors.cursorPrefab, new Vector3(playerCursorPos[a + 1].posX, playerCursorPos[a + 1].posY, playerCursorPos[a + 1].posZ), Quaternion.identity);
            csCursorG[a].name = "CharacterSelectCursor_" + System.Convert.ToString(a + 1);
            characterSelectManager.csCursors[a] = csCursorG[a];
            csCursorB[a] = csCursorG[a].GetComponent<CSCursor>();
            csCursorB[a].cursorId = a;
            csCursorB[a].setBurstColor(starBaseColor[a], false);
            csCursorBS[a] = csCursorG[a].GetComponent<SpriteRenderer>();
            csCursorBS[a].color = new Color(playerCursorColors[a + 1].r, playerCursorColors[a + 1].g, playerCursorColors[a + 1].b, playerCursorColors[a + 1].a);
            csCursorBG[a] = csCursorB[a].cursorGlow.GetComponent<SpriteRenderer>();
            csCursorBSN[a] = csCursorB[a].cursorNumber.GetComponent<SpriteRenderer>();
            Sprite spr = Resources.Load<Sprite>("CharacterSelect/CSCursorNo" + System.Convert.ToString(a + 1));
            csCursorBSN[a].sprite = spr;
            csCursorBSN[a].color = new Color(playerCursorColors[a + 1].r, playerCursorColors[a + 1].g, playerCursorColors[a + 1].b, playerCursorColors[a + 1].a);
            
            ++assignmentNumberCheck;
        }

        assignmentNumberCheck = 0;
        for (int a = 0; a < playerOn.Length; a++)
        {
            csCursorStar[a] = Instantiate(summonCursors.cursorStarPrefab, new Vector3(playerCursorPos[a + 1].posX - 24.5f, playerCursorPos[a + 1].posY + 24.5f, playerStarPos[a + 1].posZ), Quaternion.identity);
            csCursorStar[a].name = "CharacterSelectCursorStar_" + System.Convert.ToString(a + 1);
            characterSelectManager.csCursorStars[a] = csCursorStar[a];
            csPlayerInput[a].initialCursorStar = csCursorStar[a].GetComponent<CSCursorStar>();
            csPlayerInput[a].currentCursorStar = csPlayerInput[a].initialCursorStar;
            _csCursorStar[a] = csCursorStar[a].GetComponent<CSCursorStar>();
            _csCursorStar[a].cursorId = a;
            _csCursorStar[a].AssignLabels(sprL[a + 1], sprL[0]);
            _csCursorStar[a].AssignPlayerColors(starEdgeColor[a], starBaseColor[a], starLabelColor[a]);
            _csCursorStar[a].AssignCPUColors(starEdgeColor[4], starBaseColor[4], starLabelColor[4]);
            csCursorBS[a].enabled = false;
            csCursorBG[a].enabled = false;
            csCursorBSN[a].enabled = false;
            csCursorStar[a].SetActive(false);
            
            ++assignmentNumberCheck;
        }
    }

    // Update is called once per frame
    void Update()
    {
        playerHierarchyCheck = findHierarchyNumber();
    }

    public int findHierarchyNumber()
    {
        int tempHierarchyNumber = 0;
        for (int i = 0; i < csPlayerInput.Length; i++)
        {
            tempHierarchyNumber = Mathf.Max(tempHierarchyNumber, csPlayerInput[i].playerHierarchy);
        }
        return tempHierarchyNumber;
    }
}
