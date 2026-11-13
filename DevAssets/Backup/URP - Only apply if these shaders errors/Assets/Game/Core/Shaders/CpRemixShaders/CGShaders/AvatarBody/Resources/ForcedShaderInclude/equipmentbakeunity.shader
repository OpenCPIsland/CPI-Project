Shader "CpRemix/Equipment Bake"
{
    Properties
    {
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
        _EmissiveColorTint ("EmissiveColorTint", Color) = (1,1,1,1)
        _DetailAndMatcapMaskAndEmissive ("r=detail g=matcap b=emissive", 2D) = "black" {}
        _AtlasOffsetU ("AtlasOffset U", Float) = 0
        _AtlasOffsetV ("AtlasOffset U", Float) = 0
        _AtlasOffsetScaleU ("AtlasOffset U Scale", Float) = 1
        _AtlasOffsetScaleV ("AtlasOffset V Scale", Float) = 1
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Blend One One, One One
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.0
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float _Decal1Scale;
            float _Decal1UOffset;
            float _Decal1VOffset;
            float _Decal1RotationRads;
            float _Decal2Scale;
            float _Decal2UOffset;
            float _Decal2VOffset;
            float _Decal2RotationRads;
            float _Decal3Scale;
            float _Decal3UOffset;
            float _Decal3VOffset;
            float _Decal3RotationRads;
            float _Decal4Scale;
            float _Decal4UOffset;
            float _Decal4VOffset;
            float _Decal4RotationRads;
            float _Decal5Scale;
            float _Decal5UOffset;
            float _Decal5VOffset;
            float _Decal5RotationRads;
            float _Decal6Scale;
            float _Decal6UOffset;
            float _Decal6VOffset;
            float _Decal6RotationRads;
            float _AtlasOffsetU;
            float _AtlasOffsetV;
            float _AtlasOffsetScaleU;
            float _AtlasOffsetScaleV;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

                #ifdef UNITY_INSTANCING_ENABLED
                UNITY_VERTEX_INPUT_INSTANCE_ID
                #endif
            };

            struct v2f
            {
                float2 uvDecal1 : TEXCOORD0;
                float2 uvAtlas  : TEXCOORD1;
                float2 uvDecal2 : TEXCOORD2;
                float2 uvDecal3 : TEXCOORD3;
                float2 uvDecal4 : TEXCOORD4;
                float2 uvDecal5 : TEXCOORD5;
                float2 uvDecal6 : TEXCOORD6;
                float4 pos : SV_POSITION;

                #ifdef UNITY_INSTANCING_ENABLED
                UNITY_VERTEX_INPUT_INSTANCE_ID
                #endif
            };

            float2 RotateDecalUV(float2 baseUV, float uOffset, float vOffset, float scale, float rotation)
            {
                float pivotU = uOffset - 0.5;
                float pivotV = vOffset - 0.5;
                float s = sin(rotation);
                float c = cos(rotation);
                float tx = pivotU + baseUV.x;
                float ty = pivotV + baseUV.y;
                float rx = -pivotU + (tx * c + ty * s);
                float ry = -pivotV + (-tx * s + ty * c);
                float2 result = float2(rx + uOffset - 0.5, ry + vOffset - 0.5);
                return result * scale + 0.5;
            }

            v2f vert(appdata v)
            {
                #ifdef UNITY_INSTANCING_ENABLED
                UNITY_SETUP_INSTANCE_ID(v);
                #endif
                v2f o;
                #ifdef UNITY_INSTANCING_ENABLED
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                #endif

                o.pos = TransformObjectToHClip(v.vertex.xyz);

                float2 uvAtlas;
                uvAtlas.x = (v.uv.x - _AtlasOffsetU) / max(_AtlasOffsetScaleU, 0.0001);
                uvAtlas.y = (v.uv.y - _AtlasOffsetV) / max(_AtlasOffsetScaleV, 0.0001);
                o.uvAtlas = uvAtlas;

                o.uvDecal1 = RotateDecalUV(uvAtlas, _Decal1UOffset, _Decal1VOffset, _Decal1Scale, _Decal1RotationRads);
                o.uvDecal2 = RotateDecalUV(uvAtlas, _Decal2UOffset, _Decal2VOffset, _Decal2Scale, _Decal2RotationRads);
                o.uvDecal3 = RotateDecalUV(uvAtlas, _Decal3UOffset, _Decal3VOffset, _Decal3Scale, _Decal3RotationRads);
                o.uvDecal4 = RotateDecalUV(uvAtlas, _Decal4UOffset, _Decal4VOffset, _Decal4Scale, _Decal4RotationRads);
                o.uvDecal5 = RotateDecalUV(uvAtlas, _Decal5UOffset, _Decal5VOffset, _Decal5Scale, _Decal5RotationRads);
                o.uvDecal6 = RotateDecalUV(uvAtlas, _Decal6UOffset, _Decal6VOffset, _Decal6Scale, _Decal6RotationRads);

                return o;
            }

            Texture2D _Diffuse;
            SamplerState sampler_Diffuse;
            Texture2D _Decal123OpacityTex;
            SamplerState sampler_Decal123OpacityTex;
            Texture2D _Decal1Tex;
            SamplerState sampler_Decal1Tex;
            Texture2D _Decal2Tex;
            SamplerState sampler_Decal2Tex;
            Texture2D _Decal3Tex;
            SamplerState sampler_Decal3Tex;
            Texture2D _Decal4Tex;
            SamplerState sampler_Decal4Tex;
            Texture2D _Decal5Tex;
            SamplerState sampler_Decal5Tex;
            Texture2D _Decal6Tex;
            SamplerState sampler_Decal6Tex;

            float3 _Decal1Color;
            float _Decal1Repeat;
            float3 _Decal2Color;
            float _Decal2Repeat;
            float3 _Decal3Color;
            float _Decal3Repeat;
            float3 _Decal4Color;
            float _Decal4Repeat;
            float3 _Decal5Color;
            float _Decal5Repeat;
            float3 _Decal6Color;
            float _Decal6Repeat;

            float DecalMask(float2 uv, float repeatToggle)
            {
                float2 c = abs((uv - 0.5) * 2.0);
                float boundary = repeatToggle * 255.0 + 1.0;
                return boundary >= max(c.x, c.y) ? 1.0 : 0.0;
            }

            float4 frag(v2f i) : SV_Target
            {
                float diffuseMask = DecalMask(i.uvAtlas, 0.0);
                float4 diffuseSample = _Diffuse.Sample(sampler_Diffuse, i.uvAtlas);
                float3 opacity123 = _Decal123OpacityTex.Sample(sampler_Decal123OpacityTex, i.uvAtlas).rgb;
                float op1 = opacity123.r;
                float op2 = opacity123.g;
                float op3 = opacity123.b;

                float4 d1 = _Decal1Tex.Sample(sampler_Decal1Tex, i.uvDecal1) * DecalMask(i.uvDecal1, _Decal1Repeat);
                float4 d2 = _Decal2Tex.Sample(sampler_Decal2Tex, i.uvDecal2) * DecalMask(i.uvDecal2, _Decal2Repeat);
                float4 d3 = _Decal3Tex.Sample(sampler_Decal3Tex, i.uvDecal3) * DecalMask(i.uvDecal3, _Decal3Repeat);
                float4 d4 = _Decal4Tex.Sample(sampler_Decal4Tex, i.uvDecal4) * DecalMask(i.uvDecal4, _Decal4Repeat);
                float4 d5 = _Decal5Tex.Sample(sampler_Decal5Tex, i.uvDecal5) * DecalMask(i.uvDecal5, _Decal5Repeat);
                float4 d6 = _Decal6Tex.Sample(sampler_Decal6Tex, i.uvDecal6) * DecalMask(i.uvDecal6, _Decal6Repeat);

                float3 d1Tinted = d1.rgb * _Decal1Color;
                float3 d2Tinted = d2.rgb * _Decal2Color;
                float3 d3Tinted = d3.rgb * _Decal3Color;
                float3 d4Tinted = d4.rgb * _Decal4Color;
                float3 d5Tinted = d5.rgb * _Decal5Color;
                float3 d6Tinted = d6.rgb * _Decal6Color;

                float a1 = d1.a * op1;
                float a2 = d2.a * op2;
                float a3 = d3.a * op3;
                float a4 = d4.a * op1;
                float a5 = d5.a * op2;
                float a6 = d6.a * op3;

                float bg6 = mad(-d6.a, op3, 1.0);
                float a5Over6 = bg6 * a5;
                float a4Over56 = mad(-a5Over6, bg6, 1.0) * a4;
                float bgAfter456 = bg6 * a4Over56;

                float coverage456 = min(mad(a4Over56, bg6, mad(d6.a, op3, a5Over6)), 1.0);
                float oneMinus456 = 1.0 - coverage456;

                float w2r = a2 * d2Tinted.r;
                float w2g = a2 * d2Tinted.g;
                float w2b = a2 * d2Tinted.b;

                float blend123R = mad(d3Tinted.r, a3, mad(d1Tinted.r, a1, w2r)) * oneMinus456;
                float blend123G = mad(d3Tinted.g, a3, mad(d1Tinted.g, a1, w2g)) * oneMinus456;
                float blend123B = mad(d3Tinted.b, a3, mad(d1Tinted.b, a1, w2b)) * oneMinus456;

                float w5r = a5Over6 * d5Tinted.r;
                float w5g = a5Over6 * d5Tinted.g;
                float w5b = a5Over6 * d5Tinted.b;

                float coverageAll = min(min(mad(d1.a, op1, mad(d3.a, op3, a2)), 1.0) + coverage456, 1.0);

                float blend456R = coverageAll * (mad(mad(d6Tinted.r, a6, mad(d4Tinted.r, bgAfter456, w5r)), coverage456, blend123R));
                float blend456G = coverageAll * (mad(mad(d6Tinted.g, a6, mad(d4Tinted.g, bgAfter456, w5g)), coverage456, blend123G));
                float blend456B = coverageAll * (mad(mad(d6Tinted.b, a6, mad(d4Tinted.b, bgAfter456, w5b)), coverage456, blend123B));

                float backgroundWeight = 1.0 - coverageAll;

                float4 result;
                result.r = diffuseMask * mad(diffuseSample.r, backgroundWeight, blend456R);
                result.g = diffuseMask * mad(diffuseSample.g, backgroundWeight, blend456G);
                result.b = diffuseMask * mad(diffuseSample.b, backgroundWeight, blend456B);
                result.a = diffuseMask;
                return result;
            }
            ENDHLSL
        }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.0

            float _AtlasOffsetU;
            float _AtlasOffsetV;
            float _AtlasOffsetScaleU;
            float _AtlasOffsetScaleV;
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.uv.x = (v.uv.x - _AtlasOffsetU) / max(_AtlasOffsetScaleU, 0.0001);
                o.uv.y = (v.uv.y - _AtlasOffsetV) / max(_AtlasOffsetScaleV, 0.0001);
                return o;
            }

            Texture2D _BodyColorsMaskTex;
            SamplerState sampler_BodyColorsMaskTex;
            float3 _BodyRedChannelColor;
            float3 _BodyGreenChannelColor;
            float3 _BodyBlueChannelColor;

            float4 frag(v2f i) : SV_Target
            {
                float2 c = abs((i.uv - 0.5) * 2.0);
                float boundsMask = 1.0 >= max(c.x, c.y) ? 1.0 : 0.0;

                float3 mask = _BodyColorsMaskTex.Sample(sampler_BodyColorsMaskTex, i.uv).rgb;

                float4 result;
                result.rgb = boundsMask * (mask.r * _BodyRedChannelColor + mask.g * _BodyGreenChannelColor + mask.b * _BodyBlueChannelColor);
                result.a = boundsMask * max(mask.r, max(mask.g, mask.b));
                return result;
            }
            ENDHLSL
        }
        Pass
        {
            Blend Zero SrcColor, Zero SrcColor
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            float _AtlasOffsetU;
            float _AtlasOffsetV;
            float _AtlasOffsetScaleU;
            float _AtlasOffsetScaleV;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uvAtlas : TEXCOORD0;
                float3 lighting : TEXCOORD1;
                float3 color : COLOR;
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;

                float3 worldPos = TransformObjectToWorld(v.vertex.xyz);
                o.pos = TransformWorldToHClip(worldPos);

                float3 worldNormal = normalize(TransformObjectToWorldNormal(v.normal.xyz));
                Light mainLight = GetMainLight();
                float ndotl = max(dot(worldNormal, mainLight.direction), 0.0);

                float3 diffuse = ndotl * mainLight.color;
                float3 ambient = unity_AmbientSky.rgb * 0.45;
                o.lighting = mad(diffuse, 0.65, ambient);

                o.uvAtlas.x = (v.uv.x - _AtlasOffsetU) / max(_AtlasOffsetScaleU, 0.0001);
                o.uvAtlas.y = (v.uv.y - _AtlasOffsetV) / max(_AtlasOffsetScaleV, 0.0001);

                o.color = v.color.xyz;

                return o;
            }

            Texture2D _DetailAndMatcapMaskAndEmissive;
            SamplerState sampler_DetailAndMatcapMaskAndEmissive;
            float3 _EmissiveColorTint;

            float4 frag(v2f i) : SV_Target
            {
                float2 c = abs((i.uvAtlas - 0.5) * 2.0);
                bool inBounds = 1.0 >= max(c.x, c.y);
                float boundsMask = inBounds ? 1.0 : 0.0;
                float outsideWeight = inBounds ? 0.0 : 1.0;

                float4 tex = _DetailAndMatcapMaskAndEmissive.Sample(sampler_DetailAndMatcapMaskAndEmissive, i.uvAtlas);
                float detail = tex.r;
                float emissive = tex.b;
                bool emissiveOverflow = 127.0 < emissive;

                float overflowTerm = (emissiveOverflow ? 1.0 : 0.0) * mad(emissive, 0.5, 0.5);
                float overflowGate = emissiveOverflow ? 0.0 : 1.0;

                float oneMinusEmissive = 1.0 - emissive;
                float emissive2 = emissive + emissive;

                float4 result;
                result.r = mad(mad(detail, oneMinusEmissive, emissive2 * _EmissiveColorTint.r), boundsMask, outsideWeight);
                result.g = mad(mad(detail, oneMinusEmissive, emissive2 * _EmissiveColorTint.g), boundsMask, outsideWeight);
                result.b = mad(mad(detail, oneMinusEmissive, emissive2 * _EmissiveColorTint.b), boundsMask, outsideWeight);

                float matcapTerm = mad(-tex.g, 0.5, 0.5);
                result.a = mad(mad(matcapTerm, overflowGate, overflowTerm), boundsMask, outsideWeight);

                return result;
            }
            ENDHLSL
        }
    }
}
