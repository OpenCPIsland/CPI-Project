Shader "CpRemix/UI/TextSlow"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15

        [KeywordEnum(Off,Sine)] _OscMode ("Oscillation Mode", Float) = 0
        _Oscillation ("X Speed, X Amp, Y Speed, Y Amp", Vector) = (1,0.01,1,0.01)
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
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
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "DEFAULT"

            HLSLPROGRAM

            #pragma target 4.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #pragma multi_compile_local _OSCMODE_OFF _OSCMODE_SINE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);

            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;

            float4 _Color;
            float4 _TextureSampleAdd;
            float4 _Oscillation;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color  : COLOR;
                float2 uv     : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color  : COLOR;
                float2 uv     : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                float4 clipPos = TransformObjectToHClip(v.vertex.xyz);

                #if defined(_OSCMODE_SINE)
                clipPos.x += sin(_Time.y * _Oscillation.x) * _Oscillation.y;
                clipPos.y += sin(_Time.y * _Oscillation.z) * _Oscillation.w;
                #endif

                o.vertex = clipPos;
                o.uv = (v.uv * _MainTex_ST.xy + _MainTex_ST.zw);
                o.color = v.color * _Color;

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 texcol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                texcol += _TextureSampleAdd;

                return texcol * i.color;
            }

            ENDHLSL
        }
    }

}