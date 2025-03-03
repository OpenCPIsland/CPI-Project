Shader "CpRemix/UI/TextSlow"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_StencilComp("Stencil Comparison", float) = 8
		_Stencil("Stencil ID", float) = 0
		_StencilOp("Stencil Operation", float) = 0
		_StencilWriteMask("Stencil Write Mask", float) = 255
		_StencilReadMask("Stencil Read Mask", float) = 255
		_ColorMask("Color Mask", float) = 15
		[KeywordEnum(Off, Sine)] _OscMode("Oscillation mode", float) = 0
		_Oscillation("X speed, X amplitude, Y speed, Y amplitude", Vector) = (1,0.01,1,0.01)
		_TextureSampleAdd("Texture Sample Add", Vector) = (0,0,0,0)
	}
	SubShader
	{
		Tags
		{
			"CanUseSpriteAtlas" = "true"
			"IGNOREPROJECTOR" = "true"
			"PreviewType" = "Plane"
			"QUEUE" = "Transparent"
			"RenderType" = "Transparent"
		}

		Pass
		{
			Name "DEFAULT"
			Tags
			{
				"CanUseSpriteAtlas" = "true"
				"IGNOREPROJECTOR" = "true"
				"PreviewType" = "Plane"
				"QUEUE" = "Transparent"
				"RenderType" = "Transparent"
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
			#pragma multi_compile _OSCMODE_OFF
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			float4 _Color;
			float4 _TextureSampleAdd;
			sampler2D _MainTex;

			struct v2f
			{
				float4 xlv_COLOR : COLOR;
				float2 xlv_TEXCOORD0 : TEXCOORD0;
			};

			struct FragOutput
			{
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
				float2 tmpvar_1 = _glesMultiTexCoord0.xy;
				float4 tmpvar_2 = (_glesColor * _Color);

				gl_Position = UnityObjectToClipPos(_glesVertex);
				o.xlv_COLOR = tmpvar_2;
				o.xlv_TEXCOORD0 = tmpvar_1;

				return o;
			}

			FragOutput frag(v2f i)
			{
				FragOutput o;
				float4 texColor = tex2D(_MainTex, i.xlv_TEXCOORD0);
				o.gl_FragData = ((texColor + _TextureSampleAdd) * i.xlv_COLOR);
				return o;
			}

			ENDCG
		}
	}

	FallBack "UI/Default"
}
