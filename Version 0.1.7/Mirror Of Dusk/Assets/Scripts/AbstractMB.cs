using System;
using System.Collections;
using UnityEngine;

public abstract class AbstractMB : MonoBehaviour
{
    private Transform _transform;
    private RectTransform _rectTransform;

    /*private Rigidbody _rigidbody;
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;*/
    private SpriteAnimator _animator;

    protected bool ignoreFrameTime;
    protected bool ignoreGlobalTime;
    
    //protected MirrorOfDuskTime.Layer timeLayer;
    
    public delegate void TweenUpdateHandler(float value);

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

    /*public Rigidbody baseRigidbody
    {
        get
        {
            return base.GetComponent<Rigidbody>();
        }
    }

    public Rigidbody rigidbody
    {
        get
        {
            if (this._rigidbody == null)
            {
                this._rigidbody = this.baseRigidbody;
            }
            return this._rigidbody;
        }
    }

    public Rigidbody2D baseRigidbody2D
    {
        get
        {
            return base.GetComponent<Rigidbody2D>();
        }
    }

    public Rigidbody2D rigidbody2D
    {
        get
        {
            if (this._rigidbody2D == null)
            {
                this._rigidbody2D = this.baseRigidbody2D;
            }
            return this._rigidbody2D;
        }
    }

    public Animator baseAnimator
    {
        get
        {
            return base.GetComponent<Animator>();
        }
    }

    public Animator animator
    {
        get
        {
            if (this._animator == null)
            {
                this._animator = this.baseAnimator;
            }
            return this._animator;
        }
    }*/

    public SpriteAnimator baseAnimator
    {
        get
        {
            return base.GetComponent<SpriteAnimator>();
        }
    }

    public SpriteAnimator animator
    {
        get
        {
            if (this._animator == null)
            {
                this._animator = this.baseAnimator;
            }
            return this._animator;
        }
    }

    protected virtual void Awake()
    {
        base.useGUILayout = false;
    }

    protected virtual void Reset()
    {
    }

    /*protected virtual void OnDrawGizmos()
    {
    }
    
    protected virtual void OnDrawGizmosSelected()
    {
    }*/

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

    public Coroutine FrameDelayedCallbackIntOne(Action<int> callback, int param, int frames)
    {
        return base.StartCoroutine(this.frameDelayedCallbackIntOne_cr(callback, param, frames));
    }

    public IEnumerator frameDelayedCallbackIntOne_cr(Action<int> callback, int param, int frames)
    {
        for (int i = 0; i < frames; i++)
        {
            yield return null;
        }
        if (callback != null)
        {
            callback(param);
        }
        yield break;
    }

    public Coroutine FrameDelayedCallbackIntTwo(Action<int, int> callback, int param1, int param2, int frames)
    {
        return base.StartCoroutine(this.frameDelayedCallbackIntTwo_cr(callback, param1, param2, frames));
    }

    public IEnumerator frameDelayedCallbackIntTwo_cr(Action<int, int> callback, int param1, int param2, int frames)
    {
        for (int i = 0; i < frames; i++)
        {
            yield return null;
        }
        if (callback != null)
        {
            callback(param1, param2);
        }
        yield break;
    }

    protected float LocalDeltaTime
    {
        get
        {
            if (this.ignoreFrameTime)
            {
                if (this.ignoreGlobalTime)
                {
                    return Time.deltaTime;
                }
                //return MirrorOfDuskTime.Delta[this.timeLayer];
                return 1f;
            }
            return 1f;
        }
    }

    public Coroutine TweenValue(float start, float end, float time, EaseUtils.EaseType ease, AbstractMB.TweenUpdateHandler updateCallback)
    {
        return base.StartCoroutine(this.tweenValue_cr(start, end, time, ease, updateCallback));
    }
    
