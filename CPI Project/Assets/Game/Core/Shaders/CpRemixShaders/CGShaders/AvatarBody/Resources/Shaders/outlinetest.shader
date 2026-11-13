Shader "Cg shader for toon shading"
{
    Properties
    {
        _Color ("Diffuse Color", Color) = (1,1,1,1)
        _UnlitColor ("Unlit Diffuse Color", Color) = (0.5,0.5,0.5,1)
        _DiffuseThreshold ("Threshold for Diffuse Colors", Range(0,1)) = 0.1
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _LitOutlineThickness ("Lit Outline Thickness", Range(0,1)) = 0.1
        _UnlitOutlineThickness ("Unlit Outline Thickness", Range(0,1)) = 0.4
        _SpecColor ("Specular Color", Color) = (1,1,1,1)
        _Shininess ("Shininess", Float) = 10
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }

        Pass
        {
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma target 4.0
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile _ _ADDITIONAL_LIGHTS

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half4 _UnlitColor;
                half _DiffuseThreshold;
                half4 _OutlineColor;
                half _LitOutlineThickness;
                half _UnlitOutlineThickness;
                half4 _SpecColor;
                half _Shininess;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                half3 normalWS : TEXCOORD1;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionHCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                return output;
            }

            half3 ApplySpecular(half3 color, half3 normalWS, half3 viewDirectionWS, Light light)
            {
                half ndotl = saturate(dot(normalWS, light.direction));
                half3 reflectionDirection = reflect(-light.direction, normalWS);
                half specular = pow(
                    saturate(dot(reflectionDirection, viewDirectionWS)),
                    max(_Shininess, 0.0001)
                ) * light.distanceAttenuation * light.shadowAttenuation;

                if (specular > 0.5 && ndotl > 0.0)
                {
                    color = lerp(color, _SpecColor.rgb * light.color, _SpecColor.a);
                }

                return color;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                half3 normalWS = normalize(input.normalWS);
                half3 viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
                Light mainLight = GetMainLight();

                half ndotl = saturate(dot(normalWS, mainLight.direction))
                    * mainLight.distanceAttenuation;
                half3 color = ndotl >= _DiffuseThreshold
                    ? _Color.rgb * mainLight.color
                    : _UnlitColor.rgb;

                half outlineThreshold = lerp(
                    _UnlitOutlineThickness,
                    _LitOutlineThickness,
                    ndotl
                );
                if (dot(viewDirectionWS, normalWS) < outlineThreshold)
                {
                    color = _OutlineColor.rgb;
                }

                color = ApplySpecular(color, normalWS, viewDirectionWS, mainLight);

                #if defined(_ADDITIONAL_LIGHTS)
                uint additionalLightsCount = GetAdditionalLightsCount();
                for (uint lightIndex = 0u; lightIndex < additionalLightsCount; ++lightIndex)
                {
                    Light light = GetAdditionalLight(lightIndex, input.positionWS);
                    color = ApplySpecular(color, normalWS, viewDirectionWS, light);
                }
                #endif

                return half4(color, 1.0);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
