Shader "CpRemix/World/Vertex Alpha Sine Bounce Unlit (Fog)" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
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
				float2 texcoord : TEXCOORD0;
				float4 color : COLOR0;
				float4 position : SV_POSITION0;
				float fogFactor : TEXCOORD1;
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
			v2f vert(Attributes v)
{
    v2f o;

    // Calculate texture coordinates
    o.texcoord.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;

    // Copy vertex color
    o.color = v.color;

    // Transform vertex position from object space to world space
    float4 worldPosition = mul(unity_ObjectToWorld, v.vertex);

    // Transform the world position to clip space
    float4 clipPosition = mul(UNITY_MATRIX_VP, worldPosition);

    // Apply sine bounce effect based on vertex alpha and time
    float sineOffset = sin(v.color.w * _Time.y);
    o.position.y = clipPosition.y + sineOffset;
    o.position.xzw = clipPosition.xzw;

    // Handle fog
    o.fogFactor = ComputeFogFactor(o.position.z);

    return o;
}

			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                tmp0 = tex2D(_MainTex, inp.texcoord.xy);
                o.sv_target = tmp0 * inp.color;
				o.sv_target.rgb = MixFog(o.sv_target.rgb, inp.fogFactor);
				o.sv_target.w = 1.0;
                return o;
			}
			ENDHLSL
		}
	}
	Fallback Off
}