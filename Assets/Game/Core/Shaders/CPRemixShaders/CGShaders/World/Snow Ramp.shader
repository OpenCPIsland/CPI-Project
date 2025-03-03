Shader "CpRemix/World/Snow Ramp" {
	Properties {
		_SnowRampTex ("Snow Ramp", 2D) = "white" {}
		[HideInspector] _BlobShadowTex ("Blob Shadow Tex", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		Pass {
			Tags { "RenderType" = "Opaque" }
			GpuProgramID 17079
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float3 texcoord3 : TEXCOORD3;
				UNITY_FOG_COORDS(2)
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
			v2f vert(appdata_full v) {
                v2f o;
                
                // Calculate world position
                float4 worldPos = float4(
                    dot(v.vertex, unity_ObjectToWorld[0]),
                    dot(v.vertex, unity_ObjectToWorld[1]),
                    dot(v.vertex, unity_ObjectToWorld[2]),
                    dot(v.vertex, unity_ObjectToWorld[3])
                );

                // Transform world position to clip space
                o.position = mul(unity_MatrixVP, worldPos);

                // Set texture coordinates
                o.texcoord1 = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                o.texcoord = v.normal.yy * float2(0.45, 0.45) + float2(0.5, 0.5);

                // Calculate shadow texture coordinates
                float2 shadowCoord = (worldPos.xz - _ShadowPlaneWorldPos.xz) / (_ShadowPlaneDim * 0.5);
                o.texcoord3.xy = (shadowCoord + float2(1.0, 1.0)) * 0.5;
                o.texcoord3.z = worldPos.y;

                // Transfer fog coordinates
                UNITY_TRANSFER_FOG(o, o.position);

                return o;
            }

			// Fragment function
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                float4 tmp1;
                tmp0 = tex2D(_BlobShadowTex, inp.texcoord3.xy);
                tmp0.z = inp.texcoord3.z >= tmp0.y;
                tmp0.z = tmp0.z ? 2.0 : 1.0;
                tmp0.y = tmp0.y - inp.texcoord3.z;
                tmp0.y = abs(tmp0.y) * tmp0.z + tmp0.z;
                tmp0.y = tmp0.y - 0.5;
                tmp0.y = max(tmp0.y, 1.0);
                tmp0.x = tmp0.x * tmp0.y;
                tmp0.x = min(tmp0.x, 1.0);
                tmp1 = UNITY_SAMPLE_TEX2D_SAMPLER(unity_Lightmap, unity_Lightmap, inp.texcoord1.xy);
                tmp0.y = tmp1.w * unity_Lightmap_HDR.x;
                tmp0.yzw = tmp1.xyz * tmp0.yyy;
                tmp1 = tex2D(_SnowRampTex, inp.texcoord.xy);
                tmp1.xyz = tmp0.yzw * tmp1.xyz;
                o.sv_target = tmp0.xxxx * tmp1;
				UNITY_APPLY_FOG(inp.fogCoord, o.sv_target);
				UNITY_OPAQUE_ALPHA(o.sv_target.w);
                return o;
			}
			ENDCG
		}
	}
}
