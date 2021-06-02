using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizationAsset", menuName = "Localization Asset", order = 1)]
public class Localization : ScriptableObject, ISerializationCallbackReceiver
{
    public const int LanguageEnumSize = 9;
    public const string PATH = "LocalizationAsset";

    private static string[] csvKeys = new string[]
    {
        "id",
        "key",
        "category",
        "description",
        "|lang|_text",
        "|lang|_image",
        "|lang|_spriteAtlasName",
        "|lang|_spriteAtlasImageName",
        "|lang|_font",
        "|lang|_fontSize",
        "|lang|_fontAsset",
        "|lang|_fontAssetSize"
    };

    private static Localization _instance;

    public static Localization.Languages language1 = Localization.Languages.English;
    public static Localization.Languages language2 = Localization.Languages.Japanese;

    [SerializeField] private List<TranslationElement> m_TranslationElements = new List<TranslationElement>();
    [SerializeField] public Localization.CategoryLanguageFonts[] m_Fonts;

    [SerializeField]
    public enum Languages
    {
        English,
        Japanese,
        French,
        German,
        Italian,
        Spanish,
        Portuguese,
        Chinese,
        Korean
    }

    [SerializeField]
    public enum Categories
    {
        NoCategory,
        CharacterSelectionName,
        StageSelectionName,
        StageSelectionIn,
        StageSelectionStage,
        MainMenuItems,
        MainMenuDetails,
        PauseMenuItems,
        ResultsMenuTitle,
        ResultsMenuCategories,
        IntroEndingText,
        IntroEndingAction,
        CutscenesText,
        Glyphs,
        TitleScreenSelection,
        Notifications,
        Tutorials,
        OptionMenu,
        UserGUI,
        RemappingMenu,
        RemappingButton,
        AttractScreen,
        JoinPrompt,
        ConfirmMenu,
        DifficultyMenu,
        StageTitles,
        Achievements
    }

    [Serializable]
    public struct Translation
    {
        [SerializeField] public bool hasImage;
        [SerializeField] public string text;
        [SerializeField] public Localization.CategoryLanguageFont fonts;
        [SerializeField] public Sprite image;
        [SerializeField] public string spriteAtlasName;
        [SerializeField] public string spriteAtlasImageName;

        public bool hasSpriteAtlasImage
        {
            get
            {
                return this.spriteAtlasName != null && this.spriteAtlasName.Length > 0 && this.spriteAtlasImageName != null && this.spriteAtlasImageName.Length > 0;
            }
        }

        public bool hasCustomFont
        {
            get
            {
                return this.fonts.fontType != FontLoader.FontType.None;
            }
        }

        public bool hasCustomFontAsset
        {
            get
            {
                return this.fonts.tmpFontType != FontLoader.TMPFontType.None;
            }
        }

        public string SanitizedText()
        {
            return this.text.Replace("\\n", "\n");
        }
    }

    [Serializable]
    public class CategoryLanguageFont
    {
        public int fontSize;
        public FontLoader.FontType fontType;
        public float fontAssetSize;
        public FontLoader.TMPFontType tmpFontType;
        public float charSpacing;

        public Font font
        {
            get { return FontLoader.GetFont(this.fontType); }
        }
        
        public TMP_FontAsset fontAsset
        {
            get { return FontLoader.GetTMPFont(this.tmpFontType); }
        }
    }

    [Serializable]
    public struct CategoryLanguageFonts
    {
        [SerializeField] public Localization.CategoryLanguageFont[] fonts;

        public Localization.CategoryLanguageFont this[int index]
        {
            get
            {
                return this.fonts[index];
            }
            set
            {
                this.fonts[index] = value;
            }
        }
    }

    public delegate void LanguageChanged();

    public static Localization Instance
    {
        get
        {
            if (Localization._instance == null)
            {
                Localization._instance = Resources.Load<Localization>("LocalizationAsset");
            }
            return Localization._instance;
        }
    }

    public static event Localization.LanguageChanged OnLanguageChangedEvent;

    public static Localization.Languages language
    {
        get
        {
            if (SettingsData.Data.language == -1)
            {
                SettingsData.Data.language = (int)DetectLanguage.GetDefaultLanguage();
            }
            return (Localization.Languages)SettingsData.Data.language;
        }
        set
        {
            SettingsData.Data.language = (int)value;
            if (Localization.OnLanguageChangedEvent != null)
            {
                Localization.OnLanguageChangedEvent();
            }
        }
    }

