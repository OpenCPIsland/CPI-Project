Shader "CpRemix/UI/CurvedBorderButtonWithShadow"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "black" {}
        _Color("Tint", Color) = (1,1,1,1)
        _Centre("Centre Color", Color) = (0,0.372,0.792,1)
        _Border("Border Color", Color) = (1,1,1,1)
        _BorderSize("Border Size ", float) = 0.15
        _AAliasSize("Anti-Aliasing Size", float) = 0.03
        _Roundness("Roundness", float) = 1
        _ShadowVec("Shadow Vector", Vector) = (-0.05,0.15,0,0)
        _ScaleBox("Scale For Shadow", float) = 1.2
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
            "PreviewType" = "Plane"
            "QUEUE" = "Transparent"
        }
        Pass
        {
            Tags
            {
                "PreviewType" = "Plane"
                "QUEUE" = "Transparent"
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
            float4 _Centre;
            float4 _Border;
            float _AAliasSize;
            float _BorderSize;
            float _Roundness;
            float2 _ShadowVec;
            float _ScaleBox;
            sampler2D _MainTex;

            struct v2f
            {
                float4 xlv_COLOR : COLOR;
                float2 xlv_TEXCOORD0 : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            struct fragOutput
            {
                float4 gl_FragData : SV_Target;
            };

            v2f vert(appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);  // Avoid extra matrix multiplications
                o.xlv_COLOR = v.color * _Color;          // Simplified color calculation
                o.xlv_TEXCOORD0 = v.texcoord.xy;
                return o;
            }

            fragOutput frag(v2f i)
            {
                fragOutput o;
                float4 fragment_1;
                float4 image_2;
                float tmpvar_3 = 1.0 / (_AAliasSize);
                float tmpvar_4 = 1.0 - _AAliasSize;
                float2 tmpvar_5 = ((i.xlv_TEXCOORD0 * 2.0) - 1.0) * _ScaleBox;
                float2 tmpvar_6 = pow(abs(tmpvar_5), _Roundness);
                float2 tmpvar_7 = pow(abs((tmpvar_5 + _ShadowVec)), _Roundness);
                float tmpvar_8 = pow(1.0 - _BorderSize, _Roundness);
                float tmpvar_9 = tmpvar_8 - _AAliasSize;
                float tmpvar_10 = sqrt(dot(tmpvar_6, tmpvar_6));
                float tmpvar_11 = 1.0 - ((clamp(tmpvar_10, tmpvar_4, 1.0) - tmpvar_4) * tmpvar_3);
                float tmpvar_12 = 1.0 - ((clamp(tmpvar_10, tmpvar_9, tmpvar_8) - tmpvar_9) * tmpvar_3);
                float tmpvar_13 = 1.0 - ((clamp(sqrt(dot(tmpvar_7, tmpvar_7)), tmpvar_9, tmpvar_8) - tmpvar_9) * tmpvar_3);
                float tmpvar_14 = 1.0 - ((clamp(sqrt(dot(tmpvar_7, tmpvar_7)), tmpvar_4, 1.0) - tmpvar_4) * tmpvar_3);
                float tmpvar_15 = min(tmpvar_11 * 1000.0, 1.0);
                float4 tmpvar_16 = tex2D(_MainTex, i.xlv_TEXCOORD0);
                image_2 = tmpvar_16;
                float tmpvar_17 = (tmpvar_13 < 0.9) ? (0.8 + (tmpvar_13 * 0.2)) : 1.0;
                float tmpvar_18 = (tmpvar_14 > 0.5) ? (tmpvar_14 * 0.2) : 0.0;

                fragment_1.xyz = ((((_Centre * tmpvar_12) * (1.0 - image_2.w)) + ((image_2 * image_2.w) * tmpvar_12)) * tmpvar_17) + ((_Border * (1.0 - tmpvar_12)) * tmpvar_15);
                fragment_1.w = max(tmpvar_11, tmpvar_18);

                float4 tmpvar_19 = fragment_1 * i.xlv_COLOR;
                o.gl_FragData = tmpvar_19;
                return o;
            }

            ENDCG
        }
    }
    FallBack Off
}
