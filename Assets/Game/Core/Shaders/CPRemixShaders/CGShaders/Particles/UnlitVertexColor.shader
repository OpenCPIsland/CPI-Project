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
            "RenderType" = "Opaque"
        }
        LOD 100

        Pass
        {
            Tags
            {
                "RenderType" = "Opaque"
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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
            v2f vert(appdata_full v)
            {
                v2f o;

                // Transform the texture coordinates
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                // Pass vertex color
                o.color = v.color.rgb;

                // Compute the vertex position in clip space
                o.pos = UnityObjectToClipPos(v.vertex);

                return o;
            }

            // Fragment shader
            fixed4 frag(v2f i) : SV_Target
            {
                // Sample the texture
                fixed4 texColor = tex2D(_MainTex, i.texcoord);

                // Multiply texture color with vertex color
                return fixed4(texColor.rgb * i.color, texColor.a);
            }

            ENDCG
        }
    }
    FallBack Off
}
