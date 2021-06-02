using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSPlayerData : MonoBehaviour
{
    private CharacterSelectManager characterSelectManager;

    public int playerId = 0;
    public string selectedCharacter;
    public int foundCharacterCode = 0;
    public int characterCode = 0;
    public int highlightedCharacter;
    public Dictionary<int, int> characterName;
    public Dictionary<int, string> selectedCharacterName;
    public bool emptyConfirmed = false;

    public int characterColorCode = 0;
    public int playerHealth;
    public int playerInitShards;
    public int playerShardStrength;

    CSPlayerInput csPlayerInput;
    CharacterSelectShard csShard;
    CSCursorStar csCursorStar;
    GameData gameData;

    private void Awake()
    {
        characterSelectManager = CharacterSelectManager.characterSelectManager;

        csPlayerInput = characterSelectManager.players[playerId].GetComponent<CSPlayerInput>();
        csShard = characterSelectManager.csShards.transform.GetChild(playerId).GetComponent<CharacterSelectShard>();
        csCursorStar = characterSelectManager.csCursorStars[playerId].GetComponent<CSCursorStar>();
        gameData = characterSelectManager.gameData.GetComponent<GameData>();
        initializePlayerData();
    }

    // Use this for initialization
    void Start()
    {
        selectedCharacter = "???";
        //characterCode = -1;
        characterColorCode = 0;
        initializeCharacterNames();
        emptyConfirmed = false;
    }

    public void initializePlayerData()
    {
        playerHealth = gameData.defaultInitialHealth;
        playerInitShards = gameData.defaultInitialShards;
        playerShardStrength = gameData.defaultShardStrength;
    }

    // Update is called once per frame
    void Update()
    {
        if (csCursorStar.isHeld)
        {
            if (foundCharacterCode != characterCode)
            {
                characterCode = foundCharacterCode;
                csShard.UpdateShard();
            }
        }
        foundCharacterCode = 0;
    }

    private void initializeCharacterNames()
    {
        characterName = new Dictionary<int, int> {
            { 0, 10 },
            { 1, 0 },
            { 2, 1 },
            { 3, 2 },
            { 4, 3 },
            { 5, 9 },
            { 6, 7 },
            { 7, 8 },
            { 8, 4 },
            { 9, 6 },
            { 10, 5 },
            { 11, 12 },
            { -1, 12 }
        };
        selectedCharacterName = new Dictionary<int, string> {
            { 0, "???" },
            { 1, "Tilly" },
            { 2, "Celinda" },
            { 3, "Leatrice" },
            { 4, "Pompion" },
            { 5, "Deimos" },
            { 6, "Aldric" },
            { 7, "Akita" },
            { 8, "Faithe" },
            { 9, "Sigmund" },
            { 10, "Mugo" },
            { 11, "Wymond" },
            { 12, "Silva" }
        };
    }

    public void selectStar()
    {
        selectedCharacter = selectedCharacterName[characterCode];
    }

    public void deselectStar()
    {
        selectedCharacter = "???";
    }
}
