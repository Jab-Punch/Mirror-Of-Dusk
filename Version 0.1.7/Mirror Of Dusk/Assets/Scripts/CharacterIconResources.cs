using System;
using UnityEngine;

public class CharacterIconResources : MonoBehaviour
{
    [SerializeField] public CharacterIconData[] data;

    [Serializable]
    public class CharacterIconData
    {
        [SerializeField] private string name;
        [SerializeField] public int characterCode;
        [SerializeField] public Data normal;
        [SerializeField] public Data reflection;
        [SerializeField] public Color backgroundColor0;
        [SerializeField] public Color backgroundColor1;

        [Serializable]
        public class Data
        {
            public Sprite _sprite;
            public SelectPalettes[] selectPalettes;

            [Serializable]
            public class SelectPalettes
            {
                public string name;
                public int colorCode;
                public Material bustUpPalette;
                public Color colorA;
                public Color colorB;
                public Color colorC;
            }
        }
    }
}
