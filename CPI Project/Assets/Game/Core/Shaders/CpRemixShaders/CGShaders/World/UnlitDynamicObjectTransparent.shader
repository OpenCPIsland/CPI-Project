Shader "CpRemix/World/Unlit Dynamic Object Transparent (FOG)" {
	Properties {
		_TintColor ("Tint Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader {
		Tags {
            "RenderPipeline" = "UniversalPipeline" "RenderType" = "Transparent" "Queue" = "Transparent" }
		Pass {
			Tags { "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
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
				float4 color : COLOR0;
				float fogFactor : TEXCOORD1;
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			float4 _MainTex_ST;
			// $Globals ConstantBuffers for Fragment Shader
			float4 _TintColor;
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _MainTex;
			
			// Keywords: 
			v2f vert(Attributes v)
{
    v2f o;

    // Compute world space position
    float4 worldPos = mul(unity_ObjectToWorld, v.vertex);

    // Compute clip space position using VP matrix
    o.position = mul(UNITY_MATRIX_VP, worldPos);

    // Apply texture transformations
    o.texcoord.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;

    // Pass through vertex color
    o.color = v.color;

    // Apply fog (Unity's built-in fog handling)
    o.fogFactor = ComputeFogFactor(o.position.z);

    return o;
}

			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                tmp0 = tex2D(_MainTex, inp.texcoord.xy);
                tmp0 = tmp0 * inp.color;
                o.sv_target = tmp0 * _TintColor;
				o.sv_target.rgb = MixFog(o.sv_target.rgb, inp.fogFactor);
                // Keep transparency
                return o;
			}
			ENDHLSL
		}
	}
}
