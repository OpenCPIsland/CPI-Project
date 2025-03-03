namespace Disney.Mix.SDK.Internal
{
	public class InvalidPrivacyPolicyError : AbstractInvalidProfileItemError, IInvalidPrivacyPolicyError, IInvalidProfileItemError
	{
		public InvalidPrivacyPolicyError(string description)
			: base(description)
		{
		}
	}
}
