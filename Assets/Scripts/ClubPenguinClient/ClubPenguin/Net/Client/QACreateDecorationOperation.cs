using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Domain.Decoration;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpAccept("application/json")]
	[HttpPOST]
	[HttpPath("cp-api-base-uri", "/igloo/v1/qa/decorations/{$type}/{$definitionId}")]
	[HttpContentType("application/json")]
	public class QACreateDecorationOperation : CPAPIHttpOperation
	{
		[HttpUriSegment("type")]
		public int Type;

		[HttpUriSegment("definitionId")]
		public int DefinitionId;

		[HttpQueryString("count")]
		public int Count;

		[HttpResponseJsonBody]
		public CreateDecorationResponse ResponseBody;

		public QACreateDecorationOperation(int definitionId, DecorationType type, int count)
		{
			DefinitionId = definitionId;
			Count = count;
			Type = (int)type;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			DecorationId decorationId = new DecorationId(DefinitionId, (DecorationType)Type);
			AddDecoration(decorationId, Count, offlineDatabase);
			ResponseBody = new CreateDecorationResponse
			{
				assets = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerAssets>().Assets,
				decorationId = decorationId
			};
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			AddDecoration(ResponseBody.decorationId, Count, offlineDatabase);
			ClubPenguin.Net.Offline.PlayerAssets value = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerAssets>();
			value.Assets = ResponseBody.assets;
			offlineDatabase.Write(value);
		}

		public static void AddDecoration(DecorationId decoration, int count, OfflineDatabase offlineDatabase)
		{
			DecorationInventoryEntity value = offlineDatabase.Read<DecorationInventoryEntity>();
			if (value.Inventory.ContainsKey(decoration))
			{
				value.Inventory[decoration] += count;
			}
			else
			{
				value.Inventory[decoration] = count;
			}
			offlineDatabase.Write(value);
		}
	}
}
