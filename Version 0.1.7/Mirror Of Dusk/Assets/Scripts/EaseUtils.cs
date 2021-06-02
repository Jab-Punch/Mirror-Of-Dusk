using System;
using UnityEngine;

public class EaseUtils
{
    public enum EaseType
    {
        easeInQuad,
        easeOutQuad,
        easeInOutQuad,
        easeInCubic,
        easeOutCubic,
        easeInOutCubic,
        easeInQuart,
        easeOutQuart,
        easeInOutQuart,
        easeInQuint,
        easeOutQuint,
        easeInOutQuint,
        easeInSine,
        easeOutSine,
        easeInOutSine,
        easeInExpo,
        easeOutExpo,
        easeInOutExpo,
        easeInCirc,
        easeOutCirc,
        easeInOutCirc,
        linear,
        spring,
        easeInBounce,
        easeOutBounce,
        easeInOutBounce,
        easeInBack,
        easeOutBack,
        easeInOutBack,
        easeInElastic,
        easeOutElastic,
        easeInOutElastic,
        punch
    }

    public static float Ease(EaseUtils.EaseType ease, float start, float end, float value)
    {
        switch (ease)
        {
            case EaseUtils.EaseType.easeInQuad:
                return EaseUtils.EaseInQuad(start, end, value);
            case EaseUtils.EaseType.easeOutQuad:
                return EaseUtils.EaseOutQuad(start, end, value);
            case EaseUtils.EaseType.easeInOutQuad:
                return EaseUtils.EaseInOutQuad(start, end, value);
            case EaseUtils.EaseType.easeInCubic:
                return EaseUtils.EaseInCubic(start, end, value);
            case EaseUtils.EaseType.easeOutCubic:
                return EaseUtils.EaseOutCubic(start, end, value);
            case EaseUtils.EaseType.easeInOutCubic:
                return EaseUtils.EaseInOutCubic(start, end, value);
            case EaseUtils.EaseType.easeInQuart:
                return EaseUtils.EaseInQuart(start, end, value);
            case EaseUtils.EaseType.easeOutQuart:
                return EaseUtils.EaseOutQuart(start, end, value);
            case EaseUtils.EaseType.easeInOutQuart:
                return EaseUtils.EaseInOutQuart(start, end, value);
            case EaseUtils.EaseType.easeInQuint:
                return EaseUtils.EaseInQuint(start, end, value);
            case EaseUtils.EaseType.easeOutQuint:
                return EaseUtils.EaseOutQuint(start, end, value);
            case EaseUtils.EaseType.easeInOutQuint:
                return EaseUtils.EaseInOutQuint(start, end, value);
            case EaseUtils.EaseType.easeInSine:
                return EaseUtils.EaseInSine(start, end, value);
            case EaseUtils.EaseType.easeOutSine:
                return EaseUtils.EaseOutSine(start, end, value);
            case EaseUtils.EaseType.easeInOutSine:
                return EaseUtils.EaseInOutSine(start, end, value);
            case EaseUtils.EaseType.easeInExpo:
                return EaseUtils.EaseInExpo(start, end, value);
            case EaseUtils.EaseType.easeOutExpo:
                return EaseUtils.EaseOutExpo(start, end, value);
            case EaseUtils.EaseType.easeInOutExpo:
                return EaseUtils.EaseInOutExpo(start, end, value);
            case EaseUtils.EaseType.easeInCirc:
                return EaseUtils.EaseInCirc(start, end, value);
            case EaseUtils.EaseType.easeOutCirc:
                return EaseUtils.EaseOutCirc(start, end, value);
            case EaseUtils.EaseType.easeInOutCirc:
                return EaseUtils.EaseInOutCirc(start, end, value);
            case EaseUtils.EaseType.spring:
                return EaseUtils.Spring(start, end, value);
            case EaseUtils.EaseType.easeInBounce:
                return EaseUtils.EaseInBounce(start, end, value);
            case EaseUtils.EaseType.easeOutBounce:
                return EaseUtils.EaseOutBounce(start, end, value);
            case EaseUtils.EaseType.easeInOutBounce:
                return EaseUtils.EaseInOutBounce(start, end, value);
            case EaseUtils.EaseType.easeInBack:
                return EaseUtils.EaseInBack(start, end, value);
            case EaseUtils.EaseType.easeOutBack:
                return EaseUtils.EaseOutBack(start, end, value);
            case EaseUtils.EaseType.easeInOutBack:
                return EaseUtils.EaseInOutBack(start, end, value);
            case EaseUtils.EaseType.easeInElastic:
                return EaseUtils.EaseInElastic(start, end, value);
            case EaseUtils.EaseType.easeOutElastic:
                return EaseUtils.EaseOutElastic(start, end, value);
            case EaseUtils.EaseType.easeInOutElastic:
                return EaseUtils.EaseInOutElastic(start, end, value);
        }
        return Mathf.Lerp(start, end, value);
    }

