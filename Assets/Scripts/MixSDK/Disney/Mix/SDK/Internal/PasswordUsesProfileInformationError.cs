namespace Disney.Mix.SDK.Internal
{
	public class PasswordUsesProfileInformationError : AbstractInvalidProfileItemError, IPasswordUsesProfileInformationError, IInvalidProfileItemError
	{
		public PasswordUsesProfileInformationError(string description)
			: base(description)
		{
		}
	}
}
