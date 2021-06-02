Shader "Unlit/BicubicColorFilter"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
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

			Stencil
			{
				Ref[_Stencil]
				Comp[_StencilComp]
				Pass[_StencilOp]
				ReadMask[_StencilReadMask]
				WriteMask[_StencilWriteMask]
			}

			Cull Off
			Lighting Off
			ZWrite Off
			ZTest[unity_GUIZTestMode]
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask[_ColorMask]

			Pass
			{
				Name "Default"
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.0

				#include "UnityCG.cginc"
				#include "UnityUI.cginc"

				#pragma multi_compile __ UNITY_UI_ALPHACLIP

				struct appdata_t
				{
					float4 vertex   : POSITION;
					float4 color    : COLOR;
					float2 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2f
				{
					float4 vertex   : SV_POSITION;
					fixed4 color : COLOR;
					float2 texcoord  : TEXCOORD0;
					float4 worldPosition : TEXCOORD1;
					UNITY_VERTEX_OUTPUT_STEREO
				};

				fixed4 _Color;
				fixed4 _TextureSampleAdd;
				float4 _ClipRect;

				v2f vert(appdata_t IN)
				{
					v2f OUT;
					UNITY_SETUP_INSTANCE_ID(IN);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
					OUT.worldPosition = IN.vertex;
					OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

					OUT.texcoord = IN.texcoord;

					OUT.color = IN.color * _Color;
					return OUT;
				}

				//sampler2D _MainTex;
				sampler2D _MainTex;
				float4 _MainTex_TexelSize;
				float _Test1;
					float _Test2;
				//            uniform sampler2D _KernelBicubic;

				//            float4 texFilter (uniform sampler2D tex, float2 uvs) {

				//              float4 c;

				//              c =

				//              return c;
				//            }

							float Triangular(float f)
							{
								f = f / 2.0;
								if (f < 0.0)
								{
									return (f + 1.0);
								}
								else
								{
									return (1.0 - f);
								}
								return 0.0;
							}

							float BellFunc(float x)
							{
								float f = (x / 2.0) * 1.5; // Converting -2 to +2 to -1.5 to +1.5
								if (f > -1.5 && f < -0.5)
								{
									return(0.5 * pow(f + 1.5, 2.0));
								}
								else if (f > -0.5 && f < 0.5)
								{
									return 3.0 / 4.0 - (f * f);
								}
								else if ((f > 0.5 && f < 1.5))
								{
									return(0.5 * pow(f - 1.5, 2.0));
								}
								return 0.0;
							}

							float BSpline(float x)
							{
								float f = x;
								if (f < 0.0)
								{
									f = -f;
								}

								if (f >= 0.0 && f <= 1.0)
								{
									return (2.0 / 3.0) + (0.5) * (f* f * f) - (f*f);
								}
								else if (f > 1.0 && f <= 2.0)
								{
									return 1.0 / 6.0 * pow((2.0 - f), 3.0);
								}
								return 1.0;
							}

							float Cubic(float x, float B, float C)
							{
								//const float B = 0.0;        // original bspline
								//const float C = 0.5;
								float f = x;
								if (f < 0.0)
								{
									f = -f;
								}
								if (f < 1.0)
								{
									return ((12 - 9 * B - 6 * C) * (pow(f,3)) +
										(-18 + 12 * B + 6 * C) * (pow(f,2)) +
										(6 - 2 * B)) / 6.0;
								}
								else if (f >= 1.0 && f < 2.0)
								{
									return ((-B - 6 * C) * (pow(f,3))
										+ (6 * B + 30 * C) * (pow(f,2)) +
										(-(12 * B) - 48 * C) * f +
										8 * B + 24 * C) / 6.0;
								}
								else
								{
									return 0.0;
								}
							}

							float CatMullRom(float x) {
								const float B = 0.0;
								const float C = 0.5;
								return (Cubic(x,B,C));
							}

							float Interpolation(float x) {
								//return Triangular(x);
								//return BellFunc(x);
								//return BSpline(x);
								//return CatMullRom(x);

								//return (Cubic(x,1,0));        // b-spline (blurry)
				//                return (Cubic(x,1/3,1.3));        // mitchel
								//return (Cubic(x,0,0.5));        // CatMullRom
								//return (Cubic(x,0,1.4));        // CatMullRom
								//return (Cubic(x,0,0.75));        // photoshop    (sharper, but they mix with blur)
								return (Cubic(x,_Test1,_Test2));        // test
								//return (Cubic(x,1,0));        // gimp
								//return Cubic(x,0, (1.125-(0.5*0.75)) );        // photoshop reduce
							}

							float4 BiCubic(sampler2D textureSampler, float2 TexCoord, bool blr)
							{
								float fWidth = 1 / _MainTex_TexelSize.x;
								float fHeight = 1 / _MainTex_TexelSize.y;
								float texelSizeX = _MainTex_TexelSize.x;
								float texelSizeY = _MainTex_TexelSize.y;

								float4 nSum = float4(0.0, 0.0, 0.0, 0.0);
								float4 nDenom = float4(0.0, 0.0, 0.0, 0.0);
								float a = frac(TexCoord.x * fWidth); // get the decimal part
								float b = frac(TexCoord.y * fHeight); // get the decimal part

								for (int m = -1; m <= 2; m++)
								{
									for (int n = -1; n <= 2; n++)
									{
										float4 vecData = tex2D(textureSampler,
												TexCoord + float2(texelSizeX * float(m),
												texelSizeY * float(n)));
										float f = Interpolation(float(m) - a);
										float4 vecCooef1 = float4(f,f,f,f);
										float f1 = Interpolation(-(float(n) - b));
										float4 vecCoeef2 = float4(f1, f1, f1, f1);
										nSum = nSum + (vecData * vecCoeef2 * vecCooef1);
										nDenom = nDenom + ((vecCoeef2 * vecCooef1));
									}
								}

								return nSum / nDenom;
							}

							// Bilinear filtering (works on image set to nearest)
							float4 tex2DBiLinear(sampler2D textureSampler_i, float2 texCoord_i)
							{
								float fWidth = 1 / _MainTex_TexelSize.x;
								float fHeight = 1 / _MainTex_TexelSize.y;
								float texelSizeX = _MainTex_TexelSize.x;
								float texelSizeY = _MainTex_TexelSize.y;

								float4 p0q0 = tex2D(textureSampler_i, texCoord_i);
								float4 p1q0 = tex2D(textureSampler_i, texCoord_i + float2(texelSizeX, 0));

								float4 p0q1 = tex2D(textureSampler_i, texCoord_i + float2(0, texelSizeY));
								float4 p1q1 = tex2D(textureSampler_i, texCoord_i + float2(texelSizeX , texelSizeY));

								float a = frac(texCoord_i.x * fWidth); // Get Interpolation factor for X direction.
												// Fraction near to valid data.

								float4 pInterp_q0 = lerp(p0q0, p1q0, a); // Interpolates top row in X direction.
								float4 pInterp_q1 = lerp(p0q1, p1q1, a); // Interpolates bottom row in X direction.

								float b = frac(texCoord_i.y * fHeight);// Get Interpolation factor for Y direction.
								return lerp(pInterp_q0, pInterp_q1, b); // Interpolate in Y direction.
							}

							fixed4 frag(v2f IN) : SV_Target
							{
								half4 colorSrc = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
								half4 color = colorSrc;

								// custom bicubic (photoshop)
								half4 color2 = (BiCubic(_MainTex, IN.texcoord, false) + _TextureSampleAdd) * IN.color;
								color = color2;

								//color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

								#ifdef UNITY_UI_ALPHACLIP
								clip(color.a - 0.001);
								#endif

								return color;
							}
						ENDCG
						}
		}
}