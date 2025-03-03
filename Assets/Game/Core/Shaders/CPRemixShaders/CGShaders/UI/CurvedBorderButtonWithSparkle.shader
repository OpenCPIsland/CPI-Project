Shader "CpRemix/UI/CurvedBorderButtonWithSparkle"
{
	Properties
	{
	  [PerRendererData] _MainTex("Sprite Texture", 2D) = "black" {}
	  _Color("Tint", Color) = (1,1,1,1)
	  _Tile("Tile", Vector) = (1,1,0,0)
	  _TileAttenuation("Tile Attenuation", float) = 0
	  _Centre("Centre Color", Color) = (0,0.372,0.792,1)
	  _Border("Border Color", Color) = (1,1,1,1)
	  _BorderSize("Border Size ", float) = 0.15
	  _AAliasSize("Anti-Aliasing Size", float) = 0.03
	  _Roundness("Roundness", float) = 1
	  _ShadowVec("Outer Shadow Vector", Vector) = (-0.05,0.15,0,0)
	  _ShadowInnerVec("Inner Shadow Vector", Vector) = (-0.05,0.15,0,0)
	  _InnerShading("Inner Shading Value", float) = 0.2
	  _OuterShading("Outer Shading Value", float) = 0.2
	  _InnerShadowBlur("Inner Shadow Falloff", float) = 0.03
	  _OuterShadowBlur("Outer Shadow Falloff", float) = 0.03
	  _EffectTex("Sparkle Texture", 2D) = "white" {}
	  _RotationScalar("Rotation Speed", float) = 0.1
	  _EffectAlpha("Effect Alpha", float) = 0.7
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
		Pass
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

		  float4 _Color;
		  float2 _ShadowVec;
		  float4 _Tile;
		  float _RotationScalar;
		  float4 _Centre;
		  float4 _Border;
		  float _AAliasSize;
		  float _BorderSize;
		  float _Roundness;
		  float2 _ShadowInnerVec;
		  float _InnerShading;
		  float _OuterShading;
		  float _OuterShadowBlur;
		  float _InnerShadowBlur;
		  float _TileAttenuation;
		  float _EffectAlpha;
		  sampler2D _MainTex;
		  sampler2D _EffectTex;

		  struct v2f
		  {
			float4 pos : SV_POSITION;
			float4 xlv_COLOR : COLOR;
			float2 xlv_TEXCOORD0 : TEXCOORD0;
			float2 xlv_TEXCOORD1 : TEXCOORD1;
			float2 xlv_TEXCOORD2 : TEXCOORD2;
			float2 xlv_TEXCOORD3 : TEXCOORD3;
		  };

		  v2f vert(appdata_full v)
		  {
			v2f o;
			float2 tmpvar_1 = (float2(1.0, 1.0) + (abs(_ShadowVec) * 0.5));
			float2 tmpvar_2;
			tmpvar_2.x = (tmpvar_1.x / tmpvar_1.y);
			tmpvar_2.y = tmpvar_1.y;

			o.pos = UnityObjectToClipPos(v.vertex);
			o.xlv_COLOR = v.color * _Color;

			float2 tmpvar_13 = (_ShadowVec * 0.5);
			o.xlv_TEXCOORD0 = (((v.texcoord.xy * _Tile.xy) + _Tile.zw) * tmpvar_2) - tmpvar_13;
			o.xlv_TEXCOORD1 = (((v.texcoord.xy * 2.0) - 1.0) * tmpvar_1) - tmpvar_13;

			float tmpvar_4;
			float4 tmpvar_5 = (_RotationScalar * _Time);
			tmpvar_4 = cos(tmpvar_5).x;
			float tmpvar_6;
			float tmpvar_7 = -(_RotationScalar);
			tmpvar_6 = cos((tmpvar_7 * _Time)).x;
			float tmpvar_8;
			tmpvar_8 = sin(tmpvar_5).x;
			float tmpvar_9;
			tmpvar_9 = sin((tmpvar_7 * _Time)).x;
			float2x2 tmpvar_10;
			tmpvar_10[0].x = tmpvar_4;
			tmpvar_10[0].y = tmpvar_9;
			tmpvar_10[1].x = tmpvar_8;
			tmpvar_10[1].y = tmpvar_4;
			float2x2 tmpvar_11;
			tmpvar_11[0].x = tmpvar_6;
			tmpvar_11[0].y = tmpvar_8;
			tmpvar_11[1].x = tmpvar_9;
			tmpvar_11[1].y = tmpvar_6;
			float2 tmpvar_12 = (v.texcoord.xy - float2(0.5, 0.5));

			o.xlv_TEXCOORD2 = (mul(tmpvar_10, tmpvar_12) + float2(0.5, 0.5));
			o.xlv_TEXCOORD3 = (mul(tmpvar_11, tmpvar_12) + float2(0.5, 0.5));

			return o;
		  }

		  struct FragOutput
		  {
			float4 gl_FragData : SV_Target;
		  };

		  FragOutput frag(v2f i)
		  {
			 FragOutput o;
			 float4 xlat_mutable_Centre;
			 xlat_mutable_Centre.w = _Centre.w;
			 float4 fragment_1;
			 float4 fx2_2;
			 float4 fx1_3;
			 float4 image_4;
			 float tmpvar_5;
			 tmpvar_5 = (1.0 / (_AAliasSize));
			 float tmpvar_6;
			 tmpvar_6 = (1.0 - _AAliasSize);
			 float tmpvar_7;
			 tmpvar_7 = (1.0 - _OuterShadowBlur);
			 float tmpvar_8;
			 tmpvar_8 = (1.0 - _InnerShadowBlur);
			 float2 tmpvar_9;
			 tmpvar_9 = pow(abs(i.xlv_TEXCOORD1), (_Roundness));
			 float2 tmpvar_10;
			 tmpvar_10 = pow(abs((i.xlv_TEXCOORD1 + _ShadowVec)), (_Roundness));
			 float2 tmpvar_11;
			 tmpvar_11 = pow(abs((i.xlv_TEXCOORD1 + _ShadowInnerVec)), (_Roundness));
			 float tmpvar_12;
			 tmpvar_12 = pow((1.0 - _BorderSize), _Roundness);
			 float tmpvar_13;
			 tmpvar_13 = (tmpvar_12 - _AAliasSize);
			 float tmpvar_14;
			 tmpvar_14 = sqrt(dot(tmpvar_9, tmpvar_9));
			 float tmpvar_15;
			 tmpvar_15 = (1.0 - ((clamp(tmpvar_14, tmpvar_6, 1.0) - tmpvar_6) * tmpvar_5));
			 float4 tmpvar_16;
			 tmpvar_16 = tex2D(_MainTex, i.xlv_TEXCOORD0);
			 image_4 = tmpvar_16;
			 float2 tmpvar_17;
			 tmpvar_17 = abs(((i.xlv_TEXCOORD0 - 0.5) * 2.0));
			 float tmpvar_18;
			 tmpvar_18 = max(tmpvar_17.x, tmpvar_17.y);
			 float4 tmpvar_19;
			 tmpvar_19 = tex2D(_EffectTex, i.xlv_TEXCOORD2);
			 fx1_3 = tmpvar_19;
			 float4 tmpvar_20;
			 tmpvar_20 = tex2D(_EffectTex, i.xlv_TEXCOORD3);
			 fx2_2 = tmpvar_20;
			 fx1_3.w = (fx1_3.w * _EffectAlpha);
			 fx2_2.w = (fx2_2.w * (_EffectAlpha * 0.7));
			 xlat_mutable_Centre.xyz = lerp(_Centre.xyz, fx1_3.xyz, fx1_3.www);
			 xlat_mutable_Centre.xyz = lerp(xlat_mutable_Centre.xyz, fx2_2.xyz, fx2_2.www);
			 float tmpvar_21;
			 tmpvar_21 = (_OuterShading * clamp((1.0 - ((clamp(sqrt(dot(tmpvar_10, tmpvar_10)), tmpvar_7, 1.0) - tmpvar_7) * (1.0 / (_OuterShadowBlur)))), 0.0, 1.0));
			 fragment_1 = lerp((_Border * min((tmpvar_15 * 1000.0), 1.0)), (lerp(xlat_mutable_Centre, image_4, (max((image_4.w - ((float((tmpvar_18 >= 1.0)) * _TileAttenuation) * tmpvar_18)), 0.0))) * lerp((1.0 - _InnerShading), 1.0, clamp((1.0 - ((clamp(sqrt(dot(tmpvar_11, tmpvar_11)), tmpvar_8, tmpvar_12) - tmpvar_8) * (1.0 / (_InnerShadowBlur)))), 0.0, 1.0))), ((1.0 - ((clamp(tmpvar_14, tmpvar_13, tmpvar_12) - tmpvar_13) * tmpvar_5))));
			 float tmpvar_22;
			 if ((tmpvar_15 > 0.0001))
			 {
			   tmpvar_22 = tmpvar_15;
			 }
			 else
			 {
			   tmpvar_22 = tmpvar_21;
			 }
			 fragment_1.w = tmpvar_22;
			 o.gl_FragData = fragment_1 * i.xlv_COLOR;
			 return o;
		  }

		  ENDCG
		}
	}

	FallBack Off
}
