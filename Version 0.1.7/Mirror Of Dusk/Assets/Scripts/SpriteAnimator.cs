using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class AnimationEventTrigger : UnityEvent<string>
{

}

[System.Serializable]
public class AnimationEventTriggerFloat : UnityEvent<float>
{

}

public class SpriteAnimator : MonoBehaviour {

    //private UnityEvent m_AnimEvent;

    [System.Serializable]
    public class Frames
    {
        public Sprite frame;
        public int fps = 1;
    }

    [System.Serializable]
    public class AnimationTrigger
    {
        public int frame;
        public string name;
        public float value;
        public AnimationEventTrigger m_AnimationEventTrigger = new AnimationEventTrigger();
        public AnimationEventTriggerFloat m_AnimationEventTriggerFloat = new AnimationEventTriggerFloat();
    }

    [System.Serializable]
    public class NextAnimationTrigger
    {
        public int frame;
        public string name;
        public bool loop;
        public int startFrame;
    }

    [System.Serializable]
    public class SoundTrigger
    {
        public string name;
        public int frame;
        public int delay = 0;
    }

    [System.Serializable]
    public class HitboxTrigger
    {
        public string name;
        public int frame;
        //public Hitbox hitbox;
    }

    [System.Serializable]
    public class HurtboxTrigger
    {
        public string name;
        public int frame;
    }

    [System.Serializable]
    public class Animation
    {
        public string name;
        public Frames[] frames;
        
        public AnimationTrigger[] triggers;
        public NextAnimationTrigger[] nextAnimationTriggers;
        public SoundTrigger[] soundTriggers;
        public HitboxTrigger[] hitboxTriggers;
        public HurtboxTrigger[] hurtboxTriggers;
    }

    public SpriteRenderer spriteRenderer;
    public Color sprColor;
    public SFXPlayer sfxPlayer { get; protected set; }
    public Animation[] animations;

    public bool playing { get; protected set; }
    public Animation currentAnimation { get; protected set; }
    public int currentFrame { get; protected set; }
    public bool loop { get; protected set; }
    private bool disableOn = false;

    public string playAnimationOnStart;
    public bool loopOnStart = true;
    protected Coroutine lastPlay = null;
    protected HurtboxSet hurtboxSet;
    protected HitboxCollection hitboxCollection;
    protected HurtboxCollection hurtboxCollection;

