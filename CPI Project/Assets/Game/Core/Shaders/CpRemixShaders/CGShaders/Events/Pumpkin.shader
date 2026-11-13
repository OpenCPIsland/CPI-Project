Shader "CpRemix/World/Events/Halloween/Pumpkin" 
{
    Properties 
    {
        _AdditiveColor ("Additive Color", Color) = (0,0,0,1)
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader 
    {
        Tags {
            "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
        Pass 
        {
            Tags { "RenderType" = "Opaque" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            struct appdata
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;

                UNITY_VERTEX_INPUT_INSTANCE_ID
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
                UNITY_SETUP_INSTANCE_ID(v);

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

            ENDHLSL
        }
    }
}
