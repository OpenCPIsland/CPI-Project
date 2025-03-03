Shader "Cg shader for toon shading" {
    Properties {
        _Color ("Diffuse Color", Vector) = (1,1,1,1)
        _UnlitColor ("Unlit Diffuse Color", Vector) = (0.5,0.5,0.5,1)
        _DiffuseThreshold ("Threshold for Diffuse Colors", Range(0, 1)) = 0.1
        _OutlineColor ("Outline Color", Vector) = (0,0,0,1)
        _LitOutlineThickness ("Lit Outline Thickness", Range(0, 1)) = 0.1
        _UnlitOutlineThickness ("Unlit Outline Thickness", Range(0, 1)) = 0.4
        _SpecColor ("Specular Color", Vector) = (1,1,1,1)
        _Shininess ("Shininess", Float) = 10
    }
    SubShader {
        Pass {
            Tags { "LIGHTMODE" = "FORWARDBASE" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2f {
                float4 position : SV_POSITION0;
                float4 texcoord : TEXCOORD0;
                float3 texcoord1 : TEXCOORD1;
            };

            struct fout {
                float4 sv_target : SV_Target0;
            };

            float4 _LightColor0;
            float4 _Color;
            float4 _UnlitColor;
            float _DiffuseThreshold;
            float4 _OutlineColor;
            float _LitOutlineThickness;
            float _UnlitOutlineThickness;
            float4 _SpecColor;
            float _Shininess;

            // Optimized Vertex Shader
            v2f vert(appdata_full v) {
                v2f o;

                // Calculate world space position
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.position = mul(UNITY_MATRIX_VP, worldPos); // Clip space position

                // Calculate the world normal
                float3 worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));

                // Pass world position and normal to fragment shader
                o.texcoord = worldPos;
                o.texcoord1 = worldNormal;

                return o;
            }

            fout frag(v2f inp) {
                fout o;
                float4 tmp0;
                float4 tmp1;
                float4 tmp2;
                float4 tmp3;

                // Existing fragment shader logic preserved
                tmp0.x = dot(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz);
                tmp0.x = rsqrt(tmp0.x);
                tmp0.xyz = tmp0.xxx * _WorldSpaceLightPos0.xyz;
                tmp1.xyz = _WorldSpaceLightPos0.xyz - inp.texcoord.xyz;
                tmp1.w = dot(tmp1.xyz, tmp1.xyz);
                tmp2.x = rsqrt(tmp1.w);
                tmp1.w = sqrt(tmp1.w);
                tmp3.w = 1.0 / tmp1.w;
                tmp3.xyz = tmp1.xyz * tmp2.xxx;
                tmp1.x = _WorldSpaceLightPos0.w == 0.0;
                tmp0.w = 1.0;
                tmp0 = tmp1.xxxx ? tmp0 : tmp3;
                tmp1.x = dot(inp.texcoord1.xyz, inp.texcoord1.xyz);
                tmp1.x = rsqrt(tmp1.x);
                tmp1.xyz = tmp1.xxx * inp.texcoord1.xyz;
                tmp1.w = dot(-tmp0.xyz, tmp1.xyz);
                tmp1.w = tmp1.w + tmp1.w;
                tmp2.xyz = tmp1.xyz * -tmp1.www + -tmp0.xyz;
                tmp3.xyz = _WorldSpaceCameraPos - inp.texcoord.xyz;
                tmp1.w = dot(tmp3.xyz, tmp3.xyz);
                tmp1.w = rsqrt(tmp1.w);
                tmp3.xyz = tmp1.www * tmp3.xyz;
                tmp1.w = dot(tmp2.xyz, tmp3.xyz);
                tmp2.x = dot(tmp3.xyz, tmp1.xyz);
                tmp0.x = dot(tmp1.xyz, tmp0.xyz);
                tmp0.y = max(tmp1.w, 0.0);
                tmp0.y = log(tmp0.y);
                tmp0.y = tmp0.y * _Shininess;
                tmp0.y = exp(tmp0.y);
                tmp0.y = tmp0.y * tmp0.w;
                tmp0.yz = tmp0.yx > float2(0.5, 0.0);
                tmp0.x = max(tmp0.x, 0.0);
                tmp0.y = tmp0.y ? tmp0.z : 0.0;
                tmp0.z = tmp0.x * tmp0.w;
                tmp0.z = tmp0.z >= _DiffuseThreshold;
                tmp1.xyz = _LightColor0.xyz * _Color.xyz;
                tmp1.xyz = tmp0.zzz ? tmp1.xyz : _UnlitColor.xyz;
                tmp0.z = _LitOutlineThickness - _UnlitOutlineThickness;
                tmp0.x = tmp0.x * tmp0.z + _UnlitOutlineThickness;
                tmp0.x = tmp2.x < tmp0.x;
                tmp2.xyz = _LightColor0.xyz * _OutlineColor.xyz;
                tmp0.xzw = tmp0.xxx ? tmp2.xyz : tmp1.xyz;
                tmp1.x = 1.0 - _SpecColor.w;
                tmp1.xyz = tmp0.xzw * tmp1.xxx;
                tmp2.xyz = _LightColor0.xyz * _SpecColor.www;
                tmp1.xyz = tmp2.xyz * _SpecColor.xyz + tmp1.xyz;
                o.sv_target.xyz = tmp0.yyy ? tmp1.xyz : tmp0.xzw;
                o.sv_target.w = 1.0;
                return o;
            }
            ENDCG
        }

        Pass {
            Tags { "LIGHTMODE" = "FORWARDADD" }
            Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2f {
                float4 position : SV_POSITION0;
                float4 texcoord : TEXCOORD0;
                float3 texcoord1 : TEXCOORD1;
            };

            struct fout {
                float4 sv_target : SV_Target0;
            };

            float4 _LightColor0;
            float4 _SpecColor;
            float _Shininess;

            // Optimized Vertex Shader for additional pass
            v2f vert(appdata_full v) {
                v2f o;

                // Calculate world space position
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.position = mul(UNITY_MATRIX_VP, worldPos); // Clip space position

                // Calculate the world normal
                float3 worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));

                // Pass world position and normal to fragment shader
                o.texcoord = worldPos;
                o.texcoord1 = worldNormal;

                return o;
            }

            fout frag(v2f inp) {
                fout o;
                float4 tmp0;
                float4 tmp1;
                float4 tmp2;
                float4 tmp3;

                // Existing fragment shader logic preserved
                tmp0.xyz = _WorldSpaceCameraPos - inp.texcoord.xyz;
                tmp0.w = dot(tmp0.xyz, tmp0.xyz);
                tmp0.w = rsqrt(tmp0.w);
                tmp0.xyz = tmp0.www * tmp0.xyz;
                tmp0.w = dot(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz);
                tmp0.w = rsqrt(tmp0.w);
                tmp1.xyz = tmp0.www * _WorldSpaceLightPos0.xyz;
                tmp2.xyz = _WorldSpaceLightPos0.xyz - inp.texcoord.xyz;
                tmp0.w = dot(tmp2.xyz, tmp2.xyz);
                tmp2.w = rsqrt(tmp0.w);
                tmp0.w = sqrt(tmp0.w);
                tmp3.w = 1.0 / tmp0.w;
                tmp3.xyz = tmp2.www * tmp2.xyz;
                tmp0.w = _WorldSpaceLightPos0.w == 0.0;
                tmp1.w = 1.0;
                tmp1 = tmp0.wwww ? tmp1 : tmp3;
                tmp0.w = dot(inp.texcoord1.xyz, inp.texcoord1.xyz);
                tmp0.w = rsqrt(tmp0.w);
                tmp2.xyz = tmp0.www * inp.texcoord1.xyz;
                tmp0.w = dot(-tmp1.xyz, tmp2.xyz);
                tmp0.w = tmp0.w + tmp0.w;
                tmp3.xyz = tmp2.xyz * -tmp0.www + -tmp1.xyz;
                tmp0.w = dot(tmp2.xyz, tmp1.xyz);
                tmp0.x = dot(tmp3.xyz, tmp0.xyz);
                tmp0.x = max(tmp0.x, 0.0);
                tmp0.x = log(tmp0.x);
                tmp0.x = tmp0.x * _Shininess;
                tmp0.x = exp(tmp0.x);
                tmp0.x = tmp0.x * tmp1.w;
                tmp0.xw = tmp0.xw > float2(0.5, 0.0);
                tmp0.x = tmp0.x ? tmp0.w : 0.0;
                tmp1.xyz = _LightColor0.xyz * _SpecColor.xyz;
                tmp1.w = _SpecColor.w;
                o.sv_target = tmp0.xxxx ? tmp1 : 0.0;
                return o;
            }
            ENDCG
        }
    }
    Fallback "Specular"
}
