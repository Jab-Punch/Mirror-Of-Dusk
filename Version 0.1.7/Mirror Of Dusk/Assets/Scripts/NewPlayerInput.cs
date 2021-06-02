using System;
using Rewired;
using UnityEngine;

public class NewPlayerInput : AbstractMB
{
    protected float prevLenX = 0f;
    protected float prevLenY = 0f;

    public enum Axis
    {
        X,
        Y
    }

    protected AbstractPlayerController player;

    public PlayerId playerId { get; private set; }

    public bool IsKO
    {
        get
        {
            return this.player != null && this.player.IsKOed;
        }
    }

    public Player actions { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        prevLenX = 0f;
        prevLenY = 0f;
        this.player = base.GetComponent<AbstractPlayerController>();
    }

    public void Init(PlayerId playerId)
    {
        this.playerId = playerId;
        this.actions = PlayerManager.GetPlayerInput(playerId);
    }

    public float GetAxisInt(NewPlayerInput.Axis axis, bool crampedDiagonal = false)
    {
        
        Vector2 vector = new Vector2(this.actions.GetAxis(0), this.actions.GetAxis(1));
        float magnitude = vector.magnitude;
        float num = (!crampedDiagonal) ? 0.38268f : 0.5f;
        if (magnitude < 0.005f)
        {
            return 0;
        }
        float num2 = ((axis != NewPlayerInput.Axis.X) ? vector.y : vector.x) / magnitude;
        if (num2 > num)
        {
            return 1;
        }
        if (num2 < -num)
        {
            return -1;
        }
        return 0;
    }

    public float GetAxisMenuInt(NewPlayerInput.Axis axis, bool crampedDiagonal = false)
    {
        Vector2 vector = new Vector2(this.actions.GetAxis(20), this.actions.GetAxis(21));
        float axisNum = ((axis != NewPlayerInput.Axis.X) ? vector.y : vector.x);
        float len = (float)Math.Sqrt(axisNum * axisNum);
        float num = 0.3f;
        if (axis == NewPlayerInput.Axis.X)
        {
            if (len < 0.005f || len < prevLenX - 0.05f)
            {
                prevLenX = len;
                return 0;
            }
            prevLenX = len;
        } else
        {
            if (len < 0.005f || len < prevLenY - 0.05f)
            {
                prevLenY = len;
                return 0;
            }
            prevLenY = len;
        }
        float num2 = ((axis != NewPlayerInput.Axis.X) ? vector.y : vector.x);
        if (num2 > num)
        {
            return num2;
        }
        if (num2 < -num)
        {
            return num2;
        }
        return 0;
    }

    public float GetAxis(NewPlayerInput.Axis axis)
    {
        if (axis == NewPlayerInput.Axis.X)
        {
            return this.actions.GetAxis(0);
        }
        return this.actions.GetAxis(1);
    }

    public bool GetButton(MirrorOfDuskButton button)
    {
        return this.actions.GetButton((int)button);
    }

    public bool GetButtonDown(MirrorOfDuskButton button)
    {
        return this.actions.GetButtonDown((int)button);
    }
}
