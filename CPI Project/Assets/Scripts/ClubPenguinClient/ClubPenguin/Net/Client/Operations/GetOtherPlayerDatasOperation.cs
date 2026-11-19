using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client.Operations
{
	[HttpPOST]
	[HttpAccept("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	public abstract class GetOtherPlayerDatasOperation : CPAPIHttpOperation
	{
		[HttpResponseJsonBody]
		public List<OtherPlayerData> ResponseBody;

		public GetOtherPlayerDatasOperation()
		{
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			if (LanDirectory.Active)
			{
				if (this is GetOtherPlayerDataBySwidsOperation)
				{
					ResponseBody = LanDirectory.BySwids(((GetOtherPlayerDataBySwidsOperation)this).RequestBody);
					return;
				}
				if (this is GetOtherPlayerDataByDisplayNamesOperation)
				{
					ResponseBody = LanDirectory.ByNames(((GetOtherPlayerDataByDisplayNamesOperation)this).RequestBody);
					return;
				}
			}
			ResponseBody = new List<OtherPlayerData>();
		}
	}
}
