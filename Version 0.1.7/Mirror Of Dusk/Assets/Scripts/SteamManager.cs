using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
//using Steamworks;
using UnityEngine;

[DisallowMultipleComponent]
internal class SteamManager : MonoBehaviour
{
    private static SteamManager s_instance;
    private static bool s_EverInitialized;
    private bool m_bInitialized;
    //private SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;

    private static SteamManager Instance
    {
        get
        {
            return SteamManager.s_instance ?? new GameObject("SteamManager").AddComponent<SteamManager>();
        }
    }

    public static bool Initialized
    {
        get
        {
            return SteamManager.Instance.m_bInitialized;
        }
    }

    private static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
    {

    }

    private void Awake()
    {
        if (SteamManager.s_instance != null)
        {
            UnityEngine.Object.Destroy(base.gameObject);
            return;
        }
        SteamManager.s_instance = this;
        if (SteamManager.s_EverInitialized)
        {
            throw new Exception("Tried to Initialize the SteamAPI twice in one session!");
        }
        

    }

    void Update()
    {
        if (!this.m_bInitialized)
            return;
        //SteamAPI.RunCallbacks();
    }
}
