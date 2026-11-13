Shader "CpRemix/World/Clothing Designer Environment" {
	Properties {
		_TintColor ("Tint Color", Color) = (1,1,1,1)
		_Diffuse ("Diffuse Texture", 2D) = "" {}
	}
	SubShader {
		Tags { "RenderPipeline" = "UniversalPipeline" }
        Pass {
			Tags { "LightMode" = "UniversalForward" }
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
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float3 color : COLOR0;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float3 normalWS : TEXCOORD2;
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
			sampler2D _Diffuse;
			
			// Keywords: 
			v2f vert(Attributes v)
{
    v2f o;

    // Transform vertex to world space and then to clip space in one step
    o.position = TransformObjectToHClip(v.vertex.xyz);

    // Pass through the vertex color
    o.color.xyz = v.color.xyz;

    // Compute texture coordinates with Lightmap scaling
    o.texcoord1.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    o.normalWS = TransformObjectToWorldNormal(v.normal);

    // Pass through base texture coordinates
    o.texcoord.xy = v.texcoord.xy;

    return o;
}

			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                float4 tmp1;
                tmp0 = float4(SampleLightmap(inp.texcoord1.xy, normalize(inp.normalWS)), 1.0);
                tmp1 = tex2D(_Diffuse, inp.texcoord.xy);
                tmp0.xyz = tmp0.xyz * tmp1.xyz;
                tmp0.xyz = tmp0.xyz * inp.color.xyz;
                o.sv_target.xyz = tmp0.xyz * _TintColor.xyz;
                o.sv_target.w = 1.0;
                return o;
			}
			ENDHLSL
		}
	}
	Fallback Off
}
