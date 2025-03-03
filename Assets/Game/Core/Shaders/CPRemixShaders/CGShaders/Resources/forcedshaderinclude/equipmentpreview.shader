Shader "CpRemix/Equipment Preview"
{
	Properties
	{
	  _Diffuse("Diffuse", 2D) = "black" {}
	  _Decal123OpacityTex("Decals 123 Opacity", 2D) = "black" {}
	  _Decal1Tex("Decal 1 Texture", 2D) = "white" {}
	  _Decal1Color("Decal 1 Color", Color) = (0.26,0.78,1,1)
	  _Decal1Scale("Decal 1 Scale", Range(0.1, 30)) = 1
	  _Decal1UOffset("Decal 1 uOffset", Range(-0.5, 0.5)) = 0
	  _Decal1VOffset("Decal 1 vOffset", Range(-0.5, 0.5)) = 0
	  _Decal1RotationRads("Decal 1 Rotation Rads", Range(-3.141, 3.141)) = 0
	  [MaterialToggle] _Decal1Repeat("Repeat Decal 1", float) = 0
	  _Decal2Tex("Decal 2 Texture", 2D) = "white" {}
	  _Decal2Color("Decal 2 Color", Color) = (0.06,0.55,1,1)
	  _Decal2Scale("Decal 2 Scale", Range(0.1, 30)) = 1
	  _Decal2UOffset("Decal 2 uOffset", Range(-0.5, 0.5)) = 0
	  _Decal2VOffset("Decal 2 vOffset", Range(-0.5, 0.5)) = 0
	  _Decal2RotationRads("Decal 2 Rotation Rads", Range(-3.141, 3.141)) = 0
	  [MaterialToggle] _Decal2Repeat("Repeat Decal 2", float) = 0
	  _Decal3Tex("Decal 3 Texture", 2D) = "white" {}
	  _Decal3Color("Decal 3 Color", Color) = (0.01,0.33,0.95,1)
	  _Decal3Scale("Decal 3 Scale", Range(0.1, 30)) = 1
	  _Decal3UOffset("Decal 3 uOffset", Range(-0.5, 0.5)) = 0
	  _Decal3VOffset("Decal 3 vOffset", Range(-0.5, 0.5)) = 0
	  _Decal3RotationRads("Decal 3 Rotation Rads", Range(-3.141, 3.141)) = 0
	  [MaterialToggle] _Decal3Repeat("Repeat Decal 3", float) = 0
	  _Decal4Tex("Decal 4 Texture", 2D) = "black" {}
	  _Decal4Color("Decal 4 Color", Color) = (1,1,1,1)
	  _Decal4Scale("Decal 4 Scale", Range(0.1, 30)) = 1
	  _Decal4UOffset("Decal 4 uOffset", Range(-0.5, 0.5)) = 0
	  _Decal4VOffset("Decal 4 vOffset", Range(-0.5, 0.5)) = 0
	  _Decal4RotationRads("Decal 4 Rotation Rads", Range(-3.141, 3.141)) = 0
	  [MaterialToggle] _Decal4Repeat("Repeat Decal 4", float) = 0
	  _Decal5Tex("Decal 5 Texture", 2D) = "black" {}
	  _Decal5Color("Decal 5 Color", Color) = (1,1,1,1)
	  _Decal5Scale("Decal 5 Scale", Range(0.1, 30)) = 1
	  _Decal5UOffset("Decal 5 uOffset", Range(-0.5, 0.5)) = 0
	  _Decal5VOffset("Decal 5 vOffset", Range(-0.5, 0.5)) = 0
	  _Decal5RotationRads("Decal 5 Rotation Rads", Range(-3.141, 3.141)) = 0
	  [MaterialToggle] _Decal5Repeat("Repeat Decal 5", float) = 0
	  _Decal6Tex("Decal 6 Texture", 2D) = "black" {}
	  _Decal6Color("Decal 6 Color", Color) = (1,1,1,1)
	  _Decal6Scale("Decal 6 Scale", Range(0.1, 30)) = 1
	  _Decal6UOffset("Decal 6 uOffset", Range(-0.5, 0.5)) = 0
	  _Decal6VOffset("Decal 6 vOffset", Range(-0.5, 0.5)) = 0
	  _Decal6RotationRads("Decal 6 Rotation Rads", Range(-3.141, 3.141)) = 0
	  [MaterialToggle] _Decal6Repeat("Repeat Decal 6", float) = 0
	  _BodyColorsMaskTex("Body Color Mask", 2D) = "black" {}
	  _BodyRedChannelColor("Body Red Channel Color", Color) = (1,0,0,1)
	  _BodyGreenChannelColor("Body Green Channel Color", Color) = (1,1,0,1)
	  _BodyBlueChannelColor("Body Blue Channel Color", Color) = (1,0,1,1)
	  _EmissiveColorTint("EmissiveColorTint", Color) = (1,1,1,1)
	  _DetailAndMatcapMaskAndEmissive("r=detail g=matcap b=emissive", 2D) = "black" {}
	}
		SubShader
	{
	  Tags
	  {
	  }
	  Pass // ind: 1, name: 
	  {
		Tags
		{
		}
		// m_ProgramMask = 6
		CGPROGRAM
		//#pragma target 4.0

		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"


		//uniform float4x4 unity_ObjectToWorld;
		//uniform float4x4 unity_MatrixVP;
		uniform  float _Decal1Scale;
		uniform  float _Decal1UOffset;
		uniform  float _Decal1VOffset;
		uniform  float _Decal1RotationRads;
		uniform  float _Decal2Scale;
		uniform  float _Decal2UOffset;
		uniform  float _Decal2VOffset;
		uniform  float _Decal2RotationRads;
		uniform  float _Decal3Scale;
		uniform  float _Decal3UOffset;
		uniform  float _Decal3VOffset;
		uniform  float _Decal3RotationRads;
		uniform  float _Decal4Scale;
		uniform  float _Decal4UOffset;
		uniform  float _Decal4VOffset;
		uniform  float _Decal4RotationRads;
		uniform  float _Decal5Scale;
		uniform  float _Decal5UOffset;
		uniform  float _Decal5VOffset;
		uniform  float _Decal5RotationRads;
		uniform  float _Decal6Scale;
		uniform  float _Decal6UOffset;
		uniform  float _Decal6VOffset;
		uniform  float _Decal6RotationRads;
		uniform sampler2D _Diffuse;
		uniform sampler2D _Decal123OpacityTex;
		uniform sampler2D _Decal1Tex;
		uniform  float3 _Decal1Color;
		uniform  float _Decal1Repeat;
		uniform sampler2D _Decal2Tex;
		uniform  float3 _Decal2Color;
		uniform  float _Decal2Repeat;
		uniform sampler2D _Decal3Tex;
		uniform  float3 _Decal3Color;
		uniform  float _Decal3Repeat;
		uniform sampler2D _Decal4Tex;
		uniform  float3 _Decal4Color;
		uniform  float _Decal4Repeat;
		uniform sampler2D _Decal5Tex;
		uniform  float3 _Decal5Color;
		uniform  float _Decal5Repeat;
		uniform sampler2D _Decal6Tex;
		uniform  float3 _Decal6Color;
		uniform  float _Decal6Repeat;

		struct appdata_t
		{
		float4 _glesVertex :POSITION;
		float4 _glesMultiTexCoord0 :TEXCOORD0;
		};

		struct OUT_Data_Vert
		{
		float2 xlv_TEXCOORD0 :TEXCOORD0;
		float2 xlv_TEXCOORD1 :TEXCOORD1;
		float2 xlv_TEXCOORD2 :TEXCOORD2;
		float2 xlv_TEXCOORD3 :TEXCOORD3;
		float2 xlv_TEXCOORD4 :TEXCOORD4;
		float2 xlv_TEXCOORD5 :TEXCOORD5;
		float2 xlv_TEXCOORD6 :TEXCOORD6;
		float4 gl_Position :SV_POSITION;
		};

		struct v2f
		{
		float2 xlv_TEXCOORD0 :TEXCOORD0;
		float2 xlv_TEXCOORD1 :TEXCOORD1;
		float2 xlv_TEXCOORD2 :TEXCOORD2;
		float2 xlv_TEXCOORD3 :TEXCOORD3;
		float2 xlv_TEXCOORD4 :TEXCOORD4;
		float2 xlv_TEXCOORD5 :TEXCOORD5;
		float2 xlv_TEXCOORD6 :TEXCOORD6;
		};

		struct OUT_Data_Frag
		{
		float4 gl_FragData :SV_Target0;
		};

		OUT_Data_Vert vert(appdata_t v)
		{
		  OUT_Data_Vert o;
		   float2 tmpvar_1;
		  tmpvar_1 = v._glesMultiTexCoord0.xy;
		   float2 decal6RotatedUVs_2;
		   float2 decal5RotatedUVs_3;
		   float2 decal4RotatedUVs_4;
		   float2 decal3RotatedUVs_5;
		   float2 decal2RotatedUVs_6;
		   float2 decal1RotatedUVs_7;
		   float4 tmpvar_8;
		  tmpvar_8.w = 1.0;
		  tmpvar_8.xyz = v._glesVertex.xyz;
		   float2 tmpvar_9;
		  tmpvar_9.x = (-0.5 + _Decal1UOffset);
		  tmpvar_9.y = (-0.5 + _Decal1VOffset);
		   float2 tmpvar_10;
		   float2 point2D_11;
		  point2D_11 = tmpvar_1;
		   float2 pointLocalCenterToOrigin_12;
		  pointLocalCenterToOrigin_12 = tmpvar_9;
		   float angleRadians_13;
		  angleRadians_13 = _Decal1RotationRads;
		   float tmpvar_14;
		  tmpvar_14 = sin(angleRadians_13);
		   float tmpvar_15;
		  tmpvar_15 = cos(angleRadians_13);
		   float2x2 tmpvar_16;
		  tmpvar_16[0].x = tmpvar_15;
		  tmpvar_16[0].y = tmpvar_14;
		  tmpvar_16[1].x = -(tmpvar_14);
		  tmpvar_16[1].y = tmpvar_15;
		  tmpvar_10 = (mul((point2D_11 + pointLocalCenterToOrigin_12), tmpvar_16) - pointLocalCenterToOrigin_12);
		  decal1RotatedUVs_7 = tmpvar_10;
		   float2 tmpvar_17;
		  tmpvar_17.x = _Decal1UOffset;
		  tmpvar_17.y = _Decal1VOffset;
		   float2 tmpvar_18;
		  tmpvar_18.x = (-0.5 + _Decal2UOffset);
		  tmpvar_18.y = (-0.5 + _Decal2VOffset);
		   float2 tmpvar_19;
		   float2 point2D_20;
		  point2D_20 = tmpvar_1;
		   float2 pointLocalCenterToOrigin_21;
		  pointLocalCenterToOrigin_21 = tmpvar_18;
		   float angleRadians_22;
		  angleRadians_22 = _Decal2RotationRads;
		   float tmpvar_23;
		  tmpvar_23 = sin(angleRadians_22);
		   float tmpvar_24;
		  tmpvar_24 = cos(angleRadians_22);
		   float2x2 tmpvar_25;
		  tmpvar_25[0].x = tmpvar_24;
		  tmpvar_25[0].y = tmpvar_23;
		  tmpvar_25[1].x = -(tmpvar_23);
		  tmpvar_25[1].y = tmpvar_24;
		  tmpvar_19 = (mul((point2D_20 + pointLocalCenterToOrigin_21), tmpvar_25) - pointLocalCenterToOrigin_21);
		  decal2RotatedUVs_6 = tmpvar_19;
		   float2 tmpvar_26;
		  tmpvar_26.x = _Decal2UOffset;
		  tmpvar_26.y = _Decal2VOffset;
		   float2 tmpvar_27;
		  tmpvar_27.x = (-0.5 + _Decal3UOffset);
		  tmpvar_27.y = (-0.5 + _Decal3VOffset);
		   float2 tmpvar_28;
		   float2 point2D_29;
		  point2D_29 = tmpvar_1;
		   float2 pointLocalCenterToOrigin_30;
		  pointLocalCenterToOrigin_30 = tmpvar_27;
		   float angleRadians_31;
		  angleRadians_31 = _Decal3RotationRads;
		   float tmpvar_32;
		  tmpvar_32 = sin(angleRadians_31);
		   float tmpvar_33;
		  tmpvar_33 = cos(angleRadians_31);
		   float2x2 tmpvar_34;
		  tmpvar_34[0].x = tmpvar_33;
		  tmpvar_34[0].y = tmpvar_32;
		  tmpvar_34[1].x = -(tmpvar_32);
		  tmpvar_34[1].y = tmpvar_33;
		  tmpvar_28 = (mul((point2D_29 + pointLocalCenterToOrigin_30), tmpvar_34) - pointLocalCenterToOrigin_30);
		  decal3RotatedUVs_5 = tmpvar_28;
		   float2 tmpvar_35;
		  tmpvar_35.x = _Decal3UOffset;
		  tmpvar_35.y = _Decal3VOffset;
		   float2 tmpvar_36;
		  tmpvar_36.x = (-0.5 + _Decal4UOffset);
		  tmpvar_36.y = (-0.5 + _Decal4VOffset);
		   float2 tmpvar_37;
		   float2 point2D_38;
		  point2D_38 = tmpvar_1;
		   float2 pointLocalCenterToOrigin_39;
		  pointLocalCenterToOrigin_39 = tmpvar_36;
		   float angleRadians_40;
		  angleRadians_40 = _Decal4RotationRads;
		   float tmpvar_41;
		  tmpvar_41 = sin(angleRadians_40);
		   float tmpvar_42;
		  tmpvar_42 = cos(angleRadians_40);
		   float2x2 tmpvar_43;
		  tmpvar_43[0].x = tmpvar_42;
		  tmpvar_43[0].y = tmpvar_41;
		  tmpvar_43[1].x = -(tmpvar_41);
		  tmpvar_43[1].y = tmpvar_42;
		  tmpvar_37 = (mul((point2D_38 + pointLocalCenterToOrigin_39), tmpvar_43) - pointLocalCenterToOrigin_39);
		  decal4RotatedUVs_4 = tmpvar_37;
		   float2 tmpvar_44;
		  tmpvar_44.x = _Decal4UOffset;
		  tmpvar_44.y = _Decal4VOffset;
		   float2 tmpvar_45;
		  tmpvar_45.x = (-0.5 + _Decal5UOffset);
		  tmpvar_45.y = (-0.5 + _Decal5VOffset);
		   float2 tmpvar_46;
		   float2 point2D_47;
		  point2D_47 = tmpvar_1;
		   float2 pointLocalCenterToOrigin_48;
		  pointLocalCenterToOrigin_48 = tmpvar_45;
		   float angleRadians_49;
		  angleRadians_49 = _Decal5RotationRads;
		   float tmpvar_50;
		  tmpvar_50 = sin(angleRadians_49);
		   float tmpvar_51;
		  tmpvar_51 = cos(angleRadians_49);
		   float2x2 tmpvar_52;
		  tmpvar_52[0].x = tmpvar_51;
		  tmpvar_52[0].y = tmpvar_50;
		  tmpvar_52[1].x = -(tmpvar_50);
		  tmpvar_52[1].y = tmpvar_51;
		  tmpvar_46 = (mul((point2D_47 + pointLocalCenterToOrigin_48), tmpvar_52) - pointLocalCenterToOrigin_48);
		  decal5RotatedUVs_3 = tmpvar_46;
		   float2 tmpvar_53;
		  tmpvar_53.x = _Decal5UOffset;
		  tmpvar_53.y = _Decal5VOffset;
		   float2 tmpvar_54;
		  tmpvar_54.x = (-0.5 + _Decal6UOffset);
		  tmpvar_54.y = (-0.5 + _Decal6VOffset);
		   float2 tmpvar_55;
		   float2 point2D_56;
		  point2D_56 = tmpvar_1;
		   float2 pointLocalCenterToOrigin_57;
		  pointLocalCenterToOrigin_57 = tmpvar_54;
		   float angleRadians_58;
		  angleRadians_58 = _Decal6RotationRads;
		   float tmpvar_59;
		  tmpvar_59 = sin(angleRadians_58);
		   float tmpvar_60;
		  tmpvar_60 = cos(angleRadians_58);
		   float2x2 tmpvar_61;
		  tmpvar_61[0].x = tmpvar_60;
		  tmpvar_61[0].y = tmpvar_59;
		  tmpvar_61[1].x = -(tmpvar_59);
		  tmpvar_61[1].y = tmpvar_60;
		  tmpvar_55 = (mul((point2D_56 + pointLocalCenterToOrigin_57), tmpvar_61) - pointLocalCenterToOrigin_57);
		  decal6RotatedUVs_2 = tmpvar_55;
		   float2 tmpvar_62;
		  tmpvar_62.x = _Decal6UOffset;
		  tmpvar_62.y = _Decal6VOffset;
		  o.gl_Position = mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_8));
		  o.xlv_TEXCOORD0 = tmpvar_1;
		  o.xlv_TEXCOORD1 = (((
			(decal1RotatedUVs_7 + tmpvar_17)
		   - float2(0.5, 0.5)) * _Decal1Scale) + float2(0.5, 0.5));
		  o.xlv_TEXCOORD2 = (((
			(decal2RotatedUVs_6 + tmpvar_26)
		   - float2(0.5, 0.5)) * _Decal2Scale) + float2(0.5, 0.5));
		  o.xlv_TEXCOORD3 = (((
			(decal3RotatedUVs_5 + tmpvar_35)
		   - float2(0.5, 0.5)) * _Decal3Scale) + float2(0.5, 0.5));
		  o.xlv_TEXCOORD4 = (((
			(decal4RotatedUVs_4 + tmpvar_44)
		   - float2(0.5, 0.5)) * _Decal4Scale) + float2(0.5, 0.5));
		  o.xlv_TEXCOORD5 = (((
			(decal5RotatedUVs_3 + tmpvar_53)
		   - float2(0.5, 0.5)) * _Decal5Scale) + float2(0.5, 0.5));
		  o.xlv_TEXCOORD6 = (((
			(decal6RotatedUVs_2 + tmpvar_62)
		   - float2(0.5, 0.5)) * _Decal6Scale) + float2(0.5, 0.5));
		   return o;
		}


		OUT_Data_Frag frag(v2f f)
		{
		  OUT_Data_Frag o;
		   float3 decalOpacitySample_1;
		   float3 diffuseSample_2;
		   float3 tmpvar_3;
		  tmpvar_3 = tex2D(_Diffuse, f.xlv_TEXCOORD0).xyz;
		  diffuseSample_2 = tmpvar_3;
		   float3 tmpvar_4;
		  tmpvar_4 = tex2D(_Decal123OpacityTex, f.xlv_TEXCOORD0).xyz;
		  decalOpacitySample_1 = tmpvar_4;
		   float4 tmpvar_5;
		  tmpvar_5 = tex2D(_Decal3Tex, f.xlv_TEXCOORD3);
		   float2 tmpvar_6;
		  tmpvar_6 = abs(((f.xlv_TEXCOORD3 - 0.5) * 2.0));
		   float4 tmpvar_7;
		  tmpvar_7 = (tmpvar_5 * float((
			(1.0 + (255.0 * _Decal3Repeat))
		   >=
			max(tmpvar_6.x, tmpvar_6.y)
		  )));
		   float tmpvar_8;
		  tmpvar_8 = (tmpvar_7.w * decalOpacitySample_1.z);
		   float4 tmpvar_9;
		  tmpvar_9 = tex2D(_Decal2Tex, f.xlv_TEXCOORD2);
		   float2 tmpvar_10;
		  tmpvar_10 = abs(((f.xlv_TEXCOORD2 - 0.5) * 2.0));
		   float4 tmpvar_11;
		  tmpvar_11 = (tmpvar_9 * float((
			(1.0 + (255.0 * _Decal2Repeat))
		   >=
			max(tmpvar_10.x, tmpvar_10.y)
		  )));
		   float tmpvar_12;
		  tmpvar_12 = (tmpvar_11.w * decalOpacitySample_1.y);
		   float4 tmpvar_13;
		  tmpvar_13 = tex2D(_Decal1Tex, f.xlv_TEXCOORD1);
		   float2 tmpvar_14;
		  tmpvar_14 = abs(((f.xlv_TEXCOORD1 - 0.5) * 2.0));
		   float4 tmpvar_15;
		  tmpvar_15 = (tmpvar_13 * float((
			(1.0 + (255.0 * _Decal1Repeat))
		   >=
			max(tmpvar_14.x, tmpvar_14.y)
		  )));
		   float tmpvar_16;
		  tmpvar_16 = (tmpvar_15.w * decalOpacitySample_1.x);
		   float4 tmpvar_17;
		  tmpvar_17 = tex2D(_Decal6Tex, f.xlv_TEXCOORD6);
		   float2 tmpvar_18;
		  tmpvar_18 = abs(((f.xlv_TEXCOORD6 - 0.5) * 2.0));
		   float4 tmpvar_19;
		  tmpvar_19 = (tmpvar_17 * float((
			(1.0 + (255.0 * _Decal6Repeat))
		   >=
			max(tmpvar_18.x, tmpvar_18.y)
		  )));
		   float tmpvar_20;
		  tmpvar_20 = (tmpvar_19.w * decalOpacitySample_1.z);
		   float4 tmpvar_21;
		  tmpvar_21 = tex2D(_Decal5Tex, f.xlv_TEXCOORD5);
		   float2 tmpvar_22;
		  tmpvar_22 = abs(((f.xlv_TEXCOORD5 - 0.5) * 2.0));
		   float4 tmpvar_23;
		  tmpvar_23 = (tmpvar_21 * float((
			(1.0 + (255.0 * _Decal5Repeat))
		   >=
			max(tmpvar_22.x, tmpvar_22.y)
		  )));
		   float tmpvar_24;
		  tmpvar_24 = ((tmpvar_23.w * decalOpacitySample_1.y) * (1.0 - tmpvar_20));
		   float4 tmpvar_25;
		  tmpvar_25 = tex2D(_Decal4Tex, f.xlv_TEXCOORD4);
		   float2 tmpvar_26;
		  tmpvar_26 = abs(((f.xlv_TEXCOORD4 - 0.5) * 2.0));
		   float4 tmpvar_27;
		  tmpvar_27 = (tmpvar_25 * float((
			(1.0 + (255.0 * _Decal4Repeat))
		   >=
			max(tmpvar_26.x, tmpvar_26.y)
		  )));
		   float tmpvar_28;
		  tmpvar_28 = (((tmpvar_27.w * decalOpacitySample_1.x) * (1.0 - tmpvar_24)) * (1.0 - tmpvar_20));
		   float tmpvar_29;
		  tmpvar_29 = min(1.0, ((tmpvar_20 + tmpvar_24) + tmpvar_28));
		   float tmpvar_30;
		  tmpvar_30 = min(1.0, (min(1.0,
			((tmpvar_8 + tmpvar_12) + tmpvar_16)
		  ) + tmpvar_29));
		   float4 tmpvar_31;
		  tmpvar_31.xyz = (((
			(((tmpvar_27.xyz * _Decal4Color) * tmpvar_28) + ((tmpvar_23.xyz * _Decal5Color) * tmpvar_24))
		   +
			((tmpvar_19.xyz * _Decal6Color) * tmpvar_20)
		  ) * tmpvar_29) + ((
			(((tmpvar_15.xyz * _Decal1Color) * tmpvar_16) + ((tmpvar_11.xyz * _Decal2Color) * tmpvar_12))
		   +
			((tmpvar_7.xyz * _Decal3Color) * tmpvar_8)
		  ) * (1.0 - tmpvar_29)));
		  tmpvar_31.w = tmpvar_30;
		   float4 tmpvar_32;
		  tmpvar_32.w = 1.0;
		  tmpvar_32.xyz = ((diffuseSample_2 * (1.0 - tmpvar_30)) + (tmpvar_31.xyz * tmpvar_30));
		  o.gl_FragData = tmpvar_32;
		  return o;
		}
	ENDCG
  } // end phase
	  Pass // ind: 2, name: 
  {
	Tags
	{
	}
	Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

		// float4x4 unity_ObjectToWorld;
		 //float4x4 unity_MatrixVP;
		 sampler2D _BodyColorsMaskTex;
		 float3 _BodyRedChannelColor;
		 float3 _BodyGreenChannelColor;
		 float3 _BodyBlueChannelColor;

		 struct v2f
		 {
		 float2 xlv_TEXCOORD0 : TEXCOORD0;
		 };

		 struct FragOutput
		 {
		 float4 gl_FragData : SV_Target;
		 };

		 v2f vert(
		 float4 _glesVertex : POSITION,
		 float4 _glesMultiTexCoord0 : TEXCOORD0,
		 out float4 gl_Position : SV_POSITION
		 )
		 {
		   v2f o;
		   float4 tmpvar_1;
		   tmpvar_1.w = 1.0;
		   tmpvar_1.xyz = _glesVertex.xyz;
		   gl_Position = UnityObjectToClipPos(tmpvar_1); //mul(unity_MatrixVP, mul(unity_ObjectToWorld * tmpvar_1));
		   o.xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
		   return o;
		 }

		 FragOutput frag(v2f i)
		 {
		   FragOutput o;
		   float4 tmpvar_1;
		   float4 bodyColors_2;
		   float2 texUV_3;
		   texUV_3 = i.xlv_TEXCOORD0;
		   float4 tmpvar_4;
		   tmpvar_4 = tex2D(_BodyColorsMaskTex, texUV_3);
		   float3 tmpvar_5;
		   tmpvar_5 = tmpvar_4.xyz;
		   float4 tmpvar_6;
		   tmpvar_6.xyz = (((tmpvar_5.x * _BodyRedChannelColor) + (tmpvar_5.y * _BodyGreenChannelColor)) + (tmpvar_5.z * _BodyBlueChannelColor));
		   tmpvar_6.w = max(max(tmpvar_5.x, tmpvar_5.y), tmpvar_5.z);
		   bodyColors_2 = tmpvar_6;
		   tmpvar_1 = bodyColors_2;
		   o.gl_FragData = tmpvar_1;
		   return o;
		 }

	   ENDCG

   } // end phase
   Pass // ind: 3, name: 
   {
	 Tags
	 {
	   "LIGHTMODE" = "FORWARDBASE"
	 }
	 Blend DstColor SrcColor
		 CGPROGRAM

		 #pragma vertex vert
		 #pragma fragment frag

		 #include "UnityCG.cginc"
		 #include "AutoLight.cginc"
		 #include "Lighting.cginc"

		   // float4 _WorldSpaceLightPos0;
		   // float4x4 unity_ObjectToWorld;
		   // float4x4 unity_WorldToObject;
		   // float4 glstate_lightmodel_ambient;
		   // float4x4 unity_MatrixVP;
		   // float4 _LightColor0;
			float3 _EmissiveColorTint;
			sampler2D _DetailAndMatcapMaskAndEmissive;


			struct v2f
			{
			float2 xlv_TEXCOORD0 : TEXCOORD0;
			float3 xlv_TEXCOORD1 : TEXCOORD1;
			float3 xlv_COLOR0 : COLOR;
			};

			struct FragOutput
			{
			float4 gl_FragData : SV_Target;
			};

			v2f vert(
			    float4 _glesVertex : POSITION,
			    float4 _glesColor : COLOR,
			    float3 _glesNormal : NORMAL,
			    float4 _glesMultiTexCoord0 : TEXCOORD0,
			    out float4 gl_Position : SV_POSITION
			)
			{
			    v2f o;
			
			    // Pass through the vertex color
			    float4 vertexColor = _glesColor;
			
			    // Transform the normal from object space to world space and normalize it
			    float3 worldSpaceNormal = normalize(mul((float3x3)unity_ObjectToWorld, _glesNormal));
			
			    // Calculate the direction of the light in world space and normalize it
			    float3 worldSpaceLightDir = normalize(_WorldSpaceLightPos0.xyz - mul(unity_ObjectToWorld, _glesVertex).xyz * _WorldSpaceLightPos0.w);
			
			    // Calculate diffuse lighting based on the angle between the normal and the light direction
			    float3 diffuseLighting = _LightColor0.xyz * max(0.0, dot(worldSpaceNormal, worldSpaceLightDir)) * 0.65;
			
			    // Calculate ambient lighting
			    float3 ambientLighting = glstate_lightmodel_ambient.xyz * 0.45 * 2.0;
			
			    // Combine diffuse and ambient lighting
			    float3 lighting = diffuseLighting + ambientLighting;
			
			    // Transform vertex position to clip space
			    gl_Position = UnityObjectToClipPos(_glesVertex);
			
			    // Pass through the texture coordinates and calculated lighting
			    o.xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
			    o.xlv_TEXCOORD1 = lighting;
			    o.xlv_COLOR0 = vertexColor;
			
			    return o;
			}



			FragOutput frag(v2f i)
			{
			  FragOutput o;
			  float3 lightingOrEmissive_1;
			  float3 detail_MatcapMask_Emissive_2;
			  float3 tmpvar_3;
			  tmpvar_3 = tex2D(_DetailAndMatcapMaskAndEmissive, i.xlv_TEXCOORD0).xyz;
			  detail_MatcapMask_Emissive_2 = tmpvar_3;
			  float3 tmpvar_4;
			  tmpvar_4 = (((i.xlv_TEXCOORD1 * detail_MatcapMask_Emissive_2.x) * (1.0 - detail_MatcapMask_Emissive_2.z)) + (_EmissiveColorTint * detail_MatcapMask_Emissive_2.z));
			  lightingOrEmissive_1 = tmpvar_4;
			  float4 tmpvar_5;
			  tmpvar_5.w = 1.0;
			  tmpvar_5.xyz = (lightingOrEmissive_1 * 0.47);
			  o.gl_FragData = tmpvar_5;
			  return o;
			}


			ENDCG

	  } // end phase
	}
		FallBack Off
}
