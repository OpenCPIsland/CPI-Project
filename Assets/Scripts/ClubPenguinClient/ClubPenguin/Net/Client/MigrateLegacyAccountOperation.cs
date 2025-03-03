using ClubPenguin.Net.Client.Attributes;
using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;

namespace ClubPenguin.Net.Client
{
	[HttpAccept("application/json")]
	[Encrypted]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPOST]
	[HttpPath("cp-api-base-uri", "/player/v1/legacy/migrate")]
	[HttpContentType("application/json")]
	public class MigrateLegacyAccountOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public CPIDCredentials RequestBody;

		[HttpResponseJsonBody]
		public MigrationData ResponseBody;

		public MigrateLegacyAccountOperation(CPIDCredentials cpidCreds)
		{
			RequestBody = cpidCreds;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			throw new NotImplementedException();
		}
	}
}
