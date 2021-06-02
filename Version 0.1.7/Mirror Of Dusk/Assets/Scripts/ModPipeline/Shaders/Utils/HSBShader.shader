Shader "Hidden/Mod Render Pipeline/HSB"
{
	SubShader
	{
		Tags { "RenderType" = "Opaque" "RenderPipeline" = "ModPipeline"}
		LOD 100

		Pass
		{
			Name "HSB"
			ZTest Always ZWrite Off

			HLSLPROGRAM
		// Required to compile gles 2.0 with standard srp library
		#pragma prefer_hlslcc gles
		#pragma exclude_renderers d3d11_9x
		#pragma vertex Vertex
		#pragma fragment Fragment

		#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

		struct Attributes
		{
			float4 positionOS   : POSITION;
			float2 uv           : TEXCOORD0;
		};

		struct Varyings
		{
			half4 positionCS       : SV_POSITION;
			half2 uv        : TEXCOORD0;
		};

		TEXTURE2D(_HSBTex);
		SAMPLER(sampler_HSBTex);
		float _Brightness;

		Varyings Vertex(Attributes input)
		{
			Varyings output;
			output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
			output.uv = input.uv;
			return output;
		}

		float4 Fragment(Varyings input) : SV_Target
		{
			float4 col = SAMPLE_TEXTURE2D(_HSBTex, sampler_HSBTex, input.uv);
			//float4 hsbColor = applyBrightEffect(col);
			return col;
		}

		float4 applyBrightEffect(float4 startColor)
		{
			float4 outputColor = startColor;
			outputColor.rgb = outputColor.rgb + _Brightness;
			return outputColor;
		}
		ENDHLSL
	}
	}
}