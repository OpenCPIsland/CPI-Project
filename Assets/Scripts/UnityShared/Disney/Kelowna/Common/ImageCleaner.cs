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
			for (int i = 0; i < imageComponents.Length; i++)
			{
				Resources.UnloadAsset(imageComponents[i].sprite.texture);
			}
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
