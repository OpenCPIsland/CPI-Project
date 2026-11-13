using ClubPenguin.DisneyStore;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class DisneyStoreMarketplaceController : AbstractDisneyStoreController, IDisneyStoreController
	{
		private bool marketplaceStateDispatched;

		public DisneyStoreFranchiseDefinition FranchiseDefinition;

		public DisneyStoreTrayAnimator TrayAnimator;

		protected override void start()
		{
			dispatchMarketplaceOpened();
			Franchise.SetFranchise(FranchiseDefinition, this);
		}

		protected override void onDestroy()
		{
			dispatchMarketplaceClosed();
		}

		public DisneyStoreTrayAnimator GetTrayAnimator()
		{
			return TrayAnimator;
		}

		public void OnCloseClicked()
		{
			DisneyStoreAudioUtils.PlayClose();
			dispatchMarketplaceClosed();
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

		private void dispatchMarketplaceOpened()
		{
			if (marketplaceStateDispatched)
			{
				return;
			}
			Service.Get<EventDispatcher>().DispatchEvent(new MarketplaceEvents.MarketplaceOpened("DisneyStore"));
			marketplaceStateDispatched = true;
		}

		private void dispatchMarketplaceClosed()
		{
			if (!marketplaceStateDispatched)
			{
				return;
			}
			Service.Get<EventDispatcher>().DispatchEvent(new MarketplaceEvents.MarketplaceClosed("DisneyStore"));
			marketplaceStateDispatched = false;
		}
	}
}
