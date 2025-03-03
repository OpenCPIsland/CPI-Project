using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Manimal.Common.Util;
using Disney.MobileNetwork;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	[HttpPOST]
	[RequestQueue("Quest")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPath("cp-api-base-uri", "/game/v1/worlds/{$world}/language/{$language}/rooms/{$room}/players")]
	[HttpAccept("application/json")]
	public class PostRoomPlayersOperation : CPAPIHttpOperation
	{
		[HttpUriSegment("world")]
		public string World;

		[HttpUriSegment("language")]
		public string Language;

		[HttpUriSegment("room")]
		public string Room;

		[HttpQueryString("bypassCaptcha")]
		public bool BypassCaptcha = false;

		[HttpResponseJsonBody]
		public SignedResponse<JoinRoomData> SignedJoinRoomData;

		public PostRoomPlayersOperation(string world, string language, string room)
		{
			World = world;
			Language = language;
			Room = room;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			SignedJoinRoomData = JoinRoom(World, Language, new ZoneId
			{
				name = Room
			}, offlineDatabase, offlineDefinitions);
		}

		public static SignedResponse<JoinRoomData> JoinRoom(string world, string language, ZoneId zoneId, OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			Dictionary<string, long> earnedRewards = new Dictionary<string, long>();
			foreach (ClubPenguin.Net.Offline.InRoomRewards.InRoomReward item in offlineDatabase.Read<ClubPenguin.Net.Offline.InRoomRewards>().Collected)
			{
				if (item.Room == zoneId.name)
				{
					earnedRewards = item.Collected;
					break;
				}
			}
			ClubPenguin.Net.Offline.Profile profile = offlineDatabase.Read<ClubPenguin.Net.Offline.Profile>();
			PlayerRoomData playerRoomData = default(PlayerRoomData);
			playerRoomData.assets = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerAssets>().Assets;
			playerRoomData.consumableInventory = new ClubPenguin.Net.Domain.ConsumableInventory
			{
				inventoryMap = offlineDatabase.Read<ClubPenguin.Net.Offline.ConsumableInventory>().Inventory
			};
			playerRoomData.dailyTaskProgress = new TaskProgressList();
			playerRoomData.member = true;
			playerRoomData.outfit = new ClubPenguin.Net.Domain.PlayerOutfitDetails
			{
				parts = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerOutfitDetails>().Parts.ToArray()
			};
			playerRoomData.profile = new ClubPenguin.Net.Domain.Profile
			{
				colour = profile.Colour,
				daysOld = profile.DaysOld
			};
			playerRoomData.quests = SetProgressOperation.GetQuestStateCollection(offlineDatabase.Read<QuestStates>(), offlineDefinitions, true);
			PlayerRoomData playerRoomData2 = playerRoomData;
			RoomIdentifier roomIdentifier = new RoomIdentifier(world, LocalizationLanguage.GetLanguageFromLanguageString(language), zoneId, new ContentIdentifier("1.13.0", "offline", DateTime.UtcNow.ToString("yyyy-MM-dd"), "NONE").ToString());
			int equippedTubeId = offlineDatabase.Read<TubeData>().EquippedTubeId;
			Random random = new Random();
			byte[] array = new byte[8];
			random.NextBytes(array);
			SignedResponse<JoinRoomData> signedResponse = new SignedResponse<JoinRoomData>();
			signedResponse.Data = new JoinRoomData
			{
				earnedRewards = earnedRewards,
				membershipRights = new MembershipRights
				{
					member = true
				},
				playerRoomData = playerRoomData2,
				room = roomIdentifier.ToString(),
				selectedTubeId = equippedTubeId,
				sessionId = Math.Abs(BitConverter.ToInt64(array, 0)),
				host = Service.Get<ICommonGameSettings>().GameServerHost,
				tcpPort = 9933,
				userName = offlineDatabase.Read<RegistrationProfile>().userName,
				swid = offlineDatabase.AccessToken
			};
			signedResponse.swid = offlineDatabase.AccessToken;
			return signedResponse;
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			TubeData value = offlineDatabase.Read<TubeData>();
			value.EquippedTubeId = SignedJoinRoomData.Data.selectedTubeId;
			offlineDatabase.Write(value);
			ClubPenguin.Net.Offline.ConsumableInventory value2 = offlineDatabase.Read<ClubPenguin.Net.Offline.ConsumableInventory>();
			value2.Inventory = SignedJoinRoomData.Data.playerRoomData.consumableInventory.inventoryMap;
			offlineDatabase.Write(value2);
			ClubPenguin.Net.Offline.PlayerAssets value3 = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerAssets>();
			value3.Assets = SignedJoinRoomData.Data.playerRoomData.assets;
			offlineDatabase.Write(value3);
			ClubPenguin.Net.Offline.PlayerOutfitDetails value4 = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerOutfitDetails>();
			value4.Parts = new List<CustomEquipment>(SignedJoinRoomData.Data.playerRoomData.outfit.parts);
			offlineDatabase.Write(value4);
			ClubPenguin.Net.Offline.Profile value5 = offlineDatabase.Read<ClubPenguin.Net.Offline.Profile>();
			value5.Colour = SignedJoinRoomData.Data.playerRoomData.profile.colour;
			value5.DateCreated = DateTime.UtcNow.AddDays(-1 * SignedJoinRoomData.Data.playerRoomData.profile.daysOld).GetTimeInMilliseconds();
			offlineDatabase.Write(value5);
			SetProgressOperation.SetOfflineQuestStateCollection(offlineDatabase, SignedJoinRoomData.Data.playerRoomData.quests);
		}
	}
}
