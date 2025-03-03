namespace Disney.Mix.SDK.Internal.MixDomain
{
	public class StartUserSessionRequest : BaseUserRequest
	{
		public string PublicKeyModulus;

		public string PublicKeyExponent;

		public long? SessionGroupId;

		public int? ProtocolVersion;
	}
}
