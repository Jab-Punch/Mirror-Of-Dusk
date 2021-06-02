using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectBorderHurtbox : AbstractCollidableObject
{
    [SerializeField] public BorderType borderType;
    
    private bool hitboxActive = false;
    private int highlightCount = 0;

    public enum BorderType
    {
        Back,
        Rules
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.hitboxActive = true;
    }

    protected void OnDisable()
    {
        this.hitboxActive = false;
    }

    protected override void Update()
    {
        base.Update();
        highlightCount = 0;
    }

    protected override void OnCollisionCursor(GameObject hit, CollisionPhase phase)
    {
        if (hitboxActive)
        {
            if (phase == CollisionPhase.Stay)
            {
                if (CharacterSelectScene.Current.state == CharacterSelectScene.State.Selecting)
                {
                    CharacterSelectCursorStatsManager stats = hit.GetComponent<CharacterSelectCursorStatsManager>();
                    if (this.borderType == BorderType.Back)
                    {
                        stats.playerCursorFoundFlags |= (CharacterSelectCursorStatsManager.CursorFoundFlags)(1 << 3);
                    }
                    highlightCount++;
                }
            }
        }
    }
}
