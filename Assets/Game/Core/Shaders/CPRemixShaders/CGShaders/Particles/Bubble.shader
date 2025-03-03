Shader "CpRemix/Particles/Bubble"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags
        {
            "CanUseSpriteAtlas" = "true"
            "IGNOREPROJECTOR" = "true"
            "PreviewType" = "Plane"
            "QUEUE" = "Transparent"
            "RenderType" = "Transparent"
        }
        Pass
        {
            Tags
            {
                "CanUseSpriteAtlas" = "true"
                "IGNOREPROJECTOR" = "true"
                "PreviewType" = "Plane"
                "QUEUE" = "Transparent"
                "RenderType" = "Transparent"
            }
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Uniforms
            float4 _Color;
            sampler2D _MainTex;

            // Vertex-to-fragment structure
            struct v2f
            {
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            // Vertex Shader
            v2f vert(appdata_full v)
            {
                v2f o;

                // Apply texture coordinate and color modification
                o.texcoord = v.texcoord.xy;
                o.color = v.color * _Color;

                // Calculate position in clip space
                o.pos = UnityObjectToClipPos(v.vertex);

                return o;
            }

            // Fragment Shader
            fixed4 frag(v2f i) : SV_Target
            {
                // Sample the texture and multiply with the vertex color
                fixed4 texColor = tex2D(_MainTex, i.texcoord);
                return texColor * i.color;
            }
            ENDCG
        }
    }
    FallBack Off
}
