using System;
using TMPro;
using UnityEngine;

public class AdjustTMPMaterial : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private AdjustTMPMaterial.MaterialData[] materials;
    private Localization.Languages previousLanguage;
    private bool initialSetupComplete;

    [Serializable]
    public struct MaterialData
    {
        public Localization.Languages language;
        public string materialName;
        public bool selectedVariant;
    }

    private void Update()
    {
        if (!this.initialSetupComplete || Localization.language != this.previousLanguage)
        {
            this.initialSetupComplete = true;
            this.previousLanguage = Localization.language;
            Localization.Languages language = Localization.language;

            //Material material = this.getMaterial(language);
            Material material = AdjustTMPMaterialManager.TmpManager.GetTMPMaterial(this.getMaterial(language));
            if (material != null)
            {
                material.enableInstancing = true;
                //Debug.Log(material.GetTexture("_MainTex"));
                //this.text.fontMaterial._MainTex = 
                /*if (this.text.gameObject.name == "mainform_titletext")
                {
                    this.text.gameObject.GetComponent<CanvasRenderer>().SetTexture(material.GetTexture("_MainTex"));
                } else
                {
                    this.text.fontMaterial = AdjustTMPMaterialManager.TmpManager.GetTMPMaterial(material);
                }*/
                if (material.name == "menu_label_source_han_serif")
                {
                    Debug.Log(material);
                    Debug.Log(material.GetFloat("_UnderlayOffsetX"));
                }
                this.text.gameObject.GetComponent<CanvasRenderer>().SetTexture(material.GetTexture("_MainTex"));
                //this.text.fontMaterial.enableInstancing = true;
                //this.text.fontSharedMaterial = AdjustTMPMaterialManager.TmpManager.GetTMPMaterial(this.getMaterial(language));
                //this.text.fontSharedMaterial.enableInstancing = true;
                //Debug.Log(this.text.fontMaterial.shader.GetInstanceID());
            }
        }
    }
    
    public Material getMaterial(Localization.Languages language, bool selectedVar = false)
    {
        foreach (AdjustTMPMaterial.MaterialData materialData in this.materials)
        {
            if (materialData.language == language && materialData.selectedVariant == selectedVar)
            {
                return FontLoader.GetTMPMaterial(materialData.materialName);
            }
        }
        return this.defaultMaterial;
    }
}