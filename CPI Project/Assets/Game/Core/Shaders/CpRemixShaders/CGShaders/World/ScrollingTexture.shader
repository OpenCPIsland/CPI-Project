Shader "CpRemix/World/ScrollingTexture"
{
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Texture (RGB)", 2D) = "white" {}
		_XScrollSpeed ("X Scroll Speed", Float) = 1
		_YScrollSpeed ("Y Scroll Speed", Float) = 1
	}

	SubShader {
		Tags {
            "RenderPipeline" = "UniversalPipeline" "QUEUE" = "Geometry" "DisableBatching" = "True" }

		Pass {
			Tags { "QUEUE" = "Geometry" }
			HLSLPROGRAM
            struct Attributes
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 color : COLOR;
                float4 texcoord : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float4 texcoord2 : TEXCOORD2;
                float4 texcoord3 : TEXCOORD3;
            };

			#pragma vertex vert
			#pragma fragment frag
			// #pragma multi_compile_fog
			
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float2 texcoord1 : TEXCOORD1;
				float3 color : COLOR0;
				// float fogFactor : TEXCOORD2;
			};

			struct fout
			{
				float4 sv_target : SV_Target0;
			};

			float _XScrollSpeed;
			float _YScrollSpeed;

			float3 _Color;

			sampler2D _MainTex;
			
			v2f vert(Attributes v)
			{
				v2f o;

				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.position = mul(UNITY_MATRIX_VP, worldPos);

				o.texcoord1.xy = float2(_XScrollSpeed.x, _YScrollSpeed.x) * _Time.xx + v.texcoord.xy;

				o.color.xyz = v.color.xyz;

				// o.fogFactor = ComputeFogFactor(o.position.z);

				return o;
			}

			fout frag(v2f inp)
			{
				fout o;

				float4 tmp0;

				tmp0 = tex2D(_MainTex, inp.texcoord1.xy);

				tmp0.xyz = tmp0.xyz + inp.color.xyz;

				o.sv_target.xyz = tmp0.xyz + _Color;
				o.sv_target.w = 1.0;

				// o.sv_target.rgb = MixFog(o.sv_target.rgb, inp.fogFactor);
				// o.sv_target.w = 1.0;

				return o;
			}
			ENDHLSL
		}
	}

	Fallback Off
}