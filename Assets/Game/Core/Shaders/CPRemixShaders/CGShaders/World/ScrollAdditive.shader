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
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			float _XScrollSpeed;
			float _YScrollSpeed;
			float4 _Color;
			sampler2D _MainTex;

			struct v2f
			{
				float2 texcoord : TEXCOORD0;
				float4 position : SV_POSITION;
			};

			v2f vert(appdata_full v)
			{
				v2f o;

				// Simplified vertex position calculation
				o.position = UnityObjectToClipPos(v.vertex);

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

			ENDCG
		}
	}
	Fallback "Diffuse"
}
