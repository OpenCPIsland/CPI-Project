namespace Disney.Mix.SDK.Internal
{
	public class InvalidParentEmailError : AbstractInvalidProfileItemError, IInvalidParentEmailError, IInvalidProfileItemError
	{
		public InvalidParentEmailError(string description)
			: base(description)
		{
		}
	}
}
