using Disney.Kelowna.Common;
using UnityEngine;

namespace ClubPenguin.FeatureToggle
{
	internal class FeatureToggleLoader : AbstractFeatureToggler
	{
		[Tooltip("If this is a non RC build and this feature is enabled the Prefab will be loaded as a child of this game object")]
		public PrefabContentKey Prefab;

		private GameObject loadedInstance;

		protected override void onFeatureEnabled()
		{
			Content.LoadAsync(onPrefabLoaded, Prefab);
		}

		private void onPrefabLoaded(string path, GameObject asset)
		{
			loadedInstance = Object.Instantiate(asset, base.transform, false);
		}

		protected override void onFeatureDisabled()
		{
			if (loadedInstance != null)
			{
				Object.Destroy(loadedInstance);
			}
		}
	}
}
