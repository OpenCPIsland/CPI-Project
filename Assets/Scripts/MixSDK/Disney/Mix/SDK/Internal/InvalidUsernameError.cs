namespace Disney.Mix.SDK.Internal
{
	public class InvalidUsernameError : AbstractInvalidProfileItemError, IInvalidUsernameError, IInvalidProfileItemError
	{
		public InvalidUsernameError(string description)
			: base(description)
		{
		}
	}
}
