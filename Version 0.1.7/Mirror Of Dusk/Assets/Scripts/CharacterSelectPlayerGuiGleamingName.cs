using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectPlayerGuiGleamingName : AbstractMB
{
    [SerializeField] private SpriteRenderer nameSprite;
    [NonSerialized] public CharacterSelectPlayerGUI parentGUI;

    protected override void Awake()
    {
        base.Awake();
    }

    public void Init(CharacterSelectPlayerGUI pGui, Sprite sprit)
    {
        this.parentGUI = pGui;
        this.nameSprite.sprite = sprit;
        this.StartCoroutine(nameBurst_cr());
    }

    public IEnumerator nameBurst_cr()
    {
        float scal = 1.0f;
        float alpha = 1.0f;
        nameSprite.transform.localScale = new Vector3(scal, scal, 1.0f);
        nameSprite.color = new Color(nameSprite.color.r, nameSprite.color.g, nameSprite.color.b, alpha);
        yield return null;
        for (int j = 0; j < 8; j++)
        {
            nameSprite.transform.localScale += new Vector3(0.015f, 0.04f, 0.0f);
            yield return null;
        }
        while (alpha > 0)
        {
            nameSprite.transform.localScale += new Vector3(0.015f, 0.04f, 0.0f);
            nameSprite.color = new Color(nameSprite.color.r, nameSprite.color.g, nameSprite.color.b, alpha);
            alpha -= 0.05f;
            yield return null;
        }
        nameSprite.color = new Color(nameSprite.color.r, nameSprite.color.g, nameSprite.color.b, 0.0f);
        yield return null;
        if (parentGUI != null)
            this.parentGUI.RecycleGleam(this);
        yield break;
    }
}
