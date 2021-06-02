using System;
using System.Globalization;

public static class DetectLanguage
{
    public static Localization.Languages GetDefaultLanguage()
    {
        Localization.Languages result = Localization.Languages.English;
        DetectLanguage.getDefaultLanguage(ref result);
        return result;
    }

    private static void getDefaultLanguage(ref Localization.Languages defaultLanguage)
    {
        CultureInfo currentUICulture = CultureInfo.CurrentUICulture;
        string twoLetterISOLanguageName = currentUICulture.TwoLetterISOLanguageName;
        if (twoLetterISOLanguageName == "ja")
        {
            defaultLanguage = Localization.Languages.Japanese;
        }
        else if (twoLetterISOLanguageName == "fr")
        {
            defaultLanguage = Localization.Languages.French;
        }
        else if (twoLetterISOLanguageName == "de")
        {
            defaultLanguage = Localization.Languages.German;
        }
        else if (twoLetterISOLanguageName == "it")
        {
            defaultLanguage = Localization.Languages.Italian;
        }
        else if (twoLetterISOLanguageName == "es")
        {
            defaultLanguage = Localization.Languages.Spanish;
        }
        else if (twoLetterISOLanguageName == "pt-BR")
        {
            defaultLanguage = Localization.Languages.Portuguese;
        }
        else if (twoLetterISOLanguageName == "zh")
        {
            defaultLanguage = Localization.Languages.Chinese;
        }
        else if (twoLetterISOLanguageName == "ko")
        {
            defaultLanguage = Localization.Languages.Korean;
        }
        else
        {
            defaultLanguage = Localization.Languages.English;
        }
    }
}
