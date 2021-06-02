using System;
using UnityEngine;

public static class MathUtils
{
    public static float GetPercentage(float min, float max, float t)
    {
        return (t - min) / (max - min);
    }
    
    public static int PlusOrMinus()
    {
        return (UnityEngine.Random.value <= 0.5f) ? -1 : 1;
    }
    
    public static float ExpRandom(float mean)
    {
        return -Mathf.Log(UnityEngine.Random.Range(0f, 1f)) * mean;
    }
    
    public static bool RandomBool()
    {
        return UnityEngine.Random.value > 0.5f;
    }
    
    public static Vector2 RandomPointInUnitCircle()
    {
        return MathUtils.AngleToDirection(UnityEngine.Random.Range(0f, 360f)) * Mathf.Sqrt(UnityEngine.Random.Range(0f, 1f));
    }
    
    public static float DirectionToAngle(Vector2 direction)
    {
        return Mathf.Atan2(direction.y, direction.x) * 360f / 6.2831855f;
    }
    
    public static Vector2 AngleToDirection(float angle)
    {
        float f = angle * 3.1415927f * 2f / 360f;
        return new Vector2(Mathf.Cos(f), Mathf.Sin(f));
    }
    
    public static bool CircleContains(Vector2 center, float radius, Vector2 point)
    {
        return Mathf.Pow(point.x - center.x, 2f) + Mathf.Pow(point.y - center.y, 2f) < Mathf.Pow(radius, 2f);
    }
}