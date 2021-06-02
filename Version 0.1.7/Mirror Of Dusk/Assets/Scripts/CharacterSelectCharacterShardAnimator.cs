using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectCharacterShardAnimator : MonoBehaviour
{
    public SpriteRenderer BaseRenderer;
    public SpriteMask SprMask;
    public SpriteRenderer IconRenderer;
    public SpriteRenderer IconRenderer2;
    public SpriteRenderer BackgroundRenderer0;
    public SpriteRenderer BackgroundRenderer1;
    public int FrameRate = 60;
    private int _currentStateIndex;
    public ShardSpriteData spriteData;

    public State[] States;

    [Serializable]
    public class State
    {
        public string name;
        public ShardAnimation sprites;
        public int loopAmount = 1;
        [NonSerialized] public int currentFrame = 0;
        [NonSerialized] public int currentloop = 0;
        [NonSerialized] public int frameCount = 0;
        public string TransitionState;
    }

    [Serializable]
    public class ShardSpriteData
    {
        public Sprite[] _sprite;
        public Sprite[] _spriteMask;
        public float[] _rotation;
        public bool[] _reflection;
    }

    [Serializable]
    public class ShardAnimation
    {
        public Sequence[] _sequence;

        [Serializable]
        public class Sequence
        {
            public int spriteKey;
            public int delay;
        }
    }

    public string CurrentState
    {
        get
        {
            return this.States[_currentStateIndex].name;
        }
    }

    public void ResetAnimation()
    {
        if (this.States.Length > 0)
        {
            this._currentStateIndex = 0;
        }
    }

    private void OnEnable()
    {
        this.ResetAnimation();
        Play(States[this._currentStateIndex]);
    }

    private void OnDisable()
    {
        this.ResetAnimation();
    }

    public void Play(string state, Action completionHandler = null, bool completionHandlerBreak = false)
    {
        for (int i = 0; i < States.Length; i++)
        {
            if (state == States[i].name)
            {
                _currentStateIndex = i;
                Play(States[i], 0, completionHandler, completionHandlerBreak);
                return;
            }
        }
    }

    public void Play(string state, int startFrame, Action completionHandler = null, bool completionHandlerBreak = false)
    {
        for (int i = 0; i < States.Length; i++)
        {
            if (state == States[i].name)
            {
                _currentStateIndex = i;
                Play(States[i], startFrame, completionHandler, completionHandlerBreak);
                return;
            }
        }
    }

    public void Play(State state, int startFrame = 0, Action completionHandler = null, bool completionHandlerBreak = false)
    {
        this.StopAllCoroutines();
        this.StartCoroutine(playAnimation_cr(state, startFrame, completionHandler, completionHandlerBreak));
    }

    public IEnumerator playAnimation_cr(State state, int startFrame, Action completionHandler = null, bool completionHandlerBreak = false)
    {
        if (!BaseRenderer.enabled)
            BaseRenderer.enabled = true;
        if (!SprMask.enabled)
            SprMask.enabled = true;
        if (!IconRenderer.enabled)
            IconRenderer.enabled = true;
        if (!IconRenderer2.enabled)
            IconRenderer2.enabled = true;
        if (!BackgroundRenderer0.enabled)
            BackgroundRenderer0.enabled = true;
        if (!BackgroundRenderer1.enabled)
            BackgroundRenderer1.enabled = true;
        state.currentFrame = startFrame;
        state.currentloop = state.loopAmount;
        while (state.currentloop > 0)
        {
            int timer = 0;
            if (state.sprites._sequence[state.currentFrame].spriteKey != -1)
            {
                this.BaseRenderer.sprite = spriteData._sprite[state.sprites._sequence[state.currentFrame].spriteKey];
                if (spriteData._spriteMask[state.sprites._sequence[state.currentFrame].spriteKey] != null)
                {
                    this.SprMask.sprite = spriteData._spriteMask[state.sprites._sequence[state.currentFrame].spriteKey];
                } else
                {
                    this.SprMask.sprite = null;
                }
                this.IconRenderer.enabled = (!spriteData._reflection[state.sprites._sequence[state.currentFrame].spriteKey]) ? true : false;
                this.IconRenderer2.enabled = (spriteData._reflection[state.sprites._sequence[state.currentFrame].spriteKey]) ? true : false;
                this.IconRenderer.transform.rotation = new Quaternion(0, rotIcon(spriteData._rotation[state.sprites._sequence[state.currentFrame].spriteKey]), 0, 1);
                this.IconRenderer2.transform.rotation = new Quaternion(0, rotIcon(spriteData._rotation[state.sprites._sequence[state.currentFrame].spriteKey]), 0, 1);
                this.BackgroundRenderer0.transform.rotation = new Quaternion(0, rotIcon(spriteData._rotation[state.sprites._sequence[state.currentFrame].spriteKey]), 0, 1);
                this.BackgroundRenderer1.transform.rotation = new Quaternion(0, rotIcon(spriteData._rotation[state.sprites._sequence[state.currentFrame].spriteKey]), 0, 1);
            }
            else
            {
                this.BaseRenderer.sprite = null;
                this.SprMask.sprite = null;
            }
            state.frameCount = state.sprites._sequence[state.currentFrame].delay;
            while (timer < state.frameCount)
            {
                timer += 1;
                yield return 0f;
            }
            yield return 0f;
            state.currentFrame++;
            if (state.currentloop > 0)
            {
                state.currentFrame = (state.currentFrame + state.sprites._sequence.Length) % state.sprites._sequence.Length;
                if (state.currentFrame == 0)
                    state.currentloop--;
            }
        }
        if (completionHandler != null)
        {
            completionHandler();
            if (completionHandlerBreak)
            {
                yield break;
            }
        }
        if (!String.IsNullOrEmpty(state.TransitionState))
        {
            Play(state.TransitionState);
        }
        yield break;
        /*if (!loop && currentFrame == animation.frames.Length - 1)
        {
            NextTrigger(animation);
        }
        currentAnimation = null;
        if (disableOn)
        {
            this.gameObject.SetActive(false);
        }*/
    }

    private float rotIcon(float num)
    {
        float result = ((num / 1000f) * 9f);
        return result;
    }
}
