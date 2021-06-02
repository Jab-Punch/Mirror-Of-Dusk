using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CustomLanguageLayout : MonoBehaviour
{
    [SerializeField] public List<CustomLanguageLayout.LanguageLayout> customLayouts;
    private RectTransform rectTransform;
    private CustomLanguageLayout.LanguageLayout englishBasicLayout;
    private TextContainer textContainer;

    [Serializable]
    public struct LanguageLayout
    {
        public Localization.Languages languageApplied;
        public bool needCustomOffset;
        public Vector3 positionOffset;
        public bool needCustomWidth;
        public float customWidth;
        public bool needCustomHeight;
        public float customHeight;
    }

    private void Awake()
    {
        this.rectTransform = base.GetComponent<RectTransform>();
        this.textContainer = base.GetComponent<TextContainer>();
        this.englishBasicLayout = default(CustomLanguageLayout.LanguageLayout);
        this.englishBasicLayout.positionOffset = this.rectTransform.localPosition;
        this.englishBasicLayout.customWidth = this.rectTransform.sizeDelta.x;
        this.englishBasicLayout.customHeight = this.rectTransform.sizeDelta.y;
    }

    private void OnDestroy()
    {
        Localization.OnLanguageChangedEvent -= this.ReviewLayout;
    }

    private void OnEnable()
    {
        Localization.OnLanguageChangedEvent += this.ReviewLayout;
        this.ReviewLayout();
    }

    private void OnDisable()
    {
        this.ResetToEnglish();
        Localization.OnLanguageChangedEvent -= this.ReviewLayout;
    }

    private void ReviewLayout()
    {
        if (this.rectTransform == null)
        {
            return;
        }
        int num = 0;
        bool flag = false;
        while (!flag && num < this.customLayouts.Count)
        {
            flag = (this.customLayouts[num].languageApplied == Localization.language);
            num++;
        }
        num--;
        if (flag)
        {
            CustomLanguageLayout.LanguageLayout languageLayout = this.customLayouts[num];
            this.ApplylayoutChanges(languageLayout);
        }
        else
        {
            this.ResetToEnglish();
        }
    }

    private void ResetToEnglish()
    {
        this.rectTransform.localPosition = this.englishBasicLayout.positionOffset;
        if (this.textContainer != null)
        {
            this.textContainer.height = this.englishBasicLayout.customHeight;
            this.textContainer.width = this.englishBasicLayout.customWidth;
        }
        else
        {
            this.rectTransform.sizeDelta = new Vector2(this.englishBasicLayout.customWidth, this.englishBasicLayout.customHeight);
        }
    }

    private void ApplylayoutChanges(CustomLanguageLayout.LanguageLayout languageLayout)
    {
        if (languageLayout.needCustomOffset)
        {
            this.rectTransform.localPosition = new Vector3(this.englishBasicLayout.positionOffset.x + languageLayout.positionOffset.x, this.englishBasicLayout.positionOffset.y + languageLayout.positionOffset.y, this.englishBasicLayout.positionOffset.z + languageLayout.positionOffset.z);
        }
        else
        {
            this.rectTransform.localPosition = new Vector3(this.englishBasicLayout.positionOffset.x, this.englishBasicLayout.positionOffset.y, this.englishBasicLayout.positionOffset.z);
        }
        if (this.textContainer != null)
        {
            this.textContainer.width = ((!languageLayout.needCustomWidth) ? this.englishBasicLayout.customWidth : languageLayout.customWidth);
            this.textContainer.height = ((!languageLayout.needCustomHeight) ? this.englishBasicLayout.customHeight : languageLayout.customHeight);
        }
        else
        {
            float x = (!languageLayout.needCustomWidth) ? this.englishBasicLayout.customWidth : languageLayout.customWidth;
            float y = (!languageLayout.needCustomHeight) ? this.englishBasicLayout.customHeight : languageLayout.customHeight;
            this.rectTransform.sizeDelta = new Vector2(x, y);
        }
    }
}
