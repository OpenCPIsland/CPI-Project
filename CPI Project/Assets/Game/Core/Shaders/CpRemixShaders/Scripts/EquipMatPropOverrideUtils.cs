using System;
using MixAvatar;
using UnityEngine;

namespace CpRemixShaders
{
	public class EquipMatPropOverrideUtils
	{
		public static EquipPartMatPropsOverride[] GetMaterialPropOverridesForParts(EquipmentPart[] parts)
		{
			EquipPartMatPropsOverride[] array = new EquipPartMatPropsOverride[parts.Length];
			for (int i = 0; i < array.Length; i++)
			{
				EquipmentShaderParams propertiesOverride = EquipmentShaderParams.FromMaterial(parts[i].SkinnedMeshRenderer.sharedMaterial);
				array[i] = new EquipPartMatPropsOverride(parts[i].SkinnedMeshRenderer.sharedMaterial, propertiesOverride);
			}
			return array;
		}

		public static void OverridePartMaterialsDecalTexture(DecalColorChannel decalColorChannel, Texture2D decalTexture, EquipPartMatPropsOverride[] partMaterialOverrides)
		{
			for (int i = 0; i < partMaterialOverrides.Length; i++)
			{
				switch (decalColorChannel)
				{
				case DecalColorChannel.RED_1:
					partMaterialOverrides[i].PropertiesOverride.DecalRed1Texture = decalTexture;
					break;
				case DecalColorChannel.GREEN_2:
					partMaterialOverrides[i].PropertiesOverride.DecalGreen2Texture = decalTexture;
					break;
				case DecalColorChannel.BLUE_3:
					partMaterialOverrides[i].PropertiesOverride.DecalBlue3Texture = decalTexture;
					break;
				case DecalColorChannel.RED_4:
					partMaterialOverrides[i].PropertiesOverride.DecalRed4Texture = decalTexture;
					break;
				case DecalColorChannel.GREEN_5:
					partMaterialOverrides[i].PropertiesOverride.DecalGreen5Texture = decalTexture;
					break;
				case DecalColorChannel.BLUE_6:
					partMaterialOverrides[i].PropertiesOverride.DecalBlue6Texture = decalTexture;
					break;
				}
			}
		}

		public static void OverridePartMaterialsDecalColor(DecalColorChannel decalColorChannel, Color color, EquipPartMatPropsOverride[] partMaterialOverrides)
		{
			for (int i = 0; i < partMaterialOverrides.Length; i++)
			{
				switch (decalColorChannel)
				{
				case DecalColorChannel.RED_1:
					partMaterialOverrides[i].PropertiesOverride.DecalRed1Color = color;
					break;
				case DecalColorChannel.GREEN_2:
					partMaterialOverrides[i].PropertiesOverride.DecalGreen2Color = color;
					break;
				case DecalColorChannel.BLUE_3:
					partMaterialOverrides[i].PropertiesOverride.DecalBlue3Color = color;
					break;
				case DecalColorChannel.RED_4:
					partMaterialOverrides[i].PropertiesOverride.DecalRed4Color = color;
					break;
				case DecalColorChannel.GREEN_5:
					partMaterialOverrides[i].PropertiesOverride.DecalGreen5Color = color;
					break;
				case DecalColorChannel.BLUE_6:
					partMaterialOverrides[i].PropertiesOverride.DecalBlue6Color = color;
					break;
				}
			}
		}

		public static void OverridePartMaterialsDecalOffset(DecalColorChannel decalColorChannel, Vector2 offsetFromCenter, EquipPartMatPropsOverride[] partMaterialOverrides)
		{
			for (int i = 0; i < partMaterialOverrides.Length; i++)
			{
				switch (decalColorChannel)
				{
				case DecalColorChannel.RED_1:
					partMaterialOverrides[i].PropertiesOverride.DecalRed1UOffset = offsetFromCenter.x;
					partMaterialOverrides[i].PropertiesOverride.DecalRed1VOffset = offsetFromCenter.y;
					break;
				case DecalColorChannel.GREEN_2:
					partMaterialOverrides[i].PropertiesOverride.DecalGreen2UOffset = offsetFromCenter.x;
					partMaterialOverrides[i].PropertiesOverride.DecalGreen2VOffset = offsetFromCenter.y;
					break;
				case DecalColorChannel.BLUE_3:
					partMaterialOverrides[i].PropertiesOverride.DecalBlue3UOffset = offsetFromCenter.x;
					partMaterialOverrides[i].PropertiesOverride.DecalBlue3VOffset = offsetFromCenter.y;
					break;
				case DecalColorChannel.RED_4:
					partMaterialOverrides[i].PropertiesOverride.DecalRed4UOffset = offsetFromCenter.x;
					partMaterialOverrides[i].PropertiesOverride.DecalRed4VOffset = offsetFromCenter.y;
					break;
				case DecalColorChannel.GREEN_5:
					partMaterialOverrides[i].PropertiesOverride.DecalGreen5UOffset = offsetFromCenter.x;
					partMaterialOverrides[i].PropertiesOverride.DecalGreen5VOffset = offsetFromCenter.y;
					break;
				case DecalColorChannel.BLUE_6:
					partMaterialOverrides[i].PropertiesOverride.DecalBlue6UOffset = offsetFromCenter.x;
					partMaterialOverrides[i].PropertiesOverride.DecalBlue6VOffset = offsetFromCenter.y;
					break;
				}
			}
		}

