using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteMaskAnimator : SpriteAnimator {

    public SpriteMask spriteMask;

    protected override void Awake()
    {
        sfxPlayer = GameObject.Find("SFX Player").GetComponent<SFXPlayer>();
        if (!spriteMask)
        {
            spriteMask = GetComponent<SpriteMask>();
        }
    }

    public override void ForcePlay(string name, bool loop = true, int startFrame = 0)
    {
        Animation animation = GetAnimation(name);
        if (animation != null)
        {
            this.loop = loop;
            currentAnimation = animation;
            playing = true;
            currentFrame = startFrame;
            spriteMask.sprite = animation.frames[currentFrame].frame;
            if (lastPlay != null)
            {
                StopCoroutine(lastPlay);
            }
            lastPlay = StartCoroutine(PlayAnimation(currentAnimation));
        }
    }

    protected override IEnumerator PlayAnimation(Animation animation)
    {
        int timer = 0;
        //float delay = 1f / (float)animation.fps;
        while (loop || currentFrame < animation.frames.Length - 1)
        {
            bool skipNF = false;
            int delay = animation.frames[currentFrame].fps;
            while (timer < delay)
            {
                if (timer == 0)
                {
                    NextSound(animation);
                }
                //timer += Time.deltaTime;
                timer += 1;
                yield return 0f;
                if (timer >= delay)
                {
                    skipNF = NextAnimation(animation);
                }
            }
            if (!skipNF)
            {
                while (timer >= delay)
                {
                    timer -= delay;
                    NextFrame(animation);
                }
                spriteMask.sprite = animation.frames[currentFrame].frame;
            }
            else
            {
                yield return null;
            }
        }

        currentAnimation = null;

    }

    public override int GetFacing()
    {
        return (int)Mathf.Sign(spriteMask.transform.localScale.x);
    }

    public override void FlipTo(float dir)
    {
        if (dir < 0f)
        {
            spriteMask.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            spriteMask.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    public override void FlipTo(Vector3 position)
    {
        float diff = position.x - transform.position.x;
        if (diff < 0f)
        {
            spriteMask.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            spriteMask.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
