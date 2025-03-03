using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client.Operations
{
	[HttpContentType("application/json")]
	[HttpPOST]
	[HttpPath("cp-api-base-uri", "/encryption-trusted/v1/encryptionKey")]
	[HttpAccept("application/json")]
	public class GetEncryptionKeyOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public PublicKeyData RequestBody;

		[HttpResponseJsonBody]
		public EncryptionKey ResponseBody;

		public GetEncryptionKeyOperation(string publicKeyExponent, string publicKeyModulus)
		{
			RequestBody = new PublicKeyData
			{
				exponent = publicKeyExponent,
				modulus = publicKeyModulus
			};
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
		}
	}
}
