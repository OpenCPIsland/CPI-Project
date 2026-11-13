using ClubPenguin.Avatar;
using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class OtherPlayerDataModelService : AbstractDataModelService
	{
		private EventChannel eventChannel;

		private void Awake()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<PlayerStateServiceEvents.OtherPlayerDataReceived>(onOtherPlayerDataReceived);
			eventChannel.AddListener<PlayerStateServiceEvents.OtherPlayerDataListReceived>(onOtherPlayerDataListReceived);
			eventChannel.AddListener<IglooServiceEvents.FriendIgloosListLoaded>(onFriendsIglooListReceived, EventDispatcher.Priority.FIRST);
			eventChannel.AddListener<IglooServiceEvents.PopularIgloosListLoaded>(onPopulareIglooListReceived, EventDispatcher.Priority.FIRST);
			eventChannel.AddListener<IglooServiceEvents.IgloosFromZoneIdsLoaded>(onIgloosFromZoneIdsLoaded, EventDispatcher.Priority.FIRST);
		}

		private void OnDestroy()
		{
			eventChannel.RemoveAllListeners();
		}

		private bool onOtherPlayerDataReceived(PlayerStateServiceEvents.OtherPlayerDataReceived evt)
		{
			addOtherPlayerDetails(evt.Data);
			return false;
		}

		private bool onFriendsIglooListReceived(IglooServiceEvents.FriendIgloosListLoaded evt)
		{
			for (int i = 0; i < evt.IglooListItems.Count; i++)
			{
				addOtherPlayerDetails(evt.IglooListItems[i].ownerData);
				addOtherPlayerIglooListing(evt.IglooListItems[i]);
			}
			return false;
		}

		private bool onIgloosFromZoneIdsLoaded(IglooServiceEvents.IgloosFromZoneIdsLoaded evt)
		{
			List<DataEntityHandle> list = new List<DataEntityHandle>(Service.Get<CPDataEntityCollection>().GetEntitiesByType<ProfileData>());
			for (int i = 0; i < evt.RoomPopulations.Count; i++)
			{
				RoomPopulation roomPopulation = evt.RoomPopulations[i];
				DataEntityHandle handle = evt.Handles[i];
				addOtherPlayerIglooListing(handle, roomPopulation.populationScaled);
			}
			return false;
		}

		private bool onPopulareIglooListReceived(IglooServiceEvents.PopularIgloosListLoaded evt)
		{
			for (int i = 0; i < evt.IglooListItems.Count; i++)
			{
				addOtherPlayerDetails(evt.IglooListItems[i].ownerData);
				addOtherPlayerIglooListing(evt.IglooListItems[i]);
			}
			return false;
		}

		private void addOtherPlayerIglooListing(DataEntityHandle handle, RoomPopulationScale scale)
		{
			IglooListingData component;
			if (!dataEntityCollection.TryGetComponent(handle, out component))
			{
				component = dataEntityCollection.AddComponent<IglooListingData>(handle);
			}
			component.RoomPopulation = scale;
		}

		private void addOtherPlayerIglooListing(IglooListItem iglooListItem)
		{
			DataEntityHandle handle = PlayerDataEntityFactory.CreateRemotePlayerEntity(dataEntityCollection, iglooListItem.ownerData.name);
			IglooListingData component;
			if (!dataEntityCollection.TryGetComponent(handle, out component))
			{
				component = dataEntityCollection.AddComponent<IglooListingData>(handle);
			}
			RoomPopulationScale roomPopulation = RoomPopulationScale.ZERO;
			if (iglooListItem.iglooPopulation.HasValue)
			{
				roomPopulation = iglooListItem.iglooPopulation.Value;
			}
			component.RoomPopulation = roomPopulation;
		}

		private bool onOtherPlayerDataListReceived(PlayerStateServiceEvents.OtherPlayerDataListReceived evt)
		{
			if (evt.Data.Count > 0)
			{
				for (int i = 0; i < evt.Data.Count; i++)
				{
					addOtherPlayerDetails(evt.Data[i]);
				}
			}
			return false;
		}

		private void addOtherPlayerDetails(OtherPlayerData data)
		{
			DataEntityHandle handle;
			if (data.id.type == PlayerId.PlayerIdType.SWID)
			{
				handle = dataEntityCollection.FindEntity<SwidData, string>(data.id.id);
				if (DataEntityHandle.IsNullValue(handle))
				{
					handle = PlayerDataEntityFactory.CreateRemotePlayerEntity(dataEntityCollection, data.name);
				}
			}
			else
			{
				handle = PlayerDataEntityFactory.CreateRemotePlayerEntity(dataEntityCollection, data.name);
			}
			AvatarDetailsData component;
			if (!dataEntityCollection.TryGetComponent(handle, out component))
			{
				component = dataEntityCollection.AddComponent<AvatarDetailsData>(handle);
			}
			ProfileData component2;
			if (!dataEntityCollection.TryGetComponent(handle, out component2))
			{
				component2 = dataEntityCollection.AddComponent<ProfileData>(handle);
			}
			MembershipData component3;
			if (!dataEntityCollection.TryGetComponent(handle, out component3))
			{
				component3 = dataEntityCollection.AddComponent<MembershipData>(handle);
			}
			PresenceData component4;
			if (!dataEntityCollection.TryGetComponent(handle, out component4))
			{
				component4 = dataEntityCollection.AddComponent<PresenceData>(handle);
			}
			setUpAvatarDetails(component, data);
			setUpProfile(component2, component4, component3, data);
			component4.IsDisconnecting = false;
		}

		private void setUpAvatarDetails(AvatarDetailsData avatarDetailsData, OtherPlayerData data)
		{
			DCustomEquipment[] outfit = (data.outfit == null) ? new DCustomEquipment[0] : CustomEquipmentResponseAdaptor.ConvertResponseToOutfit(data.outfit);
			if (data.profile != null)
			{
				Dictionary<int, AvatarColorDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, AvatarColorDefinition>>();
				AvatarColorDefinition value;
				if (dictionary.TryGetValue(data.profile.colour, out value) && value != null)
				{
					Color color;
					if (ColorUtility.TryParseHtmlString("#" + value.Color, out color))
					{
						avatarDetailsData.Init(outfit, color);
						return;
					}
					Log.LogErrorFormatted(this, "Could not parse a color from color string {0}", value.Color);
					avatarDetailsData.Init(outfit);
				}
				else
				{
					Log.LogErrorFormatted(this, "Avatar color definitions did not contain a value for color id {0}", data.profile.colour);
					avatarDetailsData.Init(outfit);
				}
			}
			else
			{
				avatarDetailsData.Init(outfit);
			}
		}

		private void setUpProfile(ProfileData profileData, PresenceData presenceData, MembershipData membershipData, OtherPlayerData data)
		{
			profileData.PenguinAgeInDays = ((data.profile != null) ? data.profile.daysOld : 0);
			profileData.MascotXP = data.mascotXP;
			membershipData.IsMember = data.member;
			profileData.ZoneId = data.zoneId;
			membershipData.MembershipType = (data.member ? MembershipType.Member : MembershipType.None);
			if (!membershipData.IsMember && Service.Get<AllAccessService>().IsAllAccessActive())
			{
				membershipData.IsMember = true;
				membershipData.MembershipType = MembershipType.AllAccessEventMember;
			}
			if (data.onlineLocation != null)
			{
				profileData.IsOnline = true;
				presenceData.World = data.onlineLocation.world;
				presenceData.Room = data.onlineLocation.zoneId.name;
				presenceData.ContentIdentifier = data.onlineLocation.contentIdentifier;
				if (string.IsNullOrEmpty(data.onlineLocation.zoneId.instanceId))
				{
					presenceData.InstanceRoom = null;
				}
				else
				{
					presenceData.InstanceRoom = data.onlineLocation.zoneId;
				}
			}
			else
			{
				profileData.IsOnline = false;
				presenceData.World = null;
				presenceData.Room = null;
				presenceData.ContentIdentifier = null;
				presenceData.InstanceRoom = null;
			}
		}
	}
}
