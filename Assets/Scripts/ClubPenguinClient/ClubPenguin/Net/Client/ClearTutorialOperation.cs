using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	[HttpDELETE]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPath("cp-api-base-uri", "/tutorial/v1/qa/cleartutorial")]
	[HttpAccept("application/json")]
	public class ClearTutorialOperation : CPAPIHttpOperation
	{
		[HttpResponseJsonBody]
		public TutorialResponse TutorialResponse;

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			TutorialData value = offlineDatabase.Read<TutorialData>();
			value.Init();
			offlineDatabase.Write(value);
			TutorialResponse = new TutorialResponse
			{
				tutorialBytes = new List<sbyte>(value.Bytes)
			};
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			TutorialData value = offlineDatabase.Read<TutorialData>();
			value.Init();
			offlineDatabase.Write(value);
		}
	}
}
