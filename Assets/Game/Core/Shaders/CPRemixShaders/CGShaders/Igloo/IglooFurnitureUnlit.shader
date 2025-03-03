Shader "CpRemix/Igloo/IglooFurnitureUnlit"
{
	Properties
	{
	  _Color("Tint Color", Color) = (1,1,1,1)
	  _MainTex("Texture (RGB)", 2D) = "white" {}
	  _Highlight("Additional Highlight", Range(0, 1)) = 0
	}
		SubShader
	  {
		Tags
		{
		  "LIGHTMODE" = "FORWARDBASE"
		  "QUEUE" = "Geometry"
		  "RenderType" = "Opaque"
		}
		Pass
		{
		  Tags
		  {
			"LIGHTMODE" = "FORWARDBASE"
			"QUEUE" = "Geometry"
			"RenderType" = "Opaque"
		  }
			  CGPROGRAM


			  #pragma vertex vert
			  #pragma fragment frag

			  #include "UnityCG.cginc"

				float4 _Color;
				float _Highlight;
				sampler2D _MainTex;

			  struct v2f
			  {
			  float4 color : COLOR;
			  float4 uv : TEXCOORD0;
			  };

			  struct FragOutput
			  {
			  fixed4 color : SV_Target;
			  };

			  v2f vert(
			  float4 vertex : POSITION,
			  float4 uv : TEXCOORD0,
			  out float4 outpos : SV_POSITION
			  )
			  {
				v2f o;
				o.color = _Color + _Highlight;
				o.uv = uv;
				outpos = UnityObjectToClipPos(vertex);
				return o;
			  }


			  FragOutput frag(v2f i)
			  {
				FragOutput o;
				o.color = tex2D(_MainTex, i.uv) * i.color;
				return o;
			  }

		  ENDCG
		}
	  }
		  FallBack "VertexLit"
}
