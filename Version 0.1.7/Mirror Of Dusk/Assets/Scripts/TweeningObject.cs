using System;
using System.Collections;
using UnityEngine;

public class TweeningObject : MonoBehaviour
{
    private Transform _transform;
    private RectTransform _rectTransform;

    protected bool ignoreGlobalTime;
    protected bool ignoreTime;

    public delegate void TweenUpdateHandler(float value);
    public delegate void TweenEndCompleteHandler();

    public TweeningObject.TweenEndCompleteHandler updateTweenEnd;

    public Transform baseTransform
    {
        get { return base.transform; }
    }

    public new Transform transform
    {
        get
        {
            if (this._transform == null)
                this._transform = this.baseTransform;
            return this._transform;
        }
    }

    public RectTransform baseRectTransform
    {
        get { return base.transform as RectTransform; }
    }

    public RectTransform rectTransform
    {
        get
        {
            if (this._rectTransform == null)
                this._rectTransform = this.baseRectTransform;
            return this._rectTransform;
        }
    }

    protected virtual void Awake()
    {
        base.useGUILayout = false;
    }

    protected virtual void Reset()
    {
    }

    public virtual T InstantiatePrefab<T>() where T : MonoBehaviour
    {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(base.gameObject);
        gameObject.name = gameObject.name.Replace("(Clone)", string.Empty);
        return gameObject.GetComponent<T>();
    }

    public Coroutine FrameDelayedCallback(Action callback, int frames)
    {
        return base.StartCoroutine(this.frameDelayedCallback_cr(callback, frames));
    }

    public IEnumerator frameDelayedCallback_cr(Action callback, int frames)
    {
        for (int i = 0; i < frames; i++)
        {
            yield return null;
        }
        if (callback != null)
        {
            callback();
        }
        yield break;
    }

    protected float LocalDeltaTime
    {
        get
        {
            if (!this.ignoreTime)
            {
                if (this.ignoreGlobalTime)
                {
                    return Time.deltaTime;
                }
                //return MirrorOfDuskTime.Delta[this.timeLayer];
                return Time.deltaTime;
            }
            return 1f;
        }
    }

    public Coroutine TweenValue(float start, float end, float time, EaseUtils.EaseType ease, TweeningObject.TweenUpdateHandler updateCallback)
    {
        if (this.ignoreTime)
        {
            return base.StartCoroutine(this.tweenValueFrame_cr(start, end, time, ease, updateCallback));
        } else
        {
            return base.StartCoroutine(this.tweenValue_cr(start, end, time, ease, updateCallback));
        }
    }

