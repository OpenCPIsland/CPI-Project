using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPath("cp-api-base-uri", "/reward/v1/qa/dailySpin")]
	[HttpContentType("application/json")]
	[HttpPOST]
	public class QASetDailySpinDataOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public QASetDailySpinDataParams requestBody;

		public QASetDailySpinDataOperation(QASetDailySpinDataParams dailySpinData)
		{
			requestBody = dailySpinData;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ClubPenguin.Net.Offline.DailySpinData value = offlineDatabase.Read<ClubPenguin.Net.Offline.DailySpinData>();
			value.CurrentChestId = requestBody.CurrentChestId;
			value.NumPunchesOnCurrentChest = requestBody.NumPunchesOnCurrentChest;
			value.NumChestsReceivedOfCurrentChestId = requestBody.NumChestsReceivedOfCurrentChestId;
			value.TimeOfLastSpinInMilliseconds = requestBody.TimeOfLastSpinInMilliseconds;
			if (requestBody.ResetRewards)
			{
				value.EarnedNonRepeatableRewardIds.Clear();
				value.EarnedRepeatableRewardIds.Clear();
			}
			offlineDatabase.Write(value);
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			PerformOfflineAction(offlineDatabase, offlineDefinitions);
		}
	}
}
