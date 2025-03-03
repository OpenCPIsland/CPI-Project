using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;

namespace ClubPenguin.Net.Client
{
	[HttpPOST]
	[HttpPath("cp-api-base-uri", "/membership/v1/qa/membershipgrant")]
	[HttpAccept("application/json")]
	[HttpContentType("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	public class QAMembershipGrantOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public MembershipGrantRequest MembershipGrantRequest;

		[HttpResponseJsonBody]
		public PurchaseResponse PurchaseResponse;

		public QAMembershipGrantOperation(MembershipGrantRequest membershipGrantRequest)
		{
			MembershipGrantRequest = membershipGrantRequest;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			throw new NotImplementedException();
		}
	}
}
