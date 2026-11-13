using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Disney.Kelowna.Common
{
	[DisallowMultipleComponent]
	public class ImageCleaner : MonoBehaviour
	{
		public bool IsRecursive = false;

		private void OnDestroy()
		{
			unloadImageTextures();
		}

		private void unloadImageTextures()
		{
			Image[] imageComponents = getImageComponents();
			HashSet<Texture2D> checkedTextures = new HashSet<Texture2D>();
			for (int i = 0; i < imageComponents.Length; i++)
			{
				Image image = imageComponents[i];
				if (image == null || image.sprite == null)
				{
					continue;
				}
				Texture2D texture = image.sprite.texture;
				if (texture != null && checkedTextures.Add(texture) && !isTextureUsedByOtherImage(texture, imageComponents))
				{
					Resources.UnloadAsset(texture);
				}
			}
		}

		private bool isTextureUsedByOtherImage(Texture2D texture, Image[] imagesBeingUnloaded)
		{
			Image[] allImages = Resources.FindObjectsOfTypeAll<Image>();
			for (int i = 0; i < allImages.Length; i++)
			{
				Image image = allImages[i];
				if (image == null || isBeingUnloaded(image, imagesBeingUnloaded) || image.sprite == null)
				{
					continue;
				}
				if (image.sprite.texture == texture)
				{
					return true;
				}
			}
			return false;
		}

		private bool isBeingUnloaded(Image image, Image[] imagesBeingUnloaded)
		{
			for (int i = 0; i < imagesBeingUnloaded.Length; i++)
			{
				if (imagesBeingUnloaded[i] == image)
				{
					return true;
				}
			}
			return false;
		}

		private Image[] getImageComponents()
		{
			if (IsRecursive)
			{
				return GetComponentsInChildren<Image>();
			}
			Image component = GetComponent<Image>();
			if (component != null)
			{
				return new Image[1]
				{
					component
				};
			}
			return new Image[0];
		}
	}
}
