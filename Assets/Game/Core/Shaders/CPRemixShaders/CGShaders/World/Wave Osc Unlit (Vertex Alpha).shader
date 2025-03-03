Shader "CpRemix/World/Wave Osc Unlit (Vertex Alpha)"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _OscDir ("World Osc Dir", Vector) = (1,0,0,1)
        _OscAxis ("World Osc Axis (w = wave freq)", Vector) = (0,1,0,1)
        _OscSpeed ("Osc Speed", Float) = 1
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
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct v2f
            {
                float4 position : SV_POSITION0;
                float3 color : COLOR0;
                float2 texcoord : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };

            float4 _MainTex_ST;
            float3 _OscDir;
            float4 _OscAxis;
            float _OscSpeed;
            sampler2D _MainTex;

            v2f vert(appdata_full v)
            {
                v2f o;

                // Calculate the wave effect based on object space
                float3 objPos = v.vertex.xyz;
                float waveFactor = dot(objPos, _OscAxis.xyz) * _OscAxis.w;
                waveFactor = sin(_Time.y * _OscSpeed + waveFactor);

                // Modify vertex position by the wave factor
                objPos += _OscDir * waveFactor * (1.0 - v.color.w);

                // Transform to clip space using Unity's built-in function
                o.position = UnityObjectToClipPos(float4(objPos, 1.0));

                // Pass texture coordinates and color
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color.rgb;

                UNITY_TRANSFER_FOG(o, o.position);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                // Sample texture
                float4 texColor = tex2D(_MainTex, i.texcoord);
                texColor.rgb *= i.color;

                UNITY_APPLY_FOG(i.fogCoord, texColor);

                return texColor;
            }

            ENDCG
        }
    }
    FallBack Off
}
