Shader "CpRemix/World/3DWorldText"
{
	Properties
	{
		_MainTex("Font Texture", 2D) = "white" {}
		_Color("Text Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags
		{
			"IGNOREPROJECTOR" = "true"
			"QUEUE" = "Transparent"
			"RenderType" = "Transparent"
		}
		Pass
		{
			Tags
			{
				"IGNOREPROJECTOR" = "true"
				"QUEUE" = "Transparent"
				"RenderType" = "Transparent"
			}
			ZWrite Off
			Cull Off
			Fog { Mode Off }
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			// Shader properties
			float4 _Color;
			float4 _MainTex_ST;
			sampler2D _MainTex;

			// Vertex to fragment structure
			struct v2f
			{
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			// Vertex shader
			v2f vert(appdata_full v)
			{
				v2f o;

				// Transform the vertex position into clip space
				o.pos = UnityObjectToClipPos(v.vertex);

				// Apply the texture scaling and offset (for tiling and UV adjustments)
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

				// Pass through color
				o.color = clamp(_Color, 0.0, 1.0);

				return o;
			}

			// Fragment shader
			fixed4 frag(v2f i) : SV_Target
			{
				// Sample the alpha value from the font texture
				float4 texColor = tex2D(_MainTex, i.uv);

				// Multiply by the vertex color and texture alpha
				float4 finalColor = float4(i.color.rgb, i.color.a * texColor.a);

				return finalColor;
			}

			ENDCG
		}
	}
	Fallback Off
}
