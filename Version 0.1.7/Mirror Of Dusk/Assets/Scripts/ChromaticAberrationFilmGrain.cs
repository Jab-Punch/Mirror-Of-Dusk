using System;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

// Token: 0x020002B7 RID: 695
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class ChromaticAberrationFilmGrain : PostEffectsBase
{
    public Shader shader;
    public Material material;

    private const float FRAME_TIME = 0.025f;
    private Vector4 UV_Transform = new Vector4(1f, 0f, 0f, 1f);
    public float intensity = 1f;
    public bool animated = true;
    public int earlyLoopPoint = 5;

    private int currentTexture;
    public Vector2 r;
    public Vector2 g;
    public Vector2 b;
    private Texture2D[] textures;
    private Vector2 rStart;
    private Vector2 gStart;
    private Vector2 bStart;
    
    public void Initialize(Texture2D[] filmGrain)
    {
        base.enabled = true;
        this.rStart = this.r;
        this.gStart = this.g;
        this.bStart = this.b;
        if (!SystemInfo.supportsImageEffects)
        {
            base.enabled = false;
            return;
        }
        this.textures = filmGrain;
        base.StartCoroutine(this.animate_cr());
    }
    
    private IEnumerator animate_cr()
    {
        float t = 0f;
        int loopsUntilFullLoop = UnityEngine.Random.Range(7, 15);
        for (; ; )
        {
            t += Time.deltaTime;
            while (t > 0.025f)
            {
                t -= 0.025f;
                if (this.animated)
                {
                    this.currentTexture++;
                    if (loopsUntilFullLoop > 0)
                    {
                        if (this.currentTexture >= this.earlyLoopPoint)
                        {
                            this.currentTexture = 0;
                            loopsUntilFullLoop--;
                            this.UV_Transform = new Vector4((float)MathUtils.PlusOrMinus(), 0f, 0f, (float)MathUtils.PlusOrMinus());
                        }
                    }
                    else if (this.currentTexture >= this.textures.Length)
                    {
                        this.currentTexture = 0;
                        loopsUntilFullLoop = UnityEngine.Random.Range(7, 15);
                        this.UV_Transform = new Vector4((float)MathUtils.PlusOrMinus(), 0f, 0f, (float)MathUtils.PlusOrMinus());
                    }
                }
            }
            yield return null;
        }
        yield break;
    }
    
    public override bool CheckResources()
    {
        base.CheckSupport(false);
        this.material = base.CheckShaderAndCreateMaterial(this.shader, this.material);
        if (!this.isSupported)
        {
            base.ReportAutoDisable();
        }
        return this.isSupported;
    }
    
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!this.CheckResources())
        {
            Graphics.Blit(source, destination);
            return;
        }
        this.material.SetVector("_UV_Transform", this.UV_Transform);
        this.material.SetFloat("_Intensity", this.intensity);
        if (this.textures != null && this.textures.Length > this.currentTexture && this.textures[this.currentTexture] != null)
        {
            this.material.SetTexture("_Overlay", this.textures[this.currentTexture]);
        }
        float num = (float)source.width / (float)source.height;
        float num2 = (num >= 1.7777778f) ? 1f : (num / 1.7777778f);
        num2 *= 1f - 0.1f * 0f;
        float d = 1f * num2 * (float)source.height / 1080f;
        Vector2 v = this.r * d;
        Vector2 vector = this.g * d;
        Vector2 vector2 = this.b * d;
        /*this.material.SetVector("_Screen", new Vector2((float)source.width, (float)source.height));
        this.material.SetVector("_Red", v);
        this.material.SetVector("_Green", vector);
        this.material.SetVector("_Blue", vector2);*/
        this.material.SetFloat("_Hue", -160f);
        Debug.Log(this.material.GetFloat("_Hue"));
        int num3 = 0;
        Debug.Log("S:"+source);
        Debug.Log("D:"+destination);
        Graphics.Blit(source, destination, this.material, num3);
    }
    
    protected virtual void OnDisable()
    {
        if (this.material)
        {
            UnityEngine.Object.DestroyImmediate(this.material);
        }
    }
    
    public void PsychedelicEffect(float amount, float speed, float time)
    {
        base.StartCoroutine(this.psychedelic_effect(amount, speed, time));
    }
    
    private IEnumerator psychedelic_effect(float amount, float speed, float time)
    {
        float t = 0f;
        float slowdownTime = 0.5f;
        while (amount > 0f)
        {
            t += Time.deltaTime;
            float angle = speed * t;
            float phase = Mathf.Sin(angle) * amount;
            this.r = Vector2.up * phase;
            this.g = Vector2.up * phase / 2f;
            this.b = Vector2.down * phase;
            if (t >= time)
            {
                amount -= slowdownTime;
            }
            yield return null;
        }
        this.r = this.rStart;
        this.g = this.gStart;
        this.b = this.bStart;
        yield return null;
        yield break;
    }
}