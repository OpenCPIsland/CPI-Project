Shader "CpRemix/Combined Avatar Depth"
{
    Properties
    {
        _MainTex ("Diffuse Texture", 2D) = "white" {}
        _Diffuse ("Diffuse", 2D) = "black" {}
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
        [HideInInspector] _UseEquipmentTextures ("Use Equipment Textures", Float) = 0
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry+99" "RenderType" = "Opaque" }

        Pass
        {
            Tags { "LightMode" = "UniversalForwardOnly" }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.0
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
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
            TEXTURE2D(_SurfaceReflectionsRGB);
            SAMPLER(sampler_SurfaceReflectionsRGB);

            float4 _Decal1Color;
            float4 _Decal2Color;
            float4 _Decal3Color;
            float4 _Decal4Color;
            float4 _Decal5Color;
            float4 _Decal6Color;
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
            float _UseEquipmentTextures;

            float3 _BodyRedChannelColor;
            float3 _BodyGreenChannelColor;
            float3 _BodyBlueChannelColor;

            float  _SurfaceYCoord;
            float  _DeepestYCoord;
            float3 _DepthColor;
            float3 _SurfaceReflectionColor;
            float  _DynSurfaceTexTile;
            float  _DynSurfaceMultiplier;
            float  _SurfaceVelocityX;
            float  _SurfaceVelocityZ;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv     : TEXCOORD0;
                float3 color  : COLOR;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos         : SV_POSITION;
                float2 uvMain      : TEXCOORD0;
                float2 uvSurface   : TEXCOORD5;
                float3 lighting    : TEXCOORD1;
                float3 color       : COLOR;
                float3 depthColor  : TEXCOORD3;
                float3 reflection  : TEXCOORD4;

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

            v2f vert(appdata v)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v2f o;
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                float3 worldPos = TransformObjectToWorld(v.vertex.xyz);
                o.pos = TransformWorldToHClip(worldPos);
                o.color = v.color;

                float velX = _SurfaceVelocityX * _Time.x;
                float velZ = _SurfaceVelocityZ * _Time.x;
                o.uvSurface.x = mad(worldPos.x, _DynSurfaceTexTile, -velX);
                o.uvSurface.y = mad(worldPos.z, _DynSurfaceTexTile, -velZ);

                o.uvMain = v.uv;

                float3 worldNormal = normalize(TransformObjectToWorldNormal(v.normal));
                Light mainLight = GetMainLight();
                float NdotL = max(dot(worldNormal, mainLight.direction), 0.0);
                float3 ambient = unity_AmbientSky.rgb * 0.45;
                float3 diffuse = NdotL * mainLight.color;

                float wrap = (NdotL + 0.5) * 0.6;

                o.lighting.x = max(wrap, mad(diffuse.x, 0.75, ambient.x));
                o.lighting.y = max(wrap, mad(diffuse.y, 0.75, ambient.y));
                o.lighting.z = max(wrap, mad(diffuse.z, 0.75, ambient.z));

                float depthRange  = _SurfaceYCoord - _DeepestYCoord;
                float depthT      = clamp((worldPos.y - _DeepestYCoord * 2.0 + _SurfaceYCoord) / (_SurfaceYCoord - _DeepestYCoord * 2.0 + _SurfaceYCoord), 0.0, 1.0);
                float invDepthT   = 1.0 - depthT;
                float invInvDepth = 1.0 - invDepthT;

                o.depthColor.x = mad(_DepthColor.x, invDepthT, invInvDepth);
                o.depthColor.y = mad(_DepthColor.y, invDepthT, invInvDepth);
                o.depthColor.z = mad(_DepthColor.z, invDepthT, invInvDepth);

                float aboveSurface = (0.0 < (worldPos.y - _SurfaceYCoord)) ? (worldPos.y - _SurfaceYCoord) : 0.0;
                float normalUpSq   = worldNormal.y * worldNormal.y;
                float normalUpMask = (0.0 < worldNormal.y) ? normalUpSq : 0.0;
                float reflStrength = min(aboveSurface, 1.0) * normalUpMask * invInvDepth * _DynSurfaceMultiplier * 0.5;

                o.reflection.x = reflStrength * _SurfaceReflectionColor.x;
                o.reflection.y = reflStrength * _SurfaceReflectionColor.y;
                o.reflection.z = reflStrength * _SurfaceReflectionColor.z;

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 main = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uvMain);
                float4 surf = SAMPLE_TEXTURE2D(_SurfaceReflectionsRGB, sampler_SurfaceReflectionsRGB, i.uvSurface);
                float4 bodyDetail = SAMPLE_TEXTURE2D(_DetailAndMatcapMaskAndEmissive, sampler_DetailAndMatcapMaskAndEmissive, i.uvMain);
                float4 bodyMask = SAMPLE_TEXTURE2D(_BodyColorsMaskTex, sampler_BodyColorsMaskTex, i.uvMain);
                float4 bodyDiff = SAMPLE_TEXTURE2D(_Diffuse, sampler_Diffuse, i.uvMain);

                if (_UseEquipmentTextures > 0.5)
                {
                    float3 opacity = SAMPLE_TEXTURE2D(_Decal123OpacityTex, sampler_Decal123OpacityTex, i.uvMain).rgb;
                    float2 uv1 = RotateDecalUV(i.uvMain, _Decal1UOffset, _Decal1VOffset, _Decal1Scale, _Decal1RotationRads);
                    float2 uv2 = RotateDecalUV(i.uvMain, _Decal2UOffset, _Decal2VOffset, _Decal2Scale, _Decal2RotationRads);
                    float2 uv3 = RotateDecalUV(i.uvMain, _Decal3UOffset, _Decal3VOffset, _Decal3Scale, _Decal3RotationRads);
                    float2 uv4 = RotateDecalUV(i.uvMain, _Decal4UOffset, _Decal4VOffset, _Decal4Scale, _Decal4RotationRads);
                    float2 uv5 = RotateDecalUV(i.uvMain, _Decal5UOffset, _Decal5VOffset, _Decal5Scale, _Decal5RotationRads);
                    float2 uv6 = RotateDecalUV(i.uvMain, _Decal6UOffset, _Decal6VOffset, _Decal6Scale, _Decal6RotationRads);
                    float4 d1 = SAMPLE_TEXTURE2D(_Decal1Tex, sampler_Decal1Tex, uv1) * DecalMask(uv1, _Decal1Repeat);
                    float4 d2 = SAMPLE_TEXTURE2D(_Decal2Tex, sampler_Decal2Tex, uv2) * DecalMask(uv2, _Decal2Repeat);
                    float4 d3 = SAMPLE_TEXTURE2D(_Decal3Tex, sampler_Decal3Tex, uv3) * DecalMask(uv3, _Decal3Repeat);
                    float4 d4 = SAMPLE_TEXTURE2D(_Decal4Tex, sampler_Decal4Tex, uv4) * DecalMask(uv4, _Decal4Repeat);
                    float4 d5 = SAMPLE_TEXTURE2D(_Decal5Tex, sampler_Decal5Tex, uv5) * DecalMask(uv5, _Decal5Repeat);
                    float4 d6 = SAMPLE_TEXTURE2D(_Decal6Tex, sampler_Decal6Tex, uv6) * DecalMask(uv6, _Decal6Repeat);

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
                    bodyDiff.rgb = bodyDiff.rgb * (1.0 - coverage) + decalColor * coverage;
                }

                float3 bodyColorFromMask;
                bodyColorFromMask.x = mad(bodyMask.z, _BodyBlueChannelColor.x, mad(bodyMask.x, _BodyRedChannelColor.x, bodyMask.y * _BodyGreenChannelColor.x));
                bodyColorFromMask.y = mad(bodyMask.z, _BodyBlueChannelColor.y, mad(bodyMask.x, _BodyRedChannelColor.y, bodyMask.y * _BodyGreenChannelColor.y));
                bodyColorFromMask.z = mad(bodyMask.z, _BodyBlueChannelColor.z, mad(bodyMask.x, _BodyRedChannelColor.z, bodyMask.y * _BodyGreenChannelColor.z));
                float bodyMaskMax = max(bodyMask.z, max(bodyMask.y, bodyMask.x));
                float3 bodyColor = bodyDiff.rgb * (1.0 - bodyMaskMax) + bodyMaskMax * bodyColorFromMask;
                float3 sourceColor = (_UseEquipmentTextures > 0.5)
                    ? bodyColor
                    : ((bodyMaskMax > 0.0) ? bodyColor : main.rgb);
                float sourceAlpha = (_UseEquipmentTextures > 0.5)
                    ? 0.0
                    : ((bodyMaskMax > 0.0) ? bodyDiff.a : main.a);

                if (bodyMaskMax > 0.0 && _UseEquipmentTextures < 0.5)
                {
                    float3 previewColor = bodyColor * bodyDetail.x * i.lighting * 2;
                    float3 depthTint = lerp(1.0.xxx, i.depthColor, 0.1);
                    previewColor *= depthTint;
                    return float4(previewColor, 1.0);
                }

                float  blend     = mad(sourceAlpha, 2.0, -1.0);
                float  mask      = (blend >= 0.0) ? 1.0 : 0.0;
                float  litBlend  = blend * mask;
                float  rawBlend  = mad(-blend, mask, 1.0);

                float3 litColor  = sourceColor * litBlend;
                float3 rawColor  = sourceColor * i.lighting * rawBlend;
                float3 combined  = rawColor + litColor;

                float  surfRed   = (_UseEquipmentTextures > 0.5) ? 0.0 : surf.x;

                float4 result;
                result.x = mad(combined.x, i.depthColor.x, surfRed * i.reflection.x);
                result.y = mad(combined.y, i.depthColor.y, surfRed * i.reflection.y);
                result.z = mad(combined.z, i.depthColor.z, surfRed * i.reflection.z);
                result.w = 1.0;

                return result;
            }

            ENDHLSL
        }
    }
}