    public static Localization.Translation Translate(string key)
    {
        int id;
        if (Parser.IntTryParse(key, out id))
        {
            return Localization.Translate(id);
        }
        Localization.Translation result = default(Localization.Translation);
        for (int i = 0; i < Localization.Instance.m_TranslationElements.Count; i++)
        {
            if (Localization._instance.m_TranslationElements[i].key == key)
            {
                TranslationElement translationElement = Localization._instance.m_TranslationElements[i];
                result = translationElement.translation;
            }
        }
        return result;
    }

    public static Localization.Translation Translate(int id)
    {
        Localization.Translation result = default(Localization.Translation);
        for (int i = 0; i < Localization.Instance.m_TranslationElements.Count; i++)
        {
            if (Localization._instance.m_TranslationElements[i].id == id)
            {
                TranslationElement translationElement = Localization._instance.m_TranslationElements[i];
                result = translationElement.translation;
            }
        }
        return result;
    }

    public static TranslationElement Find(string key)
    {
        for (int i = 0; i < Localization.Instance.m_TranslationElements.Count; i++)
        {
            if (Localization._instance.m_TranslationElements[i].key == key)
            {
                return Localization._instance.m_TranslationElements[i];
            }
        }
        return null;
    }

    public static TranslationElement Find(int id)
    {
        for (int i = 0; i < Localization.Instance.m_TranslationElements.Count; i++)
        {
            if (Localization._instance.m_TranslationElements[i].id == id)
            {
                return Localization._instance.m_TranslationElements[i];
            }
        }
        return null;
    }

    public static void ExportCsv(string path)
    {
        string text = "|lang|";
        char value = '@';
        string value2 = "\r\n";
        StringBuilder stringBuilder = new StringBuilder();
        int num = Enum.GetNames(typeof(Localization.Languages)).Length;
        int num2 = Enum.GetNames(typeof(Localization.Categories)).Length;
        for (int i = 0; i < Localization.csvKeys.Length; i++)
        {
            if (Localization.csvKeys[i].Contains(text))
            {
                string value3 = Localization.csvKeys[i].Replace(text, string.Empty);
                for (int j = 0; j < num; j++)
                {
                    if (i > 0)
                    {
                        stringBuilder.Append(value);
                    }
                    StringBuilder stringBuilder2 = stringBuilder;
                    Localization.Languages languages = (Localization.Languages)j;
                    stringBuilder2.Append(languages.ToString());
                    stringBuilder.Append(value3);
                }
            }
            else
            {
                if (i > 0)
                {
                    stringBuilder.Append(value);
                }
                stringBuilder.Append(Localization.csvKeys[i]);
            }
        }
        stringBuilder.Append(value2);
        string text4 = string.Empty;
        for (int k = 0; k < Localization.Instance.m_TranslationElements.Count; k++)
        {
            TranslationElement translationElement = Localization._instance.m_TranslationElements[k];
            if (translationElement.depth != -1)
            {
                for (int l = 0; l < Localization.csvKeys.Length; l++)
                {
                    if (Localization.csvKeys[l].Contains(text))
                    {
                        for (int m = 0; m < num; m++)
                        {
                            if (l > 0)
                            {
                                stringBuilder.Append(value);
                            }
                            text4 = string.Empty;
                            string a;
                            Localization.Translation translation;
                            a = Localization.csvKeys[l].Replace(text, string.Empty);
                            translation = translationElement.translations[m];
                            if (a == "_text")
                            {
                                text4 = translation.text;
                                if (!string.IsNullOrEmpty(text4))
                                {
                                    text4 = text4.Replace('\n'.ToString(), '\\' + "n");
                                }
                            }
                            else if (a == "_image")
                            {
                                if (translation.image != null)
                                {
                                    text4 = translation.image.name;
                                }
                            }
                            else if (a == "_spriteAtlasName")
                            {
                                text4 = translation.spriteAtlasName;
                            }
                            else if (a == "_spriteAtlasImageName")
                            {
                                text4 = translation.spriteAtlasImageName;
                            }
                            else if (a == "_font")
                            {
                                if (translation.fonts.fontType != FontLoader.FontType.None)
                                {
                                    text4 = FontLoader.GetFilename(translation.fonts.fontType);
                                }
                            }
                            else if (a == "_fontSize")
                            {
                                text4 = translation.fonts.fontSize.ToString();
                            }
                            else if (a == "_fontAsset")
                            {
                                if (translation.fonts.tmpFontType != FontLoader.TMPFontType.None)
                                {
                                    text4 = FontLoader.GetFilename(translation.fonts.tmpFontType);
                                }
                            }
                            else if (a == "_fontAssetSize")
                            {
                                text4 = translation.fonts.fontAssetSize.ToString();
                            }
                            if (text4 != null)
                            {
                                stringBuilder.Append(text4);
                            }
                        }
                    }
                    else
                    {
                        if (l > 0)
                        {
                            stringBuilder.Append(value);
                        }
                        text4 = string.Empty;
                        string a = Localization.csvKeys[l];
                        if (a == "id")
                        {
                            text4 = translationElement.id.ToString();
                        }
                        else if (a == "key")
                        {
                            text4 = translationElement.key;
                        }
                        else if (a == "category")
                        {
                            text4 = translationElement.category.ToString();
                        }
                        else if (a == "description")
                        {
                            text4 = translationElement.description;
                        }
                        if (text4 != null)
                        {
                            stringBuilder.Append(text4);
                        }
                    }
                }
                stringBuilder.Append(value2);
            }
        }
        Encoding encoding = new UTF8Encoding(true);
        byte[] bytes = encoding.GetBytes(stringBuilder.ToString());
        FileStream fileStream = new FileStream(path, FileMode.Create);
        byte[] preamble = encoding.GetPreamble();
        fileStream.Write(preamble, 0, preamble.Length);
        fileStream.Write(bytes, 0, bytes.Length);
        fileStream.Dispose();
    }

