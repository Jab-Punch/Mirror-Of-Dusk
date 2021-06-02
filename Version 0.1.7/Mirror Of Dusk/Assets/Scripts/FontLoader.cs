using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class FontLoader
{
    private static Dictionary<FontLoader.FontType, string> FontTypeMapping = new Dictionary<FontLoader.FontType, string> {
        {FontLoader.FontType.Arial, "Arial" },
        {FontLoader.FontType.Source_Han_Serif_JP_Light, "source_han_serif_jp_light" },
        {FontLoader.FontType.Source_Han_Serif_JP, "source_han_serif_jp" }
    };

    private static Dictionary<FontLoader.TMPFontType, string> TMPFontTypeMapping = new Dictionary<FontLoader.TMPFontType, string> {
        {FontLoader.TMPFontType.NotoSerifCJK, "NotoSerifCJK" },
        {FontLoader.TMPFontType.NotoSerifCJK_EnterText, "NotoSerifCJK EnterText" },
        {FontLoader.TMPFontType.Source_Han_Serif_Jp_MainMenu, "source_han_serif_jp_mainmenu" },
        {FontLoader.TMPFontType.Source_Han_Serif_Jp_GoudosDetail, "source_han_serif_jp_goudosdetail" },
        {FontLoader.TMPFontType.Goudosi_MainMenu, "Goudosi MainMenu" },
        {FontLoader.TMPFontType.Goudosi_SmallDetail, "Goudosi SmallDetail" },
        {FontLoader.TMPFontType.Goudosi_MainMenu_Selected, "Goudosi MainMenu Selected" },
        {FontLoader.TMPFontType.Source_Han_Serif_Jp_MainMenu_Selected, "source_han_serif_jp_mainmenu_selected" },
        {FontLoader.TMPFontType.Menu_Label_Source_Han_Serif, "Menu Label Source Han Serif" },
        {FontLoader.TMPFontType.Menu_Letter_Source_Han_Serif, "Menu Letter Source Han Serif" },
        {FontLoader.TMPFontType.Goudosi_CharacterSelect_Title_Ro, "Goudosi CharacterSelect Title Ro" },
        {FontLoader.TMPFontType.Goudosi_CharacterSelect_Title_Jp, "Goudosi CharacterSelect Title Jp" }
    };

    private static Dictionary<FontLoader.FontType, Font> FontCache = new Dictionary<FontLoader.FontType, Font>();
    private static Dictionary<FontLoader.TMPFontType, TMP_FontAsset> TMPFontCache = new Dictionary<FontLoader.TMPFontType, TMP_FontAsset>();
    private static Dictionary<string, Material> TMPMaterialCache = new Dictionary<string, Material>();

    public enum FontType
    {
        None,
        Arial,
        Source_Han_Serif_JP_Light,
        Source_Han_Serif_JP
    }

    public enum TMPFontType
    {
        None,
        NotoSerifCJK,
        NotoSerifCJK_EnterText,
        Source_Han_Serif_Jp_MainMenu,
        Source_Han_Serif_Jp_GoudosDetail,
        Goudosi_MainMenu,
        Goudosi_SmallDetail,
        Goudosi_MainMenu_Selected,
        Source_Han_Serif_Jp_MainMenu_Selected,
        Menu_Label_Source_Han_Serif,
        Menu_Letter_Source_Han_Serif,
        Goudosi_CharacterSelect_Title_Ro,
        Goudosi_CharacterSelect_Title_Jp
    }

    public static Coroutine[] Initialize()
    {
        Array values = Enum.GetValues(typeof(FontLoader.FontType));
        Array values2 = Enum.GetValues(typeof(FontLoader.TMPFontType));
        Coroutine[] array = new Coroutine[values.Length + values2.Length - 2];
        for (int i = 1; i < values.Length; i++)
        {
            FontLoader.FontType fontType = (FontLoader.FontType)values.GetValue(i);
            string bundleName = fontType.ToString();
            Action<Font> completionHandler = delegate (Font font)
            {
                FontLoader.FontType fontTypeB = fontType;
                FontLoader.FontCache.Add(fontTypeB, font);
            };
            array[i - 1] = AssetBundleLoader.LoadFont(bundleName, FontLoader.GetFilename(fontType), completionHandler);
        }
        for (int j = 1; j < values2.Length; j++)
        {
            FontLoader.TMPFontType fontType = (FontLoader.TMPFontType)values2.GetValue(j);
            string bundleName2 = fontType.ToString();
            Action<UnityEngine.Object[]> completionHandler2 = delegate (UnityEngine.Object[] objects)
            {
                foreach (UnityEngine.Object @object in objects)
                {
                    if (@object is TMP_FontAsset)
                    {
                        FontLoader.TMPFontType fontTypeB = fontType;
                        FontLoader.TMPFontCache.Add(fontTypeB, (TMP_FontAsset)@object);
                    } else
                    {
                        if (!(@object is Material))
                        {
                            if (!(@object is Texture2D))
                            {
                                throw new Exception("Unhandled object type: " + @object.GetType());
                            } else
                            {
                                continue;
                            }
                        }
                        Debug.Log(@object.name);
                        FontLoader.TMPMaterialCache.Add(@object.name, (Material)@object);
                    }
                }
            };
            array[j - 1 + (values.Length - 1)] = AssetBundleLoader.LoadTMPFont(bundleName2, completionHandler2);
        }
        return array;
    }

    public static Font GetFont(FontLoader.FontType fontType)
    {
        if (fontType == FontLoader.FontType.None)
        {
            return null;
        }
        return FontLoader.FontCache[fontType];
    }

    public static TMP_FontAsset GetTMPFont(FontLoader.TMPFontType fontType)
    {
        if (fontType == FontLoader.TMPFontType.None)
        {
            return null;
        }
        return FontLoader.TMPFontCache[fontType];
    }

    public static Material GetTMPMaterial(string materialName)
    {
        return FontLoader.TMPMaterialCache[materialName];
    }

    public static string GetFilename(FontLoader.FontType fontType)
    {
        return FontLoader.FontTypeMapping[fontType];
    }

    public static string GetFilename(FontLoader.TMPFontType fontType)
    {
        return FontLoader.TMPFontTypeMapping[fontType];
    }
}
