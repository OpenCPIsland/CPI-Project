Shader "CpRemix/Particles/UnlitDiffuse"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _TintColor("Tint Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
        }
        LOD 100
        Pass
        {
            Tags
            {
                "RenderType" = "Opaque"
            }
            LOD 100

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _MainTex_ST;
            sampler2D _MainTex;
            float3 _TintColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 texcoord : TEXCOORD0;
                float3 color : COLOR;
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.texcoord = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                o.color = v.color.rgb;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 texColor = tex2D(_MainTex, i.texcoord);
                texColor.rgb = (texColor.rgb + i.color) * _TintColor;
                texColor.a = 1.0; // Force alpha to be 1
                return texColor;
            }

            ENDCG
        }
    }
    FallBack Off
}
