using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BumpSubChoice : MonoBehaviour
{

    MenuSelector menuSelector;
    FlashTheGleam flashTheGleam;
    ToChosenScene toChosenScene;
    public TextMeshProUGUI[] tm;
    int readSelector;
    private IEnumerator coroutine;
    SFXPlayer sfxPlayer;

    // Use this for initialization
    void Start()
    {
        //menuSelector = GameObject.Find("MenuSelector").GetComponent<MenuSelector>();
        //readSelector = menuSelector.currentSelector;
        //refreshMenu();
        sfxPlayer = GameObject.Find("SFX Player").GetComponent<SFXPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (readSelector != menuSelector.currentSelector && tm != null)
        {
            if (GameObject.Find("SubChoiceText0") != null)
            {
                tm = new TextMeshProUGUI[menuSelector.numSelectors];
                for (int i = 0; i < menuSelector.numSelectors; i++)
                {
                    tm[i] = (TextMeshProUGUI)GameObject.Find("SubChoiceText" + i).GetComponent<TextMeshProUGUI>();
                }
                readSelector = menuSelector.currentSelector;
                StartCoroutine("BumpSelect", readSelector);
                toChosenScene = GameObject.Find("MenuSelector").GetComponent<ToChosenScene>();
                if (!toChosenScene.enableFadeScene)
                {
                    flashTheGleam.StopCoroutine("FlashGleam");
                    flashTheGleam.StartCoroutine("FlashGleam", true);
                }
            }
        }
    }

    IEnumerator BumpSelect(int rS)
    {
        bool moveBack = false;
        bool moveFinish = false;
        int moveCount = 0;
        sfxPlayer.PlaySound("Scroll");
        while (!moveFinish)
        {
            if (!moveBack)
            {
                tm[rS].transform.position = new Vector3(tm[rS].transform.position.x + 3, tm[rS].transform.position.y);
                moveCount++;
                if (moveCount > 5)
                {
                    moveBack = true;
                }
            }
            else
            {
                tm[rS].transform.position = new Vector3(tm[rS].transform.position.x - 3, tm[rS].transform.position.y);
                moveCount--;
                if (moveCount <= 0)
                {
                    moveFinish = true;
                }
            }

            // Yield execution of this coroutine and return to the main loop until next frame
            yield return null;
        }
    }

    public void refreshMenu()
    {
        menuSelector = GameObject.Find("MenuSelector").GetComponent<MenuSelector>();
        flashTheGleam = GameObject.Find("MenuSelector").GetComponent<FlashTheGleam>();
        readSelector = menuSelector.currentSelector;
        tm = new TextMeshProUGUI[menuSelector.numSelectors];
        for (int i = 0; i < menuSelector.numSelectors; i++)
        {
            tm[i] = (TextMeshProUGUI)GameObject.Find("SubChoiceText" + i).GetComponent<TextMeshProUGUI>();
        }
    }
}
