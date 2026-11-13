Shader "CpRemix/Particles/UnlitVertexColor"
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
            "RenderType" = "Opaque"
        }
        LOD 100

        Pass
        {
            Tags
            {
                "RenderType" = "Opaque"
            }

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
            // Uniforms
            sampler2D _MainTex;
            float4 _MainTex_ST;

            // Structure for vertex-to-fragment data transfer
            struct v2f
            {
                float2 texcoord : TEXCOORD0;
                float3 color : COLOR;
                float4 pos : SV_POSITION;
            };

            // Vertex shader
            v2f vert(Attributes v)
            {
                v2f o;

                // Transform the texture coordinates
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                // Pass vertex color
                o.color = v.color.rgb;

                // Compute the vertex position in clip space
                o.pos = TransformObjectToHClip(v.vertex.xyz);

                return o;
            }

            // Fragment shader
            half4 frag(v2f i) : SV_Target
            {
                // Sample the texture
                half4 texColor = tex2D(_MainTex, i.texcoord);

                // Multiply texture color with vertex color
                return half4(texColor.rgb * i.color, texColor.a);
            }

            ENDHLSL
        }
    }
    FallBack Off
}
