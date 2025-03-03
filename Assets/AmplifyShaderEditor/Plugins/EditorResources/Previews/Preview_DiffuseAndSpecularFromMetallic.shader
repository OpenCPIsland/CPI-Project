Shader "Hidden/DiffuseAndSpecularFromMetallicNode"
{
	Properties
	{
		_A ("_A", 2D) = "white" {}
		_B ("_B", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#include "Preview.cginc"
			#include "UnityStandardUtils.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;

			float3 ComputeDiffuseAndFresnel0( float3 baseColor, float metallic, out float3 specularColor, out float oneMinusReflectivity )
			{
			#if defined( UNITY_COLORSPACE_GAMMA )
				const float dielectricF0 = 0.220916301;
			#else
				const float dielectricF0 = 0.04;
			#endif
				specularColor = lerp( dielectricF0.xxx, baseColor, metallic );
				oneMinusReflectivity = 1.0 - metallic;
				return baseColor * oneMinusReflectivity;
			}
		
			float4 frag( v2f_img i ) : SV_Target
			{
				float4 baseColor = tex2D( _A, i.uv );
				float metallic = tex2D( _B, i.uv ).r;
				float3 specularColor = 0;
				float oneMinusReflectivity = 0;	
				float3 diffuseColor = ComputeDiffuseAndFresnel0( baseColor, metallic, specularColor, oneMinusReflectivity );
				return float4( diffuseColor, 1 );
			}
			ENDCG
		}
	}
}
