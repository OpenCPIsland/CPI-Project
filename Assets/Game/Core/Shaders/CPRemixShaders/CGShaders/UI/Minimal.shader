Shader "CpRemix/UI/Minimal"
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
        [KeywordEnum(Off, Sine)] _OscMode("Oscillation mode", float) = 0
        _Oscillation("X speed, X amplitude, Y speed, Y amplitude", Vector) = (1, 0.01, 1, 0.01)
    }

    SubShader
    {
        Tags { "QUEUE" = "Transparent" }

        Pass
        {
            Tags { "QUEUE" = "Transparent" }
            ZTest [unity_GUIZTestMode]
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM

            #pragma multi_compile _OSCMODE_OFF OSCMODE_SINE
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _Color;
            sampler2D _MainTex;
            float4 _Oscillation;
            float _OscMode;

            struct v2f
            {
                float4 xlv_COLOR : COLOR;
                float2 xlv_TEXCOORD0 : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata_full v)
            {
                v2f o;

                // Base vertex position
                float4 vertexPos = v.vertex;

                // Oscillation logic based on mode
                #if defined(OSCMODE_SINE)
                    if (_OscMode == 1.0) // Sine mode
                    {
                        // Use Unity's built-in _Time variable (x = time since start, y = sine wave, z = cosine wave)
                        float xOsc = sin(_Time.y * _Oscillation.x) * _Oscillation.y;
                        float yOsc = cos(_Time.y * _Oscillation.z) * _Oscillation.w;
                        vertexPos.xy += float2(xOsc, yOsc);
                    }
                #endif

                o.pos = UnityObjectToClipPos(vertexPos); // Transform vertex
                o.xlv_COLOR = v.color * _Color;
                o.xlv_TEXCOORD0 = v.texcoord.xy;

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 texColor = tex2D(_MainTex, i.xlv_TEXCOORD0);
                return texColor * i.xlv_COLOR;
            }

            ENDCG
        }
    }

    FallBack "UI/Default"
}
