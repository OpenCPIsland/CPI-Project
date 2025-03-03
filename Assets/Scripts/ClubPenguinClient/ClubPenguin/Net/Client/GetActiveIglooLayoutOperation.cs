using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain.Scene;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpPath("cp-api-base-uri", "/igloo/v1/iglooId/layout/active")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPOST]
	[HttpAccept("application/json")]
	[RequestQueue("IglooLayout")]
	public class GetActiveIglooLayoutOperation : CPAPIHttpOperation
	{
		[HttpRequestTextBody]
		public string IglooId;

		[HttpResponseJsonBody]
		public SceneLayout ResponseBody;

		public GetActiveIglooLayoutOperation(string iglooId)
		{
			IglooId = iglooId;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			IglooEntity iglooEntity = offlineDatabase.Read<IglooEntity>();
			ResponseBody = iglooEntity.Data.activeLayout;
		}
	}
}
