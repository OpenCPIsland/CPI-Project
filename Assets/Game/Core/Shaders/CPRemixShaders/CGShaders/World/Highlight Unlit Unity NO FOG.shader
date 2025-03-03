Shader "CpRemix/World/Highlight Unlit Unity NO FOG" {
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
			GpuProgramID 41547
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float2 texcoord : TEXCOORD0;
				float3 texcoord2 : TEXCOORD2;
				float4 color : COLOR0;
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
    
    // Transform the vertex to world space using a single matrix multiplication
    float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
    
    // Convert world position to clip space in one step
    o.position = mul(UNITY_MATRIX_VP, worldPos);

    // Pass through the texture coordinates as they are
    o.texcoord.xy = v.texcoord.xy;

    // Compute highlight intensity based on sine and cosine time
    float highlightX = _CosTime.w * 6.0 - o.position.x;
    float highlightY = _SinTime.w * 6.0 - o.position.x;

    // Calculate highlight effect
    float2 highlight = float2(1.0, 1.0) - abs(float2(highlightX, highlightY));
    float highlightIntensity = max(max(highlight.x, highlight.y), 0.0) * _HighlightIntensity;

    // Apply the highlight color based on the intensity
    o.texcoord2.xyz = highlightIntensity * _HighlightColor;

    // Pass through vertex color
    o.color = v.color;

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
                return o;
			}
			ENDCG
		}
	}
}