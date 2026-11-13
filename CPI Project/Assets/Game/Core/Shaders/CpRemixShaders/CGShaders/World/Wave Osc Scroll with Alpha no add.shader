Shader "CpRemix/World/Wave Osc Scroll with Alpha no add"
{
	Properties
	{
	  _MainTex("Base (RGB)", 2D) = "white" {}
	  _OscDir("World Osc  Dir", Vector) = (1,0,0,1)
	  _OscAxis("World Osc Axs (w = wave freq)", Vector) = (0,1,0,1)
	  _OscSpeed("Osc Speed", float) = 1
	  _XScrollSpeed("X Scroll Speed", float) = 1
	  _YScrollSpeed("Y Scroll Speed", float) = 1
	}
	SubShader
	{
		Tags
		{
            "RenderPipeline" = "UniversalPipeline"
			"QUEUE" = "Transparent"
			"RenderType" = "Transparent"
		}
		Pass
		{
			Tags
			{
				"QUEUE" = "Transparent"
				"RenderType" = "Transparent"
			}
			ZWrite Off
			Cull Off
			Blend SrcAlpha One

			HLSLPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			float3 _OscDir;
			float4 _OscAxis;
			float _OscSpeed;
			float _XScrollSpeed;
			float _YScrollSpeed;
			sampler2D _MainTex;

			struct v2f
			{
				float4 xlv_TEXCOORD0 : TEXCOORD0;
				float2 xlv_TEXCOORD1 : TEXCOORD1;
				float fogFactor : TEXCOORD2;
			};

			struct FragOutput
			{
				float4 gl_FragData : SV_Target;
			};

			// Optimized Vertex Function
			v2f vert(float4 _glesVertex : POSITION, float4 _glesColor : COLOR, float4 _glesMultiTexCoord0 : TEXCOORD0, out float4 gl_Position : SV_POSITION)
			{
				v2f o;

				// Oscillation axis offset
				float axisOffset = dot(_glesVertex.xyz, mul((float3x3)unity_WorldToObject, _OscAxis.xyz)) * _OscAxis.w;
				float vertexColorApplication = 1.0 - _glesColor.w;

				// Calculate oscillated vertex position
				float3 oscDirWorld = mul((float3x3)unity_WorldToObject, _OscDir);
				float oscillatedVertex = sin((_Time.y * _OscSpeed) + axisOffset) * vertexColorApplication;
				float3 newPosition = _glesVertex.xyz + oscillatedVertex * oscDirWorld;

				// Transform the oscillated vertex position to clip space
				gl_Position = TransformObjectToHClip(newPosition);

				// Scroll texture coordinates
				o.xlv_TEXCOORD0 = _glesColor;
				o.xlv_TEXCOORD1 = _glesMultiTexCoord0.xy + float2(_XScrollSpeed * _Time.x, _YScrollSpeed * _Time.x);

				// Transfer fog coordinates
				o.fogFactor = ComputeFogFactor(gl_Position.z);

				return o;
			}

			// Fragment Function
			FragOutput frag(v2f i)
			{
				FragOutput o;
				float4 outputColor = tex2D(_MainTex, i.xlv_TEXCOORD1) * i.xlv_TEXCOORD0.w;

				// Apply fog and handle alpha transparency
				outputColor.rgb = MixFog(outputColor.rgb, i.fogFactor);
				outputColor.w = 1.0;

				o.gl_FragData = outputColor;
				return o;
			}

			ENDHLSL
		}
	}
	FallBack Off
}
