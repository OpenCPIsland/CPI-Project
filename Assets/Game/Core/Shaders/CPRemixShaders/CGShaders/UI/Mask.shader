Shader "CpRemix/UI/Mask"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _StencilComp("Stencil Comparison", float) = 8
        _Stencil("Stencil ID", float) = 0
        _StencilOp("Stencil Operation", float) = 0
        _StencilWriteMask("Stencil Write Mask", float) = 255
        _StencilReadMask("Stencil Read Mask", float) = 255
        _ColorMask("Color Mask", float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", float) = 0
    }

    SubShader
    {
        Tags
        {
            "CanUseSpriteAtlas" = "true"
            "IGNOREPROJECTOR" = "true"
            "PreviewType" = "Plane"
            "QUEUE" = "Transparent"
            "RenderType" = "Transparent"
        }

        Pass
        {
            Tags
            {
                "CanUseSpriteAtlas" = "true"
                "IGNOREPROJECTOR" = "true"
                "PreviewType" = "Plane"
                "QUEUE" = "Transparent"
                "RenderType" = "Transparent"
            }

            ZWrite Off
            Cull Off
            Stencil
            {
                Ref [_Stencil]
                ReadMask [_StencilReadMask]
                WriteMask[_StencilWriteMask]
                Pass [_StencilOp]
                Comp[_StencilComp]
                Fail Keep
                ZFail Keep
                PassFront Keep
                FailFront Keep
                ZFailFront Keep
                PassBack Keep
                FailBack Keep
                ZFailBack Keep
            }

            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask [_ColorMask]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            float4 _Color;
            sampler2D _MainTex;

            struct v2f
            {
                float4 xlv_COLOR : COLOR;
                float2 xlv_TEXCOORD0 : TEXCOORD0;
                float4 position : SV_POSITION;
            };

            v2f vert(appdata_full v)
            {
                v2f o;
                o.xlv_TEXCOORD0 = v.texcoord.xy;
                o.xlv_COLOR = v.color * _Color;
                o.position = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 color = tex2D(_MainTex, i.xlv_TEXCOORD0) * i.xlv_COLOR;
                #ifdef UNITY_UI_ALPHACLIP
                clip(color.a - 0.001);
                #endif
                return color;
            }

            ENDCG
        }
    }
    FallBack "UI/Default"
}
