using System;
using System.Collections;
using System.Collections.Generic;
using Rewired.UI.ControlMapper;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsGUISlot : MonoBehaviour
{
    public Button button;

    [Serializable]
    public class Button
    {
        //public Canvas labelCanvas;
        public TextMeshProUGUI labelText;
        public LocalizationHelper labelLocalizationHelper;
        public TextMeshProUGUI text;
        public LocalizationHelper textLocalizationHelper;
        public Image glyph;
        public CanvasGroup highlighterCanvasGroup;
        public GameImageAnimator leftArrow;
        public GameImageAnimator rightArrow;
        public RectTransform sliderFill;
        public string[] options;
        public int selection;
        public bool wrap = true;
        public int detailsCode = -1;
        [SerializeField] public int textSpriteAction1 = -1;
        [SerializeField] public int textSpriteAction2 = -1;
        [SerializeField] public int textSpriteAxis1 = 1;
        [SerializeField] public int textSpriteAxis2 = 1;
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
                if (this.sliderFill != null)
                {
                    horizontalHoldTime = value;
                } else
                {
                    horizontalHoldTime = 0f;
                }
            }
        }

        public void updateSelection(int index)
        {
            this.selection = index;
            if (this.options.Length > 0)
                FindTextLocalizer(this.options[index]);
            /*if (this.textLocalizationHelper == null)
            {
                this.text.text = this.options[index];
            }
            else
            {
                TranslationElement translationElement = Localization.Find(this.options[index]);
                if (translationElement != null) {
                    this.textLocalizationHelper.ApplyTranslation(translationElement, null);
                } else
                {
                    this.text.text = this.options[index];
                }
            }*/
            if (this.leftArrow != null)
            {
                if (index > 0)
                {
                    if (this.leftArrow.CurrentState != "Active")
                        this.leftArrow.Play("Active");
                }
                else
                {
                    this.leftArrow.Play("Off");
                }
            }
            if (this.rightArrow != null)
            {
                if (index < this.options.Length - 1)
                {
                    if (this.rightArrow.CurrentState != "Active")
                        this.rightArrow.Play("Active");
                }
                else
                {
                    this.rightArrow.Play("Off");
                }
            }
            if (this.sliderFill != null)
            {
                if (this.options.Length > 1)
                {
                    this.sliderFill.sizeDelta = new Vector2(Mathf.Round(((float)index / (this.options.Length - 1)) * (200f)), this.sliderFill.sizeDelta.y);
                }
            }
        }

        //For updating selections that are not currently highlighted.
        public void updateNonSelection(int index)
        {
            this.selection = index;
            if (this.options.Length > 0)
                FindTextLocalizer(this.options[index]);
            if (this.leftArrow != null)
            {
                if (index > 0)
                {
                    if (this.leftArrow.CurrentState != "Inactive")
                        this.leftArrow.Play("Inactive");
                }
                else
                {
                    this.leftArrow.Play("Off");
                }
            }
            if (this.rightArrow != null)
            {
                if (index < this.options.Length - 1)
                {
                    if (this.rightArrow.CurrentState != "Inactive")
                        this.rightArrow.Play("Inactive");
                }
                else
                {
                    this.rightArrow.Play("Off");
                }
            }
            if (this.sliderFill != null)
            {
                if (this.options.Length > 1)
                {
                    this.sliderFill.sizeDelta = new Vector2(Mathf.Round(((float)index / (this.options.Length - 1)) * (200f)), this.sliderFill.sizeDelta.y);
                }
            }
        }

        private void FindTextLocalizer(string buttonText)
        {
            if (this.textLocalizationHelper != null)
            {
                TranslationElement translationElement = Localization.Find(buttonText);
                if (translationElement != null)
                {
                    this.textLocalizationHelper.ApplyTranslation(translationElement, null);
                    return;
                }
            }
            this.text.text = buttonText;
        }

        public void incrementSelection()
        {
            if (this.wrap || this.selection < this.options.Length - 1)
            {
                AudioManager.Play("menu_scroll");
                float num = (((float)this.selection / (float)((this.options.Length - 1) + (this.options.Length - 1))) * 2 * (this.options.Length - 1));
                if (this.horizontalHoldTime >= 0.15f && (num % 10) == 0)
                {
                    this.updateSelection((this.selection + 10) % this.options.Length);
                    return;
                }
                this.updateSelection((this.selection + 1) % this.options.Length);
            }
        }

        public void decrementSelection()
        {
            if (this.wrap || this.selection > 0)
            {
                AudioManager.Play("menu_scroll");
                float num = (((float)this.selection / (float)((this.options.Length - 1) + (this.options.Length - 1))) * 2 * (this.options.Length - 1));
                if (this.horizontalHoldTime >= 0.15f && (num % 10) == 0)
                {
                    this.updateSelection((this.selection != 0) ? (this.selection - 10) : (this.options.Length - 1));
                    return;
                }
                this.updateSelection((this.selection != 0) ? (this.selection - 1) : (this.options.Length - 1));
            }
        }

        public void UpdateGlyph(Rewired.Joystick _joystick, int elementIdentifierId, Rewired.AxisRange axisRange)
        {
            if (glyph != null)
            {
                if (_joystick != null)
                {
                    glyph.sprite = ControllerGlyphs.GetGlyph(_joystick, elementIdentifierId, axisRange);
                } else
                {
                    glyph.sprite = ControllerGlyphs.GetDefaultGlyph(elementIdentifierId, axisRange);
                }
                if (glyph.sprite != null)
                {
                    glyph.color = new Color(glyph.color.r, glyph.color.g, glyph.color.b, 1f);
                    if (this.text != null) this.text.text = String.Empty;
                }
                else
                {
                    glyph.color = new Color(glyph.color.r, glyph.color.g, glyph.color.b, 0f);
                }
            }
        }
    }

    private void Start()
    {
        /*RectTransform rekt = this.button.labelCanvas.gameObject.GetComponent<RectTransform>();
        rekt.anchorMin = new Vector2(0f, 0f);
        rekt.anchorMax = new Vector2(1f, 1f);
        rekt.offsetMin = new Vector2(0f, 0f);
        rekt.offsetMax = new Vector2(0f, 0f);
        rekt.ForceUpdateRectTransforms();
        this.button.labelCanvas.sortingLayerName = this.transform.root.GetComponent<Canvas>().sortingLayerName;*/
        //this.button.labelCanvas.renderMode = RenderMode.ScreenSpaceCamera;
    }

    private void OnEnable()
    {
        OptionsGUI.Current.OnFadeHighlighterEvent += this.OnFadeHighlighter;
        if (button.leftArrow != null || button.rightArrow != null)
        {
            OptionsGUI.Current.OnUpdateArrowAnimationEvent += this.OnUpdateArrowAnimation;
        }
    }

    private void OnDisable()
    {
        OptionsGUI.Current.OnFadeHighlighterEvent -= this.OnFadeHighlighter;
        if (button.leftArrow != null || button.rightArrow != null)
        {
            OptionsGUI.Current.OnUpdateArrowAnimationEvent -= this.OnUpdateArrowAnimation;
        }
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

    private void OnUpdateArrowAnimation()
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
    }

    public void UpdateArrowsVertical()
    {
        if (this.button.leftArrow != null)
        {
            if (this.button.selection > 0)
            {
                this.button.leftArrow.Play("Active");
            } else
            {
                this.button.leftArrow.Play("Off");
            }
        }
        if (this.button.rightArrow != null)
        {
            if (this.button.selection < this.button.options.Length - 1)
            {
                this.button.rightArrow.Play("Active");
            }
            else
            {
                this.button.rightArrow.Play("Off");
            }
        }
    }
}
