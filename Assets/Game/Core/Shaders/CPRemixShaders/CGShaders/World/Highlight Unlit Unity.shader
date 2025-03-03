Shader "CpRemix/World/Highlight Unlit Unity" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_HighlightColor ("Highlight Color", Vector) = (1,1,1,1)
		_HighlightIntensity ("Highlight Intensity", Range(0, 1)) = 0.6
	}
	SubShader {
		LOD 100
		Tags { "RenderType" = "Opaque" }
		Pass {
			LOD 100
			Tags { "RenderType" = "Opaque" }
			GpuProgramID 2226
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float2 texcoord : TEXCOORD0;
				float3 texcoord2 : TEXCOORD2;
				float4 color : COLOR0;
				UNITY_FOG_COORDS(3)
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			float3 _HighlightColor;
			float _HighlightIntensity;
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
    
    // Transform the vertex to world space
    float4 worldPos = mul(unity_ObjectToWorld, v.vertex);

    // Transform world position to clip space
    float4 clipPos = mul(UNITY_MATRIX_VP, worldPos);
    o.position = clipPos;

    // Pass through the texture coordinates
    o.texcoord.xy = v.texcoord.xy;

    // Compute highlight intensity based on sine and cosine time
    float2 sinCosOffset;
    sinCosOffset.y = _SinTime.w * 6.0 + -clipPos.x;
    sinCosOffset.x = _CosTime.w * 6.0 + -clipPos.x;

    // Calculate highlight effect
    sinCosOffset = float2(1.0, 1.0) - abs(sinCosOffset);
    float highlightIntensity = max(max(sinCosOffset.x, sinCosOffset.y), 0.0);
    highlightIntensity *= _HighlightIntensity;

    // Apply highlight color and intensity
    o.texcoord2.xyz = highlightIntensity * _HighlightColor;

    // Pass through vertex color
    o.color = v.color;

    // Apply fog
    UNITY_TRANSFER_FOG(o, o.position);
    
    return o;
}

			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                tmp0 = tex2D(_MainTex, inp.texcoord.xy);
                o.sv_target.xyz = tmp0.xyz * inp.color.xyz + inp.texcoord2.xyz;
                o.sv_target.w = 1.0;
				UNITY_APPLY_FOG(inp.fogCoord, o.sv_target);
				UNITY_OPAQUE_ALPHA(o.sv_target.w);
                return o;
			}
			ENDCG
		}
	}
}