Shader "CpRemix/UI/TextureWithMaskAndDetail"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _MaskTex("Mask Texture", 2D) = "white" {}
        _DetailTex("Detail Texture", 2D) = "black" {}
        _Color("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags
        {
            "QUEUE" = "Transparent"
        }
        Pass
        {
            Tags
            {
                "QUEUE" = "Transparent"
            }
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _Color;
            sampler2D _MainTex;
            sampler2D _MaskTex;
            sampler2D _DetailTex;

            struct v2f
            {
                float4 xlv_COLOR : COLOR;
                float2 xlv_TEXCOORD0 : TEXCOORD0;
            };

            struct FragOutput
            {
                float4 gl_FragData : SV_Target;
            };

            v2f vert(float4 _glesVertex : POSITION, float4 _glesColor : COLOR, float4 _glesMultiTexCoord0 : TEXCOORD0, out float4 gl_Position : SV_POSITION)
            {
                v2f o;
                float4 tmpvar_1;
                tmpvar_1.w = 1.0;
                tmpvar_1.xyz = _glesVertex.xyz;
                gl_Position = UnityObjectToClipPos(tmpvar_1);
                o.xlv_COLOR = (_glesColor * _Color);
                o.xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
                return o;
            }

            FragOutput frag(v2f i)
            {
                FragOutput o;
                
                // Sample the mask texture (use the alpha channel to control blending)
                float maskAlpha = tex2D(_MaskTex, i.xlv_TEXCOORD0).a;

                // Sample the detail texture
                float4 detailColor = tex2D(_DetailTex, i.xlv_TEXCOORD0);

                // Sample the main texture
                float4 mainColor = tex2D(_MainTex, i.xlv_TEXCOORD0) * i.xlv_COLOR;

                // Blend the main texture and detail texture based on the detail texture alpha
                mainColor.rgb = lerp(mainColor.rgb, detailColor.rgb, detailColor.a);

                // Control the final alpha using the mask and detail texture alpha
                mainColor.a = max(detailColor.a, maskAlpha);

                // Output the final color
                o.gl_FragData = mainColor;
                return o;
            }

            ENDCG
        }
    }
    FallBack Off
}
