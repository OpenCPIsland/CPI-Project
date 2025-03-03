using Disney.Kelowna.Common;
using UnityEngine;

namespace DevonLocalization
{
	[RequireComponent(typeof(Renderer))]
	public class LocalizedTexture : MonoBehaviour
	{
		public LocalizedTextureAssetContentKey GenericContentKey;

		private Renderer textureRenderer;

		private void Awake()
		{
			textureRenderer = GetComponent<Renderer>();
			Content.LoadAsync(onTextureLoaded, GenericContentKey);
		}

		private void onTextureLoaded(string path, Texture texture)
		{
			textureRenderer.material.mainTexture = texture;
		}
	}
}
