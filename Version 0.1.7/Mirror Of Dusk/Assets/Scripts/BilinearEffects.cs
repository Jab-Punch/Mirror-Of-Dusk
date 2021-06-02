using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class BilinearEffects : PostEffectsBase
{
    public Shader bilinearShader;
    private Material bilinearMaterial;
    [Range(0.1f, 20f)]
    public float bilinearAmount = 3f;
    [Range(0f, 20f)]
    public float test1 = 0.05f;
    [Range(0f, 20f)]
    public float test2 = 1.2f;
    public int activateAmount = 0;
    private int currentMode = 0;

    public override bool CheckResources()
    {
        base.CheckSupport(false);
        this.bilinearMaterial = base.CheckShaderAndCreateMaterial(this.bilinearShader, this.bilinearMaterial);
        if (!this.isSupported)
        {
            base.ReportAutoDisable();
        }
        return this.isSupported;
    }

    public void OnDisable()
    {
        if (this.bilinearMaterial)
        {
            UnityEngine.Object.DestroyImmediate(this.bilinearMaterial);
        }
    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!this.CheckResources())
        {
            Graphics.Blit(source, destination);
            return;
        }
        //this.bilinearMaterial.SetFloat("_BilinearAmount", bilinearAmount);
        this.bilinearMaterial.SetFloat("_Test1", test1);
        this.bilinearMaterial.SetFloat("_Test2", test2);
        if (activateAmount != currentMode)
        {
            //Shader.SetGlobalInt("_activateBilinear", activateAmount);
            //currentMode = activateAmount;
        }
        source.filterMode = FilterMode.Point;
        RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
        Graphics.Blit(source, temporary, this.bilinearMaterial, 0);
        Graphics.Blit(temporary, destination, this.bilinearMaterial, 0);
        RenderTexture.ReleaseTemporary(temporary);
    }
}
