Shader "Custom/TimeBasedSkyboxWithOptionalTimeControl"
{
    Properties
    {
        // Cubemaps for the skybox phases
        _CubemapDay ("Day Cubemap", CUBE) = "" {}
        _CubemapSunset ("Sunset Cubemap", CUBE) = "" {}
        _CubemapNight ("Night Cubemap", CUBE) = "" {}
        _CubemapSunrise ("Sunrise Cubemap", CUBE) = "" {}

        // 2D Textures for the skybox phases
        _TextureDay ("Day Texture", 2D) = "" {}
        _TextureSunset ("Sunset Texture", 2D) = "" {}
        _TextureNight ("Night Texture", 2D) = "" {}
        _TextureSunrise ("Sunrise Texture", 2D) = "" {}

        // Control which type to use (0 = Cubemap, 1 = Texture)
        _UseTextures ("Use Textures instead of Cubemaps", Float) = 0.0

        // Optional time control for hours, minutes, and seconds
        _DayCycleLengthHours ("Day Cycle Length (hours) [Optional]", Range(0, 24)) = 0.0
        _DayCycleLengthMinutes ("Day Cycle Length (minutes) [Optional]", Range(0, 60)) = 15.0
        _DayCycleLengthSeconds ("Day Cycle Length (seconds) [Optional]", Range(0, 60)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Cubemap variables
            samplerCUBE _CubemapDay;
            samplerCUBE _CubemapSunset;
            samplerCUBE _CubemapNight;
            samplerCUBE _CubemapSunrise;

            // 2D texture variables
            sampler2D _TextureDay;
            sampler2D _TextureSunset;
            sampler2D _TextureNight;
            sampler2D _TextureSunrise;

            // Control variable for texture or cubemap
            float _UseTextures;

            // Optional time control variables
            float _DayCycleLengthHours;
            float _DayCycleLengthMinutes;
            float _DayCycleLengthSeconds;

            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 texcoord : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.vertex.xyz;
                o.uv = v.vertex.xy * 0.5 + 0.5; // Use UVs for 2D textures
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Calculate the total cycle time in seconds (hours + minutes + seconds)
                float totalCycleTimeSeconds = (_DayCycleLengthHours * 3600.0) + (_DayCycleLengthMinutes * 60.0) + _DayCycleLengthSeconds;

                // If all values are zero, we default to a 24-hour cycle (to avoid division by zero)
                if (totalCycleTimeSeconds == 0.0)
                {
                    totalCycleTimeSeconds = 24.0 * 3600.0; // Default to 24 hours
                }

                // Calculate the time progression normalized to 0-1 over the cycle
                float time = fmod(_Time.y, totalCycleTimeSeconds) / totalCycleTimeSeconds;

                float3 blendedColor;

                if (_UseTextures == 0.0) // Use cubemaps
                {
                    float3 texDay = texCUBE(_CubemapDay, i.texcoord).rgb;
                    float3 texSunset = texCUBE(_CubemapSunset, i.texcoord).rgb;
                    float3 texNight = texCUBE(_CubemapNight, i.texcoord).rgb;
                    float3 texSunrise = texCUBE(_CubemapSunrise, i.texcoord).rgb;

                    if (time < 0.25) // Day -> Sunset
                    {
                        float t = time / 0.25;
                        blendedColor = lerp(texDay, texSunset, t);
                    }
                    else if (time < 0.5) // Sunset -> Night
                    {
                        float t = (time - 0.25) / 0.25;
                        blendedColor = lerp(texSunset, texNight, t);
                    }
                    else if (time < 0.75) // Night -> Sunrise
                    {
                        float t = (time - 0.5) / 0.25;
                        blendedColor = lerp(texNight, texSunrise, t);
                    }
                    else // Sunrise -> Day
                    {
                        float t = (time - 0.75) / 0.25;
                        blendedColor = lerp(texSunrise, texDay, t);
                    }
                }
                else // Use 2D textures
                {
                    float3 texDay = tex2D(_TextureDay, i.uv).rgb;
                    float3 texSunset = tex2D(_TextureSunset, i.uv).rgb;
                    float3 texNight = tex2D(_TextureNight, i.uv).rgb;
                    float3 texSunrise = tex2D(_TextureSunrise, i.uv).rgb;

                    if (time < 0.25) // Day -> Sunset
                    {
                        float t = time / 0.25;
                        blendedColor = lerp(texDay, texSunset, t);
                    }
                    else if (time < 0.5) // Sunset -> Night
                    {
                        float t = (time - 0.25) / 0.25;
                        blendedColor = lerp(texSunset, texNight, t);
                    }
                    else if (time < 0.75) // Night -> Sunrise
                    {
                        float t = (time - 0.5) / 0.25;
                        blendedColor = lerp(texNight, texSunrise, t);
                    }
                    else // Sunrise -> Day
                    {
                        float t = (time - 0.75) / 0.25;
                        blendedColor = lerp(texSunrise, texDay, t);
                    }
                }

                return half4(blendedColor, 1.0);
            }
            ENDCG
        }
    }
    FallBack "RenderType"
}
