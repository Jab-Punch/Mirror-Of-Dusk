using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;

public class AssetBundleLoader : MonoBehaviour
{
    public static int loadCounter { get; private set; }
    private static AssetBundleLoader Instance;
    private Dictionary<string, AssetBundle> loadedBundles = new Dictionary<string, AssetBundle>();

    public static List<string> DEBUG_LoadedAssetBundles()
    {
        return new List<string>(AssetBundleLoader.Instance.loadedBundles.Keys);
    }

    private void Awake()
    {
        if (AssetBundleLoader.Instance != null)
        {
            throw new Exception("Should only be one instance");
        }
        AssetBundleLoader.Instance = this;
        //UnityEditor.BuildPipeline.BuildAssetBundles("Assets/AssetBundles", UnityEditor.BuildAssetBundleOptions.None, UnityEditor.BuildTarget.StandaloneWindows);
        //Debug.Log(Application.streamingAssetsPath);
    }

    public static void UnloadAssetBundles()
    {
        foreach (KeyValuePair<string, AssetBundle> keyValuePair in AssetBundleLoader.Instance.loadedBundles)
        {
            keyValuePair.Value.Unload(false);
        }
        AssetBundleLoader.Instance.loadedBundles.Clear();
    }

    public static Coroutine LoadSpriteAtlas(string atlasName, Action<SpriteAtlas> completionHandler)
    {
        string assetBundleName = "atlas_" + atlasName.ToLowerInvariant();
        return AssetBundleLoader.Instance.StartCoroutine(AssetBundleLoader.Instance.loadAsset<SpriteAtlas>(assetBundleName, atlasName, completionHandler));
    }

    public static Coroutine LoadMusic(string audioClipName, Action<AudioClip> completionHandler)
    {
        string assetBundleName = "music_" + audioClipName.ToLowerInvariant();
        return AssetBundleLoader.Instance.StartCoroutine(AssetBundleLoader.Instance.loadAsset<AudioClip>(assetBundleName, audioClipName, completionHandler));
    }

    public static Coroutine LoadFont(string bundleName, string assetName, Action<Font> completionHandler)
    {
        bundleName = "font_" + bundleName.ToLowerInvariant();
        return AssetBundleLoader.Instance.StartCoroutine(AssetBundleLoader.Instance.loadAsset<Font>(bundleName, assetName, completionHandler));
    }

    public static Coroutine LoadTMPFont(string bundleName, Action<UnityEngine.Object[]> completionHandler)
    {
        bundleName = "tmpfont_" + bundleName.ToLowerInvariant();
        return AssetBundleLoader.Instance.StartCoroutine(AssetBundleLoader.Instance.loadAllAssets<UnityEngine.Object>(bundleName, completionHandler));
    }

    public static Coroutine LoadTextures(string bundleName, Action<Texture2D[]> completionHandler)
    {
        bundleName = "tex_" + bundleName.ToLowerInvariant();
        return AssetBundleLoader.Instance.StartCoroutine(AssetBundleLoader.Instance.loadAllAssets<Texture2D>(bundleName, completionHandler));
    }

    public static Texture2D[] LoadTexturesSynchronous(string bundleName)
    {
        bundleName = "tex_" + bundleName.ToLowerInvariant();
        return AssetBundleLoader.Instance.loadAllAssetsSynchronous<Texture2D>(bundleName);
    }

    private IEnumerator loadAssetBundle(string assetBundleName)
    {
        AssetBundleLoader.loadCounter++;
        string path = Path.Combine(Application.streamingAssetsPath, "AssetBundles");
        path = Path.Combine(path, assetBundleName);
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);
        yield return request;
        this.loadedBundles.Add(assetBundleName, request.assetBundle);
        AssetBundleLoader.loadCounter--;
        yield break;
    }

    private AssetBundle loadAssetBundleSynchronous(string assetBundleName)
    {
        string text = Path.Combine(Application.streamingAssetsPath, "AssetBundles");
        text = Path.Combine(text, assetBundleName);
        AssetBundle assetBundle = AssetBundle.LoadFromFile(text);
        this.loadedBundles.Add(assetBundleName, assetBundle);
        return assetBundle;
    }

    private IEnumerator loadAsset<T>(string assetBundleName, string assetName, Action<T> completionHandler) where T : UnityEngine.Object
    {
        AssetBundleLoader.loadCounter++;
        AssetBundle assetBundle;
        if (!this.loadedBundles.TryGetValue(assetBundleName, out assetBundle))
        {
            yield return base.StartCoroutine(this.loadAssetBundle(assetBundleName));
            assetBundle = this.loadedBundles[assetBundleName];
        }
        AssetBundleRequest assetRequest = assetBundle.LoadAssetAsync<T>(assetName);
        yield return assetRequest;
        completionHandler(assetRequest.asset as T);
        AssetBundleLoader.loadCounter--;
        yield break;
    }

    private IEnumerator loadAllAssets<T>(string assetBundleName, Action<T[]> completionHandler) where T : UnityEngine.Object
    {
        AssetBundleLoader.loadCounter++;
        AssetBundle assetBundle;
        if (!this.loadedBundles.TryGetValue(assetBundleName, out assetBundle))
        {
            yield return base.StartCoroutine(this.loadAssetBundle(assetBundleName));
            assetBundle = this.loadedBundles[assetBundleName];
        }
        AssetBundleRequest assetRequest = assetBundle.LoadAllAssetsAsync<T>();
        yield return assetRequest;
        UnityEngine.Object[] allAssets = assetRequest.allAssets;
        T[] castAssets = new T[allAssets.Length];
        for (int i = 0; i < allAssets.Length; i++)
        {
            castAssets[i] = (T)((object)allAssets[i]);
        }
        completionHandler(castAssets);
        AssetBundleLoader.loadCounter--;
        yield break;
    }

    public T[] loadAllAssetsSynchronous<T>(string assetBundleName) where T : UnityEngine.Object
    {
        AssetBundle assetBundle;
        if (!this.loadedBundles.TryGetValue(assetBundleName, out assetBundle))
        {
            assetBundle = this.loadAssetBundleSynchronous(assetBundleName);
        }
        return assetBundle.LoadAllAssets<T>();
    }
}
