using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleInput : MonoBehaviour
{

    public bool enableMenuInput = false;
    ToMenuScene toMenuScene;
    DelayInputTitle delayInput;

    // Use this for initialization
    void Start()
    {
        toMenuScene = GameObject.Find("MenuEntry").GetComponent<ToMenuScene>();
        delayInput = GameObject.Find("MenuEntry").GetComponent<DelayInputTitle>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enableMenuInput)
        {
            if (Input.GetButtonDown("Jump_1") || Input.GetButtonDown("Start_1"))
            {
                delayInput.enabled = false;
                toMenuScene.enableFadeScene = true;
            }
        }
    }
}
