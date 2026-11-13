namespace Disney.Mix.SDK.Internal
{
	public class InvalidEmailError : AbstractInvalidProfileItemError, IInvalidEmailError, IInvalidProfileItemError
	{
		public InvalidEmailError(string description)
			: base(description)
		{
		}
	}
}
