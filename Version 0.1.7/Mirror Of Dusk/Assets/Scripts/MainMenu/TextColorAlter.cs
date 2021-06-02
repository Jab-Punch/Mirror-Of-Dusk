using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class TextColorAlter : MonoBehaviour
{

    MenuSelector menuSelector;
    public TextMeshProUGUI[] tm;
    GameObject mScreen;
    SpriteRenderer mScreenRender;
    ClearMirrorDew[] clearMirrorDew;
    DelayInputMainMenu delayInput;

    // Use this for initialization
    void Awake()
    {
        delayInput = gameObject.GetComponent<DelayInputMainMenu>();
        mScreen = GameObject.Find("TintScreenSprite");
        mScreenRender = mScreen.GetComponent<SpriteRenderer>();
        menuSelector = gameObject.GetComponent<MenuSelector>();
    }

    // Update is called once per frame
    /*void Update()
    {

    }*/

    public IEnumerator textAlterColor()
    {
        int uCount = 0;

        clearMirrorDew = FindObjectsOfType<ClearMirrorDew>();
        if (GameObject.Find("SubChoiceText0") != null)
        {
            tm = new TextMeshProUGUI[menuSelector.numSelectors];
            for (int i = 0; i < menuSelector.numSelectors; i++)
            {
                tm[i] = (TextMeshProUGUI)GameObject.Find("SubChoiceText" + i).GetComponent<TextMeshProUGUI>();
            }
            while (uCount < 50)
            {
                for (int m = 0; m < menuSelector.numSelectors; m++)
                {
                    if (m == menuSelector.currentSelector)
                    {
                        tm[m].color = new Color(140f / 255f, 117f / 255f, 226f / 255f, 1);

                        tm[m].outlineColor = new Color(67f / 255f, 67f / 255f, 67f / 255f, 1);
                        //tm[m].outlineWidth = 0.25f;
                        Color glowCol = new Color32(203, 5, 72, 55);
                        //Color glowCol = new Color32(255, 255, 255, 255);
                        tm[m].fontSharedMaterial.SetColor("_GlowColor", glowCol);
                        tm[m].fontSharedMaterial.SetFloat("_GlowOuter", 0.5f);
                        bool checkDew = false;
                        foreach (ClearMirrorDew cMD in clearMirrorDew)
                        {
                            if (cMD.clearMDewCheck())
                            {
                                checkDew = true;
                            }
                        }
                        if (!menuSelector.clearMDewCheck() && !checkDew && !(delayInput.delayTime > 0 && delayInput.checkRefresh()) && uCount >= 8)
                        {
                            string currentMenuChoice = menuSelector.mainMenuDictKey[menuSelector.currentSelector];
                            bool foundOptionRender = false;
                            foreach (KeyValuePair<string, Color32> list in menuSelector.mainMenuDictColors)
                            {
                                if (list.Key == currentMenuChoice)
                                {
                                    foundOptionRender = true;
                                }
                            }
                            if (!foundOptionRender) { currentMenuChoice = "DEFAULT"; }
                            mScreenRender.color = menuSelector.mainMenuDictColors[currentMenuChoice];
                        }
                    }
                    else
                    {
                        bool dictFound = false;
                        foreach (string list in menuSelector.mainMenuList)
                        {
                            if (list == menuSelector.mainMenuDictKey[m])
                            {
                                dictFound = true;
                            }
                        }
                        if (dictFound)
                        {
                            tm[m].color = new Color(201f / 255f, 198f / 255f, 154f / 255f, 1);
                        }
                        else
                        {
                            tm[m].color = new Color(110f / 255f, 110f / 255f, 110f / 255f, 1);
                        }
                        tm[m].outlineColor = new Color(67f / 255f, 67f / 255f, 67f / 255f, 1);
                        //tm[m].outlineWidth = 0.25f;
                        Color glowCol = new Color32(99, 31, 118, 128);
                        //Color glowCol = new Color32(255, 255, 255, 255);
                        tm[m].fontSharedMaterial.SetColor("_GlowColor", glowCol);
                        tm[m].fontSharedMaterial.SetFloat("_GlowOuter", 0.25f);
                    }
                }
                uCount++;
                yield return null;
            }
            
        }
        yield return null;
    }

    public void refreshMenu()
    {
        menuSelector = GameObject.Find("MenuSelector").GetComponent<MenuSelector>();
        tm = new TextMeshProUGUI[menuSelector.numSelectors];
        for (int i = 0; i < menuSelector.numSelectors; i++)
        {
            tm[i] = (TextMeshProUGUI)GameObject.Find("SubChoiceText" + i).GetComponent<TextMeshProUGUI>();
        }
    }
}
