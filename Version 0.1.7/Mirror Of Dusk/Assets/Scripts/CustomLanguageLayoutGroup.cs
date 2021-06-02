using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomLanguageLayoutGroup : MonoBehaviour
{
    [SerializeField] private HorizontalOrVerticalLayoutGroup layoutComponent;
    [SerializeField] public List<CustomLanguageLayoutGroup.LanguageLayoutGroup> customLayouts;
    
    private CustomLanguageLayoutGroup.LanguageLayoutGroup englishBasicLayout;
    
    [Serializable]
    public struct LanguageLayoutGroup
    {
        public Localization.Languages languageApplied;
        public bool needPadding;
        public RectOffset padding;
        public bool needSpacing;
        public float spacing;
    }
    
    private void Awake()
    {
        this.englishBasicLayout = default(CustomLanguageLayoutGroup.LanguageLayoutGroup);
        this.englishBasicLayout.needPadding = true;
        this.englishBasicLayout.padding = this.layoutComponent.padding;
        this.englishBasicLayout.needSpacing = true;
        this.englishBasicLayout.spacing = this.layoutComponent.spacing;
    }
    
    private void Start()
    {
        Localization.OnLanguageChangedEvent += this.ReviewLayout;
    }
    
    private void OnDestroy()
    {
        Localization.OnLanguageChangedEvent -= this.ReviewLayout;
    }
    
    private void OnEnable()
    {
        this.ReviewLayout();
    }
    
    private void ReviewLayout()
    {
        if (this.layoutComponent == null)
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
            if (this.customLayouts[num].needSpacing)
            {
                this.ApplySpacingChanges(this.customLayouts[num]);
            }
            if (this.customLayouts[num].needPadding)
            {
                this.ApplyPaddingChanges(this.customLayouts[num]);
            }
        }
        else
        {
            this.ApplySpacingChanges(this.englishBasicLayout);
            this.ApplyPaddingChanges(this.englishBasicLayout);
        }
    }
    
    private void ApplySpacingChanges(CustomLanguageLayoutGroup.LanguageLayoutGroup languageLayout)
    {
        this.layoutComponent.spacing = languageLayout.spacing;
    }
    
    private void ApplyPaddingChanges(CustomLanguageLayoutGroup.LanguageLayoutGroup languageLayout)
    {
        this.layoutComponent.padding = languageLayout.padding;
    }
}