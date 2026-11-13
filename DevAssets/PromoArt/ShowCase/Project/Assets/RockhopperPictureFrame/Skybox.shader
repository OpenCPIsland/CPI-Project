Shader "CpRemix/Igloo/SkyboxFurnitureIconBG"
{
    Properties
    {
        // Default uses #7089DC (112/255,137/255,220/255)
        _Color ("Sky Color", Color) = (0.4392157, 0.5372549, 0.8627451, 1)
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "IgnoreProjector"="True" }

        Cull Off
        ZWrite Off
        Fog { Mode Off }
        Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            fixed4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _Color;
            }
            ENDCG
        }
    }

    FallBack "RenderFX/Skybox"
}
