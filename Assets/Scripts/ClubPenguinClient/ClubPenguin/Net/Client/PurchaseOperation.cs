using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;

namespace ClubPenguin.Net.Client
{
	[HttpPath("cp-api-base-uri", "/iap/v1/purchase")]
	[HttpPOST]
	[HttpContentType("application/json")]
	[HttpAccept("application/json")]
	[HttpTimeout(65f)]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	public class PurchaseOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public PurchaseRequest PurchaseRequest;

		[HttpResponseJsonBody]
		public PurchaseResponse PurchaseResponse;

		public PurchaseOperation(PurchaseRequest purchaseRequest)
		{
			PurchaseRequest = purchaseRequest;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			throw new NotImplementedException();
		}
	}
}
