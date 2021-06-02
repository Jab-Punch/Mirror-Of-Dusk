using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class GameSpriteAnimation
{
    public string name;
    public Sprite[] _sprite;
    public Sequence[] _sequence;
    public Sounds[] _sounds;

    [Serializable]
    public class Sequence
    {
        public int key;
        public int spriteKey;
        public int delay;
    }

    [Serializable]
    public class Sounds
    {
        public int key;
        public string sound;
        public int delay;
    }
}
