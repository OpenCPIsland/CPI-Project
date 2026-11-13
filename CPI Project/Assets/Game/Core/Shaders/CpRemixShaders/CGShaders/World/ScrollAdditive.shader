Shader "CpRemix/World/ScrollAdditive"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Color (RGB) Alpha (A)", 2D) = "black" {}
		_XScrollSpeed("X Scroll Speed", float) = 1
		_YScrollSpeed("Y Scroll Speed", float) = 1
	}
	SubShader
	{
		Tags
		{
            "RenderPipeline" = "UniversalPipeline"
			"QUEUE" = "Transparent"
		}
		Pass
		{
			Tags
			{
				"QUEUE" = "Transparent"
			}
			//ZWrite Off
			Blend SrcAlpha One
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
			float _XScrollSpeed;
			float _YScrollSpeed;
			float4 _Color;
			sampler2D _MainTex;

			struct v2f
			{
				float2 texcoord : TEXCOORD0;
				float4 position : SV_POSITION;
			};

			v2f vert(Attributes v)
			{
				v2f o;

				// Simplified vertex position calculation
				o.position = TransformObjectToHClip(v.vertex.xyz);

				// Scroll the texture coordinates based on time and scroll speed
				o.texcoord = v.texcoord.xy + float2(_XScrollSpeed, _YScrollSpeed) * _Time.x;

				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				// Sample the texture at the scrolled coordinates
				float4 texColor = tex2D(_MainTex, i.texcoord);

				// Apply color blending
				float4 outputColor;
				outputColor.rgb = texColor.rgb + _Color.rgb; // Additive color
				outputColor.a = texColor.a * _Color.a;       // Preserve alpha blending

				return outputColor;
			}

			ENDHLSL
		}
	}
	Fallback Off
}
