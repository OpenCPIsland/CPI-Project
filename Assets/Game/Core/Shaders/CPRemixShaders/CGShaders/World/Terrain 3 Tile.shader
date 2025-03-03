Shader "CpRemix/World/Terrain 3 Tile" {
	Properties {
		_RedChannelTex ("Red Channel Texture", 2D) = "" {}
		_RedChannelTexTile ("Red Channel Tiling", Range(0.1, 50)) = 1
		_RedChannelSpec ("Red Channel Specularity", Range(0, 1)) = 0
		_RedChannelShininess ("Red Channel Shininess", Range(0.5, 20)) = 0.5
		_RedSpecFallOff ("Red Spec FallOff", Range(1, 10)) = 1
		_GreenChannelTex ("Green Channel Texture", 2D) = "" {}
		_GreenChannelTexTile ("Green Channel Tiling", Range(0.1, 50)) = 1
		_GreenChannelSpec ("Green Channel Specularity", Range(0, 1)) = 0
		_GreenChannelShininess ("Green Channel Shininess", Range(0.5, 20)) = 0.5
		_GreenSpecFallOff ("Green Spec FallOff", Range(1, 10)) = 1
		_BlueChannelTex ("Blue Channel Texture", 2D) = "" {}
		_BlueChannelTexTile ("Blue Channel Tiling", Range(0.1, 50)) = 1
		_BlueChannelSpec ("Blue Channel Specularity", Range(0, 1)) = 0
		_BlueChannelShininess ("Blue Channel Shininess", Range(0.5, 20)) = 0.5
		_BlueSpecFallOff ("Blue Spec FallOff", Range(1, 10)) = 1
		_AlphaDepthColor ("Alpha Depth Color", Vector) = (0,0,0.5,1)
		[HideInspector] _BlobShadowTex ("Blob Shadow Tex", 2D) = "white" {}
	}
	SubShader {
		Pass {
			Tags { "LIGHTMODE" = "ALWAYS" }
			GpuProgramID 47581
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			struct v2f
			{
			
				float4 position : SV_POSITION0;
				float4 color : COLOR0;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float2 texcoord2 : TEXCOORD2;
				float2 texcoord3 : TEXCOORD3;
				float3 texcoord5 : TEXCOORD5;
				UNITY_FOG_COORDS(6)
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			float _RedChannelTexTile;
			float _GreenChannelTexTile;
			float _BlueChannelTexTile;
			float _ShadowPlaneDim;
			float _ShadowTextureDim;
			float3 _ShadowPlaneWorldPos;
			// $Globals ConstantBuffers for Fragment Shader
			float3 _AlphaDepthColor;
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _RedChannelTex;
			sampler2D _GreenChannelTex;
			sampler2D _BlueChannelTex;
			sampler2D _BlobShadowTex;
			
			// Keywords: 
			v2f vert(appdata_full v)
{
    v2f o;

    // Compute world-space position (use built-in mul for matrix multiplication)
    float4 worldPos = mul(unity_ObjectToWorld, v.vertex);

    // Compute clip-space position (using View-Projection matrix)
    o.position = mul(unity_MatrixVP, worldPos);

    // Pass vertex color to the fragment shader
    o.color = v.color;

    // Compute lightmap and texture coordinates
    o.texcoord.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    o.texcoord1.xy = v.texcoord.xy * _RedChannelTexTile.xx;
    o.texcoord2.xy = v.texcoord.xy * _GreenChannelTexTile.xx;
    o.texcoord3.xy = v.texcoord.xy * _BlueChannelTexTile.xx;

    // Shadow plane calculations
    float2 shadowOffset = worldPos.xz - _ShadowPlaneWorldPos.xz;
    o.texcoord5.z = worldPos.y;
    float shadowPlaneHalfDim = _ShadowPlaneDim * 0.5;
    float2 shadowCoord = shadowOffset / shadowPlaneHalfDim;
    float shadowScale = _ShadowPlaneDim / _ShadowTextureDim;
    shadowCoord += shadowScale / _ShadowPlaneDim;
    shadowCoord += float2(1.0, 1.0);
    o.texcoord5.xy = shadowCoord * 0.5;

    // Apply fog (Unity built-in)
    UNITY_TRANSFER_FOG(o, o.position);

    return o;
}

			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                float4 tmp1;
                float4 tmp2;
                tmp0 = tex2D(_GreenChannelTex, inp.texcoord2.xy);
                tmp1 = tex2D(_BlueChannelTex, inp.texcoord3.xy);
                tmp2.xyz = float3(1.0, 1.0, 1.0) - inp.color.xyw;
                tmp1.xyz = tmp1.xyz * tmp2.yyy;
                tmp0.xyz = tmp0.xyz * inp.color.yyy + tmp1.xyz;
                tmp0.xyz = tmp2.xxx * tmp0.xyz;
                tmp1.xyz = _AlphaDepthColor * tmp2.zzz + inp.color.www;
                tmp2 = tex2D(_RedChannelTex, inp.texcoord1.xy);
                tmp0.xyz = tmp2.xyz * inp.color.xxx + tmp0.xyz;
                tmp2 = UNITY_SAMPLE_TEX2D_SAMPLER(unity_Lightmap, unity_Lightmap, inp.texcoord.xy);
                tmp0.w = tmp2.w * unity_Lightmap_HDR.x;
                tmp2.xyz = tmp2.xyz * tmp0.www;
                tmp0.xyz = tmp0.xyz * tmp2.xyz;
                tmp0.xyz = tmp1.xyz * tmp0.xyz;
                tmp1 = tex2D(_BlobShadowTex, inp.texcoord5.xy);
                tmp0.w = inp.texcoord5.z >= tmp1.y;
                tmp0.w = tmp0.w ? 2.0 : 1.0;
                tmp1.y = tmp1.y - inp.texcoord5.z;
                tmp0.w = abs(tmp1.y) * tmp0.w + tmp0.w;
                tmp0.w = tmp0.w - 0.5;
                tmp0.w = max(tmp0.w, 1.0);
                tmp0.w = tmp1.x * tmp0.w;
                tmp0.w = min(tmp0.w, 1.0);
                o.sv_target.xyz = tmp0.www * tmp0.xyz;
                o.sv_target.w = 1.0;
				UNITY_APPLY_FOG(inp.fogCoord, o.sv_target);
				UNITY_OPAQUE_ALPHA(o.sv_target.w);
                return o;
			}
			ENDCG
		}
	}
}