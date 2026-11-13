Shader "CpRemix/Avatar Body Preview"
{
    Properties
    {
        _Diffuse ("Diffuse", 2D) = "black" {}
        _BodyColorsMaskTex ("Body Color Mask", 2D) = "black" {}
        _BodyRedChannelColor ("Body Red Channel Color", Color) = (1,0,0,1)
        _BodyGreenChannelColor ("Body Green Channel Color", Color) = (1,1,0,1)
        _BodyBlueChannelColor ("Body Blue Channel Color", Color) = (1,0,1,1)
        _DetailAndMatcapMaskAndEmissive ("r=detail g=MatCapMask b=emissive", 2D) = "black" {}
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
            TEXTURE2D(_BodyColorsMaskTex);
            SAMPLER(sampler_BodyColorsMaskTex);
            TEXTURE2D(_DetailAndMatcapMaskAndEmissive);
            SAMPLER(sampler_DetailAndMatcapMaskAndEmissive);

            CBUFFER_START(UnityPerMaterial)
                float4 _BodyRedChannelColor;
                float4 _BodyGreenChannelColor;
                float4 _BodyBlueChannelColor;
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
                float3 lighting : TEXCOORD1;
                float4 shadowCoord : TEXCOORD2;
                float3 positionWS : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings vert(Attributes input)
            {
                UNITY_SETUP_INSTANCE_ID(input);
                Varyings output;
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                float3 normalWS = normalize(TransformObjectToWorldNormal(input.normalOS));
                Light mainLight = GetMainLight();
                float ndotl = max(dot(normalWS, mainLight.direction), 0.0);

                output.positionCS = positionInputs.positionCS;
                output.uv = input.uv;
                output.positionWS = positionInputs.positionWS;
                output.lighting = ndotl * mainLight.color * 0.75;
                output.shadowCoord = GetShadowCoord(positionInputs);
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);

                float4 mask = SAMPLE_TEXTURE2D(_BodyColorsMaskTex, sampler_BodyColorsMaskTex, input.uv);
                float4 diffuse = SAMPLE_TEXTURE2D(_Diffuse, sampler_Diffuse, input.uv);
                float detail = SAMPLE_TEXTURE2D(_DetailAndMatcapMaskAndEmissive, sampler_DetailAndMatcapMaskAndEmissive, input.uv).r;

                float3 colorFromMask =
                    mask.r * _BodyRedChannelColor.rgb +
                    mask.g * _BodyGreenChannelColor.rgb +
                    mask.b * _BodyBlueChannelColor.rgb;
                float maxMask = max(mask.r, max(mask.g, mask.b));
                float3 bodyColor = diffuse.rgb * (1.0 - maxMask) + colorFromMask * maxMask;

                Light mainLight = GetMainLight(input.shadowCoord, input.positionWS, half4(1.0, 1.0, 1.0, 1.0));
                float3 ambient = unity_AmbientSky.rgb * 0.45;
                float3 lighting = ambient + input.lighting * mainLight.shadowAttenuation;
                return float4(bodyColor * detail * lighting * 0.94, 1.0);
            }
            ENDHLSL
        }
    }
}
