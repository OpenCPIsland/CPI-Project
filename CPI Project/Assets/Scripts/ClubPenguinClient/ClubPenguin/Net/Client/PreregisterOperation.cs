using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;

namespace ClubPenguin.Net.Client
{
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPOST]
	[HttpPath("cp-api-base-uri", "/player/v1/preregistration")]
	[HttpAccept("application/json")]
	public class PreregisterOperation : CPAPIHttpOperation
	{
		[HttpQueryString("clientId")]
		public string ClientID;

		[HttpResponseJsonBody]
		public MigrationData ResponseBody;

		public PreregisterOperation(string clientId)
		{
			ClientID = clientId;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			throw new NotImplementedException();
		}
	}
}
