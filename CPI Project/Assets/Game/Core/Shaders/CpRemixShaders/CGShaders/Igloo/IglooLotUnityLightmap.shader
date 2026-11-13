Shader "CpRemix/Igloo/IglooLotUnityLightmap"
{
	Properties
	{
		_Color ("Tint Color", Color) = (1,1,1,1)
		_MainTex ("Texture (RGB)", 2D) = "white" {}
		_Highlight ("Additional Highlight", Range(0, 1)) = 0
		[HideInInspector] _BlobShadowTex ("Blob Shadow Tex", 2D) = "white" {}
	}

	SubShader
	{
		Tags
		{
			"RenderPipeline" = "UniversalPipeline"
			"QUEUE" = "Geometry"
			"RenderType" = "Opaque"
		}

		Pass
		{
			Tags
			{
				"LightMode" = "UniversalForward"
				"QUEUE" = "Geometry"
				"RenderType" = "Opaque"
			}

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma multi_compile _ LIGHTMAP_ON

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);
			TEXTURE2D(_BlobShadowTex);
			SAMPLER(sampler_BlobShadowTex);
			float _ShadowPlaneDim;
			float _ShadowTextureDim;
			float3 _ShadowPlaneWorldPos;

			CBUFFER_START(UnityPerMaterial)
				float4 _MainTex_ST;
				float4 _Color;
				float _Highlight;
			CBUFFER_END

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
			};

			struct v2f
			{
				float4 position : SV_POSITION;
				float4 color : COLOR0;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float fogFactor : TEXCOORD2;
				float3 shadowData : TEXCOORD3;
			};

			inline float3 DecodeDirectionalVertexLighting(float3 worldNormal)
			{
				float upWeight = saturate(worldNormal.y);
				float downWeight = 1.0 - smoothstep(-1.0, 0.8, worldNormal.y);
				float skyWeight = upWeight;
				float groundWeight = downWeight;
				float equatorWeight = max(1.0 - skyWeight - groundWeight, 0.0);
				float3 skyAmbient = unity_AmbientSky.rgb;
				float3 equatorAmbient = unity_AmbientEquator.rgb;
				float skyLuminance = dot(skyAmbient, float3(0.2126, 0.7152, 0.0722));
				float equatorLuminance = dot(equatorAmbient, float3(0.2126, 0.7152, 0.0722));
				skyAmbient = lerp(skyAmbient, skyLuminance.xxx, 0.1);
				equatorAmbient = lerp(equatorAmbient, equatorLuminance.xxx, 0.1);
				float3 ambient = skyAmbient * skyWeight
					+ equatorAmbient * equatorWeight
					+ unity_AmbientGround.rgb * groundWeight * 0.7;

				Light mainLight = GetMainLight();
				float ndl = saturate((dot(worldNormal, mainLight.direction) + 1.0) * 0.25);
				return ambient + mainLight.color * ndl + float3(0.1, 0.1, 0.1);
			}

			v2f vert(appdata v)
			{
				v2f o;

				o.position = TransformObjectToHClip(v.vertex.xyz);

				float3 worldNormal = normalize(TransformObjectToWorldNormal(v.normal));
				float3 lighting = DecodeDirectionalVertexLighting(worldNormal);

				o.color = float4(lighting, 2.0) * _Color + _Highlight.xxxx;
				o.texcoord = v.texcoord;
				#if defined(LIGHTMAP_ON)
					o.texcoord1 = v.texcoord1 * unity_LightmapST.xy + unity_LightmapST.zw;
				#else
					o.texcoord1 = 0.0;
				#endif

				float3 worldPos = TransformObjectToWorld(v.vertex.xyz);
				float halfDim = max(_ShadowPlaneDim * 0.5, 0.0001);
				float aspectOfs = 1.0 / max(_ShadowTextureDim, 0.0001);
				float offsetX = worldPos.x - _ShadowPlaneWorldPos.x;
				float offsetZ = worldPos.z - _ShadowPlaneWorldPos.z;
				o.shadowData.x = (aspectOfs + offsetX / halfDim + 1.0) * 0.5;
				o.shadowData.y = (aspectOfs + offsetZ / halfDim + 1.0) * 0.5;
				o.shadowData.z = worldPos.y;

				o.fogFactor = ComputeFogFactor(o.position.z);
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				half4 lightmapSample = half4(1.0, 1.0, 1.0, 1.0);
				#if defined(LIGHTMAP_ON)
					lightmapSample = SAMPLE_TEXTURE2D(unity_Lightmap, samplerunity_Lightmap, i.texcoord1);
					lightmapSample.rgb *= lightmapSample.a * LIGHTMAP_HDR_MULTIPLIER;
					lightmapSample.a = 1.0;
				#endif
				float4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
				float4 col = lightmapSample * i.color * albedo * 0.9;

				float4 shadowSample = SAMPLE_TEXTURE2D(_BlobShadowTex, sampler_BlobShadowTex, i.shadowData.xy);
				float shadowDepth = shadowSample.y;
				float shadowIntensity = shadowSample.x;
				float isAbove = (i.shadowData.z >= shadowDepth) ? 2.0 : 1.0;
				float depthDiff = shadowDepth - i.shadowData.z;
				float shadowFactor = mad(abs(depthDiff), isAbove, isAbove) - 0.5;
				float shadowMult = min(shadowIntensity * max(shadowFactor, 1.0), 1.0);
				col.rgb *= shadowMult;

				col.rgb = MixFog(col.rgb, i.fogFactor);
				col.a = 1.0;
				return col;
			}
			ENDHLSL
		}
	}

	Fallback Off
}
