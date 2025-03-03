Shader "CpRemix/World/Events/Color/ColorEventLightScroll" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_ScrollSpeed ("Y Scroll Speed", Range(-20, 20)) = 0
	}
	SubShader {
		LOD 100
		Tags { "RenderType" = "Opaque" }
		Pass {
			LOD 100
			Tags { "RenderType" = "Opaque" }
			ZClip Off
			GpuProgramID 5222
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float2 texcoord : TEXCOORD0;
				float4 position : SV_POSITION0;
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			float4 _MainTex_ST;
			// $Globals ConstantBuffers for Fragment Shader
			float _ScrollSpeed;
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _MainTex;
			
			// Keywords: 
			v2f vert(appdata_full v)
{
    v2f o;
    
    // Calculate the texture coordinates
    o.texcoord.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
    
    // Transform vertex position to world space using the object to world matrix
    float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
    
    // Transform the world position to clip space using the view-projection matrix
    o.position = mul(UNITY_MATRIX_VP, worldPos);
    
    return o;
}

			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                tmp0.y = _ScrollSpeed * _Time.x;
                tmp0.x = 0.0;
                tmp0.xy = tmp0.xy + inp.texcoord.xy;
                o.sv_target = tex2D(_MainTex, tmp0.xy);
                return o;
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}