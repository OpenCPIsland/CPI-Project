Shader "Custom/OutlineSilouetted Diffuse" {
    Properties {
        _Color ("Color", Vector) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0, 1)) = 0.5
        _Metallic ("Metallic", Range(0, 1)) = 0
    }
    SubShader {
        LOD 200
        Tags { "RenderType" = "Opaque" }
        Pass {
            Name "FORWARD"
            LOD 200
            Tags { "LIGHTMODE" = "FORWARDBASE" "RenderType" = "Opaque" "SHADOWSUPPORT" = "true" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct v2f {
                float4 position : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            float4 _MainTex_ST;
            float4 _LightColor0;
            float _Glossiness;
            float _Metallic;
            float4 _Color;
            sampler2D _MainTex;

            v2f vert(appdata_full v) {
                v2f o;
                // Transform vertex to world space
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldPos = worldPos.xyz;

                // Transform vertex to clip space
                o.position = UnityObjectToClipPos(v.vertex);

                // Pass through UVs
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                // Transform normal to world space
                o.worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
                return o;
            }

            float4 frag(v2f i) : SV_Target {
                // Sample main texture and apply color tint
                float4 albedo = tex2D(_MainTex, i.texcoord) * _Color;

                // Simple Lambertian reflection model
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
                float diff = max(dot(i.worldNormal, lightDir), 0.0);
                float3 diffuse = diff * _LightColor0.rgb * albedo.rgb;

                // Combine diffuse with albedo for final color
                return float4(diffuse, albedo.a);
            }
            ENDCG
        }

        Pass {
            Name "FORWARDADD"
            LOD 200
            Tags { "LIGHTMODE" = "FORWARDADD" "RenderType" = "Opaque" "SHADOWSUPPORT" = "true" }
            Blend One One, One One
            ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f {
                float4 position : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            float4 _MainTex_ST;
            float4 _LightColor0;
            float _Glossiness;
            float _Metallic;
            float4 _Color;
            sampler2D _MainTex;

            v2f vert(appdata_full v) {
                v2f o;
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldPos = worldPos.xyz;
                o.position = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
                return o;
            }

            float4 frag(v2f i) : SV_Target {
                float4 albedo = tex2D(_MainTex, i.texcoord) * _Color;
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
                float diff = max(dot(i.worldNormal, lightDir), 0.0);
                float3 diffuse = diff * _LightColor0.rgb * albedo.rgb;
                return float4(diffuse, albedo.a);
            }
            ENDCG
        }
        
        Pass {
            Name "DEFERRED"
            LOD 200
            Tags { "LIGHTMODE" = "DEFERRED" "RenderType" = "Opaque" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f {
                float4 position : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            float4 _MainTex_ST;
            float4 _Color;
            float _Glossiness;
            float _Metallic;
            sampler2D _MainTex;

            v2f vert(appdata_full v) {
                v2f o;
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldPos = worldPos.xyz;
                o.position = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
                return o;
            }

            float4 frag(v2f i) : SV_Target {
                float4 albedo = tex2D(_MainTex, i.texcoord) * _Color;
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
                float diff = max(dot(i.worldNormal, lightDir), 0.0);
                float3 diffuse = diff * albedo.rgb;
                return float4(diffuse, albedo.a);
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}
