Shader "CpRemix/Avatar Body Bake"
{
    Properties
    {
        _Diffuse ("Diffuse", 2D) = "black" {}
        _BodyColorsMaskTex ("Body Color Mask", 2D) = "black" {}
        _BodyRedChannelColor ("Body Red Channel Color", Color) = (1,0,0,1)
        _BodyGreenChannelColor ("Body Green Channel Color", Color) = (1,1,0,1)
        _BodyBlueChannelColor ("Body Blue Channel Color", Color) = (1,0,1,1)
        _DetailAndMatcapMaskAndEmissive ("r=detail g=MatCapMask b=emissive", 2D) = "black" {}
        _AtlasOffsetU ("AtlasOffset U", Float) = 0
        _AtlasOffsetV ("AtlasOffset V", Float) = 0
        _AtlasOffsetScaleU ("AtlasOffset U Scale", Float) = 1
        _AtlasOffsetScaleV ("AtlasOffset V Scale", Float) = 1
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }

        Pass
        {
            Tags { "LightMode" = "SRPDefaultUnlit" }
            Blend One One, One One

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.0
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_Diffuse);
            SAMPLER(sampler_Diffuse);
            TEXTURE2D(_BodyColorsMaskTex);
            SAMPLER(sampler_BodyColorsMaskTex);
            TEXTURE2D(_DetailAndMatcapMaskAndEmissive);
            SAMPLER(sampler_DetailAndMatcapMaskAndEmissive);

            CBUFFER_START(UnityPerMaterial)
                float4 _BodyRedChannelColor;
                float4 _BodyGreenChannelColor;
                float4 _BodyBlueChannelColor;
                float _AtlasOffsetU;
                float _AtlasOffsetV;
                float _AtlasOffsetScaleU;
                float _AtlasOffsetScaleV;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings vert(Attributes input)
            {
                UNITY_SETUP_INSTANCE_ID(input);
                Varyings output;
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = float2(
                    (input.uv.x - _AtlasOffsetU) / max(_AtlasOffsetScaleU, 0.0001),
                    (input.uv.y - _AtlasOffsetV) / max(_AtlasOffsetScaleV, 0.0001));
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                float2 uv = input.uv;
                float4 mask = SAMPLE_TEXTURE2D(_BodyColorsMaskTex, sampler_BodyColorsMaskTex, uv);
                float4 detail = SAMPLE_TEXTURE2D(_DetailAndMatcapMaskAndEmissive, sampler_DetailAndMatcapMaskAndEmissive, uv);
                float4 diffuse = SAMPLE_TEXTURE2D(_Diffuse, sampler_Diffuse, uv);

                float3 colorFromMask =
                    mask.r * _BodyRedChannelColor.rgb +
                    mask.g * _BodyGreenChannelColor.rgb +
                    mask.b * _BodyBlueChannelColor.rgb;
                float maxChannel = max(mask.b, max(mask.g, mask.r));
                float3 composite = (diffuse.rgb * (1.0 - maxChannel) + maxChannel * colorFromMask) * detail.r;
                float inBounds = max(abs((uv - 0.5) * 2.0).x, abs((uv - 0.5) * 2.0).y) <= 1.0 ? 1.0 : 0.0;
                return float4(composite * inBounds, (0.5 - detail.g * 0.5) * inBounds);
            }
            ENDHLSL
        }
    }
}
