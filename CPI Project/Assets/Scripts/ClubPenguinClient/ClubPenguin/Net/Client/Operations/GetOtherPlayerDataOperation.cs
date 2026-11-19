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
			OtherPlayerData data = null;
			if (this is GetOtherPlayerDataBySwidOperation)
			{
				data = LanDirectory.BySwid(((GetOtherPlayerDataBySwidOperation)this).Swid, offlineDatabase.AccessToken);
			}
			else if (this is GetOtherPlayerDataByDisplayNameOperation)
			{
				data = LanDirectory.ByName(((GetOtherPlayerDataByDisplayNameOperation)this).DisplayName, offlineDatabase.AccessToken);
			}
			else if (this is GetOtherPlayerDataBySessionIdOperation && LanDirectory.Active)
			{
				data = LanDirectory.BySession(((GetOtherPlayerDataBySessionIdOperation)this).SessionId);
			}
			if (data != null)
			{
				ResponseBody = data;
				return;
			}
			ResponseBody = new OtherPlayerData();
			ResponseBody.mascotXP = new Dictionary<string, long>();
			ResponseBody.outfit = new List<CustomEquipment>();
			ResponseBody.name = "";
			ResponseBody.id = new PlayerId
			{
				id = "",
				type = PlayerId.PlayerIdType.NAME
			};
			ResponseBody.zoneId = null;
			ResponseBody.member = true;
		}
	}
}
