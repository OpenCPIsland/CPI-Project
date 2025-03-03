using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;

namespace ClubPenguin.Net.Client
{
	[HttpAccept("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPOST]
	[HttpContentType("application/json")]
	[HttpPath("cp-api-base-uri", "/catalog/v1/clothing/purchase")]
	public class ItemPurchaseOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public ItemPurchaseRequest PurchaseRequest;

		[HttpResponseJsonBody]
		public ItemPurchaseResponse Response;

		public ItemPurchaseOperation(long clothingCatalogItemId)
		{
			PurchaseRequest = default(ItemPurchaseRequest);
			PurchaseRequest.clothingCatalogItemId = clothingCatalogItemId;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			throw new NotImplementedException();
		}
	}
}
