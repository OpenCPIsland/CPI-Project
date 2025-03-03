using ClubPenguin.Net.Client.Operations;

namespace ClubPenguin.Net.Client
{
	public class EncryptionApi
	{
		private ClubPenguinClient clubPenguinClient;

		public EncryptionApi(ClubPenguinClient clubPenguinClient)
		{
			this.clubPenguinClient = clubPenguinClient;
		}

		public APICall<GetEncryptionKeyOperation> GetEncryptionKey(string publicKeyExponent, string publicKeyModulus)
		{
			GetEncryptionKeyOperation operation = new GetEncryptionKeyOperation(publicKeyExponent, publicKeyModulus);
			return new APICall<GetEncryptionKeyOperation>(clubPenguinClient, operation);
		}
	}
}
