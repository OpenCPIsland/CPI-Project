Shader "CpRemix/UI/CurvedBorderButtonWithShadowAndTiledGreyed"
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
      _GreyGamma("Grey Gamma (1/gamma)", float) = 1
      _GreySaturate("Grey Black Level", float) = 0
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
            float2 _ShadowVec;
            float4 _Tile;
            float4 _Centre;
            float4 _Border;
            float _AAliasSize;
            float _BorderSize;
            float _Roundness;
            float2 _ShadowInnerVec;
            float _InnerShading;
            float _OuterShading;
            float _OuterShadowBlur;
            float _InnerShadowBlur;
            float _TileAttenuation;
            float _GreyGamma;
            float _GreySaturate;
            sampler2D _MainTex;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 xlv_COLOR : COLOR;
                float2 xlv_TEXCOORD0 : TEXCOORD0;
                float2 xlv_TEXCOORD1 : TEXCOORD1;
            };

            struct FragOutput
            {
                float4 gl_FragData : SV_Target;
            };

            v2f vert(appdata_full v)
            {
                v2f o;
                float2 shadowVecAbs = abs(_ShadowVec) * 0.5;
                float2 tmpvar_1 = float2(1.0, 1.0) + shadowVecAbs;
                float2 tmpvar_2;
                tmpvar_2.x = tmpvar_1.x / tmpvar_1.y;
                tmpvar_2.y = tmpvar_1.y;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.xlv_COLOR = v.color * _Color;
                o.xlv_TEXCOORD0 = ((v.texcoord.xy * _Tile.xy + _Tile.zw) * tmpvar_2) - (0.5 * _ShadowVec);
                o.xlv_TEXCOORD1 = ((v.texcoord.xy * 2.0 - 1.0) * tmpvar_1) - (0.5 * _ShadowVec);
                
                return o;
            }

            FragOutput frag(v2f i)
            {
                FragOutput o;
                float4 fragment_1;
                float4 image_2;
                float tmpvar_3 = 1.0 / _AAliasSize;
                float tmpvar_4 = 1.0 - _AAliasSize;
                float tmpvar_5 = 1.0 - _OuterShadowBlur;
                float tmpvar_6 = 1.0 - _InnerShadowBlur;
                float2 tmpvar_7 = pow(abs(i.xlv_TEXCOORD1), _Roundness);
                float2 tmpvar_8 = pow(abs(i.xlv_TEXCOORD1 + _ShadowVec), _Roundness);
                float2 tmpvar_9 = pow(abs(i.xlv_TEXCOORD1 + _ShadowInnerVec), _Roundness);
                float tmpvar_10 = pow(1.0 - _BorderSize, _Roundness);
                float tmpvar_11 = tmpvar_10 - _AAliasSize;
                float tmpvar_12 = sqrt(dot(tmpvar_7, tmpvar_7));
                float tmpvar_13 = 1.0 - ((clamp(tmpvar_12, tmpvar_4, 1.0) - tmpvar_4) * tmpvar_3);
                image_2 = tex2D(_MainTex, i.xlv_TEXCOORD0);
                float2 tmpvar_15 = abs((i.xlv_TEXCOORD0 - 0.5) * 2.0);
                float tmpvar_16 = max(tmpvar_15.x, tmpvar_15.y);
                float tmpvar_17 = _OuterShading * clamp(1.0 - ((clamp(sqrt(dot(tmpvar_8, tmpvar_8)), tmpvar_5, 1.0) - tmpvar_5) * (1.0 / _OuterShadowBlur)), 0.0, 1.0);
                fragment_1 = lerp(_Border * min(tmpvar_13 * 1000.0, 1.0), lerp(_Centre, image_2, max(image_2.w - (float(tmpvar_16 >= 1.0) * _TileAttenuation * tmpvar_16), 0.0)) * lerp(1.0 - _InnerShading, 1.0, clamp(1.0 - (clamp(sqrt(dot(tmpvar_9, tmpvar_9)), tmpvar_6, tmpvar_10) - tmpvar_6) * (1.0 / _InnerShadowBlur), 0.0, 1.0)), 1.0 - ((clamp(tmpvar_12, tmpvar_11, tmpvar_10) - tmpvar_11) * tmpvar_3));

                float tmpvar_18;
                if (tmpvar_13 > 0.0001) 
                    tmpvar_18 = tmpvar_13;
                else 
                    tmpvar_18 = tmpvar_17;

                fragment_1.w = tmpvar_18;
                fragment_1.xyz = pow(dot(fragment_1.xyz, float3(0.2126, 0.7152, 0.0722)), _GreyGamma) + _GreySaturate;
                float4 tmpvar_19 = fragment_1 * i.xlv_COLOR;
                o.gl_FragData = tmpvar_19;
                return o;
            }

            ENDCG
        }
    }
    FallBack Off
}
