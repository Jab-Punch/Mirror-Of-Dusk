using System;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class LocalizationHelper : MonoBehaviour
{
    public bool existingKey;
    public int currentID = -1;
    public Localization.Languages currentLanguage = (Localization.Languages)(-1);
    public Localization.Categories currentCategory;
    public bool currentCustomFont;

    public Text textComponent;
    public Image imageComponent;
    public SpriteRenderer spriteRendererComponent;
    public TMP_Text textMeshProComponent;

    private int initialFontSize;
    private float initialFontAssetSize;

    private bool isInit;
    private LocalizationHelper.LocalizationSubtext[] subTranslations;
    private bool hasOverride;
    private LocalizationHelperPlatformOverride platformOverride;

    public struct LocalizationSubtext
    {
        public string key;
        public string value;
        public bool dontTranslate;

        public LocalizationSubtext(string key, string value, bool dontTranslate = false)
        {
            this.key = key;
            this.value = value;
            this.dontTranslate = dontTranslate;
        }
    }

    private void Init()
    {
        if (this.textComponent != null)
        {
            this.initialFontSize = this.textComponent.fontSize;
        }
        if (this.textMeshProComponent != null)
        {
            this.initialFontAssetSize = this.textMeshProComponent.fontSize;
        }
        this.isInit = true;
    }

    private void Awake()
    {
        this.platformOverride = base.GetComponent<LocalizationHelperPlatformOverride>();
        this.hasOverride = (this.platformOverride != null);
    }

    private void Start()
    {
        Localization.OnLanguageChangedEvent += this.ApplyTranslation;
    }

    private void OnEnable()
    {
        if (DEBUG_AssetLoaderManager.debugWasFound)
        {
            this.ApplyTranslation();
        }
    }

    private void OnDestroy()
    {
        Localization.OnLanguageChangedEvent -= this.ApplyTranslation;
    }

    public void ApplyTranslation()
    {
        int id = this.currentID;
        int num;
        if (this.hasOverride && this.platformOverride.HasOverrideForCurrentPlatform(out num))
        {
            id = num;
        }
        this.ApplyTranslation(Localization.Find(id));
    }

    public void ApplyTranslation(TranslationElement translationElement, LocalizationHelper.LocalizationSubtext[] subTranslations = null)
    {
        this.subTranslations = subTranslations;
        this.ApplyTranslation(translationElement);
    }

    private void ApplyTranslation(TranslationElement translationElement)
    {
        if (!this.isInit)
        {
            this.Init();
        }
        this.currentLanguage = Localization.language;
        if (this.currentLanguage == (Localization.Languages)(-1) || translationElement == null)
        {
            return;
        }
        if (string.IsNullOrEmpty(translationElement.key))
        {
            return;
        }
        Localization.Translation translation = translationElement.translation;
        if (string.IsNullOrEmpty(translation.text))
        {
            translation = Localization.Translate(translationElement.key);
        }
        string text = translation.text;
        if (text != null)
        {
            text = text.Replace("\\n", "\n");
        }
        if (text != null && text.Contains("{") && text.Contains("}"))
        {
            if (this.subTranslations != null)
            {
                bool flag = true;
                while (flag)
                {
                    flag = false;
                    for (int i = 0; i < this.subTranslations.Length; i++)
                    {
                        if (text.Contains("{" + this.subTranslations[i].key + "}"))
                        {
                            flag = true;
                            if (this.subTranslations[i].dontTranslate)
                            {
                                text = text.Replace("{" + this.subTranslations[i].key + "}", this.subTranslations[i].value);
                            }
                            else
                            {
                                Localization.Translation translation2 = Localization.Translate(this.subTranslations[i].value);
                                if (string.IsNullOrEmpty(translation2.text))
                                {
                                    text = text.Replace("{" + this.subTranslations[i].key + "}", this.subTranslations[i].value);
                                }
                                else
                                {
                                    text = text.Replace("{" + this.subTranslations[i].key + "}", translation2.text);
                                }
                            }
                        }
                    }
                }
            }
            string[] array = text.Split(new char[]
            {
                '{'
            });
            if (array.Length > 1)
            {
                string[] array2 = array[1].Split(new char[]
                {
                    '}'
                });
                if (array2.Length > 1)
                {
                    string text2 = array2[0];
                    Localization.Translation translation3 = Localization.Translate(text2);
                    if (!string.IsNullOrEmpty(translation3.text))
                    {
                        text = text.Replace("{" + text2 + "}", translation3.text);
                    }
                }
            }
        }
        if (this.textComponent != null)
        {
            this.textComponent.text = text;
            this.textComponent.enabled = !string.IsNullOrEmpty(text);
            if (translation.hasCustomFont)
            {
                this.textComponent.font = translation.fonts.font;
            }
            else if (Localization.Instance.fonts[(int)this.currentLanguage][(int)translationElement.category].fontType != FontLoader.FontType.None)
            {
                this.textComponent.font = Localization.Instance.fonts[(int)this.currentLanguage][(int)translationElement.category].font;
            }
            this.textComponent.fontSize = ((translation.fonts.fontSize <= 0) ? this.initialFontSize : translation.fonts.fontSize);
        }
        if (this.textMeshProComponent != null)
        {
            this.textMeshProComponent.text = text;
            this.textMeshProComponent.enabled = !string.IsNullOrEmpty(text);
            this.textMeshProComponent.characterSpacing = translation.fonts.charSpacing;
            if (translation.hasCustomFontAsset)
            {
                this.textMeshProComponent.font = translation.fonts.fontAsset;
            }
            else
            {
                this.textMeshProComponent.font = Localization.Instance.fonts[(int)this.currentLanguage][(int)translationElement.category].fontAsset;
            }
            this.textMeshProComponent.fontSize = ((translation.fonts.fontAssetSize <= 0f) ? this.initialFontAssetSize : translation.fonts.fontAssetSize);
        }
        if (this.spriteRendererComponent != null)
        {
            Sprite sprite;
            if (translation.hasSpriteAtlasImage)
            {
                SpriteAtlas cachedAsset = AssetLoader<SpriteAtlas>.GetCachedAsset(translation.spriteAtlasName);
                sprite = cachedAsset.GetSprite(translation.spriteAtlasImageName);
            }
            else
            {
                sprite = translation.image;
            }
            this.spriteRendererComponent.sprite = sprite;
            this.spriteRendererComponent.enabled = false;
            this.spriteRendererComponent.enabled = (sprite != null);
        }
        if (this.imageComponent != null)
        {
            Sprite sprite2;
            if (translation.hasSpriteAtlasImage)
            {
                SpriteAtlas cachedAsset2 = AssetLoader<SpriteAtlas>.GetCachedAsset(translation.spriteAtlasName);
                sprite2 = cachedAsset2.GetSprite(translation.spriteAtlasImageName);
            }
            else
            {
                sprite2 = translation.image;
            }
            this.imageComponent.sprite = sprite2;
            this.imageComponent.enabled = false;
            this.imageComponent.enabled = (sprite2 != null);
        }
    }
}
