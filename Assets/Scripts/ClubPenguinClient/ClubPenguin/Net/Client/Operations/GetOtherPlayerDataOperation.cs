using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client.Operations
{
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpGET]
	[HttpAccept("application/json")]
	public abstract class GetOtherPlayerDataOperation : CPAPIHttpOperation
	{
		[HttpResponseJsonBody]
		public OtherPlayerData ResponseBody;

		public GetOtherPlayerDataOperation()
		{
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ResponseBody = new OtherPlayerData();
			ResponseBody.mascotXP = new Dictionary<string, long>();
			ResponseBody.outfit = new List<CustomEquipment>();
			ResponseBody.name = "";
			ResponseBody.id = new PlayerId
			{
				id = offlineDatabase.AccessToken,
				type = PlayerId.PlayerIdType.SWID
			};
			ResponseBody.zoneId = null;
		}
	}
}
