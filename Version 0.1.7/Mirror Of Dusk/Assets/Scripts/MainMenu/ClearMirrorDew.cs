using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearMirrorDew : MonoBehaviour
{

    MenuSelector menuSelector;
    SpriteRenderer render;
    GameObject tScreen;
    RawImage tScreenRender;
    GameObject mScreen;
    SpriteRenderer mScreenRender;
    private int dewCount;
    private int colCount;
    private bool startDewClear = false;
    private bool startDewRestore = false;

    // Use this for initialization
    void Start()
    {
        menuSelector = FindObjectOfType<MenuSelector>();
        render = gameObject.GetComponent<SpriteRenderer>();
        tScreen = GameObject.Find("BaseTint");
        tScreenRender = (RawImage)tScreen.GetComponent<RawImage>();
        mScreen = GameObject.Find("TintScreenSprite");
        mScreenRender = mScreen.GetComponent<SpriteRenderer>();
        dewCount = 25;
        colCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (name.Contains("BigShard-White"))
        {
            if (menuSelector.clearMDewCheck())
            {
                dewCount--;
                if (dewCount <= 0)
                {
                    menuSelector.clearMDewOff();
                    startDewClear = true;
                }
            }
            if (startDewClear)
            {
                colCount++;
                float dewRed = render.color.r;
                float dewGreen = render.color.g;
                float dewBlue = render.color.b;
                float alpha = render.color.a;
                alpha -= 0.05f;
                float tAlpha = tScreenRender.color.a;
                tAlpha -= ((colCount > 6) ? 0.05f : 0f);
                if (alpha <= 0) { alpha = 0; }
                if (tAlpha <= 0) { tAlpha = 0; }
                render.color = new Color(dewRed, dewGreen, dewBlue, alpha);
                tScreenRender.color = new Color(tScreenRender.color.r, tScreenRender.color.g, tScreenRender.color.b, tAlpha);
                if (alpha == 0)
                {
                    tScreenRender.color = new Color(mScreenRender.color.r, mScreenRender.color.g, mScreenRender.color.b, mScreenRender.color.a);
                    mScreenRender.color = new Color(mScreenRender.color.r, mScreenRender.color.g, mScreenRender.color.b, 0f);
                    startDewClear = false;
                }
            }
            if (startDewRestore)
            {
                colCount++;
                float alpha = render.color.a;
                //alpha += 0.03f;
                alpha += ((colCount >= 10) ? 0.03f : 0f);
                float tAlpha = tScreenRender.color.a;
                tAlpha += ((colCount >= 10) ? 0.05f : 0f);
                if (alpha >= 0.5f)
                {
                    alpha = 0.5f;
                }
                if (tAlpha >= (76f / 255f))
                {
                    tAlpha = (76f / 255f);
                }
                render.color = new Color(render.color.r, render.color.g, render.color.b, alpha);
                tScreenRender.color = new Color(tScreenRender.color.r, tScreenRender.color.g, tScreenRender.color.b, tAlpha);
                if (alpha == 0.5f)
                {
                    startDewRestore = false;
                }
            }
        }
        if (name.Contains("BigShard-Inner"))
        {
            if (menuSelector.clearMDewCheck())
            {
                dewCount--;
                if (dewCount <= 0)
                {
                    menuSelector.clearMWhiteOff();
                    startDewClear = true;
                }
            }
            if (startDewClear)
            {
                float dewRed = render.color.r;
                float dewGreen = render.color.g;
                float dewBlue = render.color.b;
                float alpha = render.color.a;
                alpha -= 0.015f;
                if (alpha <= 0)
                {
                    alpha = 0;
                }
                render.color = new Color(dewRed, dewGreen, dewBlue, alpha);
            }
            if (startDewRestore)
            {
                colCount++;
                float alpha = render.color.a;
                //alpha += 0.015f;
                alpha += ((colCount >= 10) ? 0.015f : 0f);
                if (alpha >= 0.25f)
                {
                    alpha = 0.25f;
                }
                render.color = new Color(render.color.r, render.color.g, render.color.b, alpha);
                if (alpha == 0.25f)
                {
                    startDewRestore = false;
                }
            }
        }

    }

    public bool clearMDewCheck()
    {
        if (startDewClear)
        {
            return true;
        }
        return false;
    }

    public void restoreMDew()
    {
        render = gameObject.GetComponent<SpriteRenderer>();
        float alpha = 0;
        render.color = new Color(render.color.r, render.color.g, render.color.b, alpha);
        startDewRestore = true;
    }

    public bool restoreMDewCheck()
    {
        if (startDewRestore)
        {
            return true;
        }
        return false;
    }
}
