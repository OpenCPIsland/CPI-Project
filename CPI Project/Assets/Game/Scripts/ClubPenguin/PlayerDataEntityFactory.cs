using ClubPenguin.Analytics;
using ClubPenguin.Avatar;
using ClubPenguin.CellPhone;
using ClubPenguin.Chat;
using ClubPenguin.Net.Domain;
using ClubPenguin.Tutorial;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ClubPenguin
{
	public static class PlayerDataEntityFactory
	{
		public static DataEntityHandle AddLocalPlayerProfileDataComponents(DataEntityCollection dataEntityCollection, LocalPlayerData data, bool isOnline = false)
		{
			DataEntityHandle dataEntityHandle = dataEntityCollection.FindEntityByName("LocalPlayer");
			if (dataEntityHandle.IsNull)
			{
				return dataEntityHandle;
			}
			if (data == null)
			{
				return dataEntityHandle;
			}
			DisplayNameData component2;
			if (!dataEntityCollection.TryGetComponent(dataEntityHandle, out component2))
			{
				component2 = dataEntityCollection.AddComponent<DisplayNameData>(dataEntityHandle);
			}
			component2.DisplayName = data.name;
			QuestStateData component3;
			if (!dataEntityCollection.TryGetComponent(dataEntityHandle, out component3))
			{
				component3 = dataEntityCollection.AddComponent<QuestStateData>(dataEntityHandle);
			}
			component3.Data = data.quests;
			AvatarDetailsData component4;
			if (!dataEntityCollection.TryGetComponent(dataEntityHandle, out component4))
			{
				component4 = dataEntityCollection.AddComponent<AvatarDetailsData>(dataEntityHandle);
				component4.BodyColor = AvatarService.DefaultBodyColor;
				component4.Outfit = new DCustomEquipment[0];
			}
			if (data.outfit != null && data.outfit.Count > 0)
			{
				component4.Outfit = CustomEquipmentResponseAdaptor.ConvertResponseToOutfit(data.outfit);
			}
			if (data.profile != null)
			{
				Dictionary<int, AvatarColorDefinition> avatarColors = Service.Get<GameData>().Get<Dictionary<int, AvatarColorDefinition>>();
				component4.BodyColor = AvatarBodyColorAdaptor.GetColorFromDefinitions(avatarColors, data.profile.colour);
			}
			ProfileData component5;
			if (!dataEntityCollection.TryGetComponent(dataEntityHandle, out component5))
			{
				component5 = dataEntityCollection.AddComponent<ProfileData>(dataEntityHandle);
			}
			component5.PenguinAgeInDays = ((data.profile != null) ? data.profile.daysOld : 0);
			component5.IsOnline = isOnline;
			component5.IsMigratedPlayer = (data.migrationData != null && data.migrationData.status.Equals("MIGRATED"));
			component5.ZoneId = data.zoneId;
			MembershipData component6;
			if (!dataEntityCollection.TryGetComponent(dataEntityHandle, out component6))
			{
				component6 = dataEntityCollection.AddComponent<MembershipData>(dataEntityHandle);
			}
			component6.IsMember = data.member;
			component6.MembershipType = (data.member ? MembershipType.Member : MembershipType.None);
			component6.MembershipExpireDate = data.membershipExpireDate;
			component6.MembershipTrialAvailable = data.trialAvailable;
			if (!component6.IsMember && Service.Get<AllAccessService>().IsAllAccessActive())
			{
				component6.IsMember = true;
				component6.MembershipType = MembershipType.AllAccessEventMember;
			}
			logPlayerStatusBI(data);
			SubscriptionData component7;
			if (!dataEntityCollection.TryGetComponent(dataEntityHandle, out component7))
			{
				component7 = dataEntityCollection.AddComponent<SubscriptionData>(dataEntityHandle);
			}
			component7.SubscriptionVendor = data.subscriptionVendor;
			component7.SubscriptionProductId = data.subscriptionProductId;
			component7.SubscriptionPaymentPending = data.subscriptionPaymentPending;
			component7.SubscriptionRecurring = data.recurring;
			if (data.migrationData != null && data.migrationData.legacyAccountData != null)
			{
				LegacyProfileData component8;
				if (!dataEntityCollection.TryGetComponent(dataEntityHandle, out component8))
				{
					component8 = dataEntityCollection.AddComponent<LegacyProfileData>(dataEntityHandle);
				}
				component8.Username = data.migrationData.legacyAccountData.username;
				component8.IsMember = data.migrationData.legacyAccountData.member;
				component8.CreatedDate = data.migrationData.legacyAccountData.createdDate;
				component8.MigratedDate = data.migrationData.migratedDate;
			}
			byte[] array = new byte[data.tutorialData.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (byte)data.tutorialData[i];
			}
			TutorialData component9;
			if (!dataEntityCollection.TryGetComponent(dataEntityHandle, out component9))
			{
				component9 = dataEntityCollection.AddComponent<TutorialData>(dataEntityHandle);
			}
			component9.Data = new BitArray(array);
			switch (data.id.type)
			{
			case PlayerId.PlayerIdType.SESSION_ID:
				if (!dataEntityCollection.HasComponent<SessionIdData>(dataEntityHandle))
				{
					dataEntityCollection.AddComponent(dataEntityHandle, delegate(SessionIdData component)
					{
						component.SessionId = Convert.ToInt64(data.id.id);
					});
				}
				break;
			case PlayerId.PlayerIdType.SWID:
				if (!dataEntityCollection.HasComponent<SwidData>(dataEntityHandle))
				{
					dataEntityCollection.AddComponent<SwidData>(dataEntityHandle).Swid = data.id.id;
				}
				break;
			}
			setClaimedRewardIds(dataEntityCollection, dataEntityHandle, data.claimedRewardIds);
			addDailySpinDataComponent(dataEntityCollection, dataEntityHandle, data);
			return dataEntityHandle;
		}

		private static void addDailySpinDataComponent(DataEntityCollection dataEntityCollection, DataEntityHandle handle, LocalPlayerData localPlayerData)
		{
			if (localPlayerData.dailySpinData != null)
			{
				DailySpinEntityData component;
				if (!dataEntityCollection.TryGetComponent(handle, out component))
				{
					component = dataEntityCollection.AddComponent<DailySpinEntityData>(handle);
				}
				component.CurrentChestId = localPlayerData.dailySpinData.currentChestId;
				component.NumChestsReceivedOfCurrentChestId = localPlayerData.dailySpinData.numChestsReceivedOfCurrentChestId;
				component.NumPunchesOnCurrentChest = localPlayerData.dailySpinData.numPunchesOnCurrentChest;
				component.TimeOfLastSpinInMilliseconds = localPlayerData.dailySpinData.timeOfLastSpinInMilliseconds;
			}
		}

		private static void setClaimedRewardIds(DataEntityCollection dataEntityCollection, DataEntityHandle handle, List<int> claimedRewardIds)
		{
			if (!dataEntityCollection.HasComponent<ClaimedRewardIdsData>(handle))
			{
				dataEntityCollection.AddComponent<ClaimedRewardIdsData>(handle);
			}
			if (claimedRewardIds != null && claimedRewardIds.Count > 0)
			{
				ClaimedRewardIdsData component = dataEntityCollection.GetComponent<ClaimedRewardIdsData>(handle);
				component.RewardIds = claimedRewardIds;
			}
		}

		private static void logPlayerStatusBI(LocalPlayerData data)
		{
			string tier = "free";
			if (data.member)
			{
				tier = "paid_member";
			}
			else if (data.membershipExpireDate > 0)
			{
				tier = "lapsed_member";
			}
			Service.Get<ICPSwrveService>().Action("login_type", tier);
		}

		public static DataEntityHandle CreateRemotePlayerEntity(DataEntityCollection dataEntityCollection, string displayName, long sessionId)
		{
			DataEntityHandle dataEntityHandle = createRemotePlayerEntity(dataEntityCollection, displayName);
			SessionIdData component2;
			if (dataEntityCollection.TryGetComponent(dataEntityHandle, out component2))
			{
				component2.SessionId = sessionId;
			}
			else
			{
				component2 = dataEntityCollection.AddComponent(dataEntityHandle, delegate(SessionIdData component)
				{
					component.SessionId = sessionId;
				});
			}
			return dataEntityHandle;
		}

		public static DataEntityHandle CreateRemotePlayerEntity(DataEntityCollection dataEntityCollection, string displayName, string swid)
		{
			DataEntityHandle dataEntityHandle = createRemotePlayerEntity(dataEntityCollection, displayName);
			SwidData component;
			if (!dataEntityCollection.TryGetComponent(dataEntityHandle, out component))
			{
				component = dataEntityCollection.AddComponent<SwidData>(dataEntityHandle);
			}
			component.Swid = swid;
			return dataEntityHandle;
		}

		public static DataEntityHandle CreateRemotePlayerEntity(DataEntityCollection dataEntityCollection, string displayName)
		{
			return createRemotePlayerEntity(dataEntityCollection, displayName);
		}

		private static DataEntityHandle createRemotePlayerEntity(DataEntityCollection dataEntityCollection, string displayName)
		{
			DataEntityHandle dataEntityHandle;
			if (!dataEntityCollection.TryFindEntity<DisplayNameData, string>(displayName, out dataEntityHandle))
			{
				dataEntityHandle = dataEntityCollection.AddEntity("rp_" + displayName);
			}
			if (!dataEntityCollection.HasComponent<DisplayNameData>(dataEntityHandle))
			{
				dataEntityCollection.AddComponent<DisplayNameData>(dataEntityHandle).DisplayName = displayName;
			}
			return dataEntityHandle;
		}

		public static void AddLocalPlayerSessionScopeDataComponents(DataEntityCollection dataEntityCollection, DataEntityHandle handle)
		{
			if (!dataEntityCollection.HasComponent<CoinsData>(handle))
			{
				dataEntityCollection.AddComponent<CoinsData>(handle);
			}
			if (!dataEntityCollection.HasComponent<CollectiblesData>(handle))
			{
				dataEntityCollection.AddComponent<CollectiblesData>(handle);
			}
			if (!dataEntityCollection.HasComponent<ConsumableInventoryData>(handle))
			{
				dataEntityCollection.AddComponent<ConsumableInventoryData>(handle);
			}
			if (!dataEntityCollection.HasComponent<MiniGamePlayCountData>(handle))
			{
				dataEntityCollection.AddComponent<MiniGamePlayCountData>(handle);
			}
		}

		public static void AddCommonDataComponents(DataEntityCollection dataEntityCollection, DataEntityHandle handle)
		{
			if (!dataEntityCollection.HasComponent<TubeData>(handle))
			{
				dataEntityCollection.AddComponent<TubeData>(handle);
			}
			if (!dataEntityCollection.HasComponent<HeldObjectsData>(handle))
			{
				dataEntityCollection.AddComponent<HeldObjectsData>(handle);
			}
			if (!dataEntityCollection.HasComponent<PresenceData>(handle))
			{
				dataEntityCollection.AddComponent<PresenceData>(handle);
			}
		}

		public static void AddCommonZoneScopeDataComponents(DataEntityCollection dataEntityCollection, DataEntityHandle handle)
		{
			if (!dataEntityCollection.HasComponent<LocomotionData>(handle))
			{
				dataEntityCollection.AddComponent<LocomotionData>(handle);
			}
			if (!dataEntityCollection.HasComponent<PositionData>(handle))
			{
				dataEntityCollection.AddComponent<PositionData>(handle);
			}
			if (!dataEntityCollection.HasComponent<ChatHistoryData>(handle))
			{
				dataEntityCollection.AddComponent<ChatHistoryData>(handle);
			}
		}
	}
}
