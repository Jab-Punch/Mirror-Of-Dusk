using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorMenuGUISlot : MonoBehaviour
{
    [SerializeField] ColorMenuGUI rootGUI;

    public Button button;

    [Serializable]
    public class Button
    {
        public CanvasGroup highlighterCanvasGroup;
        [SerializeField] public int colorCode;
        [SerializeField] public Image ColorA;
        [SerializeField] public Image ColorB;
        [SerializeField] public Image ColorC;
        [NonSerialized] public bool holdHighlighter = false;
        private float horizontalHoldTime = 0;

        public float HorizontalHoldTime
        {
            get
            {
                return horizontalHoldTime;
            }
            set
            {
                horizontalHoldTime = 0f;
            }
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
            base.StartCoroutine(fadeHighlighter_cr());
    }

    private IEnumerator fadeHighlighter_cr()
    {
        float alph = this.button.highlighterCanvasGroup.alpha;
        while (alph > 0f)
        {
            alph -= 0.1f;
            this.button.highlighterCanvasGroup.alpha = Mathf.Clamp(alph, 0f, 1f);
            yield return null;
        }
        yield break;
    }

    public void InstantFadeHighlighter()
    {
        this.button.highlighterCanvasGroup.alpha = 0f;
    }

    public void SummonHighlighter()
    {
        this.StopAllCoroutines();
        base.StartCoroutine(summonHighlighter_cr());
    }

    private IEnumerator summonHighlighter_cr()
    {
        float alph = this.button.highlighterCanvasGroup.alpha;
        while (alph < 1f)
        {
            alph += 0.1f;
            this.button.highlighterCanvasGroup.alpha = Mathf.Clamp(alph, 0f, 1f);
            yield return null;
        }
        yield break;
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
