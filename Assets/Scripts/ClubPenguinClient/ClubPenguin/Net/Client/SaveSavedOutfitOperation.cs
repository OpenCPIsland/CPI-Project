using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;

namespace ClubPenguin.Net.Client
{
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPath("cp-api-base-uri", "/inventory/v1/outfit")]
	[HttpContentType("application/json")]
	[HttpAccept("application/json")]
	[HttpPOST]
	public class SaveSavedOutfitOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public SavedOutfit RequestBody;

		[HttpResponseJsonBody]
		public SavedOutfitResponse ResponseBody;

		public SaveSavedOutfitOperation(SavedOutfit savedOutfit)
		{
			RequestBody = savedOutfit;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			throw new NotImplementedException();
		}
	}
}
