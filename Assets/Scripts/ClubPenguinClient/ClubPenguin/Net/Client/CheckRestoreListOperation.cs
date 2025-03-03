using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	[HttpTimeout(65f)]
	[HttpPOST]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPath("cp-api-base-uri", "/iap/v1/checkrestore")]
	[HttpContentType("application/json")]
	[HttpAccept("application/json")]
	public class CheckRestoreListOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public List<PurchaseRequest> PurchaseRequestList;

		[HttpResponseJsonBody]
		public PurchaseResponse PurchaseResponse;

		public CheckRestoreListOperation(List<PurchaseRequest> purchaseRequestList)
		{
			PurchaseRequestList = purchaseRequestList;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			throw new NotImplementedException();
		}
	}
}
