Shader "CpRemix/World/Unlit Dynamic Object (FOG)"
{
    Properties
    {
        _TintColor ("Tint Color", Color) = (1, 1, 1, 1)
        [HideInInspector] _TintColorFloat ("Tint Color Float", Float) = 1.0
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
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _TintColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 texcoord : TEXCOORD0;
                float4 color : COLOR;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color    : COLOR0;
                float fogFactor : TEXCOORD1;
            };

            struct fout
            {
                float4 sv_target : SV_Target0;
            };

            v2f vert(appdata v)
            {
                UNITY_SETUP_INSTANCE_ID(v);

                v2f o;

                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.position = mul(UNITY_MATRIX_VP, worldPos);
                o.texcoord.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                o.color = v.color;

                o.fogFactor = ComputeFogFactor(o.position.z);

                return o;
            }

            fout frag(v2f inp)
            {
                fout o;

                float4 tmp0 = tex2D(_MainTex, inp.texcoord.xy);
                tmp0 = tmp0 * inp.color;
                o.sv_target = tmp0 * _TintColor;

                o.sv_target.rgb = MixFog(o.sv_target.rgb, inp.fogFactor);
                o.sv_target.w = 1.0;

                return o;
            }
            ENDHLSL
        }
    }
}