Shader "CpRemix/UI/MinimalGrey"
{
	Properties
	{
	  [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
	  _Brightness("Brightness", Range(0, 1)) = 0
	  _GreyScaleEffect("Saturation Amount", Range(0, 1)) = 1
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
			"QUEUE" = "Transparent"
		}
		Pass // ind: 1, name: 
		{
			Tags
			{
				"QUEUE" = "Transparent"
			}
			ZTest [unity_GUIZTestMode]
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float _GreyScaleEffect;
			float _Brightness;

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
				float4 vertexPos = _glesVertex;
				vertexPos.w = 1.0;
				gl_Position = UnityObjectToClipPos(vertexPos); // Perform transformation

				o.xlv_COLOR = _glesColor;
				o.xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;

				return o;
			}

			FragOutput frag(v2f i)
			{
				FragOutput o;
				float4 texcol = tex2D(_MainTex, i.xlv_TEXCOORD0); // Sample texture
				float grey = ((texcol.x + texcol.y + texcol.z) * 0.33) * (1.0 + (_Brightness * 2.0)); // Calculate grayscale
				texcol.xyz = lerp(texcol.xyz, float3(grey, grey, grey), _GreyScaleEffect); // Apply greyscale effect based on _GreyScaleEffect value
				o.gl_FragData = texcol * i.xlv_COLOR; // Output final color
				return o;
			}

			ENDCG
		}
	}
	FallBack "UI/Default"
}
