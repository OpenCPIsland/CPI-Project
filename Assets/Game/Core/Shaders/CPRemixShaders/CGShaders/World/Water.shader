Shader "CpRemix/World/Water"
{
	Properties
	{
	  _Color("Water Color", Color) = (0,0.5,1,0.7)
	  _WavesMap("Waves r=shore g=diffuse b=spec", 2D) = "white" {}
	  _ShoreFoamBrightness("Shore Foam Brightness", Range(0, 2)) = 1
	  _ShoreTile("Shore Waves Tile", Range(0.05, 299)) = 1
	  _ShoreWavesColor("Shore Waves Color", Color) = (0,0,1,1)
	  _ShoreWavesTimeScale("Shore Time Scale", Range(0.05, 5)) = 1.2
	  _ShoreWavesOpacity("Shore Waves Opacity", Range(0.05, 1)) = 0.5
	  _ShoreWavesUVDirection("Shore Waves UV direction", Vector) = (0.5,0.5,0,0)
	  _ShoreTextureSampleAmnt("Shore Sample Amount", Range(0.05, 1)) = 0.5
	  _DiffuseWavesBounce("Diffuse Waves Bounce", Range(0, 0.1)) = 0.03
	  _DiffuseTile("Diffuse Waves Tile", Range(0.05, 299)) = 1
	  _DiffuseWavesColor("Diffuse Waves Color", Color) = (1,1,1,1)
	  _DiffuseWavesTimeScale("Diffuse Time Scale", Range(0.001, 5)) = 0.7
	  _DiffuseWavesOpacity("Diffuse Waves opacity", Range(0.05, 1)) = 0.5
	  _DiffuseWavesUVDirection("Diffuse Waves UV direction", Vector) = (1,0,0,0)
	  _SpecWavesBounce("Spec Waves Bounce", Range(0, 0.1)) = 0
	  _SpecTile("Spec Waves Tile", Range(0.05, 299)) = 1
	  _SpecWavesColor("Spec Waves Color", Color) = (1,1,1,1)
	  _SpecTimeScale("Spec Time Scale", Range(0.001, 5)) = 1
	  _SpecIntensity("Specular Intensity", Range(0.05, 5)) = 1
	  _SpecUVDirection("Spec Waves UV direction", Vector) = (1,0,0,0)
	  _Shininess("Specular Shininess", float) = 5
	}
	SubShader
	{
		Tags
		{
		  "QUEUE" = "Transparent"
		}
		LOD 200
		Pass
		{
		  Tags
		  {
			"QUEUE" = "Transparent"
		  }
		  LOD 200
		  Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog

			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			#include "Lighting.cginc"

			float _Shininess;
			float _SpecIntensity;
			float _ShoreTile;
			float _ShoreWavesTimeScale;
			float2 _ShoreWavesUVDirection;
			float _DiffuseWavesBounce;
			float _DiffuseTile;
			float _DiffuseWavesTimeScale;
			float2 _DiffuseWavesUVDirection;
			float _SpecWavesBounce;
			float _SpecTile;
			float _SpecTimeScale;
			float2 _SpecUVDirection;
			float4 _Color;
			sampler2D _WavesMap;
			float _ShoreFoamBrightness;
			float4 _ShoreWavesColor;
			float _ShoreWavesOpacity;
			float _ShoreTextureSampleAmnt;
			float4 _DiffuseWavesColor;
			float _DiffuseWavesOpacity;

			// Declare the fog coordinates
			UNITY_FOG_COORDS(1)

			struct v2f
			{
				float2 xlv_TEXCOORD0 : TEXCOORD0;
				float2 xlv_TEXCOORD1 : TEXCOORD1;
				float2 xlv_TEXCOORD2 : TEXCOORD2;
				float3 xlv_TEXCOORD3 : TEXCOORD3;
				float3 xlv_TEXCOORD4 : TEXCOORD4;
				float2 xlv_TEXCOORD5 : TEXCOORD5;
				float xlv_TEXCOORD6 : TEXCOORD6;
				UNITY_FOG_COORDS(7) // Fog coordinate for vertex shader output
			};

			struct fragOutput {
				float4 gl_FragData : SV_Target;
			};


			v2f vert(
			float4 _glesVertex : POSITION,
			float4 _glesColor : COLOR,
			float3 _glesNormal : NORMAL,
			float4 _glesMultiTexCoord0 : TEXCOORD0,
			out float4 gl_Position : SV_POSITION
			)
			{
			  v2f o;
			  float3 worldSpaceLightDirNormalized_1;
			  float3 worldSpaceViewDirNormalized_2;
			  float3 worldSpaceNormalNormalized_3;
			  float specWaveBounce_4;
			  float diffuseWaveBounce_5;
			  float3 tmpvar_6;
			  float3 tmpvar_7;
			  float4 tmpvar_8;
			  tmpvar_8.w = 1.0;
			  tmpvar_8.xyz = _glesVertex.xyz;
			  float tmpvar_9;
			  tmpvar_9 = (1.0 + (_SinTime.w * _DiffuseWavesBounce));
			  diffuseWaveBounce_5 = tmpvar_9;
			  float tmpvar_10;
			  tmpvar_10 = (1.0 + (_SinTime.w * _SpecWavesBounce));
			  specWaveBounce_4 = tmpvar_10;
			  float4 tmpvar_11;
			  tmpvar_11.w = 0.0;
			  tmpvar_11.xyz = _glesNormal;
			  float3 tmpvar_12;
			  tmpvar_12 = normalize(UnityObjectToClipPos(tmpvar_11).xyz);//(tmpvar_11 * unity_WorldToObject).xyz);
			  worldSpaceNormalNormalized_3 = tmpvar_12;
			  float3 tmpvar_13;
			  float4 tmpvar_14;
			  tmpvar_14 = UnityObjectToClipPos(_glesVertex);//mul(unity_ObjectToWorld, _glesVertex);
			  tmpvar_13 = normalize((_WorldSpaceCameraPos - tmpvar_14.xyz));
			  worldSpaceViewDirNormalized_2 = tmpvar_13;
			  float3 tmpvar_15;
			  tmpvar_15 = normalize((_WorldSpaceLightPos0.xyz - (tmpvar_14.xyz * _WorldSpaceLightPos0.w)));
			  worldSpaceLightDirNormalized_1 = tmpvar_15;
			  float3 tmpvar_16;
			  tmpvar_16 = _LightColor0.xyz;
			  float3 tmpvar_17;
			  float3 lightColor_18;
			  lightColor_18 = tmpvar_16;
			  float spec_19;
			  float3 I_20;
			  I_20 = -(worldSpaceLightDirNormalized_1);
			  float tmpvar_21;
			  tmpvar_21 = max(0.0, dot((I_20 -
				(2.0 * (dot(worldSpaceNormalNormalized_3, I_20) * worldSpaceNormalNormalized_3))
			  ), worldSpaceViewDirNormalized_2));
			  spec_19 = tmpvar_21;
			  float tmpvar_22;
			  tmpvar_22 = max(0.0, ((spec_19 * _Shininess) + (1.0 - _Shininess)));
			  tmpvar_17 = (lightColor_18 * tmpvar_22);
			  float3 tmpvar_23;
			  tmpvar_23 = max(float3(0.5, 0.5, 0.5), (tmpvar_17 * _SpecIntensity));
			  tmpvar_6 = tmpvar_23;
			  tmpvar_7.x = sign(_glesColor.x);
			  float tmpvar_24;
			  tmpvar_24 = min(2.0, (1.0 - _SinTime.w));
			  tmpvar_7.y = (tmpvar_24 - _glesColor.x);
			  float tmpvar_25;
			  tmpvar_25 = max(0.0, (1.0 - (
				(_CosTime.w + 1.5)
			   * 0.5)));
			  tmpvar_7.z = tmpvar_25;
			  float tmpvar_26;
			  tmpvar_26 = max(0.0, ((_glesColor.x - _glesColor.y) - _glesColor.z));
			  float2 tmpvar_27;
			  tmpvar_27.x = tmpvar_26;
			  tmpvar_27.y = (1.0 - tmpvar_26);
			  gl_Position = UnityObjectToClipPos(tmpvar_8);//mul(unity_MatrixVP, (unity_ObjectToWorld, tmpvar_8));
			  o.xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy + (
				(normalize(_ShoreWavesUVDirection) * _Time.x)
			   * _ShoreWavesTimeScale)) * _ShoreTile);
			  o.xlv_TEXCOORD1 = ((_glesMultiTexCoord0.xy + (
				((normalize(_DiffuseWavesUVDirection) * _Time.x) * _DiffuseWavesTimeScale)
			   * diffuseWaveBounce_5)) * _DiffuseTile);
			  o.xlv_TEXCOORD2 = (((_glesMultiTexCoord0.xy +
				((normalize(_SpecUVDirection) * _Time.x) * _SpecTimeScale)
			  ) * _SpecTile) * specWaveBounce_4);
			  o.xlv_TEXCOORD3 = tmpvar_6;
			  o.xlv_TEXCOORD4 = tmpvar_7;
			  o.xlv_TEXCOORD5 = tmpvar_27;
			  o.xlv_TEXCOORD6 = max(max(tmpvar_6.x, tmpvar_6.y), tmpvar_6.z);

			  // Handle fog
			  UNITY_TRANSFER_FOG(o, gl_Position);

			  return o;
			}


			fragOutput frag(v2f i)
			{
			  fragOutput o;
			  float specSample_1;
			  float3 shoreLineColor_2;
			  float shoreLineSample_3;
			  float tmpvar_4;
			  tmpvar_4 = tex2D(_WavesMap, i.xlv_TEXCOORD0).x;
			  shoreLineSample_3 = tmpvar_4;
			  float tmpvar_5;
			  tmpvar_5 = (((
				((i.xlv_TEXCOORD4.y * min(1.0, (1.0 +
				  sign(i.xlv_TEXCOORD4.y)
				))) * i.xlv_TEXCOORD4.z)
			   * i.xlv_TEXCOORD4.x) * _ShoreFoamBrightness) + 1.0);
			  float3 tmpvar_6;
			  tmpvar_6 = (((
				((shoreLineSample_3 * _ShoreTextureSampleAmnt) + (1.0 - _ShoreTextureSampleAmnt))
			   * tmpvar_5) * _ShoreWavesOpacity) * _ShoreWavesColor).xyz;
			  shoreLineColor_2 = tmpvar_6;
			  float4 tmpvar_7;
			  tmpvar_7 = tex2D(_WavesMap, i.xlv_TEXCOORD1);
			  float tmpvar_8;
			  tmpvar_8 = (tmpvar_7.y * _DiffuseWavesOpacity);
			  float tmpvar_9;
			  tmpvar_9 = tex2D(_WavesMap, i.xlv_TEXCOORD2).z;
			  specSample_1 = tmpvar_9;
			  float4 tmpvar_10;
			  tmpvar_10.xyz = (((_Color.xyz +
				(shoreLineColor_2 * i.xlv_TEXCOORD5.x)
			  ) + (
				(_DiffuseWavesColor * tmpvar_8)
			   * i.xlv_TEXCOORD5.y).xyz) + ((specSample_1 * i.xlv_TEXCOORD3) * i.xlv_TEXCOORD5.y));
			  tmpvar_10.w = max(_Color.w, max((
				max((specSample_1 * i.xlv_TEXCOORD6), tmpvar_8)
			   * i.xlv_TEXCOORD5.y), (
				(shoreLineSample_3 * _ShoreWavesOpacity)
			   *
				(i.xlv_TEXCOORD5.x * tmpvar_5)
			  )));
			  o.gl_FragData = tmpvar_10;

			  // Apply fog in fragment shader
			  UNITY_APPLY_FOG(i.fogCoord, o.gl_FragData);

			  return o;
			}

		ENDCG
	  }
	  }
		  FallBack Off
}
