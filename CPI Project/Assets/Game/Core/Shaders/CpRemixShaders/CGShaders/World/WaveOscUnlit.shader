Shader "CpRemix/World/Wave Osc Unlit (Vertex Alpha)"
{
    Properties
    {
        _MainTex  ("Base (RGB)", 2D) = "white" {}
        _OscDir   ("World Osc  Dir", Vector) = (1,0,0,1)
        _OscAxis  ("World Osc Axs (w = wave freq)", Vector) = (0,1,0,1)
        _OscSpeed ("Osc Speed", Float) = 1
    }

    SubShader
    {
        Tags {
            "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" "DisableBatching" = "True" }

        Pass
        {
            Tags { "LightMode" = "UniversalForward" "RenderType" = "Opaque" }

            HLSLPROGRAM

            #pragma vertex   vert
            #pragma fragment frag
            #pragma target   4.0
            #pragma multi_compile_fog
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            sampler2D _MainTex;
            float4    _MainTex_ST;
            float3    _OscDir;
            float4    _OscAxis;
            float     _OscSpeed;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 color  : COLOR;
                float2 uv     : TEXCOORD0;
                float2 uv2    : TEXCOORD1;
            };

            struct v2f
            {
                float4 pos   : SV_POSITION;
                float3 color : COLOR;
                float2 uv    : TEXCOORD0;
                float2 uv2   : TEXCOORD1;
                float3 normalWS : TEXCOORD3;
                float fogFactor : TEXCOORD2;
            };

            v2f vert(appdata v)
            {
                v2f o;

                float3x3 w2o3     = (float3x3)unity_WorldToObject;
                float3 oscAxisObj = mul(w2o3, _OscAxis.xyz);
                float3 oscDirObj  = mul(w2o3, _OscDir);

                float phase     = dot(v.vertex.xyz, oscAxisObj) * _OscAxis.w;
                float amplitude = (1.0 - v.color.a) * sin(_Time.y * _OscSpeed + phase);

                float4 displaced = v.vertex;
                displaced.xyz += amplitude * oscDirObj;

                o.pos   = TransformObjectToHClip(displaced.xyz);
                o.color = v.color.rgb;
                o.uv    = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv2   = v.uv2 * unity_LightmapST.xy + unity_LightmapST.zw;
                o.normalWS = TransformObjectToWorldNormal(v.normal);

                o.fogFactor = ComputeFogFactor(o.pos.z);

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float3 lightmap = SampleLightmap(i.uv2, normalize(i.normalWS));

                float4 mainTex = tex2D(_MainTex, i.uv);
                float3 col     = lightmap * mainTex.rgb * i.color;

                float4 result = float4(col, 1.0);

                result.rgb = MixFog(result.rgb, i.fogFactor);

                return result;
            }

            ENDHLSL
        }
    }
}
