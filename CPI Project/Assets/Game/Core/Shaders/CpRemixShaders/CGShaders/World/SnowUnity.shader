Shader "CpRemix/World/Snow Ramp" {
	Properties {
		_SnowRampTex ("Snow Ramp", 2D) = "white" {}
		[HideInspector] _BlobShadowTex ("Blob Shadow Tex", 2D) = "white" {}
	}
	SubShader {
		Tags {
            "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
		Pass {
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
			struct v2f
			{
				float4 position : SV_POSITION0;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float3 normalWS : TEXCOORD4;
				float3 texcoord3 : TEXCOORD3;
				float fogFactor : TEXCOORD2;
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			float _ShadowPlaneDim;
			float _ShadowTextureDim;
			float3 _ShadowPlaneWorldPos;
			// $Globals ConstantBuffers for Fragment Shader
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _SnowRampTex;
			sampler2D _BlobShadowTex;
			
			// Optimized vert function
			v2f vert(Attributes v) {
                v2f o;
                
                // Calculate world position
                float4 worldPos = float4(
                    dot(v.vertex, unity_ObjectToWorld[0]),
                    dot(v.vertex, unity_ObjectToWorld[1]),
                    dot(v.vertex, unity_ObjectToWorld[2]),
                    dot(v.vertex, unity_ObjectToWorld[3])
                );

                // Transform world position to clip space
                o.position = mul(UNITY_MATRIX_VP, worldPos);

                // Set texture coordinates
                o.texcoord1 = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                o.normalWS = TransformObjectToWorldNormal(v.normal);
                o.texcoord = v.normal.yy * float2(0.45, 0.45) + float2(0.5, 0.5);

                // Calculate shadow texture coordinates
				float2 shadowCoord = (worldPos.xz - _ShadowPlaneWorldPos.xz) / max(_ShadowPlaneDim * 0.5, 0.0001);
                o.texcoord3.xy = (shadowCoord + float2(1.0, 1.0)) * 0.5;
                o.texcoord3.z = worldPos.y;

                // Transfer fog coordinates
                o.fogFactor = ComputeFogFactor(o.position.z);

                return o;
            }

			// Fragment function
			fout frag(v2f inp)
			{
                fout o;
                
                float4 shadowResult = tex2D(_BlobShadowTex, inp.texcoord3.xy);
                shadowResult.z = inp.texcoord3.z >= shadowResult.y ? 2.0 : 1.0;
                shadowResult.y = (abs(shadowResult.y - inp.texcoord3.z) * shadowResult.z + shadowResult.z) - 0.5;
                shadowResult.y = max(shadowResult.y, 1.0);
                float shadowFactor = shadowResult.x * shadowResult.y;
                shadowFactor = min(shadowFactor, 1.0);
                
                float4 lightmapResult = float4(SampleLightmap(inp.texcoord1.xy, normalize(inp.normalWS)), 1.0);
                
                float4 snowRampResult = tex2D(_SnowRampTex, inp.texcoord.xy);
                snowRampResult.xyz = lightmapResult.xyz * snowRampResult.xyz;
                
                o.sv_target = float4(shadowFactor.xxx * snowRampResult.xyz, snowRampResult.w);
				o.sv_target.rgb = MixFog(o.sv_target.rgb, inp.fogFactor);
				o.sv_target.w = 1.0;
                return o;
			}
			ENDHLSL
		}
	}
}
