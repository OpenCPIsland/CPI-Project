Shader "CpRemix/World/Highlight Unlit Unity" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_HighlightColor ("Highlight Color", Color) = (1,1,1,1)
		_HighlightIntensity ("Highlight Intensity", Range(0, 1)) = 0.6
	}
	SubShader {
		Tags { "RenderPipeline" = "UniversalPipeline" }
        LOD 100
		Tags { "RenderType" = "Opaque" }
		Pass {
			LOD 100
			Tags { "RenderType" = "Opaque" }
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
			
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float2 texcoord : TEXCOORD0;
				float3 texcoord2 : TEXCOORD2;
				float4 color : COLOR0;
				float fogFactor : TEXCOORD3;
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
			v2f vert(Attributes v)
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
    o.fogFactor = ComputeFogFactor(o.position.z);
    
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
				o.sv_target.rgb = MixFog(o.sv_target.rgb, inp.fogFactor);
				o.sv_target.w = 1.0;
                return o;
			}
			ENDHLSL
		}
	}
}