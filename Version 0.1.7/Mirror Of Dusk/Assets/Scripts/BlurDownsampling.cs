using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class BlurDownsampling : PostEffectsBase
{
    public Shader blurShader;
    private Material blurMaterial;
    [Range(1, 16)] public int iterations = 1;

    public override bool CheckResources()
    {
        base.CheckSupport(false);
        this.blurMaterial = base.CheckShaderAndCreateMaterial(this.blurShader, this.blurMaterial);
        if (!this.isSupported)
        {
            base.ReportAutoDisable();
        }
        return this.isSupported;
    }

    public void OnDisable()
    {
        if (this.blurMaterial)
        {
            UnityEngine.Object.DestroyImmediate(this.blurMaterial);
        }
    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!this.CheckResources())
        {
            Graphics.Blit(source, destination);
            return;
        }
        
        int width = source.width;
        int height = source.height;
        RenderTextureFormat format = source.format;

        RenderTexture currentDestination = RenderTexture.GetTemporary(width, height, 0, format);
        Graphics.Blit(source, currentDestination);
        RenderTexture currentSource = currentDestination;

        for (int i = 1; i < iterations; i++) {
            width /= 2;
            height /= 2;
            if (height < 2)
            {
                break;
            }
            currentDestination = RenderTexture.GetTemporary(width, height, 0, format);
            Graphics.Blit(currentSource, currentDestination);
            RenderTexture.ReleaseTemporary(currentSource);
            currentSource = currentDestination;
        }
        Graphics.Blit(currentSource, destination);
        //RenderTexture.ReleaseTemporary(currentSource);

        /*this.blurMaterial.SetFloat("_Brightness", (SettingsData.Data.Brightness / 4f));
        source.filterMode = FilterMode.Point;
        RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
        Graphics.Blit(source, temporary, this.brightMaterial, 0);
        Graphics.Blit(temporary, destination, this.brightMaterial, 0);
        RenderTexture.ReleaseTemporary(temporary);*/
    }
}
