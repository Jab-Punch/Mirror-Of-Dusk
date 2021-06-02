using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;

public class DEBUG_StartAssetLoader : MonoBehaviour
{
    [SerializeField] Scenes debugScene;
    [SerializeField] int debugLanguage;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        MirrorOfDusk.Init();
        if (!DEBUG_AssetLoaderManager.debugUsed)
        {
            DEBUG_AssetLoaderManager.debugUsed = true;
            DEBUG_AssetLoaderManager.language = debugLanguage;
            DEBUG_AssetLoaderManager.currentDebugScene = debugScene;
            SettingsData.ApplySettingsOnStartup();
            SceneManager.LoadScene("scene_debug_loader");
        }
    }

    private void Start()
    {
        if (DEBUG_AssetLoaderManager.debugWasFound)
        {
            SettingsData.ApplySettingsOnStartup();
        }
    }
}
