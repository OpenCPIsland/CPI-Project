using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;

namespace ClubPenguin.Net.Client
{
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPath("cp-api-base-uri", "/inventory/v1/outfit/{$outfitSlot}")]
	[HttpAccept("application/json")]
	[HttpDELETE]
	public class DeleteSavedOutfitOperation : CPAPIHttpOperation
	{
		[HttpUriSegment("outfitSlot")]
		public int OutfitSlot;

		[HttpResponseJsonBody]
		public SavedOutfitResponse ResponseBody;

		public DeleteSavedOutfitOperation(int outfitSlot)
		{
			OutfitSlot = outfitSlot;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			throw new NotImplementedException();
		}
	}
}
