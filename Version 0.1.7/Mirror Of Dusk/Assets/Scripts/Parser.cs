using System;
using System.Globalization;

public static class Parser
{
    private static NumberFormatInfo InvariantInfo = CultureInfo.InvariantCulture.NumberFormat;

    public static string ToStringInvariant(this int value)
    {
        return value.ToString(Parser.InvariantInfo);
    }
    
    public static string ToStringInvariant(this float value)
    {
        return value.ToString(Parser.InvariantInfo);
    }
    
    public static int IntParse(string s)
    {
        return int.Parse(s, Parser.InvariantInfo);
    }
    
    public static bool IntTryParse(string s, out int result)
    {
        return int.TryParse(s, NumberStyles.Integer, Parser.InvariantInfo, out result);
    }
    
    public static float FloatParse(string s)
    {
        return float.Parse(s, Parser.InvariantInfo);
    }
    
    public static bool FloatTryParse(string s, out float result)
    {
        return float.TryParse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, Parser.InvariantInfo, out result);
    }
    
    public static byte ByteParse(string s)
    {
        return byte.Parse(s, Parser.InvariantInfo);
    }
    
    public static byte ByteParse(string s, NumberStyles style)
    {
        return byte.Parse(s, style, Parser.InvariantInfo);
    }
    
    public static bool ByteTryParse(string s, out byte result)
    {
        return byte.TryParse(s, NumberStyles.Integer, Parser.InvariantInfo, out result);
    }
}
