Shader "CpRemix/Skybox/Cubemap"
{
    Properties
    {
        _Cubemap("Cubemap (HDR)", Cube) = "white" {}        // Cubemap texture, marked as HDR
        _MainTex("Main Texture", 2D) = "white" {}            // 2D texture (for the main effect)
        _TintColor("Tint Color", Color) = (1, 1, 1, 1)      // Tint color for the skybox
        _Exposure("Exposure", Range(0, 10)) = 1.0           // Exposure control (brightness)
        _Rotation("Rotation", Range(0, 360)) = 0            // Rotation of the cubemap
    }
    SubShader
    {
        Tags { "RenderType" = "Skybox" "Queue" = "Background" } // Set render queue to 1000

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Samplers for cubemap and main texture
            samplerCUBE _Cubemap;    // HDR cubemap
            sampler2D _MainTex;
            float4 _TintColor;
            float _Exposure;
            float _Rotation;

            // Vertex structure
            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 uv : TEXCOORD0; // Store the UV coordinates for cubemap lookup
            };

            // Vertex shader - generates UV coordinates from the vertex position
            v2f vert(float4 vertex : POSITION)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(vertex);
                o.uv = vertex.xyz; // Pass the object-space position to fragment shader for cubemap lookup
                return o;
            }

            // Fragment shader - samples the cubemap and applies exposure, tint, and rotation
            float4 frag(v2f i) : SV_Target
            {
                // Rotate the cubemap UV coordinates based on the _Rotation value
                float angle = _Rotation * 3.14159265359 / 180.0; // Convert rotation to radians
                float s = sin(angle);
                float c = cos(angle);

                // Apply rotation to the UV coordinates (simple rotation matrix)
                float3 rotatedUV = float3(
                    c * i.uv.x - s * i.uv.y,
                    s * i.uv.x + c * i.uv.y,
                    i.uv.z
                );

                // Sample the cubemap using the rotated UVs (HDR cubemap)
                float4 cubemapColor = texCUBE(_Cubemap, rotatedUV);

                // Apply the _MainTex (2D texture) - sample it based on screen-space coordinates
                float2 screenUV = i.uv.xy * 0.5 + 0.5; // Convert from [-1,1] to [0,1] for 2D texture sampling
                float4 mainTexColor = tex2D(_MainTex, screenUV);

                // Blend cubemap and main texture
                float4 finalColor = cubemapColor * _TintColor + mainTexColor * (1.0 - _Exposure);

                // Apply exposure and tint
                finalColor *= _Exposure;
                finalColor = saturate(finalColor); // Ensure the final color is clamped to [0, 1]

                return finalColor;
            }
            ENDCG
        }
    }
    Fallback "Skybox/Cubemap"
}