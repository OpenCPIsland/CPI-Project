Shader "CpRemix/World/Clothing Designer Environment" {
	Properties {
		_TintColor ("Tint Color", Vector) = (1,1,1,1)
		_Diffuse ("Diffuse Texture", 2D) = "" {}
	}
	SubShader {
		Pass {
			Tags { "LIGHTMODE" = "FORWARDBASE" }
			GpuProgramID 57088
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float3 color : COLOR0;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			// $Globals ConstantBuffers for Fragment Shader
			float4 _TintColor;
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _Diffuse;
			
			// Keywords: 
			v2f vert(appdata_full v)
{
    v2f o;

    // Transform vertex to world space and then to clip space in one step
    o.position = UnityObjectToClipPos(v.vertex);

    // Pass through the vertex color
    o.color.xyz = v.color.xyz;

    // Compute texture coordinates with Lightmap scaling
    o.texcoord1.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;

    // Pass through base texture coordinates
    o.texcoord.xy = v.texcoord.xy;

    return o;
}

			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                float4 tmp1;
                tmp0 = UNITY_SAMPLE_TEX2D_SAMPLER(unity_Lightmap, unity_Lightmap, inp.texcoord1.xy);
                tmp0.w = tmp0.w * unity_Lightmap_HDR.x;
                tmp0.xyz = tmp0.xyz * tmp0.www;
                tmp1 = tex2D(_Diffuse, inp.texcoord.xy);
                tmp0.xyz = tmp0.xyz * tmp1.xyz;
                tmp0.xyz = tmp0.xyz * inp.color.xyz;
                o.sv_target.xyz = tmp0.xyz * _TintColor.xyz;
                o.sv_target.w = 1.0;
                return o;
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}