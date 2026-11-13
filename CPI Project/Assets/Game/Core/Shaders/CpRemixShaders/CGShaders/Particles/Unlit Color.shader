Shader "CpRemix/Particles/Unlit Color"
{
	Properties
	{
	  _Color("Color", Color) = (1,1,1,1)
	}
		SubShader
	{
	  Tags
	  {
            "RenderPipeline" = "UniversalPipeline"
		"RenderType" = "Opaque"
	  }
	  Pass
	  {
		Tags
		{
		  "RenderType" = "Opaque"
		}

			HLSLPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			half4 _Color;

			struct FragOutput
			{
			  half4 color : SV_Target;
			};

			void vert(
			float4 vertex : POSITION,
			out float4 outpos : SV_POSITION
			)
			{
			  outpos = TransformObjectToHClip(vertex.xyz);
			}

			FragOutput frag()
			{
			  FragOutput o;
			  o.color = _Color;
			  return o;
			}

			ENDHLSL
	  }
	}
		Fallback Off
}
