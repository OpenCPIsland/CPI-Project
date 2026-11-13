Shader "CpRemix/Combined Avatar Alpha"
{
    Properties
    {
        _MainTex ("Diffuse Texture", 2D) = "white" {}
        _Alpha ("Alpha", Float) = 0
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent" "RenderType" = "Transparent" }

        Pass
        {
            Tags { "LightMode" = "UniversalForwardOnly" }
            Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.0
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float _Alpha;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                float3 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 lighting : TEXCOORD1;
                float3 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings vert(Attributes input)
            {
                UNITY_SETUP_INSTANCE_ID(input);
                Varyings output;
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, input.normalOS));
                Light mainLight = GetMainLight();
                float ndotl = max(dot(worldNormal, mainLight.direction), 0.0);
                float3 ambient = unity_AmbientSky.rgb * 0.45;
                float3 diffuse = ndotl * mainLight.color;
                float wrap = (ndotl + 0.5) * 0.6;

                output.positionCS = TransformWorldToHClip(positionWS);
                output.uv = input.uv;
                output.color = input.color;
                output.lighting = max(wrap.xxx, diffuse * 0.75 + ambient);
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                float4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                float blend = mad(tex.a, 2.0, -1.0);
                float mask = blend >= 0.0 ? 1.0 : 0.0;
                float litBlend = blend * mask;
                float rawBlend = mad(-blend, mask, 1.0);
                float3 result = tex.rgb * litBlend + tex.rgb * input.lighting * rawBlend;
                return float4(result, _Alpha);
            }
            ENDHLSL
        }
    }
}
