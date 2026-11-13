Shader "CpRemix/World/Waterfall" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Color (RGB) Alpha (A)", 2D) = "white" {}
		_XScrollSpeed ("X Scroll Speed", Float) = 1
		_YScrollSpeed ("Y Scroll Speed", Float) = 1
        _Reverse ("Reverse (X,Y)", Vector) = (0,0,0,0)
	}
	SubShader {
		Tags {
            "RenderPipeline" = "UniversalPipeline" "QUEUE" = "Transparent" }
		Pass {
			Tags { "QUEUE" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
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
			//#pragma multi_compile_fog
			
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float2 texcoord1 : TEXCOORD1;
				//float fogFactor : TEXCOORD2;
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			float _XScrollSpeed;
			float _YScrollSpeed;
            float4 _Reverse; // X,Y for reversing directions
			// $Globals ConstantBuffers for Fragment Shader
			float4 _Color;
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

                // Compute reverse multipliers: 1 for normal, -1 for reversed
                float reverseX = (_Reverse.x > 0.5) ? -1.0 : 1.0;
                float reverseY = (_Reverse.y > 0.5) ? -1.0 : 1.0;

                // Calculate scrolling texture coordinates
                o.texcoord1.xy = float2(reverseX * _XScrollSpeed.x, reverseY * _YScrollSpeed.x) * _Time.xx + v.texcoord.xy;

                // Handle fog
                //o.fogFactor = ComputeFogFactor(o.position.z);

                return o;
            }

			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                tmp0 = tex2D(_MainTex, inp.texcoord1.xy);
                o.sv_target.w = tmp0.w * _Color.w;
                o.sv_target.xyz = tmp0.xyz + _Color.xyz;
				//o.sv_target.rgb = MixFog(o.sv_target.rgb, inp.fogFactor);
                return o;
			}
			ENDHLSL
		}
	}
	Fallback Off
}
