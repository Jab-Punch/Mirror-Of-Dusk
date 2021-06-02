using System;
using UnityEngine;

// Token: 0x02000275 RID: 629
public static class EnumUtils
{
    // Token: 0x06000795 RID: 1941 RVA: 0x00055982 File Offset: 0x00053D82
    public static T[] GetValues<T>()
    {
        if (!typeof(T).IsEnum)
		{
            throw new ArgumentException("T must be an enum type");
        }
        return (T[])Enum.GetValues(typeof(T));
    }

    // Token: 0x06000796 RID: 1942 RVA: 0x000559B8 File Offset: 0x00053DB8
    public static string[] GetValuesAsStrings<T>()
    {
        T[] values = EnumUtils.GetValues<T>();
        string[] array = new string[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            array[i] = values[i].ToString();
        }
        return array;
    }

    // Token: 0x06000797 RID: 1943 RVA: 0x00055A00 File Offset: 0x00053E00
    public static int GetCount<T>()
    {
        return EnumUtils.GetValues<T>().Length;
    }

    // Token: 0x06000798 RID: 1944 RVA: 0x00055A0C File Offset: 0x00053E0C
    public static T Random<T>()
    {
        T[] values = EnumUtils.GetValues<T>();
        return values[UnityEngine.Random.Range(0, values.Length)];
    }

    // Token: 0x06000799 RID: 1945 RVA: 0x00055A30 File Offset: 0x00053E30
    public static T Parse<T>(string name)
    {
        T[] values = EnumUtils.GetValues<T>();
        for (int i = 0; i < values.Length; i++)
        {
            if (name == values[i].ToString())
            {
                return values[i];
            }
        }
        return values[0];
    }

    // Token: 0x0600079A RID: 1946 RVA: 0x00055A88 File Offset: 0x00053E88
    public static bool TryParse<T>(string name, out T result)
    {
        T[] values = EnumUtils.GetValues<T>();
        for (int i = 0; i < values.Length; i++)
        {
            if (name == values[i].ToString())
            {
                result = values[i];
                return true;
            }
        }
        result = values[0];
        return false;
    }
}