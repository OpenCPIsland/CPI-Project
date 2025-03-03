Shader "CpRemix/Avatar Body Bake"
{
	Properties
	{
		_Diffuse("Diffuse", 2D) = "black" {}
		_BodyColorsMaskTex("Body Color Mask", 2D) = "black" {}
		_BodyRedChannelColor("Body Red Channel Color", Color) = (1,0,0,1)
		_BodyGreenChannelColor("Body Green Channel Color", Color) = (1,1,0,1)
		_BodyBlueChannelColor("Body Blue Channel Color", Color) = (1,0,1,1)
		_DetailAndMatcapMaskAndEmissive("r=detail g=MatCapMask b=emissive", 2D) = "black" {}
		_AtlasOffsetU("AtlasOffset U", float) = 0
		_AtlasOffsetV("AtlasOffset V", float) = 0
		_AtlasOffsetScaleU("AtlasOffset U Scale", float) = 1
		_AtlasOffsetScaleV("AtlasOffset V Scale", float) = 1
	}
	SubShader
	{
		Tags {}
		Pass
		{
			Tags
			{
				"LIGHTMODE" = "FORWARDBASE"
			}
			Blend One One
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform sampler2D _Diffuse;
			uniform sampler2D _BodyColorsMaskTex;
			uniform float3 _BodyRedChannelColor;
			uniform float3 _BodyGreenChannelColor;
			uniform float3 _BodyBlueChannelColor;
			uniform sampler2D _DetailAndMatcapMaskAndEmissive;
			uniform float _AtlasOffsetU;
			uniform float _AtlasOffsetV;
			uniform float _AtlasOffsetScaleU;
			uniform float _AtlasOffsetScaleV;

			struct appdata_t
			{
				float4 _glesVertex : POSITION;
				float4 _glesMultiTexCoord0 : TEXCOORD0;
			};

			struct OUT_Data_Vert
			{
				float2 xlv_TEXCOORD0 : TEXCOORD0;
				float4 gl_Position : SV_POSITION;
			};

			struct v2f
			{
				float2 xlv_TEXCOORD0 : TEXCOORD0;
			};

			struct OUT_Data_Frag
			{
				float4 gl_FragData : SV_Target0;
			};

			OUT_Data_Vert vert(appdata_t v)
			{
				OUT_Data_Vert o;
				float2 texCoordOffset;
				texCoordOffset.x = (v._glesMultiTexCoord0.x - _AtlasOffsetU) / _AtlasOffsetScaleU;
				texCoordOffset.y = (v._glesMultiTexCoord0.y - _AtlasOffsetV) / _AtlasOffsetScaleV;
				o.gl_Position = UnityObjectToClipPos(v._glesVertex);
				o.xlv_TEXCOORD0 = texCoordOffset;
				return o;
			}

			OUT_Data_Frag frag(v2f f)
			{
				OUT_Data_Frag o;
				float3 detail_MatcapMask;
				float3 diffuseAndBodyColor;
				float4 bodyColors;
				float3 diffuseSample = tex2D(_Diffuse, f.xlv_TEXCOORD0).xyz;
				float3 bodyMask = tex2D(_BodyColorsMaskTex, f.xlv_TEXCOORD0).xyz;

				bodyColors.xyz = bodyMask.x * _BodyRedChannelColor + bodyMask.y * _BodyGreenChannelColor + bodyMask.z * _BodyBlueChannelColor;
				bodyColors.w = max(max(bodyMask.x, bodyMask.y), bodyMask.z);

				diffuseAndBodyColor = diffuseSample * (1.0 - bodyColors.w) + bodyColors.xyz * bodyColors.w;

				detail_MatcapMask = tex2D(_DetailAndMatcapMaskAndEmissive, f.xlv_TEXCOORD0).xyz;

				float2 absTexCoord = abs(f.xlv_TEXCOORD0 - 0.5) * 2.0;
				float alphaMask = step(max(absTexCoord.x, absTexCoord.y), 1.0); // Optimize conditional calculation

				o.gl_FragData.xyz = diffuseAndBodyColor * detail_MatcapMask.x;
				o.gl_FragData.w = 0.5 - detail_MatcapMask.y * 0.5;
				o.gl_FragData *= alphaMask; // Apply alpha mask to all components

				return o;
			}

			ENDCG
		}
	}
	FallBack Off
}
