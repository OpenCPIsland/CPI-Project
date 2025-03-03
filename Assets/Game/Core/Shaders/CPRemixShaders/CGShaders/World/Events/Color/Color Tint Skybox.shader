Shader "CpRemix/World/Events/Color/Color Tint Skybox" {
	Properties {
		_TintColor ("Tint Color", Vector) = (1,1,1,1)
		_cubemap ("Environment Map", Cube) = "white" {}
	}
	SubShader {
		Tags { "QUEUE" = "Background" }
		Pass {
			Tags { "QUEUE" = "Background" }
			ZClip Off
			ZWrite Off
			Cull Off
			GpuProgramID 20910
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float3 texcoord : TEXCOORD0;
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
			samplerCUBE _cubemap;
			
			// Keywords: 
			v2f vert(appdata_full v)
{
    v2f o;
    
    // Transform the vertex position from object space to world space
    float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
    
    // Transform the world position to clip space using the view-projection matrix
    o.position = mul(UNITY_MATRIX_VP, worldPos);

    // Pass through the texture coordinates (xyz if required, otherwise xy)
    o.texcoord.xyz = v.texcoord.xyz;

    return o;
}

			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                tmp0 = texCUBE(_cubemap, inp.texcoord.xyz);
                o.sv_target = tmp0 * _TintColor;
                return o;
			}
			ENDCG
		}
	}
}