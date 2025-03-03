Shader "CpRemix/World/Waterfall" {
	Properties {
		_Color ("Color", Vector) = (1,1,1,1)
		_MainTex ("Color (RGB) Alpha (A)", 2D) = "white" {}
		_XScrollSpeed ("X Scroll Speed", Float) = 1
		_YScrollSpeed ("Y Scroll Speed", Float) = 1
	}
	SubShader {
		Tags { "QUEUE" = "Transparent" }
		Pass {
			Tags { "QUEUE" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			GpuProgramID 36015
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float2 texcoord1 : TEXCOORD1;
				//UNITY_FOG_COORDS(2)
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
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _MainTex;
			
			// Keywords: 
			v2f vert(appdata_full v) {
    v2f o;

    // Use Unity's built-in function to transform vertex position from object space to clip space
    o.position = UnityObjectToClipPos(v.vertex);

    // Calculate scrolling texture coordinates
    o.texcoord1.xy = float2(_XScrollSpeed.x, _YScrollSpeed.x) * _Time.xx + v.texcoord.xy;

    // Handle fog
    //UNITY_TRANSFER_FOG(o, o.position);

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
				//UNITY_APPLY_FOG(inp.fogCoord, o.sv_target);
                return o;
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}