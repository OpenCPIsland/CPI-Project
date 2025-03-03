Shader "CpRemix/World/Vertex Alpha Sine Bounce Lightmap (Fog)"
{
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader {
		LOD 100
		Tags { "RenderType" = "Opaque" }
		Pass {
			LOD 100
			Tags { "RenderType" = "Opaque" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			// Input structure for the vertex shader
			struct v2f {
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float4 color : COLOR0;
				float4 position : SV_POSITION0;
				UNITY_FOG_COORDS(2)
			};

			// Declare shader properties
			float4 _MainTex_ST;
			sampler2D _MainTex;

			// Vertex shader
			v2f vert(appdata_full v)
			{
				v2f o;

				// Calculate texture coordinates
				o.texcoord1.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				o.texcoord.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;

				// Copy vertex color
				o.color = v.color;

				// Transform the vertex position from object space to world space and then to clip space
				float4 worldPosition = mul(unity_ObjectToWorld, v.vertex);
				float4 clipPosition = mul(UNITY_MATRIX_VP, worldPosition);

				// Apply sine bounce effect based on vertex alpha and time
				float sineOffset = sin(v.color.w * _Time.y);
				o.position.y = clipPosition.y + sineOffset;
				o.position.xzw = clipPosition.xzw;

				// Handle fog
				UNITY_TRANSFER_FOG(o, o.position);

				return o;
			}

			// Fragment shader
			struct fout {
				float4 sv_target : SV_Target0;
			};

			fout frag(v2f inp)
			{
				fout o;

				// Sample the lightmap and texture
				float4 lightmapColor = UNITY_SAMPLE_TEX2D_SAMPLER(unity_Lightmap, unity_Lightmap, inp.texcoord1.xy);
				lightmapColor.w = lightmapColor.w * unity_Lightmap_HDR.x;
				lightmapColor.xyz *= lightmapColor.w;

				float4 texColor = tex2D(_MainTex, inp.texcoord.xy);

				// Combine the lightmap, texture, and vertex color
				o.sv_target.xyz = lightmapColor.xyz * texColor.xyz * inp.color.xyz;
				o.sv_target.w = 1.0;

				// Apply fog and handle transparency
				UNITY_APPLY_FOG(inp.fogCoord, o.sv_target);
				UNITY_OPAQUE_ALPHA(o.sv_target.w);

				return o;
			}

			ENDCG
		}
	}
	Fallback "Mobile/Diffuse"
}
