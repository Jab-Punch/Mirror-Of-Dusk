using System;

public static class PlatformHelper
{
    public static bool IsConsole
    {
        get { return false; }
    }

    public static bool PreloadSettingsData
    {
        get { return false; }
    }
    
    public static bool ShowAchievements
    {
        get { return false; }
    }
    
    public static bool GarbageCollectOnPause
    {
        get { return false; }
    }
    
    public static bool ForceAdditionalHeapMemory
    {
        get { return false; }
    }
}