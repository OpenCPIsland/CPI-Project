Shader "Hidden/ClothingOutlinerImageEffect" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineTex ("Outline", 2D) = "white" {}
        _OutlineColor ("Outline Color", Vector) = (1,1,1,1)
        _OutlineLookups ("Outline Lookups", Range(1, 8)) = 3
        _OutlineLookupDistance ("Outline Lookup Distance", Float) = 0.01
    }
    SubShader {
        Pass {
            ZTest Always
            ZWrite Off
            Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct v2f {
                float2 texcoord : TEXCOORD0;
                float4 position : SV_POSITION0;
            };

            struct fout {
                float4 sv_target : SV_Target0;
            };

            float4 _OutlineColor;
            sampler2D _MainTex;
            sampler2D _OutlineTex;

            v2f vert(appdata_full v) {
                v2f o;
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.texcoord = v.texcoord.xy;
                o.position = UnityObjectToClipPos(worldPos);
                return o;
            }

            fout frag(v2f inp) {
                fout o;
                float4 mainColor = tex2D(_MainTex, inp.texcoord);
                float4 outlineTexSample = tex2D(_OutlineTex, inp.texcoord);
                float outlineWeight = 0.0;

                // Coordinate offsets for outline detection
                float2 offset1 = float2(0.0039063, 0.0039063);
                float2 offset2 = float2(0.0117188, 0.0117188);

                // Sample neighboring texels and calculate outline differences
                outlineWeight += dot(abs(tex2D(_OutlineTex, inp.texcoord - offset1.xy).xyz - outlineTexSample.xyz), float3(0.2126, 0.7152, 0.0722)) * 0.707;
                outlineWeight += dot(abs(tex2D(_OutlineTex, inp.texcoord + offset1.xy).xyz - outlineTexSample.xyz), float3(0.2126, 0.7152, 0.0722)) * 0.707;
                outlineWeight += dot(abs(tex2D(_OutlineTex, inp.texcoord - offset2.xy).xyz - outlineTexSample.xyz), float3(0.2126, 0.7152, 0.0722)) * 0.303;
                outlineWeight += dot(abs(tex2D(_OutlineTex, inp.texcoord + offset2.xy).xyz - outlineTexSample.xyz), float3(0.2126, 0.7152, 0.0722)) * 0.303;

                // Normalize outline weight
                outlineWeight *= 0.1666667;

                // Combine outline color and main texture color
                o.sv_target = _OutlineColor * outlineWeight + mainColor;
                return o;
            }
            ENDCG
        }
    }
}
