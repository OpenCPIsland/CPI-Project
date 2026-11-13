using UnityEngine;

namespace MixAvatar
{
	public class CombinedMeshAtlasData
	{
		public Rect[] AtlasUVOffsets;

		public Material AtlasMaterial;

		public CombinedMeshAtlasData(Rect[] atlasUVOffsets, Material atlasMaterial)
		{
			AtlasUVOffsets = atlasUVOffsets;
			AtlasMaterial = atlasMaterial;
		}
	}
}
