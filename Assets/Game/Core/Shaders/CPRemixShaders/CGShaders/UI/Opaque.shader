Shader "CpRemix/UI/Opaque"
{
	Properties
	{
	  [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
	  _Color("Tint", Color) = (1,1,1,1)
	  [HideInInspector] _StencilComp("Stencil Comparison", float) = 8
	  [HideInInspector] _Stencil("Stencil ID", float) = 0
	  [HideInInspector] _StencilOp("Stencil Operation", float) = 0
	  [HideInInspector] _StencilWriteMask("Stencil Write Mask", float) = 255
	  [HideInInspector] _StencilReadMask("Stencil Read Mask", float) = 255
	  [HideInInspector] _ColorMask("Color Mask", float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Opaque"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        ZWrite Off //this fixes the UI Bug?
        ColorMask [_ColorMask]

        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float4 _MainTex_ST;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color = tex2D(_MainTex, IN.texcoord)* IN.color;
                return color;
            }
        ENDCG
        }
    }
	FallBack "UI/Default"
}