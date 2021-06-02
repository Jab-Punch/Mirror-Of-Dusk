using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class GameImageAnimation
{
    public string name;
    public Sprite[] _sprite;
    public Sequence[] _sequence;

    [Serializable]
    public class Sequence
    {
        public int key;
        public int spriteKey;
        public int delay;
    }
}
