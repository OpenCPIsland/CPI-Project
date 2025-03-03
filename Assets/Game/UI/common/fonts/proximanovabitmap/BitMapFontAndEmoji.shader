Shader "CpRemix/UI/BitMapFontAndEmoji"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _EmojiTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _EmojiScalar("Emoji Scale", float) = 1
        _StencilComp("Stencil Comparison", float) = 8
        _Stencil("Stencil ID", float) = 0
        _StencilOp("Stencil Operation", float) = 0
        _StencilWriteMask("Stencil Write Mask", float) = 255
        _StencilReadMask("Stencil Read Mask", float) = 255
        _ColorMask("Color Mask", float) = 15
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
                Ref[_Stencil]
                ReadMask[_StencilReadMask]
                WriteMask[_StencilWriteMask]
                Pass[_StencilOp]
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
            ColorMask[_ColorMask]
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _Color;
            sampler2D _MainTex;
            sampler2D _EmojiTex;

            struct v2f
            {
                float4 xlv_COLOR : COLOR;
                float2 xlv_TEXCOORD0 : TEXCOORD0;
                float2 xlv_TEXCOORD1 : TEXCOORD1;
                float4 pos : SV_POSITION;  // Changed to ensure proper vertex position handling
            };

            v2f vert(
                float4 _glesVertex : POSITION,
                float4 _glesColor : COLOR,
                float4 _glesMultiTexCoord0 : TEXCOORD0)
            {
                v2f o;
                float2 coord_1 = _glesMultiTexCoord0.xy;
                float2 tmpvar_4;

                if (_glesMultiTexCoord0.x < 0.499) {
                    tmpvar_4.x = 1.0;
                } else {
                    tmpvar_4.x = 0.0;
                }
                tmpvar_4.y = 1.0 - tmpvar_4.x;

                coord_1.x = max(0.0, (_glesMultiTexCoord0.x - (0.5 * tmpvar_4.y))) * 2.0;
                
                o.pos = UnityObjectToClipPos(_glesVertex);  // Correct vertex transformation
                o.xlv_COLOR = _glesColor * _Color;
                o.xlv_TEXCOORD0 = coord_1;
                o.xlv_TEXCOORD1 = tmpvar_4;
                return o;
            }

            struct fragOutput
            {
                float4 gl_FragData : SV_Target;
            };

            fragOutput frag(v2f i)
            {
                fragOutput o;
                float4 emojiColor = tex2D(_EmojiTex, i.xlv_TEXCOORD0);
                float4 mainColor = tex2D(_MainTex, i.xlv_TEXCOORD0);
                float4 blendedColor;
                
                blendedColor.xyz = i.xlv_COLOR.xyz;
                blendedColor.w = mainColor.w * i.xlv_COLOR.w;
                
                float4 finalColor = (blendedColor * i.xlv_TEXCOORD1.x) + (emojiColor * i.xlv_TEXCOORD1.y);
                
                if (finalColor.w < 0.01)
                {
                    discard;
                }
                
                o.gl_FragData = finalColor;
                return o;
            }

            ENDCG
        }
    }
    FallBack Off
}
