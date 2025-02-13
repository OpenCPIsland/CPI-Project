Shader "CpRemix/Equipment Screenshot" {
	Properties {
		_Diffuse ("Diffuse", 2D) = "black" {}
		[MaterialToggle] _UseUV2ForDecals ("Use UV2 for Decals", Float) = 0
		_Decal123OpacityTex ("Decals 123 Opacity", 2D) = "black" {}
		_Decal1Tex ("Decal 1 Texture", 2D) = "white" {}
		_Decal1Color ("Decal 1 Color", Color) = (0.26,0.78,1,1)
		_Decal1Scale ("Decal 1 Scale", Range(0.1, 30)) = 1
		_Decal1UOffset ("Decal 1 uOffset", Range(-0.5, 0.5)) = 0
		_Decal1VOffset ("Decal 1 vOffset", Range(-0.5, 0.5)) = 0
		_Decal1RotationRads ("Decal 1 Rotation Rads", Range(-3.141, 3.141)) = 0
		[MaterialToggle] _Decal1Repeat ("Repeat Decal 1", Float) = 0
		_Decal2Tex ("Decal 2 Texture", 2D) = "white" {}
		_Decal2Color ("Decal 2 Color", Color) = (0.06,0.55,1,1)
		_Decal2Scale ("Decal 2 Scale", Range(0.1, 30)) = 1
		_Decal2UOffset ("Decal 2 uOffset", Range(-0.5, 0.5)) = 0
		_Decal2VOffset ("Decal 2 vOffset", Range(-0.5, 0.5)) = 0
		_Decal2RotationRads ("Decal 2 Rotation Rads", Range(-3.141, 3.141)) = 0
		[MaterialToggle] _Decal2Repeat ("Repeat Decal 2", Float) = 0
		_Decal3Tex ("Decal 3 Texture", 2D) = "white" {}
		_Decal3Color ("Decal 3 Color", Color) = (0.01,0.33,0.95,1)
		_Decal3Scale ("Decal 3 Scale", Range(0.1, 30)) = 1
		_Decal3UOffset ("Decal 3 uOffset", Range(-0.5, 0.5)) = 0
		_Decal3VOffset ("Decal 3 vOffset", Range(-0.5, 0.5)) = 0
		_Decal3RotationRads ("Decal 3 Rotation Rads", Range(-3.141, 3.141)) = 0
		[MaterialToggle] _Decal3Repeat ("Repeat Decal 3", Float) = 0
		_Decal4Tex ("Decal 4 Texture", 2D) = "black" {}
		_Decal4Color ("Decal 4 Color", Color) = (1,1,1,1)
		_Decal4Scale ("Decal 4 Scale", Range(0.1, 30)) = 1
		_Decal4UOffset ("Decal 4 uOffset", Range(-0.5, 0.5)) = 0
		_Decal4VOffset ("Decal 4 vOffset", Range(-0.5, 0.5)) = 0
		_Decal4RotationRads ("Decal 4 Rotation Rads", Range(-3.141, 3.141)) = 0
		[MaterialToggle] _Decal4Repeat ("Repeat Decal 4", Float) = 0
		_Decal5Tex ("Decal 5 Texture", 2D) = "black" {}
		_Decal5Color ("Decal 5 Color", Color) = (1,1,1,1)
		_Decal5Scale ("Decal 5 Scale", Range(0.1, 30)) = 1
		_Decal5UOffset ("Decal 5 uOffset", Range(-0.5, 0.5)) = 0
		_Decal5VOffset ("Decal 5 vOffset", Range(-0.5, 0.5)) = 0
		_Decal5RotationRads ("Decal 5 Rotation Rads", Range(-3.141, 3.141)) = 0
		[MaterialToggle] _Decal5Repeat ("Repeat Decal 5", Float) = 0
		_Decal6Tex ("Decal 6 Texture", 2D) = "black" {}
		_Decal6Color ("Decal 6 Color", Color) = (1,1,1,1)
		_Decal6Scale ("Decal 6 Scale", Range(0.1, 30)) = 1
		_Decal6UOffset ("Decal 6 uOffset", Range(-0.5, 0.5)) = 0
		_Decal6VOffset ("Decal 6 vOffset", Range(-0.5, 0.5)) = 0
		_Decal6RotationRads ("Decal 6 Rotation Rads", Range(-3.141, 3.141)) = 0
		[MaterialToggle] _Decal6Repeat ("Repeat Decal 6", Float) = 0
		_BodyColorsMaskTex ("Body Color Mask", 2D) = "black" {}
		_BodyRedChannelColor ("Body Red Channel Color", Color) = (1,0,0,1)
		_BodyGreenChannelColor ("Body Green Channel Color", Color) = (1,1,0,1)
		_BodyBlueChannelColor ("Body Blue Channel Color", Color) = (1,0,1,1)
		_EmissiveColorTint ("EmissiveColorTint", Color) = (1,1,1,1)
		_DetailAndMatcapMaskAndEmissive ("r=detail g=matcap b=emissive", 2D) = "black" {}
		_ScreenshotBGColor ("Screenshot Background Color", Color) = (0.03,0.03,0.03,1)
	}
	SubShader {
		Pass {
			GpuProgramID 48317
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float2 texcoord2 : TEXCOORD2;
				float2 texcoord3 : TEXCOORD3;
				float2 texcoord4 : TEXCOORD4;
				float2 texcoord5 : TEXCOORD5;
				float2 texcoord6 : TEXCOORD6;
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			float _Decal1Scale;
			float _Decal1UOffset;
			float _Decal1VOffset;
			float _Decal1RotationRads;
			float _Decal2Scale;
			float _Decal2UOffset;
			float _Decal2VOffset;
			float _Decal2RotationRads;
			float _Decal3Scale;
			float _Decal3UOffset;
			float _Decal3VOffset;
			float _Decal3RotationRads;
			float _Decal4Scale;
			float _Decal4UOffset;
			float _Decal4VOffset;
			float _Decal4RotationRads;
			float _Decal5Scale;
			float _Decal5UOffset;
			float _Decal5VOffset;
			float _Decal5RotationRads;
			float _Decal6Scale;
			float _Decal6UOffset;
			float _Decal6VOffset;
			float _Decal6RotationRads;
			// $Globals ConstantBuffers for Fragment Shader
			float3 _Decal1Color;
			float _Decal1Repeat;
			float3 _Decal2Color;
			float _Decal2Repeat;
			float3 _Decal3Color;
			float _Decal3Repeat;
			float3 _Decal4Color;
			float _Decal4Repeat;
			float3 _Decal5Color;
			float _Decal5Repeat;
			float3 _Decal6Color;
			float _Decal6Repeat;
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _Diffuse;
			sampler2D _Decal123OpacityTex;
			sampler2D _Decal3Tex;
			sampler2D _Decal2Tex;
			sampler2D _Decal1Tex;
			sampler2D _Decal6Tex;
			sampler2D _Decal5Tex;
			sampler2D _Decal4Tex;
			
			// Keywords: 
			v2f vert(appdata_full v)
			{
                v2f o;
                float4 tmp0;
                float4 tmp1;
                float4 tmp2;
                float4 tmp3;
                tmp0 = v.vertex.yyyy * unity_ObjectToWorld._m01_m11_m21_m31;
                tmp0 = unity_ObjectToWorld._m00_m10_m20_m30 * v.vertex.xxxx + tmp0;
                tmp0 = unity_ObjectToWorld._m02_m12_m22_m32 * v.vertex.zzzz + tmp0;
                tmp0 = tmp0 + unity_ObjectToWorld._m03_m13_m23_m33;
                tmp1 = tmp0.yyyy * unity_MatrixVP._m01_m11_m21_m31;
                tmp1 = unity_MatrixVP._m00_m10_m20_m30 * tmp0.xxxx + tmp1;
                tmp1 = unity_MatrixVP._m02_m12_m22_m32 * tmp0.zzzz + tmp1;
                o.position = unity_MatrixVP._m03_m13_m23_m33 * tmp0.wwww + tmp1;
                tmp0.xy = float2(_Decal1UOffset.x, _Decal1VOffset.x) - float2(0.5, 0.5);
                tmp0.zw = tmp0.xy + v.texcoord.xy;
                tmp1.x = sin(_Decal1RotationRads);
                tmp2.x = cos(_Decal1RotationRads);
                tmp3.z = tmp1.x;
                tmp3.y = tmp2.x;
                tmp3.x = -tmp1.x;
                tmp1.w = dot(tmp0.xy, tmp3.xy);
                tmp1.z = dot(tmp0.xy, tmp3.xy);
                tmp0.xy = tmp1.zw - tmp0.xy;
                tmp0.xy = tmp0.xy + float2(_Decal1UOffset.x, _Decal1VOffset.x);
                tmp0.xy = tmp0.xy - float2(0.5, 0.5);
                o.texcoord1.xy = tmp0.xy * _Decal1Scale.xx + float2(0.5, 0.5);
                o.texcoord.xy = v.texcoord.xy;
                tmp0.xy = float2(_Decal2UOffset.x, _Decal2VOffset.x) - float2(0.5, 0.5);
                tmp0.zw = tmp0.xy + v.texcoord.xy;
                tmp1.x = sin(_Decal2RotationRads);
                tmp2.x = cos(_Decal2RotationRads);
                tmp3.z = tmp1.x;
                tmp3.y = tmp2.x;
                tmp3.x = -tmp1.x;
                tmp1.y = dot(tmp0.xy, tmp3.xy);
                tmp1.x = dot(tmp0.xy, tmp3.xy);
                tmp0.xy = tmp1.xy - tmp0.xy;
                tmp0.xy = tmp0.xy + float2(_Decal2UOffset.x, _Decal2VOffset.x);
                tmp0.xy = tmp0.xy - float2(0.5, 0.5);
                o.texcoord2.xy = tmp0.xy * _Decal2Scale.xx + float2(0.5, 0.5);
                tmp0.xy = float2(_Decal3UOffset.x, _Decal3VOffset.x) - float2(0.5, 0.5);
                tmp0.zw = tmp0.xy + v.texcoord.xy;
                tmp1.x = sin(_Decal3RotationRads);
                tmp2.x = cos(_Decal3RotationRads);
                tmp3.z = tmp1.x;
                tmp3.y = tmp2.x;
                tmp3.x = -tmp1.x;
                tmp1.w = dot(tmp0.xy, tmp3.xy);
                tmp1.z = dot(tmp0.xy, tmp3.xy);
                tmp0.xy = tmp1.zw - tmp0.xy;
                tmp0.xy = tmp0.xy + float2(_Decal3UOffset.x, _Decal3VOffset.x);
                tmp0.xy = tmp0.xy - float2(0.5, 0.5);
                o.texcoord3.xy = tmp0.xy * _Decal3Scale.xx + float2(0.5, 0.5);
                tmp0.xy = float2(_Decal4UOffset.x, _Decal4VOffset.x) - float2(0.5, 0.5);
                tmp0.zw = tmp0.xy + v.texcoord.xy;
                tmp1.x = sin(_Decal4RotationRads);
                tmp2.x = cos(_Decal4RotationRads);
                tmp3.z = tmp1.x;
                tmp3.y = tmp2.x;
                tmp3.x = -tmp1.x;
                tmp1.y = dot(tmp0.xy, tmp3.xy);
                tmp1.x = dot(tmp0.xy, tmp3.xy);
                tmp0.xy = tmp1.xy - tmp0.xy;
                tmp0.xy = tmp0.xy + float2(_Decal4UOffset.x, _Decal4VOffset.x);
                tmp0.xy = tmp0.xy - float2(0.5, 0.5);
                o.texcoord4.xy = tmp0.xy * _Decal4Scale.xx + float2(0.5, 0.5);
                tmp0.xy = float2(_Decal5UOffset.x, _Decal5VOffset.x) - float2(0.5, 0.5);
                tmp0.zw = tmp0.xy + v.texcoord.xy;
                tmp1.x = sin(_Decal5RotationRads);
                tmp2.x = cos(_Decal5RotationRads);
                tmp3.z = tmp1.x;
                tmp3.y = tmp2.x;
                tmp3.x = -tmp1.x;
                tmp1.w = dot(tmp0.xy, tmp3.xy);
                tmp1.z = dot(tmp0.xy, tmp3.xy);
                tmp0.xy = tmp1.zw - tmp0.xy;
                tmp0.xy = tmp0.xy + float2(_Decal5UOffset.x, _Decal5VOffset.x);
                tmp0.xy = tmp0.xy - float2(0.5, 0.5);
                o.texcoord5.xy = tmp0.xy * _Decal5Scale.xx + float2(0.5, 0.5);
                tmp0.xy = float2(_Decal6UOffset.x, _Decal6VOffset.x) - float2(0.5, 0.5);
                tmp0.zw = tmp0.xy + v.texcoord.xy;
                tmp1.x = sin(_Decal6RotationRads);
                tmp2.x = cos(_Decal6RotationRads);
                tmp3.z = tmp1.x;
                tmp3.y = tmp2.x;
                tmp3.x = -tmp1.x;
                tmp1.y = dot(tmp0.xy, tmp3.xy);
                tmp1.x = dot(tmp0.xy, tmp3.xy);
                tmp0.xy = tmp1.xy - tmp0.xy;
                tmp0.xy = tmp0.xy + float2(_Decal6UOffset.x, _Decal6VOffset.x);
                tmp0.xy = tmp0.xy - float2(0.5, 0.5);
                o.texcoord6.xy = tmp0.xy * _Decal6Scale.xx + float2(0.5, 0.5);
                return o;
			}
			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                float4 tmp1;
                float4 tmp2;
                float4 tmp3;
                float4 tmp4;
                float4 tmp5;
                tmp0 = float4(inp.texcoord3.xy, 0.0, 0.0) - float4(0.5, 0.5, 0.5, 0.5);
                tmp0 = tmp0 + tmp0;
                tmp0.xy = max(abs(tmp0.yw), abs(tmp0.xz));
                tmp0.z = _Decal3Repeat * 255.0 + 1.0;
                tmp0.x = tmp0.z >= tmp0.x;
                tmp0.x = tmp0.x ? 1.0 : 0.0;
                tmp1 = tex2D(_Decal3Tex, inp.texcoord3.xy);
                tmp1 = tmp0.xxxx * tmp1;
                tmp0.xzw = tmp1.xyz * _Decal3Color;
                tmp1.xy = inp.texcoord1.xy - float2(0.5, 0.5);
                tmp1.xy = tmp1.xy + tmp1.xy;
                tmp1.x = max(abs(tmp1.y), abs(tmp1.x));
                tmp1.y = _Decal1Repeat * 255.0 + 1.0;
                tmp1.x = tmp1.y >= tmp1.x;
                tmp1.x = tmp1.x ? 1.0 : 0.0;
                tmp2 = tex2D(_Decal1Tex, inp.texcoord1.xy);
                tmp2 = tmp1.xxxx * tmp2;
                tmp1.xyz = tmp2.xyz * _Decal1Color;
                tmp2.x = _Decal2Repeat * 255.0 + 1.0;
                tmp0.y = tmp2.x >= tmp0.y;
                tmp0.y = tmp0.y ? 1.0 : 0.0;
                tmp3 = tex2D(_Decal2Tex, inp.texcoord2.xy);
                tmp3 = tmp0.yyyy * tmp3;
                tmp2.xyz = tmp3.xyz * _Decal2Color;
                tmp4 = tex2D(_Decal123OpacityTex, inp.texcoord.xy);
                tmp0.y = tmp3.w * tmp4.y;
                tmp2.xyz = tmp0.yyy * tmp2.xyz;
                tmp0.y = tmp1.w * tmp4.z + tmp0.y;
                tmp1.w = tmp1.w * tmp4.z;
                tmp0.y = tmp2.w * tmp4.x + tmp0.y;
                tmp2.w = tmp2.w * tmp4.x;
                tmp1.xyz = tmp1.xyz * tmp2.www + tmp2.xyz;
                tmp0.xzw = tmp0.xzw * tmp1.www + tmp1.xyz;
                tmp0.y = min(tmp0.y, 1.0);
                tmp1 = float4(inp.texcoord5.xy, 0.0, 0.0) - float4(0.5, 0.5, 0.5, 0.5);
                tmp1 = tmp1 + tmp1;
                tmp1.xy = max(abs(tmp1.yw), abs(tmp1.xz));
                tmp1.z = _Decal5Repeat * 255.0 + 1.0;
                tmp1.x = tmp1.z >= tmp1.x;
                tmp1.x = tmp1.x ? 1.0 : 0.0;
                tmp2 = tex2D(_Decal5Tex, inp.texcoord5.xy);
                tmp2 = tmp1.xxxx * tmp2;
                tmp1.x = tmp4.y * tmp2.w;
                tmp2.xyz = tmp2.xyz * _Decal5Color;
                tmp1.zw = inp.texcoord6.xy - float2(0.5, 0.5);
                tmp1.zw = tmp1.zw + tmp1.zw;
                tmp1.z = max(abs(tmp1.w), abs(tmp1.z));
                tmp1.w = _Decal6Repeat * 255.0 + 1.0;
                tmp1.z = tmp1.w >= tmp1.z;
                tmp1.z = tmp1.z ? 1.0 : 0.0;
                tmp3 = tex2D(_Decal6Tex, inp.texcoord6.xy);
                tmp3 = tmp1.zzzz * tmp3;
                tmp1.z = -tmp3.w * tmp4.z + 1.0;
                tmp1.w = -tmp1.x * tmp1.z + 1.0;
                tmp2.w = _Decal4Repeat * 255.0 + 1.0;
                tmp1.y = tmp2.w >= tmp1.y;
                tmp1.y = tmp1.y ? 1.0 : 0.0;
                tmp5 = tex2D(_Decal4Tex, inp.texcoord4.xy);
                tmp5 = tmp1.yyyy * tmp5;
                tmp1.y = tmp4.x * tmp5.w;
                tmp4.xyw = tmp5.xyz * _Decal4Color;
                tmp1.xy = tmp1.zw * tmp1.xy;
                tmp1.w = tmp3.w * tmp4.z + tmp1.x;
                tmp2.xyz = tmp1.xxx * tmp2.xyz;
                tmp1.x = tmp4.z * tmp3.w;
                tmp3.xyz = tmp3.xyz * _Decal6Color;
                tmp1.w = tmp1.y * tmp1.z + tmp1.w;
                tmp1.y = tmp1.z * tmp1.y;
                tmp2.xyz = tmp4.xyw * tmp1.yyy + tmp2.xyz;
                tmp1.xyz = tmp3.xyz * tmp1.xxx + tmp2.xyz;
                tmp1.w = min(tmp1.w, 1.0);
                tmp2.x = 1.0 - tmp1.w;
                tmp0.xzw = tmp0.xzw * tmp2.xxx;
                tmp0.xzw = tmp1.xyz * tmp1.www + tmp0.xzw;
                tmp0.y = tmp0.y + tmp1.w;
                tmp0.y = min(tmp0.y, 1.0);
                tmp0.xzw = tmp0.yyy * tmp0.xzw;
                tmp0.y = 1.0 - tmp0.y;
                tmp1 = tex2D(_Diffuse, inp.texcoord.xy);
                o.sv_target.xyz = tmp1.xyz * tmp0.yyy + tmp0.xzw;
                o.sv_target.w = 1.0;
                return o;
			}
			ENDCG
		}
		Pass {
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			GpuProgramID 67386
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float2 texcoord : TEXCOORD0;
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			// $Globals ConstantBuffers for Fragment Shader
			float3 _BodyRedChannelColor;
			float3 _BodyGreenChannelColor;
			float3 _BodyBlueChannelColor;
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _BodyColorsMaskTex;
			
			// Keywords: 
			v2f vert(appdata_full v)
			{
                v2f o;
                float4 tmp0;
                float4 tmp1;
                tmp0 = v.vertex.yyyy * unity_ObjectToWorld._m01_m11_m21_m31;
                tmp0 = unity_ObjectToWorld._m00_m10_m20_m30 * v.vertex.xxxx + tmp0;
                tmp0 = unity_ObjectToWorld._m02_m12_m22_m32 * v.vertex.zzzz + tmp0;
                tmp0 = tmp0 + unity_ObjectToWorld._m03_m13_m23_m33;
                tmp1 = tmp0.yyyy * unity_MatrixVP._m01_m11_m21_m31;
                tmp1 = unity_MatrixVP._m00_m10_m20_m30 * tmp0.xxxx + tmp1;
                tmp1 = unity_MatrixVP._m02_m12_m22_m32 * tmp0.zzzz + tmp1;
                o.position = unity_MatrixVP._m03_m13_m23_m33 * tmp0.wwww + tmp1;
                o.texcoord.xy = v.texcoord.xy;
                return o;
			}
			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                float4 tmp1;
                float4 tmp2;
                tmp0 = tex2D(_BodyColorsMaskTex, inp.texcoord.xy);
                tmp1.xyz = tmp0.yyy * _BodyGreenChannelColor;
                tmp1.xyz = tmp0.xxx * _BodyRedChannelColor + tmp1.xyz;
                tmp1.xyz = tmp0.zzz * _BodyBlueChannelColor + tmp1.xyz;
                tmp0.w = tmp0.x + tmp0.z;
                tmp0.w = tmp0.y + tmp0.w;
                tmp0.w = tmp0.w > 0.3;
                tmp1.w = tmp0.w ? 1.0 : 0.0;
                tmp0.w = tmp0.w ? 0.0 : 1.0;
                tmp2.xyz = tmp1.www * _BodyRedChannelColor;
                o.sv_target.xyz = tmp1.xyz * tmp0.www + tmp2.xyz;
                tmp0.x = max(tmp0.y, tmp0.x);
                o.sv_target.w = max(tmp0.z, tmp0.x);
                return o;
			}
			ENDCG
		}
		Pass {
			Tags { "LIGHTMODE" = "FORWARDBASE" }
			Blend Zero SrcColor, Zero SrcColor
			GpuProgramID 133977
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float2 texcoord : TEXCOORD0;
				float3 texcoord1 : TEXCOORD1;
				float3 color : COLOR0;
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			float4 _LightColor0;
			// $Globals ConstantBuffers for Fragment Shader
			float3 _EmissiveColorTint;
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _DetailAndMatcapMaskAndEmissive;
			sampler2D _BodyColorsMaskTex;
			
			// Keywords: 
			v2f vert(appdata_full v)
			{
                v2f o;
                float4 tmp0;
                float4 tmp1;
                float4 tmp2;
                tmp0 = v.vertex.yyyy * unity_ObjectToWorld._m01_m11_m21_m31;
                tmp0 = unity_ObjectToWorld._m00_m10_m20_m30 * v.vertex.xxxx + tmp0;
                tmp0 = unity_ObjectToWorld._m02_m12_m22_m32 * v.vertex.zzzz + tmp0;
                tmp1 = tmp0 + unity_ObjectToWorld._m03_m13_m23_m33;
                tmp0.xyz = unity_ObjectToWorld._m03_m13_m23 * v.vertex.www + tmp0.xyz;
                tmp0.xyz = -tmp0.xyz * _WorldSpaceLightPos0.www + _WorldSpaceLightPos0.xyz;
                tmp2 = tmp1.yyyy * unity_MatrixVP._m01_m11_m21_m31;
                tmp2 = unity_MatrixVP._m00_m10_m20_m30 * tmp1.xxxx + tmp2;
                tmp2 = unity_MatrixVP._m02_m12_m22_m32 * tmp1.zzzz + tmp2;
                o.position = unity_MatrixVP._m03_m13_m23_m33 * tmp1.wwww + tmp2;
                o.texcoord.xy = v.texcoord.xy;
                tmp0.w = dot(tmp0.xyz, tmp0.xyz);
                tmp0.w = rsqrt(tmp0.w);
                tmp0.xyz = tmp0.www * tmp0.xyz;
                tmp1.x = v.normal.x * unity_WorldToObject._m00;
                tmp1.y = v.normal.x * unity_WorldToObject._m01;
                tmp1.z = v.normal.x * unity_WorldToObject._m02;
                tmp2.x = v.normal.y * unity_WorldToObject._m10;
                tmp2.y = v.normal.y * unity_WorldToObject._m11;
                tmp2.z = v.normal.y * unity_WorldToObject._m12;
                tmp1.xyz = tmp1.xyz + tmp2.xyz;
                tmp2.x = v.normal.z * unity_WorldToObject._m20;
                tmp2.y = v.normal.z * unity_WorldToObject._m21;
                tmp2.z = v.normal.z * unity_WorldToObject._m22;
                tmp1.xyz = tmp1.xyz + tmp2.xyz;
                tmp0.w = dot(tmp1.xyz, tmp1.xyz);
                tmp0.w = rsqrt(tmp0.w);
                tmp1.xyz = tmp0.www * tmp1.xyz;
                tmp0.x = dot(tmp1.xyz, tmp0.xyz);
                tmp0.x = max(tmp0.x, 0.0);
                tmp0.xyz = tmp0.xxx * _LightColor0.xyz;
                tmp1.xyz = glstate_lightmodel_ambient.xyz * float3(0.9, 0.9, 0.9);
                o.texcoord1.xyz = tmp0.xyz * float3(0.65, 0.65, 0.65) + tmp1.xyz;
                o.color.xyz = v.color.xyz;
                return o;
			}
			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                float4 tmp1;
                tmp0 = tex2D(_DetailAndMatcapMaskAndEmissive, inp.texcoord.xy);
                tmp0.xyw = tmp0.xxx * inp.texcoord1.xyz;
                tmp1.x = 1.0 - tmp0.z;
                tmp1.yzw = tmp0.zzz * _EmissiveColorTint;
                tmp0.xyz = tmp0.xyw * tmp1.xxx + tmp1.yzw;
                tmp1 = tex2D(_BodyColorsMaskTex, inp.texcoord.xy);
                tmp0.w = tmp1.x + tmp1.z;
                tmp0.w = tmp1.y + tmp0.w;
                tmp0.w = tmp0.w > 0.3;
                tmp1.x = tmp0.w ? 1.0 : 0.0;
                tmp0.w = tmp0.w ? 0.0 : 1.0;
                o.sv_target.xyz = tmp0.xyz * tmp0.www + tmp1.xxx;
                o.sv_target.w = 1.0;
                return o;
			}
			ENDCG
		}
	}
}