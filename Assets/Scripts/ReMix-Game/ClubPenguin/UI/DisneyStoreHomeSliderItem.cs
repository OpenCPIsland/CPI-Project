using ClubPenguin.Analytics;
using ClubPenguin.DisneyStore;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class DisneyStoreHomeSliderItem : MonoBehaviour
	{
		public string FranchiseDefinitionKeyPath;

		private DisneyStoreController storeController;

		private DisneyStoreFranchiseDefinition definition;

		private void Start()
		{
			loadFranchiseDefinition();
		}

		public void InjectStoreController(DisneyStoreController storeController)
		{
			this.storeController = storeController;
		}

		public void OnClick()
		{
			if (definition != null)
			{
				if (storeController != null)
				{
					storeController.ShowFranchise(definition);
				}
				logFranchiseClicked();
			}
		}

		private void loadFranchiseDefinition()
		{
			TypedAssetContentKey<ScriptableObject> key = new TypedAssetContentKey<ScriptableObject>(FranchiseDefinitionKeyPath);
			Content.LoadAsync(onFranchiseDefinitionLoaded, key);
		}

		private void onFranchiseDefinitionLoaded(string path, ScriptableObject definition)
		{
			this.definition = (definition as DisneyStoreFranchiseDefinition);
		}

		private void logFranchiseClicked()
		{
			Service.Get<ICPSwrveService>().Action("game.disney_store_franchise", definition.name, "header");
		}
	}
}
