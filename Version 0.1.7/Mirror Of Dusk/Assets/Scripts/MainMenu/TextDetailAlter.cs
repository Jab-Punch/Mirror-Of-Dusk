using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class TextDetailAlter : MonoBehaviour
{

    MenuSelector menuSelector;
    MenuInput menuInput;
    public TextMeshProUGUI tm;

    // Use this for initialization
    void Start()
    {
        menuSelector = GameObject.Find("MenuSelector").GetComponent<MenuSelector>();
        menuInput = GameObject.Find("MenuSelector").GetComponent<MenuInput>();
        tm = gameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (menuInput.enableMenuInput)
        {
            for (int m = 0; m < menuSelector.numSelectors; m++)
            {
                if (m == menuSelector.currentSelector)
                {
                    if (gameObject.name == "MainMenuMainDetail")
                    {
                        tm.text = menuSelector.mainMenuDictMainDetails[menuSelector.mainMenuDict[menuSelector.currentMainMenuMode][menuSelector.currentSelector]];
                    }
                    else if (gameObject.name == "MainMenuSubDetail")
                    {
                        tm.text = menuSelector.mainMenuDictSubDetails[menuSelector.mainMenuDict[menuSelector.currentMainMenuMode][menuSelector.currentSelector]];
                    }
                }
                else
                {

                }
            }
        }
    }
}
