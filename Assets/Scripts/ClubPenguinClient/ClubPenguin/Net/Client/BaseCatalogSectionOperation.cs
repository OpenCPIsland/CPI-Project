using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;

namespace ClubPenguin.Net.Client
{
	[HttpContentType("application/json")]
	[HttpPath("cp-api-base-uri", "/catalog/v1/clothing/items/{$catalogSection}")]
	[HttpAccept("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPOST]
	public class BaseCatalogSectionOperation<T> : CPAPIHttpOperation
	{
		[HttpUriSegment("catalogSection")]
		public string CatalogSection;

		[HttpRequestJsonBody]
		public T SectionRequest;

		[HttpResponseJsonBody]
		public CatalogSectionResponse Response;

		public BaseCatalogSectionOperation(T sectionRequest)
		{
			SectionRequest = sectionRequest;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			throw new NotImplementedException();
		}
	}
}
