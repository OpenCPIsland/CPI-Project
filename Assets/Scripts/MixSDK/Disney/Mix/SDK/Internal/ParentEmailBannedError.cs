namespace Disney.Mix.SDK.Internal
{
	public class ParentEmailBannedError : AbstractInvalidProfileItemError, IParentEmailBannedError, IInvalidProfileItemError
	{
		public ParentEmailBannedError(string description)
			: base(description)
		{
		}
	}
}
