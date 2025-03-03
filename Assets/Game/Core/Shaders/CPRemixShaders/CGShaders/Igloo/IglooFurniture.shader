Shader "CpRemix/Igloo/IglooFurniture"
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
		Pass // ind: 1, name: 
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
			  #include "AutoLight.cginc"
			  #include "Lighting.cginc"


		  //float4 _WorldSpaceLightPos0;
		  //float4 unity_SHAr;
		  //float4 unity_SHAg;
		  //float4 unity_SHAb;
		  //float4 unity_SHBr;
		  //float4 unity_SHBg;
		 // float4 unity_SHBb;
		 // float4 unity_SHC;
		 // float4x4 unity_ObjectToWorld;
		 // float4x4 unity_WorldToObject;
		//  float4x4 unity_MatrixVP;
		  //float4 _LightColor0;
			float4 _Color;
			float _Highlight;
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
		  float3 _glesNormal : NORMAL,
		  float4 _glesMultiTexCoord0 : TEXCOORD0,
		  out float4 gl_Position : SV_POSITION
		  )
		  {
			v2f o;
			float3 tmpvar_1;
			tmpvar_1 = _glesNormal;
			float3 normalWorldSpace_2;
			float4 tmpvar_3;
			float3 norm_4;
			norm_4 = tmpvar_1;
			float3x3 tmpvar_5;
			tmpvar_5[0] = unity_WorldToObject[0].xyz;
			tmpvar_5[1] = unity_WorldToObject[1].xyz;
			tmpvar_5[2] = unity_WorldToObject[2].xyz;
			float3 tmpvar_6;
			tmpvar_6 = normalize(mul(norm_4, tmpvar_5));
			normalWorldSpace_2 = tmpvar_6;
			float4 tmpvar_7;
			tmpvar_7.w = 1.0;
			tmpvar_7.xyz = _glesVertex.xyz;
			float4 tmpvar_8;
			tmpvar_8.w = 1.0;
			tmpvar_8.xyz = normalWorldSpace_2;
			float3 res_9;
			float3 x_10;
			x_10.x = dot(unity_SHAr, tmpvar_8);
			x_10.y = dot(unity_SHAg, tmpvar_8);
			x_10.z = dot(unity_SHAb, tmpvar_8);
			float3 x1_11;
			float4 tmpvar_12;
			tmpvar_12 = (normalWorldSpace_2.xyzz * normalWorldSpace_2.yzzx);
			x1_11.x = dot(unity_SHBr, tmpvar_12);
			x1_11.y = dot(unity_SHBg, tmpvar_12);
			x1_11.z = dot(unity_SHBb, tmpvar_12);
			res_9 = (x_10 + (x1_11 + (unity_SHC.xyz *
			  ((normalWorldSpace_2.x * normalWorldSpace_2.x) - (normalWorldSpace_2.y * normalWorldSpace_2.y))
			)));
			float3 tmpvar_13;
			tmpvar_13 = max(((1.055 *
			  pow(max(res_9, float3(0.0, 0.0, 0.0)), float3(0.4166667, 0.4166667, 0.4166667))
			) - 0.055), float3(0.0, 0.0, 0.0));
			res_9 = tmpvar_13;
			float4 tmpvar_14;
			tmpvar_14.w = 1.0;
			tmpvar_14.xyz = max(float3(0.0, 0.0, 0.0), tmpvar_13);
			float3 tmpvar_15;
			float4 tmpvar_16;
			tmpvar_16.w = 0.0;
			tmpvar_16.xyz = normalWorldSpace_2;
			float tmpvar_17;
			tmpvar_17 = clamp(((
			  dot(tmpvar_16, _WorldSpaceLightPos0)
			 + 1.0) / 4.0), 0.0, 1.0);
			float3 tmpvar_18;
			tmpvar_18 = (_LightColor0 * tmpvar_17).xyz;
			tmpvar_15 = tmpvar_18;
			float4 tmpvar_19;
			tmpvar_19.w = 1.0;
			tmpvar_19.xyz = tmpvar_15;
			tmpvar_3 = (tmpvar_14 + tmpvar_19);
			tmpvar_3 = (tmpvar_3 * _Color);
			tmpvar_3 = (tmpvar_3 + _Highlight);
			gl_Position = UnityObjectToClipPos(tmpvar_7); //mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_7));
			o.xlv_COLOR = tmpvar_3;
			o.xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
			return o;
		  }


		  FragOutput frag(v2f i)
		  {
			FragOutput o;
			float4 tmpvar_1;
			float4 tmpvar_2;
			tmpvar_2 = tex2D(_MainTex, i.xlv_TEXCOORD0);
			tmpvar_1 = tmpvar_2;
			o.gl_FragData = ((i.xlv_COLOR * float4(0.9, 0.9, 0.9, 0.9)) * tmpvar_1);
			return o;
		  }


		  ENDCG

	} // end phase
	  }
		  FallBack "VertexLit"
}
