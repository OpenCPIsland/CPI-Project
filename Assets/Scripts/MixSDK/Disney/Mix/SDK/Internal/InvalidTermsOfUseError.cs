namespace Disney.Mix.SDK.Internal
{
	public class InvalidTermsOfUseError : AbstractInvalidProfileItemError, IInvalidTermsOfUseError, IInvalidProfileItemError
	{
		public InvalidTermsOfUseError(string description)
			: base(description)
		{
		}
	}
}
