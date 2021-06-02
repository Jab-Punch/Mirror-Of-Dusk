using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectCursorStatsManager : AbstractCollidableObject
{
    private CharacterSelectCursorPlayerController _baseCursor;

    [NonSerialized] public int characterSelectedId = -1;
    [NonSerialized] private int holding = -1;

    public CursorHitboxPriority playerCursorFound = CursorHitboxPriority.None;
    [NonSerialized] public CursorFoundFlags playerCursorFoundFlags = 0;
    [NonSerialized] public int foundFlagsLength = System.Enum.GetNames(typeof(CharacterSelectCursorStatsManager.CursorFoundFlags)).Length;
    [NonSerialized] public CStarPickFlags cStarPickFlags = 0;
    [NonSerialized] public GuiPickFlags guiPickFlags = 0;

    public enum CursorFoundFlags
    {
        CursorStar = (1 << 0),
        CharacterStar = (1 << 1),
        Rules = (1 << 2),
        Back = (1 << 3),
        MenuUserName = (1 << 4),
        MenuColor = (1 << 5),
        MenuHP = (1 << 6),
        MenuShards = (1 << 7),
        MenuConfig = (1 << 8),
        GUIPlayer = (1 << 9),
        GUICPU = (1 << 10),
        GUINone = (1 << 11)
    }

    public enum CStarPickFlags
    {
        P1 = (1 << 0),
        P2 = (1 << 1),
        P3 = (1 << 2),
        P4 = (1 << 3)
    }

    public enum GuiPickFlags
    {
        P1 = (1 << 0),
        P2 = (1 << 1),
        P3 = (1 << 2),
        P4 = (1 << 3)
    }

    public CharacterSelectCursorPlayerController baseCursor
    {
        get
        {
            if (this._baseCursor == null)
            {
                this._baseCursor = base.GetComponent<CharacterSelectCursorPlayerController>();
            }
            return _baseCursor;
        }
    }

    public int Holding
    {
        get { return holding; }
        set { holding = value; }
    }

    protected override void Awake()
    {
        base.Awake();
        characterSelectedId = -1;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnCollisionCursor(GameObject hit, CollisionPhase phase)
    {
        
    }
}
