Shader "CpRemix/World/HighlightUnity" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_HighlightColor ("Highlight Color", Color) = (1,1,1,1)
		_HighlightIntensity ("Highlight Intensity", Range(0, 1)) = 0.6
	}
	SubShader {
		Tags { "RenderPipeline" = "UniversalPipeline" }
        LOD 100
		Tags { "LightMode" = "UniversalForward" "RenderType" = "Opaque" }
		Pass {
			LOD 100
			Tags { "LightMode" = "UniversalForward" "RenderType" = "Opaque" }
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
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float3 normalWS : TEXCOORD2;
				float3 color : COLOR0;
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
    
    // Calculate world position of the vertex
    float4 worldPos = mul(unity_ObjectToWorld, v.vertex);

    // Calculate position in view space (Clip space)
    float4 clipPos = mul(UNITY_MATRIX_VP, worldPos);
    o.position = clipPos;

    // Calculate texture coordinates
    o.texcoord1.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    o.normalWS = TransformObjectToWorldNormal(v.normal);
    o.texcoord.xy = v.texcoord.xy;

    // Compute highlight effect using sine and cosine time-based distortion
    float2 sinCosOffset;
    sinCosOffset.y = _SinTime.w * 6.0 + -clipPos.x;
    sinCosOffset.x = _CosTime.w * 6.0 + -clipPos.x;
    
    // Calculate the highlight intensity based on the offset
    sinCosOffset = float2(1.0, 1.0) - abs(sinCosOffset);
    float highlight = max(max(sinCosOffset.x, sinCosOffset.y), 0.0);
    
    // Apply highlight intensity and color
    o.color.xyz = highlight * _HighlightIntensity * _HighlightColor;

    // Transfer fog data
    o.fogFactor = ComputeFogFactor(o.position.z);
    
    return o;
}

			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                float4 tmp1;
                tmp0 = float4(SampleLightmap(inp.texcoord1.xy, normalize(inp.normalWS)), 1.0);
                tmp1 = tex2D(_MainTex, inp.texcoord.xy);
                o.sv_target.xyz = tmp1.xyz * tmp0.xyz + inp.color.xyz;
                o.sv_target.w = 1.0;
				o.sv_target.rgb = MixFog(o.sv_target.rgb, inp.fogFactor);
				o.sv_target.w = 1.0;
                return o;
			}
			ENDHLSL
		}
	}
}
