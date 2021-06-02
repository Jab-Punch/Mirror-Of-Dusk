using System;

public static class MainMenuItemProperties
{
    public static int GetDisplayId(MainMenuItemSubType menuItem)
    {
        TranslationElement translationElement = Localization.Find(menuItem.ToString() + "_descname");
        if (translationElement == null)
        {
            return -1;
        }
        return translationElement.id;
    }

    public static int GetDescriptionId(MainMenuItemSubType menuItem)
    {
        TranslationElement translationElement = Localization.Find(menuItem.ToString() + "_description");
        if (translationElement == null)
        {
            return -1;
        }
        return translationElement.id;
    }

    public static string GetDisplayName(MainMenuItemMainType menuItem)
    {
        TranslationElement translationElement = Localization.Find(menuItem.ToString() + "_name");
        if (translationElement == null)
        {
            return "ERROR";
        }
        return translationElement.translation.text;
    }

    public static string GetDisplayName(MainMenuItemSubType menuItem)
    {
        TranslationElement translationElement = Localization.Find(menuItem.ToString() + "_name");
        if (translationElement == null)
        {
            return "ERROR";
        }
        return translationElement.translation.text;
    }

    public static float GetDisplayNameFont(MainMenuItemMainType menuItem)
    {
        TranslationElement translationElement = Localization.Find(menuItem.ToString() + "_name");
        if (translationElement == null)
        {
            return 0f;
        }
        return translationElement.translation.fonts.fontAssetSize;
    }

    public static string GetDescName(MainMenuItemSubType menuItem)
    {
        TranslationElement translationElement = Localization.Find(menuItem.ToString() + "_descname");
        if (translationElement == null)
        {
            return "ERROR";
        }
        return translationElement.translation.text;
    }


    public static string GetDescription(MainMenuItemMainType menuItem)
    {
        TranslationElement translationElement = Localization.Find(menuItem.ToString() + "_description");
        if (translationElement == null)
        {
            return "ERROR";
        }
        return translationElement.translation.text;
    }

    public static string GetDescription(MainMenuItemSubType menuItem)
    {
        TranslationElement translationElement = Localization.Find(menuItem.ToString() + "_description");
        if (translationElement == null)
        {
            return "ERROR";
        }
        return translationElement.translation.text;
    }
}
