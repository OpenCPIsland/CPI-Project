using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;

namespace ClubPenguin.Net.Client
{
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpAccept("application/json")]
	[RequestQueue("Task")]
	[HttpPOST]
	[HttpPath("cp-api-base-uri", "/task/v1/qa")]
	[HttpContentType("application/json")]
	public class QASetTaskProgressOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public TaskProgress RequestBody;

		[HttpResponseJsonBody]
		public SignedResponse<TaskProgressList> ResponseBody;

		public QASetTaskProgressOperation(TaskProgress task)
		{
			RequestBody = task;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			throw new NotImplementedException();
		}
	}
}
