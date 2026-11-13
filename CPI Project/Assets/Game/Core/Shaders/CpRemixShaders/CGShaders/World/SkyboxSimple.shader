Shader "CpRemix/Skybox/Simple Cubemap Shader"
{
    Properties
    {
        _cubemap("Environment Map", Cube) = "white" {}
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "QUEUE" = "Background"
        }
        Pass
        {
            Tags
            {
                "QUEUE" = "Background"
            }
            ZWrite Off
            Cull Off
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            samplerCUBE _cubemap;
            sampler2D _MainTex;
            half4 _Color;

            struct v2f
            {
                float4 uv : TEXCOORD0;
            };

            struct FragOutput
            {
                half4 color : SV_Target;
            };

            v2f vert(
                float4 vertex : POSITION,
                float4 uv : TEXCOORD0,
                out float4 outpos : SV_POSITION
            )
            {
                v2f o;
                outpos = TransformObjectToHClip(vertex.xyz);
                o.uv = uv;
                return o;
            }

            FragOutput frag(v2f i)
            {
                FragOutput o;

                float4 texColor = tex2D(_MainTex, i.uv.xy);
                half4 cubemapColor = texCUBE(_cubemap, i.uv.xyz);

                o.color = texColor * cubemapColor * _Color;

                return o;
            }

            ENDHLSL
        }
    }
    FallBack Off
}
