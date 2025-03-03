Shader "CpRemix/GPU Combined Avatar"
{
    Properties
    {
        _MainTex("Diffuse Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "QUEUE" = "Geometry+99"
            "RenderType" = "Opaque"
        }
        Pass
        {
            Tags
            {
                "LIGHTMODE" = "FORWARDBASE"
                "QUEUE" = "Geometry+99"
                "RenderType" = "Opaque"
            }

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"

            uniform float4 bonepos[48];
            uniform float4 bonequat[48];
            uniform sampler2D _MainTex;

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
                float3 normal_2 = v._glesNormal;
                float tmpvar_3 = float(v._glesTANGENT.x);
                float4 tmpvar_4 = bonequat[tmpvar_3];
                float3 tmpvar_5 = (2.0 * ((tmpvar_4.yzx * normal_2.zxy) - (tmpvar_4.zxy * normal_2.yzx)));
                float3 tmpvar_6 = normalize(((normal_2 + (tmpvar_4.w * tmpvar_5)) + ((tmpvar_4.yzx * tmpvar_5.zxy) - (tmpvar_4.zxy * tmpvar_5.yzx))));
                float3 BlendedPosition_7;
                float4 bonePosition_8;
                float4 boneQuaternion_9;
                int4 tmpvar_10 = int4(v._glesTANGENT);
                float4 tmpvar_11 = (frac(v._glesTANGENT) * 2.0);
                float4 tmpvar_12 = bonequat[tmpvar_10.x];
                float3 tmpvar_13 = (2.0 * ((tmpvar_12.yzx * v._glesVertex.zxy) - (tmpvar_12.zxy * v._glesVertex.yzx)));
                boneQuaternion_9 = bonequat[tmpvar_10.y];
                bonePosition_8 = bonepos[tmpvar_10.y];
                float3 tmpvar_14 = (2.0 * ((boneQuaternion_9.yzx * v._glesVertex.zxy) - (boneQuaternion_9.zxy * v._glesVertex.yzx)));
                BlendedPosition_7 = (((bonepos[tmpvar_10.x].xyz + ((v._glesVertex.xyz + (tmpvar_12.w * tmpvar_13)) + ((tmpvar_12.yzx * tmpvar_13.zxy) - (tmpvar_12.zxy * tmpvar_13.yzx)))) * tmpvar_11.x) + ((bonePosition_8.xyz + ((v._glesVertex.xyz + (boneQuaternion_9.w * tmpvar_14)) + ((boneQuaternion_9.yzx * tmpvar_14.zxy) - (boneQuaternion_9.zxy * tmpvar_14.yzx)))) * tmpvar_11.y));
                boneQuaternion_9 = bonequat[tmpvar_10.z];
                bonePosition_8 = bonepos[tmpvar_10.z];
                float3 tmpvar_15 = (2.0 * ((boneQuaternion_9.yzx * v._glesVertex.zxy) - (boneQuaternion_9.zxy * v._glesVertex.yzx)));
                BlendedPosition_7 = (BlendedPosition_7 + ((bonePosition_8.xyz + ((v._glesVertex.xyz + (boneQuaternion_9.w * tmpvar_15)) + ((boneQuaternion_9.yzx * tmpvar_15.zxy) - (boneQuaternion_9.zxy * tmpvar_15.yzx)))) * tmpvar_11.z));
                boneQuaternion_9 = bonequat[tmpvar_10.w];
                bonePosition_8 = bonepos[tmpvar_10.w];
                float3 tmpvar_16 = (2.0 * ((boneQuaternion_9.yzx * v._glesVertex.zxy) - (boneQuaternion_9.zxy * v._glesVertex.yzx)));
                BlendedPosition_7 = (BlendedPosition_7 + ((bonePosition_8.xyz + ((v._glesVertex.xyz + (boneQuaternion_9.w * tmpvar_16)) + ((boneQuaternion_9.yzx * tmpvar_16.zxy) - (boneQuaternion_9.zxy * tmpvar_16.yzx)))) * tmpvar_11.w));
                float3 worldSpaceNormalNormalized_17 = tmpvar_6;
                float3 worldSpaceLightDirNormalized_18;
                float3 tmpvar_19;
                float4 tmpvar_20;
                tmpvar_20.w = 1;
                tmpvar_20.xyz = BlendedPosition_7;
                float3 tmpvar_21 = normalize((_WorldSpaceLightPos0.xyz - (BlendedPosition_7 * _WorldSpaceLightPos0.w)));
                worldSpaceLightDirNormalized_18 = tmpvar_21;
                float tmpvar_22 = max(0.0, dot(worldSpaceNormalNormalized_17, worldSpaceLightDirNormalized_18));
                tmpvar_19 = ((_LightColor0.xyz * tmpvar_22) * 0.75);
                tmpvar_19 = (tmpvar_19 + ((glstate_lightmodel_ambient * 2.0).xyz * 0.45));
                float3 tmpvar_23 = max(tmpvar_19, (float3(0.6, 0.6, 0.6) * (tmpvar_22 + 0.5)));
                tmpvar_19 = tmpvar_23;
                o.gl_Position = mul(unity_MatrixVP , tmpvar_20);
                o.xlv_TEXCOORD0 = v._glesMultiTexCoord0.xy;
                o.xlv_TEXCOORD1 = tmpvar_23;
                o.xlv_COLOR = v._glesColor.xyz;
                return o;
            }

            OUT_Data_Frag frag(v2f f)
            {
                OUT_Data_Frag o;
                float4 tmpvar_1;
                float3 lightingOrEmissive_2;
                float emissive_3;
                float4 diffuseAndPacked_4;
                float4 tmpvar_5 = tex2D(_MainTex, f.xlv_TEXCOORD0);
                diffuseAndPacked_4 = tmpvar_5;
                float tmpvar_6 = ((diffuseAndPacked_4.w * 2.0) - 1.0);
                emissive_3 = tmpvar_6;
                emissive_3 = (emissive_3 * float((emissive_3 >= 0.0)));
                float3 tmpvar_7 = (((f.xlv_TEXCOORD1 * diffuseAndPacked_4.xyz) * (1.0 - emissive_3)) + (diffuseAndPacked_4.xyz * emissive_3));
                lightingOrEmissive_2 = tmpvar_7;
                float4 tmpvar_8;
                tmpvar_8.w = 1.0;
                tmpvar_8.xyz = lightingOrEmissive_2;
                tmpvar_1 = tmpvar_8;
                o.gl_FragData = tmpvar_1;
                return o;
            }

            ENDCG
        }
    }
    FallBack Off
}
