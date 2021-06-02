using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuEditNumber : MonoBehaviour, IMenuEditNumber, IMenuSelectArrowUp, IMenuSelectArrowDown
{
    private CharacterSelectManager characterSelectManager;

    public SFXPlayer sfxPlayer;
    public GameObject menuSelectArrowUpPrefab;
    public GameObject menuSelectArrowDownPrefab;
    public List<GameObject> menuSelectArrowUp { get; set; }
    public List<GameObject> menuSelectArrowDown { get; set; }
    private SpriteRenderer menuSelectArrowUpSprite;
    private SpriteRenderer menuSelectArrowDownSprite;
    private TextMeshProUGUI defaultText;
    private int editedNumber;
    HorizontalLayoutGroup horiz;
    public int horizLen;
    public GameObject[] slots;
    public int slotPos;
    private int minLimit;
    private int maxLimit;
    public string sortLayerName = "MenuBaseB";
    //private bool scriptOn = false;

    // Use this for initialization
    void Awake () {
        characterSelectManager = CharacterSelectManager.characterSelectManager;

        sfxPlayer = characterSelectManager.sfxPlayer.GetComponent<SFXPlayer>();
        defaultText = gameObject.transform.GetComponentInChildren<TextMeshProUGUI>();
        defaultText.color = new Color(0.5f, 0.5f, 0.5f, defaultText.color.a);
        horiz = gameObject.transform.GetComponentInChildren<HorizontalLayoutGroup>();
        horizLen = horiz.gameObject.transform.childCount;
        slots = new GameObject[horizLen];
        for (int i = 0; i < horizLen; i++)
        {
            slots[i] = horiz.gameObject.transform.GetChild(i).gameObject;
        }
        slotPos = 0;
        menuSelectArrowUp = new List<GameObject>();
        menuSelectArrowDown = new List<GameObject>();
    }

    public void updateSortLayer(string layerName)
    {
        sortLayerName = layerName;
    }

    public void setMinMax(int min, int max)
    {
        minLimit = min;
        maxLimit = max;
    }

    public void setMin(int min)
    {
        minLimit = min;
    }

    public void setMax(int max)
    {
        maxLimit = max;
    }

    public void activateText()
    {
        defaultText.color = new Color(19f / 255f, 1.0f, 145f / 255f, defaultText.color.a);
    }

    public void deactivateText()
    {
        defaultText.color = new Color(0.5f, 0.5f, 0.5f, defaultText.color.a);
    }

    public void updateNumber(int edit)
    {
        editedNumber = edit;
        string fmt = new System.String('0', horizLen);
        defaultText.text = editedNumber.ToString(fmt);
    }

    public void updateNumber(char edit)
    {
        string editText = "";
        for (int i = 0; i < horizLen; i++)
        {
            editText += edit;
        }
        defaultText.text = editText;
    }

    public int ScrollDown(int number, int digitAim)
    {
        int digitFactor = FindDigitFactor(digitAim);
        if (number >= maxLimit)
        {
            updateArrowUp(false);
        }
        if (number > minLimit)
        {
            sfxPlayer.PlaySound("Scroll");
        }
        number -= digitFactor;
        if (number <= minLimit)
        {
            number = minLimit;
            updateArrowDown(true);
        }
        if (minLimit == maxLimit)
        {
            arrowsOff();
        }
        updateNumber(number);
        return number;
    }

    public int ScrollUp(int number, int digitAim)
    {
        int digitFactor = FindDigitFactor(digitAim);
        if (number <= minLimit)
        {
            updateArrowDown(false);
        }
        if (number < maxLimit)
        {
            sfxPlayer.PlaySound("Scroll");
        }
        number += digitFactor;
        if (number >= maxLimit)
        {
            number = maxLimit;
            updateArrowUp(true);
        }
        if (minLimit == maxLimit)
        {
            arrowsOff();
        }
        updateNumber(number);
        return number;
    }

    public bool ScrollRight()
    {
        if (slotPos < horizLen - 1)
        {
            sfxPlayer.PlaySound("Scroll");
            slotPos++;
            summonUI();
            return true;
        } else
        {
            return false;
        }
    }

    public bool ScrollLeft()
    {
        if (slotPos > 0)
        {
            sfxPlayer.PlaySound("Scroll");
            slotPos--;
            summonUI();
            return true;
        } else
        {
            return false;
        }
    }

    public int FindDigitFactor(int digitAim)
    {
        int digitFactor = 1;
        for (int i = horizLen - 1; i >= 0; i--)
        {
            if (i == digitAim)
            {
                return digitFactor;
            }
            digitFactor = digitFactor * 10;
        }
        return 1;
    }

    public void summonUI()
    {
        summonMenuSelectArrowUp();
        summonMenuSelectArrowDown();
    }

    public void resetUI()
    {
        resetMenuSelectArrowUp();
        resetMenuSelectArrowDown();
    }

    private void summonMenuSelectArrowUp()
    {
        destroyMenuSelectArrowUp();
        float posX = ((slots[slotPos].transform.parent.transform.position.x - (slots[slotPos].transform.parent.GetComponent<RectTransform>().rect.width / 2)) - slots[slotPos].GetComponent<RectTransform>().rect.x + (slots[slotPos].GetComponent<RectTransform>().rect.width * slotPos));
        float posY = ((slots[slotPos].transform.parent.transform.position.y + (slots[slotPos].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + slots[slotPos].GetComponent<RectTransform>().rect.y);

        menuSelectArrowUp.Add(Instantiate(menuSelectArrowUpPrefab, new Vector3(posX, posY + 20f, -5f), Quaternion.identity));
        menuSelectArrowUp[menuSelectArrowUp.Count - 1].GetComponent<MenuSelectArrowUp>().sortLayer(sortLayerName);
        if (editedNumber >= maxLimit)
        {
            menuSelectArrowUp[menuSelectArrowUp.Count - 1].GetComponent<MenuSelectArrowUp>().arrowActive = false;
        }
        menuSelectArrowUp[menuSelectArrowUp.Count - 1].transform.parent = gameObject.transform;
        menuSelectArrowUp[menuSelectArrowUp.Count - 1].GetComponent<MenuSelectArrowUp>().summonInstant = true;
        /*if (menuSelectArrowUp.Count > 1)
        {
            menuSelectArrowUp[menuSelectArrowUp.Count - 2].GetComponent<MenuSelectArrowUp>().destroyMenuSelectArrow();
        }*/
        //screenHighlighter.RemoveAt(screenHighlighter.Count - 2);
    }

    private void summonMenuSelectArrowDown()
    {
        destroyMenuSelectArrowDown();
        float posX = ((slots[slotPos].transform.parent.transform.position.x - (slots[slotPos].transform.parent.GetComponent<RectTransform>().rect.width / 2)) - slots[slotPos].GetComponent<RectTransform>().rect.x + (slots[slotPos].GetComponent<RectTransform>().rect.width * slotPos));
        float posY = ((slots[slotPos].transform.parent.transform.position.y + (slots[slotPos].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + slots[slotPos].GetComponent<RectTransform>().rect.y);

        menuSelectArrowDown.Add(Instantiate(menuSelectArrowDownPrefab, new Vector3(posX, posY - 20f, -5f), Quaternion.identity));
        menuSelectArrowDown[menuSelectArrowDown.Count - 1].GetComponent<MenuSelectArrowDown>().sortLayer(sortLayerName);
        if (editedNumber <= minLimit)
        {
            menuSelectArrowDown[menuSelectArrowDown.Count - 1].GetComponent<MenuSelectArrowDown>().arrowActive = false;
        }
        menuSelectArrowDown[menuSelectArrowDown.Count - 1].transform.parent = gameObject.transform;
        menuSelectArrowDown[menuSelectArrowDown.Count - 1].GetComponent<MenuSelectArrowDown>().summonInstant = true;
        /*if (menuSelectArrowDown.Count > 1)
        {
            menuSelectArrowDown[menuSelectArrowDown.Count - 2].GetComponent<MenuSelectArrowDown>().destroyMenuSelectArrow();
        }*/
    }

    public void updateArrowUp(bool on)
    {
        if (on)
        {
            float posY = ((slots[slotPos].transform.parent.transform.position.y + (slots[slotPos].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + slots[slotPos].GetComponent<RectTransform>().rect.y);
            menuSelectArrowUp[0].GetComponent<MenuSelectArrowUp>().turnOffArrow(posY + 20f);
            menuSelectArrowDown[0].GetComponent<MenuSelectArrowDown>().resetArrow(posY - 20f);
        }
        else
        {
            float posY = ((slots[slotPos].transform.parent.transform.position.y + (slots[slotPos].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + slots[slotPos].GetComponent<RectTransform>().rect.y);
            menuSelectArrowUp[0].GetComponent<MenuSelectArrowUp>().turnOnArrow();
            menuSelectArrowUp[0].GetComponent<MenuSelectArrowUp>().resetArrow(posY + 20f);
            menuSelectArrowDown[0].GetComponent<MenuSelectArrowDown>().resetArrow(posY - 20f);
        }
    }

    public void updateArrowDown(bool on)
    {
        if (on)
        {
            float posY = ((slots[slotPos].transform.parent.transform.position.y + (slots[slotPos].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + slots[slotPos].GetComponent<RectTransform>().rect.y);
            menuSelectArrowDown[0].GetComponent<MenuSelectArrowDown>().turnOffArrow(posY - 20f);
            menuSelectArrowUp[0].GetComponent<MenuSelectArrowUp>().resetArrow(posY + 20f);
        }
        else
        {
            float posY = ((slots[slotPos].transform.parent.transform.position.y + (slots[slotPos].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + slots[slotPos].GetComponent<RectTransform>().rect.y);
            menuSelectArrowDown[0].GetComponent<MenuSelectArrowDown>().turnOnArrow();
            menuSelectArrowUp[0].GetComponent<MenuSelectArrowUp>().resetArrow(posY + 20f);
            menuSelectArrowDown[0].GetComponent<MenuSelectArrowDown>().resetArrow(posY - 20f);
        }
    }

    public void arrowsOff()
    {
        float posY = ((slots[slotPos].transform.parent.transform.position.y + (slots[slotPos].transform.parent.GetComponent<RectTransform>().rect.height / 2)) + slots[slotPos].GetComponent<RectTransform>().rect.y);
        menuSelectArrowUp[0].GetComponent<MenuSelectArrowUp>().turnOffArrow(posY + 20f);
        menuSelectArrowDown[0].GetComponent<MenuSelectArrowDown>().turnOffArrow(posY - 20f);
    }

    public void destroyMenuSelectArrowUp()
    {
        if (menuSelectArrowUp.Count > 0)
        {
            for (int i = 0; i < menuSelectArrowUp.Count; i++)
            {
                Destroy(menuSelectArrowUp[i]);
            }
            menuSelectArrowUp.Clear();
        }
    }

    public void destroyMenuSelectArrowDown()
    {
        if (menuSelectArrowDown.Count > 0)
        {
            for (int i = 0; i < menuSelectArrowDown.Count; i++)
            {
                Destroy(menuSelectArrowDown[i]);
            }
            menuSelectArrowDown.Clear();
        }
    }

    public void removeMenuSelectArrowUp()
    {
        menuSelectArrowUp.RemoveAt(0);
    }

    public void removeMenuSelectArrowDown()
    {
        menuSelectArrowDown.RemoveAt(0);
    }

    public void resetMenuSelectArrowUp()
    {
        for (int i = 0; i < menuSelectArrowUp.Count; i++)
        {
            Destroy(menuSelectArrowUp[i]);
        }
        menuSelectArrowUp.Clear();
    }

    public void resetMenuSelectArrowDown()
    {
        for (int i = 0; i < menuSelectArrowDown.Count; i++)
        {
            Destroy(menuSelectArrowDown[i]);
        }
        menuSelectArrowDown.Clear();
    }

    public void reviewMaxLimit()
    {
        if (editedNumber >= maxLimit)
        {
            updateArrowUp(true);
        } else
        {
            if (menuSelectArrowUp[menuSelectArrowUp.Count - 1].GetComponent<MenuSelectArrowUp>().arrowActive == false)
            {
                updateArrowUp(false);
            }
        }
    }
}
