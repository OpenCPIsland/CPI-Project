namespace Disney.Mix.SDK.Internal
{
	public class PasswordSizeError : AbstractInvalidProfileItemError, IPasswordSizeError, IInvalidProfileItemError
	{
		public PasswordSizeError(string description)
			: base(description)
		{
		}
	}
}
