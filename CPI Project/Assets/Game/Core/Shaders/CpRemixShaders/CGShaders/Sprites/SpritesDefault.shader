
Shader "CpRemix/Sprites/Default"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("Renderer Color", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
            "RenderPipeline" = "UniversalPipeline"
        }

        Cull Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            Name "SpriteUnlit"

            Tags
            {
                "LightMode" = "SRPDefaultUnlit"
            }

            HLSLPROGRAM

            #pragma target 2.0
            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_AlphaTex);
            SAMPLER(sampler_AlphaTex);

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half4 _RendererColor;
                float4 _Flip;
                float _EnableExternalAlpha;
                float PixelSnap;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                half4 color : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                half4 color : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings SpriteVert(Attributes input)
            {
                Varyings output;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                input.positionOS.xy *= _Flip.xy;

                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);

                #if defined(PIXELSNAP_ON)
                    float2 pixelPosition =
                        (output.positionCS.xy / output.positionCS.w)
                        * (_ScreenParams.xy * 0.5);

                    pixelPosition = floor(pixelPosition) + 0.5;

                    output.positionCS.xy =
                        (pixelPosition * 2.0 / _ScreenParams.xy)
                        * output.positionCS.w;
                #endif

                output.uv = input.uv;
                output.color = input.color * _Color * _RendererColor;

                return output;
            }

            half4 SpriteFrag(Varyings input) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(
                    _MainTex,
                    sampler_MainTex,
                    input.uv
                );

                #if defined(ETC1_EXTERNAL_ALPHA)
                    half externalAlpha = SAMPLE_TEXTURE2D(
                        _AlphaTex,
                        sampler_AlphaTex,
                        input.uv
                    ).r;

                    color.a = lerp(
                        color.a,
                        externalAlpha,
                        _EnableExternalAlpha
                    );
                #endif

                color *= input.color;

                color.rgb *= color.a;

                return color;
            }

            ENDHLSL
        }
    }
}
