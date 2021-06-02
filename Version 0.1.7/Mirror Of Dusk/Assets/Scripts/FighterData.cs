using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterData : MonoBehaviour
{

    [System.Serializable]
    public class FighterColorData
    {
        public string name;
        public int colorCode;
        public SelectColors selectColors = new SelectColors();
        public SelectPalettes[] selectPalettes;
    }

    [System.Serializable]
    public class SelectColors
    {
        public Color colorA;
        public Color colorB;
        public Color colorC;
    }

    [System.Serializable]
    public class SelectPalettes
    {
        public int iconCode;
        public Material bustUpPalette;
    }

    public FighterColorData[] fighterColorData;

}
