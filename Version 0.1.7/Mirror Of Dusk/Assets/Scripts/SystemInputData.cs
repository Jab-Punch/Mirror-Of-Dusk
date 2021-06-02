using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemInputData : MonoBehaviour {

    public int playerFoundCounter = 0;
    public class InputClass
    {
        public int _isHold;
        public int _negative;
        public int _used;

        public InputClass(int isHold, int negative, int used)
        {
            _isHold = isHold;
            _negative = negative;
            _used = used;
        }
    }

    public static Dictionary<string, InputClass> systemInputPressList = new Dictionary<string, InputClass>()
    {
        { "UI_Left", new InputClass(2, -1, 0) },
        { "UI_Right", new InputClass(2, 1, 0) },
        { "UI_Up", new InputClass(1, 1, 0) },
        { "UI_Down", new InputClass(1, -1, 0) },
        { "Confirm", new InputClass(0, 1, 0) },
        { "Cancel", new InputClass(0, 1, 0) },
        { "UI_Pause", new InputClass(0, 1, 0) }
    };

    public static Dictionary<string, InputClass> systemInputReleaseList = new Dictionary<string, InputClass>()
    {
        { "UI_Left", new InputClass(2, -1, 0) },
        { "UI_Right", new InputClass(2, 1, 0) },
        { "UI_Up", new InputClass(1, 1, 0) },
        { "UI_Down", new InputClass(1, -1, 0) },
        { "Confirm", new InputClass(0, 1, 0) },
        { "Cancel", new InputClass(0, 1, 0) },
        { "UI_Pause", new InputClass(0, 1, 0) }
    };

    private void Awake()
    {
        //systemInputPressList = new Dictionary<string, InputClass>();
        //systemInputReleaseList = new Dictionary<string, InputClass>();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
