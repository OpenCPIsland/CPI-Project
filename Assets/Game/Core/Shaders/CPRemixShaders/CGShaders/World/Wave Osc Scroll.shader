Shader "CpRemix/World/Wave Osc Scroll" {
    Properties {
        _TintColor ("Tint Colour", Vector) = (0,0,0,1)
        _MultColor ("Mult Colour", Vector) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _OscDir ("World Osc Dir", Vector) = (1,0,0,1)
        _OscAxis ("World Osc Axs (w = wave freq)", Vector) = (0,1,0,1)
        _OscSpeed ("Osc Speed", Float) = 1
        _XScrollSpeed ("X Scroll Speed", Float) = 1
        _YScrollSpeed ("Y Scroll Speed", Float) = 1
    }
    SubShader {
        Tags { "RenderType" = "Opaque" }
        Pass {
            Tags { "RenderType" = "Opaque" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2f {
                float4 position : SV_POSITION0;
                float4 texcoord : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
            };

            struct fout {
                float4 sv_target : SV_Target0;
            };

            // $Globals ConstantBuffers for Vertex Shader
            float3 _OscDir;
            float4 _OscAxis;
            float _OscSpeed;
            float _XScrollSpeed;
            float _YScrollSpeed;

            // $Globals ConstantBuffers for Fragment Shader
            float4 _TintColor;
            float4 _MultColor;

            // Texture params for Fragment Shader
            sampler2D _MainTex;

            v2f vert(appdata_full v) {
                v2f o;
                float4 tmp0;
                float4 tmp1;

                tmp0.xyz = _OscAxis.yyy * unity_WorldToObject._m01_m11_m21;
                tmp0.xyz = unity_WorldToObject._m00_m10_m20 * _OscAxis.xxx + tmp0.xyz;
                tmp0.xyz = unity_WorldToObject._m02_m12_m22 * _OscAxis.zzz + tmp0.xyz;
                tmp0.x = dot(v.vertex.xyz, tmp0.xyz);
                tmp0.y = _OscSpeed * _Time.y;
                tmp0.x = tmp0.x * _OscAxis.w + tmp0.y;
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

                o.texcoord = v.color;
                o.texcoord1.xy = _Time.xx * float2(_XScrollSpeed.x, _YScrollSpeed.x) + v.texcoord.xy;

                return o;
            }

            fout frag(v2f inp) {
                fout o;
                float4 tmp0;

                tmp0 = tex2D(_MainTex, inp.texcoord1.xy);
                tmp0.xyz = tmp0.xyz + _TintColor.xyz;
                o.sv_target.xyz = tmp0.xyz * _MultColor.xyz + inp.texcoord.xyz;
                o.sv_target.w = 1.0;

                return o;
            }

            ENDCG
        }
    }
}
