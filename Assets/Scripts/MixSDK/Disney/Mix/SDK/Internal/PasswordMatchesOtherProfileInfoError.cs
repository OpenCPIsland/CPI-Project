namespace Disney.Mix.SDK.Internal
{
	public class PasswordMatchesOtherProfileInfoError : AbstractInvalidProfileItemError, IPasswordMatchesOtherProfileInfoError, IInvalidProfileItemError
	{
		public PasswordMatchesOtherProfileInfoError(string description)
			: base(description)
		{
		}
	}
}
