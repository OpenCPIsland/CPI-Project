Shader "CpRemix/Igloo/IglooFurnitureUnlit"
{
    Properties
    {
        _Color("Tint Color", Color) = (1,1,1,1)
        _MainTex("Texture (RGB)", 2D) = "white" {}
        _Highlight("Additional Highlight", Range(0,1)) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "LightMode" = "UniversalForward"
            "QUEUE" = "Geometry"
            "RenderType" = "Opaque"
        }

        Pass
        {
            Tags
            {
                "LightMode" = "UniversalForward"
                "QUEUE" = "Geometry"
                "RenderType" = "Opaque"
            }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _Color;
            float _Highlight;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR0;
                float4 vertexColor : COLOR1;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            v2f vert(appdata v)
            {
                UNITY_SETUP_INSTANCE_ID(v);

                v2f o;
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = _Color + _Highlight;
                o.vertexColor = v.color;

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 tex = tex2D(_MainTex, i.uv);
                return tex * i.color * i.vertexColor;
            }

            ENDHLSL
        }
    }

    Fallback Off
}