// Made with Amplify Shader Editor v1.9.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/Simple Potion Liquid"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Speed("Speed", Float) = 0
		_Size("Size", Float) = 0.1
		_Height("Height", Float) = 0
		_Falloff("Falloff", Float) = 0.02
		_Opacity("Opacity", Float) = 0.76
		_Color("Color", Color) = (1,0,0,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Pass
		{
			ColorMask 0
			ZWrite On
		}

		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+1" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Front
		ZWrite On
		ZTest Always
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf StandardCustomLighting keepalpha 
		struct Input
		{
			float3 worldPos;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float4 _Color;
		uniform float _Speed;
		uniform float _Size;
		uniform float _Height;
		uniform float _Falloff;
		uniform float _Opacity;
		uniform float _Cutoff = 0.5;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			float4 transform7 = mul(unity_ObjectToWorld,float4( 0,0,0,1 ));
			float3 ase_worldPos = i.worldPos;
			float4 temp_output_5_0 = ( transform7 - float4( ase_worldPos , 0.0 ) );
			float mulTime2 = _Time.y * _Speed;
			float4 temp_cast_4 = (( 1.0 - _Height )).xxxx;
			float4 clampResult18 = clamp( ( ( ( ( temp_output_5_0 * sin( mulTime2 ) * _Size ) + (temp_output_5_0).y ) - temp_cast_4 ) / _Falloff ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
			float4 temp_output_21_0 = ( clampResult18 * _Opacity );
			c.rgb = 0;
			c.a = 1;
			clip( temp_output_21_0.x - _Cutoff );
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			float4 temp_output_22_0 = _Color;
			o.Albedo = temp_output_22_0.rgb;
			o.Emission = temp_output_22_0.rgb;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19200
Node;AmplifyShaderEditor.RangedFloatNode;1;-1061.122,-52.18835;Float;False;Property;_Speed;Speed;1;0;Create;True;0;0;0;False;0;False;0;7.88;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ObjectToWorldTransfNode;7;-1144.93,49.36661;Inherit;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldPosInputsNode;8;-1009.122,236.8116;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleTimeNode;2;-877,-46;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;3;-692.1223,-45.18835;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-777.4153,46.94485;Float;False;Property;_Size;Size;2;0;Create;True;0;0;0;False;0;False;0.1;0.31;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;5;-756.1223,189.8116;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-289.2947,357.9797;Float;False;Property;_Height;Height;3;0;Create;True;0;0;0;False;0;False;0;1.09;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-536.1223,-37.18835;Inherit;True;3;3;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ComponentMaskNode;10;-480.0292,226.2977;Inherit;False;False;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;12;-116.7756,290.3243;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;9;-238.1219,31.81161;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;11;51.78445,172.7243;Inherit;True;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;16;221.9577,414.4579;Float;False;Property;_Falloff;Falloff;4;0;Create;True;0;0;0;False;0;False;0.02;0.03;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;14;403.2778,188.4044;Inherit;True;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ClampOpNode;18;627.8206,189.7693;Inherit;True;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;1,1,1,1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1784.594,-164.4324;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;ASESampleShaders/Simple Potion Liquid;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Front;1;False;;7;False;;False;0.6;False;;2.65;False;;True;0;Custom;0.5;True;False;1;True;Transparent;;Transparent;All;0;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.ColorNode;22;1314.2,-259.9293;Float;False;Property;_Color;Color;6;0;Create;True;0;0;0;False;0;False;1,0,0,0;1,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;20;1183.922,325.4955;Float;False;Property;_Opacity;Opacity;5;0;Create;True;0;0;0;False;0;False;0.76;0.95;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;1332.177,162.2935;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
WireConnection;2;0;1;0
WireConnection;3;0;2;0
WireConnection;5;0;7;0
WireConnection;5;1;8;0
WireConnection;4;0;5;0
WireConnection;4;1;3;0
WireConnection;4;2;6;0
WireConnection;10;0;5;0
WireConnection;12;0;13;0
WireConnection;9;0;4;0
WireConnection;9;1;10;0
WireConnection;11;0;9;0
WireConnection;11;1;12;0
WireConnection;14;0;11;0
WireConnection;14;1;16;0
WireConnection;18;0;14;0
WireConnection;0;0;22;0
WireConnection;0;2;22;0
WireConnection;0;10;21;0
WireConnection;21;0;18;0
WireConnection;21;1;20;0
ASEEND*/
//CHKSM=E85B47E4D3CB2261836102CD3DB221806F1417AF