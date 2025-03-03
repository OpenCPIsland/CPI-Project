Shader "Cg shader for toon shading" {
	Properties {
		_Color ("Diffuse Color", Vector) = (1,1,1,1)
		_UnlitColor ("Unlit Diffuse Color", Vector) = (0.5,0.5,0.5,1)
		_DiffuseThreshold ("Threshold for Diffuse Colors", Range(0, 1)) = 0.1
		_OutlineColor ("Outline Color", Vector) = (0,0,0,1)
		_LitOutlineThickness ("Lit Outline Thickness", Range(0, 1)) = 0.1
		_UnlitOutlineThickness ("Unlit Outline Thickness", Range(0, 1)) = 0.4
		_SpecColor ("Specular Color", Vector) = (1,1,1,1)
		_Shininess ("Shininess", Float) = 10
		
		_MainTex ("Texture", 2D) = "white" {}
	}
SubShader {
Tags { "LIGHTMODE" = "FORWARDBASE" }
 
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
 Fallback "Specular"
 }