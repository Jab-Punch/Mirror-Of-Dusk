using System;
using System.Collections;
using System.Collections.Generic;
using Rewired.UI.ControlMapper;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsNewUserNameGUISlot : MonoBehaviour
{
    [SerializeField] private OptionsNewUserNameGUI rootGUI;
    public GameImageAnimator highlighterAnimation;

    private void OnEnable()
    {
        if (rootGUI != null)
            rootGUI.OnFadeNameHighlighterEvent += this.OnFadeNameHighlighter;
    }

    private void OnDisable()
    {
        if (rootGUI != null)
            rootGUI.OnFadeNameHighlighterEvent -= this.OnFadeNameHighlighter;
    }

    public void OnFadeNameHighlighter()
    {
        if (this.highlighterAnimation != null)
        {
            this.highlighterAnimation.ImageRenderer.enabled = false;
            this.highlighterAnimation.enabled = false;
        }
    }

    public void UpdateNameHighlighterAnimation()
    {
        if (this.highlighterAnimation != null)
        {
            this.highlighterAnimation.enabled = true;
            this.highlighterAnimation.ImageRenderer.enabled = true;
            this.highlighterAnimation.Play("Glow");
        }
    }
}
