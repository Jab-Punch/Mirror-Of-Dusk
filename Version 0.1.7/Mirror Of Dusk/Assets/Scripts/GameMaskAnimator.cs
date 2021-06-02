using System;
using System.Collections;
using UnityEngine;

public class GameMaskAnimator : MonoBehaviour
{
    public bool Animating;
    public bool AnimateOnStart = false;
    public bool disableOnStart = false;
    //public bool Loop;
    public SpriteMask SprMask;
    //public SpriteSet[] Sprites;
    public int FrameRate = 60;
    private int _currentStateIndex;
    //private float _lastRefreshTime;

    public State[] States;

    [Serializable]
    public class State
    {
        public string name;
        public int id;
        public GameSpriteAnimation sprites;
        public bool loop;
        [NonSerialized] public int currentFrame = 0;
        [NonSerialized] public int frameCount = 0;
        public bool disableOnFinish = false;
        public string TransitionState;
    }

    public string CurrentState
    {
        get
        {
            return this.States[_currentStateIndex].name;
        }
    }

    /*[Serializable]
    public class SpriteSet
    {
        public Sprite _sprite;
        public int frameDelay = 1;
        [NonSerialized] public int frameCount = 0;
    }*/

    public void ResetAnimation()
    {
        if (this.States.Length > 0)
        {
            this._currentStateIndex = 0;
            //this.Sprites[this._currentSpriteIndex].frameCount = this.Sprites[this._currentSpriteIndex].frameDelay;
        }
    }

    private void OnEnable()
    {
        this.ResetAnimation();
        if (AnimateOnStart)
        {
            Play(States[this._currentStateIndex]);
        }
    }

    private void OnDisable()
    {
        this.ResetAnimation();
    }

    void Update()
    {
        /*if (!this.Loop && this._currentSpriteIndex >= this.Sprites.Length - 1)
        {
            return;
        }*/
        /*if (this.Animating && Time.time >= this._lastRefreshTime + 1f / (float)this.FrameRate)
        {
            this._currentSpriteIndex++;
            if (this._currentSpriteIndex >= this.Sprites.Length)
            {
                this._currentSpriteIndex = 0;
            }
            this.SprRenderer.sprite = this.Sprites[this._currentSpriteIndex];
            this._lastRefreshTime = Time.time;
        }*/
        /*if (this.Animating)
        {
            if (CountSpriteFrame(ref this.Sprites[this._currentSpriteIndex].frameCount) <= 0)
            {
                this._currentSpriteIndex++;
                if (this._currentSpriteIndex >= this.Sprites.Length)
                {
                    this._currentSpriteIndex = 0;
                }
                this.Sprites[this._currentSpriteIndex].frameCount = this.Sprites[this._currentSpriteIndex].frameDelay;
                this.SprRenderer.sprite = this.Sprites[this._currentSpriteIndex]._sprite;
            }
        }*/
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
        if (!SprMask.enabled)
        {
            SprMask.enabled = true;
        }
        state.currentFrame = startFrame;
        while (state.loop || state.currentFrame < state.sprites._sequence.Length)
        {
            int timer = 0;
            this.SprMask.sprite = (state.sprites._sequence[state.currentFrame].spriteKey != -1) ? state.sprites._sprite[state.sprites._sequence[state.currentFrame].spriteKey] : null;
            state.frameCount = state.sprites._sequence[state.currentFrame].delay;
            while (timer < state.frameCount)
            {
                for (int s = 0; s < state.sprites._sounds.Length; s++)
                {
                    if (state.sprites._sounds[s].key == state.sprites._sequence[state.currentFrame].key && state.sprites._sounds[s].delay == timer)
                    {
                        AudioManager.Play(state.sprites._sounds[s].sound);
                    }
                }
                timer += 1;
                yield return 0f;
            }
            for (int s = 0; s < state.sprites._sounds.Length; s++)
            {
                if (state.sprites._sounds[s].key == state.sprites._sequence[state.currentFrame].key && state.sprites._sounds[s].delay == timer)
                {
                    AudioManager.Play(state.sprites._sounds[s].sound);
                }
            }
            yield return 0f;
            state.currentFrame++;
            if (state.loop)
            {
                state.currentFrame = (state.currentFrame + state.sprites._sequence.Length) % state.sprites._sequence.Length;
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
        if (state.disableOnFinish)
        {
            this.SprMask.enabled = false;
            yield break;
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

    private int CountSpriteFrame(ref int sprFrame)
    {
        return sprFrame--;
    }
}
