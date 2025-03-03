Shader "CpRemix/World/Events/Halloween/Pumpkin" 
{
    Properties 
    {
        _AdditiveColor ("Additive Color", Color) = (0,0,0,1)
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader 
    {
        Tags { "RenderType" = "Opaque" }
        Pass 
        {
            Tags { "RenderType" = "Opaque" }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };

            struct fout
            {
                float4 sv_target : SV_Target;
            };

            float4 _MainTex_ST;
            float4 _AdditiveColor;
            sampler2D _MainTex;

            v2f vert(appdata v)
            {
                v2f o;
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex); 
                o.position = mul(UNITY_MATRIX_VP, worldPos); 
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex); 
                o.color = v.color;
                return o;
            }

            fout frag(v2f i)
            {
                fout o;
                float4 texColor = tex2D(_MainTex, i.texcoord);
                o.sv_target = i.color * _AdditiveColor + texColor;
                return o;
            }

            ENDCG
        }
    }
}
