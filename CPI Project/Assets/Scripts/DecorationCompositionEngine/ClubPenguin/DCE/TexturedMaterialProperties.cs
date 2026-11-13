using UnityEngine;

namespace ClubPenguin.DCE
{
	public abstract class TexturedMaterialProperties : BaseMaterialProperties
	{
		private static Material bakeMaterial;

		protected static Material GetBakeMaterial(Shader shader)
		{
			if (!bakeMaterial)
			{
				bakeMaterial = new Material(shader);
			}
			else
			{
				bakeMaterial.shader = shader;
			}
			return bakeMaterial;
		}

		public abstract Vector2 GetTextureSize();

		public abstract Texture GetMaskTexture();

		public abstract Material GetMaterial(bool baking);

		public abstract void ResetMaterial(Material mat);
	}
}
