Shader "CpRemix/World/Unlit Vertex Color Additive"
{
	Properties
	{
	  _Color("Color", Color) = (1,1,1,1)
	}
		SubShader
	{
	  Tags
	  {
		"QUEUE" = "Transparent"
	  }
	  LOD 100
	  Pass
	  {
		Tags
		{
		  "QUEUE" = "Transparent"
		}
		LOD 100
		ZWrite Off
		Cull Off
		Blend One One
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#pragma multi_compile_fog

			fixed4 _Color;

			struct v2f
			{
			float4 color : COLOR;
			};

			struct FragOutput
			{
			  fixed4 color : SV_Target;
			};

			v2f vert(
			float4 vertex : POSITION,
			float4 color : COLOR,
			out float4 outpos : SV_POSITION
			)
			{
			  v2f o;
			  outpos = UnityObjectToClipPos(vertex);
			  o.color = color;
			  return o;
			}


			FragOutput frag(v2f i)
			{
			  FragOutput o;
			  o.color = ((_Color * i.color) * i.color.w);
			  return o;
			}

			ENDCG
	  }
	}
		FallBack "Diffuse"
}
