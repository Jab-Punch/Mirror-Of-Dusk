using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSelectArrowDown : MonoBehaviour {

    public bool summonInstant = true;
    SpriteRenderer spr;
    public Sprite arrowOn;
    public Sprite arrowOff;
    IMenuSelectArrowDown menuSelectArrowDownGroup;
    public bool arrowActive = true;
    private bool scriptOn = false;
    private bool coRountineActive = false;

    private void Awake()
    {
        gameObject.layer = 9;
        spr = gameObject.GetComponent<SpriteRenderer>();
        spr.sortingLayerName = "MenuBase";
    }

    // Use this for initialization
    void Start()
    {
        menuSelectArrowDownGroup = gameObject.transform.parent.GetComponent<IMenuSelectArrowDown>();
        if (!summonInstant)
        {
            spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 0);
            StartCoroutine("summonMenuSelectArrow");
        }
        scriptOn = true;
        if (arrowActive)
        {
            StartCoroutine("animateArrow");
        }
        else
        {
            spr.sprite = arrowOff;
        }
    }

    private void OnEnable()
    {
        if (scriptOn)
        {
            if (arrowActive)
            {
                StartCoroutine("animateArrow");
            }
            else
            {
                spr.sprite = arrowOff;
            }
        }
    }

    public void sortLayer(string layerName)
    {
        spr.sortingLayerName = layerName;
    }

    public IEnumerator summonMenuSelectArrow()
    {
        float curAlpha = spr.color.a;
        while (curAlpha < 1.0f)
        {
            curAlpha += 0.1f;
            if (curAlpha > 1.0f)
            {
                curAlpha = 1.0f;
            }
            spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, curAlpha);
            yield return null;
        }
        yield return null;
    }

    public IEnumerator animateArrow()
    {
        coRountineActive = true;
        int yLevel = 0;
        bool yState = false;
        while (true)
        {
            if (!yState)
            {
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 1, gameObject.transform.position.z);
                yLevel++;
                if (yLevel >= 5)
                {
                    yState = true;
                }
            } else
            {
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1, gameObject.transform.position.z);
                yLevel--;
                if (yLevel <= 0)
                {
                    yState = false;
                }
            }
            yield return null;
            yield return null;
        }
    }

    public void turnOnArrow()
    {
        arrowActive = true;
        spr.sprite = arrowOn;
        if (!coRountineActive)
        {
            StartCoroutine("animateArrow");
        }
    }

    public void turnOffArrow(float posY)
    {
        arrowActive = false;
        spr.sprite = arrowOff;
        spr.transform.position = new Vector3(spr.transform.position.x, posY, spr.transform.position.z);
        if (coRountineActive)
        {
            StopCoroutine("animateArrow");
            coRountineActive = false;
        }
    }

    public void resetArrow(float posY)
    {
        if (coRountineActive)
        {
            StopCoroutine("animateArrow");
            coRountineActive = false;
        }
        spr.transform.position = new Vector3(spr.transform.position.x, posY, spr.transform.position.z);
        arrowActive = true;
        spr.sprite = arrowOn;
        StartCoroutine("animateArrow");
    }

    public void destroyMenuSelectArrow()
    {
        menuSelectArrowDownGroup.removeMenuSelectArrowDown();
        Destroy(gameObject);
    }
}
