Shader "Custom/mg_pt_Shader_PizzaSauce"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,0.5)
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM

            #pragma target 4.5
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            sampler2D _MainTex;
            float4 _Color;
            float4x4 _ScaleTransform;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float4 localPos : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;

                float4 scaledVertex = mul(_ScaleTransform, v.vertex);

                o.position = mul(UNITY_MATRIX_VP, scaledVertex);
                o.localPos = v.vertex;

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                if (abs(i.localPos.x) >= 0.5 || abs(i.localPos.y) >= 0.5)
                    return 0;

                float2 uv;
                uv.x = i.localPos.x + 0.5;
                uv.y = i.localPos.y + 0.5;

                float4 texCol = tex2D(_MainTex, uv);

                if (texCol.a <= 0.0)
                    return 0;

                return texCol * _Color;
            }

            ENDHLSL
        }
    }

    Fallback Off
}