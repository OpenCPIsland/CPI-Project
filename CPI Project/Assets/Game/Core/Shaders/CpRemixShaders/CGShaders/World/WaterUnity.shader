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
            "RenderPipeline" = "UniversalPipeline"
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

			HLSLPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
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
			float4 _SpecWavesColor;

			struct v2f
			{
				float2 xlv_TEXCOORD0 : TEXCOORD0;
				float2 xlv_TEXCOORD1 : TEXCOORD1;
				float2 xlv_TEXCOORD2 : TEXCOORD2;
				float3 xlv_TEXCOORD3 : TEXCOORD3;
				float3 xlv_TEXCOORD4 : TEXCOORD4;
				float2 xlv_TEXCOORD5 : TEXCOORD5;
				float xlv_TEXCOORD6 : TEXCOORD6;
				float fogFactor : TEXCOORD7; // Fog coordinate for vertex shader output
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

			  float diffuseWaveBounce = 1.0 + (_SinTime.w * _DiffuseWavesBounce);
			  float specWaveBounce = 1.0 + (_SinTime.w * _SpecWavesBounce);

			  float4 normalVec = float4(_glesNormal, 0.0);
			  float3 worldNormal = normalize(TransformObjectToWorldNormal(_glesNormal));

			  float4 clipPos = TransformObjectToHClip(_glesVertex.xyz);
			  float3 viewDir = normalize(_WorldSpaceCameraPos - clipPos.xyz);
			  float3 lightDir = normalize(_MainLightPosition.xyz - (clipPos.xyz * _MainLightPosition.w));

			  // Specular reflection
			  float3 incidentLight = -lightDir;
			  float spec = max(0.0, dot((incidentLight -
				(2.0 * (dot(worldNormal, incidentLight) * worldNormal))
			  ), viewDir));
			  float specFactor = max(0.0, (spec * _Shininess) + (1.0 - _Shininess));
			  float3 specColor = max(float3(0.5, 0.5, 0.5), (_MainLightColor.xyz * specFactor) * _SpecIntensity);

			  // Shore wave animation data
			  float3 waveData;
			  waveData.x = sign(_glesColor.x);
			  waveData.y = min(2.0, (1.0 - _SinTime.w)) - _glesColor.x;
			  waveData.z = max(0.0, 1.0 - ((_CosTime.w + 1.5) * 0.5));

			  float shoreMask = max(0.0, (_glesColor.x - _glesColor.y) - _glesColor.z);

			  gl_Position = TransformObjectToHClip(_glesVertex.xyz);
			  o.xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy + (
				(normalize(_ShoreWavesUVDirection) * _Time.x)
			   * _ShoreWavesTimeScale)) * _ShoreTile);
			  o.xlv_TEXCOORD1 = ((_glesMultiTexCoord0.xy + (
				((normalize(_DiffuseWavesUVDirection) * _Time.x) * _DiffuseWavesTimeScale)
			   * diffuseWaveBounce)) * _DiffuseTile);
			  o.xlv_TEXCOORD2 = (((_glesMultiTexCoord0.xy +
				((normalize(_SpecUVDirection) * _Time.x) * _SpecTimeScale)
			  ) * _SpecTile) * specWaveBounce);
			  o.xlv_TEXCOORD3 = specColor;
			  o.xlv_TEXCOORD4 = waveData;
			  o.xlv_TEXCOORD5 = float2(shoreMask, 1.0 - shoreMask);
			  o.xlv_TEXCOORD6 = max(max(specColor.x, specColor.y), specColor.z);

			  o.fogFactor = ComputeFogFactor(gl_Position.z);

			  return o;
			}


			fragOutput frag(v2f i)
			{
			  fragOutput o;

			  float shoreLineSample = tex2D(_WavesMap, i.xlv_TEXCOORD0).x;

			  float shoreFoam = (((i.xlv_TEXCOORD4.y * min(1.0, 1.0 + sign(i.xlv_TEXCOORD4.y)))
				* i.xlv_TEXCOORD4.z) * i.xlv_TEXCOORD4.x * _ShoreFoamBrightness) + 1.0;

			  float3 shoreLineColor = (((shoreLineSample * _ShoreTextureSampleAmnt + (1.0 - _ShoreTextureSampleAmnt))
				* shoreFoam) * _ShoreWavesOpacity * _ShoreWavesColor).xyz;

			  float diffuseSample = tex2D(_WavesMap, i.xlv_TEXCOORD1).y * _DiffuseWavesOpacity;
			  float specSample = tex2D(_WavesMap, i.xlv_TEXCOORD2).z;

			  o.gl_FragData.xyz = ((_Color.xyz +
				(shoreLineColor * i.xlv_TEXCOORD5.x)
			  ) + (
				(_DiffuseWavesColor * diffuseSample)
			   * i.xlv_TEXCOORD5.y).xyz) + ((specSample * _SpecWavesColor.xyz * i.xlv_TEXCOORD3) * i.xlv_TEXCOORD5.y);

			  o.gl_FragData.w = max(_Color.w, max((
				max(specSample * i.xlv_TEXCOORD6, diffuseSample)
			   * i.xlv_TEXCOORD5.y), (
				(shoreLineSample * _ShoreWavesOpacity)
			   * (i.xlv_TEXCOORD5.x * shoreFoam)
			  )));

			  o.gl_FragData.rgb = MixFog(o.gl_FragData.rgb, i.fogFactor);

			  return o;
			}

		ENDHLSL
	  }
	  }
		  FallBack Off
}
