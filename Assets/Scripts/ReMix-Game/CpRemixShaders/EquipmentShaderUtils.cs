using System;
using System.Collections.Generic;
using UnityEngine;

namespace CpRemixShaders
{
	public static class EquipmentShaderUtils
	{
		public const int MAX_REFERENCE_TEXTURE_DIMENSION = 512;

		private static Shader _equipmentBakeShader;

		private static Shader _equipmentPreviewShader;

		private static Shader _bodyBakeShader;

		private static Shader _bodyPreviewShader;

		public static Shader GetEquipmentBakeShader()
		{
			if (_equipmentBakeShader == null)
			{
				_equipmentBakeShader = Shader.Find("CpRemix/Equipment Bake");
				if (_equipmentBakeShader == null)
				{
					throw new NullReferenceException("Could not find shader: CpRemix/Equipment Bake. Make sure it is referenced somewhere in the program.");
				}
			}
			return _equipmentBakeShader;
		}

		public static bool IsEquipmentBakeShader(Shader shader)
		{
			return shader.name == GetEquipmentBakeShader().name;
		}

		public static Shader GetEquipmentPreviewShader()
		{
			if (_equipmentPreviewShader == null)
			{
				_equipmentPreviewShader = Shader.Find("CpRemix/Equipment Preview");
				if (_equipmentPreviewShader == null)
				{
					throw new NullReferenceException("Could not find shader: CpRemix/Equipment Preview. Make sure it is referenced somewhere in the program.");
				}
			}
			return _equipmentPreviewShader;
		}

		public static bool IsEquipmentPreviewShader(Shader shader)
		{
			return shader.name == GetEquipmentPreviewShader().name;
		}

		public static Shader GetBodyBakeShader()
		{
			if (_bodyBakeShader == null)
			{
				_bodyBakeShader = Shader.Find("CpRemix/Avatar Body Bake");
				if (_bodyBakeShader == null)
				{
					throw new NullReferenceException("Could not find shader: CpRemix/Avatar Body Bake. Make sure it is referenced somewhere in the program.");
				}
			}
			return _bodyBakeShader;
		}

		public static bool IsBodyBakeShader(Shader shader)
		{
			return shader.name == GetBodyBakeShader().name;
		}

		public static Shader GetBodyPreviewShader()
		{
			if (_bodyPreviewShader == null)
			{
				_bodyPreviewShader = Shader.Find("CpRemix/Avatar Body Preview");
				if (_bodyPreviewShader == null)
				{
					throw new NullReferenceException("Could not find shader: CpRemix/Avatar Body Preview. Make sure it is referenced somewhere in the program.");
				}
			}
			return _bodyPreviewShader;
		}

		public static bool IsBodyPreviewShader(Shader shader)
		{
			return shader.name == GetBodyPreviewShader().name;
		}

		private static void validateEquipmentShader(Material material)
		{
			if (!IsEquipmentPreviewShader(material.shader) && !IsEquipmentBakeShader(material.shader))
			{
				throw new Exception("EquipmentPart material must use CPRemix preview or bake shader.");
			}
		}

		public static void ApplyDecalTexture(DecalColorChannel decalColorChannel, Texture2D decalTexture, Renderer renderer)
		{
			int nameID = 0;
			switch (decalColorChannel)
			{
			case DecalColorChannel.RED_1:
				nameID = EquipmentShaderParams.DECAL_RED_1_TEX;
				break;
			case DecalColorChannel.GREEN_2:
				nameID = EquipmentShaderParams.DECAL_GREEN_2_TEX;
				break;
			case DecalColorChannel.BLUE_3:
				nameID = EquipmentShaderParams.DECAL_BLUE_3_TEX;
				break;
			case DecalColorChannel.RED_4:
				nameID = EquipmentShaderParams.DECAL_RED_4_TEX;
				break;
			case DecalColorChannel.GREEN_5:
				nameID = EquipmentShaderParams.DECAL_GREEN_5_TEX;
				break;
			case DecalColorChannel.BLUE_6:
				nameID = EquipmentShaderParams.DECAL_BLUE_6_TEX;
				break;
			}
			Material material = renderer.material;
			validateEquipmentShader(material);
			material.SetTexture(nameID, decalTexture);
		}

