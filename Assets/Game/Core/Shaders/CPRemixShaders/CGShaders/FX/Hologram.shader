Shader "FX/Hologram Effect" {
	Properties{
		_TintColor("Tint Color", Color) = (0,0.5,1,1)
		_RimColor("Rim Color", Color) = (0,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}
		_GlitchTime("Glitches Over Time", Range(0.01,3.0)) = 1.0
		_WorldScale("Line Amount", Range(1,200)) = 20
	}

		Category{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Sphere" }
		Blend SrcAlpha OneMinusSrcAlpha 
		ColorMask RGB
		Cull Back 
		SubShader{
		Pass{

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 2.0


#include "UnityCG.cginc"

	sampler2D _MainTex;
	fixed4 _TintColor;
	fixed4 _RimColor;


	struct appdata_t {
		float4 vertex : POSITION;
		fixed4 color : COLOR;

		float2 texcoord : TEXCOORD0;
		float3 normal : NORMAL; // vertex normal
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f {
		float4 vertex : SV_POSITION;
		fixed4 color : COLOR;
		float2 texcoord : TEXCOORD0;
		float3 wpos : TEXCOORD1; // worldposition
		float3 normalDir : TEXCOORD2; // normal direction for rimlighting

	};

	float4 _MainTex_ST;
	float _GlitchTime;
	float _WorldScale;
	float _OptTime = 0;

	v2f vert(appdata_t v)
	{
		v2f o;

		o.vertex = UnityObjectToClipPos(v.vertex);

		// Vertex glitching
		_OptTime = _OptTime == 0 ? sin(_Time.w * _GlitchTime) : _OptTime;// optimisation
		float glitchtime = step(0.99, _OptTime); // returns 1 when sine is near top, otherwise returns 0;
		float glitchPos = v.vertex.y + _SinTime.y;// position on model
		float glitchPosClamped = step(0, glitchPos) * step(glitchPos, 0.2);// clamped segment of model
		o.vertex.xz += glitchPosClamped * 0.1 * glitchtime * _SinTime.y;// moving the vertices when glitchtime returns 1;


		o.color = v.color;
		o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);

		// world position and normal direction
		o.wpos = mul(unity_ObjectToWorld, v.vertex).xyz;
		o.normalDir = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);

		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		float4 text = tex2D(_MainTex, i.texcoord) * _TintColor;// texture

		// rim lighting
		float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.wpos.xyz);
		half rim = 1.0 - saturate(dot(viewDirection, i.normalDir));// rimlight based on view and normal

		// small scanlines down
		float fraclines = frac((i.wpos.y * _WorldScale) + _Time.y);//small lines 
		float scanlines = step(fraclines, 0.5);// cut off based on 0.5
		// big scanline up
		float bigfracline = frac((i.wpos.y ) - _Time.x * 4);// big gradient line

		fixed4 col = text + (bigfracline * 0.4 * _TintColor) +(rim * _RimColor);// end result color 
	
		col.a = 0.8 * (scanlines + rim + bigfracline);// alpha based on scanlines and rim
	
		return col;
		}
		ENDCG
	}
	}
	}
}