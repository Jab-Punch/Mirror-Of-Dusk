using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Runtime Scene Asset Database", menuName = "Runtime Scene Asset Database")]
public class RuntimeSceneAssetDatabase : ScriptableObject
{
    [Serializable]
    public class SceneAssetMapping
    {
        public string sceneName;
        public string[] assetNames;
    }

    public string[] INTERNAL_persistentAssetNames;
    public RuntimeSceneAssetDatabase.SceneAssetMapping[] INTERNAL_sceneAssetMappings;
    private HashSet<string> _persistentAssets;
    private Dictionary<string, string[]> _sceneAssetMappings;

    public HashSet<string> persistentAssets
    {
        get
        {
            if (this._persistentAssets == null)
            {
                this._persistentAssets = new HashSet<string>(this.INTERNAL_persistentAssetNames);
            }
            return this._persistentAssets;
        }
    }

    public Dictionary<string, string[]> sceneAssetMappings
    {
        get
        {
            if (this._sceneAssetMappings == null)
            {
                this._sceneAssetMappings = new Dictionary<string, string[]>();
                foreach (RuntimeSceneAssetDatabase.SceneAssetMapping sceneAssetMapping in this.INTERNAL_sceneAssetMappings)
                {
                    this._sceneAssetMappings.Add(sceneAssetMapping.sceneName, sceneAssetMapping.assetNames);
                }
            }
            return this._sceneAssetMappings;
        }
    }
}
