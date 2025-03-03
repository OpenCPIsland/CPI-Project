namespace Disney.Mix.SDK.Internal
{
	public class PasswordLikePhoneNumberError : AbstractInvalidProfileItemError, IPasswordLikePhoneNumberError, IInvalidProfileItemError
	{
		public PasswordLikePhoneNumberError(string description)
			: base(description)
		{
		}
	}
}
