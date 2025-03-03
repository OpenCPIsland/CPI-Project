using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;

namespace ClubPenguin.Net.Client
{
	[HttpPath("cp-api-base-uri", "/task/v1")]
	[RequestQueue("Task")]
	[HttpContentType("application/json")]
	[HttpAccept("application/json")]
	[HttpPOST]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	public class SetTaskProgressOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public SignedResponse<TaskProgress> RequestBody;

		public SetTaskProgressOperation(SignedResponse<TaskProgress> task)
		{
			RequestBody = task;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			throw new NotImplementedException();
		}
	}
}
