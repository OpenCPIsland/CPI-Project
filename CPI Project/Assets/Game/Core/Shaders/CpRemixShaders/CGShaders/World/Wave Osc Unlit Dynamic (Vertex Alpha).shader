Shader "CpRemix/World/Wave Osc Unlit Dynamic (Vertex Alpha)"
{
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_OscDir ("World Osc  Dir", Vector) = (1,0,0,1)
		_OscAxis ("World Osc Axs (w = wave freq)", Vector) = (0,1,0,1)
		_OscSpeed ("Osc Speed", Float) = 1
	}

	SubShader {
		Tags {
            "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" "DisableBatching" = "True" }

		Pass {
			Tags { "RenderType" = "Opaque" }

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
			
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			struct v2f
			{
				float4 position : SV_POSITION;
				float3 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
			};

			struct fout
			{
				float4 sv_target : SV_Target;
			};

			float4 _MainTex_ST;
			float3 _OscDir;
			float4 _OscAxis;
			float _OscSpeed;
			sampler2D _MainTex;
			
			v2f vert(Attributes v)
			{
				v2f o;

				float axisOffset = dot(v.vertex.xyz, mul((float3x3)unity_WorldToObject, _OscAxis.xyz)) * _OscAxis.w;
				float vertexColorApplication = 1.0 - v.color.w;

				float3 oscDirWorld = mul((float3x3)unity_WorldToObject, _OscDir);
				float oscillatedVertex = sin((_Time.y * _OscSpeed) + axisOffset) * vertexColorApplication;
				float3 newPosition = v.vertex.xyz + oscillatedVertex * oscDirWorld;

				o.position = TransformObjectToHClip(newPosition);

				o.texcoord = v.color.xyz;
				o.texcoord1.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;

				return o;
			}

			fout frag(v2f inp)
			{
				fout o;

				float4 texColor = tex2D(_MainTex, inp.texcoord1.xy);
				o.sv_target.xyz = texColor.xyz * inp.texcoord.xyz;
				o.sv_target.w = 1.0;

				o.sv_target.w = 1.0;

				return o;
			}
			ENDHLSL
		}
	}
}