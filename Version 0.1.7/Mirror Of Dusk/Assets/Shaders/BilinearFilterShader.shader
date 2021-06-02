Shader "Unlit/BilinearFilterShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			float _BilinearAmount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

			float4 SampleBilinear(sampler2D tex, float2 uv, float4 texelSize)
			{
				// scale & offset uvs to integer values at texel centers
				float2 uv_texels = uv * texelSize.zw + 0.5;

				// get uvs for the center of the 4 surrounding texels by flooring
				//float4 uv_min_max = float4((floor(uv_texels) - 0.5) * texelSize.xy, (floor(uv_texels) + 0.5) * texelSize.xy);
				float4 uv_min_max = float4(uv - texelSize.xy * _BilinearAmount, uv + texelSize * _BilinearAmount);

				// blend factor
				float2 uv_frac = frac(uv_texels);

				// sample all 4 texels
				float4 texelA = tex2Dlod(tex, float4(uv_min_max.xy, 0, 0));
				float4 texelB = tex2Dlod(tex, float4(uv_min_max.xw, 0, 0));
				float4 texelC = tex2Dlod(tex, float4(uv_min_max.zy, 0, 0));
				float4 texelD = tex2Dlod(tex, float4(uv_min_max.zw, 0, 0));

				// bilinear interpolation
				return lerp(lerp(texelA, texelB, uv_frac.y), lerp(texelC, texelD, uv_frac.y), uv_frac.x);
			}

            fixed4 frag (v2f i) : SV_Target
            {
				float4 col = SampleBilinear(_MainTex, i.uv, _MainTex_TexelSize);
                return col;
            }

			
            ENDCG
        }
    }
}
