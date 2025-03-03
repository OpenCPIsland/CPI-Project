Shader "CpRemix/World/Wave Osc Scroll with Alpha"
{
	Properties
	{
	  _TintColor("Tint Colour", Color) = (0,0,0,1)
	  _MainTex("Base (RGB)", 2D) = "white" {}
	  _OscDir("World Osc  Dir", Vector) = (1,0,0,1)
	  _OscAxis("World Osc Axs (w = wave freq)", Vector) = (0,1,0,1)
	  _OscSpeed("Osc Speed", float) = 1
	  _XScrollSpeed("X Scroll Speed", float) = 1
	  _YScrollSpeed("Y Scroll Speed", float) = 1
	}
	SubShader
	{
		Tags { "QUEUE" = "Transparent" }
		Pass
		{
			Tags { "QUEUE" = "Transparent" }
			Cull Off
			Blend One One

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			float3 _OscDir;
			float4 _OscAxis;
			float _OscSpeed;
			float _XScrollSpeed;
			float _YScrollSpeed;
			sampler2D _MainTex;
			float4 _TintColor;

			struct v2f
			{
				float4 xlv_TEXCOORD0 : TEXCOORD0;
				float2 xlv_TEXCOORD1 : TEXCOORD1;
				UNITY_FOG_COORDS(2)
			};

			struct FragOutput
			{
				float4 gl_FragData : SV_Target;
			};

			// Optimized Vertex Function
			v2f vert(float4 _glesVertex : POSITION, float4 _glesColor : COLOR, float4 _glesMultiTexCoord0 : TEXCOORD0, out float4 gl_Position : SV_POSITION)
			{
				v2f o;
				float axisOffset = dot(_glesVertex.xyz, mul((float3x3)unity_WorldToObject, _OscAxis.xyz)) * _OscAxis.w;
				float vertexColorApplication = 1.0 - _glesColor.w;

				// Calculate oscillated vertex position
				float3 oscDirWorld = mul((float3x3)unity_WorldToObject, _OscDir);
				float oscillatedVertex = sin((_Time.y * _OscSpeed) + axisOffset) * vertexColorApplication;
				float3 newPosition = _glesVertex.xyz + oscillatedVertex * oscDirWorld;

				// Transform the oscillated vertex position to clip space
				gl_Position = UnityObjectToClipPos(float4(newPosition, 1.0));

				// Scroll texture coordinates
				o.xlv_TEXCOORD0 = _glesColor;
				o.xlv_TEXCOORD1 = _glesMultiTexCoord0.xy + float2(_XScrollSpeed * _Time.x, _YScrollSpeed * _Time.x);

				// Transfer fog coordinates
				UNITY_TRANSFER_FOG(o, gl_Position);

				return o;
			}

			// Fragment Function (unchanged)
			FragOutput frag(v2f i)
			{
				FragOutput o;
				float4 outputColor = tex2D(_MainTex, i.xlv_TEXCOORD1) * i.xlv_TEXCOORD0.w;
				outputColor = outputColor + _TintColor;

				UNITY_APPLY_FOG(i.fogCoord, outputColor);
				UNITY_OPAQUE_ALPHA(outputColor.w);

				o.gl_FragData = outputColor;
				return o;
			}

			ENDCG
		}
	}
	FallBack Off
}
