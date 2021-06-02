using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToMenuScene : MonoBehaviour
{

    public bool enableFadeScene = false;
    public bool enableLoadScene = false;
    TitleInput titleInput;
    GameObject scrollObject;
    Other_Background_Scroller[] otherBack;
    FlashText flashText;
    LoadTitleFade loadTitleFade;

    SFXPlayer sfxPlayer;

    // Use this for initialization
    void Start()
    {
        titleInput = GameObject.Find("MenuEntry").GetComponent<TitleInput>();
        otherBack = FindObjectsOfType<Other_Background_Scroller>();
        flashText = FindObjectOfType<FlashText>();
        //loadTitleFade = FindObjectOfType<LoadTitleFade>();
        sfxPlayer = GameObject.Find("SFX Player").GetComponent<SFXPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enableFadeScene)
        {
            titleInput.enableMenuInput = false;
            sfxPlayer.PlaySound("EnterMain");
            foreach (Other_Background_Scroller oB in otherBack)
            {
                oB.enableScroll = false;
            }
            flashText.flashOnTime = 3f;
            flashText.flashOffTime = 3f;
            //StartCoroutine(loadTitleFade.BeginTitleFade());
            enableFadeScene = false;
            enableLoadScene = true;
        }
        if (enableLoadScene)
        {
            //StartCoroutine(loadTitleFade.EndTitle());
            enableLoadScene = false;
        }
    }
}
