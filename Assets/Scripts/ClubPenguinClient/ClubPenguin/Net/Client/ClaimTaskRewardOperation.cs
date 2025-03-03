using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;

namespace ClubPenguin.Net.Client
{
	[HttpAccept("application/json")]
	[HttpPOST]
	[HttpContentType("text/plain")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[RequestQueue("Task")]
	[HttpPath("cp-api-base-uri", "/task/v1/reward")]
	public class ClaimTaskRewardOperation : CPAPIHttpOperation
	{
		[HttpRequestTextBody]
		public string RequestBody;

		[HttpResponseJsonBody]
		public ClaimTaskRewardResponse ResponseBody;

		public ClaimTaskRewardOperation(string taskId)
		{
			RequestBody = taskId;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			throw new NotImplementedException();
		}
	}
}
