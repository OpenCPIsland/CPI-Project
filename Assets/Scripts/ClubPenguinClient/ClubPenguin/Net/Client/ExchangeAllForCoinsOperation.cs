using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpContentType("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPOST]
	[HttpPath("cp-api-base-uri", "/reward/v1/exchangeall")]
	public class ExchangeAllForCoinsOperation : CPAPIHttpOperation
	{
		[HttpResponseJsonBody]
		public RewardGrantedResponse ResponseBody;

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ClubPenguin.Net.Offline.PlayerAssets value = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerAssets>();
			int coinsForExchange = offlineDefinitions.GetCoinsForExchange(value.Assets.collectibleCurrencies);
			value.Assets.coins += coinsForExchange;
			value.Assets.collectibleCurrencies.Clear();
			offlineDatabase.Write(value);
			ResponseBody = new RewardGrantedResponse
			{
				assets = value.Assets
			};
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ClubPenguin.Net.Offline.PlayerAssets value = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerAssets>();
			value.Assets = ResponseBody.assets;
			offlineDatabase.Write(value);
		}
	}
}
