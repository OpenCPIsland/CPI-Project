Shader "CpRemix/Avatar Body Preview" {
	Properties {
		_Diffuse ("Diffuse", 2D) = "black" {}
		_BodyColorsMaskTex ("Body Color Mask", 2D) = "black" {}
		_BodyRedChannelColor ("Body Red Channel Color", Vector) = (1,0,0,1)
		_BodyGreenChannelColor ("Body Green Channel Color", Vector) = (1,1,0,1)
		_BodyBlueChannelColor ("Body Blue Channel Color", Vector) = (1,0,1,1)
		_DetailAndMatcapMaskAndEmissive ("r=detail g=MatCapMask b=emissive", 2D) = "black" {}
	}
	SubShader {
		Pass {
			Tags { "LIGHTMODE" = "ALWAYS" }
			GpuProgramID 61298
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float2 texcoord : TEXCOORD0;
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			// $Globals ConstantBuffers for Fragment Shader
			float3 _BodyRedChannelColor;
			float3 _BodyGreenChannelColor;
			float3 _BodyBlueChannelColor;
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _Diffuse;
			sampler2D _BodyColorsMaskTex;
			
			// Keywords: 
v2f vert(appdata_full v) {
    v2f o;

    // Use Unity's built-in matrix multiplication for transformation
    o.position = UnityObjectToClipPos(v.vertex);

    // Pass the texture coordinates
    o.texcoord.xy = v.texcoord.xy;

    return o;
}

			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                float4 tmp1;
                tmp0 = tex2D(_BodyColorsMaskTex, inp.texcoord.xy);
                tmp1.xyz = tmp0.yyy * _BodyGreenChannelColor;
                tmp1.xyz = tmp0.xxx * _BodyRedChannelColor + tmp1.xyz;
                tmp1.xyz = tmp0.zzz * _BodyBlueChannelColor + tmp1.xyz;
                tmp0.x = max(tmp0.y, tmp0.x);
                tmp0.x = max(tmp0.z, tmp0.x);
                tmp0.yzw = tmp0.xxx * tmp1.xyz;
                tmp0.x = 1.0 - tmp0.x;
                tmp1 = tex2D(_Diffuse, inp.texcoord.xy);
                o.sv_target.xyz = tmp1.xyz * tmp0.xxx + tmp0.yzw;
                o.sv_target.w = 1.0;
                return o;
			}
			ENDCG
		}
		Pass {
			Tags { "LIGHTMODE" = "FORWARDBASE" }
			Blend DstColor SrcColor, DstColor SrcColor
			GpuProgramID 68099
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
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _DetailAndMatcapMaskAndEmissive;
			
			// Keywords: 
			v2f vert(appdata_full v) {
    v2f o;

    // Use Unity's built-in matrix multiplication for transformation
    o.position = UnityObjectToClipPos(v.vertex);

    // Pass texture coordinates
    o.texcoord.xy = v.texcoord.xy;

    // World-space normal transformation using built-in functions
    float3 worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
    float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
    
    // Calculate diffuse light contribution
    float NdotL = max(0.0, dot(worldNormal, lightDir));
    float3 diffuse = _LightColor0.rgb * NdotL;
    
    // Ambient light contribution
    float3 ambient = glstate_lightmodel_ambient.xyz * 0.9;

    // Final texture coordinate and color output
    o.texcoord1.xyz = diffuse * 0.75 + ambient;
    o.color.xyz = v.color.xyz;

    return o;
}

			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                tmp0 = tex2D(_DetailAndMatcapMaskAndEmissive, inp.texcoord.xy);
                tmp0.xyz = tmp0.xxx * inp.texcoord1.xyz;
                o.sv_target.xyz = tmp0.xyz * float3(0.47, 0.47, 0.47);
                o.sv_target.w = 1.0;
                return o;
			}
			ENDCG
		}
	}
}