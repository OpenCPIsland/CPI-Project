Shader "Hidden/ClothingOutlinerImageEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineTex ("Outline", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineLookups ("Outline Lookups", Range(1, 8)) = 3
        _OutlineLookupDistance ("Outline Lookup Distance", Float) = 0.01
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent" "RenderType" = "Transparent" }
        Pass
        {
            Blend One One
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_OutlineTex);
            SAMPLER(sampler_OutlineTex);
            float4 _OutlineColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_Position;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            static const float3 LUM = float3(0.2125999927520751953125f, 0.715200006961822509765625f, 0.072200000286102294921875f);

            float lum(float3 c) { return dot(abs(c), LUM); }

            float4 frag(v2f i) : SV_Target0
            {
                float outlineStrength = 0.0f;

                if (i.uv.x > 0.125f && i.uv.x < 0.875f && i.uv.y > 0.125f && i.uv.y < 0.875f)
                {
                    float3 center = SAMPLE_TEXTURE2D(_OutlineTex, sampler_OutlineTex, i.uv).xyz;

                    float2 o1 = float2(0.00390625f, 0.00390625f);
                    float2 o2 = float2(0.01171875f, 0.01171875f);

                    float3 d0 = SAMPLE_TEXTURE2D(_OutlineTex, sampler_OutlineTex, i.uv + float2(-o1.x, -o1.y)).xyz - center;
                    float3 d1 = SAMPLE_TEXTURE2D(_OutlineTex, sampler_OutlineTex, i.uv + float2(-o1.x,  0.0f)).xyz - center;
                    float3 d2 = SAMPLE_TEXTURE2D(_OutlineTex, sampler_OutlineTex, i.uv + float2(-o1.x,  o1.y)).xyz - center;
                    float3 d3 = SAMPLE_TEXTURE2D(_OutlineTex, sampler_OutlineTex, i.uv + float2( o1.x, -o1.y)).xyz - center;
                    float3 d4 = SAMPLE_TEXTURE2D(_OutlineTex, sampler_OutlineTex, i.uv + float2( o1.x,  0.0f)).xyz - center;
                    float3 d5 = SAMPLE_TEXTURE2D(_OutlineTex, sampler_OutlineTex, i.uv + float2( o1.x,  o1.y)).xyz - center;
                    float3 d6 = SAMPLE_TEXTURE2D(_OutlineTex, sampler_OutlineTex, i.uv + float2( 0.0f,  o1.y)).xyz - center;
                    float3 d7 = SAMPLE_TEXTURE2D(_OutlineTex, sampler_OutlineTex, i.uv + float2( 0.0f, -o1.y)).xyz - center;

                    float3 e0 = SAMPLE_TEXTURE2D(_OutlineTex, sampler_OutlineTex, i.uv + float2(-o2.x, -o2.y)).xyz - center;
                    float3 e1 = SAMPLE_TEXTURE2D(_OutlineTex, sampler_OutlineTex, i.uv + float2(-o2.x,  0.0f)).xyz - center;
                    float3 e2 = SAMPLE_TEXTURE2D(_OutlineTex, sampler_OutlineTex, i.uv + float2(-o2.x,  o2.y)).xyz - center;
                    float3 e3 = SAMPLE_TEXTURE2D(_OutlineTex, sampler_OutlineTex, i.uv + float2( o2.x, -o2.y)).xyz - center;
                    float3 e4 = SAMPLE_TEXTURE2D(_OutlineTex, sampler_OutlineTex, i.uv + float2( o2.x,  0.0f)).xyz - center;
                    float3 e5 = SAMPLE_TEXTURE2D(_OutlineTex, sampler_OutlineTex, i.uv + float2( o2.x,  o2.y)).xyz - center;
                    float3 e6 = SAMPLE_TEXTURE2D(_OutlineTex, sampler_OutlineTex, i.uv + float2( 0.0f,  o2.y)).xyz - center;
                    float3 e7 = SAMPLE_TEXTURE2D(_OutlineTex, sampler_OutlineTex, i.uv + float2( 0.0f, -o2.y)).xyz - center;

                    float strength =
                        lum(d0) * 0.7070000171661376953125f +
                        lum(d1) * 0.7070000171661376953125f +
                        lum(d2) * 0.7070000171661376953125f +
                        lum(d3) * 0.7070000171661376953125f +
                        lum(d4) * 0.7070000171661376953125f +
                        lum(d5) * 0.7070000171661376953125f +
                        lum(d6) * 0.7070000171661376953125f +
                        lum(d7) * 0.7070000171661376953125f +
                        lum(e0) * 0.3030000030994415283203125f +
                        lum(e1) * 0.5f +
                        lum(e2) * 0.3030000030994415283203125f +
                        lum(e3) * 0.3030000030994415283203125f +
                        lum(e4) * 0.5f +
                        lum(e5) * 0.3030000030994415283203125f +
                        lum(e6) * 0.5f +
                        lum(e7) * 0.5f;

                    outlineStrength = strength * 0.16666667163372039794921875f;
                }

                return float4(_OutlineColor.rgb * outlineStrength * i.color.rgb, 0.0f);
            }

            ENDHLSL
        }
    }
}