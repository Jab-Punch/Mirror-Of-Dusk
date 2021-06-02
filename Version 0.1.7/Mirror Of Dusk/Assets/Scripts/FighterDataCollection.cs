using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterDataCollection : MonoBehaviour {

    [System.Serializable]
    public class FighterSelectData
    {
        public string name;
        public int id;
        public bool active = true;
        public GameObject fighterData;
    }

    public FighterSelectData[] fighterSelectData;
}
