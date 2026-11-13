Shader "CpRemix/World/WorldObject"
{
    Properties
    {
        _Diffuse ("Diffuse Texture", 2D) = "" {}
        [HideInInspector] _MainTex ("Main Tex", 2D) = "" {}
        [HideInInspector] _BlobShadowTex ("Blob Shadow Tex", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "ForwardLit"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_fog
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_Diffuse);
            SAMPLER(sampler_Diffuse);

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_BlobShadowTex);
            SAMPLER(sampler_BlobShadowTex);

            float _ShadowPlaneDim;
            float _ShadowTextureDim;
            float3 _ShadowPlaneWorldPos;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float4 color      : COLOR;
                float2 uv0        : TEXCOORD0;
                float2 uv1        : TEXCOORD1;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 color      : COLOR;
                float2 uv         : TEXCOORD0;
                float2 lightmapUV : TEXCOORD1;
                float3 shadowData : TEXCOORD2;
                float fogFactor   : TEXCOORD3;
                float3 normalWS   : TEXCOORD4;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Diffuse_ST;
                float4 _MainTex_ST;
                float4 _BlobShadowTex_ST;
            CBUFFER_END

            Varyings vert(Attributes v)
            {
                Varyings o;

                float3 worldPos = TransformObjectToWorld(v.positionOS.xyz);

                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);

                o.color = v.color;
                o.uv = v.uv0;

                #if defined(LIGHTMAP_ON)
                    o.lightmapUV =
                        v.uv1 * unity_LightmapST.xy +
                        unity_LightmapST.zw;
                #else
                    o.lightmapUV = v.uv1;
                #endif

                float halfDim =
                    max(_ShadowPlaneDim * 0.5, 0.0001);

                float aspectOfs =
                    1.0 / max(_ShadowTextureDim, 0.0001);

                float offsetX =
                    worldPos.x - _ShadowPlaneWorldPos.x;

                float offsetZ =
                    worldPos.z - _ShadowPlaneWorldPos.z;

                o.shadowData.x =
                    (aspectOfs + offsetX / halfDim + 1.0) * 0.5;

                o.shadowData.y =
                    (aspectOfs + offsetZ / halfDim + 1.0) * 0.5;

                o.shadowData.z =
                    worldPos.y;

                o.fogFactor =
                    ComputeFogFactor(o.positionCS.z);

                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float4 shadowSample =
                    SAMPLE_TEXTURE2D(
                        _BlobShadowTex,
                        sampler_BlobShadowTex,
                        i.shadowData.xy
                    );

                float shadowDepth =
                    shadowSample.y;

                float shadowIntensity =
                    shadowSample.x;

                float isAbove =
                    (i.shadowData.z >= shadowDepth)
                    ? 2.0
                    : 1.0;

                float depthDiff =
                    shadowDepth - i.shadowData.z;

                float shadowFactor =
                    mad(
                        abs(depthDiff),
                        isAbove,
                        isAbove
                    ) - 0.5;

                float shadowMult =
                    min(
                        shadowIntensity *
                        max(shadowFactor, 1.0),
                        1.0
                    );

                float3 lightColor = 1.0;

                #if defined(LIGHTMAP_ON)

                    half3 bakedGI =
                        SampleLightmap(
                            i.lightmapUV,
                            normalize(i.normalWS)
                        );

                    lightColor = bakedGI;

                #endif

                float4 diffSample =
                    SAMPLE_TEXTURE2D(
                        _Diffuse,
                        sampler_Diffuse,
                        i.uv
                    );

                float4 mainSample =
                    SAMPLE_TEXTURE2D(
                        _MainTex,
                        sampler_MainTex,
                        i.uv
                    );

                float4 diff =
                    diffSample.a > 0.0
                        ? diffSample
                        : mainSample;

                float3 col =
                    diff.rgb *
                    lightColor *
                    i.color.rgb *
                    shadowMult;

                float4 final =
                    float4(col, 1.0);

                final.rgb =
                    MixFog(
                        final.rgb,
                        i.fogFactor
                    );

                return final;
            }

            ENDHLSL
        }
    }
}
