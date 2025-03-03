using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;

namespace ClubPenguin.Net.Client
{
	[HttpContentType("application/json")]
	[HttpAccept("application/json")]
	[HttpPath("cp-api-base-uri", "/catalog/v1/clothing/submit")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPOST]
	public class ItemSubmissionOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public ItemSubmissionRequest SubmissionRequest;

		[HttpResponseJsonBody]
		public ItemSubmissionResponse Response;

		public ItemSubmissionOperation(long scheduledThemeChallengeId, CustomEquipment equipment)
		{
			SubmissionRequest = default(ItemSubmissionRequest);
			SubmissionRequest.scheduledThemeChallengeId = scheduledThemeChallengeId;
			SubmissionRequest.equipment = default(CatalogSubmissionEquipment);
			SubmissionRequest.equipment.definitionId = equipment.definitionId;
			SubmissionRequest.equipment.parts = equipment.parts;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			throw new NotImplementedException();
		}
	}
}
