Shader "CpRemix/World/WorldObject Depth"
{
	Properties
	{
	  _Diffuse("Diffuse Texture", 2D) = "" {}
	}
		SubShader
	{
	  Tags
	  {
	  }
	  Pass // ind: 1, name: 
	  {
		Tags
		{
		  "LIGHTMODE" = "FORWARDBASE"
		}
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog

			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			#include "Lighting.cginc"

		//float4 _Time;
	   // float4x4 unity_ObjectToWorld;
		//float4x4 unity_WorldToObject;
		//float4x4 unity_MatrixVP;
		//float4 unity_LightmapST;
		float _SurfaceYCoord;
		float _DeepestYCoord;
		float3 _DepthColor;
		float3 _SurfaceReflectionColor;
		float _SurfaceTexTile;
		float _SurfaceMultiplier;
		float _SurfaceVelocityX;
		float _SurfaceVelocityZ;
		//sampler2D unity_Lightmap;
		//float4 unity_Lightmap_HDR;
		sampler2D _Diffuse;
		sampler2D _SurfaceReflectionsRGB;

		struct v2f
		{
		float3 xlv_COLOR : COLOR;
		float2 xlv_TEXCOORD0 : TEXCOORD0;
		float2 xlv_TEXCOORD1 : TEXCOORD1;
		float3 xlv_TEXCOORD3 : TEXCOORD3;
		float3 xlv_TEXCOORD4 : TEXCOORD4;
		float2 xlv_TEXCOORD5 : TEXCOORD5;
		UNITY_FOG_COORDS(2)
		};

		struct FragOutput
		{
		float4 FragData : SV_Target;
		};

		v2f vert(
		float4 _glesVertex : POSITION,
		float4 _glesColor : COLOR,
		float3 _glesNormal : NORMAL,
		float4 _glesMultiTexCoord0 : TEXCOORD0,
		float4 _glesMultiTexCoord1 : TEXCOORD1,
		out float4 Position : SV_POSITION
		)
		{
		  v2f o;
		  float4 tmpvar_1;
		  tmpvar_1 = _glesColor;
		  float isBelowSurface_2;
		  float depthDeltaNormalized_3;
		  float3 worldSpaceNormalNormalized_4;
		  float3 tmpvar_5;
		  float2 tmpvar_6;
		  float2 tmpvar_7;
		  float4 tmpvar_8;
		  tmpvar_8.w = 1.0;
		  tmpvar_8.xyz = _glesVertex.xyz;
		  tmpvar_5 = tmpvar_1.xyz;
		  tmpvar_6 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
		  float4 v_9;
		  v_9.x = unity_WorldToObject[0].x;
		  v_9.y = unity_WorldToObject[1].x;
		  v_9.z = unity_WorldToObject[2].x;
		  v_9.w = unity_WorldToObject[3].x;
		  float4 v_10;
		  v_10.x = unity_WorldToObject[0].y;
		  v_10.y = unity_WorldToObject[1].y;
		  v_10.z = unity_WorldToObject[2].y;
		  v_10.w = unity_WorldToObject[3].y;
		  float4 v_11;
		  v_11.x = unity_WorldToObject[0].z;
		  v_11.y = unity_WorldToObject[1].z;
		  v_11.z = unity_WorldToObject[2].z;
		  v_11.w = unity_WorldToObject[3].z;
		  float3 tmpvar_12;
		  tmpvar_12 = normalize(((
			(v_9.xyz * _glesNormal.x)
		   +
			(v_10.xyz * _glesNormal.y)
		  ) + (v_11.xyz * _glesNormal.z)));
		  worldSpaceNormalNormalized_4 = tmpvar_12;
		  float3 tmpvar_13;
		  tmpvar_13 = mul(unity_ObjectToWorld, _glesVertex).xyz;
		  float tmpvar_14;
		  tmpvar_14 = (1.0 - clamp((
			(tmpvar_13.y - _DeepestYCoord)
		   /
			(_SurfaceYCoord - _DeepestYCoord)
		  ), 0.0, 1.0));
		  depthDeltaNormalized_3 = tmpvar_14;
		  float tmpvar_15;
		  tmpvar_15 = (_SurfaceYCoord - tmpvar_13.y);
		  isBelowSurface_2 = tmpvar_15;
		  isBelowSurface_2 = (isBelowSurface_2 * float((isBelowSurface_2 > 0.0)));
		  float tmpvar_16;
		  tmpvar_16 = min(1.0, isBelowSurface_2);
		  isBelowSurface_2 = tmpvar_16;
		  float2 tmpvar_17;
		  tmpvar_17.x = (_SurfaceVelocityX * _SurfaceTexTile);
		  tmpvar_17.y = (_SurfaceVelocityZ * _SurfaceTexTile);
		  tmpvar_7 = ((tmpvar_13.xz * _SurfaceTexTile) - (_Time.xx * tmpvar_17));
		  Position = UnityObjectToClipPos(tmpvar_8);//mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_8));
		  o.xlv_COLOR = tmpvar_5;
		  o.xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
		  o.xlv_TEXCOORD1 = tmpvar_6;
		  o.xlv_TEXCOORD3 = ((_DepthColor * depthDeltaNormalized_3) + float((1.0 - depthDeltaNormalized_3)));
		  o.xlv_TEXCOORD4 = (_SurfaceReflectionColor * ((
			((((worldSpaceNormalNormalized_4.y * worldSpaceNormalNormalized_4.y) * float(
			  (worldSpaceNormalNormalized_4.y > 0.0)
			))* tmpvar_16)* (1.0 - depthDeltaNormalized_3))
		   * 0.5)* _SurfaceMultiplier));
		  o.xlv_TEXCOORD5 = tmpvar_7;
		  UNITY_TRANSFER_FOG(o,Position);
		  return o;
		}


		FragOutput frag(v2f i)
		{
		  FragOutput o;
		  float3 outputColor_1;
		  float3 diffuseSample_2;
		  float3 tmpvar_3;
		  tmpvar_3 = tex2D(_Diffuse, i.xlv_TEXCOORD0).xyz;
		  diffuseSample_2 = tmpvar_3;
		  float3 tmpvar_4;
		  tmpvar_4 = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.xlv_TEXCOORD1));
		  float3 color_5;
		  color_5 = tmpvar_4;
		  outputColor_1 = ((diffuseSample_2 * unity_Lightmap_HDR.x) * ((color_5.xyz * i.xlv_COLOR) * i.xlv_TEXCOORD3));
		  float4 tmpvar_6;
		  tmpvar_6 = tex2D(_SurfaceReflectionsRGB, i.xlv_TEXCOORD5);
		  outputColor_1 = (outputColor_1 + (tmpvar_6.x * i.xlv_TEXCOORD4));
		  float4 tmpvar_7;
		  tmpvar_7.w = 1.0;
		  tmpvar_7.xyz = outputColor_1;
		  UNITY_APPLY_FOG(i.fogCoord, tmpvar_7);
		  UNITY_OPAQUE_ALPHA(tmpvar_7.w);
		  o.FragData = tmpvar_7;
		  return o;
		}


		ENDCG

  } // end phase
	}
		FallBack Off
}

