Shader "CpRemix/Particles/UnlitDiffuseAdditive"
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
            "IGNOREPROJECTOR" = "true"
            "QUEUE" = "Transparent"
            "RenderType" = "Transparent"
        }

        Pass
        {
            Tags
            {
                "IGNOREPROJECTOR" = "true"
                "QUEUE" = "Transparent"
                "RenderType" = "Transparent"
            }

            ZWrite Off
            Blend SrcAlpha One  // Additive blending

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
            float4 _MainTex_ST;
            sampler2D _MainTex;

            struct v2f
            {
                float2 texcoord : TEXCOORD0;
                float3 color : COLOR;
                float4 pos : SV_POSITION;
            };

            // Vertex Shader
            v2f vert(Attributes v)
            {
                v2f o;

                // Calculate the transformed texture coordinates
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                // Pass the vertex color
                o.color = v.color.rgb;

                // Calculate the vertex position in clip space
                o.pos = TransformObjectToHClip(v.vertex.xyz);

                return o;
            }

            // Fragment Shader
            half4 frag(v2f i) : SV_Target
            {
                // Sample the texture
                half4 texColor = tex2D(_MainTex, i.texcoord);

                // Multiply texture color with vertex color (additive blending will be applied later)
                float3 finalColor = texColor.rgb * i.color;

                // Compute the alpha as the maximum of the RGB channels, scaled by 1.5 for brightness
                float alpha = max(finalColor.r, max(finalColor.g, finalColor.b)) * 1.5;

                return half4(finalColor, alpha);
            }

            ENDHLSL
        }
    }
    FallBack Off
}
