using System;
using Rewired;
using UnityEngine;

public class CharacterSelectCursorInput : NewPlayerInput
{
    //private float prevLenX = 0f;
    //private float prevLenY = 0f;

    protected override void Awake()
    {
        base.Awake();
        //prevLenX = 0f;
        //prevLenY = 0f;
    }

    public float GetAxisIntX(NewPlayerInput.Axis axis, bool crampedDiagonal = false)
    {
        Vector2 vector = new Vector2(this.actions.GetAxis(20), this.actions.GetAxis(21));
        float lenX = (float)Math.Sqrt(vector.x * vector.x);
        float num = 0.3f;
        if (lenX < 0.005f || lenX < prevLenX - 0.05f)
        {
            prevLenX = lenX;
            return 0;
        }
        prevLenX = lenX;
        float num2 = vector.x;
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

    public float GetAxisIntY(NewPlayerInput.Axis axis, bool crampedDiagonal = false)
    {
        Vector2 vector = new Vector2(this.actions.GetAxis(20), this.actions.GetAxis(21));
        float lenY = (float)Math.Sqrt(vector.y * vector.y);
        float num = 0.3f;
        if (lenY < 0.005f || lenY < prevLenY - 0.05f)
        {
            prevLenY = lenY;
            return 0;
        }
        prevLenY = lenY;
        float num2 = vector.y;
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


}
