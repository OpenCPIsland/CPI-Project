Shader "CpRemix/World/Events/Color/Color Tint Skybox" {
	Properties {
		_TintColor ("Tint Color", Color) = (1,1,1,1)
		_cubemap ("Environment Map", Cube) = "white" {}
	}
	SubShader {
		Tags { "RenderPipeline" = "UniversalPipeline" "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" }
		Pass {
			ZClip Off
			ZWrite Off
			Cull Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			TEXTURECUBE(_cubemap);
			SAMPLER(sampler_cubemap);
			CBUFFER_START(UnityPerMaterial)
			float4 _TintColor;
			CBUFFER_END

			struct Attributes
			{
				float4 positionOS : POSITION;
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float3 direction : TEXCOORD0;
			};

			Varyings vert(Attributes input)
			{
				Varyings output;
				output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
				output.positionCS.z = UNITY_RAW_FAR_CLIP_VALUE * output.positionCS.w;
				output.direction = input.positionOS.xyz;
				return output;
			}

			half4 frag(Varyings input) : SV_Target
			{
				return SAMPLE_TEXTURECUBE(_cubemap, sampler_cubemap, input.direction) * _TintColor;
			}
			ENDHLSL
		}
	}
	Fallback Off
}