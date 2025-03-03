namespace Disney.Mix.SDK.Internal
{
	public abstract class AbstractInvalidProfileItemError : IInvalidProfileItemError
	{
		public string Description
		{
			get;
			private set;
		}

		protected AbstractInvalidProfileItemError(string description)
		{
			Description = description;
		}
	}
}
