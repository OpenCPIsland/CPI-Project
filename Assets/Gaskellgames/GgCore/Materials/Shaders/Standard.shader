// Made with Amplify Shader Editor v1.9.7.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Gaskellgames/Basic/Standard"
{
	Properties
	{
		[Header(Surface Inputs)][Space][Space]_BaseColor("Base Color", Color) = (1,1,1,0)
		[SingleLineTexture]_MainTex("Albedo", 2D) = "white" {}
		[Normal][SingleLineTexture]_BumpMap("Normal Map", 2D) = "bump" {}
		[SingleLineTexture]_MaskMap("Mask (MSAoA)", 2D) = "white" {}
		_MetallicGloss("Metallic Gloss", Range( 0 , 1)) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.3
		_AmbientOcclusion("Ambient Occlusion", Range( 0 , 1)) = 1
		[HDR][Header(Emission)][Space][Space]_EmissiveColor("Emissive Color", Color) = (0,0,0,0)
		[HDR][SingleLineTexture]_EmissiveColorMap("Emissive", 2D) = "white" {}
		_EmissiveIntensity("Emissive Intensity", Range( 0 , 1)) = 0
		[Header(Tiling and Offset)][Space][Space][Toggle(_SCALEINDEPENDENTUV_ON)] _ScaleIndependentUV("Scale Independent UV", Float) = 1
		_TilingX("Tiling X", Float) = 1
		_TilingY("Tiling Y", Float) = 1
		_OffsetX("Offset X", Float) = 0
		_OffsetY("Offset Y", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
		[Header(Forward Rendering Options)]
		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _GlossyReflections("Reflections", Float) = 1.0
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 5.0
		#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
		#pragma shader_feature _GLOSSYREFLECTIONS_OFF
		#pragma shader_feature_local _SCALEINDEPENDENTUV_ON
		#define ASE_VERSION 19701
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred dithercrossfade 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _BumpMap;
		uniform float _TilingX;
		uniform float _TilingY;
		uniform float _OffsetX;
		uniform float _OffsetY;
		uniform sampler2D _MainTex;
		uniform half4 _BaseColor;
		uniform half4 _EmissiveColor;
		uniform half _EmissiveIntensity;
		uniform sampler2D _EmissiveColorMap;
		uniform half _MetallicGloss;
		uniform sampler2D _MaskMap;
		uniform half _Smoothness;
		uniform half _AmbientOcclusion;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 appendResult283_g76 = (float2(_TilingX , _TilingY));
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			ase_vertexNormal = normalize( ase_vertexNormal );
			float3 normalizeResult2_g77 = normalize( ase_vertexNormal );
			float3 break3_g77 = normalizeResult2_g77;
			float3 ase_parentObjectScale = (1.0/float3( length( unity_WorldToObject[ 0 ].xyz ), length( unity_WorldToObject[ 1 ].xyz ), length( unity_WorldToObject[ 2 ].xyz ) ));
			float3 break3_g78 = ase_parentObjectScale;
			float2 appendResult6_g78 = (float2(break3_g78.z , break3_g78.y));
			float2 appendResult4_g78 = (float2(break3_g78.x , break3_g78.z));
			float2 appendResult5_g78 = (float2(break3_g78.x , break3_g78.y));
			float2 uv_TexCoord10_g77 = i.uv_texcoord * ( ( ( break3_g77.x * appendResult6_g78 ) + ( break3_g77.y * appendResult4_g78 ) + ( break3_g77.z * appendResult5_g78 ) ) * appendResult283_g76 );
			#ifdef _SCALEINDEPENDENTUV_ON
				float2 staticSwitch289_g76 = uv_TexCoord10_g77;
			#else
				float2 staticSwitch289_g76 = ( i.uv_texcoord * appendResult283_g76 );
			#endif
			float2 appendResult290_g76 = (float2(_OffsetX , _OffsetY));
			float2 UV206_g76 = ( staticSwitch289_g76 + appendResult290_g76 );
			o.Normal = UnpackNormal( tex2D( _BumpMap, UV206_g76 ) );
			float4 tex2DNode21_g76 = tex2D( _MainTex, UV206_g76 );
			float3 temp_output_51_0_g76 = ( (tex2DNode21_g76).rgb * (_BaseColor).rgb );
			o.Albedo = temp_output_51_0_g76;
			float3 temp_output_172_0_g76 = (_EmissiveColor).rgb;
			float3 temp_output_166_0_g76 = ( temp_output_172_0_g76 * _EmissiveIntensity );
			float3 BaseColor119_g76 = temp_output_51_0_g76;
			float3 lerpResult169_g76 = lerp( temp_output_166_0_g76 , BaseColor119_g76 , ( 1.0 - (tex2D( _EmissiveColorMap, UV206_g76 )).rgb ));
			o.Emission = lerpResult169_g76;
			float4 tex2DNode269_g76 = tex2D( _MaskMap, UV206_g76 );
			o.Metallic = ( _MetallicGloss * tex2DNode269_g76.r );
			o.Smoothness = ( tex2DNode269_g76.g * _Smoothness );
			o.Occlusion = ( saturate( tex2DNode269_g76.b ) * _AmbientOcclusion );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19701
Node;AmplifyShaderEditor.FunctionNode;109;-256,-128;Inherit;False;StandardMaterial;0;;76;94aeb3da542692a4688861de18cc1d5e;4,205,0,188,0,110,0,174,1;0;11;FLOAT3;204;FLOAT3;202;FLOAT3;201;FLOAT;197;FLOAT3;200;FLOAT;198;FLOAT;199;FLOAT;195;FLOAT;194;FLOAT;196;FLOAT3;203
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;21;0,-128;Float;False;True;-1;7;ASEMaterialInspector;0;0;Standard;Gaskellgames/Basic/Standard;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;True;True;True;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexScale;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;_Cull;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;21;0;109;204
WireConnection;21;1;109;202
WireConnection;21;2;109;201
WireConnection;21;3;109;197
WireConnection;21;4;109;198
WireConnection;21;5;109;199
ASEEND*/
//CHKSM=F974964F230761BBE730EF34DE018FDAA7173F80