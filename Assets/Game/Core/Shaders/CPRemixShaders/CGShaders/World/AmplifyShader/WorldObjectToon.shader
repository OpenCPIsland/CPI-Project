// Made with Amplify Shader Editor v1.9.8.0
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "WorldObjectToon"
{
	Properties
	{
		_unity_Lightmap("unity_Lightmap", 2D) = "white" {}
		_Diffuse("Diffuse Texture", 2D) = "white" {}
		_BlobShadowTex("Blob Shadow Tex", 2D) = "white" {}
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv2_texcoord2;
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _unity_Lightmap;
		uniform sampler2D _Diffuse;
		uniform float4 _Diffuse_ST;
		uniform sampler2D _BlobShadowTex;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 temp_output_7_0 = (i.uv2_texcoord2*(unity_LightmapST).xy + (unity_LightmapST).zw);
			float2 uv_Diffuse = i.uv_texcoord * _Diffuse_ST.xy + _Diffuse_ST.zw;
			float2 uv_TexCoord10 = i.uv_texcoord + float2( 0,0 );
			o.Albedo = ( ( ( tex2D( _unity_Lightmap, temp_output_7_0 ) * tex2D( _Diffuse, uv_Diffuse ) ) * i.vertexColor ) * tex2D( _BlobShadowTex, uv_TexCoord10 ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19503
Node;AmplifyShaderEditor.FunctionNode;7;-1008,-96;Inherit;False;Lightmap UV;-1;;1;1940f027d0458684eb0ad486f669d7d5;1,1,0;0;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;4;-448,-208;Inherit;True;Property;_unity_Lightmap;unity_Lightmap;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.Vector2Node;11;-896,496;Inherit;False;Constant;_ShadowPlaneWorldPos;_ShadowPlaneWorldPos;3;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;5;-448,16;Inherit;True;Property;_Diffuse;Diffuse Texture;1;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-32,-32;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;8;-240,272;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;10;-528,528;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;80,128;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;9;-176,544;Inherit;True;Property;_BlobShadowTex;Blob Shadow Tex;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1;208,304;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BakedGINode;6;-654.8367,-374.4798;Inherit;False;True;5;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT4;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;368,-16;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;WorldObjectToon;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;4;1;7;0
WireConnection;3;0;4;0
WireConnection;3;1;5;0
WireConnection;10;1;11;0
WireConnection;2;0;3;0
WireConnection;2;1;8;0
WireConnection;9;1;10;0
WireConnection;1;0;2;0
WireConnection;1;1;9;0
WireConnection;6;2;7;0
WireConnection;6;3;7;0
WireConnection;0;0;1;0
ASEEND*/
//CHKSM=09950742685221F5562D67947732600DCAE51EDC