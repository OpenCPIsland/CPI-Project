Shader "CpRemix/Particles/UnlitVertexColorAlpha"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "QUEUE" = "Transparent"
            "RenderType" = "Transparent"
        }
        Pass
        {
            Tags
            {
                "QUEUE" = "Transparent"
                "RenderType" = "Transparent"
            }
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            struct Attributes
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 color : COLOR;
                float4 texcoord : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float4 texcoord2 : TEXCOORD2;
                float4 texcoord3 : TEXCOORD3;
            };


            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct v2f
            {
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
                float4 pos : SV_POSITION;
            };

            // Vertex Shader
            v2f vert(Attributes v)
            {
                v2f o;

                // Transform texture coordinates
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                // Pass vertex color (includes alpha)
                o.color = v.color;

                // Compute vertex position in clip space
                o.pos = TransformObjectToHClip(v.vertex.xyz);

                return o;
            }

            // Fragment Shader
            half4 frag(v2f i) : SV_Target
            {
                // Sample the texture
                half4 texColor = tex2D(_MainTex, i.texcoord);

                // Combine texture color and vertex color, respecting alpha
                return texColor * i.color;
            }

            ENDHLSL
        }
    }
    FallBack Off
}
