using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Avatar
{
	[Serializable]
	public class EquipmentMaterialProperties : TexturedMaterialProperties
	{
		public const int NUM_DECAL_COLORS = 6;

		public Texture Decals123OpacityTexture;

		public Color EmissiveColorTint;

		public Color[] DecalColors;

		private static readonly EquipmentMaterialProperties DefaultProperties = new EquipmentMaterialProperties();

		public EquipmentMaterialProperties()
		{
			Decals123OpacityTexture = null;
			EmissiveColorTint = Color.white;
			DecalColors = new Color[6];
			for (int i = 0; i < 6; i++)
			{
				DecalColors[i] = Color.white;
			}
		}

		public EquipmentMaterialProperties(Texture decals123OpacityTexture, Color emissiveColorTint, Color[] decalColors)
		{
			Decals123OpacityTexture = decals123OpacityTexture;
			EmissiveColorTint = emissiveColorTint;
			DecalColors = decalColors;
		}

		public override void Apply(Material mat)
		{
			mat.SetTexture(ShaderParams.DECALS_123_OPACITY_TEX, Decals123OpacityTexture);
			mat.SetColor(ShaderParams.EMISSIVE_COLOR_TINT, EmissiveColorTint);
			for (int i = 0; i < 6; i++)
			{
				mat.SetColor(ShaderParams.DECAL_COLOR[i], DecalColors[i]);
			}
		}

		public override Vector2 GetTextureSize()
		{
			if ((bool)Decals123OpacityTexture)
			{
				return new Vector2(Decals123OpacityTexture.width, Decals123OpacityTexture.height);
			}
			return Vector2.zero;
		}

		public override Texture GetMaskTexture()
		{
			return Decals123OpacityTexture;
		}

		public override Material GetMaterial(bool baking)
		{
			if (baking)
			{
				return TexturedMaterialProperties.GetBakeMaterial(AvatarService.EquipmentBakeShader);
			}
			return new Material(AvatarService.EquipmentPreviewShader);
		}

		public override void ResetMaterial(Material mat)
		{
			DefaultProperties.Apply(mat);
		}

		public override List<UnityEngine.Object> InternalReferences()
		{
			List<UnityEngine.Object> list = new List<UnityEngine.Object>();
			list.Add(Decals123OpacityTexture);
			return list;
		}

		public override string ToString()
		{
			return string.Format("[EquipmentMaterial : Decals123OpacityTexture={0}, EmissiveColorTint={1}]", Decals123OpacityTexture, EmissiveColorTint);
		}
	}
}
