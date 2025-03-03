Shader "CpRemix/World/Fix/Cull Off/Unlit Dynamic Object Depth" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _DepthMultiply ("DepthMultiply", Range(0, 1)) = 1
    }
    SubShader {
        Tags { "RenderType" = "Opaque" }
        Pass {
            Cull Off
            Tags { "RenderType" = "Opaque" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"


            struct v2f {
                float4 position : SV_POSITION0;
                float2 texcoord : TEXCOORD0;
                float2 texcoord4 : TEXCOORD4;
                float3 texcoord2 : TEXCOORD2;
                float3 texcoord3 : TEXCOORD3;
                float4 color : COLOR0;
                UNITY_FOG_COORDS(1)
            };

            struct fout {
                float4 sv_target : SV_Target0;
            };

            float4 _MainTex_ST;
            float _DepthMultiply;
            float _SurfaceYCoord;
            float _DeepestYCoord;
            float3 _DepthColor;
            float3 _SurfaceReflectionColor;
            float _DynSurfaceTexTile;
            float _DynSurfaceMultiplier;
            float _SurfaceVelocityX;
            float _SurfaceVelocityZ;
            sampler2D _MainTex;
            sampler2D _SurfaceReflectionsRGB;

            v2f vert(appdata_full v) {
                v2f o;
                float4 tmp0;
                float4 tmp1;
                float4 tmp2;

                tmp0 = v.vertex.yyyy * unity_ObjectToWorld._m01_m11_m21_m31;
                tmp0 = unity_ObjectToWorld._m00_m10_m20_m30 * v.vertex.xxxx + tmp0;
                tmp0 = unity_ObjectToWorld._m02_m12_m22_m32 * v.vertex.zzzz + tmp0;
                tmp1 = tmp0 + unity_ObjectToWorld._m03_m13_m23_m33;
                tmp0.xyz = unity_ObjectToWorld._m03_m13_m23 * v.vertex.www + tmp0.xyz;
                tmp2 = tmp1.yyyy * unity_MatrixVP._m01_m11_m21_m31;
                tmp2 = unity_MatrixVP._m00_m10_m20_m30 * tmp1.xxxx + tmp2;
                tmp2 = unity_MatrixVP._m02_m12_m22_m32 * tmp1.zzzz + tmp2;
                o.position = unity_MatrixVP._m03_m13_m23_m33 * tmp1.wwww + tmp2;

                tmp1.xy = _DynSurfaceTexTile * float2(_SurfaceVelocityX, _SurfaceVelocityZ);
                tmp1.xy = tmp1.xy * _Time.xx;
                o.texcoord4.xy = tmp0.xz * _DynSurfaceTexTile - tmp1.xy;
                o.texcoord.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;

                tmp0.x = tmp0.y - _DeepestYCoord;
                tmp0.y = _SurfaceYCoord - tmp0.y;
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
                o.color = v.color;

                UNITY_TRANSFER_FOG(o, o.position);
                return o;
            }

            fout frag(v2f inp) {
                fout o;
                float4 tmp0;
                float4 tmp1;

                tmp0 = tex2D(_SurfaceReflectionsRGB, inp.texcoord4.xy);
                tmp0.xyz = tmp0.xxx * inp.texcoord3.xyz;
                tmp1 = tex2D(_MainTex, inp.texcoord.xy);
                tmp0.xyz = tmp1.xyz * inp.texcoord2.xyz + tmp0.xyz;

                o.sv_target.xyz = tmp0.xyz * inp.color.xyz;
                o.sv_target.w = 1.0;

                UNITY_APPLY_FOG(inp.fogCoord, o.sv_target);
                UNITY_OPAQUE_ALPHA(o.sv_target.w);

                return o;
            }
            ENDCG
        }
    }
}
