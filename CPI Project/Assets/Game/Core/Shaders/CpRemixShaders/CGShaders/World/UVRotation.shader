Shader "CpRemix/World/UV Rotation Unlit (No Fog)"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_RotationSpeed("Rotation Speed", float) = 2
		_PivotX("Pivot X", float) = 0.5
		_PivotY("Pivot Y", float) = 0.5
	}
	SubShader
	{
		Tags {
            "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
		LOD 100
		Pass
		{
			Tags { "RenderType" = "Opaque" }
			LOD 100
			HLSLPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			// Shader properties
			float _RotationSpeed;
			float _PivotX;
			float _PivotY;
			sampler2D _MainTex;

			struct v2f
			{
				float2 xlv_TEXCOORD0 : TEXCOORD0;
				float4 xlv_COLOR : COLOR;
				float4 pos : SV_POSITION;  // Only one SV_POSITION semantic
			};

			struct FragOutput
			{
				float4 gl_FragData : SV_Target;
			};

			v2f vert(
				float4 _glesVertex : POSITION,
				float4 _glesColor : COLOR,
				float4 _glesMultiTexCoord0 : TEXCOORD0
			)
			{
				v2f o;

				// Pivot for UV rotation
				float2 pivot = float2(_PivotX, _PivotY);

				// Calculate rotation angle and matrix
				float rotationAngle = _RotationSpeed * _Time.y;
				float cosTheta = cos(rotationAngle);
				float sinTheta = sin(rotationAngle);
				float2x2 rotationMatrix = float2x2(cosTheta, sinTheta, -sinTheta, cosTheta);

				// Rotate texture coordinates around the pivot
				float2 centeredTexCoord = _glesMultiTexCoord0.xy - pivot;
				o.xlv_TEXCOORD0 = mul(rotationMatrix, centeredTexCoord) + pivot;

				// Pass color
				o.xlv_COLOR = _glesColor;

				// Transform vertex position to clip space (SV_POSITION defined here)
				o.pos = TransformObjectToHClip(_glesVertex.xyz);

				return o;
			}

			FragOutput frag(v2f i)
			{
				FragOutput o;

				// Sample the texture
				float4 texColor = tex2D(_MainTex, i.xlv_TEXCOORD0);

				// Apply vertex color and return
				o.gl_FragData = texColor * i.xlv_COLOR;

				return o;
			}

			ENDHLSL
		}
	}
	Fallback Off
}
