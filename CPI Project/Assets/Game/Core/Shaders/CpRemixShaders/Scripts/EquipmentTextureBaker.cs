using System;
using System.Collections.Generic;
using MixAvatar;
using UnityEngine;

namespace CpRemixShaders
{
	public class EquipmentTextureBaker : IMaterialToTextureBaker
	{
		public Color BodyRedChannelColor = Color.red;

		public Color BodyGreenChannelColor = Color.green;

		public Color BodyBlueChannelColor = Color.blue;

		private static Material equipmentBakeMaterial;

		private static Material bodyBakeMaterial;

		private Dictionary<string, EquipmentShaderParams> materialNameToShaderPropOverride;

		private Dictionary<string, EquipmentShaderParams> MaterialNameToShaderPropOverride
		{
			get
			{
				if (materialNameToShaderPropOverride == null)
				{
					materialNameToShaderPropOverride = new Dictionary<string, EquipmentShaderParams>();
				}
				return materialNameToShaderPropOverride;
			}
		}

		public EquipmentTextureBaker(Color bodyRedChannelColor, Color bodyGreenChannelColor, Color bodyBlueChannelColor)
		{
			BodyRedChannelColor = bodyRedChannelColor;
			BodyBlueChannelColor = bodyBlueChannelColor;
			BodyGreenChannelColor = bodyGreenChannelColor;
			#if UNITY_ANDROID || UNITY_IOS
            QualitySettings.antiAliasing = 4;
			#else
			QualitySettings.antiAliasing = 8;
			#endif
            if (equipmentBakeMaterial == null)
			{
				equipmentBakeMaterial = new Material(EquipmentShaderUtils.GetEquipmentBakeShader());
				equipmentBakeMaterial.enableInstancing = true;
			}
			if (bodyBakeMaterial == null)
			{
				bodyBakeMaterial = new Material(EquipmentShaderUtils.GetBodyBakeShader());
				bodyBakeMaterial.enableInstancing = true;
			}
		}

		public void OverrideMaterialProperties(EquipPartMatPropsOverride materialPropertiesOverride)
		{
			if (MaterialNameToShaderPropOverride.ContainsKey(materialPropertiesOverride.Material.name))
			{
				MaterialNameToShaderPropOverride[materialPropertiesOverride.Material.name] = materialPropertiesOverride.PropertiesOverride;
			}
			else
			{
				MaterialNameToShaderPropOverride.Add(materialPropertiesOverride.Material.name, materialPropertiesOverride.PropertiesOverride);
			}
		}

		public void RemoveMaterialPropertiesOverride(EquipPartMatPropsOverride materialPropertiesOverride)
		{
			if (MaterialNameToShaderPropOverride.ContainsKey(materialPropertiesOverride.Material.name))
			{
				MaterialNameToShaderPropOverride.Remove(materialPropertiesOverride.Material.name);
			}
		}

		public void ClearAllMatPropOverrides()
		{
			MaterialNameToShaderPropOverride.Clear();
		}

		public int GetLargestTextureDimensionInMaterial(Material material)
		{
			Texture texture = null;
			if (EquipmentShaderUtils.IsEquipmentPreviewShader(material.shader) || EquipmentShaderUtils.IsEquipmentBakeShader(material.shader))
			{
				texture = material.GetTexture(EquipmentShaderParams.DECALS_123_OPACITY_TEX);
			}
			if (texture == null)
			{
				texture = material.GetTexture(EquipmentShaderParams.BODY_COLORS_MASK_TEX);
			}
			return (!(texture != null)) ? 16 : texture.width;
		}

		public void BakeEquipmentMaterialToAtlas(Material equipmentMaterial, Rect offsetInAtlas, RenderTexture destinationAtlas)
		{
			#if UNITY_ANDROID || UNITY_IOS
            QualitySettings.antiAliasing = 4;
            if (destinationAtlas != null)
            {
                destinationAtlas.antiAliasing = 4;
            }
			#else
			QualitySettings.antiAliasing = 8;
			if (destinationAtlas != null)
			{
				destinationAtlas.antiAliasing = 8;
			}
			#endif
            if (!EquipmentShaderUtils.IsEquipmentPreviewShader(equipmentMaterial.shader) && !EquipmentShaderUtils.IsBodyPreviewShader(equipmentMaterial.shader))
			{
				throw new Exception("Material must use one of following shaders: CpRemix/Equipment Preview, CpRemix/Avatar Body Preview. Was using " + equipmentMaterial.shader.name);
			}
			EquipmentShaderParams equipmentShaderParams = ((!MaterialNameToShaderPropOverride.ContainsKey(equipmentMaterial.name)) ? EquipmentShaderParams.FromMaterial(equipmentMaterial) : MaterialNameToShaderPropOverride[equipmentMaterial.name]);
			equipmentShaderParams.BodyRedChannelColor = BodyRedChannelColor;
			equipmentShaderParams.BodyGreenChannelColor = BodyGreenChannelColor;
			equipmentShaderParams.BodyBlueChannelColor = BodyBlueChannelColor;
			Texture source;
			Material material;
			if (EquipmentShaderUtils.IsEquipmentPreviewShader(equipmentMaterial.shader))
			{
				source = equipmentShaderParams.Decals123OpacityTexture ?? equipmentShaderParams.BodyColorsMaskTexture;
				material = equipmentBakeMaterial;
			}
			else
			{
				source = equipmentShaderParams.BodyColorsMaskTexture;
				material = bodyBakeMaterial;
			}
			equipmentShaderParams.AtlasOffsetU = offsetInAtlas.x;
			equipmentShaderParams.AtlasOffsetV = offsetInAtlas.y;
			equipmentShaderParams.AtlasScaleU = offsetInAtlas.width;
			equipmentShaderParams.AtlasScaleV = offsetInAtlas.height;
			equipmentShaderParams.ApplyToMaterial(material);
			Graphics.Blit(source, destinationAtlas, material);
		}

		public void UnloadBlitMaterials()
		{
		}
	}
}
