using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Igloo.UI
{
	public class IglooListScreenController : AbstractPlayerListController
	{
		private enum InteractiveState
		{
			NonInteractive,
			Interactive
		}

		public enum IglooListingType
		{
			Friends,
			HighPopulations
		}

		private const string KEY = "IglooListType";

		public GameObject EmptyFriendsGraphic;

		public GameObject EmptyPopularGraphic;

		public GameObject LoadingSpinner;

		public Button friendsButton;

		public Button popularButton;

		public Transform emptyGraphicParent;

		private bool allIgloodataLoaded = false;

		private IglooListingType iglooListingType = IglooListingType.HighPopulations;

		private List<ZoneId> friendIgloos;

		private HashSet<DataEntityHandle> waitingForProfileDataPlayers = new HashSet<DataEntityHandle>();

		private GameObject emptyFriendsObject;

		private GameObject emptyPopularObject;

		protected override void awake()
		{
			if (PlatformUtils.GetAspectRatioType() == AspectRatioType.Landscape)
			{
				GetComponent<UIDisabler>().enabled = false;
				GetComponent<UIDisabler>().ResetOnDestroy = false;
			}
			Service.Get<EventDispatcher>().AddListener<IglooServiceEvents.PopularIgloosListLoaded>(OnPopularIgloosListLoaded);
			Service.Get<EventDispatcher>().AddListener<IglooServiceEvents.IgloosFromZoneIdsLoaded>(onIgloosFromZoneIdsLoaded);
			dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<IglooListingData>>(onIglooListingDataAdded);
			DataEntityHandle[] entitiesByType = dataEntityCollection.GetEntitiesByType<IglooListingData>();
			foreach (DataEntityHandle handle in entitiesByType)
			{
				IglooListingData component = dataEntityCollection.GetComponent<IglooListingData>(handle);
				component.IglooListingDataUpdated += onIglooListingDataUpdated;
			}
		}

		protected override void start()
		{
			ProfileData component = dataEntityCollection.GetComponent<ProfileData>(dataEntityCollection.LocalPlayerHandle);
			if (Service.Get<ZoneTransitionService>().CurrentInstanceId == component.ZoneId.instanceId)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("MyIglooButton"));
			}
			emptyFriendsObject = Object.Instantiate(EmptyFriendsGraphic, emptyGraphicParent);
			emptyPopularObject = Object.Instantiate(EmptyPopularGraphic, emptyGraphicParent);
			ClickSelectedOption();
		}

		private void ClickSelectedOption()
		{
			if (DisplayNamePlayerPrefs.HasKey("IglooListType") && DisplayNamePlayerPrefs.HasKey("IglooListType"))
			{
				iglooListingType = (IglooListingType)DisplayNamePlayerPrefs.GetInt("IglooListType");
			}
			if (iglooListingType == IglooListingType.HighPopulations)
			{
				popularButton.onClick.Invoke();
			}
			else
			{
				friendsButton.onClick.Invoke();
			}
		}

		protected override void onDestroy()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("MyIglooButton"));
			Service.Get<EventDispatcher>().RemoveListener<IglooServiceEvents.PopularIgloosListLoaded>(OnPopularIgloosListLoaded);
			Service.Get<EventDispatcher>().RemoveListener<IglooServiceEvents.IgloosFromZoneIdsLoaded>(onIgloosFromZoneIdsLoaded);
			if (dataEntityCollection != null)
			{
				dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<IglooListingData>>(onIglooListingDataAdded);
			}
			DataEntityHandle[] entitiesByType = dataEntityCollection.GetEntitiesByType<IglooListingData>();
			foreach (DataEntityHandle handle in entitiesByType)
			{
				IglooListingData component = dataEntityCollection.GetComponent<IglooListingData>(handle);
				component.IglooListingDataUpdated -= onIglooListingDataUpdated;
			}
			DisplayNamePlayerPrefs.SetInt("IglooListType", (int)iglooListingType);
			Object.Destroy(emptyPopularObject);
			Object.Destroy(emptyFriendsObject);
		}

		public void OnPopularPressed()
		{
			iglooListingType = IglooListingType.HighPopulations;
			getPlayers();
		}

		public void OnFriendsPressed()
		{
			iglooListingType = IglooListingType.Friends;
			getPlayers();
		}

		public void OnClosePressed()
		{
			if (PlatformUtils.GetAspectRatioType() == AspectRatioType.Landscape)
			{
				Object.Destroy(base.transform.parent.parent.gameObject);
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}

		private void getPlayers()
		{
			setStateOfSelectionButtons(InteractiveState.NonInteractive);
			refreshServerData();
			pooledScrollRect.ResetContent();
			emptyFriendsObject.SetActive(false);
			emptyPopularObject.SetActive(false);
			allIgloodataLoaded = false;
			LoadingSpinner.SetActive(true);
			allPlayersList.Clear();
		}

		private void QueryForZoneIds(string language)
		{
			if (waitingForProfileDataPlayers.Count == 0)
			{
				List<DataEntityHandle> handles;
				IList<ZoneId> zoneIds;
				FriendsDataModelService.GetFriendIgloos(out handles, out zoneIds);
				Service.Get<INetworkServicesManager>().IglooService.GetIglooPopulationsByZoneIds(language, zoneIds, handles);
			}
		}

		private void refreshServerData()
		{
			string languageString = LocalizationLanguage.GetLanguageString(Service.Get<Localizer>().Language);
			switch (iglooListingType)
			{
			case IglooListingType.Friends:
			{
				List<DataEntityHandle> friendsList = FriendsDataModelService.FriendsList;
				for (int i = 0; i < friendsList.Count; i++)
				{
					if (!dataEntityCollection.HasComponent<ProfileData>(friendsList[i]))
					{
						waitingForProfileDataPlayers.Add(friendsList[i]);
					}
				}
				QueryForZoneIds(languageString);
				break;
			}
			case IglooListingType.HighPopulations:
				Service.Get<INetworkServicesManager>().IglooService.GetPopularIgloos(languageString);
				break;
			}
		}

		protected override void sortPlayers(List<DataEntityHandle> listToSort)
		{
			if (allIgloodataLoaded)
			{
				listToSort.Sort(delegate(DataEntityHandle a, DataEntityHandle b)
				{
					int num = dataEntityCollection.GetComponent<DisplayNameData>(a).DisplayName.CompareTo(dataEntityCollection.GetComponent<DisplayNameData>(b).DisplayName);
					bool hasPublicIgloo = dataEntityCollection.GetComponent<ProfileData>(a).HasPublicIgloo;
					bool hasPublicIgloo2 = dataEntityCollection.GetComponent<ProfileData>(b).HasPublicIgloo;
					if (hasPublicIgloo && hasPublicIgloo2)
					{
						IglooListingData component = dataEntityCollection.GetComponent<IglooListingData>(a);
						IglooListingData component2 = dataEntityCollection.GetComponent<IglooListingData>(b);
						int num2 = -1 * component.RoomPopulation.CompareTo(component2.RoomPopulation);
						if (num2 == 0)
						{
							return num;
						}
						return num2;
					}
					if (hasPublicIgloo && !hasPublicIgloo2)
					{
						return -1;
					}
					if (!hasPublicIgloo && hasPublicIgloo2)
					{
						return 1;
					}
					return (!hasPublicIgloo && !hasPublicIgloo2) ? num : 0;
				});
			}
		}

		protected override bool onProfileDataAdded(DataEntityEvents.ComponentAddedEvent<ProfileData> evt)
		{
			base.onProfileDataAdded(evt);
			waitingForProfileDataPlayers.Remove(evt.Handle);
			string languageString = LocalizationLanguage.GetLanguageString(Service.Get<Localizer>().Language);
			if (iglooListingType == IglooListingType.Friends)
			{
				QueryForZoneIds(languageString);
			}
			return false;
		}

		protected override void setUpObject(RectTransform item, int poolIndex)
		{
			base.setUpObject(item, poolIndex);
			IglooListItem component = item.GetComponent<IglooListItem>();
			DataEntityHandle handleFromPoolIndex = getHandleFromPoolIndex(poolIndex);
			ProfileData component2;
			IglooListingData component3;
			if (dataEntityCollection.TryGetComponent(handleFromPoolIndex, out component2) && dataEntityCollection.TryGetComponent(handleFromPoolIndex, out component3))
			{
				RoomPopulation population = new RoomPopulation(new RoomIdentifier(), component3.RoomPopulation);
				component.SetPopulation(population, false, component2.HasPublicIgloo);
			}
			MembershipData component4;
			if (dataEntityCollection.TryGetComponent(handleFromPoolIndex, out component4))
			{
				AbstractPlayerListItem component5 = item.GetComponent<AbstractPlayerListItem>();
				component5.SetMembershipType(getMembershipType(handleFromPoolIndex));
			}
		}

		private void IglooLoadComplete()
		{
			LoadingSpinner.SetActive(false);
			allIgloodataLoaded = true;
			if (allPlayersList.Count > 0)
			{
				sortPlayers(allPlayersList);
				visiblePlayersList = allPlayersList;
			}
			setStateOfSelectionButtons(InteractiveState.Interactive);
			initializePool();
		}

		protected override void initializePool()
		{
			if (allIgloodataLoaded)
			{
				base.initializePool();
			}
		}

		private bool onIglooListingDataAdded(DataEntityEvents.ComponentAddedEvent<IglooListingData> evt)
		{
			evt.Component.IglooListingDataUpdated += onIglooListingDataUpdated;
			DataEntityHandle entityByComponent = dataEntityCollection.GetEntityByComponent(evt.Component);
			if (!DataEntityHandle.IsNullValue(entityByComponent) && !allPlayersList.Contains(entityByComponent))
			{
				allPlayersList.Add(entityByComponent);
			}
			return false;
		}

		private void onIglooListingDataUpdated(IglooListingData iglooListingData)
		{
			DataEntityHandle entityByComponent = dataEntityCollection.GetEntityByComponent(iglooListingData);
			if (!DataEntityHandle.IsNullValue(entityByComponent) && !allPlayersList.Contains(entityByComponent))
			{
				allPlayersList.Add(entityByComponent);
			}
		}

		private bool onIgloosFromZoneIdsLoaded(IglooServiceEvents.IgloosFromZoneIdsLoaded evt)
		{
			if (iglooListingType == IglooListingType.Friends)
			{
				allPlayersList.Clear();
				allPlayersList.AddRange(FriendsDataModelService.FriendsList);
				if (allPlayersList.Count == 0)
				{
					emptyFriendsObject.SetActive(true);
				}
				IglooLoadComplete();
			}
			return false;
		}

		private bool OnPopularIgloosListLoaded(IglooServiceEvents.PopularIgloosListLoaded evt)
		{
			if (iglooListingType == IglooListingType.HighPopulations)
			{
				if (allPlayersList.Count == 0)
				{
					emptyPopularObject.SetActive(true);
				}
				else
				{
					for (int num = allPlayersList.Count - 1; num >= 0; num--)
					{
						ProfileData component;
						if (dataEntityCollection.TryGetComponent(allPlayersList[num], out component) && !component.HasPublicIgloo)
						{
							allPlayersList.RemoveAt(num);
						}
					}
				}
				IglooLoadComplete();
			}
			return false;
		}

		private void setStateOfSelectionButtons(InteractiveState state)
		{
			switch (state)
			{
			case InteractiveState.Interactive:
				pooledScrollRect.IsScrollingAllowed = true;
				StartCoroutine(setStateOfButtonsDelayed(state));
				break;
			case InteractiveState.NonInteractive:
				friendsButton.interactable = false;
				popularButton.interactable = false;
				pooledScrollRect.IsScrollingAllowed = false;
				break;
			}
		}

		private IEnumerator setStateOfButtonsDelayed(InteractiveState state)
		{
			for (int i = 0; i < 10; i++)
			{
				yield return null;
			}
			friendsButton.interactable = (state == InteractiveState.Interactive);
			popularButton.interactable = (state == InteractiveState.Interactive);
		}
	}
}
