using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AssetLoader<T> : MonoBehaviour
{
    private class AssetContainer<U>
    {
        public U asset;
        public AssetLoaderOptions assetOptions;

        public AssetContainer(U asset, AssetLoaderOptions assetOptions)
        {
            this.asset = asset;
            this.assetOptions = assetOptions;
        }
    }

    private class LoadOperation
    {
        public Coroutine coroutine;
        public List<Action<T>> completionHandlers = new List<Action<T>>();
    }

    protected static AssetLoader<T> Instance;
    [SerializeField] private RuntimeSceneAssetDatabase sceneAssetDatabase;
    private Dictionary<string, AssetLoader<T>.LoadOperation> loadOperations = new Dictionary<string, AssetLoader<T>.LoadOperation>();
    private Dictionary<string, AssetLoader<T>.AssetContainer<T>> loadedAssets = new Dictionary<string, AssetLoader<T>.AssetContainer<T>>();
    private bool _persistentAssetsLoaded;

    public static bool persistentAssetsLoaded
    {
        get
        {
            return AssetLoader<T>.Instance._persistentAssetsLoaded;
        }
        private set
        {
            AssetLoader<T>.Instance._persistentAssetsLoaded = value;
        }
    }

    private void Awake()
    {
        if (AssetLoader<T>.Instance != null)
        {
            throw new Exception("More than one instance found");
        }
        AssetLoader<T>.Instance = this;
    }

    void Start()
    {
        base.StartCoroutine(this.loadPersistentAssets());
    }

    private IEnumerator loadPersistentAssets()
    {
        if (this.sceneAssetDatabase != null)
        {
            foreach(string assetName in sceneAssetDatabase.persistentAssets)
            {
                yield return this.loadAssetFromAssetBundle(assetName, AssetLoaderOptions.PersistInCache, null);
            }
        }
        AssetLoader<T>.persistentAssetsLoaded = true;
        yield break;
    }

    public static String[] GetPreloadAssetNames(string sceneName)
    {
        string[] result;
        if (!AssetLoader<T>.Instance.sceneAssetDatabase.sceneAssetMappings.TryGetValue(sceneName, out result))
        {
            return new string[0];
        }
        return result;
    }

    public static Coroutine LoadAsset(string assetName, AssetLoaderOptions options)
    {
        return AssetLoader<T>.Instance.loadAssetFromAssetBundle(assetName, options, null);
    }

    public static T LoadAssetSynchronous(string assetName, AssetLoaderOptions options)
    {
        return AssetLoader<T>.Instance.loadAssetFromAssetBundleSynchronous(assetName, options);
    }

    protected abstract Coroutine loadAsset(string assetName, Action<T> completionHandler);

    protected abstract T loadAssetSynchronous(string assetName);

    public static T GetCachedAsset(string assetName)
    {
        T result;
        if (AssetLoader<T>.Instance.tryGetAsset(assetName, out result))
        {
            return result;
        }
        throw new Exception("Asset not cached: " + assetName);
    }

    public static void UnloadAssets()
    {
        List<string> list = new List<string>(AssetLoader<T>.Instance.loadedAssets.Keys);
        for (int i = list.Count - 1; i >= 0; i--)
        {
            AssetLoader<T>.AssetContainer<T> assetContainer = AssetLoader<T>.Instance.loadedAssets[list[i]];
            if ((assetContainer.assetOptions & AssetLoaderOptions.PersistInCache) != AssetLoaderOptions.None)
            {
                list.RemoveAt(i);
            } else if ((assetContainer.assetOptions & AssetLoaderOptions.DontDestroyOnUnload) != AssetLoaderOptions.None)
            {
                AssetLoader<T>.Instance.destroyAsset(assetContainer.asset);
            }
        }
        foreach(string key in list)
        {
            AssetLoader<T>.Instance.loadedAssets.Remove(key);
        }
    }

    protected abstract void destroyAsset(T asset);

    private void cacheAsset(string assetName, AssetLoader<T>.AssetContainer<T> container)
    {
        this.loadedAssets.Add(assetName, container);
    }

    protected bool tryGetAsset(string assetName, out T asset)
    {
        asset = default(T);
        AssetLoader<T>.AssetContainer<T> assetContainer;
        if (this.loadedAssets.TryGetValue(assetName, out assetContainer))
        {
            asset = assetContainer.asset;
            return true;
        }
        return false;
    }

    protected Coroutine loadAssetFromAssetBundle(string assetName, AssetLoaderOptions options, Action<T> completionAction)
    {
        T obj;
        if (this.tryGetAsset(assetName, out obj))
        {
            if (completionAction != null)
            {
                completionAction(obj);
            }
            return null;
        }
        AssetLoader<T>.LoadOperation loadOperation;
        if (!this.loadOperations.TryGetValue(assetName, out loadOperation))
        {
            loadOperation = new AssetLoader<T>.LoadOperation();
            this.loadOperations.Add(assetName, loadOperation);
            loadOperation.coroutine = this.loadAsset(assetName, delegate (T asset) {
                this.cacheAsset(assetName, new AssetLoader<T>.AssetContainer<T>(asset, options));
                AssetLoader<T>.LoadOperation loadOperation2 = this.loadOperations[assetName];
                foreach(Action<T> action in loadOperation2.completionHandlers)
                {
                    if (action != null)
                    {
                        action(asset);
                    }
                }
                this.loadOperations.Remove(assetName);
            });
        }
        loadOperation.completionHandlers.Add(completionAction);
        return loadOperation.coroutine;
    }

    protected T loadAssetFromAssetBundleSynchronous(string assetName, AssetLoaderOptions options)
    {
        T result;
        if (this.tryGetAsset(assetName, out result))
        {
            return result;
        }
        T t = this.loadAssetSynchronous(assetName);
        this.cacheAsset(assetName, new AssetLoader<T>.AssetContainer<T>(t, options));
        return t;
    }

    public static List<string> DEBUG_GetLoadedAssets()
    {
        List<string> list = new List<string>(AssetLoader<T>.Instance.loadedAssets.Count);
        foreach (KeyValuePair<string, AssetLoader<T>.AssetContainer<T>> keyValuePair in AssetLoader<T>.Instance.loadedAssets)
        {
            list.Add(string.Format("{0} ({1})", keyValuePair.Key, keyValuePair.Value.assetOptions.ToString()));
        }
        return list;
    }
}
