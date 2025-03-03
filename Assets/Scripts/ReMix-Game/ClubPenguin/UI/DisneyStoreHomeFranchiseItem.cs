using ClubPenguin.Analytics;
using ClubPenguin.DisneyStore;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class DisneyStoreHomeFranchiseItem : MonoBehaviour
	{
		private DisneyStoreFranchiseDefinition franchiseDef;

		private DisneyStoreController storeController;

		public void SetFranchise(DisneyStoreFranchiseDefinition franchiseDef, DisneyStoreController storeController)
		{
			this.franchiseDef = franchiseDef;
			this.storeController = storeController;
		}

		public void OnItemClicked()
		{
			storeController.ShowFranchise(franchiseDef);
			logFranchiseClicked();
		}

		private void logFranchiseClicked()
		{
			Service.Get<ICPSwrveService>().Action("game.disney_store_franchise", franchiseDef.name, "body");
		}
	}
}