/*Shader "CpRemix/World/WorldObject Depth" {
	Properties {
		_Diffuse ("Diffuse Texture", 2D) = "" {}
	}
	SubShader {
		Pass {
			Tags { "LIGHTMODE" = "FORWARDBASE" }
			GpuProgramID 62689
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float3 color : COLOR0;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float3 texcoord3 : TEXCOORD3;
				float3 texcoord4 : TEXCOORD4;
				float2 texcoord5 : TEXCOORD5;
				UNITY_FOG_COORDS(2)
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			float _SurfaceYCoord;
			float _DeepestYCoord;
			float3 _DepthColor;
			float3 _SurfaceReflectionColor;
			float _SurfaceTexTile;
			float _SurfaceMultiplier;
			float _SurfaceVelocityX;
			float _SurfaceVelocityZ;
			// $Globals ConstantBuffers for Fragment Shader
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _Diffuse;
			sampler2D _SurfaceReflectionsRGB;
			
			// Keywords: 
			v2f vert(appdata_full v) {
    v2f o;

    // Use Unity's built-in matrix multiplication for object-to-world transformation
    float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
    
    // Use Unity's built-in function for position transformation
    o.position = UnityObjectToClipPos(v.vertex);

    // Copy vertex colors and texture coordinates
    o.color.xyz = v.color.xyz;
    o.texcoord1.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    o.texcoord.xy = v.texcoord.xy;

    // Depth-related calculations (surface and deepest Y coordinates)
    float depthFactor = saturate((worldPos.y - _DeepestYCoord) / (_SurfaceYCoord - _DeepestYCoord));
    depthFactor = 1.0 - depthFactor;
    o.texcoord3.xyz = _DepthColor * depthFactor + (1.0 - depthFactor);

    // Normal transformation using Unity's built-in matrix
    float3 worldNormal = mul((float3x3)unity_ObjectToWorld, v.normal);
    worldNormal = normalize(worldNormal);

    // Surface reflection calculation based on depth
    float surfaceReflectionFactor = max(0.0, (worldPos.y - _SurfaceYCoord)) * _SurfaceMultiplier * 0.5;
    o.texcoord4.xyz = surfaceReflectionFactor * _SurfaceReflectionColor;

    // Surface texture tiling and movement based on time
    float2 surfaceMovement = _SurfaceTexTile.xx * float2(_SurfaceVelocityX.x, _SurfaceVelocityZ.x) * _Time.x;
    o.texcoord5.xy = v.texcoord.xy * _SurfaceTexTile.xx - surfaceMovement;

    // Handle fog
    UNITY_TRANSFER_FOG(o, o.position);

    return o;
}

			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                float4 tmp1;
                tmp0 = UNITY_SAMPLE_TEX2D_SAMPLER(unity_Lightmap, unity_Lightmap, inp.texcoord1.xy);
                tmp0.w = tmp0.w * unity_Lightmap_HDR.x;
                tmp0.xyz = tmp0.xyz * tmp0.www;
                tmp1 = tex2D(_Diffuse, inp.texcoord.xy);
                tmp0.xyz = tmp0.xyz * tmp1.xyz;
                tmp0.xyz = tmp0.xyz * inp.color.xyz;
                tmp1 = tex2D(_SurfaceReflectionsRGB, inp.texcoord5.xy);
                tmp1.xyz = tmp1.xxx * inp.texcoord4.xyz;
                o.sv_target.xyz = tmp0.xyz * inp.texcoord3.xyz + tmp1.xyz;
                o.sv_target.w = 1.0;
				UNITY_APPLY_FOG(inp.fogCoord, o.sv_target);
				UNITY_OPAQUE_ALPHA(o.sv_target.w);
                return o;
			}
			ENDCG
		}
	}
}*/