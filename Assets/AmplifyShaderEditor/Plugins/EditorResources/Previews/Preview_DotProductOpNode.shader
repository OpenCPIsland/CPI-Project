Shader "Hidden/DotProductOpNode"
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
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;
			int _Type_PID;

			float4 frag(v2f_img i) : SV_Target
			{
				float4 a = tex2D(_A, i.uv);
				float4 b = tex2D(_B, i.uv);
	
				float result;
				switch ( _Type_PID )
				{
					case 1: result = dot( a.x, b.x ); break;
					case 2: result = dot( a.xy, b.xy ); break;
					case 3: result = dot( a.xyz, b.xyz ); break;
					default: result = dot( a.xyzw, b.xyzw ); break;
				}
				return result;
			}
			ENDCG
		}
	}
}
