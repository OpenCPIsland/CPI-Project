using UnityEngine;

namespace MixAvatar
{
	public interface IMaterialToTextureBaker
	{
		int GetLargestTextureDimensionInMaterial(Material material);

		void BakeEquipmentMaterialToAtlas(Material equipmentMaterial, Rect offsetInAtlas, RenderTexture destinationAtlas);

		void UnloadBlitMaterials();
	}
}
