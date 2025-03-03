Shader "CpRemix/Combined Avatar Alpha" {
	Properties {
		_MainTex ("Diffuse Texture", 2D) = "white" {}
		_Alpha ("Alpha", Float) = 0
	}
	SubShader {
		Tags { "QUEUE" = "Transparent" "RenderType" = "Transparent" }
		Pass {
			Tags { "LIGHTMODE" = "FORWARDBASE" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			GpuProgramID 26519
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float2 texcoord : TEXCOORD0;
				float3 texcoord1 : TEXCOORD1;
				float3 color : COLOR0;
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			float4 _LightColor0;
			// $Globals ConstantBuffers for Fragment Shader
			float _Alpha;
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _MainTex;
			
			// Keywords: 
			v2f vert(appdata_full v)
{
    v2f o;
    float3 worldPos;
    float3 normalDir;
    float3 lightDir;
    float3 finalColor;
    
    // Transform vertex position to world space
    worldPos = mul((float3x3)unity_ObjectToWorld, v.vertex.xyz) + unity_ObjectToWorld._m03_m13_m23;

    // Calculate the light direction
    lightDir = normalize(_WorldSpaceLightPos0.xyz - worldPos * _WorldSpaceLightPos0.w);

    // Position in clip space
    o.position = mul(unity_MatrixVP, float4(worldPos, 1.0));

    // Pass through the texture coordinates
    o.texcoord.xy = v.texcoord.xy;

    // Normalize and transform normal
    normalDir = mul((float3x3)unity_ObjectToWorld, v.normal.xyz);
    normalDir = normalize(normalDir);

    // Calculate diffuse lighting using dot product between the light direction and the normal
    float NdotL = max(dot(normalDir, lightDir), 0.0);
    
    // Calculate the diffuse light intensity
    float3 diffuseLight = _LightColor0.xyz * NdotL * 0.75;

    // Ambient light contribution
    float3 ambientLight = glstate_lightmodel_ambient.xyz * 0.9;

    // Combine diffuse and ambient light
    finalColor = diffuseLight + ambientLight;

    // Adjust brightness
    finalColor = max(float3(0.6, 0.6, 0.6) * (NdotL + 0.5), finalColor);

    // Assign final color to output
    o.texcoord1.xyz = finalColor;
    o.color.xyz = v.color.xyz;
    
    return o;
}

			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                float4 tmp1;
                tmp0 = tex2D(_MainTex, inp.texcoord.xy);
                tmp0.w = tmp0.w * 2.0 + -1.0;
                tmp1.x = tmp0.w >= 0.0;
                tmp1.x = tmp1.x ? 1.0 : 0.0;
                tmp1.y = tmp0.w * tmp1.x;
                tmp0.w = -tmp0.w * tmp1.x + 1.0;
                tmp1.xyz = tmp0.xyz * tmp1.yyy;
                tmp0.xyz = tmp0.xyz * inp.texcoord1.xyz;
                o.sv_target.xyz = tmp0.xyz * tmp0.www + tmp1.xyz;
                o.sv_target.w = _Alpha;
                return o;
			}
			ENDCG
		}
	}
}