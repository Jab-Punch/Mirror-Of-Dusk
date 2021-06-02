using System;
using UnityEngine;

public static class DEBUG_AssetLoaderManager
{
    public static Scenes currentDebugScene = Scenes.scene_start;
    public static bool debugUsed = false;
    public static bool debugDone = false;
    public static int language = 0;

    public static bool debugWasFound
    {
        get { return ((DEBUG_AssetLoaderManager.debugUsed && DEBUG_AssetLoaderManager.debugDone) || (!DEBUG_AssetLoaderManager.debugUsed && !DEBUG_AssetLoaderManager.debugDone)); }
    }
}
