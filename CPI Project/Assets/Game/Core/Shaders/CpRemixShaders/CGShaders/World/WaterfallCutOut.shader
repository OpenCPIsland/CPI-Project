Shader "CpRemix/World/WaterfallCutOut" {
	Properties {
		_Color ("Color", Vector) = (1,1,1,1)
		_MainTex ("Color (RGB) Alpha (A)", 2D) = "white" {}
		_XScrollSpeed ("X Scroll Speed", Float) = 1
		_YScrollSpeed ("Y Scroll Speed", Float) = 1
		_Cutoff ("Alpha cutoff", Range(0, 1)) = 0.5
	}
	SubShader {
		Tags {
            "RenderPipeline" = "UniversalPipeline" "QUEUE" = "Transparent" }
		Pass {
			Tags { "QUEUE" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			Cull Off
			//GpuProgramID 25894
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
			
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float2 texcoord1 : TEXCOORD1;
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			float _XScrollSpeed;
			float _YScrollSpeed;
			// $Globals ConstantBuffers for Fragment Shader
			float4 _Color;
			float _Cutoff;
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _MainTex;
			
			// Keywords: 
			v2f vert(Attributes v) {
    v2f o;

    // Use Unity's built-in function to transform vertex position from object space to clip space
    o.position = TransformObjectToHClip(v.vertex.xyz);

    // Calculate scrolling texture coordinates
    o.texcoord1.xy = float2(_XScrollSpeed.x, _YScrollSpeed.x) * _Time.xx + v.texcoord.xy;

    return o;
}

			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                
                float4 texResult = tex2D(_MainTex, inp.texcoord1.xy);
                
                if (texResult.w < _Cutoff) {
                    discard;
                }
                
                o.sv_target.w = texResult.w * _Color.w;
                o.sv_target.xyz = texResult.xyz + _Color.xyz;
                return o;
			}
			ENDHLSL
		}
	}
	Fallback Off
}