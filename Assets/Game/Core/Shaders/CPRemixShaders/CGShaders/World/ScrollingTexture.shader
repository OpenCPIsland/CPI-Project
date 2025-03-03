Shader "CpRemix/World/ScrollingTexture" {
	Properties {
		_Color ("Color", Vector) = (1,1,1,1)
		_MainTex ("Texture (RGB)", 2D) = "white" {}
		_XScrollSpeed ("X Scroll Speed", Float) = 1
		_YScrollSpeed ("Y Scroll Speed", Float) = 1
	}
	SubShader {
		Tags { "QUEUE" = "Geometry" }
		Pass {
			Tags { "QUEUE" = "Geometry" }
			GpuProgramID 48069
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// #pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float2 texcoord1 : TEXCOORD1;
				float3 color : COLOR0;
				// UNITY_FOG_COORDS(2)
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			float _XScrollSpeed;
			float _YScrollSpeed;
			// $Globals ConstantBuffers for Fragment Shader
			float3 _Color;
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _MainTex;
			
			// Keywords: 
v2f vert(appdata_full v)
{
    v2f o;

    // Simplified transformation to reduce temporary variable usage
    float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
    o.position = mul(unity_MatrixVP, worldPos);

    // Scroll texture coordinates based on time and scroll speed
    o.texcoord1.xy = float2(_XScrollSpeed.x, _YScrollSpeed.x) * _Time.xx + v.texcoord.xy;

    // Assign vertex color directly to output color
    o.color.xyz = v.color.xyz;

    // Apply fog
    // UNITY_TRANSFER_FOG(o, o.position);

    return o;
}


			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                tmp0 = tex2D(_MainTex, inp.texcoord1.xy);
                tmp0.xyz = tmp0.xyz + inp.color.xyz;
                o.sv_target.xyz = tmp0.xyz + _Color;
                o.sv_target.w = 1.0;
				// UNITY_APPLY_FOG(inp.fogCoord, o.sv_target);
				UNITY_OPAQUE_ALPHA(o.sv_target.w);
                return o;
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}