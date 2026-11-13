Shader "Hidden/BlueprintIconOutline"
{
    Properties
    {
        _FillColor ("Fill Color", Color) = (0.039, 0.247, 0.549, 1)
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" "RenderType"="Opaque" }

        Pass
        {
            Cull Back
            ZWrite On

            Tags { "LightMode"="UniversalForwardOnly" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _FillColor;
            CBUFFER_END

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(v.vertex.xyz);
                o.pos = positionInputs.positionCS;
                o.worldNormal = TransformObjectToWorldNormal(v.normal);
                o.viewDir = GetWorldSpaceViewDir(positionInputs.positionWS);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float3 n = normalize(i.worldNormal);
                float3 v = normalize(i.viewDir);

                float ndotv = saturate(dot(n, v));

                float shade = lerp(0.4, 1.0, ndotv);

                half4 col;
                col.rgb = _FillColor.rgb * shade;
                col.a = 1.0;
                return col;
            }
            ENDHLSL
        }
    }
}
