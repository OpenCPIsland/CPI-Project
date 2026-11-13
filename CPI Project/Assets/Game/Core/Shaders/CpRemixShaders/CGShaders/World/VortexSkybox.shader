Shader "CpRemix/Skybox/Vortex"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_RotationSpeed("Rotation Speed", Float) = 2
		_PivotX("Pivot X", Float) = 0.5
		_PivotY("Pivot Y", Float) = 0.5
		_Tint("Tint", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags
		{
            "RenderPipeline" = "UniversalPipeline"
			"Queue" = "Background"
			"RenderType" = "Background"
			"PreviewType" = "Skybox"
			"IgnoreProjector" = "True"
		}

		Cull Off
		ZWrite Off
		ZTest LEqual

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Tint;
			float _RotationSpeed;
			float _PivotX;
			float _PivotY;

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 screenPos : TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = TransformObjectToHClip(v.vertex.xyz);
				o.screenPos = ComputeScreenPos(o.pos);
				return o;
			}

			half4 frag(v2f i) : SV_Target
			{
				float2 uv = i.screenPos.xy / i.screenPos.w;
				uv = TRANSFORM_TEX(uv, _MainTex);

				float2 pivot = float2(_PivotX, _PivotY);

				float rotationAngle = _RotationSpeed * _Time.y;
				float cosTheta = cos(rotationAngle);
				float sinTheta = sin(rotationAngle);

				float2 centeredUV = uv - pivot;
				float2 rotatedUV;
				rotatedUV.x = centeredUV.x * cosTheta + centeredUV.y * sinTheta;
				rotatedUV.y = -centeredUV.x * sinTheta + centeredUV.y * cosTheta;
				rotatedUV += pivot;

				return tex2D(_MainTex, rotatedUV) * _Tint;
			}
			ENDHLSL
		}
	}

	Fallback Off
}