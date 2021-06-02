using System;
using UnityEngine;

public class CharacterSelectCursorSprite : AbstractPausableComponent
{
    protected override void Awake()
    {
        base.Awake();
        this.SetLayer(base.GetComponent<SpriteRenderer>(), 0);
        int sortOrder = -1;
        foreach (SpriteRenderer layer in base.GetComponentsInChildren<SpriteRenderer>())
        {
            this.SetLayer(layer, 0, sortOrder++);
        }
    }

    protected void SetLayer(SpriteRenderer renderer, int player_id)
    {
        if (renderer == null) return;
        renderer.sortingLayerName = "Foreground_3";
        renderer.sortingOrder = (0 - (4 * player_id));
    }

    protected void SetLayer(SpriteRenderer renderer, int player_id, int sortOrder)
    {
        if (renderer == null) return;
        renderer.sortingLayerName = "Foreground_3";
        renderer.sortingOrder = (sortOrder - (4 * player_id));
    }

    protected void ResetLayers(int player_id)
    {
        this.SetLayer(base.GetComponent<SpriteRenderer>(), player_id);
        int sortOrder = -1;
        foreach (SpriteRenderer layer in base.GetComponentsInChildren<SpriteRenderer>())
        {
            this.SetLayer(layer, player_id, sortOrder++);
        }
    }

    protected virtual void Update()
    {
        Vector3 position = base.transform.position;
        base.transform.position = new Vector3(position.x, position.y, position.z);
    }
}
