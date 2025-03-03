namespace Disney.Mix.SDK.Internal
{
	public class LinkChildFailedNotChildResult : ILinkChildFailedNotChildResult, ILinkChildResult
	{
		public bool Success
		{
			get
			{
				return false;
			}
		}
	}
}
