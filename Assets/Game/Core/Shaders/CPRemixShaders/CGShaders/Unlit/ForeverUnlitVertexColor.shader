Shader "Unlit/ForeverUnlitVertexColor" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader {
		LOD 100
		Tags { "RenderType" = "Opaque" }
		Pass {
			LOD 100
			Tags { "RenderType" = "Opaque" }
			GpuProgramID 28247
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float2 texcoord : TEXCOORD0;
				float4 position : SV_POSITION0;
				float4 color : COLOR0;
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			float4 _MainTex_ST;
			// $Globals ConstantBuffers for Fragment Shader
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _MainTex;
			
			// Keywords: 
			v2f vert(appdata_full v)
{
    v2f o;
    
    // Adjust texture coordinates using the _MainTex_ST for scale and offset
    o.texcoord.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;

    // Transform vertex position to clip space using UnityObjectToClipPos
    o.position = UnityObjectToClipPos(v.vertex);
    
    // Pass vertex color through to the output
    o.color = v.color;

    return o;
}

			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                tmp0 = tex2D(_MainTex, inp.texcoord.xy);
                o.sv_target = tmp0 * inp.color;
                return o;
			}
			ENDCG
		}
	}
}