		public static void ApplyDecalTexture(DecalColorChannel decalColorChannel, Texture2D decalTexture, List<Renderer> renderers)
		{
			int nameID = 0;
			switch (decalColorChannel)
			{
			case DecalColorChannel.RED_1:
				nameID = EquipmentShaderParams.DECAL_RED_1_TEX;
				break;
			case DecalColorChannel.GREEN_2:
				nameID = EquipmentShaderParams.DECAL_GREEN_2_TEX;
				break;
			case DecalColorChannel.BLUE_3:
				nameID = EquipmentShaderParams.DECAL_BLUE_3_TEX;
				break;
			case DecalColorChannel.RED_4:
				nameID = EquipmentShaderParams.DECAL_RED_4_TEX;
				break;
			case DecalColorChannel.GREEN_5:
				nameID = EquipmentShaderParams.DECAL_GREEN_5_TEX;
				break;
			case DecalColorChannel.BLUE_6:
				nameID = EquipmentShaderParams.DECAL_BLUE_6_TEX;
				break;
			}
			for (int i = 0; i < renderers.Count; i++)
			{
				Material material = renderers[i].material;
				validateEquipmentShader(material);
				material.SetTexture(nameID, decalTexture);
			}
		}

		public static void ApplyDecalColorToAllChannels(Color color, List<Renderer> renderers)
		{
			ApplyDecalColor(DecalColorChannel.RED_1, color, renderers);
			ApplyDecalColor(DecalColorChannel.GREEN_2, color, renderers);
			ApplyDecalColor(DecalColorChannel.BLUE_3, color, renderers);
			ApplyDecalColor(DecalColorChannel.RED_4, color, renderers);
			ApplyDecalColor(DecalColorChannel.GREEN_5, color, renderers);
			ApplyDecalColor(DecalColorChannel.BLUE_6, color, renderers);
		}

		public static void ApplyDecalColor(DecalColorChannel decalColorChannel, Color color, List<Renderer> renderers)
		{
			int nameID = 0;
			switch (decalColorChannel)
			{
			case DecalColorChannel.RED_1:
				nameID = EquipmentShaderParams.DECAL_RED_1_COLOR;
				break;
			case DecalColorChannel.GREEN_2:
				nameID = EquipmentShaderParams.DECAL_GREEN_2_COLOR;
				break;
			case DecalColorChannel.BLUE_3:
				nameID = EquipmentShaderParams.DECAL_BLUE_3_COLOR;
				break;
			case DecalColorChannel.RED_4:
				nameID = EquipmentShaderParams.DECAL_RED_4_COLOR;
				break;
			case DecalColorChannel.GREEN_5:
				nameID = EquipmentShaderParams.DECAL_GREEN_5_COLOR;
				break;
			case DecalColorChannel.BLUE_6:
				nameID = EquipmentShaderParams.DECAL_BLUE_6_COLOR;
				break;
			}
			for (int i = 0; i < renderers.Count; i++)
			{
				Material material = renderers[i].material;
				validateEquipmentShader(material);
				material.SetColor(nameID, color);
			}
		}

		public static void ApplyDecalOffset(DecalColorChannel decalColorChannel, Vector2 offsetFromCenter, List<Renderer> renderers)
		{
			int nameID = 0;
			int nameID2 = 0;
			switch (decalColorChannel)
			{
			case DecalColorChannel.RED_1:
				nameID = EquipmentShaderParams.DECAL_RED_1_U_OFFSET;
				nameID2 = EquipmentShaderParams.DECAL_RED_1_V_OFFSET;
				break;
			case DecalColorChannel.GREEN_2:
				nameID = EquipmentShaderParams.DECAL_GREEN_2_U_OFFSET;
				nameID2 = EquipmentShaderParams.DECAL_GREEN_2_V_OFFSET;
				break;
			case DecalColorChannel.BLUE_3:
				nameID = EquipmentShaderParams.DECAL_BLUE_3_U_OFFSET;
				nameID2 = EquipmentShaderParams.DECAL_BLUE_3_V_OFFSET;
				break;
			case DecalColorChannel.RED_4:
				nameID = EquipmentShaderParams.DECAL_RED_4_U_OFFSET;
				nameID2 = EquipmentShaderParams.DECAL_RED_4_V_OFFSET;
				break;
			case DecalColorChannel.GREEN_5:
				nameID = EquipmentShaderParams.DECAL_GREEN_5_U_OFFSET;
				nameID2 = EquipmentShaderParams.DECAL_GREEN_5_V_OFFSET;
				break;
			case DecalColorChannel.BLUE_6:
				nameID = EquipmentShaderParams.DECAL_BLUE_6_U_OFFSET;
				nameID2 = EquipmentShaderParams.DECAL_BLUE_6_V_OFFSET;
				break;
			}
			for (int i = 0; i < renderers.Count; i++)
			{
				Material material = renderers[i].material;
				validateEquipmentShader(material);
				material.SetFloat(nameID, offsetFromCenter.x);
				material.SetFloat(nameID2, offsetFromCenter.y);
			}
		}

