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

                float2 texCoordOffset = float2(0.0039063, 0.0039063);
                float4 offsetCoord1 = inp.texcoord.xyxy - texCoordOffset.xyyy;
                float4 offsetCoord2 = inp.texcoord.xyxy + texCoordOffset.xyxy;

                float4 outlineDiff1 = abs(tex2D(_OutlineTex, offsetCoord1.xy) - outlineTexSample);
                float4 outlineDiff2 = abs(tex2D(_OutlineTex, offsetCoord1.zw) - outlineTexSample);
                float4 outlineDiff3 = abs(tex2D(_OutlineTex, offsetCoord2.xy) - outlineTexSample);
                float4 outlineDiff4 = abs(tex2D(_OutlineTex, offsetCoord2.zw) - outlineTexSample);

                float outlineWeight = dot(outlineDiff1.xyz, float3(0.2126, 0.7152, 0.0722)) * 0.707 +
                                      dot(outlineDiff2.xyz, float3(0.2126, 0.7152, 0.0722)) * 0.707 +
                                      dot(outlineDiff3.xyz, float3(0.2126, 0.7152, 0.0722)) * 0.5 +
                                      dot(outlineDiff4.xyz, float3(0.2126, 0.7152, 0.0722)) * 0.303;

                o.sv_target = _OutlineColor * outlineWeight + mainColor;
                return o;
            }
            ENDCG
        }
    }
}
