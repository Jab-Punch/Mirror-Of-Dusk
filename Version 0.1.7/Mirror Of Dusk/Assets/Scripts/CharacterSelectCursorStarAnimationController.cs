using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectCursorStarAnimationController : AbstractCollidableObject
{
    [SerializeField] private CharacterSelectCursorStar cursorStar;
    public bool hitboxActive = false;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        hitboxActive = true;
    }

    protected void OnDisable()
    {
        hitboxActive = false;
    }

    protected override void Update()
    {
        base.Update();
    }

    /*private void LateUpdate()
    {

    }*/

    protected override void OnCollisionBackground(GameObject hit, CollisionPhase phase)
    {
        if (hitboxActive)
        {
            if (phase == CollisionPhase.Enter)
            {
                cursorStar.StartCoroutine(cursorStar.reGrowSprites_cr());
            }
            else if (phase == CollisionPhase.Stay)
            {
                cursorStar.nearStars = true;
            }
            else if (phase == CollisionPhase.Exit)
            {
                cursorStar.StartCoroutine(cursorStar.reShrinkSprites_cr());
                cursorStar.nearStars = false;
            }
        }
    }
}
