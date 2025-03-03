namespace Disney.Mix.SDK.Internal
{
	public class LinkChildFailedNotAdultResult : ILinkChildFailedNotAdultResult, ILinkChildResult
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
