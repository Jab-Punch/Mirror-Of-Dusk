using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShardsMenuControl : MonoBehaviour {

    private CharacterSelectManager characterSelectManager;

    public CSPlayerData[] csPlayerData;
    public GameData gameData;
    public int totalShardsLeft;

    private void Awake()
    {
        characterSelectManager = CharacterSelectManager.characterSelectManager;
        csPlayerData = new CSPlayerData[4];
        for (int i = 0; i < csPlayerData.Length; i++)
        {
            csPlayerData[i] = characterSelectManager.players[i].GetComponent<CSPlayerData>();
        }
        gameData = characterSelectManager.gameData.GetComponent<GameData>();
    }

    // Use this for initialization
    void Start () {
        DeclareShardsLeft();
    }

    public void DeclareShardsLeft()
    {
        totalShardsLeft = gameData.defaultTotalShards;
        for (int i = 0; i < csPlayerData.Length; i++)
        {
            totalShardsLeft -= csPlayerData[i].playerInitShards;
        }
    }
}