    protected IEnumerator tweenValue_cr(float start, float end, float time, EaseUtils.EaseType ease, AbstractMB.TweenUpdateHandler updateCallback)
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
        yield return null;
        yield break;
    }

    public Coroutine TweenScale(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease)
    {
        return base.StartCoroutine(this.tweenScale_cr(start, end, time, ease, null));
    }
    
    public Coroutine TweenScale(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease, AbstractMB.TweenUpdateHandler updateCallback)
    {
        return base.StartCoroutine(this.tweenScale_cr(start, end, time, ease, updateCallback));
    }
    
    private IEnumerator tweenScale_cr(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease, AbstractMB.TweenUpdateHandler updateCallback = null)
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
        yield return null;
        yield break;
    }
    
    public Coroutine TweenPosition(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease)
    {
        return base.StartCoroutine(this.tweenPosition_cr(start, end, time, ease, null));
    }
    
    public Coroutine TweenPosition(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease, AbstractMB.TweenUpdateHandler updateCallback)
    {
        return base.StartCoroutine(this.tweenPosition_cr(start, end, time, ease, updateCallback));
    }
    
    private IEnumerator tweenPosition_cr(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease, AbstractMB.TweenUpdateHandler updateCallback = null)
    {
        this.transform.position = start;
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            float x = EaseUtils.Ease(ease, start.x, end.x, val);
            float y = EaseUtils.Ease(ease, start.y, end.y, val);
            this.transform.SetPosition(new float?(x), new float?(y), new float?(0f));
            if (updateCallback != null)
            {
                updateCallback(val);
            }
            t += this.LocalDeltaTime;
            yield return null;
        }
        this.transform.position = end;
        if (updateCallback != null)
        {
            updateCallback(1f);
        }
        yield return null;
        yield break;
    }

    public Coroutine TweenPositionVec3(Vector3 start, Vector3 end, float time, EaseUtils.EaseType ease)
    {
        return base.StartCoroutine(this.tweenPositionVec3_cr(start, end, time, ease, null));
    }

    public Coroutine TweenPositionVec3(Vector3 start, Vector3 end, float time, EaseUtils.EaseType ease, AbstractMB.TweenUpdateHandler updateCallback)
    {
        return base.StartCoroutine(this.tweenPositionVec3_cr(start, end, time, ease, updateCallback));
    }

    private IEnumerator tweenPositionVec3_cr(Vector3 start, Vector3 end, float time, EaseUtils.EaseType ease, AbstractMB.TweenUpdateHandler updateCallback = null)
    {
        this.transform.position = start;
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            float x = EaseUtils.Ease(ease, start.x, end.x, val);
            float y = EaseUtils.Ease(ease, start.y, end.y, val);
            float z = EaseUtils.Ease(ease, start.z, end.z, val);
            this.transform.SetPosition(new float?(x), new float?(y), new float?(z));
            if (updateCallback != null)
            {
                updateCallback(val);
            }
            t += this.LocalDeltaTime;
            yield return null;
        }
        this.transform.position = end;
        if (updateCallback != null)
        {
            updateCallback(1f);
        }
        yield return null;
        yield break;
    }

    public Coroutine TweenLocalPosition(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease)
    {
        return base.StartCoroutine(this.tweenLocalPosition_cr(start, end, time, ease, null));
    }
    
    public Coroutine TweenLocalPosition(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease, AbstractMB.TweenUpdateHandler updateCallback)
    {
        return base.StartCoroutine(this.tweenLocalPosition_cr(start, end, time, ease, updateCallback));
    }
    
    private IEnumerator tweenLocalPosition_cr(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease, AbstractMB.TweenUpdateHandler updateCallback = null)
    {
        this.transform.localPosition = start;
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            float x = EaseUtils.Ease(ease, start.x, end.x, val);
            float y = EaseUtils.Ease(ease, start.y, end.y, val);
            this.transform.SetLocalPosition(new float?(x), new float?(y), new float?(0f));
            if (updateCallback != null)
            {
                updateCallback(val);
            }
            t += this.LocalDeltaTime;
            yield return null;
        }
        this.transform.localPosition = end;
        if (updateCallback != null)
        {
            updateCallback(1f);
        }
        yield return null;
        yield break;
    }

    public Coroutine TweenPositionX(float start, float end, float time, EaseUtils.EaseType ease)
    {
        return base.StartCoroutine(this.tweenPositionX_cr(start, end, time, ease, null));
    }
    
    public Coroutine TweenPositionX(float start, float end, float time, EaseUtils.EaseType ease, AbstractMB.TweenUpdateHandler updateCallback)
    {
        return base.StartCoroutine(this.tweenPositionX_cr(start, end, time, ease, updateCallback));
    }
    
    private IEnumerator tweenPositionX_cr(float start, float end, float time, EaseUtils.EaseType ease, AbstractMB.TweenUpdateHandler updateCallback = null)
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
        yield return null;
        yield break;
    }
    
    public Coroutine TweenLocalPositionX(float start, float end, float time, EaseUtils.EaseType ease)
    {
        return base.StartCoroutine(this.tweenLocalPositionX_cr(start, end, time, ease, null));
    }
    
    public Coroutine TweenLocalPositionX(float start, float end, float time, EaseUtils.EaseType ease, AbstractMB.TweenUpdateHandler updateCallback)
    {
        return base.StartCoroutine(this.tweenLocalPositionX_cr(start, end, time, ease, updateCallback));
    }
    
    private IEnumerator tweenLocalPositionX_cr(float start, float end, float time, EaseUtils.EaseType ease, AbstractMB.TweenUpdateHandler updateCallback = null)
    {
        this.transform.SetLocalPosition(new float?(start), null, null);
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            this.transform.SetLocalPosition(new float?(EaseUtils.Ease(ease, start, end, val)), null, null);
            if (updateCallback != null)
            {
                updateCallback(val);
            }
            t += this.LocalDeltaTime;
            yield return null;
        }
        this.transform.SetLocalPosition(new float?(end), null, null);
        if (updateCallback != null)
        {
            updateCallback(1f);
        }
        yield return null;
        yield break;
    }
    
    public Coroutine TweenPositionY(float start, float end, float time, EaseUtils.EaseType ease)
    {
        return base.StartCoroutine(this.tweenPositionY_cr(start, end, time, ease, null));
    }
    
    public Coroutine TweenPositionY(float start, float end, float time, EaseUtils.EaseType ease, AbstractMB.TweenUpdateHandler updateCallback)
    {
        return base.StartCoroutine(this.tweenPositionY_cr(start, end, time, ease, updateCallback));
    }
    
    private IEnumerator tweenPositionY_cr(float start, float end, float time, EaseUtils.EaseType ease, AbstractMB.TweenUpdateHandler updateCallback = null)
    {
        this.transform.SetPosition(null, new float?(start), null);
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            this.transform.SetPosition(null, new float?(EaseUtils.Ease(ease, start, end, val)), null);
            if (updateCallback != null)
            {
                updateCallback(val);
            }
            t += this.LocalDeltaTime;
            yield return null;
        }
        this.transform.SetPosition(null, new float?(end), null);
        if (updateCallback != null)
        {
            updateCallback(1f);
        }
        yield return null;
        yield break;
    }
    
    public Coroutine TweenLocalPositionY(float start, float end, float time, EaseUtils.EaseType ease)
    {
        return base.StartCoroutine(this.tweenLocalPositionY_cr(start, end, time, ease, null));
    }
    
    public Coroutine TweenLocalPositionY(float start, float end, float time, EaseUtils.EaseType ease, AbstractMB.TweenUpdateHandler updateCallback)
    {
        return base.StartCoroutine(this.tweenLocalPositionY_cr(start, end, time, ease, updateCallback));
    }
    
    private IEnumerator tweenLocalPositionY_cr(float start, float end, float time, EaseUtils.EaseType ease, AbstractMB.TweenUpdateHandler updateCallback = null)
    {
        this.transform.SetLocalPosition(null, new float?(start), null);
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            this.transform.SetLocalPosition(null, new float?(EaseUtils.Ease(ease, start, end, val)), null);
            if (updateCallback != null)
            {
                updateCallback(val);
            }
            t += this.LocalDeltaTime;
            yield return null;
        }
        this.transform.SetLocalPosition(null, new float?(end), null);
        if (updateCallback != null)
        {
            updateCallback(1f);
        }
        yield return null;
        yield break;
    }
    
    public Coroutine TweenPositionZ(float start, float end, float time, EaseUtils.EaseType ease)
    {
        return base.StartCoroutine(this.tweenPositionZ_cr(start, end, time, ease, null));
    }
    
    public Coroutine TweenPositionZ(float start, float end, float time, EaseUtils.EaseType ease, AbstractMB.TweenUpdateHandler updateCallback)
    {
        return base.StartCoroutine(this.tweenPositionZ_cr(start, end, time, ease, updateCallback));
    }
    
    private IEnumerator tweenPositionZ_cr(float start, float end, float time, EaseUtils.EaseType ease, AbstractMB.TweenUpdateHandler updateCallback = null)
    {
        this.transform.SetPosition(null, null, new float?(start));
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            this.transform.SetPosition(null, null, new float?(EaseUtils.Ease(ease, start, end, val)));
            if (updateCallback != null)
            {
                updateCallback(val);
            }
            t += this.LocalDeltaTime;
            yield return null;
        }
        this.transform.SetPosition(null, null, new float?(end));
        if (updateCallback != null)
        {
            updateCallback(1f);
        }
        yield return null;
        yield break;
    }
    
    public Coroutine TweenLocalPositionZ(float start, float end, float time, EaseUtils.EaseType ease)
    {
        return base.StartCoroutine(this.tweenLocalPositionZ_cr(start, end, time, ease, null));
    }
    
    public Coroutine TweenLocalPositionZ(float start, float end, float time, EaseUtils.EaseType ease, AbstractMB.TweenUpdateHandler updateCallback)
    {
        return base.StartCoroutine(this.tweenLocalPositionZ_cr(start, end, time, ease, updateCallback));
    }
    
    private IEnumerator tweenLocalPositionZ_cr(float start, float end, float time, EaseUtils.EaseType ease, AbstractMB.TweenUpdateHandler updateCallback = null)
    {
        this.transform.SetLocalPosition(null, null, new float?(start));
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            this.transform.SetLocalPosition(null, null, new float?(EaseUtils.Ease(ease, start, end, val)));
            if (updateCallback != null)
            {
                updateCallback(val);
            }
            t += this.LocalDeltaTime;
            yield return null;
        }
        this.transform.SetLocalPosition(null, null, new float?(end));
        if (updateCallback != null)
        {
            updateCallback(1f);
        }
        yield return null;
        yield break;
    }

    public Coroutine TweenRotation2D(float start, float end, float time, EaseUtils.EaseType ease)
    {
        return base.StartCoroutine(this.tweenRotation2D_cr(start, end, time, ease, null));
    }
    
    public Coroutine TweenRotation2D(float start, float end, float time, EaseUtils.EaseType ease, AbstractMB.TweenUpdateHandler updateCallback)
    {
        return base.StartCoroutine(this.tweenRotation2D_cr(start, end, time, ease, updateCallback));
    }
    
    private IEnumerator tweenRotation2D_cr(float start, float end, float time, EaseUtils.EaseType ease, AbstractMB.TweenUpdateHandler updateCallback = null)
    {
        this.transform.SetEulerAngles(null, null, new float?(start));
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            this.transform.SetEulerAngles(null, null, new float?(EaseUtils.Ease(ease, start, end, val)));
            if (updateCallback != null)
            {
                updateCallback(val);
            }
            t += this.LocalDeltaTime;
            yield return null;
        }
        this.transform.SetEulerAngles(null, null, new float?(end));
        if (updateCallback != null)
        {
            updateCallback(1f);
        }
        yield return null;
        yield break;
    }
    
    public Coroutine TweenLocalRotation2D(float start, float end, float time, EaseUtils.EaseType ease)
    {
        return base.StartCoroutine(this.tweenLocalRotation2D_cr(start, end, time, ease, null));
    }
    
    public Coroutine TweenLocalRotation2D(float start, float end, float time, EaseUtils.EaseType ease, AbstractMB.TweenUpdateHandler updateCallback)
    {
        return base.StartCoroutine(this.tweenLocalRotation2D_cr(start, end, time, ease, updateCallback));
    }
    
    private IEnumerator tweenLocalRotation2D_cr(float start, float end, float time, EaseUtils.EaseType ease, AbstractMB.TweenUpdateHandler updateCallback = null)
    {
        this.transform.SetLocalEulerAngles(null, null, new float?(start));
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            this.transform.SetLocalEulerAngles(null, null, new float?(EaseUtils.Ease(ease, start, end, val)));
            if (updateCallback != null)
            {
                updateCallback(val);
            }
            t += this.LocalDeltaTime;
            yield return null;
        }
        this.transform.SetLocalEulerAngles(null, null, new float?(end));
        if (updateCallback != null)
        {
            updateCallback(1f);
        }
        yield return null;
        yield break;
    }
    
    public new virtual void StopAllCoroutines()
    {
        base.StopAllCoroutines();
    }
}
