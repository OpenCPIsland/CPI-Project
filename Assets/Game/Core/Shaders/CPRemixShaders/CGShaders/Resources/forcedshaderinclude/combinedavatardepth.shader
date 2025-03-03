Shader "CpRemix/Combined Avatar Depth" {
    Properties {
        _MainTex ("Diffuse Texture", 2D) = "white" {}
    }
    SubShader {
        Pass {
            Tags { "LIGHTMODE" = "FORWARDBASE" }
            GpuProgramID 44191
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            struct v2f {
                float4 position : SV_POSITION0;
                float2 texcoord : TEXCOORD0;
                float2 texcoord5 : TEXCOORD5;
                float3 texcoord1 : TEXCOORD1;
                float3 color : COLOR0;
                float3 texcoord3 : TEXCOORD3;
                float3 texcoord4 : TEXCOORD4;
            };
            
            struct fout {
                float4 sv_target : SV_Target0;
            };
            
            float4 _LightColor0;
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
                float4 tmp0 = float4(0.0, 0.0, 0.0, 0.0); // Correct number of arguments
                float4 tmp1 = float4(0.0, 0.0, 0.0, 0.0);
                float4 tmp2 = float4(0.0, 0.0, 0.0, 0.0);

                tmp0.xyz = v.vertex.yyy * float3(unity_ObjectToWorld._m01, unity_ObjectToWorld._m11, unity_ObjectToWorld._m21);
                tmp0.xyz = v.vertex.xxx * float3(unity_ObjectToWorld._m00, unity_ObjectToWorld._m10, unity_ObjectToWorld._m20) + tmp0.xyz;
                tmp0.xyz = v.vertex.zzz * float3(unity_ObjectToWorld._m02, unity_ObjectToWorld._m12, unity_ObjectToWorld._m22) + tmp0.xyz;
                tmp0.xyz = v.vertex.www * float3(unity_ObjectToWorld._m03, unity_ObjectToWorld._m13, unity_ObjectToWorld._m23) + tmp0.xyz;

                tmp1 = float4(tmp0.yyy * unity_MatrixVP._m01_m11_m21_m31, 1.0); // Fixed the constructor
                tmp1 = unity_MatrixVP._m00_m10_m20_m30 * tmp0.xxxx + tmp1;
                tmp1 = unity_MatrixVP._m02_m12_m22_m32 * tmp0.zzzz + tmp1;
                o.position = tmp1 + unity_MatrixVP._m03_m13_m23_m33;

                tmp1.xy = float2(_SurfaceVelocityX, _SurfaceVelocityZ) * _Time.xx;
                o.texcoord5.xy = tmp0.xz * _DynSurfaceTexTile + -tmp1.xy;
                o.texcoord.xy = v.texcoord.xy;

                tmp0.xzw = -tmp0.xyz * _WorldSpaceLightPos0.www + _WorldSpaceLightPos0.xyz;
                tmp1.x = dot(tmp0.xyz, tmp0.xyz);
                tmp1.x = rsqrt(tmp1.x);
                tmp0.xzw = tmp0.xzw * tmp1.xxx;

                tmp1.xyz = v.normal.yyy * float3(unity_ObjectToWorld._m01, unity_ObjectToWorld._m11, unity_ObjectToWorld._m21);
                tmp1.xyz = v.normal.xxx * float3(unity_ObjectToWorld._m00, unity_ObjectToWorld._m10, unity_ObjectToWorld._m20) + tmp1.xyz;
                tmp1.xyz = v.normal.zzz * float3(unity_ObjectToWorld._m02, unity_ObjectToWorld._m12, unity_ObjectToWorld._m22) + tmp1.xyz;

                tmp1.w = dot(tmp1.xyz, tmp1.xyz);
                tmp1.w = rsqrt(tmp1.w);
                tmp1.xyz = tmp1.www * tmp1.xyz;

                tmp0.x = dot(tmp1.xyz, tmp0.xyz);
                tmp0.x = max(tmp0.x, 0.0);

                tmp1.xzw = tmp0.xxx * _LightColor0.xyz;
                tmp0.x = tmp0.x + 0.5;
                tmp0.x = tmp0.x * 0.6;

                tmp2.xyz = glstate_lightmodel_ambient.xyz * float3(0.9, 0.9, 0.9);
                tmp1.xzw = tmp1.xzw * float3(0.75, 0.75, 0.75) + tmp2.xyz;
                o.texcoord1.xyz = max(tmp0.xxx, tmp1.xzw);

                o.color.xyz = v.color.xyz;

                tmp0.x = -_DeepestYCoord * 2.0 + tmp0.y;
                tmp0.y = _SurfaceYCoord - tmp0.y;
                tmp0.z = -_DeepestYCoord * 2.0 + _SurfaceYCoord;
                tmp0.x = saturate(tmp0.x / tmp0.z);
                tmp0.x = 1.0 - tmp0.x;
                tmp0.z = 1.0 - tmp0.x;
                o.texcoord3.xyz = _DepthColor * tmp0.xxx + tmp0.zzz;

                tmp0.x = tmp0.y > 0.0 ? 1.0 : 0.0;
                tmp0.x *= tmp0.y;
                tmp0.x = min(tmp0.x, 1.0);

                tmp0.y = tmp1.y > 0.0;
                tmp0.w = tmp1.y * tmp1.y;
                tmp0.y = tmp0.y ? 1.0 : 0.0;
                tmp0.y *= tmp0.w;
                tmp0.x *= tmp0.y;
                tmp0.x *= _DynSurfaceMultiplier;
                tmp0.x *= 0.5;
                o.texcoord4.xyz = tmp0.xxx * _SurfaceReflectionColor;

                return o;
            }

            fout frag(v2f inp) {
                fout o;
                float4 tmp0 = tex2D(_MainTex, inp.texcoord.xy);
                tmp0.w = tmp0.w * 2.0 + -1.0;
                float4 tmp1;
                tmp1.x = tmp0.w >= 0.0 ? 1.0 : 0.0;
                tmp1.y = tmp0.w * tmp1.x;
                tmp0.w = -tmp0.w * tmp1.x + 1.0;
                tmp1.xyz = tmp0.xyz * tmp1.yyy;
                tmp0.xyz = tmp0.xyz * inp.texcoord1.xyz;
                tmp0.xyz = tmp0.xyz * tmp0.www + tmp1.xyz;
                tmp1 = tex2D(_SurfaceReflectionsRGB, inp.texcoord5.xy);
                tmp1.xyz = tmp1.xxx * inp.texcoord4.xyz;
                o.sv_target.xyz = tmp0.xyz * inp.texcoord3.xyz + tmp1.xyz;
                o.sv_target.w = 1.0;
                return o;
            }

            ENDCG
        }
    }
}
