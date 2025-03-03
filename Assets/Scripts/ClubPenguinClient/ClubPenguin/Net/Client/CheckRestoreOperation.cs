using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;

namespace ClubPenguin.Net.Client
{
	[HttpPOST]
	[HttpTimeout(65f)]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPath("cp-api-base-uri", "/iap/v1/checkrestore")]
	[HttpContentType("application/json")]
	[HttpAccept("application/json")]
	public class CheckRestoreOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public PurchaseRequest PurchaseRequest;

		[HttpResponseJsonBody]
		public PurchaseResponse PurchaseResponse;

		public CheckRestoreOperation(PurchaseRequest purchaseRequest)
		{
			PurchaseRequest = purchaseRequest;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			throw new NotImplementedException();
		}
	}
}
