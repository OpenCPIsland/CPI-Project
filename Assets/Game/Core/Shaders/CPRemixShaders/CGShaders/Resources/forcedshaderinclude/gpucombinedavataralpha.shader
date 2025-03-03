Shader "CpRemix/GPU Combined Avatar Alpha"
{
    Properties
    {
        _MainTex("Diffuse Texture", 2D) = "white" {}
        _Alpha("Alpha", float) = 0
    }
    SubShader
    {
        Tags
        {
            "QUEUE" = "Transparent"
            "RenderType" = "Transparent"
        }
        Pass
        {
            Tags
            {
                "LIGHTMODE" = "FORWARDBASE"
                "QUEUE" = "Transparent"
                "RenderType" = "Transparent"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"

            uniform float4 bonepos[48];
            uniform float4 bonequat[48];
            uniform sampler2D _MainTex;
            uniform float _Alpha;

            struct appdata_t
            {
                float4 _glesTANGENT : TANGENT;
                float4 _glesVertex : POSITION;
                float4 _glesColor : COLOR;
                float3 _glesNormal : NORMAL;
                float4 _glesMultiTexCoord0 : TEXCOORD0;
            };

            struct OUT_Data_Vert
            {
                float2 xlv_TEXCOORD0 : TEXCOORD0;
                float3 xlv_TEXCOORD1 : TEXCOORD1;
                float3 xlv_COLOR : COLOR;
                float4 gl_Position : SV_POSITION;
            };

            struct v2f
            {
                float2 xlv_TEXCOORD0 : TEXCOORD0;
                float3 xlv_TEXCOORD1 : TEXCOORD1;
            };

            struct OUT_Data_Frag
            {
                float4 gl_FragData : SV_Target0;
            };

            OUT_Data_Vert vert(appdata_t v)
            {
                OUT_Data_Vert o;

                // Extract bone indices and weights
                int4 boneIndices = int4(v._glesTANGENT);
                float4 boneWeights = frac(v._glesTANGENT) * 2.0;

                // Initialize blended position and normal
                float3 blendedPosition = float3(0.0, 0.0, 0.0);
                float3 blendedNormal = float3(0.0, 0.0, 0.0);

                // Loop over all bone influences (up to 4 bones)
                for (int i = 0; i < 4; i++) {
                    float4 boneQuat = bonequat[boneIndices[i]];
                    float3 bonePos = bonepos[boneIndices[i]].xyz;

                    // Transform vertex position using bone quaternion
                    float3 localPos = v._glesVertex.xyz;
                    float3 rotatedPos = 2.0 * cross(boneQuat.yzx, localPos.zxy) - cross(boneQuat.zxy, localPos.yzx);
                    float3 transformedPos = localPos + (boneQuat.w * rotatedPos) + cross(boneQuat.yzx, rotatedPos.zxy);

                    // Blend the transformed position by the bone weight
                    blendedPosition += (bonePos + transformedPos) * boneWeights[i];

                    // Transform and blend normal
                    float3 localNormal = v._glesNormal;
                    float3 rotatedNormal = 2.0 * cross(boneQuat.yzx, localNormal.zxy) - cross(boneQuat.zxy, localNormal.yzx);
                    float3 transformedNormal = localNormal + (boneQuat.w * rotatedNormal) + cross(boneQuat.yzx, rotatedNormal.zxy);
                    blendedNormal += normalize(transformedNormal) * boneWeights[i];
                }

                // Normalize the blended normal vector
                blendedNormal = normalize(blendedNormal);

                // Calculate lighting and final vertex position
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz - (blendedPosition * _WorldSpaceLightPos0.w));
                float NdotL = max(0.0, dot(blendedNormal, lightDir));
                float3 lighting = _LightColor0.xyz * NdotL * 0.75 + glstate_lightmodel_ambient.xyz * 0.45;

                o.gl_Position = UnityObjectToClipPos(float4(blendedPosition, 1.0));
                o.xlv_TEXCOORD0 = v._glesMultiTexCoord0.xy;
                o.xlv_TEXCOORD1 = lighting;
                o.xlv_COLOR = v._glesColor.xyz;

                return o;
            }

            OUT_Data_Frag frag(v2f f)
            {
                OUT_Data_Frag o;
                float4 finalColor;
                float3 lightingOrEmissive;
                float emissive;
                float4 diffuseAndPacked = tex2D(_MainTex, f.xlv_TEXCOORD0);

                // Calculate emissive factor and lighting
                emissive = ((diffuseAndPacked.w * 2.0) - 1.0);
                emissive = max(emissive, 0.0);
                lightingOrEmissive = lerp(f.xlv_TEXCOORD1 * diffuseAndPacked.xyz, diffuseAndPacked.xyz, emissive);

                // Apply alpha blending
                finalColor.xyz = lightingOrEmissive;
                finalColor.w = _Alpha;
                o.gl_FragData = finalColor;
                return o;
            }

            ENDCG
        }
    }
    FallBack Off
}
