namespace Disney.Mix.SDK.Internal
{
	public class InvalidLastNameError : AbstractInvalidProfileItemError, IInvalidLastNameError, IInvalidProfileItemError
	{
		public InvalidLastNameError(string description)
			: base(description)
		{
		}
	}
}
