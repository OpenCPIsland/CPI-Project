using ClubPenguin.DisneyStore;
using Disney.Kelowna.Common;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class DisneyStoreMarketplaceController : AbstractDisneyStoreController, IDisneyStoreController
	{
		public DisneyStoreFranchiseDefinition FranchiseDefinition;

		public DisneyStoreTrayAnimator TrayAnimator;

		protected override void start()
		{
			Franchise.SetFranchise(FranchiseDefinition, this);
		}

		protected override void onDestroy()
		{
		}

		public DisneyStoreTrayAnimator GetTrayAnimator()
		{
			return TrayAnimator;
		}

		public void OnCloseClicked()
		{
			Object.Destroy(base.gameObject);
		}

		public void ShowLoadingModal()
		{
			shouldLoadingModalBeShown = true;
			if (loadingModal == null)
			{
				Content.LoadAsync(onLoadingModalLoadComplete, LoadingPrefabKey);
			}
		}

		public void HideLoadingModal()
		{
			shouldLoadingModalBeShown = false;
			if (loadingModal != null)
			{
				Object.Destroy(loadingModal);
				loadingModal = null;
			}
		}

		public void onLoadingModalLoadComplete(string Path, GameObject loadingModalPrefab)
		{
			if (shouldLoadingModalBeShown)
			{
				loadingModal = Object.Instantiate(loadingModalPrefab, base.transform, false);
			}
		}
	}
}
