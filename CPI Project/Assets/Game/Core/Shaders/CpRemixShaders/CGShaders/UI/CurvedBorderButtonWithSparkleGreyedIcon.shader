Shader "CpRemix/UI/CurvedBorderButtonWithSparkleGreyedIcon"
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
      _ShadowVec("Shadow Vector", Vector) = (-0.05,0.15,0,0)
      _ShadowInnerVec("Shadow Inner Vector", Vector) = (-0.05,0.15,0,0)
      _InnerShading("Inner Shading Value", float) = 0.2
      _OuterShading("Outer Shading Value", float) = 0.2
      _InnerShadowBlur("Inner Shadow Falloff", float) = 0.03
      _OuterShadowBlur("Outer Shadow Falloff", float) = 0.03
      _EffectTex("Sparkle Texture", 2D) = "white" {}
      _RotationScalar("Rotation Speed", float) = 0.1
      _EffectAlpha("Effect Alpha", float) = 0.7
      _GreyGamma("Grey Gamma (1/gamma)", float) = 0.46
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
          "RenderPipeline" = "UniversalPipeline"
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
          Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
          ColorMask[_ColorMask]

        HLSLPROGRAM

        #pragma vertex vert
        #pragma fragment frag

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        float4 _Color;
        float2 _ShadowVec;
        float4 _Tile;
        float _RotationScalar;
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
        float _EffectAlpha;
        float _GreyGamma;
        float _GreySaturate;
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_EffectTex);
        SAMPLER(sampler_EffectTex);

        struct appdata_t
        {
          float4 vertex : POSITION;
          float4 color : COLOR;
          float2 texcoord : TEXCOORD0;
        };

        struct v2f
        {
          float4 pos : SV_POSITION;
          float4 xlv_COLOR : COLOR;
          float2 xlv_TEXCOORD0 : TEXCOORD0;
          float2 xlv_TEXCOORD1 : TEXCOORD1;
          float2 xlv_TEXCOORD2 : TEXCOORD2;
          float2 xlv_TEXCOORD3 : TEXCOORD3;
        };

        v2f vert(appdata_t input)
        {
          v2f o;
          float2 tmpvar_1;
          tmpvar_1 = (float2(1.0, 1.0) + (abs(_ShadowVec) * 0.5));
          float tmpvar_4;
          float4 tmpvar_5;
          tmpvar_5 = (_RotationScalar * _Time);
          tmpvar_4 = cos(tmpvar_5).x;
          float tmpvar_6;
          float tmpvar_7;
          tmpvar_7 = -(_RotationScalar);
          tmpvar_6 = cos((tmpvar_7 * _Time)).x;
          float tmpvar_8;
          tmpvar_8 = sin(tmpvar_5).x;
          float tmpvar_9;
          tmpvar_9 = sin((tmpvar_7 * _Time)).x;
          float2x2 tmpvar_10;
          tmpvar_10[0].x = tmpvar_4;
          tmpvar_10[0].y = tmpvar_9;
          tmpvar_10[1].x = tmpvar_8;
          tmpvar_10[1].y = tmpvar_4;
          float2x2 tmpvar_11;
          tmpvar_11[0].x = tmpvar_6;
          tmpvar_11[0].y = tmpvar_8;
          tmpvar_11[1].x = tmpvar_9;
          tmpvar_11[1].y = tmpvar_6;
          float2 tmpvar_12;
          tmpvar_12 = (input.texcoord - float2(0.5, 0.5));
          o.xlv_COLOR = (input.color * _Color);
          float2 tmpvar_13;
          tmpvar_13 = (_ShadowVec * 0.5);
          o.pos = TransformObjectToHClip(input.vertex.xyz);
          o.xlv_TEXCOORD0 = (((input.texcoord * _Tile.xy) + _Tile.zw) * float2(tmpvar_1.x / tmpvar_1.y, tmpvar_1.y)) - tmpvar_13;
          o.xlv_TEXCOORD1 = (((input.texcoord * 2.0) - 1.0) * tmpvar_1) - tmpvar_13;
          o.xlv_TEXCOORD2 = (mul(tmpvar_10, tmpvar_12) + float2(0.5, 0.5));
          o.xlv_TEXCOORD3 = (mul(tmpvar_11, tmpvar_12) + float2(0.5, 0.5));
          return o;
        }

        float4 sparkleCentre;
        float4 frag(v2f i) : SV_Target
        {
          sparkleCentre = _Centre;
          float4 fragment_1;
          float4 fx2_2;
          float4 fx1_3;
          float4 image_4;
          float tmpvar_5;
          tmpvar_5 = (1.0 / max(_AAliasSize, 0.0001));
          float tmpvar_6;
          tmpvar_6 = (1.0 - _AAliasSize);
          float tmpvar_7;
          tmpvar_7 = (1.0 - _OuterShadowBlur);
          float tmpvar_8;
          tmpvar_8 = (1.0 - _InnerShadowBlur);
          float2 tmpvar_9;
          tmpvar_9 = pow(abs(i.xlv_TEXCOORD1), (_Roundness));
          float2 tmpvar_10;
          tmpvar_10 = pow(abs((i.xlv_TEXCOORD1 + _ShadowVec)), (_Roundness));
          float2 tmpvar_11;
          tmpvar_11 = pow(abs((i.xlv_TEXCOORD1 + _ShadowInnerVec)), (_Roundness));
          float tmpvar_12;
          tmpvar_12 = pow(max(1.0 - _BorderSize, 0.0), max(_Roundness, 0.0));
          float tmpvar_13;
          tmpvar_13 = (tmpvar_12 - _AAliasSize);
          float tmpvar_14;
          tmpvar_14 = sqrt(dot(tmpvar_9, tmpvar_9));
          float tmpvar_15;
          tmpvar_15 = (1.0 - ((clamp(tmpvar_14, tmpvar_6, 1.0) - tmpvar_6) * tmpvar_5));
          image_4 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.xlv_TEXCOORD0);
          float2 tmpvar_17;
          tmpvar_17 = abs(((i.xlv_TEXCOORD0 - 0.5) * 2.0));
          float tmpvar_18;
          tmpvar_18 = max(tmpvar_17.x, tmpvar_17.y);
          fx1_3 = SAMPLE_TEXTURE2D(_EffectTex, sampler_EffectTex, i.xlv_TEXCOORD2);
          fx2_2 = SAMPLE_TEXTURE2D(_EffectTex, sampler_EffectTex, i.xlv_TEXCOORD3);
          fx1_3.w = (fx1_3.w * _EffectAlpha);
          fx2_2.w = (fx2_2.w * (_EffectAlpha * 0.7));
          sparkleCentre.xyz = lerp(sparkleCentre.xyz, fx1_3.xyz, fx1_3.www);
          sparkleCentre.xyz = lerp(sparkleCentre.xyz, fx2_2.xyz, fx2_2.www);
          float tmpvar_21;
          tmpvar_21 = (_OuterShading * clamp((1.0 - ((clamp(sqrt(dot(tmpvar_10, tmpvar_10)), tmpvar_7, 1.0) - tmpvar_7) * (1.0 / max(_OuterShadowBlur, 0.0001)))), 0.0, 1.0));
          fragment_1 = lerp((_Border * min((tmpvar_15 * 1000.0), 1.0)), (lerp(sparkleCentre, image_4, (max((image_4.w - ((float((tmpvar_18 >= 1.0)) * _TileAttenuation) * tmpvar_18)), 0.0))) * lerp((1.0 - _InnerShading), 1.0, clamp((1.0 - ((clamp(sqrt(dot(tmpvar_11, tmpvar_11)), tmpvar_8, tmpvar_12) - tmpvar_8) * (1.0 / max(_InnerShadowBlur, 0.0001)))), 0.0, 1.0))), ((1.0 - ((clamp(tmpvar_14, tmpvar_13, tmpvar_12) - tmpvar_13) * tmpvar_5))));
          float tmpvar_22;
          if ((tmpvar_15 > 0.0001)) {
            tmpvar_22 = tmpvar_15;
          }
          else {
            tmpvar_22 = tmpvar_21;
          };
          fragment_1.w = tmpvar_22;
          fragment_1.xyz = ((pow(max(dot(fragment_1.xyz, float3(0.2126, 0.7152, 0.0722)), 0.0), max(_GreyGamma, 0.0)) + _GreySaturate));
          return (fragment_1 * i.xlv_COLOR);
        }

        ENDHLSL
        }
    }
    FallBack Off
}
