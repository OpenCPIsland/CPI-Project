using ClubPenguin.Net.Domain.Diagnostics;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	[HttpPOST]
	[HttpPath("cp-monitoring-base-uri", "/diagnostics/v1/log")]
	[HttpContentType("application/json")]
	public class PostDiagnosticsLogOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public List<LogParameters> RequestBody;

		public PostDiagnosticsLogOperation(List<LogParameters> logParametersList)
		{
			RequestBody = logParametersList;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
		}
	}
}
