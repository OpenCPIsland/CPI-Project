Shader "CpRemix/UI/BitMapFontAndEmoji"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _EmojiTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _EmojiScalar ("Emoji Scale", Float) = 1
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                half4 color : COLOR;
                float2 flags : TEXCOORD0;
                float2 uv : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_EmojiTex);
            SAMPLER(sampler_EmojiTex);
            half4 _Color;
            float _EmojiScalar;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.vertex = TransformObjectToHClip(v.vertex.xyz);
                OUT.color = v.color * _Color;

                float u = v.texcoord.x;
                bool isFont = u < 0.499;

                OUT.flags = float2(isFont ? 1.0 : 0.0, isFont ? 0.0 : 1.0);
                OUT.uv = float2(max(isFont ? u : (u - 0.5), 0.0) * 2.0, v.texcoord.y);

                return OUT;
            }

            half4 frag(v2f IN) : SV_Target
            {
                half fontAlpha = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).a;
                half4 emoji = SAMPLE_TEXTURE2D(_EmojiTex, sampler_EmojiTex, IN.uv) * _EmojiScalar;

                half4 color;
                color.rgb = IN.color.rgb * IN.flags.x + emoji.rgb * IN.flags.y;
                color.a = fontAlpha * IN.color.a * IN.flags.x + emoji.a * IN.flags.y;

                clip(color.a - 0.01);

                return color;
            }
            ENDHLSL
        }
    }

    FallBack Off
}