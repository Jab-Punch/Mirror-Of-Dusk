using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSelectOrb : MonoBehaviour {

    public bool summonInstant = true;
    SpriteRenderer spr;
    IMenuSelectOrb menuSelectOrbGroup;

    void Awake()
    {
        gameObject.layer = 9;
        spr = gameObject.GetComponent<SpriteRenderer>();
        spr.sortingLayerName = "MenuBase";
    }

    // Use this for initialization
    void Start () {
        menuSelectOrbGroup = gameObject.transform.parent.GetComponent<IMenuSelectOrb>();
        if (!summonInstant)
        {
            spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 0);
            StartCoroutine("summonMenuSelectOrb");
        }
    }

    public void sortLayer(string layerName)
    {
        spr.sortingLayerName = layerName;
    }

    public IEnumerator summonMenuSelectOrb()
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

    public IEnumerator animateOrb(string direction)
    {
        int yLevel = 0;
        bool yState = false;
        while (true)
        {
            switch (direction)
            {
                case "Left":
                    if (!yState)
                    {
                        gameObject.transform.position = new Vector3(gameObject.transform.position.x - 1, gameObject.transform.position.y, gameObject.transform.position.z);
                        yLevel++;
                        if (yLevel >= 5)
                        {
                            yState = true;
                        }
                    }
                    else
                    {
                        gameObject.transform.position = new Vector3(gameObject.transform.position.x + 1, gameObject.transform.position.y, gameObject.transform.position.z);
                        yLevel--;
                        if (yLevel <= 0)
                        {
                            yState = false;
                        }
                    }
                    break;
                case "Right":
                    if (!yState)
                    {
                        gameObject.transform.position = new Vector3(gameObject.transform.position.x + 1, gameObject.transform.position.y, gameObject.transform.position.z);
                        yLevel++;
                        if (yLevel >= 5)
                        {
                            yState = true;
                        }
                    }
                    else
                    {
                        gameObject.transform.position = new Vector3(gameObject.transform.position.x - 1, gameObject.transform.position.y, gameObject.transform.position.z);
                        yLevel--;
                        if (yLevel <= 0)
                        {
                            yState = false;
                        }
                    }
                    break;
                case "Up":
                    if (!yState)
                    {
                        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1, gameObject.transform.position.z);
                        yLevel++;
                        if (yLevel >= 5)
                        {
                            yState = true;
                        }
                    }
                    else
                    {
                        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 1, gameObject.transform.position.z);
                        yLevel--;
                        if (yLevel <= 0)
                        {
                            yState = false;
                        }
                    }
                    break;
                case "Down":
                    if (!yState)
                    {
                        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 1, gameObject.transform.position.z);
                        yLevel++;
                        if (yLevel >= 5)
                        {
                            yState = true;
                        }
                    }
                    else
                    {
                        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1, gameObject.transform.position.z);
                        yLevel--;
                        if (yLevel <= 0)
                        {
                            yState = false;
                        }
                    }
                    break;
                default:
                    break;
            }
            yield return null;
            yield return null;
        }
    }

    public IEnumerator destroyMenuSelectOrb()
    {
        float curAlpha = spr.color.a;
        while (curAlpha > 0.0f)
        {
            curAlpha -= 0.1f;
            if (curAlpha < 0.0f)
            {
                curAlpha = 0.0f;
            }
            spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, curAlpha);
            yield return null;
        }
        menuSelectOrbGroup.removeMenuSelectOrb();
        Destroy(gameObject);
        yield return null;
    }
}
