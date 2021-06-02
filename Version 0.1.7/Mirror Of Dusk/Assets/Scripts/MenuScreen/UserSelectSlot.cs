using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UserSelectSlot : MenuSlot {

    private SelectUserMenuScreen rootContent;
    private RectTransform slotRect;
    private SpriteRenderer spr;
    public TextMeshProUGUI nameText;
    private GameObject boxHighlight;
    private SpriteRenderer boxHighlightSpr;

    [Header("Sprite Prefabs")]
    public Sprite _deselected;
    public Sprite _selected;

    private void Awake()
    {
        rootMenu = gameObject;
        int rootCheck = 10;
        while (rootCheck > 0)
        {
            if (rootMenu.GetComponent<NewMenuScreenRoot>() == null)
            {
                rootMenu = rootMenu.transform.parent.gameObject;
            } else
            {
                rootCheck = 0;
            }
            rootCheck--;
        }
        rootContent = rootMenu.GetComponent<SelectUserMenuScreen>();
        slotRect = gameObject.GetComponent<RectTransform>();
        spr = gameObject.transform.GetComponentInChildren<SpriteRenderer>();
        nameText = gameObject.transform.GetComponentInChildren<TextMeshProUGUI>();
        boxHighlight = rootContent._boxHighlight;
        boxHighlightSpr = boxHighlight.GetComponent<SpriteRenderer>();
    }

    void Start () {
        enabled = false;
	}

    private void OnEnable()
    {
        boxHighlight.transform.position = gameObject.transform.position;
        boxHighlightSpr.size = new Vector2(slotRect.rect.width, slotRect.rect.height);
    }

    public void selectUpdate()
    {
        spr.sprite = _selected;
    }

    public void deselectUpdate()
    {
        spr.sprite = _deselected;
    }
}
