Shader "CpRemix/GPU Combined Avatar Alpha"
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
            Blend SrcAlpha OneMinusSrcAlpha

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

            float4 bonepos[48];
            float4 bonequat[48];

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                float3 color : COLOR;
                float4 tangent : TANGENT;
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

            float3 RotateByQuat(float3 value, float4 rotation)
            {
                float3 t = 2.0 * cross(rotation.xyz, value);
                return value + rotation.w * t + cross(rotation.xyz, t);
            }

            Varyings vert(Attributes input)
            {
                UNITY_SETUP_INSTANCE_ID(input);
                Varyings output;
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                float4 weights = frac(input.tangent);
                uint4 indices = (uint4)(int4)input.tangent;
                float3 worldPosition = 0.0;
                float3 worldNormal = 0.0;

                float3 p0 = RotateByQuat(input.positionOS.xyz, bonequat[indices.x]) + bonepos[indices.x].xyz;
                worldPosition += p0 * weights.x;
                worldNormal += RotateByQuat(input.normalOS, bonequat[indices.x]) * weights.x;
                worldPosition += (RotateByQuat(input.positionOS.xyz, bonequat[indices.y]) + bonepos[indices.y].xyz) * weights.y;
                worldPosition += (RotateByQuat(input.positionOS.xyz, bonequat[indices.z]) + bonepos[indices.z].xyz) * weights.z;
                worldPosition += (RotateByQuat(input.positionOS.xyz, bonequat[indices.w]) + bonepos[indices.w].xyz) * weights.w;

                float nDotL = max(dot(normalize(worldNormal), GetMainLight().direction), 0.0);
                float3 diffuse = nDotL * GetMainLight().color;
                float rim = (nDotL + 0.5) * 0.6;
                float3 ambient = unity_AmbientSky.rgb * 0.45;

                output.positionCS = TransformWorldToHClip(worldPosition);
                output.uv = input.uv;
                output.lighting = max(rim.xxx, diffuse * 0.75 + ambient);
                output.color = input.color;
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                float4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                float alphaSigned = tex.a * 2.0 - 1.0;
                float posPart = max(alphaSigned, 0.0);
                float negPart = 1.0 - posPart;
                float3 result = tex.rgb * input.lighting * negPart + tex.rgb * posPart;
                return float4(result, _Alpha);
            }
            ENDHLSL
        }
    }
}
