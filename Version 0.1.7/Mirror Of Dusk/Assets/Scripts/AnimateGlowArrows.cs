using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateGlowArrows : AnimateSprite {

    /*private Dictionary<int, int[]> animPhaseSet;
    private Dictionary<int, int[]> animSpeedSet;*/
    //SpriteRenderer sprend;
    //Sprite[] spr;

    // Use this for initialization
    /*protected override void Start () {
        Debug.Log("Initializing");
        base.Start();
    }*/

    protected override void StartCall()
    {
        initializeAnimation();
    }
	
	// Update is called once per frame
	/*void Update () {
		
	}*/

    public void initializeAnimation()
    {
        /*animPhase = new Dictionary<int, int[]>
        {
            { 0, new int[1] { 0 } },
            { 1, new int[6] { 1, 2, 3, 4, 3, 2 } },
            { 2, new int[1] { 5 } }
        };
        animPhase = new Dictionary<int, int[]>
        {
            { 0, new int[1] { 0 } },
            { 1, new int[6] { 2, 2, 3, 3, 3, 2 } },
            { 2, new int[1] { 0 } }
        };*/
        animSeq = new Dictionary<int, AnimData>
        {
            { 0, new AnimData{ phase = new int[1]{ 5 }, speed = new int[1]{ 1 }, spriteName = "MenuScreenArrow1" } },
            { 1, new AnimData{ phase = new int[6]{ 1, 2, 3, 4, 3, 2 }, speed = new int[6]{ 2, 2, 3, 3, 3, 2 }, spriteName = "MenuScreenArrow1" } },
            { 2, new AnimData{ phase = new int[1]{ 0 }, speed = new int[1]{ 1 }, spriteName = "MenuScreenArrow1" } }
        };
        animSkip = new Dictionary<int, AnimNext>
        {
            { 0, new AnimNext{ nextPhase = 0, loopPhase = false, loopAmount = 1 } },
            { 1, new AnimNext{ nextPhase = 1, loopPhase = false, loopAmount = 1 } },
            { 2, new AnimNext{ nextPhase = 2, loopPhase = false, loopAmount = 1 } }
        };
        loopOn = animSkip[animState].loopPhase;
        loopCount = animSkip[animState].loopAmount;
    }
}
