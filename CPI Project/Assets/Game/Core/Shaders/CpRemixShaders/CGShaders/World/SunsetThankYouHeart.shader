Shader "CpRemix/World/SunsetThankYouHeart"
{
	Properties
	{
	  _ColorA("Color A", Color) = (1,1,1,1)
	  _ColorB("Color B", Color) = (1,1,1,1)
	  _MainTex("Texture (RGB)", 2D) = "white" {}
	  _XScrollSpeedA("X Scroll Speed A", float) = 1
	  _XScrollSpeedB("X Scroll Speed B", float) = -1
	  _AlphaYScrollSpeed("Alpha Scroll Speed", float) = 1
	  _Cutoff("Alpha cutoff", float) = 0.1
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
			Tags { "QUEUE" = "Transparent" }
			Blend One One

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
			float _XScrollSpeedA;
			float _XScrollSpeedB;
			float _AlphaYScrollSpeed;
			float _Cutoff;
			float4 _ColorA;
			float4 _ColorB;
			sampler2D _MainTex;

			struct v2f
			{
				float2 xlv_TEXCOORD1 : TEXCOORD1;
				float2 xlv_TEXCOORD2 : TEXCOORD2;
				float2 xlv_TEXCOORD3 : TEXCOORD3;
				float4 xlv_COLOR : COLOR;
				float4 pos : SV_POSITION; // Add SV_POSITION to avoid missing semantics
			};

			struct FragOutput
			{
				float4 gl_FragData : SV_Target;
			};

			v2f vert(Attributes v)
			{
				v2f o;

				// Handle vertex positions and transformations
				o.pos = TransformObjectToHClip(v.vertex.xyz);

				// Calculate scrolling texture coordinates
				o.xlv_TEXCOORD1 = v.texcoord.xy + float2(_XScrollSpeedA, 0) * _Time.x;
				o.xlv_TEXCOORD2 = v.texcoord.xy * 1.5 + float2(_XScrollSpeedB, 0) * _Time.x;
				o.xlv_TEXCOORD3 = v.texcoord.xy + float2(0, _AlphaYScrollSpeed) * _Time.x;
				
				// Pass vertex color to fragment shader
				o.xlv_COLOR = v.color;
				return o;
			}

			FragOutput frag(v2f i)
			{
				FragOutput o;

				// Sample the texture at the calculated coordinates
				float4 AwavesCol = tex2D(_MainTex, i.xlv_TEXCOORD1).x * _ColorA;
				float4 BwavesCol = tex2D(_MainTex, i.xlv_TEXCOORD2).y * _ColorB;
				float alphaMask = tex2D(_MainTex, i.xlv_TEXCOORD3).z;

				// Blend the two textures based on the alpha mask and cutoff
				float4 blendedColor = (AwavesCol + BwavesCol) * clamp(alphaMask - _Cutoff, 0.0, 1.0);
				o.gl_FragData = blendedColor * i.xlv_COLOR;

				return o;
			}

			ENDHLSL
		}
	}
	Fallback Off
}
