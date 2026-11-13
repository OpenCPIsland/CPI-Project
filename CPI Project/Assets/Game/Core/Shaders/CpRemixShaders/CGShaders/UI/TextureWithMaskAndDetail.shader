Shader "CpRemix/UI/TextureWithMaskAndDetail"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}
        _DetailTex ("Detail Texture", 2D) = "black" {}
        _Color ("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma target 4.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);
            TEXTURE2D(_DetailTex);
            SAMPLER(sampler_DetailTex);

            half4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                half4 color : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.color = v.color * _Color;
                o.uv = v.uv;

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 mainCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * i.color;
                half4 detailCol = SAMPLE_TEXTURE2D(_DetailTex, sampler_DetailTex, i.uv);
                half4 maskCol = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv);

                mainCol.rgb = lerp(
                    mainCol.rgb,
                    detailCol.rgb,
                    detailCol.a
                );

                mainCol.a = max(
                    detailCol.a,
                    maskCol.a
                );

                return mainCol;
            }

            ENDHLSL
        }
    }

    FallBack Off
}