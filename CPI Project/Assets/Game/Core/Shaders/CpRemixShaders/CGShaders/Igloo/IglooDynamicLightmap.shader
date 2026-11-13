Shader "CpRemix/Igloo/IglooDynamicLightmap"
{
	Properties
	{
		_Color("Tint Color", Color) = (1,1,1,1)
		_MainTex("Texture (RGB)", 2D) = "white" {}
		_Highlight("Additional Highlight", Range(0, 1)) = 0
		_Lightmap("Lightmap", 2D) = "white" {}
		_ShadowColor("Shadow Color", Color) = (0,0,0,1)
		_ShadowBrightness("ShadowBrightness", Range(0, 1)) = 0
		[HideInInspector] _BlobShadowTex ("Blob Shadow Tex", 2D) = "white" {}
	}

	SubShader
	{
		Tags
		{
            "RenderPipeline" = "UniversalPipeline"
			"LightMode" = "UniversalForward"
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

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			sampler2D _MainTex;
			sampler2D _Lightmap;
			sampler2D _BlobShadowTex;

			float4 _MainTex_ST;
			float4 _Color;
			float4 _ShadowColor;
			float _Highlight;
			float _ShadowBrightness;
			float _ShadowPlaneDim;
			float _ShadowTextureDim;
			float3 _ShadowPlaneWorldPos;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
			};

			struct v2f
			{
				float4 color : COLOR0;
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float3 shadowData : TEXCOORD2;
				float4 pos : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;

				float3 normalWorldSpace;
				float3x3 worldToObject3x3;
				worldToObject3x3[0] = unity_WorldToObject[0].xyz;
				worldToObject3x3[1] = unity_WorldToObject[1].xyz;
				worldToObject3x3[2] = unity_WorldToObject[2].xyz;
				normalWorldSpace = normalize(mul(v.normal, worldToObject3x3));

				float4 normalWS4 = float4(normalWorldSpace, 1.0);

				float3 sh0;
				sh0.x = dot(unity_SHAr, normalWS4);
				sh0.y = dot(unity_SHAg, normalWS4);
				sh0.z = dot(unity_SHAb, normalWS4);

				float4 shCross = normalWorldSpace.xyzz * normalWorldSpace.yzzx;

				float3 sh1;
				sh1.x = dot(unity_SHBr, shCross);
				sh1.y = dot(unity_SHBg, shCross);
				sh1.z = dot(unity_SHBb, shCross);

				float3 ambient = sh0 + sh1 + unity_SHC.xyz * ((normalWorldSpace.x * normalWorldSpace.x) - (normalWorldSpace.y * normalWorldSpace.y));
				ambient = max((1.055 * pow(max(ambient, 0.0), 0.4166667)) - 0.055, 0.0);

				float ndl = clamp((dot(float4(normalWorldSpace, 0.0), _MainLightPosition) + 1.0) / 4.0, 0.0, 1.0);
				float3 direct = (_MainLightColor.rgb * ndl);

				float4 litColor = float4(max(0.0, ambient) + direct, 1.0);
				litColor *= _Color;
				litColor += _Highlight;

				o.pos = TransformObjectToHClip(v.vertex.xyz);
				o.color = litColor;
				o.uv = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
				o.uv2 = v.texcoord1.xy;

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				float halfDim = max(_ShadowPlaneDim * 0.5, 0.0001);
				float aspectOfs = 1.0 / max(_ShadowTextureDim, 0.0001);
				float offsetX = worldPos.x - _ShadowPlaneWorldPos.x;
				float offsetZ = worldPos.z - _ShadowPlaneWorldPos.z;
				o.shadowData.x = (aspectOfs + offsetX / halfDim + 1.0) * 0.5;
				o.shadowData.y = (aspectOfs + offsetZ / halfDim + 1.0) * 0.5;
				o.shadowData.z = worldPos.y;

				return o;
			}

			half4 frag(v2f i) : SV_Target
			{
				half4 baseTex = tex2D(_MainTex, i.uv);
				half4 lm = tex2D(_Lightmap, i.uv2);

				half lit = saturate(max(lm.r, _ShadowBrightness));
				half shadowLerp = lit * 0.9;

				float4 shadowSample = tex2D(_BlobShadowTex, i.shadowData.xy);
				float shadowDepth = shadowSample.y;
				float shadowIntensity = shadowSample.x;
				float isAbove = (i.shadowData.z >= shadowDepth) ? 2.0 : 1.0;
				float depthDiff = shadowDepth - i.shadowData.z;
				float shadowFactor = mad(abs(depthDiff), isAbove, isAbove) - 0.5;
				float shadowMult = min(shadowIntensity * max(shadowFactor, 1.0), 1.0);

				half4 col = lerp(_ShadowColor, i.color * baseTex, shadowLerp);
				col.rgb *= shadowMult;
				return col;
			}
			ENDHLSL
		}
	}

	Fallback Off
}