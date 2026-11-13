namespace Disney.Mix.SDK.Internal
{
	public class PasswordTooCommonError : AbstractInvalidProfileItemError, IPasswordTooCommonError, IInvalidProfileItemError
	{
		public PasswordTooCommonError(string description)
			: base(description)
		{
		}
	}
}
