namespace Disney.Mix.SDK.Internal.MixDomain
{
	public class StartUserSessionResponse : BaseResponse
	{
		public string EncryptedSymmetricKey;

		public long? SessionId;

		public string HashedUserId;
	}
}
