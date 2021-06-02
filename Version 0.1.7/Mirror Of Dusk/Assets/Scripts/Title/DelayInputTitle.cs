using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DelayInputTitle : DelayInput
{

    TitleInput titleInput;

    // Use this for initialization
    void Start()
    {
        titleInput = GameObject.Find("MenuEntry").GetComponent<TitleInput>();
    }

    // Update is called once per frame
    void Update()
    {
        delayTime--;
        if (delayTime <= 0)
        {
            delayTime = 0f;
            titleInput.enableMenuInput = true;
        }
    }
}