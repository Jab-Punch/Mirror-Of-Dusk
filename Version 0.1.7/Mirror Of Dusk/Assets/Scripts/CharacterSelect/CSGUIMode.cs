using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSGUIMode : MonoBehaviour, IHurtboxResponder
{
    private CharacterSelectManager characterSelectManager;

    [System.Serializable]
    public class ModeSprites    //Indicate the sprite color of the icon based on its conditions
    {
        public string name;
        public Sprite off;      //When no cursor is above the icon
        public Sprite on;       //When any cursor is above the icon while its mode is inactive
        public Sprite active;   //When the current mode is active
    }

    public int modeID;
    public ModeSprites modeSprites;
    private SpriteRenderer modeIcon;
    private CSPlayerGUI rootGUI;            //Parent GUI that the mode relates to
    private CSPlayerInput[] csPlayerInput;
    public int rootID;                      //Player id of the parent GUI
    private Hurtbox hurtboxSet;

    private Dictionary<int, int> modeFlag = new Dictionary<int, int>    //For assigning the selected mode to the player
    {
        { 0, 9 },
        { 1, 10 },
        { 2, 11 }
    };

    void Awake()
    {
        characterSelectManager = CharacterSelectManager.characterSelectManager;
        modeIcon = gameObject.GetComponent<SpriteRenderer>();
        rootGUI = gameObject.transform.parent.gameObject.transform.parent.GetComponent<CSPlayerGUI>();
        rootID = rootGUI.guiID;
        csPlayerInput = new CSPlayerInput[4];
        for (int i = 0; i < characterSelectManager.players.Length; i++)
        {
            csPlayerInput[i] = characterSelectManager.players[i].GetComponent<CSPlayerInput>();
        }
        hurtboxSet = gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<Hurtbox>();
    }

    void Start()
    {
        hurtboxSet.useHurtResponder(this);
    }
	
	// Update is called once per frame
	void Update () {
        if (modeSprites.name == "ModePlayer")
        {
            if (rootGUI.currentGUIMode == CSPlayerGUI.GUIMode.Player)
            {
                modeIcon.sprite = modeSprites.active;
            }
            else
            {
                modeIcon.sprite = modeSprites.off;
            }
        } else if (modeSprites.name == "ModeCPU")
        {
            if (rootGUI.currentGUIMode == CSPlayerGUI.GUIMode.CPU)
            {
                modeIcon.sprite = modeSprites.active;
            }
            else
            {
                modeIcon.sprite = modeSprites.off;
            }
        } else if (modeSprites.name == "ModeNone")
        {
            if (rootGUI.currentGUIMode == CSPlayerGUI.GUIMode.None)
            {
                modeIcon.sprite = modeSprites.active;
            } else
            {
                modeIcon.sprite = modeSprites.off;
            }
        }
        if (modeIcon.sprite != modeSprites.active)
        {
            if (rootGUI.modeHighlighted[modeID].mhFlags > 0)
            {
                modeIcon.sprite = modeSprites.on;
            }
            else
            {
                modeIcon.sprite = modeSprites.off;
            }
        }
        for (int i = 0; i < 4; i++)     //Remove any flags from GUI whose mode that player is no longer highlighting
        {
            if (!rootGUI.modeHighlighted[modeID].mhFlagsFound.HasFlag((CSPlayerGUI.MhFlags)(1 << i)))
            {
                rootGUI.modeHighlighted[modeID].mhFlags &= ~(CSPlayerGUI.MhFlags)(1 << i);
            }
            rootGUI.modeHighlighted[modeID].mhFlagsFound &= ~(CSPlayerGUI.MhFlags)(1 << i);
        }
    }

    private void addToRootGUI(int mode, int cursor) //Set the flag for a highlighted mode to the GUI
    {
        rootGUI.modeHighlighted[mode].mhFlags |= (CSPlayerGUI.MhFlags)(1 << cursor);
        rootGUI.modeHighlighted[mode].mhFlagsFound |= (CSPlayerGUI.MhFlags)(1 << cursor);
    }

    public void collisionDetected(Hitbox hitbox, Hurtbox hurtbox)
    {
        addToRootGUI(modeID, hitbox.hitboxData.playerId);
        csPlayerInput[hitbox.hitboxData.playerId].playerCursorFoundFlags |= (CSPlayerInput.CursorFoundFlags)(1 << modeFlag[modeID]);
    }
}
