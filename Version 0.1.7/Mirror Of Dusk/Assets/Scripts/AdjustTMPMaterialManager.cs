using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AdjustTMPMaterialManager
{
    private static bool _initialized = false;
    private static AdjustTMPMaterialManager _tmpManager;

    [SerializeField]
    private AdjustTMPMaterialManager.AdjustTMPMaterialCentralManager tmpManager = new AdjustTMPMaterialManager.AdjustTMPMaterialCentralManager();

    [Serializable]
    public class AdjustTMPMaterialCentralManager
    {
        public List<Material> materials;

        public AdjustTMPMaterialCentralManager()
        {
            this.materials = new List<Material>();
        }

        public Material GetTMPMaterial(Material material)
        {
            for (int i = 0; i < this.materials.Count; i++)
            {
                if (this.materials[i] == material)
                {
                    return this.materials[i];
                }
            }
            this.materials.Add(material);
            return material;
        }
    }

    public AdjustTMPMaterialManager()
    {

    }

    public static bool Initialized
    {
        get
        {
            return AdjustTMPMaterialManager._initialized;
        }
        private set
        {
            AdjustTMPMaterialManager._initialized = value;
        }
    }

    public static AdjustTMPMaterialManager TmpManager
    {
        get
        {
            return AdjustTMPMaterialManager._tmpManager;
        }
    }

    public Material GetTMPMaterial(Material material)
    {
        return this.tmpManager.GetTMPMaterial(material);
    }

    public static void Init()
    {
        if (AdjustTMPMaterialManager.TmpManager == null)
        {
            AdjustTMPMaterialManager._tmpManager = new AdjustTMPMaterialManager();
        }
        AdjustTMPMaterialManager.Initialized = true;
    }
}
