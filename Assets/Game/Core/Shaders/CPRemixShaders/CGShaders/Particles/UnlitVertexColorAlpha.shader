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

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct v2f
            {
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
                float4 pos : SV_POSITION;
            };

            // Vertex Shader
            v2f vert(appdata_full v)
            {
                v2f o;

                // Transform texture coordinates
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                // Pass vertex color (includes alpha)
                o.color = v.color;

                // Compute vertex position in clip space
                o.pos = UnityObjectToClipPos(v.vertex);

                return o;
            }

            // Fragment Shader
            fixed4 frag(v2f i) : SV_Target
            {
                // Sample the texture
                fixed4 texColor = tex2D(_MainTex, i.texcoord);

                // Combine texture color and vertex color, respecting alpha
                return texColor * i.color;
            }

            ENDCG
        }
    }
    FallBack Off
}