    public static void ImportCsv(string path)
    {
        char c = '@';
        string text = "\r\n";
        string[] names = Enum.GetNames(typeof(Localization.Languages));
        string[] names2 = Enum.GetNames(typeof(Localization.Categories));
        Encoding encoding = new UTF8Encoding(true);
        FileStream fileStream = new FileStream(path, FileMode.Open);
        byte[] preamble = encoding.GetPreamble();
        byte[] array = new byte[preamble.Length];
        fileStream.Read(array, 0, preamble.Length);
        bool flag = true;
        for (int i = 0; i < preamble.Length; i++)
        {
            if (preamble[i] != array[i])
            {
                flag = false;
                break;
            }
        }
        if (flag)
        {
            array = new byte[fileStream.Length - (long)preamble.Length];
            fileStream.Read(array, 0, array.Length);
        }
        else
        {
            array = new byte[fileStream.Length];
            fileStream.Position = 0L;
            fileStream.Read(array, 0, (int)fileStream.Length);
        }
        fileStream.Dispose();
        string @string = encoding.GetString(array);
        string[] array2 = @string.Split(new string[]
        {
            text
        }, StringSplitOptions.RemoveEmptyEntries);
        string[] array3 = array2[0].Split(new char[]
        {
            c
        });
        Localization.Instance.m_TranslationElements.Clear();
        TranslationElement translationElement = new TranslationElement("Root", -1, 0);
        Localization._instance.m_TranslationElements.Add(translationElement);
        for (int j = 1; j < array2.Length; j++)
        {
            string[] array4 = array2[j].Split(new char[]
            {
                c
            });
            if (array4.Length != array3.Length)
            {
                if (array2[j] != string.Empty)
                {
                }
            }
            else
            {
                translationElement = Localization.Instance.AddKey();
                for (int k = 0; k < array4.Length; k++)
                {
                    if (!string.IsNullOrEmpty(array4[k]))
                    {
                        string text4 = array3[k];
                        if (text4 == "id")
                        {
                            translationElement.id = Parser.IntParse(array4[k]);
                        }
                        else if (text4 == "key")
                        {
                            translationElement.key = array4[k];
                        }
                        else if (text4 == "category")
                        {
                            int category = -1;
                            for (int l = 0; l < names2.Length; l++)
                            {
                                if (names2[l] == array4[k])
                                {
                                    category = l;
                                }
                            }
                            translationElement.category = (Localization.Categories)category;
                        }
                        else if (text4 == "description")
                        {
                            translationElement.description = array4[k];
                        }
                        else
                        {
                            for (int m = 0; m < names.Length; m++)
                            {
                                if (text4.Contains(names[m]))
                                {
                                    text4 = text4.Replace(names[m], string.Empty);
                                    Localization.Translation translation;
                                    translation = translationElement.translations[m];
                                    if (translation.fonts == null)
                                    {
                                        translation.fonts = new Localization.CategoryLanguageFont();
                                    }
                                    if (text4 == "_text")
                                    {
                                        translation.text = array4[k];
                                    }
                                    else if (text4 == "_image")
                                    {
                                        if (string.IsNullOrEmpty(array4[k]))
                                        {
                                            break;
                                        }
                                    }
                                    else if (text4 == "_spriteAtlasName")
                                    {
                                        translation.spriteAtlasName = array4[k];
                                    }
                                    else if (text4 == "_spriteAtlasImageName")
                                    {
                                        translation.spriteAtlasImageName = array4[k];
                                    }
                                    else if (text4 == "_font")
                                    {
                                        if (string.IsNullOrEmpty(array4[k]))
                                        {
                                            break;
                                        }
                                    }
                                    else if (text4 == "_fontSize")
                                    {
                                        if (!string.IsNullOrEmpty(array4[k]))
                                        {
                                            int num = Convert.ToInt32(array4[k]);
                                            if (num == 0)
                                            {
                                                break;
                                            }
                                            translation.fonts.fontSize = num;
                                        }
                                    }
                                    else if (text4 == "_fontAsset")
                                    {
                                        if (string.IsNullOrEmpty(array4[k]))
                                        {
                                            break;
                                        }
                                    }
                                    else if (text4 == "_fontAssetSize" && !string.IsNullOrEmpty(array4[k]))
                                    {
                                        float num2 = Convert.ToSingle(array4[k]);
                                        if (num2 == 0f)
                                        {
                                            break;
                                        }
                                        translation.fonts.fontAssetSize = num2;
                                    }
                                    translationElement.translations[m] = translation;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        if (Localization.OnLanguageChangedEvent != null)
        {
            Localization.OnLanguageChangedEvent();
        }
    }

    [SerializeField]
    public Localization.CategoryLanguageFonts[] fonts
    {
        get
        {
            if (this.m_Fonts == null)
            {
                int num = Enum.GetNames(typeof(Localization.Languages)).Length;
                int num2 = Enum.GetNames(typeof(Localization.Categories)).Length;
                this.m_Fonts = new Localization.CategoryLanguageFonts[num];
                for (int i = 0; i < num; i++)
                {
                    this.m_Fonts[i].fonts = new Localization.CategoryLanguageFont[num2];
                }
            }
            return this.m_Fonts;
        }
        set
        {
            this.m_Fonts = value;
        }
    }

    [SerializeField]
    public List<TranslationElement> translationElements
    {
        get
        {
            return this.m_TranslationElements;
        }
        set
        {
            this.m_TranslationElements = value;
        }
    }

    public TranslationElement AddKey()
    {
        int num = -1;
        for (int i = 0; i < this.m_TranslationElements.Count; i++)
        {
            if (this.m_TranslationElements[i].id > num)
            {
                num = this.m_TranslationElements[i].id;
            }
        }
        num++;
        TranslationElement translationElement = new TranslationElement("Key" + num, Localization.Categories.NoCategory, string.Empty, string.Empty, string.Empty, 0, num);
        this.m_TranslationElements.Add(translationElement);
        return translationElement;
    }

    private void Awake()
    {
        if (this.m_TranslationElements.Count == 0)
        {
            this.m_TranslationElements = new List<TranslationElement>(1);
            TranslationElement item = new TranslationElement("Root", -1, 0);
            this.m_TranslationElements.Add(item);
        }
    }

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        bool flag = false;
        int num = Enum.GetNames(typeof(Localization.Languages)).Length;
        if (this.fonts.Length < num)
        {
            flag = true;
        }
        int num2 = Enum.GetNames(typeof(Localization.Categories)).Length;
        if (this.fonts[0].fonts.Length < num2)
        {
            flag = true;
        }
        if (flag)
        {
            this.fonts = this.GrowFonts(this.fonts, num, num2);
        }
    }

    private Localization.CategoryLanguageFonts[] GrowFonts(Localization.CategoryLanguageFonts[] oldFonts, int newLanguagesLength, int newCategoriesLength)
    {
        Localization.CategoryLanguageFonts[] array = new Localization.CategoryLanguageFonts[newLanguagesLength];
        for (int i = 0; i < newLanguagesLength; i++)
        {
            array[i].fonts = new Localization.CategoryLanguageFont[newCategoriesLength];
        }
        for (int j = 0; j < oldFonts.Length; j++)
        {
            for (int k = 0; k < oldFonts[j].fonts.Length; k++)
            {
                array[j][k] = oldFonts[j][k];
            }
        }
        return array;
    }
}
