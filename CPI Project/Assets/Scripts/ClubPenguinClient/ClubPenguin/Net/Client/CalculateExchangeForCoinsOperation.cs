using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpAccept("application/json")]
	[HttpGET]
	[HttpPath("cp-api-base-uri", "/reward/v1/calculateexchangeall")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	public class CalculateExchangeForCoinsOperation : CPAPIHttpOperation
	{
		[HttpResponseJsonBody]
		public RewardCalculatedResponse ResponseBody;

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ClubPenguin.Net.Offline.PlayerAssets playerAssets = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerAssets>();
			ResponseBody = new RewardCalculatedResponse
			{
				coins = offlineDefinitions.GetCoinsForExchange(playerAssets.Assets.collectibleCurrencies)
			};
		}
	}
}
