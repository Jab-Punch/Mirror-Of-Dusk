using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;

public class DEBUG_AssetLoader : MonoBehaviour
{
    [NonSerialized] public Scenes scene_name;
    private bool settingsDataLoaded;

    void Start()
    {
        scene_name = DEBUG_AssetLoaderManager.currentDebugScene;
        base.StartCoroutine(this.start_cr());
    }

    private IEnumerator start_cr()
    {
        yield return null;
        yield return null;
        if (PlatformHelper.ForceAdditionalHeapMemory)
        {
            HeapAllocator.Allocate(100);
            yield return null;
            yield return null;
        }
        if (PlatformHelper.PreloadSettingsData)
        {
            OnlineManager.Instance.Init();
        }
            SettingsData.LoadFromCloud(new SettingsData.SettingsDataLoadFromCloudHandler(this.OnSettingsDataLoaded));
            while (!this.settingsDataLoaded)
            {
                yield return null;
            }
        
        yield return new WaitForSeconds(0.2f);
        SettingsData.Data.language = DEBUG_AssetLoaderManager.language;
        //Unload to avoid existing bundle errors
        AssetBundleLoader.UnloadAssetBundles();
        //Remove line above on final build

        if (!AdjustTMPMaterialManager.Initialized)
            AdjustTMPMaterialManager.Init();

        Coroutine[] fontCoroutines = FontLoader.Initialize();
        foreach (Coroutine coroutine in fontCoroutines)
        {
            yield return coroutine;
        }
        string titleSceneName = "scene_title";
        string[] preloadMusic = AssetLoader<AudioClip>.GetPreloadAssetNames(titleSceneName);
        for (int i = 0; i < preloadMusic.Length; i++)
        {
            AssetLoader<AudioClip>.LoadAsset(preloadMusic[i], AssetLoaderOptions.None);
        }
        string logoSceneName = "scene_logo";
        string[] preloadAtlases = AssetLoader<SpriteAtlas>.GetPreloadAssetNames(logoSceneName);
        for (int i = 0; i < preloadAtlases.Length; i++)
        {
            AssetLoader<SpriteAtlas>.LoadAsset(preloadAtlases[i], AssetLoaderOptions.None);
        }
        while (AssetBundleLoader.loadCounter > 0 || !AssetLoader<SpriteAtlas>.persistentAssetsLoaded || !AssetLoader<AudioClip>.persistentAssetsLoaded)
        {
            yield return null;
        }
        FPSDisplayHandler.Instance.Setup();
        DEBUG_AssetLoaderManager.debugDone = true;
        yield return null;
        SceneLoader.LoadScene(scene_name, SceneLoader.Transition.Fade, SceneLoader.Transition.Fade, SceneLoader.Icon.Shard);
        //SceneManager.LoadSceneAsync(scene_name.ToString());
        yield break;
    }

    private void OnSettingsDataLoaded(bool success)
    {
        if (!success)
        {
            SettingsData.LoadFromCloud(new SettingsData.SettingsDataLoadFromCloudHandler(this.OnSettingsDataLoaded));
            return;
        }
        this.settingsDataLoaded = true;
    }
}