    protected virtual void Awake()
    {
        sfxPlayer = GameObject.Find("SFX Player").GetComponent<SFXPlayer>();
        if (!spriteRenderer)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            sprColor = spriteRenderer.color;
        }
        hitboxCollection = HitboxCollection.instance;
        hurtboxCollection = HurtboxCollection.instance;
        //m_AnimEvent = new UnityEvent();
    }

    protected void OnEnable()
    {
        if (playAnimationOnStart != "")
        {
            spriteRenderer.color = sprColor;
            if (loopOnStart)
            {
                Play(playAnimationOnStart);
            } else
            {
                Play(playAnimationOnStart, false, 0);
            }
        }
    }

    protected void OnDisable()
    {
        playing = false;
        currentAnimation = null;
    }

    public void Play(string name, bool loop = true, int startFrame = 0)
    {
        Animation animation = GetAnimation(name);
        if (animation != null)
        {
            if (animation != currentAnimation)
            {
                ForcePlay(name, loop, startFrame);
            }
        } else
        {
            Debug.LogWarning("could not find animation: " + name);
        }
    }

    public virtual void ForcePlay(string name, bool loop = true, int startFrame = 0)
    {
        Animation animation = GetAnimation(name);
        if (animation != null)
        {
            this.loop = loop;
            currentAnimation = animation;
            playing = true;
            currentFrame = startFrame;
            spriteRenderer.sprite = animation.frames[currentFrame].frame;
            if (lastPlay != null)
            {
                StopCoroutine(lastPlay);
            }
            lastPlay = StartCoroutine(PlayAnimation(currentAnimation));
        }
    }

    public void SlipPlay(string name, int wantFrame, params string[] otherNames)
    {
        for (int i = 0; i < otherNames.Length; i++)
        {
            if (currentAnimation != null && currentAnimation.name == otherNames[i])
            {
                Play(name, true, currentFrame);
                break;
            }
        }
        Play(name, true, wantFrame);
    }

    public bool IsPlaying(string name)
    {
        return (currentAnimation != null && currentAnimation.name == name);
    }

    public bool IsPlayingNull()
    {
        return (currentAnimation == null);
    }

    public Animation GetAnimation(string name)
    {
        for (int i = 0; i < animations.Length; i++)
        {
            if (animations[i].name == name)
            {
                return animations[i];
            }
        }
        /*foreach (Animation animation in animations)
        {
            if (animation.name == name)
            {
                return animation;
            }
        }*/
        return null;
    }

    protected virtual IEnumerator PlayAnimation(Animation animation)
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
                    NextHitbox(animation);
                    NextHurtbox(animation);
                    NextTrigger(animation);
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
                spriteRenderer.sprite = animation.frames[currentFrame].frame;
                spriteRenderer.color = sprColor;
            } else
            {
                yield return null;
            }
        }

        if (!loop && currentFrame == animation.frames.Length - 1)
        {
            NextTrigger(animation);
        }
        currentAnimation = null;
        if (disableOn)
        {
            this.gameObject.SetActive(false);
        }

    }

    protected void NextFrame(Animation animation)
    {
        currentFrame++;
        /*for (int i = 0; i < currentAnimation.triggers.Length; i++)
        {
            if (currentAnimation.triggers[i].frame == currentFrame)
            {
                gameObject.transform.parent.gameObject.SendMessage(currentAnimation.triggers[i].name, currentAnimation.triggers[i].value, SendMessageOptions.DontRequireReceiver);
            }
        }*/
        /*foreach (AnimationTrigger animationTrigger in currentAnimation.triggers)
        {
            if (animationTrigger.frame == currentFrame)
            {
                gameObject.SendMessageUpwards(animationTrigger.name);
            }
        }*/

        if (currentFrame >= animation.frames.Length)
        {
            if (loop)
            {
                currentFrame = 0;
            } else
            {
                currentFrame = animation.frames.Length - 1;
            }
        }
    }

    protected bool NextTrigger(Animation animation)
    {
        for (int i = 0; i < currentAnimation.triggers.Length; i++)
        {
            if (currentAnimation.triggers[i].frame == currentFrame)
            {
                //stringFunctionToUnityAction(this, currentAnimation.triggers[i].name);
                //m_AnimEvent.AddListener(DisableSprite);
                //m_AnimEvent.Invoke();
                //m_AnimEvent.RemoveAllListeners();
                currentAnimation.triggers[i].m_AnimationEventTrigger.Invoke(currentAnimation.triggers[i].name);
                currentAnimation.triggers[i].m_AnimationEventTriggerFloat.Invoke(currentAnimation.triggers[i].value);
                //UseTrigger(currentAnimation.triggers[i].name, currentAnimation.triggers[i].value);
            }
        }
        /*for (int i = 0; i < currentAnimation.nextAnimationTriggers.Length; i++)
        {
            if (currentAnimation.nextAnimationTriggers[i].frame == currentFrame)
            {
                Play(currentAnimation.nextAnimationTriggers[i].name, currentAnimation.nextAnimationTriggers[i].loop, currentAnimation.nextAnimationTriggers[i].startFrame);
                return true;
            }
        }*/
        return false;
    }

    protected bool NextAnimation(Animation animation)
    {
        for (int i = 0; i < currentAnimation.nextAnimationTriggers.Length; i++)
        {
            if (currentAnimation.nextAnimationTriggers[i].frame == currentFrame)
            {
                Play(currentAnimation.nextAnimationTriggers[i].name, currentAnimation.nextAnimationTriggers[i].loop, currentAnimation.nextAnimationTriggers[i].startFrame);
                return true;
            }
        }
        return false;
    }

    protected void NextSound(Animation animation)
    {
        for (int i = 0; i < currentAnimation.soundTriggers.Length; i++)
        {
            if (currentAnimation.soundTriggers[i].frame == currentFrame)
            {
                StartCoroutine(PlaySound(currentAnimation.soundTriggers[i]));
            }
        }
    }

    protected void NextHitbox(Animation animation)
    {
        for (int i = 0; i < currentAnimation.hitboxTriggers.Length; i++)
        {
            if (currentAnimation.hitboxTriggers[i].frame == currentFrame)
            {
                for (int j = 0; j < hitboxCollection.hitboxes.Count; j++)
                {
                    if (gameObject.transform.root.gameObject == hitboxCollection.hitboxes[j].owner && currentAnimation.hitboxTriggers[i].name == hitboxCollection.hitboxes[j].hitbox.hitboxData.name)
                    {
                        hitboxCollection.hitboxes[j].hitbox.gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    protected void NextHurtbox(Animation animation)
    {
        for (int i = 0; i < currentAnimation.hurtboxTriggers.Length; i++)
        {
            if (currentAnimation.hurtboxTriggers[i].frame == currentFrame)
            {
                for (int j = 0; j < hurtboxCollection.hurtboxes.Count; j++)
                {
                    if (gameObject.transform.root.gameObject == hurtboxCollection.hurtboxes[j].owner && currentAnimation.hurtboxTriggers[i].name == hurtboxCollection.hurtboxes[j].hurtbox.hurtboxData.name)
                    {
                        hurtboxCollection.hurtboxes[j].hurtbox.gameObject.SetActive(true);
                        hurtboxCollection.hurtboxes[j].hurtbox.linkAnimation(gameObject, currentAnimation.name, true);
                    }
                }
            }
        }
    }

    /*public void UseTrigger(string name, float value)
    {
        switch (name)
        {
            case "Disable":
                gameObject.SetActive(false);
                break;
            case "Hide":
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, value);
                break;
            default:
                break;
        }
    }*/

    public virtual int GetFacing()
    {
        return (int)Mathf.Sign(spriteRenderer.transform.localScale.x);
    }

    public virtual void FlipTo(float dir)
    {
        if (dir < 0f)
        {
            spriteRenderer.transform.localScale = new Vector3(-1f, 1f, 1f);
        } else
        {
            spriteRenderer.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    public virtual void FlipTo(Vector3 position)
    {
        float diff = position.x - transform.position.x;
        if (diff < 0f)
        {
            spriteRenderer.transform.localScale = new Vector3(-1f, 1f, 1f);
        } else
        {
            spriteRenderer.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    public IEnumerator PlaySound (SoundTrigger soundTrigger)
    {
        int timer = 0;
        int delay = soundTrigger.delay;
        while (timer < delay)
        {
            timer++;
            yield return 0f;
        }
        sfxPlayer.PlaySound(soundTrigger.name);
        yield return null;
    }

    /*UnityAction stringFunctionToUnityAction(object target, string functionName)
    {
        UnityAction action = (UnityAction)System.Delegate.CreateDelegate(typeof(UnityAction), target, functionName);
        return action;
    }*/

    public void DisableSpriteWhenDone(string yes)
    {
        disableOn = true;
    }

    public void alterColor(Color clr)
    {
        sprColor = clr;
        spriteRenderer.color = sprColor;
    }

    public void setAlpha(float value)
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, value);
    }
}
