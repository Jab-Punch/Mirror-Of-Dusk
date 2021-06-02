using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimateSprite : MonoBehaviour {

    [SerializeField] protected bool animateOn = true;
    [SerializeField] public int animState;
    //[SerializeField] public int[] animSequence;
    //[SerializeField] public int[] animSpeed;
    //public Dictionary<int, int[]> animPhase;
    public Dictionary<int, AnimData> animSeq;
    //public Dictionary<int, int[]> animSpeed;
    public Dictionary<int, AnimNext> animSkip;
    SpriteRenderer sprend;
    Sprite[] spr;
    //private bool animDone = false;

    protected bool loopOn = false;
    protected int loopCount = 0;

    int frame = 0;
    int renderFrame = 0;

    public class AnimData
    {
        public int[] phase;
        public int[] speed;
        public string spriteName;
    }
    public class AnimNext
    {
        public int nextPhase;
        public bool loopPhase;
        public int loopAmount;
    }

    // Use this for initialization
    void Start () {
        StartCall();
        sprend = gameObject.GetComponent<SpriteRenderer>();
        spr = Resources.LoadAll<Sprite>(sprend.sprite.name.Substring(0, sprend.sprite.name.Length-2));
        frame = 0;
        renderFrame = 0;
        if (animSeq != null)
        {
            sprend.sprite = spr[animSeq[animState].phase[renderFrame]];
        } else
        {
            sprend.sprite = spr[0];
        }
    }

    protected abstract void StartCall();
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate()
    {
        if (animateOn)
        {
            if (animSeq[animState].phase != null && animSeq[animState].speed != null)
            {
                /*if (!animDone)
                {
                    frame++;
                    if (frame >= animSeq[animState].speed[renderFrame])
                    {
                        renderFrame++;
                        if (renderFrame >= animSeq[animState].phase.Length)
                        {
                            if (loopOn)
                            {
                                renderFrame = 0;
                            }
                            else
                            {
                                animDone = true;
                            }
                        }
                        if (!animDone)
                        {
                            sprend.sprite = spr[animSeq[animState].phase[renderFrame]];
                        }
                        frame = 0;
                    }
                }*/
                frame++;
                if (frame >= animSeq[animState].speed[renderFrame])
                {
                    renderFrame++;
                    if (renderFrame >= animSeq[animState].phase.Length)
                    {
                        renderFrame = 0;
                        if (loopCount > 0)
                        {
                            loopCount--;
                        }
                        if (loopCount == 0 && !loopOn)
                        {
                            animState = animSkip[animState].nextPhase;
                            loopCount = animSkip[animState].loopAmount;
                            loopOn = animSkip[animState].loopPhase;
                        }
                    }
                    spr = Resources.LoadAll<Sprite>(animSeq[animState].spriteName);
                    sprend.sprite = spr[animSeq[animState].phase[renderFrame]];
                    frame = 0;
                }
            }
        } else
        {
            this.enabled = false;
        }
    }

    /*public IEnumerator setCoAnimation(int nAnimState)
    {
        int fixCount = 0;
        while (animSeq == null && fixCount < 50)
        {
            fixCount++;
            yield return null;
        }
        animState = nAnimState;
        Debug.Log(animSeq);
        string nSprite = animSeq[nAnimState].spriteName;
        sprend = gameObject.GetComponent<SpriteRenderer>();
        spr = Resources.LoadAll<Sprite>(nSprite);
        sprend.sprite = spr[animSeq[animState].phase[0]];
        loopOn = animSkip[animState].loopPhase;
        loopCount = animSkip[animState].loopAmount;
        frame = 0;
        renderFrame = 0;
        yield return null;
    }*/

    public void setAnimation(int nAnimState)
    {
        animState = nAnimState;
        if (animSeq != null)
        {
            string nSprite = animSeq[nAnimState].spriteName;
            sprend = gameObject.GetComponent<SpriteRenderer>();
            spr = Resources.LoadAll<Sprite>(nSprite);
            sprend.sprite = spr[animSeq[animState].phase[0]];
            if (animSeq[animState].phase.Length <= 1 && animState == animSkip[animState].nextPhase)
            {
                animateOn = false;
            } else
            {
                animateOn = true;
                this.enabled = true;
            }
        }
        if (animSkip != null)
        {
            loopOn = animSkip[animState].loopPhase;
            loopCount = animSkip[animState].loopAmount;
        }
        frame = 0;
        renderFrame = 0;
    }

    public void setFrame(int nFrame, int nSpeed)
    {
        renderFrame = nFrame;
        frame = nSpeed;
    }

    public int getFrame()
    {
        return renderFrame;
    }

    public int getSpeed()
    {
        return frame;
    }
}
