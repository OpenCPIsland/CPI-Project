Shader "Custom/OutlineSilouetted Diffuse" {
	Properties {
		_Color ("Color", Vector) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0, 1)) = 0.5
		_Metallic ("Metallic", Range(0, 1)) = 0
	}
SubShader {
			Name "FORWARD"
			LOD 200
			Tags { "LIGHTMODE" = "FORWARDBASE" "RenderType" = "Opaque" "SHADOWSUPPORT" = "true" }
 
 CGPROGRAM
 #pragma surface surf Lambert
 
 sampler2D _MainTex;
 fixed4 _Color;
 
 struct Input {
     float2 uv_MainTex;
     float4 color : COLOR;
 };
 
 void surf (Input IN, inout SurfaceOutput o) {
     fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
     o.Albedo = c.rgb;
     o.Albedo *= IN.color.rgb;
     o.Alpha = c.a;
 }
 ENDCG
 }
 Fallback "Diffuse"
 }