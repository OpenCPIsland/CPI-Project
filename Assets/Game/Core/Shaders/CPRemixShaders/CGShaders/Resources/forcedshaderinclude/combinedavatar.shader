Shader "CpRemix/Combined Avatar"
{
	Properties
	{
		_MainTex("Diffuse Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags
		{
			"QUEUE" = "Geometry+99"
			"RenderType" = "Opaque"
		}
		Pass
		{
			Tags
			{
				"LIGHTMODE" = "FORWARDBASE"
				"QUEUE" = "Geometry+99"
				"RenderType" = "Opaque"
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			#include "Lighting.cginc"

			sampler2D _MainTex;

			struct appdata_t
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
			};

			struct OUT_Data_Vert
			{
				float2 uv : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float3 color : COLOR;
				float4 pos : SV_POSITION;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float3 normal : TEXCOORD1;
			};

			struct OUT_Data_Frag
			{
				float4 fragColor : SV_Target0;
			};

			OUT_Data_Vert vert(appdata_t v)
			{
				OUT_Data_Vert o;

				// Transform the vertex position into world space
				float3 worldNormal = mul((float3x3)unity_ObjectToWorld, v.normal);
				o.normal = normalize(worldNormal);

				// Calculate vertex position in clip space
				o.pos = UnityObjectToClipPos(v.vertex);

				// Pass UV coordinates
				o.uv = v.texcoord.xy;

				// Set vertex color
				o.color = v.color.rgb;

				return o;
			}

			OUT_Data_Frag frag(v2f i)
			{
				OUT_Data_Frag o;

				// Sample the main texture
				float4 texColor = tex2D(_MainTex, i.uv);

				// Simple lighting based on texture color and normal direction
				float3 lighting = i.normal * texColor.rgb;

				// Output the final color with texture and lighting
				o.fragColor = float4(lighting, texColor.a);

				return o;
			}

			ENDCG
		}
	}
	FallBack Off
}
