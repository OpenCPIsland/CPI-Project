Shader "CpRemix/World/Vertex Alpha Sine Bounce Lightmap (Fog)"
{
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderPipeline" = "UniversalPipeline" }
        LOD 100
		Tags { "RenderType" = "Opaque" }
		Pass {
			LOD 100
			Tags { "LightMode" = "UniversalForward" "RenderType" = "Opaque" }
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
			#pragma multi_compile_fog
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			// Input structure for the vertex shader
			struct v2f {
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float3 normalWS : TEXCOORD3;
				float4 color : COLOR0;
				float4 position : SV_POSITION0;
				float fogFactor : TEXCOORD2;
			};

			// Declare shader properties
			float4 _MainTex_ST;
			sampler2D _MainTex;

			// Vertex shader
			v2f vert(Attributes v)
			{
				v2f o;

				// Calculate texture coordinates
				o.texcoord1.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				o.normalWS = TransformObjectToWorldNormal(v.normal);
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
				o.fogFactor = ComputeFogFactor(o.position.z);

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
				float4 lightmapColor = float4(SampleLightmap(inp.texcoord1.xy, normalize(inp.normalWS)), 1.0);

				float4 texColor = tex2D(_MainTex, inp.texcoord.xy);

				// Combine the lightmap, texture, and vertex color
				o.sv_target.xyz = lightmapColor.xyz * texColor.xyz * inp.color.xyz;
				o.sv_target.w = 1.0;

				// Apply fog and handle transparency
				o.sv_target.rgb = MixFog(o.sv_target.rgb, inp.fogFactor);
				o.sv_target.w = 1.0;

				return o;
			}

			ENDHLSL
		}
	}
	Fallback Off
}
