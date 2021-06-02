using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class CharacterSelectData
{
    private static bool _initialized = false;
    public static bool inGame = false;
    private static CharacterSelectData _csData;

    [SerializeField]
    private CharacterSelectData.CharacterSelectDataManager characterSelectDataManager = new CharacterSelectData.CharacterSelectDataManager();

    [Serializable]
    public class CharacterSelectPlayerSlotProperties
    {
        public Color cursorBodyColor;
        public Color cursorStarEdgeColor;
        public Color cursorStarLabelColor;
        public Vector3 defaultCursorPosition;

        public CharacterSelectPlayerSlotProperties(Color cursorBodyColor, Color cursorStarEdgeColor, Color cursorStarLabelColor, Vector3 defaultCursorPosition)
        {
            this.cursorBodyColor = cursorBodyColor;
            this.cursorStarEdgeColor = cursorStarEdgeColor;
            this.cursorStarLabelColor = cursorStarLabelColor;
            this.defaultCursorPosition = defaultCursorPosition;
        }
    }

    [Serializable]
    public class CharacterSelectDataManager
    {
        public BattleMode currentMode;
        public CharacterSelectData.PlayerSlot[] playerSlots;
        public CharacterSelectData.CharacterSelectPlayerSlotProperties[] playerSlotProperties;
        public Vector2[] guiRestPos;
        public Vector3[] characterShardRestPos;
        public bool[] cursorStarHeld;

        public CharacterSelectDataManager()
        {
            InitializeCharacterSelectSlotProperties();
            cursorStarHeld = new bool[]
            {
                false,
                false,
                false,
                false
            };
            playerSlots = new CharacterSelectData.PlayerSlot[]
            {
                new CharacterSelectData.PlayerSlot(),
                new CharacterSelectData.PlayerSlot(),
                new CharacterSelectData.PlayerSlot(),
                new CharacterSelectData.PlayerSlot()
            };
            guiRestPos = new Vector2[]
            {
                new Vector2(-645f, -1000f),
                new Vector2(-215f, -1000f),
                new Vector2(215f, -1000f),
                new Vector2(645f, -1000f)
            };
            characterShardRestPos = new Vector3[]
            {
                new Vector3(-645f, -730f, 200f),
                new Vector3(-215f, -730f, 200f),
                new Vector3(215f, -730f, 200f),
                new Vector3(645f, -730f, 200f)
            };
        }

        public BattleMode GetCurrentMode()
        {
            return this.currentMode;
        }

        private void InitializeCharacterSelectSlotProperties()
        {
            playerSlotProperties = new CharacterSelectData.CharacterSelectPlayerSlotProperties[]
            {
                new CharacterSelectData.CharacterSelectPlayerSlotProperties(new Color(0.85f, 0.45f, 0.45f), new Color(0.98f, 0.75f, 0.75f, 0.8f), new Color(0.98f, 0.88f, 0.88f), new Vector3(-645f, -300f, 0f)),
                new CharacterSelectData.CharacterSelectPlayerSlotProperties(new Color(0.4f, 0.4f, 0.8f), new Color(0.50f, 0.50f, 0.90f, 0.8f), new Color(0.80f, 0.80f, 0.95f), new Vector3(-215f, -300f, 0f)),
                new CharacterSelectData.CharacterSelectPlayerSlotProperties(new Color(0.75f, 0.4f, 0.8f), new Color(0.90f, 0.72f, 0.95f, 0.8f), new Color(0.93f, 0.87f, 0.95f), new Vector3(215f, -300f, 0f)),
                new CharacterSelectData.CharacterSelectPlayerSlotProperties(new Color(0.4f, 0.7f, 0.8f), new Color(0.72f, 0.87f, 0.95f, 0.8f), new Color(0.87f, 0.93f, 0.95f), new Vector3(645f, -300f, 0f)),
                new CharacterSelectData.CharacterSelectPlayerSlotProperties(new Color(0.4f, 0.4f, 0.4f), new Color(0.8f, 0.8f, 0.8f, 0.8f), new Color(0.95f, 0.95f, 0.95f), new Vector3(0f, -300f, 0f))
            };
        }
    }

    public class PlayerSlot
    {
        public enum JoinState
        {
            NotJoining,
            JoinPromptDisplayed,
            JoinRequested,
            Joined,
            Leaving,
            LeaveRequested
        }

        public enum ControllerState
        {
            NoController,
            UsingController,
            Disconnected,
            ReconnectPromptDisplayed,
            WaitingForReconnect
        }

        public CharacterSelectData.PlayerSlot.JoinState joinState;
        public CharacterSelectData.PlayerSlot.ControllerState controllerState;
        public int controllerId;

        public Vector3 playerCursorPosition = Vector3.zero;
        public Vector3 startingPlayerCursorPosition = Vector3.zero;
        public Vector3 cursorStarPosition = Vector3.zero;
    }

    public CharacterSelectData()
    {
        //Temporary
        characterSelectDataManager.playerSlots[0].joinState = CharacterSelectData.PlayerSlot.JoinState.Joined;
        PlayerManager.SetPlayerJoinState(PlayerId.PlayerOne, 2);
        characterSelectDataManager.playerSlots[1].joinState = CharacterSelectData.PlayerSlot.JoinState.Joined;
        PlayerManager.SetPlayerJoinState(PlayerId.PlayerTwo, 2);
    }

    public static bool Initialized
    {
        get
        {
            return CharacterSelectData._initialized;
        }
        private set
        {
            CharacterSelectData._initialized = value;
        }
    }

    public static CharacterSelectData Data
    {
        get
        {
            return CharacterSelectData._csData;
        }
    }

    public static void Init()
    {
        if (CharacterSelectData.Data == null)
        {
            CharacterSelectData._csData = new CharacterSelectData();
        }
        CharacterSelectData.Initialized = true;
    }

    public BattleMode CurrentMode
    {
        get
        {
            return this.characterSelectDataManager.currentMode;
        }
        set
        {
            this.characterSelectDataManager.currentMode = value;
        }
    }

    public int GetPlayerStatus(int playerCode)
    {
        if (playerCode < 4)
        {
            if (PlayerManager.GetPlayerStatus((PlayerId)playerCode, (int)characterSelectDataManager.playerSlots[playerCode].joinState))
            {
                return (int)characterSelectDataManager.playerSlots[playerCode].joinState;
            }
        }
        return -1;
    }

    public void AssignDefaultStartPlayerCursorPositions(BattleMode mode)
    {
        switch (mode)
        {
            default:
                characterSelectDataManager.playerSlots[0].startingPlayerCursorPosition = new Vector3(-645f, -300f, 0);
                characterSelectDataManager.playerSlots[1].startingPlayerCursorPosition = new Vector3(-215f, -300f, 0);
                characterSelectDataManager.playerSlots[2].startingPlayerCursorPosition = new Vector3(215f, -300f, 0);
                characterSelectDataManager.playerSlots[3].startingPlayerCursorPosition = new Vector3(645f, -300f, 0);
                break;
        }
    }

    public void AssignStartPlayerCursorPositions()
    {
        for (int i = 0; i < characterSelectDataManager.playerSlots.Length; i++)
        {
            characterSelectDataManager.playerSlots[i].playerCursorPosition = characterSelectDataManager.playerSlots[i].startingPlayerCursorPosition;
            characterSelectDataManager.playerSlots[i].cursorStarPosition = new Vector3(characterSelectDataManager.playerSlots[i].startingPlayerCursorPosition.x, characterSelectDataManager.playerSlots[i].startingPlayerCursorPosition.y + 320f, 0);
        }
    }

    public int GetPlayerSlotCount
    {
        get { return characterSelectDataManager.playerSlots.Length; }
    }

    public CharacterSelectData.PlayerSlot GetPlayerSlot(int slot)
    {
        return characterSelectDataManager.playerSlots[slot];
    }

    public CharacterSelectData.CharacterSelectPlayerSlotProperties GetPlayerSlotProperties(int slot)
    {
        return characterSelectDataManager.playerSlotProperties[slot];
    }

    public bool GetCursorStarHeldStatus(int slot)
    {
        return characterSelectDataManager.cursorStarHeld[slot];
    }

    public void SetCursorStarHeldStatus(int slot, bool status)
    {
        characterSelectDataManager.cursorStarHeld[slot] = status;
    }

    public int GetInitialCursorStarHeldId(int slot, bool status)
    {
        if (status)
        {
            return slot;
        } else
        {
            return -1;
        }
    }

    public Vector2 GetGuiRestPos(int slot)
    {
        return characterSelectDataManager.guiRestPos[slot];
    }

    public Vector2 GetCharacterShardRestPos(int slot)
    {
        return characterSelectDataManager.characterShardRestPos[slot];
    }
}
