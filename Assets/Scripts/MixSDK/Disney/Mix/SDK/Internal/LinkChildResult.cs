namespace Disney.Mix.SDK.Internal
{
	public class LinkChildResult : ILinkChildResult
	{
		public bool Success
		{
			get;
			private set;
		}

		public LinkChildResult(bool success)
		{
			Success = success;
		}
	}
}
