using System;

[Flags]
public enum AssetLoaderOptions
{
    None = 0,
    PersistInCache = 1,
    DontDestroyOnUnload = 2
}
