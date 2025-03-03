using Disney.Kelowna.Common;
using UnityEngine;

namespace ClubPenguin.Core
{
	public class AspectRatioPrefabLoader : MonoBehaviour
	{
		[SerializeField]
		private AspectRatioPrefabLoaderSettings[] runtimeSettings;

		private void Awake()
		{
			AspectRatioPrefabLoaderSettings aspectRatioPrefabLoaderSettings = PlatformUtils.FindAspectRatioSettings(runtimeSettings);
			if (aspectRatioPrefabLoaderSettings == null || aspectRatioPrefabLoaderSettings.ContentKeys == null)
			{
				return;
			}
			for (int i = 0; i < aspectRatioPrefabLoaderSettings.ContentKeys.Length; i++)
			{
				if (aspectRatioPrefabLoaderSettings.ContentKeys[i] != null && !string.IsNullOrEmpty(aspectRatioPrefabLoaderSettings.ContentKeys[i].Key))
				{
					Content.LoadAsync(onPrefabLoaded, aspectRatioPrefabLoaderSettings.ContentKeys[i]);
				}
			}
		}

		private void onPrefabLoaded(string path, GameObject prefab)
		{
			Object.Instantiate(prefab, base.transform);
		}
	}
}
