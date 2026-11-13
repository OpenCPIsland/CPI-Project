Shader "CpRemix/GPU Combined Avatar"
{
    Properties
    {
        _MainTex ("Diffuse Texture", 2D) = "white" {}
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

            float4 bonepos[48];
            float4 bonequat[48];

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                float3 color : COLOR;
                float4 boneData : TANGENT;
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

            float3 RotateVectorByQuaternion(float3 value, float4 rotation)
            {
                float3 t = 2.0 * cross(rotation.xyz, value);
                return value + rotation.w * t + cross(rotation.xyz, t);
            }

            Varyings vert(Attributes input)
            {
                UNITY_SETUP_INSTANCE_ID(input);
                Varyings output;
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                int4 signedIndices = (int4)input.boneData;
                uint4 indices = (uint4)signedIndices;
                float4 weights = frac(input.boneData) * 2.0;
                float3 skinnedPosition =
                    weights.x * (RotateVectorByQuaternion(input.positionOS.xyz, bonequat[indices.x]) + bonepos[indices.x].xyz) +
                    weights.y * (RotateVectorByQuaternion(input.positionOS.xyz, bonequat[indices.y]) + bonepos[indices.y].xyz) +
                    weights.z * (RotateVectorByQuaternion(input.positionOS.xyz, bonequat[indices.z]) + bonepos[indices.z].xyz) +
                    weights.w * (RotateVectorByQuaternion(input.positionOS.xyz, bonequat[indices.w]) + bonepos[indices.w].xyz);

                float3 worldNormal = normalize(RotateVectorByQuaternion(input.normalOS, bonequat[indices.x]));
                Light mainLight = GetMainLight();
                float ndotl = max(dot(worldNormal, mainLight.direction), 0.0);
                float3 diffuse = ndotl * mainLight.color;
                float litFloor = (ndotl + 0.5) * 0.6;
                float3 ambient = unity_AmbientSky.rgb * 0.45;

                output.positionCS = TransformWorldToHClip(skinnedPosition);
                output.uv = input.uv;
                output.lighting = max(litFloor, diffuse * 0.75 + ambient);
                output.color = input.color;
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                float4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                float selfLit = saturate(tex.a * 2.0 - 1.0);
                float3 color = tex.rgb * (input.lighting * (1.0 - selfLit) + selfLit);
                return float4(color, 1.0);
            }
            ENDHLSL
        }
    }
}
