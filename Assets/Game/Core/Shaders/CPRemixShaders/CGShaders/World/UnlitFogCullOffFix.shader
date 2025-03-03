Shader "CpRemix/World/Fix/Cull Off/Unlit Dynamic Object (FOG)" {
	Properties {
		_TintColor ("Tint Color", Vector) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		Pass {
			Cull Off
			Tags { "RenderType" = "Opaque" }
			GpuProgramID 37350
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float2 texcoord : TEXCOORD0;
				float4 color : COLOR0;
				UNITY_FOG_COORDS(1)
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
			v2f vert(appdata_full v)
{
    v2f o;

    // Compute world space position
    float4 worldPos = mul(unity_ObjectToWorld, v.vertex);

    // Compute clip space position using VP matrix
    o.position = mul(unity_MatrixVP, worldPos);

    // Apply texture transformations
    o.texcoord.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;

    // Pass through vertex color
    o.color = v.color;

    // Apply fog (Unity's built-in fog handling)
    UNITY_TRANSFER_FOG(o, o.position);

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
				UNITY_APPLY_FOG(inp.fogCoord, o.sv_target);
				UNITY_OPAQUE_ALPHA(o.sv_target.w);
                return o;
			}
			ENDCG
		}
	}
}