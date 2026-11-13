Shader "CpRemix/BlobShadows/ShadowGeoShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        Texture2D _MainTex;
        SamplerState sampler_MainTex;

        float4x4 _blobShadowCamVp;

        struct Attributes
        {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD0;
            float height : TEXCOORD1;

            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float2 uv : TEXCOORD0;
            float fade : TEXCOORD1;
            float worldY : TEXCOORD2;

            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        Varyings vert(Attributes IN)
        {
            UNITY_SETUP_INSTANCE_ID(IN);
            Varyings OUT;
            UNITY_TRANSFER_INSTANCE_ID(IN, OUT);

            float4 positionCS = mul(_blobShadowCamVp, IN.positionOS);

            OUT.positionCS = positionCS;
            OUT.uv = IN.uv;
            OUT.worldY = IN.positionOS.y;

            float edgeFade = max(abs(positionCS.x), abs(positionCS.y)) - 0.8;
            OUT.fade = max(edgeFade * 5.0, 0.0);

            return OUT;
        }

        float4 frag(Varyings IN) : SV_Target
        {
            UNITY_SETUP_INSTANCE_ID(IN);
            float texValue = _MainTex.Sample(sampler_MainTex, IN.uv).r;

            float4 outputColor;
            outputColor.r = texValue + IN.fade;
            outputColor.g = IN.worldY;
            outputColor.b = 1.0;
            outputColor.a = 1.0;

            return outputColor;
        }

        ENDHLSL

        Pass
        {
            ZTest Always
            ZWrite Off
            Blend One One
            BlendOp Min
            ColorMask R

            HLSLPROGRAM
            #pragma target 4.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            ENDHLSL
        }

        Pass
        {
            ZTest Always
            ZWrite Off
            ColorMask G

            HLSLPROGRAM
            #pragma target 4.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            ENDHLSL
        }
    }
}