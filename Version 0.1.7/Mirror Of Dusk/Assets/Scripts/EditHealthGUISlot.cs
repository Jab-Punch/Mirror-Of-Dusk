using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EditHealthGUISlot : MonoBehaviour
{
    [SerializeField] EditHealthGUI rootGUI;
    [SerializeField] OptionsGUIVerticalArrowSet arrowSet;
    [SerializeField] int digitAmount;
    private int selection
    {
        get { return  rootGUI.editedDefaultHealth; }
        set
        {
            rootGUI.editedDefaultHealth = value;
            if (rootGUI.editedDefaultHealth > rootGUI.hpMaxLimit)
                rootGUI.editedDefaultHealth = rootGUI.hpMaxLimit;
            if (rootGUI.editedDefaultHealth < rootGUI.hpMinLimit)
                rootGUI.editedDefaultHealth = rootGUI.hpMinLimit;
            this.UpdateHighlighter();
        }
    }

    public Button button;

    [Serializable]
    public class Button
    {
        [NonSerialized] public bool holdHighlighter = false;

        
    }

    public void incrementSelection()
    {
        if (this.selection < rootGUI.hpMaxLimit)
        {
            AudioManager.Play("menu_scroll");
            this.selection = this.selection + digitAmount;
        }
    }

    public void decrementSelection()
    {
        if (this.selection > rootGUI.hpMinLimit)
        {
            AudioManager.Play("menu_scroll");
            this.selection = this.selection - digitAmount;
        }
    }

    private void OnEnable()
    {
        rootGUI.OnFadeHighlighterEvent += this.OnFadeHighlighter;
        /*if (button.leftArrow != null || button.rightArrow != null)
        {
            rootGUI.OnUpdateArrowAnimationEvent += this.OnUpdateArrowAnimation;
        }*/
    }

    private void OnDisable()
    {
        rootGUI.OnFadeHighlighterEvent -= this.OnFadeHighlighter;
        /*if (button.leftArrow != null || button.rightArrow != null)
        {
            rootGUI.OnUpdateArrowAnimationEvent -= this.OnUpdateArrowAnimation;
        }*/
    }

    public void OnFadeHighlighter()
    {
        if (!this.button.holdHighlighter)
        {
            this.arrowSet.StopAllCoroutines();
            this.arrowSet.state = OptionsGUIVerticalArrowSet.State.Inactive;
            this.arrowSet.canvasGroup.alpha = 0f;
        }
    }

    public void InstantFadeHighlighter()
    {
        this.arrowSet.StopAllCoroutines();
        this.arrowSet.state = OptionsGUIVerticalArrowSet.State.Inactive;
        this.arrowSet.canvasGroup.alpha = 0f;
    }

    public void SummonHighlighter()
    {
        this.arrowSet.canvasGroup.alpha = 1f;
        this.UpdateHighlighter();
    }

    public void UpdateHighlighter()
    {
        OptionsGUIVerticalArrowSet.State readState = OptionsGUIVerticalArrowSet.State.Active;
        if (rootGUI.editedDefaultHealth >= rootGUI.hpMaxLimit)
        {
            readState = OptionsGUIVerticalArrowSet.State.Max;
        }
        else if (rootGUI.editedDefaultHealth <= rootGUI.hpMinLimit)
        {
            readState = OptionsGUIVerticalArrowSet.State.Min;
        }
        if (this.arrowSet.state != readState)
        {
            this.arrowSet.state = readState;
            this.arrowSet.StopAllCoroutines();
            this.arrowSet.SetUpArrow();
            this.arrowSet.SetDownArrow();
        }
    }

    /*private void OnUpdateArrowAnimation()
    {
        if (this.button.leftArrow != null)
        {
            if (this.button.selection > 0)
            {
                this.button.leftArrow.Play("Inactive");
            }
            else
            {
                this.button.leftArrow.Play("Off");
            }
        }
        if (this.button.rightArrow != null)
        {
            if (this.button.selection < this.button.options.Length - 1)
            {
                this.button.rightArrow.Play("Inactive");
            }
            else
            {
                this.button.rightArrow.Play("Off");
            }
        }
    }*/
}
