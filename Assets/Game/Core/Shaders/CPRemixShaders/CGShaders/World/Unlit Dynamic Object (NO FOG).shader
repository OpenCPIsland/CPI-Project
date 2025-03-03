Shader "CpRemix/World/Unlit Dynamic Object (NO FOG)"
{
    Properties
    {
        _TintColor ("Tint Color", Vector) = (1, 1, 1, 1)
        _AdditiveColor ("Additive Color", Color) = (0, 0, 0, 1) // Additive color property
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            Tags { "RenderType" = "Opaque" }
            GpuProgramID 51075
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            struct v2f
            {
                float4 position : SV_POSITION0;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR0;
            };
            
            struct fout
            {
                float4 sv_target : SV_Target0;
            };
            
            // $Globals ConstantBuffers for Vertex Shader
            float4 _MainTex_ST;
            // $Globals ConstantBuffers for Fragment Shader
            float4 _TintColor;
            float4 _AdditiveColor; // Reference to _AdditiveColor
            
            // Texture params for Vertex Shader
            sampler2D _MainTex;
            
            // Vertex shader function
            v2f vert(appdata_full v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.vertex);
                o.texcoord.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                o.color = v.color;
                return o;
            }
            
            // Fragment shader function
            fout frag(v2f inp)
            {
                fout o;
                float4 tmp0 = tex2D(_MainTex, inp.texcoord.xy);
                tmp0 = tmp0 * inp.color;
                
                // Apply the additive color effect
                float4 finalColor = tmp0 * _TintColor; // Apply tint color
                finalColor.rgb += _AdditiveColor.rgb * _AdditiveColor.a; // Additive effect based on alpha

                o.sv_target = saturate(finalColor); // Ensure color doesn't exceed 1.0
                return o;
            }
            ENDCG
        }
    }
}
