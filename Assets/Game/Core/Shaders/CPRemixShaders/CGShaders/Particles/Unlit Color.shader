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
		"RenderType" = "Opaque"
	  }
	  Pass
	  {
		Tags
		{
		  "RenderType" = "Opaque"
		}

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			fixed4 _Color;

			struct FragOutput
			{
			  fixed4 color : SV_Target;
			};

			void vert(
			float4 vertex : POSITION,
			out float4 outpos : SV_POSITION
			)
			{
			  outpos = UnityObjectToClipPos(vertex);
			}

			FragOutput frag()
			{
			  FragOutput o;
			  o.color = _Color;
			  return o;
			}

			ENDCG
	  }
	}
		FallBack "Diffuse"
}
