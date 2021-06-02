using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectPlayerGUIHurtbox : AbstractCollidableObject
{
    private CharacterSelectPlayerGUI rootGUI;
    [SerializeField] private IconType iconType;
    [SerializeField] private CursorHitboxPriority flagType;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite inactiveSprite;
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite offlineSprite;

    private bool _initialized = false;
    private bool hitboxActive = false;
    private MaterialPropertyBlock guiBackMatOff;
    private MaterialPropertyBlock guiBackMatOn;
    private Color guiTintOff;
    private Color guiTintOn;
    private int highlightCount = 0;

    public enum IconType
    {
        Default,
        PlayerState,
        Window
    }

    protected override void Awake()
    {
        base.Awake();
        if (this.iconType == IconType.Window)
        {
            guiBackMatOff = new MaterialPropertyBlock();
            guiBackMatOn = new MaterialPropertyBlock();
            if (this.spriteRenderer != null)
            {
                this.spriteRenderer.GetPropertyBlock(guiBackMatOff);
                this.spriteRenderer.GetPropertyBlock(guiBackMatOn);
            }
            guiTintOff = new Color(0f, 0f, 0f, 1.0f);
            guiTintOn = new Color(110f / 255f, 20f / 255f, 205f / 255f, 1.0f);
            guiBackMatOff.SetColor("_Color", guiTintOff);
            guiBackMatOn.SetColor("_Color", guiTintOn);
            this.enabled = false;
        }
    }

    public void Init(CharacterSelectPlayerGUI rootGUI)
    {
        this.rootGUI = rootGUI;
        _initialized = true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (_initialized)
        {
            if (iconType == IconType.PlayerState)
            {
                this.rootGUI.ChangePlayerIconStateEvent += this.OnChangePlayerGuiIconState;
                this.spriteRenderer.sprite = offlineSprite;
                switch (CharacterSelectScene.Current.playerModes[(int)rootGUI.id])
                {
                    default:
                        this.spriteRenderer.sprite = offlineSprite;
                        break;
                    case CharacterSelectScene.PlayerMode.Player:
                        this.spriteRenderer.sprite = (this.flagType == CursorHitboxPriority.GUIPlayer) ? activeSprite : this.spriteRenderer.sprite = offlineSprite;
                        break;
                    case CharacterSelectScene.PlayerMode.CPU:
                        this.spriteRenderer.sprite = (this.flagType == CursorHitboxPriority.GUICPU) ? activeSprite : this.spriteRenderer.sprite = offlineSprite;
                        break;
                    case CharacterSelectScene.PlayerMode.Offline:
                        this.spriteRenderer.sprite = (this.flagType == CursorHitboxPriority.GUINone) ? activeSprite : this.spriteRenderer.sprite = offlineSprite;
                        break;
                }
            }
            this.hitboxActive = true;
        }
    }

    protected void OnDisable()
    {
        if (this.rootGUI != null)
        {
            if (iconType == IconType.PlayerState)
                this.rootGUI.ChangePlayerIconStateEvent -= this.OnChangePlayerGuiIconState;
        }
        this.hitboxActive = false;
    }
    
    protected override void Update()
    {
        base.Update();
        if (!_initialized)
            return;
        if (iconType == IconType.PlayerState)
        {
            if (spriteRenderer.sprite != activeSprite)
            {
                if (highlightCount > 0)
                {
                    this.spriteRenderer.sprite = inactiveSprite;
                } else
                {
                    this.spriteRenderer.sprite = offlineSprite;
                }
            } else
            {
                this.spriteRenderer.sprite = activeSprite;
            }
        } else if (iconType == IconType.Window)
        {
            if (this.spriteRenderer != null)
            {
                if (highlightCount > 0)
                {
                    this.spriteRenderer.SetPropertyBlock(guiBackMatOn);
                    rootGUI.updatedHightlightedFlags |= (CharacterSelectPlayerGUI.WindowHighlightedFlags)(1 << ((int)this.flagType - 1));
                    rootGUI.UpdateWindows();
                }
                else
                {
                    this.spriteRenderer.SetPropertyBlock(guiBackMatOff);
                    rootGUI.updatedHightlightedFlags &= ~(CharacterSelectPlayerGUI.WindowHighlightedFlags)(1 << ((int)this.flagType - 1));
                    rootGUI.UpdateWindows();
                }
            }
        }
        highlightCount = 0;
    }

    protected override void OnCollisionCursor(GameObject hit, CollisionPhase phase)
    {
        if (hitboxActive)
        {
            if (phase == CollisionPhase.Stay)
            {
                if (rootGUI.actionState == CharacterSelectPlayerGUI.ActionState.Free)
                {
                    CharacterSelectCursorStatsManager stats = hit.GetComponent<CharacterSelectCursorStatsManager>();
                    stats.playerCursorFoundFlags |= (CharacterSelectCursorStatsManager.CursorFoundFlags)(1 << ((int)this.flagType - 1));
                    stats.guiPickFlags |= (CharacterSelectCursorStatsManager.GuiPickFlags)(1 << (int)rootGUI.id);
                    /*if (this.iconType == IconType.Window)
                    {
                        rootGUI.updatedHightlightedFlags |= (CharacterSelectPlayerGUI.WindowHighlightedFlags)(1 << ((int)this.flagType - 1));
                        Debug.Log(rootGUI.updatedHightlightedFlags);
                    }*/
                    highlightCount++;
                }
            }
        }
    }

    private void OnChangePlayerGuiIconState(CursorHitboxPriority chp)
    {
        if (chp == this.flagType)
        {
            this.spriteRenderer.sprite = activeSprite;
        } else
        {
            this.spriteRenderer.sprite = offlineSprite;
        }
    }
}
