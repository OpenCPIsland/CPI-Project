using ClubPenguin.UI;
using Disney.Kelowna.Common;
using UnityEngine;

namespace ClubPenguin.Marketplace
{
	public class Marketplace : MonoBehaviour
	{
		public MarketplaceDefinition definition;

		public PrefabContentKey prefabContentKey = new PrefabContentKey("Prefabs/MarketplaceScreen");

		private GameObject marketplaceObject;

		private bool isLoadingPrefab = false;

		public void ActivateMarketplace(GameObject sender)
		{
			if (sender.CompareTag("Player") && !isLoadingPrefab && Object.FindObjectOfType<MarketplaceScreenController>() == null)
			{
				isLoadingPrefab = true;
				SceneRefs.FullScreenPopupManager.CreatePopup(prefabContentKey, "Accessibility.Popup.Title.Marketplace", false, onPrefabCreated);
			}
		}

		private void onPrefabCreated(PrefabContentKey key, GameObject marketplaceObject)
		{
			MarketplaceScreenController component = marketplaceObject.GetComponent<MarketplaceScreenController>();
			if (component != null)
			{
				marketplaceObject.GetComponent<MarketplaceScreenController>().Init(definition);
			}
			isLoadingPrefab = false;
		}
	}
}
