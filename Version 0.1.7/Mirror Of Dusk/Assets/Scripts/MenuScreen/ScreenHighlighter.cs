using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenHighlighter : MonoBehaviour {

    public bool summonInstant = true;

    GameObject gameChild;
    SpriteRenderer spr;
    SpriteRenderer[] sprChild;

    NewMenuScreenRoot menuScreenRoot;

    private void Awake()
    {
        spr = gameObject.GetComponent<SpriteRenderer>();
        sprChild = gameObject.GetComponentsInChildren<SpriteRenderer>();
    }

    // Use this for initialization
    void Start () {
        gameChild = gameObject.transform.GetChild(0).gameObject;
        gameObject.layer = 9;
        gameChild.layer = 9;
        menuScreenRoot = gameObject.transform.parent.GetComponent<NewMenuScreenRoot>();
        SpriteRenderer rootSpr = menuScreenRoot.gameObject.GetComponent<SpriteRenderer>();
        spr.size = new Vector2(menuScreenRoot.screenHighlighterSize.width, menuScreenRoot.screenHighlighterSize.height);
        //spr.sortingLayerName = "MenuBase";
        spr.sortingLayerName = rootSpr.sortingLayerName;
        foreach (SpriteRenderer child in sprChild)
        {
            child.size = new Vector2(menuScreenRoot.screenHighlighterSize.width, menuScreenRoot.screenHighlighterSize.height);
            //child.sortingLayerName = "MenuBase";
            child.sortingLayerName = rootSpr.sortingLayerName;
        }
        if (!summonInstant)
        {
            spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 0);
            foreach (SpriteRenderer child in sprChild)
            {
                child.color = new Color(child.color.r, child.color.g, child.color.b, 0);
            }
            StartCoroutine("summonHighlighter");
        } else
        {

        }
	}

    public IEnumerator summonHighlighter()
    {
        float curAlpha = spr.color.a;
        while (curAlpha < 0.5f)
        {
            curAlpha += 0.05f;
            if (curAlpha > 0.5f)
            {
                curAlpha = 0.5f;
            }
            SpriteRenderer spr = gameObject.GetComponent<SpriteRenderer>();
            spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, curAlpha);
            SpriteRenderer[] children = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer child in children)
            {
                child.color = new Color(child.color.r, child.color.g, child.color.b, curAlpha);
            }
            yield return null;
        }
        yield return null;
    }

    public IEnumerator destroyHighlighter()
    {
        float curAlpha = spr.color.a;
        while (curAlpha > 0.0f)
        {
            curAlpha -= 0.05f;
            if (curAlpha < 0.0f)
            {
                curAlpha = 0.0f;
            }
            SpriteRenderer spr = gameObject.GetComponent<SpriteRenderer>();
            spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, curAlpha);
            SpriteRenderer[] children = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer child in children)
            {
                child.color = new Color(child.color.r, child.color.g, child.color.b, curAlpha);
            }
            yield return null;
        }
        menuScreenRoot.removeHighlighter();
        Destroy(gameObject);
        yield return null;
    }
}
