using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Domain.Decoration;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpAccept("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPOST]
	[HttpPath("cp-api-base-uri", "/igloo/v1/decorations/{$decorationId}/increment/{$count}")]
	public class PurchaseDecorationOperation : CPAPIHttpOperation
	{
		[HttpUriSegment("decorationId")]
		public string DecorationId;

		[HttpUriSegment("count")]
		public int Count;

		[HttpResponseJsonBody]
		public UpdateDecorationResponse ResponseBody;

		public PurchaseDecorationOperation(string decorationId, int count)
		{
			DecorationId = decorationId;
			Count = count;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			DecorationId decorationId = ClubPenguin.Net.Domain.Decoration.DecorationId.FromString(DecorationId);
			QACreateDecorationOperation.AddDecoration(decorationId, Count, offlineDatabase);
			offlineDefinitions.SubtractDecorationCost(decorationId, Count);
			ResponseBody = new UpdateDecorationResponse
			{
				assets = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerAssets>().Assets,
				decorationId = decorationId
			};
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			QACreateDecorationOperation.AddDecoration(ResponseBody.decorationId, Count, offlineDatabase);
			ClubPenguin.Net.Offline.PlayerAssets value = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerAssets>();
			value.Assets = ResponseBody.assets;
			offlineDatabase.Write(value);
		}
	}
}
