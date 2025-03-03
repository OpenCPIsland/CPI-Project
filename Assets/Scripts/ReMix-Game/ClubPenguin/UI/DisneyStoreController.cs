using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.DisneyStore;
using ClubPenguin.Net;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class DisneyStoreController : AbstractDisneyStoreController, IDisneyStoreController
	{
		private const string MARKETPLACE_NAME = "DisneyStore";

		public DisneyStoreHome Home;

		public GameObject BackButton;

		public DisneyStoreTrayAnimator TrayAnimator;

		private DisneyStoreDefinition storeDefinition;

		[SerializeField]
		protected string StoreDefinitionKeyPath = "Definitions/DisneyStore/DisneyStore";

		protected override void start()
		{
			Content.LoadAsync<ScriptableObject>(StoreDefinitionKeyPath, onDisneyStoreDefinitionLoaded);
			logStoreVisit();
			logStoreVisitStart();
			Service.Get<EventDispatcher>().DispatchEvent(new MarketplaceEvents.MarketplaceOpened("DisneyStore"));
		}

		private void onDisneyStoreDefinitionLoaded(string path, ScriptableObject storeDefinition)
		{
			this.storeDefinition = (storeDefinition as DisneyStoreDefinition);
			Home.SetFranchises(getAvailableFranchises(), this, this.storeDefinition);
			ShowHome();
		}

		public DisneyStoreTrayAnimator GetTrayAnimator()
		{
			return TrayAnimator;
		}

		public void ShowHome()
		{
			Franchise.Clear();
			Franchise.gameObject.SetActive(false);
			Home.gameObject.SetActive(true);
			BackButton.SetActive(false);
		}

		public void ShowFranchise(DisneyStoreFranchiseDefinition franchiseDef)
		{
			Home.gameObject.SetActive(false);
			Franchise.gameObject.SetActive(true);
			Franchise.SetFranchise(franchiseDef, this);
			BackButton.SetActive(true);
		}

		public void OnBackClicked()
		{
			ShowHome();
		}

		public void OnCloseClicked()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new MarketplaceEvents.MarketplaceClosed("DisneyStore"));
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

		private List<DisneyStoreFranchiseDefinition> getAvailableFranchises()
		{
			List<DisneyStoreFranchiseDefinition> list = new List<DisneyStoreFranchiseDefinition>();
			Dictionary<int, DisneyStoreFranchiseDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, DisneyStoreFranchiseDefinition>>();
			using (Dictionary<int, DisneyStoreFranchiseDefinition>.Enumerator enumerator = dictionary.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (isFranchiseAvailable(enumerator.Current.Value))
					{
						list.Add(enumerator.Current.Value);
					}
				}
			}
			sortFranchisesBySortingId(list);
			return list;
		}

		private void sortFranchisesBySortingId(List<DisneyStoreFranchiseDefinition> franchises)
		{
			franchises.Sort((DisneyStoreFranchiseDefinition x, DisneyStoreFranchiseDefinition y) => y.SortingIdDesc.CompareTo(x.SortingIdDesc));
		}

		private bool isFranchiseAvailable(DisneyStoreFranchiseDefinition franchiseDef)
		{
			long num = Service.Get<INetworkServicesManager>().GameTimeMilliseconds / 1000;
			return num >= franchiseDef.StartTimeInSeconds && num < franchiseDef.EndTimeInSeconds;
		}

		private void logStoreVisit()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			PresenceData component = cPDataEntityCollection.GetComponent<PresenceData>(cPDataEntityCollection.LocalPlayerHandle);
			Service.Get<ICPSwrveService>().Action("game.disney_store_visit", component.Room);
		}

		private void logStoreVisitStart()
		{
			Service.Get<ICPSwrveService>().StartTimer("disney_shop", "disney_shop");
		}

		private void logStoreVisitEnd()
		{
			Service.Get<ICPSwrveService>().EndTimer("disney_shop");
		}

		protected override void onDestroy()
		{
			logStoreVisitEnd();
		}
	}
}
