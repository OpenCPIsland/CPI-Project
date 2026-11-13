using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	[HttpPath("cp-api-base-uri", "/inventory/v1/outfits")]
	[HttpAccept("application/json")]
	[HttpGET]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	public class GetSavedOutfitsOperation : CPAPIHttpOperation
	{
		[HttpResponseJsonBody]
		public List<SavedOutfit> ResponseBody;

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			throw new NotImplementedException();
		}
	}
}
