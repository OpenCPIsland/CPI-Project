using ClubPenguin.ClothingDesigner.Inventory;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public abstract class AbstractDisneyStoreController : MonoBehaviour
	{
		public DisneyStoreFranchise Franchise;

		public readonly PrefabContentKey LoadingPrefabKey = new PrefabContentKey("DisneyShop/Prefabs/LoadingModal");

		protected GameObject loadingModal;

		protected bool shouldLoadingModalBeShown;

		protected virtual void Start()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new AwayFromKeyboardEvent(AwayFromKeyboardStateType.Marketplace));
			getLatestInventory();
			ItemImageBuilder.acquire();
			start();
		}

		private void getLatestInventory()
		{
			GetLatestInventoryCMD getLatestInventoryCMD = new GetLatestInventoryCMD();
			getLatestInventoryCMD.Execute();
		}

		protected virtual void OnDestroy()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new AwayFromKeyboardEvent(AwayFromKeyboardStateType.Here));
			ItemImageBuilder.release();
			onDestroy();
		}

		protected abstract void start();

		protected abstract void onDestroy();
	}
}
