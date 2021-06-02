using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveStillObject : TweeningObject
{
    protected int pathIndex;
    public float loopRepeatDelay;
    public int startFrameDelay = 0;
    public float speed = 100f;
    public VectorPath path;
    public SpriteRenderer sprite;
    private EaseUtils.EaseType _easeType = EaseUtils.EaseType.linear;
    
    private Vector3 _offset;
    public bool instantStart = true;
    public bool hideUntilStart = false;
    [SerializeField] private TweenStyle tweenStyle;

    private float[] allValues { get; set; }

    public enum TweenStyle
    {
        DirectionX,
        Direction,
        Scale
    }


    // Start is called before the first frame update
    void Start()
    {
        this._offset = base.transform.position;
        this.ignoreTime = true;
        if (hideUntilStart && sprite != null)
            sprite.enabled = false;
        if (instantStart)
        {
            ForceStart();
        }
    }

    public void ForceStart()
    {
        if (tweenStyle == TweenStyle.DirectionX)
        {
            base.StartCoroutine(this.tweenDirectionX_cr());
        }
        else if (tweenStyle == TweenStyle.Direction)
        {
            base.StartCoroutine(this.tweenDirection_cr());
        }
        else if (tweenStyle == TweenStyle.Scale)
        {
            base.StartCoroutine(this.tweenScale_cr());
        }
        
    }

    private float CalculateTime()
    {
        return this.path.Distance / this.speed;
    }

    private void MoveCallback(float value)
    {
        base.transform.position = this._offset + this.path.Lerp(value);
    }

    private IEnumerator tweenDirectionX_cr()
    {
        this._offset = base.transform.position;
        if (startFrameDelay > 0)
        {
            int s = 0;
            while (s < startFrameDelay)
            {
                yield return null;
                s++;
            }
        }
        if (hideUntilStart && sprite != null)
            sprite.enabled = true;
        yield return base.TweenPositionX(0f, 1f, this.speed, this._easeType, new TweeningObject.TweenUpdateHandler(this.MoveCallback));
        //yield return MirrorOfDuskTime.WaitForSeconds(this, this.loopRepeatDelay);
        yield return null;
        yield break;
    }

    private IEnumerator tweenDirection_cr()
    {
        this._offset = base.transform.position;
        if (startFrameDelay > 0)
        {
            int s = 0;
            while (s < startFrameDelay)
            {
                yield return null;
                s++;
            }
        }
        if (hideUntilStart && sprite != null)
            sprite.enabled = true;
        yield return base.TweenValue(0f, 1f, this.speed, this._easeType, new TweeningObject.TweenUpdateHandler(this.MoveCallback));
        //yield return MirrorOfDuskTime.WaitForSeconds(this, this.loopRepeatDelay);
        yield return null;
        yield break;
    }

    private IEnumerator tweenScale_cr()
    {
        this._offset = base.transform.position;
        if (startFrameDelay > 0)
        {
            int s = 0;
            while (s < startFrameDelay)
            {
                yield return null;
                s++;
            }
        }
        if (hideUntilStart && sprite != null)
            sprite.enabled = true;
        yield return base.TweenScale(path.Points[0], path.Points[1], this.speed, this._easeType, new TweeningObject.TweenUpdateHandler(this.MoveCallback));
        //yield return MirrorOfDuskTime.WaitForSeconds(this, this.loopRepeatDelay);
        yield return null;
        yield break;
    }
}
