using Disney.Kelowna.Common;
using UnityEngine;
using UnityEngine.UI;

namespace DevonLocalization
{
	[RequireComponent(typeof(Image))]
	public class LocalizedSprite : MonoBehaviour
	{
		public LocalizedSpriteAssetContentKey GenericContentKey;

		private Image image;

		private void Awake()
		{
			image = GetComponent<Image>();
			image.enabled = false;
			Content.LoadAsync(onSpriteLoaded, GenericContentKey);
		}

		private void onSpriteLoaded(string path, Sprite sprite)
		{
			image.sprite = sprite;
			image.enabled = true;
		}
	}
}