    protected IEnumerator tweenValue_cr(float start, float end, float time, EaseUtils.EaseType ease, TweeningObject.TweenUpdateHandler updateCallback)
    {
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            if (updateCallback != null)
            {
                updateCallback(EaseUtils.Ease(ease, start, end, val));
            }
            t += this.LocalDeltaTime;
            yield return null;
        }
        if (updateCallback != null)
        {
            updateCallback(end);
        }
        if (updateTweenEnd != null)
        {
            updateTweenEnd();
        }
        yield return null;
        yield break;
    }

    protected IEnumerator tweenValueFrame_cr(float start, float end, float time, EaseUtils.EaseType ease, TweeningObject.TweenUpdateHandler updateCallback)
    {
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            if (updateCallback != null)
            {
                updateCallback(EaseUtils.Ease(ease, start, end, val));
            }
            t += 1f;
            yield return null;
        }
        if (updateCallback != null)
        {
            updateCallback(end);
        }
        if (updateTweenEnd != null)
        {
            updateTweenEnd();
        }
        yield return null;
        yield break;
    }

    public Coroutine TweenPositionX(float start, float end, float time, EaseUtils.EaseType ease)
    {
        if (this.ignoreTime)
        {
            return base.StartCoroutine(this.tweenPositionXFrame_cr(start, end, time, ease, null));
        } else
        {
            return base.StartCoroutine(this.tweenPositionX_cr(start, end, time, ease, null));
        }
    }
    
    public Coroutine TweenPositionX(float start, float end, float time, EaseUtils.EaseType ease, TweeningObject.TweenUpdateHandler updateCallback)
    {
        if (this.ignoreTime)
        {
            return base.StartCoroutine(this.tweenPositionXFrame_cr(start, end, time, ease, updateCallback));
        }
        else
        {
            return base.StartCoroutine(this.tweenPositionX_cr(start, end, time, ease, updateCallback));
        }
    }

    private IEnumerator tweenPositionX_cr(float start, float end, float time, EaseUtils.EaseType ease, TweeningObject.TweenUpdateHandler updateCallback = null)
    {
        this.transform.SetPosition(new float?(start), null, null);
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            this.transform.SetPosition(new float?(EaseUtils.Ease(ease, start, end, val)), null, null);
            if (updateCallback != null)
            {
                updateCallback(val);
            }
            t += this.LocalDeltaTime;
            yield return null;
        }
        this.transform.SetPosition(new float?(end), null, null);
        if (updateCallback != null)
        {
            updateCallback(1f);
        }
        if (updateTweenEnd != null)
        {
            updateTweenEnd();
        }
        yield return null;
        yield break;
    }

    private IEnumerator tweenPositionXFrame_cr(float start, float end, float time, EaseUtils.EaseType ease, TweeningObject.TweenUpdateHandler updateCallback = null)
    {
        this.transform.SetPosition(new float?(start), null, null);
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            this.transform.SetPosition(new float?(EaseUtils.Ease(ease, start, end, val)), null, null);
            if (updateCallback != null)
            {
                updateCallback(val);
            }
            t += 1f;
            yield return null;
        }
        this.transform.SetPosition(new float?(end), null, null);
        if (updateCallback != null)
        {
            updateCallback(1f);
        }
        if (updateTweenEnd != null)
        {
            updateTweenEnd();
        }
        yield return null;
        yield break;
    }

    public Coroutine TweenScale(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease)
    {
        if (this.ignoreTime)
        {
            return base.StartCoroutine(this.tweenScaleFrame_cr(start, end, time, ease, null));
        }
        else
        {
            return base.StartCoroutine(this.tweenScale_cr(start, end, time, ease, null));
        }
    }

    public Coroutine TweenScale(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease, TweeningObject.TweenUpdateHandler updateCallback)
    {
        if (this.ignoreTime)
        {
            return base.StartCoroutine(this.tweenScaleFrame_cr(start, end, time, ease, updateCallback));
        }
        else
        {
            return base.StartCoroutine(this.tweenScale_cr(start, end, time, ease, updateCallback));
        }
    }

    private IEnumerator tweenScale_cr(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease, TweeningObject.TweenUpdateHandler updateCallback = null)
    {
        this.transform.SetScale(new float?(start.x), new float?(start.y), null);
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            float x = EaseUtils.Ease(ease, start.x, end.x, val);
            float y = EaseUtils.Ease(ease, start.y, end.y, val);
            this.transform.SetScale(new float?(x), new float?(y), null);
            if (updateCallback != null)
            {
                updateCallback(val);
            }
            t += this.LocalDeltaTime;
            yield return null;
        }
        this.transform.SetScale(new float?(end.x), new float?(end.y), null);
        if (updateCallback != null)
        {
            updateCallback(1f);
        }
        if (updateTweenEnd != null)
        {
            updateTweenEnd();
        }
        yield return null;
        yield break;
    }

    private IEnumerator tweenScaleFrame_cr(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease, TweeningObject.TweenUpdateHandler updateCallback = null)
    {
        this.transform.SetScale(new float?(start.x), new float?(start.y), null);
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            float x = EaseUtils.Ease(ease, start.x, end.x, val);
            float y = EaseUtils.Ease(ease, start.y, end.y, val);
            this.transform.SetScale(new float?(x), new float?(y), null);
            t += 1f;
            yield return null;
        }
        this.transform.SetScale(new float?(end.x), new float?(end.y), null);
        if (updateTweenEnd != null)
        {
            updateTweenEnd();
        }
        yield return null;
        yield break;
    }

    public new virtual void StopAllCoroutines()
    {
        base.StopAllCoroutines();
    }
}
