using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Avatar
{
	[Serializable]
	public class BodyMaterialProperties : TexturedMaterialProperties
	{
		public Texture2D DiffuseTexture;

		public Texture2D BodyColorsMaskTexture;

		public Texture2D DetailMatCapMaskEmissiveTexture;

		private static readonly BodyMaterialProperties DefaultProperties = new BodyMaterialProperties();

		public BodyMaterialProperties()
		{
		}

		public BodyMaterialProperties(Texture2D diffuseTexture, Texture2D bodyColorsMaskTexture, Texture2D detailMatCapMaskEmissiveTexture)
		{
			DiffuseTexture = diffuseTexture;
			BodyColorsMaskTexture = bodyColorsMaskTexture;
			DetailMatCapMaskEmissiveTexture = detailMatCapMaskEmissiveTexture;
		}

		public override void Apply(Material mat)
		{
			mat.SetTexture(ShaderParams.DIFFUSE_TEX, DiffuseTexture);
			mat.SetTexture(ShaderParams.BODY_COLORS_MASK_TEX, BodyColorsMaskTexture);
			mat.SetTexture(ShaderParams.DETAIL_MATCAPMASK_EMISSIVE_TEX, DetailMatCapMaskEmissiveTexture);
		}

		public override Vector2 GetTextureSize()
		{
			if ((bool)BodyColorsMaskTexture)
			{
				return new Vector2(BodyColorsMaskTexture.width, BodyColorsMaskTexture.height);
			}
			if ((bool)DiffuseTexture)
			{
				return new Vector2(DiffuseTexture.width, DiffuseTexture.height);
			}
			if ((bool)DetailMatCapMaskEmissiveTexture)
			{
				return new Vector2(DetailMatCapMaskEmissiveTexture.width, DetailMatCapMaskEmissiveTexture.height);
			}
			return Vector2.zero;
		}

		public override Texture GetMaskTexture()
		{
			return BodyColorsMaskTexture;
		}

		public override Material GetMaterial(bool baking)
		{
			if (baking)
			{
				return TexturedMaterialProperties.GetBakeMaterial(AvatarService.BodyBakeShader);
			}
			return new Material(AvatarService.BodyPreviewShader);
		}

		public override void ResetMaterial(Material mat)
		{
			DefaultProperties.Apply(mat);
		}

		public override List<UnityEngine.Object> InternalReferences()
		{
			List<UnityEngine.Object> list = new List<UnityEngine.Object>();
			list.Add(DiffuseTexture);
			list.Add(BodyColorsMaskTexture);
			list.Add(DetailMatCapMaskEmissiveTexture);
			return list;
		}

		public override string ToString()
		{
			return string.Format("[BodyMaterial : DiffuseTexture={0}, BodyColorsMaskTexture={1}, DetailMatCapMaskEmissiveTexture={2}", DiffuseTexture, BodyColorsMaskTexture, DetailMatCapMaskEmissiveTexture);
		}
	}
}
