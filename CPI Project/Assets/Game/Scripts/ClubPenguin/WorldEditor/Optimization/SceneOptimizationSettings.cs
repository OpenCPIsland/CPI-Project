using UnityEngine;

namespace ClubPenguin.WorldEditor.Optimization
{
	[DisallowMultipleComponent]
	public class SceneOptimizationSettings : MonoBehaviour
	{
		[Header("Texture Atlas")]
		public int MinTextureSize = 4096;

		public int MaxTextureSize = 4096;

		public int MaxAtlasDimension = 4096;

		[Header("Texture Atlas Preview")]
		public Texture2D[] TextureAtlasPreviewButtons;

		[Header("Results")]
		public TextureData[] Textures;

		public Texture2D WorldObjectTextureAtlas;

		public Material WorldObjectAtlasMaterial;
	}
}
