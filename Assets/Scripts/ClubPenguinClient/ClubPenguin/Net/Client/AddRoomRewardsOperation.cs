using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	[HttpContentType("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPOST]
	[HttpPath("cp-api-base-uri", "/reward/v1/room")]
	public class AddRoomRewardsOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public SignedResponse<ClubPenguin.Net.Domain.InRoomRewards> RequestBody;

		[HttpResponseJsonBody]
		public RewardGrantedResponse ResponseBody;

		public AddRoomRewardsOperation(SignedResponse<ClubPenguin.Net.Domain.InRoomRewards> inRoomRewards)
		{
			RequestBody = inRoomRewards;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			string room = RequestBody.Data.room;
			ResponseBody = new RewardGrantedResponse();
			ClubPenguin.Net.Offline.InRoomRewards value = offlineDatabase.Read<ClubPenguin.Net.Offline.InRoomRewards>();
			int index = -1;
			Dictionary<string, long> dictionary = null;
			for (int i = 0; i < value.Collected.Count; i++)
			{
				if (value.Collected[i].Room == room)
				{
					dictionary = value.Collected[i].Collected;
					index = i;
					break;
				}
			}
			if (dictionary == null)
			{
				dictionary = new Dictionary<string, long>();
				value.Collected.Add(new ClubPenguin.Net.Offline.InRoomRewards.InRoomReward
				{
					Collected = dictionary,
					Room = room
				});
				index = value.Collected.Count - 1;
			}
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, long> item in RequestBody.Data.collected)
			{
				if (!dictionary.ContainsKey(item.Key) || dictionary[item.Key] < item.Value)
				{
					dictionary[item.Key] = item.Value;
					list.Add(item.Key);
				}
			}
			value.Collected[index] = new ClubPenguin.Net.Offline.InRoomRewards.InRoomReward
			{
				Collected = dictionary,
				Room = room
			};
			offlineDatabase.Write(value);
			if (list.Count > 0)
			{
				Reward inRoomReward = offlineDefinitions.GetInRoomReward(list);
				if (!inRoomReward.isEmpty())
				{
					offlineDefinitions.AddReward(inRoomReward, ResponseBody);
				}
			}
			ResponseBody.assets = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerAssets>().Assets;
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ClubPenguin.Net.Offline.PlayerAssets value = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerAssets>();
			value.Assets = ResponseBody.assets;
			offlineDatabase.Write(value);
		}
	}
}
