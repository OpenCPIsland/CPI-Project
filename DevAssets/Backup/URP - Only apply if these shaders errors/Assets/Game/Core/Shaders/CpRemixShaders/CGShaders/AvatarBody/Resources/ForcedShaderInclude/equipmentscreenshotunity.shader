Shader "CpRemix/Equipment Screenshot"
{
    Properties
    {
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

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }

        Pass
        {
            Tags { "LightMode" = "UniversalForwardOnly" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.0
            #pragma multi_compile_instancing
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_Diffuse);
            SAMPLER(sampler_Diffuse);
            TEXTURE2D(_Decal123OpacityTex);
            SAMPLER(sampler_Decal123OpacityTex);
            TEXTURE2D(_Decal1Tex);
            SAMPLER(sampler_Decal1Tex);
            TEXTURE2D(_Decal2Tex);
            SAMPLER(sampler_Decal2Tex);
            TEXTURE2D(_Decal3Tex);
            SAMPLER(sampler_Decal3Tex);
            TEXTURE2D(_Decal4Tex);
            SAMPLER(sampler_Decal4Tex);
            TEXTURE2D(_Decal5Tex);
            SAMPLER(sampler_Decal5Tex);
            TEXTURE2D(_Decal6Tex);
            SAMPLER(sampler_Decal6Tex);
            TEXTURE2D(_BodyColorsMaskTex);
            SAMPLER(sampler_BodyColorsMaskTex);
            TEXTURE2D(_DetailAndMatcapMaskAndEmissive);
            SAMPLER(sampler_DetailAndMatcapMaskAndEmissive);

            CBUFFER_START(UnityPerMaterial)
                float4 _Decal1Color;
                float4 _Decal2Color;
                float4 _Decal3Color;
                float4 _Decal4Color;
                float4 _Decal5Color;
                float4 _Decal6Color;
                float4 _BodyRedChannelColor;
                float4 _BodyGreenChannelColor;
                float4 _BodyBlueChannelColor;
                float4 _EmissiveColorTint;
                float4 _ScreenshotBGColor;
                float _UseUV2ForDecals;
                float _Decal1Scale;
                float _Decal1UOffset;
                float _Decal1VOffset;
                float _Decal1RotationRads;
                float _Decal1Repeat;
                float _Decal2Scale;
                float _Decal2UOffset;
                float _Decal2VOffset;
                float _Decal2RotationRads;
                float _Decal2Repeat;
                float _Decal3Scale;
                float _Decal3UOffset;
                float _Decal3VOffset;
                float _Decal3RotationRads;
                float _Decal3Repeat;
                float _Decal4Scale;
                float _Decal4UOffset;
                float _Decal4VOffset;
                float _Decal4RotationRads;
                float _Decal4Repeat;
                float _Decal5Scale;
                float _Decal5UOffset;
                float _Decal5VOffset;
                float _Decal5RotationRads;
                float _Decal5Repeat;
                float _Decal6Scale;
                float _Decal6UOffset;
                float _Decal6VOffset;
                float _Decal6RotationRads;
                float _Decal6Repeat;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 decalUV1 : TEXCOORD1;
                float2 decalUV2 : TEXCOORD2;
                float2 decalUV3 : TEXCOORD3;
                float2 decalUV4 : TEXCOORD4;
                float2 decalUV5 : TEXCOORD5;
                float2 decalUV6 : TEXCOORD6;
                float3 directLighting : TEXCOORD7;
                float4 shadowCoord : TEXCOORD8;
                float3 positionWS : TEXCOORD9;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            float2 RotateDecalUV(float2 uv, float uOffset, float vOffset, float scale, float rotation)
            {
                float2 pivot = float2(uOffset - 0.5, vOffset - 0.5);
                float sine = sin(rotation);
                float cosine = cos(rotation);
                float2 rotated = mul(uv + pivot, float2x2(cosine, sine, -sine, cosine)) - pivot;
                return (rotated + float2(uOffset, vOffset) - 0.5) * scale + 0.5;
            }

            float DecalMask(float2 uv, float repeatToggle)
            {
                float2 bounds = abs((uv - 0.5) * 2.0);
                return 1.0 + 255.0 * repeatToggle >= max(bounds.x, bounds.y) ? 1.0 : 0.0;
            }

            Varyings vert(Attributes input)
            {
                UNITY_SETUP_INSTANCE_ID(input);
                Varyings output;
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                float2 decalBaseUV = input.uv;
                float3 normalWS = normalize(TransformObjectToWorldNormal(input.normalOS));
                Light mainLight = GetMainLight();
                float ndotl = max(dot(normalWS, mainLight.direction), 0.0);
                output.positionCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.shadowCoord = GetShadowCoord(positionInputs, false);
                output.directLighting = mainLight.color * ndotl * 0.65;
                output.uv = input.uv;
                output.decalUV1 = RotateDecalUV(decalBaseUV, _Decal1UOffset, _Decal1VOffset, _Decal1Scale, _Decal1RotationRads);
                output.decalUV2 = RotateDecalUV(decalBaseUV, _Decal2UOffset, _Decal2VOffset, _Decal2Scale, _Decal2RotationRads);
                output.decalUV3 = RotateDecalUV(decalBaseUV, _Decal3UOffset, _Decal3VOffset, _Decal3Scale, _Decal3RotationRads);
                output.decalUV4 = RotateDecalUV(decalBaseUV, _Decal4UOffset, _Decal4VOffset, _Decal4Scale, _Decal4RotationRads);
                output.decalUV5 = RotateDecalUV(decalBaseUV, _Decal5UOffset, _Decal5VOffset, _Decal5Scale, _Decal5RotationRads);
                output.decalUV6 = RotateDecalUV(decalBaseUV, _Decal6UOffset, _Decal6VOffset, _Decal6Scale, _Decal6RotationRads);
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                float3 opacity = SAMPLE_TEXTURE2D(_Decal123OpacityTex, sampler_Decal123OpacityTex, input.uv).rgb;
                float4 d1 = SAMPLE_TEXTURE2D(_Decal1Tex, sampler_Decal1Tex, input.decalUV1) * DecalMask(input.decalUV1, _Decal1Repeat);
                float4 d2 = SAMPLE_TEXTURE2D(_Decal2Tex, sampler_Decal2Tex, input.decalUV2) * DecalMask(input.decalUV2, _Decal2Repeat);
                float4 d3 = SAMPLE_TEXTURE2D(_Decal3Tex, sampler_Decal3Tex, input.decalUV3) * DecalMask(input.decalUV3, _Decal3Repeat);
                float4 d4 = SAMPLE_TEXTURE2D(_Decal4Tex, sampler_Decal4Tex, input.decalUV4) * DecalMask(input.decalUV4, _Decal4Repeat);
                float4 d5 = SAMPLE_TEXTURE2D(_Decal5Tex, sampler_Decal5Tex, input.decalUV5) * DecalMask(input.decalUV5, _Decal5Repeat);
                float4 d6 = SAMPLE_TEXTURE2D(_Decal6Tex, sampler_Decal6Tex, input.decalUV6) * DecalMask(input.decalUV6, _Decal6Repeat);

                float a1 = d1.a * opacity.r;
                float a2 = d2.a * opacity.g;
                float a3 = d3.a * opacity.b;
                float a4 = d4.a * opacity.r;
                float a5 = d5.a * opacity.g;
                float a6 = d6.a * opacity.b;

                float a5Over6 = a5 * (1.0 - a6);
                float a4Over56 = a4 * (1.0 - a5Over6) * (1.0 - a6);
                float coverage456 = min(a6 + a5Over6 + a4Over56, 1.0);
                float coverage123 = min(a1 + a2 + a3, 1.0);
                float coverage = min(coverage123 + coverage456, 1.0);
                float3 color123 =
                    d1.rgb * _Decal1Color.rgb * a1 +
                    d2.rgb * _Decal2Color.rgb * a2 +
                    d3.rgb * _Decal3Color.rgb * a3;
                float3 color456 =
                    d4.rgb * _Decal4Color.rgb * a4Over56 +
                    d5.rgb * _Decal5Color.rgb * a5Over6 +
                    d6.rgb * _Decal6Color.rgb * a6;
                float3 decalColor = color456 * coverage456 + color123 * (1.0 - coverage456);
                float3 baseColor = SAMPLE_TEXTURE2D(_Diffuse, sampler_Diffuse, input.uv).rgb * (1.0 - coverage) + decalColor * coverage;

                float4 bodyMask = SAMPLE_TEXTURE2D(_BodyColorsMaskTex, sampler_BodyColorsMaskTex, input.uv);
                float bodyAlpha = max(bodyMask.r, max(bodyMask.g, bodyMask.b));
                float3 bodyTint = bodyMask.r * _BodyRedChannelColor.rgb + bodyMask.g * _BodyGreenChannelColor.rgb + bodyMask.b * _BodyBlueChannelColor.rgb;
                bodyTint = bodyMask.r + bodyMask.g + bodyMask.b > 0.3 ? _BodyRedChannelColor.rgb : bodyTint;
                baseColor = lerp(baseColor, bodyTint, bodyAlpha);

                float3 detail = SAMPLE_TEXTURE2D(_DetailAndMatcapMaskAndEmissive, sampler_DetailAndMatcapMaskAndEmissive, input.uv).rgb;
                Light mainLight = GetMainLight(input.shadowCoord, input.positionWS, half4(1.0, 1.0, 1.0, 1.0), true, false);
                float3 ambient = unity_AmbientSky.rgb * 0.45;
                float3 lighting = ambient + input.directLighting * mainLight.shadowAttenuation;
                float3 litOrEmissive = lighting * detail.r * (1.0 - detail.b) + _EmissiveColorTint.rgb * detail.b;
                float maskSum = bodyMask.r + bodyMask.g + bodyMask.b;
                float3 screenshotMultiplier = maskSum > 0.3 ? 1.0.xxx : litOrEmissive;
                return float4(baseColor * screenshotMultiplier, 1.0);
            }
            ENDHLSL
        }
    }
}
