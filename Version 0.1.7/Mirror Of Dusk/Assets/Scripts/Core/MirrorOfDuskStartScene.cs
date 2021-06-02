using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;

public class MirrorOfDuskStartScene : MonoBehaviour
{
    private bool settingsDataLoaded;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        MirrorOfDusk.Init(true);
    }
    
    void Start()
    {
        base.StartCoroutine(this.start_cr());
    }

    private IEnumerator start_cr()
    {
        yield return null;
        yield return null;
        //AssetLoader<Texture2D[]>.LoadAssetSynchronous("screen_fx", AssetLoaderOptions.DontDestroyOnUnload);
        //UnityEngine.Object.FindObjectOfType<ChromaticAberrationFilmGrain>().Initialize(AssetLoader<Texture2D[]>.GetCachedAsset("screen_fx"));
        if (PlatformHelper.ForceAdditionalHeapMemory)
        {
            HeapAllocator.Allocate(100);
            yield return null;
            yield return null;
        }
        if (PlatformHelper.PreloadSettingsData)
        {
            OnlineManager.Instance.Init();
            SettingsData.LoadFromCloud(new SettingsData.SettingsDataLoadFromCloudHandler(this.OnSettingsDataLoaded));
            while (!this.settingsDataLoaded)
            {
                yield return null;
            }
        }
        yield return new WaitForSeconds(0.2f);
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
        /*while (AssetBundleLoader.loadCounter > 0 || !AssetLoader<SpriteAtlas>.persistentAssetsLoaded || !AssetLoader<AudioClip>.persistentAssetsLoaded || !AssetLoader<Texture2D[]>.persistentAssetsLoaded)
        {
            yield return null;
        }*/
        while (AssetBundleLoader.loadCounter > 0 || !AssetLoader<SpriteAtlas>.persistentAssetsLoaded || !AssetLoader<AudioClip>.persistentAssetsLoaded || !AssetLoader<Texture2D[]>.persistentAssetsLoaded)
        {
            yield return null;
        }
        FPSDisplayHandler.Instance.Setup();
        yield return null;
        SceneManager.LoadSceneAsync(1);
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
