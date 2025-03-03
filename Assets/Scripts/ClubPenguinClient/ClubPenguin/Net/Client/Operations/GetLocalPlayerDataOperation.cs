using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Domain.Igloo;
using ClubPenguin.Net.Domain.Scene;
using ClubPenguin.Net.Offline;
using Disney.Manimal.Common.Util;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client.Operations
{
	[HttpAccept("application/json")]
	[HttpPath("cp-api-base-uri", "/player/v1/")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpGET]
	public class GetLocalPlayerDataOperation : CPAPIHttpOperation
	{
		[HttpResponseJsonBody]
		public LocalPlayerData ResponseBody;

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ResponseBody = new LocalPlayerData();
			ResponseBody.member = true;
			ResponseBody.membershipExpireDate = DateTime.UtcNow.AddMonths(1).GetTimeInMilliseconds();
			ResponseBody.trialAvailable = false;
			ResponseBody.subscriptionPaymentPending = false;
			ResponseBody.id = new PlayerId
			{
				id = offlineDatabase.AccessToken,
				type = PlayerId.PlayerIdType.SWID
			};
			ClubPenguin.Net.Offline.Profile profile = offlineDatabase.Read<ClubPenguin.Net.Offline.Profile>();
			ResponseBody.profile = new ClubPenguin.Net.Domain.Profile
			{
				colour = profile.Colour,
				daysOld = profile.DaysOld
			};
			ResponseBody.outfit = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerOutfitDetails>().Parts;
			ResponseBody.mascotXP = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerAssets>().Assets.mascotXP;
			ResponseBody.minigameProgress = new List<MinigameProgress>();
			ResponseBody.quests = SetProgressOperation.GetQuestStateCollection(offlineDatabase.Read<QuestStates>(), offlineDefinitions, true);
			ResponseBody.tutorialData = new List<sbyte>(offlineDatabase.Read<TutorialData>().Bytes);
			List<Breadcrumb> list = new List<Breadcrumb>();
			foreach (Breadcrumb breadcrumb in offlineDatabase.Read<BreadcrumbCollection>().breadcrumbs)
			{
				list.Add(new Breadcrumb
				{
					id = breadcrumb.id,
					breadcrumbType = breadcrumb.breadcrumbType
				});
			}
			ResponseBody.breadcrumbs = new BreadcrumbsResponse
			{
				breadcrumbs = list
			};
			RegistrationProfile registrationProfile = offlineDatabase.Read<RegistrationProfile>();
			ResponseBody.name = registrationProfile.displayName;
			if (string.IsNullOrEmpty(ResponseBody.name))
			{
				ResponseBody.name = registrationProfile.userName;
			}
			ResponseBody.claimedRewardIds = offlineDatabase.Read<ClaimableRewardData>().ClimedRewards;
			ZoneId zoneId = new ZoneId();
			zoneId.name = "DefaultIgloo";
			zoneId.instanceId = registrationProfile.Id();
			ZoneId zoneId2 = zoneId;
			IglooEntity iglooEntity = offlineDatabase.Read<IglooEntity>();
			if (iglooEntity.Data.activeLayout != null)
			{
				string zoneId3 = iglooEntity.Data.activeLayout.zoneId;
				if (!string.IsNullOrEmpty(zoneId3))
				{
					zoneId2.name = zoneId3;
				}
			}
			ResponseBody.zoneId = zoneId2;
			List<SavedIglooLayoutSummary> list2 = new List<SavedIglooLayoutSummary>();
			SceneLayoutEntity sceneLayoutEntity = offlineDatabase.Read<SceneLayoutEntity>();
			foreach (SavedSceneLayout layout in sceneLayoutEntity.Layouts)
			{
				list2.Add(new SavedIglooLayoutSummary
				{
					createdDate = layout.createdDate.GetValueOrDefault(0L),
					lastUpdatedDate = layout.lastModifiedDate.GetValueOrDefault(0L),
					layoutId = layout.layoutId,
					lot = layout.zoneId,
					memberOnly = layout.memberOnly,
					name = layout.name
				});
			}
			ResponseBody.iglooLayouts = new SavedIglooLayoutsSummary
			{
				activeLayoutId = iglooEntity.Data.activeLayoutId,
				activeLayoutServerChangeNotification = ActiveLayoutServerChangeNotification.NoServerChange,
				visibility = iglooEntity.Data.visibility.GetValueOrDefault(IglooVisibility.PRIVATE),
				layouts = list2
			};
			ClubPenguin.Net.Offline.DailySpinData dailySpinData = offlineDatabase.Read<ClubPenguin.Net.Offline.DailySpinData>();
			ResponseBody.dailySpinData = new ClubPenguin.Net.Domain.DailySpinData
			{
				currentChestId = dailySpinData.CurrentChestId,
				numChestsReceivedOfCurrentChestId = dailySpinData.NumChestsReceivedOfCurrentChestId,
				numPunchesOnCurrentChest = dailySpinData.NumPunchesOnCurrentChest,
				timeOfLastSpinInMilliseconds = dailySpinData.TimeOfLastSpinInMilliseconds
			};
			ClubPenguin.Net.Offline.SessionData sessionData = offlineDatabase.Read<ClubPenguin.Net.Offline.SessionData>();
			if (sessionData.Data.sessionId != 0)
			{
				ResponseBody.onlineLocation = sessionData.CurrentRoom;
			}
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			BreadcrumbCollection value = offlineDatabase.Read<BreadcrumbCollection>();
			value.breadcrumbs = ResponseBody.breadcrumbs.breadcrumbs;
			offlineDatabase.Write(value);
			ClaimableRewardData value2 = offlineDatabase.Read<ClaimableRewardData>();
			value2.ClimedRewards = ResponseBody.claimedRewardIds;
			offlineDatabase.Write(value2);
			ClubPenguin.Net.Offline.DailySpinData value3 = offlineDatabase.Read<ClubPenguin.Net.Offline.DailySpinData>();
			value3.CurrentChestId = ResponseBody.dailySpinData.currentChestId;
			value3.NumChestsReceivedOfCurrentChestId = ResponseBody.dailySpinData.numChestsReceivedOfCurrentChestId;
			value3.NumPunchesOnCurrentChest = ResponseBody.dailySpinData.numPunchesOnCurrentChest;
			value3.TimeOfLastSpinInMilliseconds = ResponseBody.dailySpinData.timeOfLastSpinInMilliseconds;
			offlineDatabase.Write(value3);
			ClubPenguin.Net.Offline.PlayerOutfitDetails value4 = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerOutfitDetails>();
			value4.Parts = ResponseBody.outfit;
			offlineDatabase.Write(value4);
			ClubPenguin.Net.Offline.Profile value5 = offlineDatabase.Read<ClubPenguin.Net.Offline.Profile>();
			value5.Colour = ResponseBody.profile.colour;
			value5.DateCreated = DateTime.UtcNow.AddDays(-1 * ResponseBody.profile.daysOld).GetTimeInMilliseconds();
			offlineDatabase.Write(value5);
			SetProgressOperation.SetOfflineQuestStateCollection(offlineDatabase, ResponseBody.quests);
			TutorialData value6 = offlineDatabase.Read<TutorialData>();
			for (int i = 0; i < value6.Bytes.Length; i++)
			{
				sbyte b = 0;
				if (i < ResponseBody.tutorialData.Count)
				{
					b = ResponseBody.tutorialData[i];
				}
				value6.Bytes[i] = b;
			}
			offlineDatabase.Write(value6);
		}
	}
}
