Shader "Unlit/BilinearColorFilter"
{
    Properties
    {
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
    }
    SubShader
    {
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}
        LOD 100

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_instancing
			#pragma multi_compile_local _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA

			#include "UnitySprites.cginc"

			float4 _MainTex_TexelSize;
			float _BilinearAmount;
			int _activateBilinear;

			fixed4 GetBilinearFilteredColor(float2 texcoord)
			{
				fixed4 s1 = SampleSpriteTexture(texcoord + float2(0.0, _MainTex_TexelSize.y / _BilinearAmount));
				fixed4 s2 = SampleSpriteTexture(texcoord + float2(_MainTex_TexelSize.x / _BilinearAmount, 0.0));
				fixed4 s3 = SampleSpriteTexture(texcoord + float2(_MainTex_TexelSize.x / _BilinearAmount, _MainTex_TexelSize.y / _BilinearAmount));
				fixed4 s4 = SampleSpriteTexture(texcoord);

				float2 TexturePosition = float2(texcoord)* _MainTex_TexelSize.z;

				float fu = frac(TexturePosition.x);
				float fv = frac(TexturePosition.y);

				float4 tmp1 = lerp(s4, s2, fu);
				float4 tmp2 = lerp(s1, s3, fu);

				return lerp(tmp1, tmp2, fv);
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				//fixed4 c = SampleSpriteTexture(RotateZ(IN.texcoord, 0)) * IN.color;
				//fixed4 c = GetBilinearFilteredColor(IN.texcoord - 0.05 * _MainTex_TexelSize.xy) * IN.color;
				fixed4 c = _BilinearAmount > 0 ? (GetBilinearFilteredColor(IN.texcoord - 0.498 * _MainTex_TexelSize.xy) * IN.color) : (SampleSpriteTexture(IN.texcoord / 2 + IN.texcoord / 2));
				//fixed4 c = _activateBilinear > 0 ? (GetBilinearFilteredColor(IN.texcoord - 0.498 * _MainTex_TexelSize.xy) * IN.color) : (SampleSpriteTexture(IN.texcoord / 2 + IN.texcoord / 2));
				c.rgb *= c.a;
				return c;
			}
            ENDCG
        }
    }
}
