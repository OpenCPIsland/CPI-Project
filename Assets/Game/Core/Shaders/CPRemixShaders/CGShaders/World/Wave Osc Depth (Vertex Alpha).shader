Shader "CpRemix/World/Wave Osc Depth (Vertex Alpha)" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_OscDir ("World Osc  Dir", Vector) = (1,0,0,1)
		_OscAxis ("World Osc Axs (w = wave freq)", Vector) = (0,1,0,1)
		_OscSpeed ("Osc Speed", Float) = 1
		_DepthMultiply ("DepthMultiply", Range(0, 1)) = 1
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		Pass {
			Tags { "RenderType" = "Opaque" }
			GpuProgramID 65288
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
				float2 texcoord4 : TEXCOORD4;
				float3 texcoord2 : TEXCOORD2;
				float3 texcoord3 : TEXCOORD3;
				float2 texcoord1 : TEXCOORD1;
				UNITY_FOG_COORDS(5)
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			float4 _MainTex_ST;
			float3 _OscDir;
			float4 _OscAxis;
			float _OscSpeed;
			float _DepthMultiply;
			float _SurfaceYCoord;
			float _DeepestYCoord;
			float3 _DepthColor;
			float3 _SurfaceReflectionColor;
			float _DynSurfaceTexTile;
			float _DynSurfaceMultiplier;
			float _SurfaceVelocityX;
			float _SurfaceVelocityZ;
			// $Globals ConstantBuffers for Fragment Shader
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _MainTex;
			sampler2D _SurfaceReflectionsRGB;
			
			// Keywords: 
v2f vert(appdata_full v)
{
    v2f o;
    float4 tmp0;
    float4 tmp1;
    float4 tmp2;
    tmp0.xyz = _OscAxis.yyy * unity_WorldToObject._m01_m11_m21;
    tmp0.xyz = unity_WorldToObject._m00_m10_m20 * _OscAxis.xxx + tmp0.xyz;
    tmp0.xyz = unity_WorldToObject._m02_m12_m22 * _OscAxis.zzz + tmp0.xyz;
    tmp0.x = dot(v.vertex.xyz, tmp0.xyz);
    tmp0.x = tmp0.x * _OscAxis.w;
    tmp0.x = _Time.y * _OscSpeed + tmp0.x;
    tmp0.x = sin(tmp0.x);
    tmp0.y = 1.0 - v.color.w;
    tmp0.x = tmp0.y * tmp0.x;
    tmp0.yzw = _OscDir * unity_WorldToObject._m01_m11_m21;
    tmp0.yzw = unity_WorldToObject._m00_m10_m20 * _OscDir + tmp0.yzw;
    tmp0.yzw = unity_WorldToObject._m02_m12_m22 * _OscDir + tmp0.yzw;
    tmp0.xyz = tmp0.xxx * tmp0.yzw + v.vertex.xyz;
    tmp1 = tmp0.yyyy * unity_ObjectToWorld._m01_m11_m21_m31;
    tmp1 = unity_ObjectToWorld._m00_m10_m20_m30 * tmp0.xxxx + tmp1;
    tmp0 = unity_ObjectToWorld._m02_m12_m22_m32 * tmp0.zzzz + tmp1;
    tmp0 = tmp0 + unity_ObjectToWorld._m03_m13_m23_m33;
    tmp1 = tmp0.yyyy * unity_MatrixVP._m01_m11_m21_m31;
    tmp1 = unity_MatrixVP._m00_m10_m20_m30 * tmp0.xxxx + tmp1;
    tmp1 = unity_MatrixVP._m02_m12_m22_m32 * tmp0.zzzz + tmp1;
    o.position = unity_MatrixVP._m03_m13_m23_m33 * tmp0.wwww + tmp1;
    o.color.xyz = v.color.xyz;

    // Corrected line for surface velocity
    tmp0.xy = _DynSurfaceTexTile.xx * float2(_SurfaceVelocityX, _SurfaceVelocityZ);
    tmp0.xy = tmp0.xy * _Time.xx;

    tmp1.xyz = v.vertex.yyy * unity_ObjectToWorld._m01_m11_m21;
    tmp1.xyz = unity_ObjectToWorld._m00_m10_m20 * v.vertex.xxx + tmp1.xyz;
    tmp1.xyz = unity_ObjectToWorld._m02_m12_m22 * v.vertex.zzz + tmp1.xyz;
    tmp1.xyz = unity_ObjectToWorld._m03_m13_m23 * v.vertex.www + tmp1.xyz;
    o.texcoord4.xy = tmp1.xz * _DynSurfaceTexTile.xx + -tmp0.xy;
    o.texcoord.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
    tmp0.x = tmp1.y - _DeepestYCoord;
    tmp0.y = _SurfaceYCoord - tmp1.y;
    tmp0.z = _SurfaceYCoord - _DeepestYCoord;
    tmp0.x = saturate(tmp0.x / tmp0.z);
    tmp0.x = 1.0 - tmp0.x;
    tmp0.z = tmp0.x * _DepthMultiply;
    tmp0.x = -tmp0.x * _DepthMultiply + 1.0;
    o.texcoord2.xyz = _DepthColor * tmp0.zzz + tmp0.xxx;
    tmp0.z = tmp0.y > 0.0;
    tmp0.z = tmp0.z ? 1.0 : 0.0;
    tmp0.y = tmp0.z * tmp0.y;
    tmp0.y = min(tmp0.y, 1.0);
    tmp1.x = unity_WorldToObject._m10;
    tmp1.y = unity_WorldToObject._m11;
    tmp1.z = unity_WorldToObject._m12;
    tmp1.xyz = tmp1.xyz * v.normal.yyy;
    tmp2.x = unity_WorldToObject._m00;
    tmp2.y = unity_WorldToObject._m01;
    tmp2.z = unity_WorldToObject._m02;
    tmp1.xyz = tmp2.xyz * v.normal.xxx + tmp1.xyz;
    tmp2.x = unity_WorldToObject._m20;
    tmp2.y = unity_WorldToObject._m21;
    tmp2.z = unity_WorldToObject._m22;
    tmp1.xyz = tmp2.xyz * v.normal.zzz + tmp1.xyz;
    tmp0.z = dot(tmp1.xyz, tmp1.xyz);
    tmp0.z = rsqrt(tmp0.z);
    tmp0.z = tmp0.z * tmp1.y;
    tmp0.w = tmp0.z > 0.0;
    tmp0.z = tmp0.z * tmp0.z;
    tmp0.w = tmp0.w ? 1.0 : 0.0;
    tmp0.z = tmp0.w * tmp0.z;
    tmp0.y = tmp0.y * tmp0.z;
    tmp0.x = tmp0.x * tmp0.y;
    tmp0.x = tmp0.x * _DynSurfaceMultiplier;
    tmp0.x = tmp0.x * 0.5;
    o.texcoord3.xyz = tmp0.xxx * _SurfaceReflectionColor;
    o.texcoord1.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    UNITY_TRANSFER_FOG(o,o.position);
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
                tmp1 = tex2D(_MainTex, inp.texcoord.xy);
                tmp1.xyz = tmp1.xyz * inp.color.xyz;
                tmp0.xyz = tmp0.xyz * tmp1.xyz;
                tmp1 = tex2D(_SurfaceReflectionsRGB, inp.texcoord4.xy);
                tmp1.xyz = tmp1.xxx * inp.texcoord3.xyz;
                o.sv_target.xyz = tmp0.xyz * inp.texcoord2.xyz + tmp1.xyz;
                o.sv_target.w = 1.0;
				UNITY_APPLY_FOG(inp.fogCoord, o.sv_target);
				UNITY_OPAQUE_ALPHA(o.sv_target.w);
                return o;
			}
			ENDCG
		}
	}
}