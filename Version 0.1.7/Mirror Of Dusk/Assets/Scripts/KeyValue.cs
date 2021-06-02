﻿using System;
using System.Collections.Generic;

[Serializable]
public class KeyValue
{
    public string key = string.Empty;
    public float value;
    
    public KeyValue()
    {
    }
    
    public KeyValue(string key, float value)
    {
        this.key = key;
        this.value = value;
    }
    
    public static KeyValue[] ListFromString(string keyValueString, char[] allowedCharacters)
    {
        List<KeyValue> list = new List<KeyValue>();
        List<char> list2 = new List<char>(allowedCharacters);
        list2.Add(',');
        list2.Add(':');
        keyValueString.Replace(" ", string.Empty);
        for (int i = 0; i < keyValueString.Length; i++)
        {
            bool flag = true;
            foreach (char c in list2)
            {
                if (keyValueString[i] == c)
                {
                    flag = false;
                }
            }
            if (flag)
            {
                keyValueString.Remove(i, 1);
            }
        }
        string[] array = keyValueString.Split(new char[]
        {
            ','
        });
        for (int j = 0; j < array.Length; j++)
        {
            string[] array2 = array[j].Split(new char[]
            {
                ':'
            });
            if (array2.Length == 2)
            {
                string text = array2[0].Replace(" ", string.Empty);
                float num = 0f;
                bool flag2 = Parser.FloatTryParse(array2[1], out num);
                if (flag2 && text != null && !(text == string.Empty))
                {
                    list.Add(new KeyValue(text, num));
                }
            }
        }
        return list.ToArray();
    }
    
    public KeyValue Clone()
    {
        return new KeyValue(this.key, this.value);
    }
}