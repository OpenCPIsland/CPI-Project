Shader "UI/BitMapFont"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
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

            struct v2f
            {
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert(float4 vertex : POSITION, float4 color : COLOR, float2 texcoord : TEXCOORD0)
            {
                v2f o;
                o.color = color;
                o.texcoord = texcoord;
                o.pos = UnityObjectToClipPos(vertex); // Simplified transform
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 texColor = tex2D(_MainTex, i.texcoord);
                float4 finalColor = texColor * i.color * _Color;

                // Alpha check to discard if below threshold
                if (finalColor.a < 0.01)
                    discard;

                return finalColor;
            }

            ENDCG
        }
    }
    FallBack Off
}
