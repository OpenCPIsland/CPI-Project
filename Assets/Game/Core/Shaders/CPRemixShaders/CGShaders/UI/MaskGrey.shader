Shader "CpRemix/UI/MaskGrey"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Brightness("Brightness", Range(0, 1)) = 0
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

            sampler2D _MainTex;
            float _Brightness;

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
                o.xlv_COLOR = v.color;
                o.position = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 texcol = tex2D(_MainTex, i.xlv_TEXCOORD0);
                
                // Calculate the greyscale color based on the luminance formula
                float greyValue = (texcol.r + texcol.g + texcol.b) / 3.0;
                
                // Adjust the greyscale value based on the brightness property
                greyValue *= (1.0 + _Brightness * 2.0);

                // Lerp between original color and greyscale
                texcol.rgb = lerp(texcol.rgb, float3(greyValue, greyValue, greyValue), i.xlv_COLOR.a);

                return texcol * i.xlv_COLOR;
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}
