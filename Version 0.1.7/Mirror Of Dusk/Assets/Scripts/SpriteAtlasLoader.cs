using System;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteAtlasLoader : AssetLoader<SpriteAtlas>
{
    private void OnEnable()
    {
        SpriteAtlasManager.atlasRequested += this.atlasRequestedHandler;
    }

    private void OnDisable()
    {
        SpriteAtlasManager.atlasRequested -= this.atlasRequestedHandler;
    }

    protected override Coroutine loadAsset(string assetName, Action<SpriteAtlas> completionHandler)
    {
        return AssetBundleLoader.LoadSpriteAtlas(assetName, completionHandler);
    }

    protected override SpriteAtlas loadAssetSynchronous(string assetName)
    {
        throw new NotImplementedException();
    }

    protected override void destroyAsset(SpriteAtlas asset)
    {
        UnityEngine.Object.Destroy(asset);
    }

    private void atlasRequestedHandler(string atlasTag, Action<SpriteAtlas> action)
    {
        base.loadAssetFromAssetBundle(atlasTag, AssetLoaderOptions.None, action);
    }
}
