Shader "CpRemix/World/Unlit Dynamic Object (NO FOG)"
{
    Properties
    {
        _TintColor ("Tint Color", Color) = (1, 1, 1, 1)
        [HideInInspector] _TintColorFloat ("Tint Color Float", Float) = 1.0
        _AdditiveColor ("Additive Color", Color) = (0, 0, 0, 1)
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags {
            "RenderPipeline" = "UniversalPipeline" "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Tags { "RenderType"="Opaque" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _TintColor;
            float4 _AdditiveColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float4 color  : COLOR;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos   : SV_POSITION;
                float2 uv    : TEXCOORD0;
                float4 color : COLOR;
            };

            v2f vert (appdata v)
            {
                UNITY_SETUP_INSTANCE_ID(v);

                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                o.color = v.color;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 c = tex2D(_MainTex, i.uv) * i.color;
                c *= _TintColor;
                c.rgb += _AdditiveColor.rgb * _AdditiveColor.a;
                return saturate(c);
            }
            ENDHLSL
        }
    }
}