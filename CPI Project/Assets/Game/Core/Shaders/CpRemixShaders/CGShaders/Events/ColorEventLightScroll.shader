Shader "CpRemix/World/Events/Color/ColorEventLightScroll" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_ScrollSpeed ("Y Scroll Speed", Range(-20, 20)) = 0
	}
	SubShader {
		LOD 100
		Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
		Pass {
			LOD 100
			Tags { "LightMode" = "UniversalForwardOnly" }
			ZClip Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);

			CBUFFER_START(UnityPerMaterial)
			float4 _MainTex_ST;
			float _ScrollSpeed;
			CBUFFER_END

			struct Attributes
			{
				float4 positionOS : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			Varyings vert(Attributes input)
			{
				Varyings output;
				output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
				output.uv = TRANSFORM_TEX(input.uv, _MainTex);
				return output;
			}

			half4 frag(Varyings input) : SV_Target
			{
				input.uv.y += _ScrollSpeed * _Time.x;
				return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
			}
			ENDHLSL
		}
	}
	Fallback Off
}