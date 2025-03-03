Shader "CpRemix/UI/CurvedBorderButtonWithShadowAndTiled"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "black" {}
        _Color("Tint", Color) = (1,1,1,1)
        _Tile("Tile", Vector) = (1,1,0,0)
        _TileAttenuation("Tile Attenuation", float) = 0
        _Centre("Centre Color", Color) = (0,0.372,0.792,1)
        _Border("Border Color", Color) = (1,1,1,1)
        _BorderSize("Border Size ", float) = 0.15
        _AAliasSize("Anti-Aliasing Size", float) = 0.03
        _Roundness("Roundness", float) = 1
        _ShadowVec("Shadow Outer Vector", Vector) = (-0.05,0.15,0,0)
        _ShadowInnerVec("Shadow Inner Vector", Vector) = (-0.05,0.15,0,0)
        _InnerShading("Inner Shading Value", float) = 0.2
        _OuterShading("Outer Shading Value", float) = 0.2
        _InnerShadowBlur("Inner Shadow Falloff", float) = 0.03
        _OuterShadowBlur("Outer Shadow Falloff", float) = 0.03
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
            float4 _Tile;
            float4 _Centre;
            float4 _Border;
            float _AAliasSize;
            float _Roundness;
            float2 _ShadowVec;
            float2 _ShadowInnerVec;
            float _InnerShading;
            float _OuterShading;
            float _OuterShadowBlur;
            float _InnerShadowBlur;
            float _TileAttenuation;
            sampler2D _MainTex;

            struct v2f
            {
                float4 xlv_COLOR : COLOR;
                float2 xlv_TEXCOORD0 : TEXCOORD0;
                float2 xlv_TEXCOORD1 : TEXCOORD1;
            };

            struct fragOutput
            {
                float4 gl_FragData : SV_Target;
            };

            v2f vert(float4 _glesVertex : POSITION, float4 _glesColor : COLOR, float4 _glesMultiTexCoord0 : TEXCOORD0, out float4 gl_Position : SV_POSITION)
            {
                v2f o;
                float2 tmpvar_1 = float2(1.0, 1.0) + (abs(_ShadowVec) * 0.5);
                float2 tmpvar_2;
                tmpvar_2.x = tmpvar_1.x / tmpvar_1.y;
                tmpvar_2.y = tmpvar_1.y;
                float4 tmpvar_3;
                tmpvar_3.w = 1.0;
                tmpvar_3.xyz = _glesVertex.xyz;
                gl_Position = UnityObjectToClipPos(tmpvar_3);
                o.xlv_COLOR = _glesColor * _Color;
                float2 tmpvar_4 = _ShadowVec * 0.5;
                o.xlv_TEXCOORD0 = (((_glesMultiTexCoord0.xy * _Tile.xy) + _Tile.zw) * tmpvar_2) - tmpvar_4;
                o.xlv_TEXCOORD1 = (((_glesMultiTexCoord0.xy * 2.0) - 1.0) * tmpvar_1) - tmpvar_4;
                return o;
            }

            fragOutput frag(v2f i)
            {
                fragOutput o;
                float4 fragment_1;
                float4 image_2;
                float tmpvar_3 = (1.0 / (_AAliasSize));
                float tmpvar_4 = (1.0 - _AAliasSize);
                float tmpvar_5 = (1.0 - _OuterShadowBlur);
                float tmpvar_6 = (1.0 - _InnerShadowBlur);
                float2 tmpvar_7 = pow(abs(i.xlv_TEXCOORD1), _Roundness);
                float2 tmpvar_8 = pow(abs(i.xlv_TEXCOORD1 + _ShadowVec), _Roundness);
                float2 tmpvar_9 = pow(abs(i.xlv_TEXCOORD1 + _ShadowInnerVec), _Roundness);
                float tmpvar_10 = sqrt(dot(tmpvar_7, tmpvar_7));
                float tmpvar_11 = (1.0 - ((clamp(tmpvar_10, tmpvar_4, 1.0) - tmpvar_4) * tmpvar_3));
                float4 tmpvar_12 = tex2D(_MainTex, i.xlv_TEXCOORD0);
                image_2 = tmpvar_12;
                float2 tmpvar_13 = abs(i.xlv_TEXCOORD0 - 0.5) * 2.0;
                float tmpvar_14 = max(tmpvar_13.x, tmpvar_13.y);
                float tmpvar_15 = _OuterShading * clamp(1.0 - ((clamp(sqrt(dot(tmpvar_8, tmpvar_8)), tmpvar_5, 1.0) - tmpvar_5) * (1.0 / _OuterShadowBlur)), 0.0, 1.0);
                fragment_1 = lerp((_Border * min((tmpvar_11 * 1000.0), 1.0)), (lerp(_Centre, image_2, max((image_2.w - ((float(tmpvar_14 >= 1.0) * _TileAttenuation) * tmpvar_14)), 0.0)) * lerp((1.0 - _InnerShading), 1.0, clamp(1.0 - ((clamp(sqrt(dot(tmpvar_9, tmpvar_9)), tmpvar_6, 1.0) - tmpvar_6) * (1.0 / _InnerShadowBlur)), 0.0, 1.0))), ((1.0 - (clamp(tmpvar_10, tmpvar_4, 1.0) - tmpvar_4) * tmpvar_3)));
                float tmpvar_16 = (tmpvar_11 > 0.0001) ? tmpvar_11 : tmpvar_15;
                fragment_1.w = tmpvar_16;
                o.gl_FragData = fragment_1 * i.xlv_COLOR;
                return o;
            }

            ENDCG
        }
    }
    FallBack Off
}