		public static void ApplyDecalRepeat(DecalColorChannel decalColorChannel, bool repeat, List<Renderer> renderers)
		{
			int nameID = 0;
			switch (decalColorChannel)
			{
			case DecalColorChannel.RED_1:
				nameID = EquipmentShaderParams.DECAL_RED_1_REPEAT;
				break;
			case DecalColorChannel.GREEN_2:
				nameID = EquipmentShaderParams.DECAL_GREEN_2_REPEAT;
				break;
			case DecalColorChannel.BLUE_3:
				nameID = EquipmentShaderParams.DECAL_BLUE_3_REPEAT;
				break;
			case DecalColorChannel.RED_4:
				nameID = EquipmentShaderParams.DECAL_RED_4_REPEAT;
				break;
			case DecalColorChannel.GREEN_5:
				nameID = EquipmentShaderParams.DECAL_GREEN_5_REPEAT;
				break;
			case DecalColorChannel.BLUE_6:
				nameID = EquipmentShaderParams.DECAL_BLUE_6_REPEAT;
				break;
			}
			for (int i = 0; i < renderers.Count; i++)
			{
				Material material = renderers[i].material;
				validateEquipmentShader(material);
				material.SetFloat(nameID, repeat ? 1f : 0f);
			}
		}

		public static void ApplyDecalScale(DecalColorChannel decalColorChannel, float scale, List<Renderer> renderers)
		{
			int nameID = 0;
			switch (decalColorChannel)
			{
			case DecalColorChannel.RED_1:
				nameID = EquipmentShaderParams.DECAL_RED_1_SCALE;
				break;
			case DecalColorChannel.GREEN_2:
				nameID = EquipmentShaderParams.DECAL_GREEN_2_SCALE;
				break;
			case DecalColorChannel.BLUE_3:
				nameID = EquipmentShaderParams.DECAL_BLUE_3_SCALE;
				break;
			case DecalColorChannel.RED_4:
				nameID = EquipmentShaderParams.DECAL_RED_4_SCALE;
				break;
			case DecalColorChannel.GREEN_5:
				nameID = EquipmentShaderParams.DECAL_GREEN_5_SCALE;
				break;
			case DecalColorChannel.BLUE_6:
				nameID = EquipmentShaderParams.DECAL_BLUE_6_SCALE;
				break;
			}
			for (int i = 0; i < renderers.Count; i++)
			{
				Material material = renderers[i].material;
				validateEquipmentShader(material);
				material.SetFloat(nameID, scale);
			}
		}

		public static void ApplyDecalRotation(DecalColorChannel decalColorChannel, float rotationDegrees, List<Renderer> renderers)
		{
			int nameID = 0;
			switch (decalColorChannel)
			{
			case DecalColorChannel.RED_1:
				nameID = EquipmentShaderParams.DECAL_RED_1_ROTATION_RADS;
				break;
			case DecalColorChannel.GREEN_2:
				nameID = EquipmentShaderParams.DECAL_GREEN_2_ROTATION_RADS;
				break;
			case DecalColorChannel.BLUE_3:
				nameID = EquipmentShaderParams.DECAL_BLUE_3_ROTATION_RADS;
				break;
			case DecalColorChannel.RED_4:
				nameID = EquipmentShaderParams.DECAL_RED_4_ROTATION_RADS;
				break;
			case DecalColorChannel.GREEN_5:
				nameID = EquipmentShaderParams.DECAL_GREEN_5_ROTATION_RADS;
				break;
			case DecalColorChannel.BLUE_6:
				nameID = EquipmentShaderParams.DECAL_BLUE_6_ROTATION_RADS;
				break;
			}
			for (int i = 0; i < renderers.Count; i++)
			{
				Material material = renderers[i].material;
				validateEquipmentShader(material);
				material.SetFloat(nameID, rotationDegrees);
			}
		}

		public static void ApplyBodyColors(Color redChannelColor, Color greenChannelColor, Color blueChannelColor, List<Renderer> renderers)
		{
			for (int i = 0; i < renderers.Count; i++)
			{
				Material material = renderers[i].material;
				validateEquipmentShader(material);
				material.SetColor(EquipmentShaderParams.BODY_RED_CHANNEL_COLOR, redChannelColor);
				material.SetColor(EquipmentShaderParams.BODY_GREEN_CHANNEL_COLOR, greenChannelColor);
				material.SetColor(EquipmentShaderParams.BODY_BLUE_CHANNEL_COLOR, blueChannelColor);
			}
		}

		public static void ApplyEmissiveColor(Color emissiveColor, List<Renderer> renderers)
		{
			for (int i = 0; i < renderers.Count; i++)
			{
				Material material = renderers[i].material;
				validateEquipmentShader(material);
				material.SetColor(EquipmentShaderParams.EMISSIVE_COLOR_TINT, emissiveColor);
			}
		}
	}
}
