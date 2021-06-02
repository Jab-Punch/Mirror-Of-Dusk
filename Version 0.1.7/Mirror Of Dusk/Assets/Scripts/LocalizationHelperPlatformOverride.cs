using System;
using UnityEngine;

public class LocalizationHelperPlatformOverride : MonoBehaviour
{
    public LocalizationHelperPlatformOverride.OverrideInfo[] overrides;

    [Serializable]
    public class OverrideInfo
    {
        public int id;
        public RuntimePlatform platform;
    }

    public bool HasOverrideForCurrentPlatform(out int newID)
    {
        RuntimePlatform platform = Application.platform;
        for (int i = 0; i < this.overrides.Length; i++)
        {
            LocalizationHelperPlatformOverride.OverrideInfo overrideInfo = this.overrides[i];
            if (overrideInfo.platform == platform)
            {
                newID = overrideInfo.id;
                return true;
            }
        }
        newID = -1;
        return false;
    }
}
