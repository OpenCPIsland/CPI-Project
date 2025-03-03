Shader "CpRemix/UI/CurvedBorderButtonShadowScaled"
{
	Properties
	{
	  [PerRendererData] _MainTex("Sprite Texture", 2D) = "black" {}
	  _Color("Tint", Color) = (1,1,1,1)
	  _Centre("Centre Color", Color) = (0,0.372,0.792,1)
	  _Border("Border Color", Color) = (1,1,1,1)
	  _BorderSize("Border Size", float) = 0.15
	  _AAliasSize("Anti-Aliasing Size", float) = 0.03
	  _Roundness("Roundness", float) = 1
	  _ShadowVec("Shadow Vector", Vector) = (-0.05,0.15,0,0)
	  _ScaleBox("Scale For Shadow", float) = 1.2
	  _ScaleImage("Scale Image", float) = 0.7
	  _ImageAttenuation("Image Attenuation", float) = 1
	  _StencilComp("Stencil Comparison", float) = 8
	  _Stencil("Stencil ID", float) = 0
	  _StencilOp("Stencil Operation", float) = 0
	  _StencilWriteMask("Stencil Write Mask", float) = 255
	  _StencilReadMask("Stencil Read Mask", float) = 255
	  _ColorMask("Color Mask", float) = 15
	}
		SubShader
	  {
		Tags
		{
		  "PreviewType" = "Plane"
		  "QUEUE" = "Transparent"
		}
		Pass // ind: 1, name: 
		{
		  Tags
		  {
			"PreviewType" = "Plane"
			"QUEUE" = "Transparent"
		  }
		  ZWrite Off
		  Cull Off
		  Stencil
		  {
			Ref[_Stencil]
			ReadMask[_StencilReadMask]
			WriteMask[_StencilWriteMask]
			Pass[_StencilOp]
			Comp[_StencilComp]
			Fail Keep
			ZFail Keep
			PassFront Keep
			FailFront Keep
			ZFailFront Keep
			PassBack Keep
			FailBack Keep
			ZFailBack Keep
		  }
		  Blend SrcAlpha OneMinusSrcAlpha
		  ColorMask[_ColorMask]
					  CGPROGRAM

			  #pragma vertex vert
			  #pragma fragment frag

			  #include "UnityCG.cginc"

		  // float4x4 unity_ObjectToWorld;
		  // float4x4 unity_MatrixVP;
		   float4 _Color;
		   float _ScaleBox;
		   float _ScaleImage;
		   float4 _Centre;
		   float4 _Border;
		   float _AAliasSize;
		   float _BorderSize;
		   float _Roundness;
		   float2 _ShadowVec;
		   float _ImageAttenuation;
		   uniform sampler2D _MainTex;

		   struct v2f
		   {
		   float4 xlv_COLOR : COLOR;
		   float2 xlv_TEXCOORD0 : TEXCOORD0;
		   float2 xlv_TEXCOORD1 : TEXCOORD1;
		   };

		   struct fragOutput {
		   float4 gl_FragData : SV_Target;
		   };

		   v2f vert(
		   float4 _glesVertex : POSITION,
		   float4 _glesColor : COLOR,
		   float4 _glesMultiTexCoord0 : TEXCOORD0,
		   out float4 gl_Position : SV_POSITION
		   )
		   {
			 v2f o;
			 float tmpvar_1;
			 tmpvar_1 = (1.0 / (_ScaleImage));
			 float4 tmpvar_2;
			 tmpvar_2.w = 1.0;
			 tmpvar_2.xyz = _glesVertex.xyz;
			 gl_Position = UnityObjectToClipPos(tmpvar_2);//mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_2));
			 o.xlv_COLOR = (_glesColor * _Color);
			 o.xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * tmpvar_1) + ((1.0 - tmpvar_1) / 2.0));
			 o.xlv_TEXCOORD1 = (((_glesMultiTexCoord0.xy * 2.0) - 1.0) * _ScaleBox);
			 return o;
		   }

		   fragOutput frag(v2f i)
		   {
			 fragOutput o;
			 float4 fragment_1;
			 float4 image_2;
			 float tmpvar_3;
			 tmpvar_3 = (1.0 / (_AAliasSize));
			 float tmpvar_4;
			 tmpvar_4 = (1.0 - _AAliasSize);
			 float2 tmpvar_5;
			 tmpvar_5 = pow(abs(i.xlv_TEXCOORD1), (_Roundness));
			 float2 tmpvar_6;
			 tmpvar_6 = pow(abs((i.xlv_TEXCOORD1 + _ShadowVec)), (_Roundness));
			 float tmpvar_7;
			 tmpvar_7 = pow((1.0 - _BorderSize), _Roundness);
			 float tmpvar_8;
			 tmpvar_8 = (tmpvar_7 - _AAliasSize);
			 float tmpvar_9;
			 tmpvar_9 = sqrt(dot(tmpvar_5, tmpvar_5));
			 float tmpvar_10;
			 tmpvar_10 = (1.0 - ((
			   clamp(tmpvar_9, tmpvar_4, 1.0)
			  - tmpvar_4) * tmpvar_3));
			 float tmpvar_11;
			 tmpvar_11 = (1.0 - ((
			   clamp(tmpvar_9, tmpvar_8, tmpvar_7)
			  - tmpvar_8) * tmpvar_3));
			 float tmpvar_12;
			 tmpvar_12 = (1.0 - ((
			   clamp(sqrt(dot(tmpvar_6, tmpvar_6)), tmpvar_8, tmpvar_7)
			  - tmpvar_8) * tmpvar_3));
			 float tmpvar_13;
			 tmpvar_13 = (1.0 - ((
			   clamp(sqrt(dot(tmpvar_6, tmpvar_6)), tmpvar_4, 1.0)
			  - tmpvar_4) * tmpvar_3));
			 float tmpvar_14;
			 tmpvar_14 = min((tmpvar_10 * 1000.0), 1.0);
			 float4 tmpvar_15;
			 tmpvar_15 = tex2D(_MainTex, i.xlv_TEXCOORD0);
			 image_2 = tmpvar_15;
			 float2 tmpvar_16;
			 tmpvar_16 = abs(((i.xlv_TEXCOORD0 - 0.5) * 2.0));
			 float tmpvar_17;
			 tmpvar_17 = max(tmpvar_16.x, tmpvar_16.y);
			 float tmpvar_18;
			 tmpvar_18 = max((image_2.w - (
			   (float((tmpvar_17 >= 1.0)) * _ImageAttenuation)
			  * tmpvar_17)), 0.0);
			 float tmpvar_19;
			 if ((tmpvar_12 < 0.9)) {
			   tmpvar_19 = (0.8 + (tmpvar_12 * 0.2));
			 }
  else {
 tmpvar_19 = 1.0;
};
float tmpvar_20;
if ((tmpvar_13 > 0.5)) {
  tmpvar_20 = (tmpvar_13 * 0.2);
}
else {
tmpvar_20 = 0.0;
};
fragment_1.xyz = (((
  ((_Centre * tmpvar_11) * (1.0 - tmpvar_18))
 +
  ((image_2 * tmpvar_18) * tmpvar_11)
) * tmpvar_19) + ((_Border *
  (1.0 - tmpvar_11)
) * tmpvar_14)).xyz;
fragment_1.w = max(tmpvar_10, tmpvar_20);
float4 tmpvar_21;
tmpvar_21 = (fragment_1 * i.xlv_COLOR);
o.gl_FragData = tmpvar_21;
return o;
}

ENDCG
} // end phase
	  }
		  FallBack Off
}
