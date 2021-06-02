using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TranslationElement : ISerializationCallbackReceiver
{
    [SerializeField] private int m_ID;
    [SerializeField] private int m_Depth;
    [NonSerialized] private TranslationElement m_Parent;
    [NonSerialized] private List<TranslationElement> m_Children;
    [SerializeField] public string key = string.Empty;
    [SerializeField] public Localization.Categories category;
    [SerializeField] public string description = string.Empty;
    [SerializeField] public Localization.Translation[] translations = new Localization.Translation[9];

    public bool enabled;
    [NonSerialized] public bool multiplayerLock;

    public TranslationElement()
    {
    }

    public TranslationElement(string key, int depth, int id)
    {
        this.key = key;
        this.m_ID = id;
        this.m_Depth = depth;
    }

    public TranslationElement(string key, Localization.Categories category, string description, string translation1, string translation2, int depth, int id)
    {
        this.m_ID = id;
        this.m_Depth = depth;
        this.key = key;
        this.category = category;
        this.description = description;
        this.translations[(int)Localization.language1].text = translation1;
        this.translations[(int)Localization.language2].text = translation2;
    }

    public Localization.Translation translation
    {
        get
        {
            return this.translations[(int)Localization.language];
        }
        set
        {
            this.translations[(int)Localization.language] = value;
        }
    }

    public int id
    {
        get { return this.m_ID; }
        set { this.m_ID = value; }
    }

    public int depth
    {
        get { return this.m_Depth; }
        set { this.m_Depth = value; }
    }

    public TranslationElement parent
    {
        get { return this.m_Parent; }
        set { this.m_Parent = value; }
    }

    public List<TranslationElement> children
    {
        get { return this.m_Children; }
        set { this.m_Children = value; }
    }

    public bool hasChildren
    {
        get { return this.children != null && this.children.Count > 0; }
    }

    private Localization.Translation[] Grow(Localization.Translation[] oldTranslations, int newLength)
    {
        Localization.Translation[] array = new Localization.Translation[newLength];
        for (int i = 0; i < oldTranslations.Length; i++)
        {
            array[i].fonts = oldTranslations[i].fonts;
            array[i].image = oldTranslations[i].image;
            array[i].spriteAtlasName = oldTranslations[i].spriteAtlasName;
            array[i].spriteAtlasImageName = oldTranslations[i].spriteAtlasImageName;
            array[i].hasImage = oldTranslations[i].hasImage;
            array[i].text = oldTranslations[i].text;
        }
        for (int j = oldTranslations.Length; j < array.Length; j++)
        {
            array[j].fonts = null;
            array[j].image = null;
            array[j].hasImage = false;
            array[j].text = string.Empty;
            array[j].spriteAtlasName = string.Empty;
            array[j].spriteAtlasImageName = string.Empty;
        }
        return array;
    }

    public void OnBeforeSerialize()
    {
    }

    // Token: 0x060027FC RID: 10236 RVA: 0x00160F14 File Offset: 0x0015F314
    public void OnAfterDeserialize()
    {
        int num = Enum.GetNames(typeof(Localization.Languages)).Length;
        if (this.translations.Length < num)
        {
            this.translations = this.Grow(this.translations, num);
        }
    }
}
