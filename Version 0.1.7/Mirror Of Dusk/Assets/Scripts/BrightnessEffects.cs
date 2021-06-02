using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class BrightnessEffects : PostEffectsBase
{
    public Shader brightShader;
    private Material brightMaterial;

    public override bool CheckResources()
    {
        base.CheckSupport(false);
        this.brightMaterial = base.CheckShaderAndCreateMaterial(this.brightShader, this.brightMaterial);
        if (!this.isSupported)
        {
            base.ReportAutoDisable();
        }
        return this.isSupported;
    }

    public void OnDisable()
    {
        if (this.brightMaterial)
        {
            UnityEngine.Object.DestroyImmediate(this.brightMaterial);
        }
    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!this.CheckResources())
        {
            Graphics.Blit(source, destination);
            return;
        }
        if (MirrorOfDuskRenderer.Instance.R_Camera != null)
        {
            this.brightMaterial.SetVector(MirrorOfDuskRenderer.Instance.R_Camera.perStillCameraBuffer._ScaledScreenParams, new Vector4(MirrorOfDuskRenderer.Instance.R_Camera.cameraWidth, 
                MirrorOfDuskRenderer.Instance.R_Camera.cameraHeight, 1.0f + 1.0f / MirrorOfDuskRenderer.Instance.R_Camera.cameraWidth, 1.0f + 1.0f / MirrorOfDuskRenderer.Instance.R_Camera.cameraHeight));
                
        }
        this.brightMaterial.SetFloat("_Brightness", (SettingsData.Data.Brightness / 4f));
        source.filterMode = FilterMode.Point;
        RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
        Graphics.Blit(source, temporary, this.brightMaterial, 0);
        Graphics.Blit(temporary, destination, this.brightMaterial, 0);
        RenderTexture.ReleaseTemporary(temporary);
    }
}
