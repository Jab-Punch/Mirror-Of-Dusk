using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class PlayerData
{
    private const string KEY = "mirrorofdusk_player_data_v1_slot_";
    private static readonly string[] SAVE_FILE_KEYS = new string[]
    {
        "mirrorofdusk_player_data_v1_slot_0",
        "mirrorofdusk_player_data_v1_slot_1",
        "mirrorofdusk_player_data_v1_slot_2"
    };

    private static string emptyDialoguerState = string.Empty;
    private static int _CurrentSaveFileIndex = 0;
    private static bool _initialized = false;
    public static bool inGame = false;
    
    private static PlayerData[] _saveFiles;
    private static PlayerData.PlayerDataInitHandler _playerDatatInitHandler;
    public int dummy;

    [SerializeField] private PlayerData.PlayerLoadouts loadouts = new PlayerData.PlayerLoadouts();
    //[SerializeField] private bool _isHiddenCharacterAvailable;
    //[SerializeField] private bool _isTutorialCompleted;

    public string dialoguerState;

    //[SerializeField] public PlayerData.PlayerShardManager shardManager = new PlayerData.PlayerShardManager();

    //[SerializeField] private PlayerData.PlayerStats statictics = new PlayerData.PlayerStats();

    private static InitializeCloudStoreHandler f__m0;
    private static LoadCloudDataHandler f__m1;
    private static LoadCloudDataHandler f__m2;
    private static SaveCloudDataHandler f__m3;
    private static SaveCloudDataHandler f__m4;

    public struct StageShardIds
    {
        public Stages stageId;
        public string[][] shardIds;

        public StageShardIds(Stages stage, string[][] shards)
        {
            this.stageId = stage;
            this.shardIds = shards;
        }
    }

    public delegate void PlayerDataInitHandler(bool success);

    [Serializable]
    public class PlayerLoadouts
    {
        public PlayerData.PlayerLoadouts.PlayerLoadout playerOne;
        public PlayerData.PlayerLoadouts.PlayerLoadout playerTwo;
        public PlayerData.PlayerLoadouts.PlayerLoadout playerThree;
        public PlayerData.PlayerLoadouts.PlayerLoadout playerFour;

        public PlayerLoadouts()
        {
            this.playerOne = new PlayerData.PlayerLoadouts.PlayerLoadout();
            this.playerTwo = new PlayerData.PlayerLoadouts.PlayerLoadout();
            this.playerThree = new PlayerData.PlayerLoadouts.PlayerLoadout();
            this.playerFour = new PlayerData.PlayerLoadouts.PlayerLoadout();
        }

        public PlayerLoadouts(PlayerData.PlayerLoadouts.PlayerLoadout playerOne, PlayerData.PlayerLoadouts.PlayerLoadout playerTwo, PlayerData.PlayerLoadouts.PlayerLoadout playerThree, PlayerData.PlayerLoadouts.PlayerLoadout playerFour)
        {
            this.playerOne = playerOne;
            this.playerTwo = playerTwo;
            this.playerTwo = playerThree;
            this.playerTwo = playerFour;
        }

        public PlayerData.PlayerLoadouts.PlayerLoadout GetPlayerLoadout(PlayerId player)
        {
            if (player == PlayerId.PlayerOne)
            {
                return this.playerOne;
            }
            if (player == PlayerId.PlayerTwo)
            {
                return this.playerTwo;
            }
            if (player == PlayerId.PlayerThree)
            {
                return this.playerThree;
            }
            if (player == PlayerId.PlayerFour)
            {
                return this.playerFour;
            }
            return null;
        }

        [Serializable]
        public class PlayerLoadout
        {
            public PlayerLoadout()
            {
                //Player features
            }
        }
    }
}
