Shader "Custom/mg_pt_Shader_PizzaSauce"
{
	Properties
	{
	  _Color("Main Color", Color) = (1,1,1,0.5)
	  _MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
	  Tags
	  {
		"QUEUE" = "Transparent"
	  }
	  Pass
	  {
		Tags
		{
		  "QUEUE" = "Transparent"
		}
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			float4x4 _ScaleTransform;
			float4 _Color;
			sampler2D _MainTex;

			struct v2f
			{
			float4 uv : TEXCOORD0;
			};

			struct FragOutput
			{
			float4 color : SV_Target;
			};

			v2f vert(
			float4 vertex : POSITION,
			out float4 gl_Position : SV_POSITION
			)
			{
			  v2f o;
			  gl_Position = mul(unity_MatrixVP, mul(_ScaleTransform, vertex));
			  o.uv = vertex;
			  return o;
			}


			FragOutput frag(v2f i)
			{
			  FragOutput o;
			  float4 result_1;
			  result_1 = float4(0.0, 0.0, 0.0, 0.0);
			  float tmpvar_2;
			  tmpvar_2 = abs(i.uv.x);
			  float tmpvar_3;
			  if ((tmpvar_2 < 0.5)) {
				float tmpvar_4;
				tmpvar_4 = abs(i.uv.y);
				tmpvar_3 = (tmpvar_4 < 0.5);
			  }
   else {
  tmpvar_3 = float(0);
};
if (tmpvar_3) {
  float4 texcol_5;
  float4 tmpvar_6;
  float2 P_7;
  P_7 = (i.uv.xy + float2(0.5, 0.5));
  tmpvar_6 = tex2D(_MainTex, P_7);
  texcol_5 = tmpvar_6;
  if ((texcol_5.w > 0.0)) {
	result_1 = (texcol_5 * _Color);
  };
};
o.color = result_1;
return o;
}


ENDCG

}
	}
		FallBack "Unlit/Transparent"
}