		public static void OverridePartMaterialsDecalRepeat(DecalColorChannel decalColorChannel, bool repeat, EquipPartMatPropsOverride[] partMaterialOverrides)
		{
			for (int i = 0; i < partMaterialOverrides.Length; i++)
			{
				switch (decalColorChannel)
				{
				case DecalColorChannel.RED_1:
					partMaterialOverrides[i].PropertiesOverride.DecalRed1Repeat = repeat;
					break;
				case DecalColorChannel.GREEN_2:
					partMaterialOverrides[i].PropertiesOverride.DecalGreen2Repeat = repeat;
					break;
				case DecalColorChannel.BLUE_3:
					partMaterialOverrides[i].PropertiesOverride.DecalBlue3Repeat = repeat;
					break;
				case DecalColorChannel.RED_4:
					partMaterialOverrides[i].PropertiesOverride.DecalRed4Repeat = repeat;
					break;
				case DecalColorChannel.GREEN_5:
					partMaterialOverrides[i].PropertiesOverride.DecalGreen5Repeat = repeat;
					break;
				case DecalColorChannel.BLUE_6:
					partMaterialOverrides[i].PropertiesOverride.DecalBlue6Repeat = repeat;
					break;
				}
			}
		}

		public static void OverridePartMaterialsDecalScale(DecalColorChannel decalColorChannel, float scale, EquipPartMatPropsOverride[] partMaterialOverrides)
		{
			for (int i = 0; i < partMaterialOverrides.Length; i++)
			{
				switch (decalColorChannel)
				{
				case DecalColorChannel.RED_1:
					partMaterialOverrides[i].PropertiesOverride.DecalRed1Scale = scale;
					break;
				case DecalColorChannel.GREEN_2:
					partMaterialOverrides[i].PropertiesOverride.DecalGreen2Scale = scale;
					break;
				case DecalColorChannel.BLUE_3:
					partMaterialOverrides[i].PropertiesOverride.DecalBlue3Scale = scale;
					break;
				case DecalColorChannel.RED_4:
					partMaterialOverrides[i].PropertiesOverride.DecalRed4Scale = scale;
					break;
				case DecalColorChannel.GREEN_5:
					partMaterialOverrides[i].PropertiesOverride.DecalGreen5Scale = scale;
					break;
				case DecalColorChannel.BLUE_6:
					partMaterialOverrides[i].PropertiesOverride.DecalBlue6Scale = scale;
					break;
				}
			}
		}

		public static void OverridePartMaterialsDecalRotation(DecalColorChannel decalColorChannel, float rotationDegrees, EquipPartMatPropsOverride[] partMaterialOverrides)
		{
			float num = rotationDegrees * ((float)Math.PI / 180f);
			for (int i = 0; i < partMaterialOverrides.Length; i++)
			{
				switch (decalColorChannel)
				{
				case DecalColorChannel.RED_1:
					partMaterialOverrides[i].PropertiesOverride.DecalRed1RotationRads = num;
					break;
				case DecalColorChannel.GREEN_2:
					partMaterialOverrides[i].PropertiesOverride.DecalGreen2RotationRads = num;
					break;
				case DecalColorChannel.BLUE_3:
					partMaterialOverrides[i].PropertiesOverride.DecalBlue3RotationRads = num;
					break;
				case DecalColorChannel.RED_4:
					partMaterialOverrides[i].PropertiesOverride.DecalRed4RotationRads = num;
					break;
				case DecalColorChannel.GREEN_5:
					partMaterialOverrides[i].PropertiesOverride.DecalGreen5RotationRads = num;
					break;
				case DecalColorChannel.BLUE_6:
					partMaterialOverrides[i].PropertiesOverride.DecalBlue6RotationRads = num;
					break;
				}
			}
		}

		public static void OverridePartMaterialsBodyColors(Color redChannelColor, Color greenChannelColor, Color blueChannelColor, EquipPartMatPropsOverride[] partMaterialOverrides)
		{
			for (int i = 0; i < partMaterialOverrides.Length; i++)
			{
				partMaterialOverrides[i].PropertiesOverride.BodyRedChannelColor = redChannelColor;
				partMaterialOverrides[i].PropertiesOverride.BodyGreenChannelColor = greenChannelColor;
				partMaterialOverrides[i].PropertiesOverride.BodyBlueChannelColor = blueChannelColor;
			}
		}

		public static void OverridePartMaterialsEmissiveCol(Color emissiveColor, EquipPartMatPropsOverride[] partMaterialOverrides)
		{
			for (int i = 0; i < partMaterialOverrides.Length; i++)
			{
				partMaterialOverrides[i].PropertiesOverride.EmissiveColorTint = emissiveColor;
			}
		}
	}
}