    public static float EaseInOut(EaseUtils.EaseType inEase, EaseUtils.EaseType outEase, float start, float end, float value)
    {
        if (value < 0.5f)
        {
            float value2 = Mathf.Clamp(value * 2f, 0f, 1f);
            float end2 = Mathf.Lerp(start, end, 0.5f);
            return EaseUtils.Ease(inEase, start, end2, value2);
        }
        if (value > 0.5f)
        {
            float value2 = Mathf.Clamp(value * 2f - 1f, 0f, 1f);
            float start2 = Mathf.Lerp(start, end, 0.5f);
            return EaseUtils.Ease(outEase, start2, end, value2);
        }
        return Mathf.Lerp(start, end, 0.5f);
    }

    public static float Linear(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, value);
    }

    public static float Clerp(float start, float end, float value)
    {
        float num = 0f;
        float num2 = 360f;
        float num3 = Mathf.Abs((num2 - num) / 2f);
        float result;
        if (end - start < -num3)
        {
            float num4 = (num2 - start + end) * value;
            result = start + num4;
        }
        else if (end - start > num3)
        {
            float num4 = -(num2 - end + start) * value;
            result = start + num4;
        }
        else
        {
            result = start + (end - start) * value;
        }
        return result;
    }

    public static float Spring(float start, float end, float value)
    {
        value = Mathf.Clamp01(value);
        value = (Mathf.Sin(value * 3.1415927f * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + 1.2f * (1f - value));
        return start + (end - start) * value;
    }

    public static float EaseInQuad(float start, float end, float value)
    {
        end -= start;
        return end * value * value + start;
    }

    public static float EaseOutQuad(float start, float end, float value)
    {
        end -= start;
        return -end * value * (value - 2f) + start;
    }

    public static float EaseInOutQuad(float start, float end, float value)
    {
        value /= 0.5f;
        end -= start;
        if (value < 1f)
        {
            return end / 2f * value * value + start;
        }
        value -= 1f;
        return -end / 2f * (value * (value - 2f) - 1f) + start;
    }

    public static float EaseInCubic(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value + start;
    }

    public static float EaseOutCubic(float start, float end, float value)
    {
        value -= 1f;
        end -= start;
        return end * (value * value * value + 1f) + start;
    }

    public static float EaseInOutCubic(float start, float end, float value)
    {
        value /= 0.5f;
        end -= start;
        if (value < 1f)
        {
            return end / 2f * value * value * value + start;
        }
        value -= 2f;
        return end / 2f * (value * value * value + 2f) + start;
    }

    public static float EaseInQuart(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value * value + start;
    }
    
    public static float EaseOutQuart(float start, float end, float value)
    {
        value -= 1f;
        end -= start;
        return -end * (value * value * value * value - 1f) + start;
    }
    
    public static float EaseInOutQuart(float start, float end, float value)
    {
        value /= 0.5f;
        end -= start;
        if (value < 1f)
        {
            return end / 2f * value * value * value * value + start;
        }
        value -= 2f;
        return -end / 2f * (value * value * value * value - 2f) + start;
    }
    
    public static float EaseInQuint(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value * value * value + start;
    }
    
    public static float EaseOutQuint(float start, float end, float value)
    {
        value -= 1f;
        end -= start;
        return end * (value * value * value * value * value + 1f) + start;
    }
    
    public static float EaseInOutQuint(float start, float end, float value)
    {
        value /= 0.5f;
        end -= start;
        if (value < 1f)
        {
            return end / 2f * value * value * value * value * value + start;
        }
        value -= 2f;
        return end / 2f * (value * value * value * value * value + 2f) + start;
    }

    public static float EaseInSine(float start, float end, float value)
    {
        end -= start;
        Debug.Log("Sine:"+(-end * Mathf.Cos(value / 1f * 1.5707964f) + end + start));
        return -end * Mathf.Cos(value / 1f * 1.5707964f) + end + start;
    }
    
    public static float EaseOutSine(float start, float end, float value)
    {
        end -= start;
        return end * Mathf.Sin(value / 1f * 1.5707964f) + start;
    }
    
    public static float EaseInOutSine(float start, float end, float value)
    {
        end -= start;
        return -end / 2f * (Mathf.Cos(3.1415927f * value / 1f) - 1f) + start;
    }

    public static float EaseInExpo(float start, float end, float value)
    {
        end -= start;
        return end * Mathf.Pow(2f, 10f * (value / 1f - 1f)) + start;
    }
    
    public static float EaseOutExpo(float start, float end, float value)
    {
        end -= start;
        return end * (-Mathf.Pow(2f, -10f * value / 1f) + 1f) + start;
    }
    
    public static float EaseInOutExpo(float start, float end, float value)
    {
        value /= 0.5f;
        end -= start;
        if (value < 1f)
        {
            return end / 2f * Mathf.Pow(2f, 10f * (value - 1f)) + start;
        }
        value -= 1f;
        return end / 2f * (-Mathf.Pow(2f, -10f * value) + 2f) + start;
    }

    public static float EaseInCirc(float start, float end, float value)
    {
        end -= start;
        return -end * (Mathf.Sqrt(1f - value * value) - 1f) + start;
    }
    
    public static float EaseOutCirc(float start, float end, float value)
    {
        value -= 1f;
        end -= start;
        return end * Mathf.Sqrt(1f - value * value) + start;
    }
    
    public static float EaseInOutCirc(float start, float end, float value)
    {
        value /= 0.5f;
        end -= start;
        if (value < 1f)
        {
            return -end / 2f * (Mathf.Sqrt(1f - value * value) - 1f) + start;
        }
        value -= 2f;
        return end / 2f * (Mathf.Sqrt(1f - value * value) + 1f) + start;
    }

    public static float EaseInBounce(float start, float end, float value)
    {
        end -= start;
        float num = 1f;
        return end - EaseUtils.EaseOutBounce(0f, end, num - value) + start;
    }
    
    public static float EaseOutBounce(float start, float end, float value)
    {
        value /= 1f;
        end -= start;
        if (value < 0.36363637f)
        {
            return end * (7.5625f * value * value) + start;
        }
        if (value < 0.72727275f)
        {
            value -= 0.54545456f;
            return end * (7.5625f * value * value + 0.75f) + start;
        }
        if ((double)value < 0.9090909090909091)
        {
            value -= 0.8181818f;
            return end * (7.5625f * value * value + 0.9375f) + start;
        }
        value -= 0.95454544f;
        return end * (7.5625f * value * value + 0.984375f) + start;
    }
    
    public static float EaseInOutBounce(float start, float end, float value)
    {
        end -= start;
        float num = 1f;
        if (value < num / 2f)
        {
            return EaseUtils.EaseInBounce(0f, end, value * 2f) * 0.5f + start;
        }
        return EaseUtils.EaseOutBounce(0f, end, value * 2f - num) * 0.5f + end * 0.5f + start;
    }
    
    public static float EaseInBack(float start, float end, float value)
    {
        end -= start;
        value /= 1f;
        float num = 1.70158f;
        return end * value * value * ((num + 1f) * value - num) + start;
    }
    
    public static float EaseOutBack(float start, float end, float value)
    {
        float num = 1.70158f;
        end -= start;
        value = value / 1f - 1f;
        return end * (value * value * ((num + 1f) * value + num) + 1f) + start;
    }
    
    public static float EaseInOutBack(float start, float end, float value)
    {
        float num = 1.70158f;
        end -= start;
        value /= 0.5f;
        if (value < 1f)
        {
            num *= 1.525f;
            return end / 2f * (value * value * ((num + 1f) * value - num)) + start;
        }
        value -= 2f;
        num *= 1.525f;
        return end / 2f * (value * value * ((num + 1f) * value + num) + 2f) + start;
    }

    public static float EaseInElastic(float start, float end, float value)
    {
        end -= start;
        float num = 1f;
        float num2 = num * 0.3f;
        float num3 = 0f;
        if (value == 0f)
        {
            return start;
        }
        if ((value /= num) == 1f)
        {
            return start + end;
        }
        float num4;
        if (num3 == 0f || num3 < Mathf.Abs(end))
        {
            num3 = end;
            num4 = num2 / 4f;
        }
        else
        {
            num4 = num2 / 6.2831855f * Mathf.Asin(end / num3);
        }
        return -(num3 * Mathf.Pow(2f, 10f * (value -= 1f)) * Mathf.Sin((value * num - num4) * 6.2831855f / num2)) + start;
    }
    
    public static float EaseOutElastic(float start, float end, float value)
    {
        end -= start;
        float num = 1f;
        float num2 = num * 0.3f;
        float num3 = 0f;
        if (value == 0f)
        {
            return start;
        }
        if ((value /= num) == 1f)
        {
            return start + end;
        }
        float num4;
        if (num3 == 0f || num3 < Mathf.Abs(end))
        {
            num3 = end;
            num4 = num2 / 4f;
        }
        else
        {
            num4 = num2 / 6.2831855f * Mathf.Asin(end / num3);
        }
        return num3 * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value * num - num4) * 6.2831855f / num2) + end + start;
    }
    
    public static float EaseInOutElastic(float start, float end, float value)
    {
        end -= start;
        float num = 1f;
        float num2 = num * 0.3f;
        float num3 = 0f;
        if (value == 0f)
        {
            return start;
        }
        if ((value /= num / 2f) == 2f)
        {
            return start + end;
        }
        float num4;
        if (num3 == 0f || num3 < Mathf.Abs(end))
        {
            num3 = end;
            num4 = num2 / 4f;
        }
        else
        {
            num4 = num2 / 6.2831855f * Mathf.Asin(end / num3);
        }
        if (value < 1f)
        {
            return -0.5f * (num3 * Mathf.Pow(2f, 10f * (value -= 1f)) * Mathf.Sin((value * num - num4) * 6.2831855f / num2)) + start;
        }
        return num3 * Mathf.Pow(2f, -10f * (value -= 1f)) * Mathf.Sin((value * num - num4) * 6.2831855f / num2) * 0.5f + end + start;
    }

    public static float Punch(float amplitude, float value)
    {
        if (value == 0f)
        {
            return 0f;
        }
        if (value == 1f)
        {
            return 0f;
        }
        float num = 0.3f;
        float num2 = num / 6.2831855f * Mathf.Asin(0f);
        return amplitude * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value * 1f - num2) * 6.2831855f / num);
    }
}
