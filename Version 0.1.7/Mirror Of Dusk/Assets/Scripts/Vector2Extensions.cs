using System;
using UnityEngine;

public static class Vector2Extensions
{
    public static Vector2 Set(this Vector2 v, float? x = null, float? y = null)
    {
        Vector2 result = v;
        if (x != null)
        {
            result.x = x.Value;
        }
        if (y != null)
        {
            result.y = y.Value;
        }
        return result;
    }
}
