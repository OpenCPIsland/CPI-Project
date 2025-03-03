Shader "CpRemix/UI/Minimal_NoStencil"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
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
            ZTest [unity_GUIZTestMode]
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _Color;
            sampler2D _MainTex;

            struct v2f
            {
                float4 xlv_COLOR : COLOR;
                float2 xlv_TEXCOORD0 : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata_full v)
            {
                v2f o;
                float4 vertexPos = v.vertex;

                o.pos = UnityObjectToClipPos(vertexPos);
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
