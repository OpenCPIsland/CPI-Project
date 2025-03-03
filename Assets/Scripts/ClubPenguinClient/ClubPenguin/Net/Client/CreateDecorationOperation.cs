using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Domain.Decoration;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpPath("cp-api-base-uri", "/igloo/v1/decorations/{$type}/{$definitionId}")]
	[HttpAccept("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPOST]
	[HttpContentType("application/json")]
	public class CreateDecorationOperation : CPAPIHttpOperation
	{
		[HttpUriSegment("type")]
		public int Type;

		[HttpUriSegment("definitionId")]
		public int DefinitionId;

		[HttpQueryString("count")]
		public int Count;

		[HttpResponseJsonBody]
		public CreateDecorationResponse ResponseBody;

		public CreateDecorationOperation(int definitionId, DecorationType type, int count)
		{
			DefinitionId = definitionId;
			Count = count;
			Type = (int)type;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			DecorationId decorationId = new DecorationId(DefinitionId, (DecorationType)Type);
			QACreateDecorationOperation.AddDecoration(decorationId, Count, offlineDatabase);
			offlineDefinitions.SubtractDecorationCost(decorationId, Count);
			ResponseBody = new CreateDecorationResponse
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
