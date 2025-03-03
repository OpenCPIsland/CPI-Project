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
	[HttpPath("cp-api-base-uri", "/iap-pc/v1/products")]
	public class PCGetProductDetailsOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public PCGetProductDetailsRequest PCGetProductDetailsRequest;

		[HttpResponseJsonBody]
		public PCGetProductDetailsResponse PCGetProductDetailsResponse;

		public PCGetProductDetailsOperation(PCGetProductDetailsRequest getProductDetailsRequest)
		{
			PCGetProductDetailsRequest = getProductDetailsRequest;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			throw new NotImplementedException();
		}
	}
}
